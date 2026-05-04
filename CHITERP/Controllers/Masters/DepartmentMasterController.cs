using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using CHITERP.Data;
using CHITERP.Helper;
using CHITERP.Models;
using System.Configuration;

namespace CHITERP.Controllers.Masters
{
    public class DepartmentMasterController : Controller
    {
        // GET: DepartmentMaster

        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        [Authorize(Roles = "DepartmentMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            return View();
        }
        #endregion

        #region GetAjaxDetailsData
        public JsonResult GetAjaxDetailsData(JQueryDataTableParamModel param)
        {
            using (var e = new FinFusionEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Masters_Department(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);
                var aaData = data.Select(d => new
                {
                    DEPTCODE = d.DEPTCODE,
                    DEPTDESC = d.DEPTDESC,
                    DISPSTATUS = d.DISPSTATUS,
                    DEPTID = d.DEPTID.ToString()
                }).ToArray();

                return Json(new
                {
                    sEcho = param.sEcho,
                    data = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Edit
        [Authorize(Roles = "DepartmentMasterEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/DepartmentMaster/Form/" + id);

                //Response.Redirect("/DepartmentMaster/Form/?id=" + id);
            }
        }
        #endregion

        #region Form
        [Authorize(Roles = "DepartmentMasterCreate")]
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            DepartmentMaster tab = new DepartmentMaster();

            int DEPTID = 0;

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            if (id != "0")
            {
                DEPTID = Convert.ToInt32(id);

                tab = context.departmentmasters.Find(DEPTID);

                selectedDISPSTATUS = new List<SelectListItem>();
                if (tab.DISPSTATUS == 0)
                {
                    selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
                    selectedDISPSTATUS.Add(selectedItem);
                    selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
                    selectedDISPSTATUS.Add(selectedItem);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS;
                }
                else
                {
                    selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS.Add(selectedItem);
                    selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS.Add(selectedItem);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS;
                }
            }

            return View(tab);
        }
        #endregion

        #region Save Department Master
        [HttpPost]
        public ActionResult SaveData(FormCollection Fdata)
        {
            using (context = new AppDbContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    string status = "";
                    try
                    {
                        DepartmentMaster depmaster = new DepartmentMaster();

                        Int32 DEPTID = 0;

                        string cmid = Convert.ToString(Fdata["DEPTID"]);
                        if (cmid == "" || cmid == null)
                        { DEPTID = 0; }
                        else
                        { DEPTID = Convert.ToInt32(cmid); }

                        if (DEPTID != 0)
                        {
                            depmaster = context.departmentmasters.Find(DEPTID);
                        }

                        depmaster.DEPTDESC = Convert.ToString(Fdata["DEPTDESC"]);
                        depmaster.DEPTCODE = Convert.ToString(Fdata["DEPTCODE"]);
                        depmaster.DISPSTATUS = Convert.ToInt16(Fdata["DISPSTATUS"]);

                        if (Session["CUSRID"] != null)
                        { depmaster.CUSRID = Session["CUSRID"].ToString(); }
                        else { depmaster.CUSRID = ""; }

                        if (DEPTID == 0)
                        {

                            depmaster.PRCSDATE = DateTime.Now;

                            string squery = "SELECT *FROM DEPARTMENTMASTER WHERE  DEPTCODE = '" + depmaster.DEPTCODE + "' AND DEPTDESC = '" + depmaster.DEPTDESC + "'";
                            var query = context.Database.SqlQuery<DepartmentMaster>(squery).ToList();

                            if (query.Count != 0)
                            {
                                status = "Existing";
                            }
                            else
                            {
                                depmaster.LMUSRID = "";
                                context.departmentmasters.Add(depmaster);
                                context.SaveChanges();
                                status = "Success";
                            }
                        }
                        else
                        {
                            if (Session["CUSRID"] != null)
                            { depmaster.LMUSRID = Session["CUSRID"].ToString(); }
                            else { depmaster.LMUSRID = ""; }
                           
                            depmaster.PRCSDATE = DateTime.Now;

                            context.Entry(depmaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            status = "Success";
                        }

                        trans.Commit();
                        return Json(status, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        var ermsg = ex.Message;
                        //Response.Write(ex.Message.ToString());
                        trans.Rollback();
                        //Response.Write("Error");
                        status = "Error";
                        //return RedirectToAction("Index", "DepartmentMaster");
                        return Json(status, JsonRequestBehavior.AllowGet);
                        //Response.Redirect("/Error/AccessDenied");
                    }

                }
            }

        }
        #endregion

        #region Delete Department Master 
        [Authorize(Roles = "DepartmentMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check(fld, id, "DepartmentMaster");

            //String temp = "PROCEED";

            if (temp.Equals("PROCEED"))
            {
                DepartmentMaster desmaster = context.departmentmasters.Find(Convert.ToInt32(id));
                context.departmentmasters.Remove(desmaster);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion 
    }
}
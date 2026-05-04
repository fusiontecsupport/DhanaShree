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
    public class StateMasterController : Controller
    {
        // GET: StateMaster

        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        [Authorize(Roles = "StateMasterIndex")]
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

                var data = e.pr_Search_Masters_State(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);
                var aaData = data.Select(d => new
                {
                    STATECODE = d.STATECODE,
                    STATEDESC = d.STATEDESC,
                    STATETYPE = d.STATETYPE,
                    DISPSTATUS = d.DISPSTATUS,
                    STATEID = d.STATEID.ToString()
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
        [Authorize(Roles = "StateMasterEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/StateMaster/Form/" + id);

                //Response.Redirect("/StateMaster/Form/?id=" + id);
            }
        }
        #endregion

        #region Form
        [Authorize(Roles = "StateMasterCreate")]
        public ActionResult Form(string id = "0")
        {
            //if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            StateMaster tab = new StateMaster();

            int STATEID = 0;

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            List<SelectListItem> selectedSTATETYPE = new List<SelectListItem>();
            SelectListItem selectedSType = new SelectListItem { Text = "Local", Value = "0", Selected = true };
            selectedSTATETYPE.Add(selectedSType);
            selectedSType = new SelectListItem { Text = "InterState", Value = "1", Selected = false };
            selectedSTATETYPE.Add(selectedSType);
            ViewBag.STATETYPE = selectedSTATETYPE;

            if (id != "0")
            {
                STATEID = Convert.ToInt32(id);

                tab = context.statemasters.Find(STATEID);

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

                selectedSTATETYPE = new List<SelectListItem>();
                if (tab.STATETYPE == 0)
                {
                    selectedSType = new SelectListItem { Text = "Local", Value = "0", Selected = true };
                    selectedSTATETYPE.Add(selectedSType);
                    selectedSType = new SelectListItem { Text = "InterState", Value = "1", Selected = false };
                    selectedSTATETYPE.Add(selectedSType);
                    ViewBag.STATETYPE = selectedSTATETYPE;
                }
                else
                {
                    selectedSType = new SelectListItem { Text = "Local", Value = "0", Selected = false };
                    selectedSTATETYPE.Add(selectedSType);
                    selectedSType = new SelectListItem { Text = "InterState", Value = "1", Selected = true };
                    selectedSTATETYPE.Add(selectedSType);
                    ViewBag.STATETYPE = selectedSTATETYPE;
                }
            }

            return View(tab);
        }
        #endregion

        #region Save State Master        
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
                        StateMaster smaster = new StateMaster();

                        Int32 STATEID = 0;

                        string cmid = Convert.ToString(Fdata["STATEID"]);
                        if (cmid == "" || cmid == null)
                        { STATEID = 0; }
                        else
                        { STATEID = Convert.ToInt32(cmid); }

                        if (STATEID != 0)
                        {
                            smaster = context.statemasters.Find(STATEID);
                        }

                        smaster.STATECODE = Convert.ToString(Fdata["STATECODE"]);
                        smaster.STATEDESC = Convert.ToString(Fdata["STATEDESC"]);
                        smaster.STATETYPE = Convert.ToInt16(Fdata["STATETYPE"]);                       
                        smaster.DISPSTATUS = Convert.ToInt16(Fdata["DISPSTATUS"]);
                        //smaster.CUSRID = "Admin";

                        if (Session["CUSRID"] != null)
                            smaster.CUSRID = Session["CUSRID"].ToString();
                        else
                            smaster.CUSRID = "";

                        if (STATEID == 0)
                        {

                            smaster.PRCSDATE = DateTime.Now;

                            string squery = "SELECT *FROM STATEMASTER WHERE STATEDESC='" + smaster.STATEDESC + "' AND STATECODE='" + smaster.STATECODE + "'";
                            var query = context.Database.SqlQuery<StateMaster>(squery).ToList();// context.crmsalestaskdetails.SqlQuery(squery).ToList<CRMSalesTaskDetail>();

                            if (query.Count != 0)
                            {
                                status = "Existing";
                            }
                            else
                            {
                                context.statemasters.Add(smaster);
                                context.SaveChanges();
                                status = "Success";
                            }
                        }
                        else
                        {

                            if (Session["CUSRID"] != null)
                                smaster.LMUSRID = Session["CUSRID"].ToString();
                            else
                                smaster.LMUSRID = "";
                            
                            smaster.PRCSDATE = DateTime.Now;

                            context.Entry(smaster).State = System.Data.Entity.EntityState.Modified;
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
                        //return RedirectToAction("Index", "StateMaster");
                        return Json(status, JsonRequestBehavior.AllowGet);
                        //Response.Redirect("/Error/AccessDenied");
                    }

                }
            }
        }
        #endregion

        #region Delete State Master 
        [Authorize(Roles = "StateMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check(fld, id, "StateMaster");

            //String temp = "PROCEED";

            if (temp.Equals("PROCEED"))
            {
                StateMaster smaster = context.statemasters.Find(Convert.ToInt32(id));
                context.statemasters.Remove(smaster);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion 
    }
}
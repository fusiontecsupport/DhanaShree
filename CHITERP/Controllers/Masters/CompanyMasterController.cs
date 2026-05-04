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
    public class CompanyMasterController : Controller
    {
        // GET: CompanyMaster

        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        [Authorize(Roles = "CompanyMasterIndex")]
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

                var data = e.pr_Search_Masters_CompanyDetails(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);
                var aaData = data.Select(d => new
                {
                    COMPCODE = d.COMPCODE,
                    COMPNAME = d.COMPNAME,
                    COMPDNAME = d.COMPDNAME,
                    DISPSTATUS = d.DISPSTATUS,
                    COMPID = d.COMPID.ToString()
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
        [Authorize(Roles = "CompanyMasterEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/CompanyMaster/Form/" + id);

               // Response.Redirect("/CompanyMaster/Form/?id=" + id);
            }
        }
        #endregion

        #region Form
        [Authorize(Roles = "CompanyMasterCreate")]
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            CompanyMaster tab = new CompanyMaster();

            int COMPID = 0;

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            if (id != "0")
            {
                COMPID = Convert.ToInt32(id);
                tab = context.companymasters.Find(COMPID);

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

        #region Save Company Master        
        [HttpPost]
        public ActionResult SaveData(FormCollection Fdata)
        {
            using (context = new AppDbContext())
            {
                using (var trans1 = context.Database.BeginTransaction())
                {
                    string status = "";
                    try
                    {
                        CompanyMaster cmaster = new CompanyMaster();
                        

                        Int32 COMPID = 0;

                        string cmid = Convert.ToString(Fdata["COMPID"]);
                        if (cmid == "" || cmid == null)
                        {
                            COMPID = 0;
                        }
                        else
                        {
                            COMPID = Convert.ToInt32(cmid);
                        }

                        if (COMPID != 0)
                        {
                            cmaster = context.companymasters.Find(COMPID);
                        }
                        cmaster.COMPID = COMPID;
                        cmaster.COMPCODE = Convert.ToString(Fdata["COMPCODE"]);
                        cmaster.COMPNAME = Convert.ToString(Fdata["COMPNAME"]);
                        cmaster.COMPDNAME = Convert.ToString(Fdata["COMPDNAME"]);
                        cmaster.COMPADDR = Convert.ToString(Fdata["COMPADDR"]);
                        cmaster.COMPPHN1 = Convert.ToString(Fdata["COMPPHN1"]);
                        cmaster.COMPPHN2 = Convert.ToString(Fdata["COMPPHN2"]);
                        cmaster.COMPPHN3 = Convert.ToString(Fdata["COMPPHN3"]);
                        cmaster.COMPPHN4 = Convert.ToString(Fdata["COMPPHN4"]);
                        cmaster.COMPMAIL = Convert.ToString(Fdata["COMPMAIL"]);
                        cmaster.COMPCPRSN = Convert.ToString(Fdata["COMPCPRSN"]);
                        cmaster.COMPPHNID = "0002";
                        string cmcofft = Convert.ToString(Fdata["COMPCOFFTIME"]);
                        if (cmcofft == "" || cmcofft == null)
                        {
                            cmaster.COMPCOFFTIME = null;
                        }
                        else
                        {
                            cmaster.COMPCOFFTIME = Convert.ToDateTime(cmcofft);
                        }
                        cmaster.COMPPDESC1 = Convert.ToString(Fdata["COMPPDESC1"]);
                        cmaster.COMPPDESC2 = Convert.ToString(Fdata["COMPPDESC1"]);
                        cmaster.COMPGSTNO = Convert.ToString(Fdata["COMPGSTNO"]);
                        cmaster.DISPSTATUS = Convert.ToInt16(Fdata["DISPSTATUS"]);

                        if (Session["CUSRID"] != null)
                        { cmaster.CUSRID = Session["CUSRID"].ToString(); }
                        else { cmaster.CUSRID = ""; }

                        if (COMPID == 0)
                        {
                           
                            cmaster.PRCSDATE = DateTime.Now;

                            string squery = "SELECT *FROM COMPANYMASTER WHERE COMPNAME = '" + cmaster.COMPNAME + "'";

                            var query = context.Database.SqlQuery<CompanyMaster>(squery).ToList();// context.crmsalestaskdetails.SqlQuery(squery).ToList<CRMSalesTaskDetail>();

                            if (query.Count != 0)
                            {                               
                                status = "Existing";
                            }
                            else
                            {
                                cmaster.LMUSRID = "";
                                context.companymasters.Add(cmaster);
                                context.SaveChanges();
                                status = "Success";
                            }                            
                        }
                        else
                        {
                            if (Session["CUSRID"] != null)
                            { cmaster.LMUSRID = Session["CUSRID"].ToString(); }
                            else { cmaster.LMUSRID = ""; }
                            
                            cmaster.PRCSDATE = DateTime.Now;

                            context.Entry(cmaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            status = "Success";
                        }

                        trans1.Commit();

                      
                        return Json(status, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        var ermsg = ex.Message;
                        //Response.Write(ex.Message.ToString());
                        trans1.Rollback();
                        //Response.Write("Error");
                        status = "Error";
                        //return RedirectToAction("Index", "ResourceMaster");
                        return Json(status, JsonRequestBehavior.AllowGet);
                        //Response.Redirect("/Error/AccessDenied");
                    }

                }
            }

        }
        #endregion

        #region Delete Company Master
        [Authorize(Roles = "CompanyMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check(fld, id, "COMPANYMASTER");

            //String temp = "PROCEED";

            if (temp.Equals("PROCEED"))
            {
                CompanyMaster commaster = context.companymasters.Find(Convert.ToInt32(id));
                context.companymasters.Remove(commaster);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion 
    }
}
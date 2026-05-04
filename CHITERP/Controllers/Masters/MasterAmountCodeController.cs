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
    public class MasterAmountCodeController : Controller
    {
        // GET: MasterAmountCode

        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        //[Authorize(Roles = "MasterAmountCodeIndex")]
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

                var data = e.pr_Search_Masters_AmountCode(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);
                var aaData = data.Select(d => new
                {
                    AmountCode = d.AmountCode,
                    AmountCodeName = d.AmountCodeName,
                    AmountCodeConstant = d.AmountCodeConstant.ToString(),
                    CustomerViewTypeID = d.CustomerViewTypeID,
                    StatusTypeID = d.StatusTypeID,
                    AmountCodeID = d.AmountCodeID.ToString()
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
        //[Authorize(Roles = "MasterAmountCodeEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/MasterAmountCode/Form/" + id);

                //Response.Redirect("/MasterAmountCode/Form/?id=" + id);
            }
        }
        #endregion

        #region Form
        //[Authorize(Roles = "MasterAmountCodeCreate")]
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            MasterAmountCode tab = new MasterAmountCode();

            int AmountCodeID = 0;

            //List<SelectListItem> CustomerViewType = new List<SelectListItem>();
            //SelectListItem selectedItem = new SelectListItem { Text = "Yes", Value = "0", Selected = true };
            //CustomerViewType.Add(selectedItem);
            //selectedItem = new SelectListItem { Text = "No", Value = "1", Selected = false };
            //CustomerViewType.Add(selectedItem);
            //ViewBag.CustomerViewTypeID = CustomerViewType;

            //List<SelectListItem> StatusType = new List<SelectListItem>();
            //SelectListItem selectedSType = new SelectListItem { Text = "Active", Value = "0", Selected = true };
            //StatusType.Add(selectedSType);
            //selectedSType = new SelectListItem { Text = "InActive", Value = "1", Selected = false };
            //StatusType.Add(selectedSType);
            //ViewBag.StatusTypeID = StatusType;
            
            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Status', ''").ToList();
            ViewBag.StatusTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();
            var mtqry1 = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Customer View', ''").ToList();
            ViewBag.CustomerViewTypeID = new SelectList(mtqry1, "TypeID", "Type").ToList();
            if (id != "0")
            {
                AmountCodeID = Convert.ToInt32(id);

                tab = context.masteramountcodes.Find(AmountCodeID);

                //CustomerViewType = new List<SelectListItem>();
                //if (tab.CustomerViewTypeID == 0)
                //{
                //    selectedItem = new SelectListItem { Text = "Yes", Value = "0", Selected = true };
                //    CustomerViewType.Add(selectedItem);
                //    selectedItem = new SelectListItem { Text = "No", Value = "1", Selected = false };
                //    CustomerViewType.Add(selectedItem);
                //    ViewBag.CustomerViewTypeID = CustomerViewType;
                //}
                //else
                //{
                //    selectedItem = new SelectListItem { Text = "Yes", Value = "0", Selected = false };
                //    CustomerViewType.Add(selectedItem);
                //    selectedItem = new SelectListItem { Text = "No", Value = "1", Selected = true };
                //    CustomerViewType.Add(selectedItem);
                //    ViewBag.CustomerViewTypeID = CustomerViewType;
                //}

                //StatusType = new List<SelectListItem>();
                //if (tab.StatusTypeID == 0)
                //{
                //    selectedSType = new SelectListItem { Text = "Active", Value = "0", Selected = true };
                //    StatusType.Add(selectedSType);
                //    selectedSType = new SelectListItem { Text = "InActive", Value = "1", Selected = false };
                //    StatusType.Add(selectedSType);
                //    ViewBag.StatusTypeID = StatusType;
                //}
                //else
                //{
                //    selectedSType = new SelectListItem { Text = "Active", Value = "0", Selected = false };
                //    StatusType.Add(selectedSType);
                //    selectedSType = new SelectListItem { Text = "InActive", Value = "1", Selected = true };
                //    StatusType.Add(selectedSType);
                //    ViewBag.StatusTypeID = StatusType;
                //}
                ViewBag.StatusTypeID = new SelectList(mtqry, "TypeID", "Type",tab.StatusTypeID).ToList();                
                ViewBag.CustomerViewTypeID = new SelectList(mtqry1, "TypeID", "Type",tab.CustomerViewTypeID).ToList();
            }

            return View(tab);
        }
        #endregion

        #region Save Amount Code Master        
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
                        //MasterAmountCode tab = new MasterAmountCode();

                        MasterAmountCode amontcode = new MasterAmountCode();

                        Int32 AmountCodeID = 0;

                        string cmid = Convert.ToString(Fdata["AmountCodeID"]);
                        if (cmid == "" || cmid == null)
                        { AmountCodeID = 0; }
                        else
                        { AmountCodeID = Convert.ToInt32(cmid); }

                        if (AmountCodeID != 0)
                        {
                            amontcode = context.masteramountcodes.Find(AmountCodeID);
                        }

                        amontcode.AmountCodeName = Convert.ToString(Fdata["AmountCodeName"]);
                        amontcode.AmountCode = Convert.ToString(Fdata["AmountCode"]);

                        string accons = Convert.ToString(Fdata["AmountCodeConstant"]);
                        if (accons == "" || accons == null)
                        { amontcode.AmountCodeConstant = 0; }
                        else
                        { amontcode.AmountCodeConstant = Convert.ToDouble(accons); }

                        //amontcode.AmountCodeConstant = Convert.ToDecimal(Fdata["AmountCodeConstant"]);
                        amontcode.CustomerViewTypeID = Convert.ToInt16(Fdata["CustomerViewTypeID"]);
                        amontcode.StatusTypeID = Convert.ToInt16(Fdata["StatusTypeID"]);
                        
                        //if (Session["CUSRID"] != null)
                        //    amontcode.CUSRID = Session["CUSRID"].ToString();
                        //else
                        //    amontcode.CUSRID = "";

                        if (AmountCodeID == 0)
                        {

                            //amontcode.PRCSDATE = DateTime.Now;

                            string squery = "SELECT *FROM MasterAmountCode WHERE AmountCode='" + amontcode.AmountCode + "' AND AmountCodeName='" + amontcode.AmountCodeName + "'";
                            var query = context.Database.SqlQuery<MasterAmountCode>(squery).ToList();// context.crmsalestaskdetails.SqlQuery(squery).ToList<CRMSalesTaskDetail>();

                            if (query.Count != 0)
                            {
                                status = "Existing";
                            }
                            else
                            {
                                context.masteramountcodes.Add(amontcode);
                                context.SaveChanges();
                                status = "Success";
                            }
                        }
                        else
                        {

                            //if (Session["CUSRID"] != null)
                            //    amontcode.LMUSRID = Session["CUSRID"].ToString();
                            //else
                            //    amontcode.LMUSRID = "";

                            //amontcode.PRCSDATE = DateTime.Now;

                            context.Entry(amontcode).State = System.Data.Entity.EntityState.Modified;
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

        #region Delete Amount Code Master 
        //[Authorize(Roles = "MasterAmountCodeDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check(fld, id, "MasterAmountCode");

            //String temp = "PROCEED";

            if (temp.Equals("PROCEED"))
            {
                MasterAmountCode smaster = context.masteramountcodes.Find(Convert.ToInt32(id));
                context.masteramountcodes.Remove(smaster);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion 

    }
}
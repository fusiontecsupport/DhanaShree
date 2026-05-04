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

namespace CHITERP.Controllers.Products
{
    public class LoanSchemeManagementController : Controller
    {
        // GET: LoanSchemeManagement

        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        //[Authorize(Roles = "LoanSchemeManagementIndex")]
        public ActionResult Index()
        {


            return View();
        }
        #endregion

        #region Edit
        //[Authorize(Roles = "LoanSchemeManagementEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/LoanSchemeManagement/Form/" + id);

                //Response.Redirect("/LoanSchemeManagement/Form/?id=" + id);
            }
        }
        #endregion

        #region GetAjaxData
        public ActionResult GetAjaxData(JQueryDataTableParamModel param)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            else
            {
                using (var e = new FinFusionEntities())
                {
                    var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                    var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
                    string crole = Convert.ToString(Session["Group"]);
                    string cusrid = Convert.ToString(Session["CUSRID"]);
                    if (crole == "SuperAdmin" || crole == "Admin")
                    {
                        cusrid = "";
                    }

                    var data = e.pr_Search_Master_LoanScheme(param.sSearch,
                                                    Convert.ToInt32(Request["iSortCol_0"]),
                                                    Request["sSortDir_0"],
                                                    param.iDisplayStart,
                                                    param.iDisplayStart + param.iDisplayLength,
                                                    totalRowsCount,
                                                    filteredRowsCount, Convert.ToString(cusrid));
                    var aaData = data.Select(d => new
                    {
                        LoanTypeDesc = d.LoanTypeDesc,
                        LoanSchemeCode = d.LoanSchemeCode,
                        LoanSchemeName = d.LoanSchemeName,
                        AmountCodeDesc = d.AmountCodeDesc,                        
                        BookVerificationIntervalTypeDesc = d.BookVerificationIntervalTypeDesc,
                        StatusTypeDesc = d.StatusTypeDesc,
                        LoanSchemeID = d.LoanSchemeID.ToString()
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
        }
        #endregion

        #region Form
        //[Authorize(Roles = "LoanSchemeManagementCreate")]
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            LoanSchemeList lsl = new LoanSchemeList();

            LoanScheme tab = new LoanScheme();

            int LoanSchemeID = 0;

            if (id == "" || id == null || id == "0")
            { LoanSchemeID = 0; }
            else { LoanSchemeID = Convert.ToInt32(id); }

            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Book Verification Interval', ''").ToList();
            ViewBag.BookVerificationIntervalTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();
            


            var amtcdqry = context.Database.SqlQuery<MasterAmountCode>("Select * From MasterAmountCode (nolock)").ToList();

            ViewBag.AmountCodeID = new SelectList(amtcdqry, "AmountCodeID", "AmountCodeName");

            ViewBag.LoanSecuredTypeId = new SelectList("");
            ViewBag.DisbursementModeTypeID = new SelectList("");
            ViewBag.VariableTypeId = new SelectList("");
            ViewBag.TermTypeID = new SelectList("");

            ViewBag.InterestDeductionTypeID = new SelectList("");
            ViewBag.LoanTransactionTypeID = new SelectList("");
            ViewBag.RepaymentIntervalTypeID = new SelectList("");

            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Loan Scheme', 'Status'").ToList();
            ViewBag.StatusTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();

            if (LoanSchemeID > 0)
            {
                tab = context.loanscheme.Find(LoanSchemeID);
                ViewBag.AmountCodeID = new SelectList(amtcdqry, "AmountCodeID", "AmountCodeName", tab.AmountCodeID);
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Book Verification Interval', ''").ToList();
                ViewBag.BookVerificationIntervalTypeID = new SelectList(mtqry, "TypeID", "Type",tab.BookVerificationIntervalTypeID).ToList();
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Loan Scheme', 'Status'").ToList();
                ViewBag.StatusTypeID = new SelectList(mtqry.Where(x => x.TypeID >= tab.StatusTypeID), "TypeID", "Type", tab.StatusTypeID).ToList();
                lsl.loanschemes = context.loanscheme.Where(x => x.LoanSchemeID == LoanSchemeID).ToList();
                lsl.loanvariablerangs = context.loanvariablerange.Where(x => x.LoanSchemeID == LoanSchemeID).ToList();
                lsl.loanformulas = context.loanformula.Where(x => x.LoanSchemeID == LoanSchemeID).ToList();
            }
            else
            {
                lsl.loanschemes = context.loanscheme.Where(x => x.LoanSchemeID == LoanSchemeID).ToList();
                lsl.loanvariablerangs = context.loanvariablerange.Where(x => x.LoanSchemeID == LoanSchemeID).ToList();
                lsl.loanformulas = context.loanformula.Where(x => x.LoanSchemeID == LoanSchemeID).ToList();
            }

            return View(lsl);
        }
        #endregion

        #region SaveData
        [HttpPost]
        public ActionResult SaveData(FormCollection formcoll)
        {
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0 || Session["CUSRID"].ToString() == "") { return RedirectToAction("Login", "Account"); }
            else
            {
                string status = "";
                using (context = new AppDbContext())
                {
                    using (var trans1 = context.Database.BeginTransaction())
                    {
                        try
                        {

                            LoanScheme master = new LoanScheme();

                            Int32 LoanSchemeID = 0;

                            string loanschemeid = Convert.ToString(formcoll["LoanSchemeID"]);
                            if (loanschemeid == "" || loanschemeid == null)
                            {
                                LoanSchemeID = 0;
                            }
                            else
                            {
                                LoanSchemeID = Convert.ToInt32(loanschemeid);
                            }

                            if (LoanSchemeID != 0)
                            {
                                master = context.loanscheme.Find(LoanSchemeID);
                            }
                            master.LoanSchemeID = LoanSchemeID;
                            master.AmountCodeID = 1;// Convert.ToInt32(formcoll["AmountCodeID"]);
                            master.EMIBased = false;
                            master.TenureBased = false;
                            if (formcoll["boolEMIBased"] != "1")
                                master.EMIBased = true;
                            if (formcoll["boolTenureBased"] == "1")                            
                                master.TenureBased = true;
                            master.SchemeName = Convert.ToString(formcoll["SchemeName"]);
                            master.SchemeCode = Convert.ToString(formcoll["SchemeCode"]);

                            
                            master.StatusTypeID = Convert.ToInt16(formcoll["StatusTypeID"]);
                            master.BookVerificationIntervalTypeID = Convert.ToInt16(formcoll["BookVerificationIntervalTypeID"]);


                            if (LoanSchemeID == 0)
                            {
                                string squery = "SELECT *FROM LoanScheme (nolock) WHERE SchemeCode ='" + master.SchemeCode + "' or SchemeName='" + master.SchemeName + "'";
                                var query = context.Database.SqlQuery<LoanScheme>(squery).ToList();

                                if (query.Count != 0)
                                {
                                    status = "Existing";
                                }
                                else
                                {
                                    //master.CreatedBy = Session["CUSRID"].ToString();
                                    //master.CreatedDt = DateTime.Now;

                                    context.loanscheme.Add(master);
                                    context.SaveChanges();
                                    status = "Success";

                                }

                            }
                            else
                            {
                                //master.UpdatedBy = Session["CUSRID"].ToString();
                                //master.UpdatedDt = DateTime.Now;

                                context.Entry(master).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                status = "Success";
                            }
                            trans1.Commit();
                            LoanSchemeID = master.LoanSchemeID;

                            
                            ViewBag.Message = "Form submitted.";
                            // Response.Redirect("Index");
                            //return RedirectToAction("Index", "LoanSchemeManagement");
                            //Response.Write("Update");
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            var ermsg = ex.Message;
                            //Response.Write(ex.Message.ToString());
                            trans1.Rollback();
                            //Response.Redirect("/Error/AccessDenied");
                            //return RedirectToAction("Index", "LoanSchemeManagement");
                            status = "Error";
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

        }
        #endregion

        #region Variable Range SaveLVData
        public ActionResult SaveLVData(FormCollection fdata)
        {
            string status = "";
            try
            {

            }
            catch (Exception ex)
            {

            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Formula SaveLFData
        public ActionResult SaveLFData(FormCollection fdata)
        {
            string status = "";
            try
            {

            }
            catch (Exception ex)
            {

            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Delete Record  
        //[Authorize(Roles = "LoanSchemeManagementDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check(fld, id, "LoanScheme");

            if (temp.Equals("PROCEED"))
            {
                LoanScheme loanscheme = context.loanscheme.Find(Convert.ToInt32(id));
                context.loanscheme.Remove(loanscheme);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion
    }
}
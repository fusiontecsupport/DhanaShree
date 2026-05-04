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
    public class FundAccountController : Controller
    {
        // GET: FundAccount

        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        //[Authorize(Roles = "MasterFundAccountIndex")]
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

                var data = e.pr_Search_Masters_FundAccount(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);
                var aaData = data.Select(d => new
                {
                    AccountCode = d.AccountCode,
                    AccountName = d.AccountName,
                    AccountOpeningBalance = d.AccountOpeningBalance.ToString(),
                    AccountBank = d.AccountBank,
                    AccountBranch = d.AccountBranch,
                    AccountNumber = d.AccountNumber,
                    ADescription = d.ADescription,                                     
                    StatusTypeID = d.StatusTypeID,
                    FundAccountID = d.FundAccountID.ToString()
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
        //[Authorize(Roles = "MasterFundAccountEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/FundAccount/Form/" + id);

                //Response.Redirect("/FundAccount/Form/?id=" + id);
            }
        }
        #endregion

        #region Form
        //[Authorize(Roles = "MasterAmountCodeCreate")]
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            CompanyFundAccount tab = new CompanyFundAccount();

            int FundAccountID = 0;

            //List<SelectListItem> StatusType = new List<SelectListItem>();
            //SelectListItem selectedSType = new SelectListItem { Text = "Active", Value = "0", Selected = true };
            //StatusType.Add(selectedSType);
            //selectedSType = new SelectListItem { Text = "InActive", Value = "1", Selected = false };
            //StatusType.Add(selectedSType);
            //ViewBag.StatusTypeID = StatusType;
            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Company Fund Account', 'Status'").ToList();
            ViewBag.StatusTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();
            if (id != "0")
            {
                FundAccountID = Convert.ToInt32(id);

                tab = context.fundaccount.Find(FundAccountID);

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
            }

            return View(tab);
        }
        #endregion

        #region Save Fund Account Master        
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

                        CompanyFundAccount fundaccountd = new CompanyFundAccount();

                        Int32 FundAccountID = 0;

                        string cmid = Convert.ToString(Fdata["FundAccountID"]);
                        if (cmid == "" || cmid == null)
                        { FundAccountID = 0; }
                        else
                        { FundAccountID = Convert.ToInt32(cmid); }

                        if (FundAccountID != 0)
                        {
                            fundaccountd = context.fundaccount.Find(FundAccountID);
                        }

                        fundaccountd.AccountCode = "FAC/" + FundAccountID.ToString(); //Convert.ToString(Fdata["AccountCode"]);
                        fundaccountd.AccountName = Convert.ToString(Fdata["AccountName"]);

                        string aob = Convert.ToString(Fdata["AccountOpeningBalance"]);
                        if (aob == "" || aob == null)
                        { fundaccountd.AccountOpeningBalance = 0; }
                        else
                        { fundaccountd.AccountOpeningBalance = Convert.ToDouble(aob); }

                        //fundaccount.AccountOpeningBalance = Convert.ToDecimal(Fdata["AccountOpeningBalance"]);
                        fundaccountd.AccountBank = Convert.ToString(Fdata["AccountBank"]);
                        fundaccountd.AccountBranch = Convert.ToString(Fdata["AccountBranch"]);
                        fundaccountd.AccountNumber = Convert.ToString(Fdata["AccountNumber"]);
                        fundaccountd.Description = Convert.ToString(Fdata["Description"]);
                        fundaccountd.StatusTypeID = Convert.ToInt16(Fdata["StatusTypeID"]);

                        //if (Session["CUSRID"] != null)
                        //    amontcode.CUSRID = Session["CUSRID"].ToString();
                        //else
                        //    amontcode.CUSRID = "";

                        if (FundAccountID == 0)
                        {

                            //amontcode.PRCSDATE = DateTime.Now;

                            string squery = "SELECT *FROM CompanyFundAccount WHERE AccountCode='" + fundaccountd.AccountCode + "' or AccountName='" + fundaccountd.AccountName + "'";
                            var query = context.Database.SqlQuery<CompanyFundAccount>(squery).ToList();// context.crmsalestaskdetails.SqlQuery(squery).ToList<CRMSalesTaskDetail>();

                            if (query.Count != 0)
                            {
                                status = "Existing";
                            }
                            else
                            {
                                context.fundaccount.Add(fundaccountd);
                                context.SaveChanges();
                                FundAccountID = fundaccountd.FundAccountID;
                                fundaccountd.AccountCode = "FAC/" + FundAccountID.ToString(); //Convert.ToString(Fdata["AccountCode"]);
                                context.Entry(fundaccountd).State = System.Data.Entity.EntityState.Modified;
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

                            context.Entry(fundaccountd).State = System.Data.Entity.EntityState.Modified;
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

        #region Delete Fund Account Master 

        //[Authorize(Roles = "MasterFundAccountDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check(fld, id, "FundAccount");

            //String temp = "PROCEED";

            if (temp.Equals("PROCEED"))
            {
                try
                {
                    CompanyFundAccount cfa = context.fundaccount.Find(Convert.ToInt32(id));
                    context.fundaccount.Remove(cfa);

                    context.SaveChanges();
                    Response.Write("Deleted Successfully ...");
                }
                catch(Exception ex)
                {
                    Response.Write("error");
                }
            }
            else
                Response.Write(temp);
        }
        #endregion 
    }
}
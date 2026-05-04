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
using QRCoder;
using System.Net;
using System.Drawing;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace CHITERP.Controllers.Products
{
    public class LoanSubscriptionController : Controller
    {
        // GET: LoanSubscription

        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        //[Authorize(Roles = "LoanSubscriptionIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            //if (string.IsNullOrEmpty(Session["SDATE"] as string))
            if (Session["SDATE"] == null)
            {

                Session["SDATE"] = DateTime.Now.ToString("yyyy-MM-dd");
                Session["EDATE"] = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                if (Request.Form.Get("from") != null)
                {
                    Session["SDATE"] = Request.Form.Get("from");
                    Session["EDATE"] = Request.Form.Get("to");
                }
            }
            return View();
        }
        #endregion

        #region Edit
        //[Authorize(Roles = "LoanSubscriptionEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/LoanSubscription/Form/" + id);

                //Response.Redirect("/LoanSubscription/Form/?id=" + id);
            }
        }
        #endregion

        #region Form
        //[Authorize(Roles = "LoanSubscriptionCreate")]
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            LoanTranRS LRS = new LoanTranRS();
            LoanSubscription tab = new LoanSubscription();
            LoanRequest tab2 = new LoanRequest();

            int LoanSubscriptionID = 0;
            
            if (id == "" || id == null || id == "0")
            { LoanSubscriptionID = 0; }
            else { LoanSubscriptionID = Convert.ToInt32(id); }

            ViewBag.LoanRequestID = "";
            ViewBag.OrganizationID = new SelectList("");
            var personqry = context.Database.SqlQuery<pr_Chit_Subscription_Organization_Name_Assgn_Result>("Exec pr_Chit_Subscription_Organization_Name_Assgn").ToList();
            ViewBag.FPersonID = new SelectList(personqry, "CLIENTID", "CNAME").ToList();
            
            var amtcdqry0 = context.Database.SqlQuery<MasterAmountCode>("Select * From MasterAmountCode (nolock)").ToList();

            ViewBag.AmountCodeID = new SelectList(amtcdqry0, "AmountCodeID", "AmountCodeName");

            var amtcdqry = context.Database.SqlQuery<LoanScheme>("Select * From LoanScheme (nolock)").ToList();
            ViewBag.LoanSchemeID = new SelectList(amtcdqry, "LoanSchemeID", "SchemeName");
            ViewBag.LoanSubsSchemeID = new SelectList(amtcdqry, "LoanSchemeID", "SchemeName");

            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Secured', ''").ToList();
            ViewBag.LoanSecuredTypeId = new SelectList(mtqry, "TypeID", "Type").ToList();
            ViewBag.LoanSubsSecuredTypeId = new SelectList(mtqry, "TypeID", "Type").ToList();

            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Transaction Mode', ''").ToList();
            ViewBag.DisbursementModeTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();
            ViewBag.RepaymentModeTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();
            ViewBag.SubsRepaymentModeTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();

            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Interest Deduction', ''").ToList();
            ViewBag.DeductionTypeId = new SelectList(mtqry, "TypeID", "Type").ToList();


            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Duration', ''").ToList();
            ViewBag.TermTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();


            var amtcdqry3 = context.Database.SqlQuery<CompanyOffice>("Select * From CompanyOffice (nolock)").ToList();
            ViewBag.LinkedOfficeID = new SelectList(amtcdqry3, "officeid", "OfficeName");
            ViewBag.SubsLinkedOfficeID = new SelectList(amtcdqry3, "officeid", "OfficeName");

            ViewBag.ClientType = new SelectList("");

            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Collection Type', ''").ToList();
            ViewBag.CollectionType = new SelectList(mtqry, "TypeID", "Type").ToList();

            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Interval', ''").ToList();
            ViewBag.RepaymentIntervalTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();

            var amtcdqry2 = context.Database.SqlQuery<CompanyFundAccount>("Select * From CompanyFundAccount (nolock)").ToList();

            ViewBag.FundAccountID = new SelectList(amtcdqry2, "FundAccountID", "AccountName");

            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Collection Mode', ''").ToList();
            ViewBag.CollectionModeTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();

            var amtcdqry4 = context.Database.SqlQuery<CompanyRoute>("select* from CompanyRoute (nolock)").ToList();
            ViewBag.RouteID = new SelectList(amtcdqry4, "RouteID", "RouteName");

            ViewBag.CoApplicants = new SelectList("");
            
            ViewBag.NomineeRelationshipTypeID = new SelectList("");
            
            ViewBag.CommissionAgentPersonID = new SelectList("");
            

            
            ViewBag.LoanSubscriptionId = new SelectList("");
            var amtcdqry5 = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_Get_Book_PickupDay 0").ToList();
            ViewBag.BookPickupDay = new SelectList(amtcdqry5, "DValue", "DText").ToList();


            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Loan Request', 'Status'").ToList();
            ViewBag.ReqStatusTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Loan Subscription', 'Status'").ToList();
            ViewBag.StatusTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();
            ViewBag.qrcdsrc = "";
            ViewBag.chk = "hide";
            string cusrid = Convert.ToString(Session["CUSRID"]);
            string crole = Convert.ToString(Session["Group"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                cusrid = "";
            }
            var empqry = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_Get_Employee_Dtl @usrid='" + cusrid + "'").ToList();
            ViewBag.AccountEmployeeID = new SelectList(empqry, "DValue", "DText").ToList();
            if (LoanSubscriptionID > 0)
            {
                ViewBag.QRSubsID = LoanSubscriptionID;
                ViewBag.ProdType = "30";
                ViewBag.OwnerID = LoanSubscriptionID;
                ViewBag.OwnerTypeID = "30";
                tab = context.loansubscription.Find(LoanSubscriptionID);
                tab2 = context.loanrequest.Find(Convert.ToInt32(tab.LoanRequestID));
                LRS.loanrequests = context.loanrequest.Where(lr => lr.LoanRequestID == tab.LoanRequestID).ToList();
                LRS.loansubs = context.loansubscription.Where(ls => ls.LoanSubscriptionID == LoanSubscriptionID).ToList();
                LRS.loanschedules = context.Database.SqlQuery<pr_Get_Loan_Schedule_n_Collection_Details_Result>("Exec pr_Get_Loan_Schedule_n_Collection_Details @LoanSubsID=" + LoanSubscriptionID + ", @usrid='" + cusrid + "'").ToList();
                personqry = context.Database.SqlQuery<pr_Chit_Subscription_Organization_Name_Assgn_Result>("Exec pr_Chit_Subscription_Organization_Name_Assgn").ToList();
                ViewBag.FPersonID = new SelectList(personqry.Where(X => X.CLIENTID == tab2.PersonID), "CLIENTID", "CNAME", tab2.PersonID).ToList();

                amtcdqry0 = context.Database.SqlQuery<MasterAmountCode>("Select * From MasterAmountCode (nolock)").ToList();

                ViewBag.AmountCodeID = new SelectList(amtcdqry0, "AmountCodeID", "AmountCodeName");

                amtcdqry = context.Database.SqlQuery<LoanScheme>("Select * From LoanScheme (nolock)").ToList();
                ViewBag.LoanSchemeID = new SelectList(amtcdqry.Where(x => x.LoanSchemeID == tab2.LoanSchemeID), "LoanSchemeID", "SchemeName", tab2.LoanSchemeID);
                ViewBag.LoanSubsSchemeID = new SelectList(amtcdqry.Where(x => x.LoanSchemeID == tab.LoanSchemeID), "LoanSchemeID", "SchemeName", tab.LoanSchemeID);

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Secured', ''").ToList();
                ViewBag.LoanSecuredTypeId = new SelectList(mtqry, "TypeID", "Type", tab2.LoanSecuredTypeId).ToList();//.Where(x => x.TypeID == tab2.LoanSecuredTypeId)
                ViewBag.LoanSubsSecuredTypeId = new SelectList(mtqry, "TypeID", "Type", tab.LoanSecuredTypeId).ToList();//.Where(x => x.TypeID == tab.LoanSecuredTypeId)

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Transaction Mode', ''").ToList();
                ViewBag.DisbursementModeTypeID = new SelectList(mtqry, "TypeID", "Type", tab2.DisbursementModeTypeID).ToList();//.Where(x => x.TypeID == tab2.DisbursementModeTypeID)
                ViewBag.RepaymentModeTypeID = new SelectList(mtqry, "TypeID", "Type", tab2.RepaymentModeTypeID).ToList();//.Where(x => x.TypeID == tab2.RepaymentModeTypeID)
                ViewBag.SubsRepaymentModeTypeID = new SelectList(mtqry, "TypeID", "Type", tab.RepaymentModeTypeID).ToList();//.Where(x => x.TypeID == tab.RepaymentModeTypeID)

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Interest Deduction', ''").ToList();
                ViewBag.DeductionTypeId = new SelectList(mtqry.Where(x => x.TypeID == tab2.DeductionTypeId || tab2.DeductionTypeId==null || tab2.DeductionTypeId == 0), "TypeID", "Type", tab2.DeductionTypeId).ToList();


                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Duration', ''").ToList();
                ViewBag.TermTypeID = new SelectList(mtqry.Where(x => x.TypeID == tab2.TermTypeID || tab2.TermTypeID == null), "TypeID", "Type", tab2.TermTypeID).ToList();


                amtcdqry3 = context.Database.SqlQuery<CompanyOffice>("Select * From CompanyOffice (nolock)").ToList();
                ViewBag.LinkedOfficeID = new SelectList(amtcdqry3, "officeid", "OfficeName", tab2.LinkedOfficeID);//.Where(x => x.OfficeID == tab2.LinkedOfficeID || tab2.LinkedOfficeID == null || tab2.LinkedOfficeID ==0)
                ViewBag.SubsLinkedOfficeID = new SelectList(amtcdqry3, "officeid", "OfficeName", tab.LinkedOfficeId);//.Where(x => x.OfficeID == tab.LinkedOfficeId || tab.LinkedOfficeId == null || tab.LinkedOfficeId == 0)
                ViewBag.ClientType = new SelectList("");

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Collection Type', ''").ToList();
                ViewBag.CollectionType = new SelectList(mtqry, "TypeID", "Type", tab.CollectionType).ToList();//.Where(x => x.TypeID == tab.CollectionType || tab.CollectionType ==null || tab.CollectionType ==0)

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Interval', ''").ToList();
                ViewBag.RepaymentIntervalTypeID = new SelectList(mtqry, "TypeID", "Type", tab.RepaymentIntervalTypeID).ToList();//.Where(x => x.TypeID == tab.RepaymentIntervalTypeID || tab.RepaymentIntervalTypeID==null || tab.RepaymentIntervalTypeID ==0)

                amtcdqry2 = context.Database.SqlQuery<CompanyFundAccount>("Select * From CompanyFundAccount (nolock)").ToList();

                ViewBag.FundAccountID = new SelectList(amtcdqry2, "FundAccountID", "AccountName");

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Collection Mode', ''").ToList();
                ViewBag.CollectionModeTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();

                amtcdqry4 = context.Database.SqlQuery<CompanyRoute>("select* from CompanyRoute (nolock)").ToList();
                ViewBag.RouteID = new SelectList(amtcdqry4, "RouteID", "RouteName",tab.RouteID);//.Where(x=>x.RouteID==tab.RouteID || tab.RouteID ==0)

                ViewBag.CoApplicants = new SelectList("");

                ViewBag.NomineeRelationshipTypeID = new SelectList("");

                ViewBag.CommissionAgentPersonID = new SelectList("");
                
                ViewBag.AccountEmployeeID = new SelectList(empqry, "DValue", "DText", tab2.AccountEmployeeID).ToList();
                ViewBag.LoanSubsSchemeID = new SelectList("");
                ViewBag.LoanSubsSecuredTypeId = new SelectList("");
                ViewBag.SubsRepaymentModeTypeID = new SelectList("");
                ViewBag.SubsLinkedOfficeID = new SelectList("");
                ViewBag.LoanSubscriptionId = new SelectList("");
                amtcdqry5 = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_Get_Book_PickupDay 0").ToList();
                ViewBag.BookPickupDay = new SelectList(amtcdqry5, "DValue", "DText", tab.BookPickupDay).ToList();//.Where(x=>x.DValue==tab.BookPickupDay || tab.BookPickupDay == null || tab.BookPickupDay == 0)


                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Loan Request', 'Status'").ToList();
                ViewBag.ReqStatusTypeID = new SelectList(mtqry, "TypeID", "Type",tab2.StatusTypeID).ToList();
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Loan Subscription', 'Status'").ToList();
                ViewBag.StatusTypeID = new SelectList(mtqry, "TypeID", "Type", tab.StatusTypeID).ToList();
                string imgurl = "~/LSubsQRCode/" + LoanSubscriptionID.ToString() + ".png";
                string imgpath = Server.MapPath("~/LSubsQRCode/" + id.ToString() + ".png");
                //bool chkurl = RemoteFileExists(imgurl);
                bool chkurl = System.IO.File.Exists(imgpath);

                //if (chkurl == false)
                //{
                    GenerateQRCodeFile(LoanSubscriptionID);
                //}
                if (System.IO.File.Exists(imgpath))
                {
                    ViewBag.qrcdsrc = @Url.Content(imgurl);
                    ViewBag.chk = "";
                }
            }
            else
            {
                tab = null;
                //LRS.loanrequests = context.loanrequest.Where(lr => lr.LoanRequestID == 0).ToList();
               // LRS.loansubs = context.loansubscription.Where(ls => ls.LoanSubscriptionID == 0).ToList();

                LRS = null;

            }

            return View(LRS);
        }
        #endregion

        #region Ledger Report
        //[Authorize(Roles = "LoanSubscriptionManagementCreate")]
        public ActionResult LoanLedgerRpt(string id = "0")
        {
            ViewBag.SubscriptionID = id;
            return View();
        }
        #endregion

        private bool RemoteFileExists(string url)
        {
            try
            {
                //Creating the HttpWebRequest
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //Setting the Request method HEAD, you can also use GET too.
                request.Method = "HEAD";
                //Getting the Web Response.
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //Returns TRUE if the Status code == 200
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                //Any exception will returns false.
                return false;
            }
        }
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

                    var data = e.pr_Search_Product_LoanSubscription(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, Convert.ToString(cusrid), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                    var aaData = data.Select(d => new
                    {

                        ClientId = d.ClientID.ToString(),
                        ClientMobileNo = d.ClientMobileNo,
                        ClientName = d.ClientName,
                        LoanSubscriptionCode = d.LoanSubscriptionCode,
                        RepaymentIntervalTypeDesc = d.RepaymentIntervalTypeDesc,
                        LoanSchemeCode = d.LoanSchemeCode,
                        InterestDeductionTypeDesc = d.InterestDeductionTypeDesc,
                        StatusTypeDesc = d.StatusTypeDesc,
                        LoanSubscriptionID = d.LoanSubscriptionID.ToString(),
                        LoanDuration = d.LoanDuration,
                        Principal = d.Principal.ToString(),
                        SubsDate = d.SubsDate,
                        InterestRate = d.InterestRate.ToString()
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

        #region Get Schedule & Collection Detail
        //public ActionResult GetScheduleDtl(string id)
        //{
        //    int SubsID = 0;
        //    if (id != "" && id != null && id != "undefined")
        //        SubsID = Convert.ToInt32(id);
        //    if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
        //    else
        //    {
        //        using (var e = new FinFusionEntities())
        //        {
        //            var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
        //            var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
        //            string crole = Convert.ToString(Session["Group"]);
        //            string cusrid = Convert.ToString(Session["CUSRID"]);
        //            if (crole == "SuperAdmin" || crole == "Admin")
        //            {
        //                cusrid = "";
        //            }

        //            var data = e.pr_Get_Loan_Schedule_n_Collection_Details(SubsID,cusrid);
        //            var aaData = data.Select(d => new
        //            {

        //                InstNo = d.InstNo.ToString(),
        //                InsttDt = d.InsttDt.ToString(),
        //                EMI_Amount = d.EMI_Amount.ToString(),
        //                Loan_Balance = d.Loan_Balance.ToString(),

        //                CollectionDt = d.collectiondt,
        //                Credit = d.Credit.ToString(),

        //                colbalance = d.colbalance.ToString(),
        //                CollectionSts = d.CollectionSts

        //            }).ToArray();

        //            return Json(new
        //            {
        //                //sEcho = param.sEcho,
        //                data = aaData,
        //                iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
        //                iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
        //            }, JsonRequestBehavior.AllowGet);

        //        }
        //    }
        //}
        public string GetScheduleDtl(string id)
        {
            int SubsID = 0;
            if (id != "" && id != null && id != "undefined")
                SubsID = Convert.ToInt32(id);
            
            
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

                    var data = e.pr_Get_Loan_Schedule_n_Collection_Details(SubsID, cusrid);
                var tabl = "";

                var count = 0;
                tabl = "<Table id=LOANSCHEDULE_TDETAIL class='table table-striped table-bordered bootstrap-datatable LOANSCHEDULEDETAIL'> <thead><tr><th>Inst.No </th><th>Inst.Dt</th><th>EMI.Amt.</th><th>Loan To Be Paid</th><th>Collection Dt</th><th>Colln. Amt</th><th>Collection Sts.</th></tr></thead><tbody id='loanschedule_tbody'>";
                var query = context.Database.SqlQuery<pr_Get_Loan_Schedule_n_Collection_Details_Result>("EXEC pr_Get_Loan_Schedule_n_Collection_Details @LoanSubsID =" + SubsID + ", @usrid = '" + cusrid + "'").ToList();

                foreach (var rslt in query)
                {
                    tabl = tabl + "<tr>";
                    tabl = tabl + "<td class='col-md-1'><input type=text id=InstNo value='" + rslt.InstNo + "' readonly class='form-control InstNo' name=InstNo maxlength='5' tabindex='105'></td>";
                    tabl = tabl + "<td class='col-md-1'><input type=text id=InsttDt value='" + rslt.InsttDt + "' readonly class='form-control InsttDt' name=InsttDt maxlength='15'tabindex='105'></td>";
                    tabl = tabl + "<td class='col-md-1'><input type=text id=EMI_Amount class='form-control EMI_Amount' readonly name=EMI_Amount value='" + rslt.EMI_Amount + "' maxlength='25' tabindex='105'></td>";
                    tabl = tabl + "<td class='col-md-1'><input type=text id=ToBePaid_Amt class='form-control ToBePaid_Amt' readonly name=ToBePaid_Amt value='" + rslt.ToBePaid_Amt + "' maxlength='25' tabindex='105'></td>";
                    tabl = tabl + "<td class='col-md-1'><input type=text id=collectiondt class='form-control collectiondt' readonly name=collectiondt value='" + rslt.collectiondt + "' maxlength='25' tabindex='105'></td>";
                    tabl = tabl + "<td class='col-md-1'><input type=text id=Credit class='form-control Credit' readonly name=Credit value='" + rslt.Credit + "' maxlength='25' tabindex='105'></td>";
                    tabl = tabl + "<td class='col-md-1'><input type=text id=colbalance class='form-control colbalance' readonly name=Credit value='" + rslt.colbalance + "' maxlength='25' tabindex='105'></td>";
                    tabl = tabl + "<td class='col-md-1'><input type=text id=CollectionSts class='form-control CollectionSts' readonly name=CollectionSts value='" + rslt.CollectionSts + "' maxlength='25' tabindex='105'></td>";
                    tabl = tabl + "</tr>";
                    count++;
                }
                tabl = tabl + "</tbody></table>";

                return tabl;
            }
            
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

                            LoanSubscription master = new LoanSubscription();
                            LoanRequest master2 = new LoanRequest();

                            Int32 LoanSubscriptionID = 0;
                            Int32 LoanRequestID = 0;

                            string LoanSubscriptionid = Convert.ToString(formcoll["LoanSubscriptionID"]);
                            
                            if (LoanSubscriptionid == "" || LoanSubscriptionid == null)
                            {
                                LoanSubscriptionID = 0;
                            }
                            else
                            {
                                LoanSubscriptionID = Convert.ToInt32(LoanSubscriptionid);
                                
                            }
                            //master.LoanSubscriptionCode = "L" + LoanSubscriptionID.ToString();
                            if (LoanSubscriptionID != 0)
                            {
                                master = context.loansubscription.Find(LoanSubscriptionID);                                
                                LoanRequestID = Convert.ToInt32(master.LoanRequestID);
                                master2 = context.loanrequest.Find(LoanRequestID);
                            }
                            master.LoanSubscriptionID= LoanSubscriptionID;

                            master2.PersonID = Convert.ToInt32(formcoll["PersonID"]);
                            master.LoanSchemeID = Convert.ToInt32(formcoll["LoanSchemeID"]);
                            master2.LoanSchemeID = Convert.ToInt32(formcoll["LoanSchemeID"]);

                            master.LoanSecuredTypeId = Convert.ToInt32(formcoll["LoanSecuredTypeId"]);
                            master2.LoanSecuredTypeId = Convert.ToInt32(formcoll["LoanSecuredTypeId"]);
                            master.LinkedOfficeId= Convert.ToInt32(formcoll["LinkedOfficeID"]);
                            master.DisbursementModeTypeID = 0;
                            if (formcoll["DisbursementModeTypeID"] != "" && formcoll["DisbursementModeTypeID"] != null)
                                master.DisbursementModeTypeID = Convert.ToInt32(formcoll["DisbursementModeTypeID"]);
                            master2.DisbursementModeTypeID= 0;
                            if (formcoll["DisbursementModeTypeID"] != "" && formcoll["DisbursementModeTypeID"] != null)
                                master2.DisbursementModeTypeID = Convert.ToInt32(formcoll["DisbursementModeTypeID"]);

                            master.RepaymentModeTypeID = 0;
                            if (formcoll["RepaymentModeTypeID"] != "" && formcoll["RepaymentModeTypeID"] != null)
                                master.RepaymentModeTypeID = Convert.ToInt32(formcoll["RepaymentModeTypeID"]);
                            
                            master2.RepaymentModeTypeID = 0;
                            if (formcoll["RepaymentModeTypeID"] != "" && formcoll["RepaymentModeTypeID"] != null)                            
                                master2.RepaymentModeTypeID = Convert.ToInt32(formcoll["RepaymentModeTypeID"]);
                            
                            master2.Principal = 0;
                            if (formcoll["Principal"] != "" && formcoll["Principal"] != null)
                                master2.Principal = Convert.ToDouble(formcoll["Principal"]);
                            master.Principal = 0;
                            if (formcoll["Principal"] != "" && formcoll["Principal"] != null)
                                master.Principal = Convert.ToDouble(formcoll["Principal"]);

                            master.LoanAmount = 0;
                            if (formcoll["LoanAmount"] != "" && formcoll["LoanAmount"] != null)
                                master.LoanAmount = Convert.ToDouble(formcoll["LoanAmount"]);
                            master2.LoanAmount = 0;
                            if (formcoll["LoanAmount"] != "" && formcoll["LoanAmount"] != null)
                                master2.LoanAmount = Convert.ToDouble(formcoll["LoanAmount"]);

                            master.InterestRate = 0;
                            if (formcoll["InterestRate"] != "" && formcoll["InterestRate"] != null)
                                master.InterestRate = Convert.ToDouble(formcoll["InterestRate"]);
                            master2.InterestRate = 0;
                            if (formcoll["InterestRate"] != "" && formcoll["InterestRate"] != null)
                                master2.InterestRate = Convert.ToDouble(formcoll["InterestRate"]);

                            
                            master2.DeductionTypeId = 0;
                            if (formcoll["DeductionTypeId"] != "" && formcoll["DeductionTypeId"] != null)
                                master2.DeductionTypeId = Convert.ToInt32(formcoll["DeductionTypeId"]);

                            master.Term = 0;
                            if (formcoll["Term"] != "" && formcoll["Term"] != null)
                                master.Term = Convert.ToDouble(formcoll["Term"]);
                            master2.Term = 0;
                            if (formcoll["Term"] != "" && formcoll["Term"] != null)
                                master2.Term = Convert.ToDouble(formcoll["Term"]);

                            master.TermTypeID = 0;
                            if (formcoll["TermTypeID"] != "" && formcoll["TermTypeID"] != null)
                                master.TermTypeID = Convert.ToInt32(formcoll["TermTypeID"]);
                            master2.TermTypeID = 0;
                            if (formcoll["TermTypeID"] != "" && formcoll["TermTypeID"] != null)
                                master2.TermTypeID = Convert.ToInt32(formcoll["TermTypeID"]);

                            
                            master.LinkedOfficeId = 0;
                            if (formcoll["LinkedOfficeID"] != "" && formcoll["LinkedOfficeID"] != null)
                                master.LinkedOfficeId = Convert.ToInt32(formcoll["LinkedOfficeID"]);
                            master2.LinkedOfficeID = 0;
                            if (formcoll["LinkedOfficeID"] != "" && formcoll["LinkedOfficeID"] != null)
                                master2.LinkedOfficeID = Convert.ToInt32(formcoll["LinkedOfficeID"]);

                            master.DisbursementDate = null;
                            master2.DisbursementDate = null;
                            if (formcoll["DisbursementDate"] != "" && formcoll["DisbursementDate"] != null)
                            {
                                master.DisbursementDate = Convert.ToDateTime(formcoll["DisbursementDate"]);
                                master2.DisbursementDate = Convert.ToDateTime(formcoll["DisbursementDate"]);
                            }
                            
                            master2.RequestedOn = null;
                            if (formcoll["RequestedOn"] != "" && formcoll["RequestedOn"] != null)
                            {
                                master2.RequestedOn = Convert.ToDateTime(formcoll["RequestedOn"]);
                            }
                            master2.AccountEmployeeID = Convert.ToInt32(formcoll["AccountEmployeeID"]);
                            master2.StatusTypeID = Convert.ToInt32(formcoll["ReqStatusTypeID"]);
                            if (formcoll["StatusTypeID"] != "" && formcoll["StatusTypeID"] != null)
                                    master.StatusTypeID = Convert.ToInt32(formcoll["StatusTypeID"]);
                            else
                            {
                                var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Loan Request', 'Status'").ToList();
                                var mtqry1 = new SelectList(mtqry.Where(x=>x.TypeID==Convert.ToInt32(master2.StatusTypeID)), "TypeID", "Type").ToList();
                                
                                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Loan Subscription', 'Status'").ToList();
                                var mtqry2 = new SelectList(mtqry.Where(x => x.Type == Convert.ToString(mtqry1[0].Text)), "TypeID", "Type").ToList();
                                if(mtqry2.Count>0)
                                {
                                    master.StatusTypeID = Convert.ToInt32(mtqry2[0].Value);
                                }
                                
                            }


                            //master.CollectionModeTypeID = Convert.ToInt32(formcoll["CollectionModeTypeID"]);
                            
                            if (formcoll["RepaymentIntervalTypeID"] != "" && formcoll["RepaymentIntervalTypeID"] != null)
                                master.RepaymentIntervalTypeID = Convert.ToInt32(formcoll["RepaymentIntervalTypeID"]);

                            if (formcoll["CollectionType"] != "" && formcoll["CollectionType"] != null)
                                master.CollectionType = Convert.ToInt32(formcoll["CollectionType"]);

                            if (formcoll["BookPickupDay"] != "" && formcoll["BookPickupDay"] != null)
                                master.BookPickupDay = Convert.ToInt32(formcoll["BookPickupDay"]);

                            string DisbursementDate = Convert.ToString(formcoll["DisbursementDate"]);
                            master.DisbursementDate = null;
                            if (DisbursementDate != null && DisbursementDate != "")
                                master.DisbursementDate = Convert.ToDateTime(formcoll["DisbursementDate"]);
                            master2.DisbursementDate = null;
                            if (DisbursementDate != null && DisbursementDate != "")
                                master2.DisbursementDate = Convert.ToDateTime(formcoll["DisbursementDate"]);

                            string RequestedOn = Convert.ToString(formcoll["RequestedOn"]);
                            master2.RequestedOn = null;
                            if (RequestedOn != null && RequestedOn != "")
                                master2.RequestedOn = Convert.ToDateTime(formcoll["RequestedOn"]);
                            master.ProcessedOn = Convert.ToDateTime(DateTime.Now);
                            //string CommencementTime = Convert.ToString(formcoll["CommencementTime"]);
                            //master.CommencementTime = null;
                            //if (CommencementTime != null && CommencementTime != "" && CommencementTime != "00:00:00" && CommencementTime != "00:00")
                            //    master.CommencementTime = Convert.ToString(formcoll["CommencementTime"]);

                            //string CommencementDate = Convert.ToString(formcoll["CommencementDate"]);
                            //master.CommencementDate = null;
                            //if (CommencementDate != null && CommencementDate != "")
                            //    master.CommencementDate = Convert.ToDateTime(formcoll["CommencementDate"]);

                            if (formcoll["RouteID"] == "" || formcoll["RouteID"] == null)
                                master.RouteID = 0;
                            else
                                master.RouteID = Convert.ToInt32(formcoll["RouteID"]);
                            //if (formcoll["TicketPaidNumber"] == "" || formcoll["TicketPaidNumber"] == null)
                            //    master.TicketPaidNumber = 0;
                            //else
                            //    master.TicketPaidNumber = Convert.ToInt32(formcoll["TicketPaidNumber"]);
                          


                            master.AssociatedOrganizationID= 0;
                            //ClientMaster cm = new ClientMaster();
                            //cm = context.clientmasters.Find(master2.PersonID);

                            //if (cm.CLIENTTID == 1)
                            //    master2.SubscriberIsPerson = 1;

                            if(LoanRequestID==0)
                            {
                                context.loanrequest.Add(master2);
                                context.SaveChanges();
                                LoanRequestID = master2.LoanRequestID;
                                master.LoanRequestID=master2.LoanRequestID;
                                
                                context.Entry(master2).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                status = "Success";

                            }
                            else
                            {
                                context.Entry(master2).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                status = "Success";
                            }
                            if (LoanSubscriptionID == 0)
                            {
                                //string squery = "SELECT *FROM LoanSubscription (nolock) WHERE LoanSubscriptionCode ='" + master.LoanSubscriptionCode + "' and personid='" + master.PersonID+ "'";
                                //var query = context.Database.SqlQuery<LoanSubscription>(squery).ToList();

                                //if (query.Count != 0)
                                //{
                                //    status = "Existing";
                                //}
                                //else
                                {
                                    //master.CreatedBy = Session["CUSRID"].ToString();
                                    //master.CreatedDt = DateTime.Now;
                                    LoanScheme lsm = new LoanScheme();
                                    int lsmid = 0;
                                    lsmid = Convert.ToInt32(master.LoanSchemeID);
                                    lsm = context.loanscheme.Find(lsmid);

                                    master.SCNO = Convert.ToInt32(Autonumber.autonum("LoanSubscription", "SCNO", "SCNO <> 0 and LoanSubscriptionCode like '"+ Session["COMPCODE"].ToString().Trim() +"/" + Convert.ToString(lsm.SchemeCode) + "%'").ToString());
                                    int scno = Convert.ToInt32(master.SCNO);
                                    string prfx = string.Format("{0:D5}", scno);


                                    string aprfx = ""+ Session["COMPCODE"].ToString().Trim() +"/" + lsm.SchemeCode + "/" + prfx;
                                    master.LoanSubscriptionCode = aprfx.ToString();
                                    context.loansubscription.Add(master);                                    
                                    context.SaveChanges();
                                    LoanSubscriptionID = master.LoanSubscriptionID;
                                    //master.LoanSubscriptionCode = "L" + LoanSubscriptionID.ToString();
                                    //context.Entry(master).State = System.Data.Entity.EntityState.Modified;
                                    //context.SaveChanges();
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
                            LoanSubscriptionID = master.LoanSubscriptionID;


                            ViewBag.Message = "Form submitted.";
                            // Response.Redirect("Index");
                            //return RedirectToAction("Index", "LoanSubscriptionManagement");
                            //Response.Write("Update");
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            var ermsg = ex.Message;
                            //Response.Write(ex.Message.ToString());
                            trans1.Rollback();
                            //Response.Redirect("/Error/AccessDenied");
                            //return RedirectToAction("Index", "LoanSubscriptionManagement");
                            status = "Error";
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

        }
        #endregion

        #region QR Code Generation
        
        public void GenerateQRCodeFile(int? id = 0)
        {
            if (!System.IO.Directory.Exists(Server.MapPath("~/LSubsQRCode/")))
                Directory.CreateDirectory(Server.MapPath("~/LSubsQRCode/"));

            string barcodePath = Server.MapPath("~/LSubsQRCode/" + id.ToString() + ".png");
            

            var result = context.Database.SqlQuery<pr_Get_QRCode_Chit_Subscription_Dtl_Result>("exec pr_Get_QRCode_Loan_Subscription_Dtl @usrid='" + Session["CUSRID"].ToString() + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ", @subsid =" + id).ToList();//........procedure  for edit mode details data
            foreach (var rslt in result)
            {
                var subscriptioncode = rslt.subscriptioncode;
                var clienttype = rslt.clienttype;
                var subscriptionid = rslt.subscriptionid;
                //var collectionamt = rslt.collectionamt;
                //var chitvalue = rslt.chitvalue;
                //var clientname = rslt.clientname;

                //string QRContent = subscriptionid + "|" + subscriptioncode + "|" + clienttype + "|" + collectionamt + "|" + chitvalue + "|" + clientname + "|";
                //string QRContent = "http://37.17.50.19/dsfinfusion/TransactionCollection/Form?id='0~30~" + subscriptionid.ToString() + "'";
                //string QRContent = "http://37.17.50.19/dsfinfusion/TrCol/Edit?id='0~30~" + subscriptionid.ToString() + "'";
                string burl = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath;
                string QRContent = burl+"/TransactionCollection/Form?id='0~30~" + subscriptionid.ToString() + "'";
                try
                {
                    //using (MemoryStream ms = new MemoryStream())
                    //{
                    //    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    //    QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(QRContent, QRCodeGenerator.ECCLevel.Q);



                    //    using (Bitmap bitMap = qrCode.GetGraphic(20))
                    //    {
                    //        bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    //        byte[] byteImage = ms.ToArray();
                    //        System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                    //        img.Save(barcodePath, System.Drawing.Imaging.ImageFormat.Png);
                    //    }
                    //}
                    //if (System.IO.File.Exists(barcodePath))
                    //    System.IO.File.Delete(barcodePath);
                    ////                    var url = string.Format("http://chart.apis.google.com/chart?cht=qr&chs={1}x{2}&chl={0}", QRContent, 500, 500);
                    //var url = string.Format("http://chart.apis.google.com/chart?cht=qr&chs={1}x{2}&chl={0}&chld={3}", QRContent, 500, 500, "L");
                    //WebResponse response = default(WebResponse);
                    //Stream remoteStream = default(Stream);
                    //StreamReader readStream = default(StreamReader);
                    //WebRequest request = WebRequest.Create(url);
                    //response = request.GetResponse();
                    //remoteStream = response.GetResponseStream();
                    //readStream = new StreamReader(remoteStream);
                    //System.Drawing.Image img = System.Drawing.Image.FromStream(remoteStream);
                    //img.Save(barcodePath);
                    //response.Close();
                    //remoteStream.Close();
                    //readStream.Close();

                    using (MemoryStream ms = new MemoryStream())
                    {
                        //QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        //QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(QRContent, QRCodeGenerator.ECCLevel.H);

                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(QRContent, QRCodeGenerator.ECCLevel.Q);
                        QRCode qrCode = new QRCode(qrCodeData);

                        using (Bitmap bitMap = qrCode.GetGraphic(20))
                        {
                            bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] byteImage = ms.ToArray();
                            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                            img.Save(barcodePath, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }

                }
                catch (Exception e)
                {
                    Response.Write(e.Message);
                }

            }

        }
        #endregion

        #region Loan Ledger Report

        public string GetLoanLedgerRpt(string id)
        {
            try
            {

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IdentityCon"].ToString()))
                {

                    conn.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataAdapter da = new SqlDataAdapter("[dbo].[CustomerAccountLedgerbyPcode_Loan]", conn);

                    var tenure = 0;
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.CommandTimeout = 0;
                    da.SelectCommand.Parameters.Add(new SqlParameter("@Code", SqlDbType.VarChar));
                    //da.SelectCommand.Parameters["@Code"].Value = "L" + id.Trim();
                    LoanSubscription ls = new LoanSubscription();
                    ls = context.loansubscription.Find(Convert.ToInt32(id));
                    da.SelectCommand.Parameters["@Code"].Value = ls.LoanSubscriptionCode;

                    DataSet ds = new DataSet();
                    StringBuilder html0 = new StringBuilder();
                    StringBuilder html1 = new StringBuilder();
                    StringBuilder html2 = new StringBuilder();
                    StringBuilder html3 = new StringBuilder();
                    StringBuilder sb = new StringBuilder();
                    da.Fill(ds, "L1");



                    //Building the Header row.

                    if (ds.Tables[4].Rows.Count > 0)
                    {
                        tenure = Convert.ToInt16(ds.Tables[4].Rows[0][5]);
                        html0.Append("<table class='table table-striped  datatable'>");
                        html0.Append("<tr>");
                        html0.Append("<th> Client Name</th>");
                        html0.Append("<td> " + ds.Tables[4].Rows[0][10].ToString() + "</td>");
                        html0.Append("<th> Address <th>");
                        html0.Append("<td colspan = '5'> " + ds.Tables[4].Rows[0][12].ToString() + "</td>");
                        html0.Append("</tr><tr>");
                        html0.Append("<th> Loan Scheme </th>");
                        html0.Append("<td> " + ds.Tables[4].Rows[0][4].ToString() + "</td>");
                        html0.Append("<th> Loan Subscription Code </th>");
                        html0.Append("<td> " + ds.Tables[4].Rows[0][0].ToString() + "</td>");

                        html0.Append("<th> Interest Rate </th>");                        
                        html0.Append("<td> " + ds.Tables[4].Rows[0][3].ToString() + " %" + "</td>");
                        html0.Append("<th> Term </th>");
                        html0.Append("<td> " + ds.Tables[4].Rows[0][6].ToString() + " " + ds.Tables[4].Rows[0][7].ToString() + "</td>");
                        html0.Append("</tr><tr>");
                        
                        html0.Append("<th> Disbursement </th>");
                        html0.Append("<td> " +  ds.Tables[4].Rows[0][2].ToString() + "</td>");
                        html0.Append("<th> Status </th>");
                        html0.Append("<td> " +  ds.Tables[4].Rows[0][9].ToString() + "</td>");
                        html0.Append("<th> </th>");
                        html0.Append("<td> </td>");
                        html0.Append("<th> </th>");
                        html0.Append("<td> </td>");

                        html0.Append("</tr>");
                        html0.Append("</table>");

                       
                    }

                   

                    if (ds.Tables[5].Rows.Count > 0)
                    {
                        html1.Append("<table class='table table-striped datatable'>");

                        html1.Append("<tr>");
                        foreach (DataColumn column in ds.Tables[5].Columns)
                        {
                            html1.Append("<th>");
                            html1.Append(column.ColumnName);
                            html1.Append("</th>");
                        }
                        html1.Append("</tr>");

                        foreach (DataRow row in ds.Tables[5].Rows)
                        {
                            html1.Append("<tr>");
                            foreach (DataColumn column in ds.Tables[5].Columns)
                            {
                                html1.Append("<td>");
                                if (column.ColumnName == "Totals")
                                {
                                    html1.Append(Convert.ToDecimal(row[column.ColumnName]).ToString("#,##,###0.00"));
                                }
                                else
                                {
                                    html1.Append(row[column.ColumnName]);
                                }
                                html1.Append("</td>");
                            }
                            html1.Append("</tr>");
                        }

                        html1.Append("</table>");
                    }
                    if (ds.Tables[6].Rows.Count > 0)
                    {
                        html2.Append("<table class='table table-striped datatable'>");

                        html2.Append("<tr>");
                        foreach (DataColumn column in ds.Tables[6].Columns)
                        {
                            html2.Append("<th>");
                            html2.Append(column.ColumnName);
                            html2.Append("</th>");
                        }
                        html2.Append("</tr>");

                        foreach (DataRow row in ds.Tables[6].Rows)
                        {
                            html2.Append("<tr>");
                            foreach (DataColumn column in ds.Tables[6].Columns)
                            {
                                html2.Append("<td>");
                                if (column.ColumnName == "Value")
                                {
                                    html2.Append(Convert.ToDecimal(row[column.ColumnName]).ToString("#,##,###0.00"));
                                }
                                else
                                {
                                    html2.Append(row[column.ColumnName]).ToString();
                                }
                                html2.Append("</td>");
                            }
                            html2.Append("</tr>");
                        }

                        html2.Append("</table>");
                    }
                    if (ds.Tables[7].Rows.Count > 0)
                    {
                        html3.Append("<table class='table table-striped datatable'>");

                        html3.Append("<tr>");
                        foreach (DataColumn column in ds.Tables[7].Columns)
                        {
                            html3.Append("<th>");
                            html3.Append(column.ColumnName);
                            html3.Append("</th>");
                        }
                        html3.Append("</tr>");

                        foreach (DataRow row in ds.Tables[7].Rows)
                        {
                            html3.Append("<tr>");
                            foreach (DataColumn column in ds.Tables[7].Columns)
                            {
                                html3.Append("<td>");
                                html3.Append(row[column.ColumnName]).ToString();
                                html3.Append("</td>");
                            }
                            html3.Append("</tr>");
                        }

                        html3.Append("</table>");
                    }

                    //if (ds.Tables[7].Rows.Count > 0)
                    //{


                    //    //Table start.
                    //    html3.Append("<table class='table-striped datatable'>");

                    //    //Building the Header row.
                    //    html3.Append("<tr>");
                    //    foreach (DataColumn column in ds.Tables[7].Columns)
                    //    {
                    //        html3.Append("<th>");
                    //        html3.Append(column.ColumnName);
                    //        html3.Append("</th>");
                    //    }
                    //    html3.Append("</tr>");

                    //    //Building the Data rows.
                    //    foreach (DataRow row in ds.Tables[7].Rows)
                    //    {
                    //        html3.Append("<tr>");
                    //        foreach (DataColumn column in ds.Tables[7].Columns)
                    //        {
                    //            html3.Append("<td>");
                    //            if (row[column.ColumnName].ToString().Contains("close"))
                    //            {
                    //                html3.Append(row[column.ColumnName]).ToString();
                    //            }
                    //            else
                    //            {
                    //                html3.Append(row[column.ColumnName]).ToString();
                    //            }

                    //            html3.Append("</td>");
                    //        }
                    //        html3.Append("</tr>");
                    //    }

                    //    //Table end.
                    //    html3.Append("</table>");

                    //    //Append the HTML string to Placeholder.

                    //}


                    if (tenure == 1)
                    {
                        //tenure start
                        if (ds.Tables.Count != 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {   //Table start.
                                sb.Append("<table class='table table-striped datatable' cellpadding='5' cellspacing='0' style='border: 1px solid #ccc;font-size: 9pt;font-family:Arial'>");

                                //Adding HeaderRow.
                                sb.Append("<tr>");
                                foreach (DataColumn column in ds.Tables[0].Columns)
                                {
                                    sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>" + column.ColumnName + "</th>");
                                }
                                sb.Append("</tr>");

                                var clr = "";
                                //Adding DataRow.
                                foreach (DataRow row in ds.Tables[0].Rows)
                                {
                                    sb.Append("<tr>");
                                    foreach (DataColumn column in ds.Tables[0].Columns)
                                    {

                                        var clnname = column.ColumnName;
                                        if (row[column.ColumnName].ToString().Contains("-"))
                                        {
                                            sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;color:red;'>" + row[column.ColumnName].ToString() + "</td>");
                                        }
                                        else if (row[column.ColumnName].ToString().Contains("Dividend"))
                                        {
                                            clr = "red";
                                            sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;color:" + clr + ";'>" + row[column.ColumnName].ToString() + "</td>");
                                        }
                                        else
                                        {
                                            if (clnname == "Debit" || clnname == "Credit" || clnname == "Cumilative")
                                            {
                                                sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;color:" + clr + ";'>" + Convert.ToDecimal(row[column.ColumnName]).ToString("#,##,###0.00") + "</td>");
                                            }
                                            else
                                            {
                                                sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right'>" + row[column.ColumnName].ToString() + "</td>");
                                                clr = "";
                                            }

                                        }
                                    }
                                    sb.Append("</tr>");
                                }

                                //Adding DataRow.

                                foreach (DataRow row in ds.Tables[1].Rows)
                                {
                                    sb.Append("<tr>");
                                    foreach (DataColumn column in ds.Tables[1].Columns)
                                    {
                                        if (row[column.ColumnName].ToString().Contains("-"))
                                            sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;background-color:#c5eafa;color:red;'>" + row[column.ColumnName].ToString() + "</td>");
                                        else if (column.ColumnName == "value" || column.ColumnName == "BalanceTotal" || column.ColumnName == "Total")
                                        {
                                            sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;background-color:#c5eafa;'>" + Convert.ToDecimal(row[column.ColumnName]).ToString("#,##,###0.00") + "</td>");
                                        }
                                        else
                                            sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;background-color:#c5eafa;'>" + row[column.ColumnName].ToString() + "</td>");
                                    }
                                    sb.Append("</tr>");
                                }

                                //Table end.
                                sb.Append("</table>");
                            }
                            
                        }
                        //tenure end
                    }
                    else
                    {
                        //non tenure starts
                        if (ds.Tables[2].Rows.Count > 0)
                        {

                            //Table start.
                            sb.Append("<table class='table table-striped datatable' cellpadding='5' cellspacing='0' style='border: 1px solid #ccc;font-size: 9pt;font-family:Arial'>");

                            //Adding HeaderRow.
                            sb.Append("<tr>");
                            foreach (DataColumn column in ds.Tables[2].Columns)
                            {
                                sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>" + column.ColumnName + "</th>");
                            }
                            sb.Append("</tr>");

                            var clr = "";
                            //Adding DataRow.
                            foreach (DataRow row in ds.Tables[2].Rows)
                            {
                                sb.Append("<tr>");
                                foreach (DataColumn column in ds.Tables[2].Columns)
                                {

                                    var clnname = column.ColumnName;
                                    if (row[column.ColumnName].ToString().Contains("-"))
                                    {
                                        sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;color:red;'>" + row[column.ColumnName].ToString() + "</td>");
                                    }
                                    else if (row[column.ColumnName].ToString().Contains("Dividend"))
                                    {
                                        clr = "red";
                                        sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;color:" + clr + ";'>" + row[column.ColumnName].ToString() + "</td>");
                                    }
                                    else
                                    {
                                        if (clnname == "Debit" || clnname == "Credit" || clnname == "Cumilative")
                                        {
                                            sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;color:" + clr + ";'>" + Convert.ToDecimal(row[column.ColumnName]).ToString("#,##,###0.00") + "</td>");
                                        }
                                        else
                                        {
                                            sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right'>" + row[column.ColumnName].ToString() + "</td>");
                                            clr = "";
                                        }

                                    }
                                }
                                sb.Append("</tr>");
                            }

                            //Adding DataRow.

                            if (tenure == 1)
                            {
                                foreach (DataRow row in ds.Tables[1].Rows)
                                {
                                    sb.Append("<tr>");
                                    foreach (DataColumn column in ds.Tables[1].Columns)
                                    {
                                        if (row[column.ColumnName].ToString().Contains("-"))
                                            sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;background-color:#c5eafa;color:red;'>" + row[column.ColumnName].ToString() + "</td>");
                                        else if (column.ColumnName == "value" || column.ColumnName == "BalanceTotal" || column.ColumnName == "Total")
                                        {
                                            sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;background-color:#c5eafa;'>" + Convert.ToDecimal(row[column.ColumnName]).ToString("#,##,###0.00") + "</td>");
                                        }
                                        else
                                            sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;background-color:#c5eafa;'>" + row[column.ColumnName].ToString() + "</td>");
                                    }
                                    sb.Append("</tr>");
                                }

                            }
                            else
                            {
                                foreach (DataRow row in ds.Tables[3].Rows)
                                {
                                    sb.Append("<tr>");
                                    foreach (DataColumn column in ds.Tables[3].Columns)
                                    {
                                        if (row[column.ColumnName].ToString().Contains("-"))
                                            sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;background-color:#c5eafa;color:red;'>" + row[column.ColumnName].ToString() + "</td>");
                                        else if (column.ColumnName == "value" || column.ColumnName == "Total")
                                        {
                                            sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;background-color:#c5eafa;'>" + Convert.ToDecimal(row[column.ColumnName]).ToString("#,##,###0.00") + "</td>");
                                        }
                                        else if (column.ColumnName == "BalanceTotal")
                                        {

                                        }
                                        else
                                            sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;background-color:#c5eafa;'>" + row[column.ColumnName].ToString() + "</td>");
                                    }
                                    sb.Append("</tr>");
                                }

                            }

                            //Table end.
                            sb.Append("</table>");

                        }


                        //non tenure end

                    }



                    html0 = html0.Append("<table class='table table-striped datatable'><tr><td valign='top'>");
                    html0 = html0.Append(html1);
                    html0 = html0.Append("</td><td valign='top'>");
                    html0 = html0.Append(html2);
                    html0 = html0.Append("</td><td valign='top'>");
                    html0 = html0.Append(html3);
                    html0 = html0.Append("</td></tr></table>");
                    html0 = html0.Append(sb);
                    return html0.ToString();
                }
            }
            catch (Exception ex)
            {
                string status = "error";
                return status;
            }

        }
        #endregion
        
        #region Delete Record  
        //[Authorize(Roles = "LoanSubscriptionManagementDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check(fld, id, "LoanSubscription");
            
            if (temp.Equals("PROCEED"))
            {
                LoanSubscription loansubs = context.loansubscription.Find(Convert.ToInt32(id));
                context.loansubscription.Remove(loansubs);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion

        #region QrCode Print
        public void PrintQRCode()
        {
            string SubsID = Request.Form.Get("QRSubsID");
            string ProdType = Request.Form.Get("ProdType");
            //string baseurl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToString();
            string baseurl = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath;
            int subsid = 0;
            int prodtype = 0;
            if (SubsID != "" && SubsID != "undefined")
                subsid = Convert.ToInt32(SubsID);
            if (ProdType != "" && ProdType != "undefined")
                prodtype = Convert.ToInt32(ProdType);

            int compyid = Convert.ToInt32(Session["compyid"]);
            string query = "";

            //query = "exec [sp_Print_QRCode_Dtl] @baseurl = '" + baseurl + "', @subsid=" + subsid;
            query = "exec [sp_Print_QRCode_Dtl] @baseurl = '" + baseurl + "', @prodtype=" + prodtype + ", @subsid=" + subsid;


            String constring = ConfigurationManager.ConnectionStrings["IdentityCon"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

            ReportDocument cryRpt = new ReportDocument();
            TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
            TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
            ConnectionInfo crConnectionInfo = new ConnectionInfo();
            CrystalDecisions.CrystalReports.Engine.Tables CrTables;

            cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "QRCode.Rpt");


            cryRpt.SetParameterValue("@baseurl", baseurl);
            cryRpt.SetParameterValue("@subsid", subsid);
            cryRpt.SetParameterValue("@prodtype", prodtype);
            crConnectionInfo.ServerName = stringbuilder.DataSource;
            crConnectionInfo.DatabaseName = stringbuilder.InitialCatalog;
            crConnectionInfo.UserID = stringbuilder.UserID;// "ftec";
            crConnectionInfo.Password = stringbuilder.Password; //"ftec";

            CrTables = cryRpt.Database.Tables;
            foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
            {
                crtableLogoninfo = CrTable.LogOnInfo;
                crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                CrTable.ApplyLogOnInfo(crtableLogoninfo);
            }


            cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
            cryRpt.Dispose();
            cryRpt.Close();
            GC.Collect();
            stringbuilder.Clear();
        }
        #endregion
    }
}
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

namespace CHITERP.Controllers.TransactionPayment
{
    public class TransactionPaymentController : Controller
    {
        // GET: TransactionPayment
        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        //[Authorize(Roles = "TransactionPaymentIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            if (string.IsNullOrEmpty(Session["SDATE"] as string))
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
            Session["ProductType"] = 0;
            if (string.IsNullOrEmpty(Session["ProductType"] as string))
            {
                if (Request.Form.Get("ProductType") != null && Request.Form.Get("ProductType") != "")
                {
                    Session["ProductType"] = Request.Form.Get("ProductType");
                }
            }
            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Product', ''").ToList();
            ViewBag.ProductType = new SelectList(mtqry.Where(x => x.Type == "Chit Fund" || x.Type == "Loan"), "TypeID", "Type", Convert.ToInt32(Session["ProductType"])).ToList();

            return View();
        }
        #endregion

        #region Edit
        //[Authorize(Roles = "TransactionPaymentEdit")]
        public void Edit(string id = "0")
        {

            if (id != "")
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/TransactionPayment/Form/" + id);

                //Response.Redirect("/TransactionPayment/Form/?id=" + id);
            }
        }
        #endregion

        #region Form
        //[Authorize(Roles = "TransactionPaymentCreate")]
        public ActionResult Form(string id = "0")
        {

            int TransactionID = 0;

            string[] param = id.Split('~');
            string Category = "P";
            if (param.Length > 1)
            {
                if (param[0] != null && param[0] != "")
                    Category = param[0];

                if (param[1] != null && param[1] != "")
                    TransactionID = Convert.ToInt32(param[1]);
            }
            else
            { TransactionID = Convert.ToInt32(id); }
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            Category = "P";
            TransactionPaymentList tab = new TransactionPaymentList();


            var clist1 = new SelectList(context.clientmasters.Where(d => d.DISPSTATUS == 0), "CLIENTID", "CLIENTNAME").ToList();
            ViewBag.PersonID = new SelectList("");// clist1;

            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Product', ''").ToList();
            ViewBag.ProductTypeTypeID = new SelectList(mtqry.Where(x => x.Type == "Chit Fund" || x.Type == "Loan"), "TypeID", "Type").ToList();

            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Transaction Type', ''").ToList();
            ViewBag.TransactionType = new SelectList(mtqry, "TypeID", "Type").ToList();

            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Transaction Mode', ''").ToList();
            ViewBag.PaymentMode = new SelectList(mtqry, "TypeID", "Type").ToList();

            var amtcdqry2 = context.Database.SqlQuery<CompanyFundAccount>("Select * From CompanyFundAccount (nolock)").ToList();

            ViewBag.FundAccountID = new SelectList(amtcdqry2, "FundAccountID", "AccountName");

            var amtcdqry3 = context.Database.SqlQuery<CompanyOffice>("Select * From CompanyOffice (nolock)").ToList();
            ViewBag.LinkedOfficeID = new SelectList(amtcdqry3, "officeid", "OfficeName");


            string trntypqry = "exec pr_Get_ProductCategory @usrid='" + Session["CUSRID"].ToString() + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod=" + tab.ProductTypeTypeID + ", @trncateg='" + Category + "'";
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>(trntypqry).ToList();
            ViewBag.SubsTranCategory = new SelectList(mtqry, "TypeID", "Type", tab.TransactionCategoryTypeID).ToList();
            ViewBag.TransferToProductID = new SelectList("");
            ViewBag.TransferToCategory = new SelectList("");
            ViewBag.TransferToSubscriptionID = new SelectList("");
            ViewBag.TransferToFundAccountID = new SelectList("");
            ViewBag.CollectionTypeID = new SelectList("");

            ViewBag.CollectionModeTypeID = new SelectList("");
            ViewBag.RouteID = new SelectList("");
            ViewBag.CollectionIntervalTypeID = new SelectList("");
            ViewBag.CoApplicants = new SelectList("");
            ViewBag.NomineeRelationshipTypeID = new SelectList("");

            ViewBag.DeductionTypeId = new SelectList("");
            ViewBag.TermTypeID = new SelectList("");
            ViewBag.CommissionAgentPersonID = new SelectList("");
            ViewBag.AccountEmployeeID = new SelectList("");


            ViewBag.LoanSubsSchemeID = new SelectList("");
            ViewBag.LoanSubsSecuredTypeId = new SelectList("");
            ViewBag.SubsRepaymentModeTypeID = new SelectList("");
            ViewBag.SubsLinkedOfficeID = new SelectList("");
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Transaction Management', 'Status'").ToList();
            ViewBag.StatusTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();
            string crole = Convert.ToString(Session["Group"]);
            string cusrid = Convert.ToString(Session["CUSRID"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                cusrid = "";
            }
            trntypqry = "exec pr_Get_Subscription_Info @usrid='" + cusrid + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod=29";
            var subsqry = context.Database.SqlQuery<pr_Get_Subscription_Info_Result>(trntypqry).ToList();
            ViewBag.SubscriptionID = new SelectList(subsqry, "subscriptionid", "subscriptioncode").ToList();

            List<SelectListItem> selTrnCatg = new List<SelectListItem>();
            SelectListItem selTrnCatgItm = new SelectListItem { Text = "Payment", Value = "P", Selected = true };
            selTrnCatg.Add(selTrnCatgItm);
            //selTrnCatgItm = new SelectListItem { Text = "Collection", Value = "C", Selected = false };
            //selTrnCatg.Add(selTrnCatgItm);
            ViewBag.TransactionCategory = selTrnCatg;
            ViewBag.hiddentrandid = "0";
            ViewBag.PaymentFiles = null;
            if (TransactionID > 0)
            {
                tab = context.trnpymtlst.Find(TransactionID);
                ViewBag.hiddentrandid = TransactionID;
                
                if (!System.IO.Directory.Exists(Server.MapPath("~/PaymentFiles/" + TransactionID)))
                    Directory.CreateDirectory(Server.MapPath("~/PaymentFiles/" + TransactionID));

                var dir = new DirectoryInfo(Server.MapPath("~/PaymentFiles/" + TransactionID));

                var files = dir.EnumerateFiles().Select(f => f.Name);
                ViewBag.PaymentFiles = files;
                ViewBag.TranID = TransactionID;
                ChitSubscription CHTS = new ChitSubscription();
                LoanSubscription LS = new LoanSubscription();
                LoanRequest LRS = new LoanRequest();
                if (tab.ProductTypeTypeID == 29)
                {
                    CHTS = context.chitsubscriptions.Find(tab.ProductID);
                    clist1 = new SelectList(context.clientmasters.Where(d => d.DISPSTATUS == 0 && d.CLIENTID == CHTS.PersonID), "CLIENTID", "CLIENTNAME").ToList();
                }
                else if (tab.ProductTypeTypeID == 30)
                {
                    LS = context.loansubscription.Find(tab.ProductID);
                    LRS = context.loanrequest.Find(LS.LoanRequestID);
                    clist1 = new SelectList(context.clientmasters.Where(d => d.DISPSTATUS == 0 && d.CLIENTID == LRS.PersonID), "CLIENTID", "CLIENTNAME").ToList();
                }



                ViewBag.PersonID = clist1;

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Product', ''").ToList();
                ViewBag.ProductTypeTypeID = new SelectList(mtqry.Where(x => x.Type == "Chit Fund" || x.Type == "Loan"), "TypeID", "Type", tab.ProductTypeTypeID).ToList();
                //ViewBag.ProductTypeTypeID = new SelectList(mtqry.Where(x => x.Type == "Chit Fund" || x.Type == "Loan"), "TypeID", "Type", tab.ProductTypeID).ToList();
                ViewBag.ProductTypeTypeID = new SelectList(mtqry.Where(x => x.TypeID == tab.ProductTypeTypeID), "TypeID", "Type", tab.ProductTypeTypeID).ToList();

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Transaction Type', ''", tab.TransactionTypeTypeID).ToList();
                ViewBag.TransactionType = new SelectList(mtqry, "TypeID", "Type", tab.TransactionTypeTypeID).ToList();

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Transaction Mode', ''", tab.TransactionModeTypeID).ToList();
                ViewBag.PaymentMode = new SelectList(mtqry, "TypeID", "Type", tab.TransactionModeTypeID).ToList();

                amtcdqry2 = context.Database.SqlQuery<CompanyFundAccount>("Select * From CompanyFundAccount (nolock)").ToList();

                ViewBag.FundAccountID = new SelectList(amtcdqry2, "FundAccountID", "AccountName", tab.FundAccountID);

                //amtcdqry3 = context.Database.SqlQuery<CompanyOffice>("Select * From CompanyOffice (nolock)").ToList();
                //ViewBag.LinkedOfficeID = new SelectList(amtcdqry3, "officeid", "OfficeName");


                trntypqry = "exec pr_Get_ProductCategory @usrid='" + Session["CUSRID"].ToString() + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod=" + tab.ProductTypeTypeID + ", @trncateg='" + Category + "'";
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>(trntypqry).ToList();
                ViewBag.SubsTranCategory = new SelectList(mtqry, "TypeID", "Type", tab.TransactionCategoryTypeID).ToList();
                ViewBag.TransferToProductID = new SelectList("");
                ViewBag.TransferToCategory = new SelectList("");
                ViewBag.TransferToSubscriptionID = new SelectList("");
                ViewBag.TransferToFundAccountID = new SelectList("");
                ViewBag.CollectionTypeID = new SelectList("");

                ViewBag.CollectionModeTypeID = new SelectList("");
                ViewBag.RouteID = new SelectList("");
                ViewBag.CollectionIntervalTypeID = new SelectList("");
                ViewBag.CoApplicants = new SelectList("");
                ViewBag.NomineeRelationshipTypeID = new SelectList("");

                ViewBag.DeductionTypeId = new SelectList("");
                ViewBag.TermTypeID = new SelectList("");
                ViewBag.CommissionAgentPersonID = new SelectList("");
                ViewBag.AccountEmployeeID = new SelectList("");


                ViewBag.LoanSubsSchemeID = new SelectList("");
                ViewBag.LoanSubsSecuredTypeId = new SelectList("");
                ViewBag.SubsRepaymentModeTypeID = new SelectList("");
                ViewBag.SubsLinkedOfficeID = new SelectList("");
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Transaction Management', 'Status'").ToList();
                ViewBag.StatusTypeID = new SelectList(mtqry, "TypeID", "Type", tab.StatusTypeID).ToList();
                //ViewBag.StatusTypeID = new SelectList(mtqry.Where(x => x.TypeID >= tab.StatusTypeID), "TypeID", "Type", tab.StatusTypeID).ToList();
                //ViewBag.StatusTypeID = new SelectList(mtqry.Where(x => (x.TypeID >= tab.StatusTypeID && tab.StatusTypeID >= 1034) || tab.StatusTypeID < 1034), "TypeID", "Type", tab.StatusTypeID).ToList();

                trntypqry = "exec pr_Get_Subscription_Info @usrid='" + cusrid + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod=" + tab.ProductTypeTypeID;
                subsqry = context.Database.SqlQuery<pr_Get_Subscription_Info_Result>(trntypqry).ToList();
                ViewBag.SubscriptionID = new SelectList(subsqry.Where(X => X.subscriptionid == tab.ProductID), "subscriptionid", "subscriptioncode", tab.ProductID).ToList();

            }
            else
            {
                tab = null;
            }

            return View(tab);
        }
        #endregion

        #region Save Client Attachments

        [HttpPost]
        public ActionResult SavePaymentFiles(string ids)//FormCollection fileData, HttpPostedFileBase logofilesupload)
        {
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0 || Session["CUSRID"].ToString() == "") { return RedirectToAction("Login", "Account"); } //  Response.Redirect(Url.Action("Login", "Account"));
            else
            {
                string status = "";
                #region Client Files Save
                int CLIENTID = 0;
                string clientid = Convert.ToString(ids);
                try
                {
                    if (clientid != "" && clientid != "0" && clientid != null && clientid != "undefined")
                    {
                        CLIENTID = Convert.ToInt32(clientid);
                    }
                    if (Request.Files.Count > 0 && CLIENTID > 0) //if (logofilesupload != null && CLIENTID>0)
                    {
                        status = "Success";

                        /*Lopp for multiple files*/
                        /*Geting the file name*/
                        if (!System.IO.Directory.Exists(Server.MapPath("~/PaymentFiles/" + CLIENTID)))
                            Directory.CreateDirectory(Server.MapPath("~/PaymentFiles/" + CLIENTID));

                        var dir = new DirectoryInfo(Server.MapPath("~/PaymentFiles/" + CLIENTID));

                        //string absolutePath = Server.MapPath(Server.MapPath("~/SRFfiles/logo/" + SRFMID));

                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase nfile = files[i];
                            /*Saving the file in server folder*/
                            //if (!System.IO.File.Exists(dir + "/" + nfile.FileName))
                            {
                                nfile.SaveAs(Server.MapPath("~/PaymentFiles/" + CLIENTID.ToString() + "/" + nfile.FileName));
                            }

                        }
                        status = "Success";


                    }
                }
                catch (Exception ex)
                {
                    var aa = ex.Message.ToString();
                    //Response.Redirect("Error");
                    status = "FileError";
                }
                #endregion
                return Json(status, JsonRequestBehavior.AllowGet);
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


                            string TransactionCategory = "P";// Convert.ToString(formcoll["TransactionCategory"]);
                            if (TransactionCategory == "P")
                            {

                                TransactionPaymentList master = new TransactionPaymentList();
                                Int32 PaymentListID = 0;

                                string paymentListID = Convert.ToString(formcoll["PaymentListID"]);
                                if (paymentListID == "" || paymentListID == null)
                                {
                                    PaymentListID = 0;
                                }
                                else
                                {
                                    PaymentListID = Convert.ToInt32(paymentListID);
                                }

                                if (PaymentListID != 0)
                                {
                                    master = context.trnpymtlst.Find(PaymentListID);
                                }
                                //master.PaymentListID = PaymentListID;
                                if (formcoll["ProductTypeTypeID"] == "" || formcoll["ProductTypeTypeID"] == null)
                                    master.ProductTypeTypeID = 0;
                                else
                                    master.ProductTypeTypeID = Convert.ToInt32(formcoll["ProductTypeTypeID"]);
                                if (formcoll["SubscriptionID"] == "" || formcoll["SubscriptionID"] == null)
                                    master.ProductID = 0;
                                else
                                    master.ProductID = Convert.ToInt32(formcoll["SubscriptionID"]);
                                int trantypeid = 0;
                                if (formcoll["TransactionType"] == "" || formcoll["TransactionType"] == null)
                                {
                                    if (master.ProductTypeTypeID == 29)
                                    {
                                        ChitSubscription chts = new ChitSubscription();
                                        chts = context.chitsubscriptions.Find(master.ProductID);
                                        trantypeid = Convert.ToInt32(chts.CollectionTypeID);
                                    }
                                    else if (master.ProductTypeTypeID == 30)
                                    {
                                        LoanSubscription ls = new LoanSubscription();
                                        ls = context.loansubscription.Find(master.ProductID);
                                        trantypeid = Convert.ToInt32(ls.CollectionType);
                                    }
                                    else
                                    {
                                        trantypeid = 342;
                                    }
                                    master.TransactionTypeTypeID = trantypeid;
                                }
                                else
                                    master.TransactionTypeTypeID = Convert.ToInt32(formcoll["TransactionType"]);

                                if (formcoll["SubsTranCategory"] == "" || formcoll["SubsTranCategory"] == null)
                                    master.TransactionCategoryTypeID = 0;
                                else
                                    master.TransactionCategoryTypeID = Convert.ToInt32(formcoll["SubsTranCategory"]);



                                master.TransactionModeTypeID = Convert.ToInt32(formcoll["PaymentMode"]);

                                master.Remarks = Convert.ToString(formcoll["Remarks"]);

                                if (formcoll["GenerratedDate"] != "" && formcoll["GenerratedDate"] != null)
                                    master.GenerratedDate = Convert.ToDateTime(formcoll["GenerratedDate"]);
                                else
                                    master.GenerratedDate = DateTime.Now.Date;

                                if (formcoll["Installment"] == "" || formcoll["Installment"] == null)
                                    master.Installment = 0;
                                else
                                    master.Installment = Convert.ToInt32(formcoll["Installment"]);

                                if (formcoll["StatusTypeID"] == "" || formcoll["StatusTypeID"] == null)
                                    master.StatusTypeID = 0;
                                else
                                    master.StatusTypeID = Convert.ToInt32(formcoll["StatusTypeID"]);

                                if (formcoll["GeneratedAmount"] == "" || formcoll["GeneratedAmount"] == null)
                                    master.GeneratedAmount = 0;
                                else
                                    master.GeneratedAmount = Convert.ToDouble(formcoll["GeneratedAmount"]);

                                master.ChequeDate = null;
                                if (formcoll["ChequeDate"] != "" && formcoll["ChequeDate"] != null)
                                    master.ChequeDate = Convert.ToDateTime(formcoll["ChequeDate"]);
                                master.ChequeNo = null;
                                if (formcoll["ChequeNo"] != "" && formcoll["ChequeNo"] != null)
                                    master.ChequeNo = Convert.ToString(formcoll["ChequeNo"]);
                                master.ChequeBankName = null;
                                if (formcoll["ChequeBankName"] != "" && formcoll["ChequeBankName"] != null)
                                    master.ChequeBankName = Convert.ToString(formcoll["ChequeBankName"]);
                                master.ChequeRealizedDate = null;
                                if (formcoll["ChequeRealizedDate"] != "" && formcoll["ChequeRealizedDate"] != null)
                                    master.ChequeRealizedDate = Convert.ToDateTime(formcoll["ChequeRealizedDate"]);
                                master.FundAccountID = null;
                                if (formcoll["FundAccountID"] != "" && formcoll["FundAccountID"] != null)
                                    master.FundAccountID = Convert.ToInt32(formcoll["FundAccountID"]);

                                master.GSTReq = 0;
                                if (formcoll["GSTReq"] != null && formcoll["GSTReq"] != "")
                                    master.GSTReq = Convert.ToInt32(formcoll["GSTReq"]);
                                if (formcoll["GSTPerc"] == "" || formcoll["GSTPerc"] == null)
                                    master.GSTPerc = 0;
                                else
                                    master.GSTPerc = Convert.ToDecimal(formcoll["GSTPerc"]);
                                if (formcoll["GSTAmt"] == "" || formcoll["GSTAmt"] == null)
                                    master.GSTAmt = 0;
                                else
                                    master.GSTAmt = Convert.ToDecimal(formcoll["GSTAmt"]);
                                if (formcoll["GrossAmt"] == "" || formcoll["GrossAmt"] == null)
                                    master.GrossAmt = 0;
                                else
                                    master.GrossAmt = Convert.ToDecimal(formcoll["GrossAmt"]);

                                if (PaymentListID == 0)
                                {
                                    //string squery = "SELECT *FROM ChitSubscription (nolock) WHERE ChitSubscriptionCode ='" + master.ChitSubscriptionCode + "' and personid='" + master.PersonID+ "'";
                                    //var query = context.Database.SqlQuery<ChitSubscription>(squery).ToList();

                                    //if (query.Count != 0)
                                    //{
                                    //    status = "Existing";
                                    //}
                                    //else
                                    {
                                        //master.CreatedBy = Session["CUSRID"].ToString();
                                        //master.CreatedDt = DateTime.Now;

                                        context.trnpymtlst.Add(master);
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
                                PaymentListID = master.PaymentListID;

                                string updqry = "exec pr_Update_Payment_Allocation @paymtlstid = " + master.PaymentListID + ",@status =" + master.StatusTypeID;
                                context.Database.ExecuteSqlCommand(updqry);

                                ViewBag.Message = "Form submitted.";
                                // Response.Redirect("Index");
                                //return RedirectToAction("Index", "ChitSubscriptionManagement");
                                //Response.Write("Update");

                            }
                            else
                            {
                                status = "Error";

                            }
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            var ermsg = ex.Message;
                            //Response.Write(ex.Message.ToString());
                            trans1.Rollback();
                            //Response.Redirect("/Error/AccessDenied");
                            //return RedirectToAction("Index", "ChitSubscriptionManagement");
                            status = "Error";
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
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
                    int cempid = Convert.ToInt32(Session["EMPLID"]);
                    if (crole == "SuperAdmin" || crole == "Admin")
                    {
                        cusrid = "";
                    }
                    int prodtype = Convert.ToInt32(Session["ProductType"]);

                    var data = e.pr_Search_Transaction_Payment_Details(param.sSearch,
                                                    Convert.ToInt32(Request["iSortCol_0"]),
                                                    Request["sSortDir_0"],
                                                    param.iDisplayStart,
                                                    param.iDisplayStart + param.iDisplayLength,
                                                    totalRowsCount,
                                                    filteredRowsCount, Convert.ToString(cusrid), Convert.ToInt32(cempid), Convert.ToInt32(prodtype), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                    var aaData = data.Select(d => new
                    {
                        CLIENTNAME = d.CLIENTNAME,
                        SUBSCRIPTIONID = d.SUBSCRIPTIONID.ToString(),
                        PRODUCTTYPE = d.PRODUCTTYPE,
                        TRANCATEGORY = d.TRANCATEGORY,
                        TRANSUBCATEGORY = d.TRANSUBCATEGORY,
                        STATUDESC = d.STATUDESC,
                        TRANMODE = d.TRANMODE,
                        AMOUNT = d.AMOUNT.ToString(),
                        PaymentDt = d.PaymentDt,
                        TRANID = d.TRANID.ToString(),
                        SUBSCRIPTIONCODE = d.SUBSCRIPTIONCODE
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


        #region Get Product Subscriptions 
        public JsonResult GetProductSubsDtl(string id)
        {
            string[] param = id.Split('~');
            int prodtype = 29;
            string trancateg = "R";
            if (param.Length > 0)
            {
                if (param[0] != "" && param[0] != null)
                {
                    trancateg = param[0];
                }
                if (param[1] != "" && param[1] != null)
                {
                    prodtype = Convert.ToInt32(param[1]);
                }
            }
            string crole = Convert.ToString(Session["Group"]);
            string cusrid = Convert.ToString(Session["CUSRID"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                cusrid = "";
            }
            string trntypqry = "exec pr_Get_Subscription_Info @usrid='" + cusrid + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod=" + prodtype;
            var subsqry = context.Database.SqlQuery<pr_Get_Subscription_Info_Result>(trntypqry).ToList();
            //ViewBag.SubscriptionID = new SelectList(subsqry, "subscriptionid", "subscriptioncode").ToList();
            return Json(subsqry, JsonRequestBehavior.AllowGet);

        }
        #endregion


        #region Get Product Information 
        public JsonResult GetProductDtl(string id)
        {
            string[] param = id.Split('~');
            int prodid = 0;
            string trancateg = "R";
            if (param.Length > 0)
            {
                if (param[0] != "" && param[0] != null)
                {
                    trancateg = param[0];
                }
                if (param[1] != "" && param[1] != null)
                {
                    prodid = Convert.ToInt32(param[1]);
                }
            }
            string trntypqry = "exec pr_Get_Transaction_Dtl @categ='" + trancateg + "',@id=" + Convert.ToInt32(prodid);
            var subsqry = context.Database.SqlQuery<pr_Get_Transaction_Dtl_Result>(trntypqry).ToList();
            //ViewBag.SubscriptionID = new SelectList(subsqry, "subscriptionid", "subscriptioncode").ToList();
            return Json(subsqry, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Get Product Sub Transaction Types
        public JsonResult GetProductSubTrnTypDtl(string id)
        {
            string[] param = id.Split('~');
            int prodtype = 29;
            string trancateg = "R";
            if (param.Length > 0)
            {
                if (param[0] != "" && param[0] != null)
                {
                    trancateg = param[0];
                }
                if (param[1] != "" && param[1] != null)
                {
                    prodtype = Convert.ToInt32(param[1]);
                }
            }
            trancateg = "P";
            string trntypqry = "exec pr_Get_ProductCategory @usrid='" + Session["CUSRID"].ToString() + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod=" + prodtype + ", @trncateg='" + trancateg + "'";
            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>(trntypqry).ToList();
            //ViewBag.SubsTranCategory = new SelectList(mtqry, "TypeID", "Type").ToList();

            return Json(mtqry, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Delete Record  
        //[Authorize(Roles = "TransactionPaymentDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check("TransactionPaymentList", id, "TransactionPaymentList");

            if (temp.Equals("PROCEED"))
            {
                TransactionPaymentList tranpymtlist = context.trnpymtlst.Find(Convert.ToInt32(id));
                string dqy = "Delete A From TransactionPaymentAllocation A join TransactionPayment B(nolock) on a.PaymentID = b.PaymentID Where b.PaymentListID = " + Convert.ToInt32(tranpymtlist.PaymentListID) + "";
                context.Database.ExecuteSqlCommand(dqy);
                dqy = "Delete from TransactionPayment Where PaymentListID = " + Convert.ToInt32(tranpymtlist.PaymentListID) + "";
                context.Database.ExecuteSqlCommand(dqy);
                context.trnpymtlst.Remove(tranpymtlist);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion
    }
}
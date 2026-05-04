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

namespace CHITERP.Controllers.TransactionBookVerification
{
    public class TransactionBookVerificationController : Controller
    {
        // GET: TransactionBookVerification
        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        [Authorize(Roles = "TransactionBookVerificationIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            //if (string.IsNullOrEmpty(Session["SDATE"] as string))
            //{

            //    Session["SDATE"] = DateTime.Now.ToString("yyyy-MM-dd");
            //    Session["EDATE"] = DateTime.Now.ToString("yyyy-MM-dd");
            //}
            //else
            //{
            //    if (Request.Form.Get("from") != null)
            //    {
            //        Session["SDATE"] = Request.Form.Get("from");
            //        Session["EDATE"] = Request.Form.Get("to");
            //    }
            //}
            if (Request.Form.Get("from") != null && Request.Form.Get("from") != "")
            {
                Session["SDATE"] = Request.Form.Get("from");
                Session["EDATE"] = Request.Form.Get("to");
            }
            else
            {
                Session["SDATE"] = DateTime.Now.ToString("yyyy-MM-dd");
                Session["EDATE"] = DateTime.Now.ToString("yyyy-MM-dd");
            }
            if (Request.Form.Get("ProductType") != null && Request.Form.Get("ProductType") != "")
            {
                Session["ProductType"] = Request.Form.Get("ProductType");
            }
            else
                Session["ProductType"] = 0;

            if (Request.Form.Get("RouteID") != null && Request.Form.Get("RouteID") != "")
            {
                Session["RouteID"] = Request.Form.Get("RouteID");
            }
            else
                Session["RouteID"] = 0;

            if (Request.Form.Get("CollectionAgent") != null && Request.Form.Get("CollectionAgent") != "")
            {
                Session["CollectionAgent"] = Request.Form.Get("CollectionAgent");
            }
            else
                Session["CollectionAgent"] = 0;
            if (Request.Form.Get("BookVerifiedBy") != null && Request.Form.Get("BookVerifiedBy") != "")
            {
                Session["BookVerifiedBy"] = Request.Form.Get("BookVerifiedBy");
            }
            else
                Session["BookVerifiedBy"] = 0;


            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Product', ''").ToList();
            ViewBag.ProductType = new SelectList(mtqry.Where(x => x.Type == "Chit Fund" || x.Type == "Loan"), "TypeID", "Type", Convert.ToInt32(Session["ProductType"])).ToList();
            var amtcdqry4 = context.Database.SqlQuery<CompanyRoute>("select* from CompanyRoute (nolock)").ToList();
            ViewBag.RouteID = new SelectList(amtcdqry4, "RouteID", "RouteName", Convert.ToInt32(Session["RouteID"]));

            string cusrid = Convert.ToString(Session["CUSRID"]);
            string crole = Convert.ToString(Session["Group"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                cusrid = "";
            }
            var empqry = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_Get_Employee_Dtl_Rpt @usrid='" + cusrid + "',@desgnid=0").ToList();
            ViewBag.CollectionAgent = new SelectList(empqry, "DValue", "DText", Convert.ToInt32(Session["CollectionAgent"])).ToList();
            empqry = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_Get_Employee_Dtl_Rpt @usrid='" + cusrid + "',@desgnid=1").ToList();
            ViewBag.BookVerifiedBy = new SelectList(empqry, "DValue", "DText", Convert.ToInt32(Session["BookVerifiedBy"])).ToList();

            return View();
        }
        #endregion

        #region Edit
        //[Authorize(Roles = "TransactionBookVerificationEdit")]
        public void Edit(string id = "0")
        {

            if (id != "")
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/TransactionBookVerification/Form/" + id);

                //Response.Redirect("/TransactionBookVerification/Form/?id=" + id);
            }
        }
        #endregion

        #region Form
        //[Authorize(Roles = "TransactionBookVerificationCreate")]
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
            TransactionBookVerificationList tab = new TransactionBookVerificationList();
            TranBookVrfcnList bvl = new TranBookVrfcnList();

            var clist1 = new SelectList(context.clientmasters.Where(d => d.DISPSTATUS == 0), "CLIENTID", "CLIENTNAME").ToList();
            ViewBag.PersonID = new SelectList("");// clist1;

            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Product', ''").ToList();
            ViewBag.ProductTypeID = new SelectList(mtqry.Where(x => x.Type == "Chit Fund" || x.Type == "Loan"), "TypeID", "Type").ToList();

            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Book Verification Finding', 'Status'").ToList();
            ViewBag.F_BookVerificationFindingID = new SelectList(mtqry.Where(x => x.TypeID != 1103), "TypeID", "Type").ToList();

            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Transaction Mode', ''").ToList();
            ViewBag.PaymentMode = new SelectList(mtqry, "TypeID", "Type").ToList();

            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_GST_Applicability_Flags ").ToList();
            ViewBag.GSTReq = new SelectList(mtqry, "TypeID", "Type").ToList();

            var amtcdqry2 = context.Database.SqlQuery<CompanyFundAccount>("Select * From CompanyFundAccount (nolock)").ToList();

            ViewBag.FundAccountID = new SelectList(amtcdqry2, "FundAccountID", "AccountName");

            var amtcdqry3 = context.Database.SqlQuery<CompanyOffice>("Select * From CompanyOffice (nolock)").ToList();
            ViewBag.LinkedOfficeID = new SelectList(amtcdqry3, "officeid", "OfficeName");


            //string trntypqry = "exec pr_Get_ProductCategory @usrid='" + Session["CUSRID"].ToString() + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod=" + tab.ProductTypeID + ", @trncateg='" + Category + "'";
            //mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>(trntypqry).ToList();
            //ViewBag.SubsTranCategory = new SelectList(mtqry, "TypeID", "Type", tab.TransactionCategoryTypeID).ToList();
            ViewBag.TransferToProductID = new SelectList("");
            ViewBag.TransferToCategory = new SelectList("");
            ViewBag.TransferToProductID = new SelectList("");
            ViewBag.TransferToFundAccountID = new SelectList("");
            ViewBag.CollectionTypeID = new SelectList("");
            ViewBag.F_OldReceiptID = new SelectList("");

            ViewBag.CollectionModeTypeID = new SelectList("");
            ViewBag.RouteID = new SelectList("");
            ViewBag.BookID = new SelectList("");

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
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Book Verification List', 'Status'").ToList();
            ViewBag.StatusTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Book Verification', 'Status'").ToList();
            ViewBag.BkVfcnStatusTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();

            string crole = Convert.ToString(Session["Group"]);
            string cusrid = Convert.ToString(Session["CUSRID"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                cusrid = "";
            }
            var trntypqry = "exec pr_Get_Subscription_Info @usrid='" + cusrid + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod=29";
            var subsqry = context.Database.SqlQuery<pr_Get_Subscription_Info_Result>(trntypqry).ToList();
            ViewBag.ProductID = new SelectList(subsqry, "subscriptionid", "subscriptioncode").ToList();

            List<SelectListItem> selTrnCatg = new List<SelectListItem>();
            SelectListItem selTrnCatgItm = new SelectListItem { Text = "Payment", Value = "P", Selected = true };
            selTrnCatg.Add(selTrnCatgItm);
            //selTrnCatgItm = new SelectListItem { Text = "Collection", Value = "C", Selected = false };
            //selTrnCatg.Add(selTrnCatgItm);
            ViewBag.TransactionCategory = selTrnCatg;
            ViewBag.hiddentrandid = "0";
            ViewBag.BookVerificationFiles = null;
            if (TransactionID > 0)
            {
                tab = context.trnbkvfcnlsts.Find(TransactionID);
                ViewBag.hiddentrandid = TransactionID;
                trntypqry = "exec pr_Get_Books_BV_Assign @bvlistid=" + tab.BookVerificationListID + ",@subsid=" + tab.ProductID + ", @prodtypid=" + tab.ProductTypeID + "";
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>(trntypqry).ToList();
                ViewBag.BookID = new SelectList(mtqry, "TypeID", "Type", tab.BookID).ToList();

                if (!System.IO.Directory.Exists(Server.MapPath("~/BookVerificationFiles/" + TransactionID)))
                    Directory.CreateDirectory(Server.MapPath("~/BookVerificationFiles/" + TransactionID));

                var dir = new DirectoryInfo(Server.MapPath("~/BookVerificationFiles/" + TransactionID));

                var files = dir.EnumerateFiles().Select(f => f.Name);
                ViewBag.BookVerificationFiles = files;
                ViewBag.TranID = TransactionID;
                ViewBag.TranBVLID = TransactionID;

                ChitSubscription CHTS = new ChitSubscription();
                LoanSubscription LS = new LoanSubscription();
                LoanRequest LRS = new LoanRequest();
                if (tab.ProductTypeID == 29)
                {
                    CHTS = context.chitsubscriptions.Find(tab.ProductID);
                    clist1 = new SelectList(context.clientmasters.Where(d => d.DISPSTATUS == 0 && d.CLIENTID == CHTS.PersonID), "CLIENTID", "CLIENTNAME").ToList();
                }
                else if (tab.ProductTypeID == 30)
                {
                    LS = context.loansubscription.Find(tab.ProductID);
                    LRS = context.loanrequest.Find(LS.LoanRequestID);
                    clist1 = new SelectList(context.clientmasters.Where(d => d.DISPSTATUS == 0 && d.CLIENTID == LRS.PersonID), "CLIENTID", "CLIENTNAME").ToList();
                }



                ViewBag.PersonID = clist1;

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Product', ''").ToList();
                ViewBag.ProductTypeID = new SelectList(mtqry.Where(x => x.Type == "Chit Fund" || x.Type == "Loan"), "TypeID", "Type", tab.ProductTypeID).ToList();
                //ViewBag.ProductTypeID = new SelectList(mtqry.Where(x => x.Type == "Chit Fund" || x.Type == "Loan"), "TypeID", "Type", tab.ProductTypeID).ToList();
                ViewBag.ProductTypeID = new SelectList(mtqry.Where(x => x.TypeID == tab.ProductTypeID), "TypeID", "Type", tab.ProductTypeID).ToList();

                //mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Transaction Type', ''", tab.TransactionTypeTypeID).ToList();
                //ViewBag.TransactionType = new SelectList(mtqry, "TypeID", "Type", tab.TransactionTypeTypeID).ToList();

                //mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Transaction Mode', ''", tab.TransactionModeTypeID).ToList();
                //ViewBag.PaymentMode = new SelectList(mtqry, "TypeID", "Type", tab.TransactionModeTypeID).ToList();
                //mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_GST_Applicability_Flags ").ToList();
                //ViewBag.GSTReq = new SelectList(mtqry, "TypeID", "Type", tab.GSTReq).ToList();
                //amtcdqry2 = context.Database.SqlQuery<CompanyFundAccount>("Select * From CompanyFundAccount (nolock)").ToList();

                //ViewBag.FundAccountID = new SelectList(amtcdqry2, "FundAccountID", "AccountName", tab.FundAccountID);

                //amtcdqry3 = context.Database.SqlQuery<CompanyOffice>("Select * From CompanyOffice (nolock)").ToList();
                //ViewBag.LinkedOfficeID = new SelectList(amtcdqry3, "officeid", "OfficeName");


                //trntypqry = "exec pr_Get_ProductCategory @usrid='" + Session["CUSRID"].ToString() + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod=" + tab.ProductTypeID + ", @trncateg='" + Category + "'";
                //mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>(trntypqry).ToList();
                //ViewBag.SubsTranCategory = new SelectList(mtqry, "TypeID", "Type", tab.TransactionCategoryTypeID).ToList();
                ViewBag.TransferToProductID = new SelectList("");
                ViewBag.TransferToCategory = new SelectList("");
                ViewBag.TransferToProductID = new SelectList("");
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
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Book Verification List', 'Status'").ToList();
                ViewBag.StatusTypeID = new SelectList(mtqry, "TypeID", "Type", tab.StatusTypeID).ToList();
                //ViewBag.StatusTypeID = new SelectList(mtqry.Where(x => x.TypeID >= tab.StatusTypeID), "TypeID", "Type", tab.StatusTypeID).ToList();
                //ViewBag.StatusTypeID = new SelectList(mtqry.Where(x => (x.TypeID >= tab.StatusTypeID && tab.StatusTypeID >= 1034) || tab.StatusTypeID < 1034), "TypeID", "Type", tab.StatusTypeID).ToList();

                trntypqry = "exec pr_Get_Subscription_Info @usrid='" + cusrid + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod=" + tab.ProductTypeID;
                subsqry = context.Database.SqlQuery<pr_Get_Subscription_Info_Result>(trntypqry).ToList();
                ViewBag.ProductID = new SelectList(subsqry.Where(X => X.subscriptionid == tab.ProductID), "subscriptionid", "subscriptioncode", tab.ProductID).ToList();

            }
            else
            {
                tab = null;
            }
            string tqry = "exec pr_Get_Book_Verification_Entry_Details @bkvfcnlstid=" + tab.BookVerificationListID + "";
            bvl.tranbookvfcnlst = context.trnbkvfcnlsts.Where(x => x.BookVerificationListID == tab.BookVerificationListID).ToList();
            bvl.tranbookentrylst = context.Database.SqlQuery<pr_Get_Book_Verification_Entry_Details_Result>(tqry).ToList();


            return View(bvl);
        }
        #endregion

        #region Save BV Attachments

        [HttpPost]
        public ActionResult SaveBookVerificationFiles(string ids)//FormCollection fileData, HttpPostedFileBase logofilesupload)
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
                        if (!System.IO.Directory.Exists(Server.MapPath("~/BookVerificationFiles/" + CLIENTID)))
                            Directory.CreateDirectory(Server.MapPath("~/BookVerificationFiles/" + CLIENTID));

                        var dir = new DirectoryInfo(Server.MapPath("~/BookVerificationFiles/" + CLIENTID));

                        //string absolutePath = Server.MapPath(Server.MapPath("~/SRFfiles/logo/" + SRFMID));

                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase nfile = files[i];
                            /*Saving the file in server folder*/
                            //if (!System.IO.File.Exists(dir + "/" + nfile.FileName))
                            {
                                nfile.SaveAs(Server.MapPath("~/BookVerificationFiles/" + CLIENTID.ToString() + "/" + nfile.FileName));
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

                            TransactionBookVerificationList master = new TransactionBookVerificationList();
                            TransBookVerification detail = new TransBookVerification();
                            int bkvfcnlistid = 0;
                            int bkvid = 0;

                            string bkvfclistid = Convert.ToString(formcoll["BookVerificationListID"]);
                            string bkvfcid = Convert.ToString(formcoll["F_BookVerificationID"]);

                            if (bkvfclistid == "" || bkvfclistid == null)
                            {
                                bkvfcnlistid = 0;
                            }
                            else
                            {
                                bkvfcnlistid = Convert.ToInt32(bkvfclistid);
                            }
                            if (bkvfcid == "" || bkvfcid == null)
                            {
                                bkvid = 0;
                            }
                            else
                            {
                                bkvid = Convert.ToInt32(bkvfcid);
                            }

                            if (bkvfcnlistid != 0)
                            {
                                master = context.trnbkvfcnlsts.Find(bkvfcnlistid);
                            }
                            if (bkvid != 0)
                            {
                                detail = context.trnbkvfcns.Find(bkvid);
                            }
                            master.BookVerificationListID = bkvfcnlistid;

                            if (formcoll["ProductTypeID"] == "" || formcoll["ProductTypeID"] == null)
                                master.ProductTypeID = 0;
                            else
                                master.ProductTypeID = Convert.ToInt32(formcoll["ProductTypeID"]);
                            if (formcoll["ProductID"] == "" || formcoll["ProductID"] == null)
                                master.ProductID = 0;
                            else
                                master.ProductID = Convert.ToInt32(formcoll["ProductID"]);
                            if (formcoll["BookID"] == "" || formcoll["BookID"] == null)
                                master.BookID = 0;
                            else
                                master.BookID = Convert.ToInt32(formcoll["BookID"]);


                            master.Remarks = Convert.ToString(formcoll["Remarks"]);

                            if (formcoll["GeneratedDate"] != "" && formcoll["GeneratedDate"] != null)
                                master.GeneratedDate = Convert.ToDateTime(formcoll["GeneratedDate"]);
                            else
                                master.GeneratedDate = DateTime.Now.Date;

                            if (formcoll["CollectedDate"] != "" && formcoll["CollectedDate"] != null)
                                master.CollectedDate = Convert.ToDateTime(formcoll["CollectedDate"]);
                            else
                                master.CollectedDate = DateTime.Now.Date;

                            if (formcoll["ReturnedDate"] != "" && formcoll["ReturnedDate"] != null)
                                master.ReturnedDate = Convert.ToDateTime(formcoll["ReturnedDate"]);
                            //else
                            //   master.ReturnedDate = DateTime.Now.Date;

                            if (formcoll["StatusTypeID"] == "" || formcoll["StatusTypeID"] == null)
                                master.StatusTypeID = 0;
                            else
                                master.StatusTypeID = Convert.ToInt32(formcoll["StatusTypeID"]);

                            int bkvfcnstsid = 0;
                            if (formcoll["BkVfcnStatusTypeID"] != "" && formcoll["BkVfcnStatusTypeID"] != null)
                                bkvfcnstsid = Convert.ToInt32(formcoll["BkVfcnStatusTypeID"]);

                            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("Select TypeId, Type From MasterType where typeid = " + bkvfcnstsid).ToList();
                            string bkvsts = "";
                            if (mtqry.Count > 0)
                            {
                                bkvsts = mtqry[0].Type.ToString().ToUpper();
                            }

                            if (bkvfcnlistid == 0)
                            {
                                //string squery = "SELECT *FROM TransactionBookVerificationList (nolock) WHERE BookId = " + master.BookID + " and GeneratedDate ='" + Convert.ToDateTime(master.GeneratedDate).ToString("yyyy-MM-dd") + "'";
                                //squery = squery + " and ProductTypeid = " + master.ProductTypeID + "";
                                //squery = squery + " and ProductId = " + master.ProductID + "";
                                //squery = squery + " and BookVerificationListId <> " + master.BookVerificationListID + "";
                                //var query = context.Database.SqlQuery<TransactionBookVerificationList>(squery).ToList();

                                //if (query.Count != 0)
                                //{
                                //    status = "Existing";
                                //}
                                //else
                                {
                                    if (bkvsts == "ASSIGNED")
                                    {
                                        master.AssignedBy = Convert.ToInt32(Session["EmplId"]);
                                        master.AssignedDate = DateTime.Now;
                                    }
                                    if (bkvsts == "APPROVED")
                                    {
                                        master.ApprovedBy = Convert.ToInt32(Session["EmplId"]);
                                        master.ApprovalAssignedTo = Convert.ToInt32(Session["EmplId"]);
                                        master.ApprovedDate = DateTime.Now;
                                    }
                                    if (bkvsts == "VERIFIED")
                                    {
                                        master.VerifiedBy = Convert.ToInt32(Session["EmplId"]);
                                        master.VerificationAssignedTo = Convert.ToInt32(Session["EmplId"]);
                                        master.VerifiedDate = DateTime.Now;

                                    }

                                    context.trnbkvfcnlsts.Add(master);
                                    context.SaveChanges();
                                    status = "Success";

                                }

                            }
                            else
                            {
                                //string squery = "SELECT *FROM TransactionBookVerificationList (nolock) WHERE BookId = " + master.BookID + " and GeneratedDate ='" + Convert.ToDateTime(master.GeneratedDate) + "'";
                                //squery = squery + " and ProductTypeid = " + master.ProductTypeID + "";
                                //squery = squery + " and ProductId = " + master.ProductID + "";
                                //squery = squery + " and ProductId = " + master.ProductID + "";
                                //squery = squery + " and BookVerificationListId <> " + master.BookVerificationListID + "";
                                //var query = context.Database.SqlQuery<TransactionBookVerificationList>(squery).ToList();

                                //if (query.Count != 0)
                                //{
                                //    status = "Existing";
                                //}
                                // else
                                {
                                    if (bkvsts == "ASSIGNED")
                                    {
                                        master.AssignedBy = Convert.ToInt32(Session["EmplId"]);
                                        master.AssignedDate = DateTime.Now;
                                    }
                                    if (bkvsts == "APPROVED")
                                    {
                                        master.ApprovedBy = Convert.ToInt32(Session["EmplId"]);
                                        master.ApprovalAssignedTo = Convert.ToInt32(Session["EmplId"]);
                                        master.ApprovedDate = DateTime.Now;
                                    }
                                    if (bkvsts == "VERIFIED")
                                    {
                                        master.VerifiedBy = Convert.ToInt32(Session["EmplId"]);
                                        master.VerificationAssignedTo = Convert.ToInt32(Session["EmplId"]);
                                        master.VerifiedDate = DateTime.Now;

                                    }

                                    context.Entry(master).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                    status = "Success";
                                }


                            }

                            detail.BookVerificationListID = master.BookVerificationListID;
                            detail.BookVerificationID = bkvid;
                            detail.StatusTypeID = bkvfcnstsid;
                            detail.Remarks = master.Remarks;
                            if (formcoll["F_BookEntryDate"] != "" && formcoll["F_BookEntryDate"] != null)
                                detail.BookEntryDate = Convert.ToDateTime(formcoll["F_BookEntryDate"]);
                            else
                                detail.BookEntryDate = DateTime.Now.Date;

                            if (formcoll["F_BookAmount"] != "" && formcoll["F_BookAmount"] != null)
                                detail.BookAmount = Convert.ToDouble(formcoll["F_BookAmount"]);
                            detail.CollectionReceiptOldAmount = 0;
                            if (formcoll["F_OldReceiptAmount"] != "" && formcoll["F_OldReceiptAmount"] != null)
                                detail.CollectionReceiptOldAmount = Convert.ToDouble(formcoll["F_OldReceiptAmount"]);
                            detail.ReferenceCollectionReceiptID = 0;
                            if (formcoll["F_OldReceiptID"] != "" && formcoll["F_OldReceiptID"] != null)
                                detail.ReferenceCollectionReceiptID = Convert.ToInt32(formcoll["F_OldReceiptID"]);
                            if (detail.ReferenceCollectionReceiptID != null)
                            {
                                if (detail.ReferenceCollectionReceiptID > 0)
                                {
                                    TransactionCollectionList clst = new TransactionCollectionList();
                                    clst = context.trncolnlst.Find(detail.ReferenceCollectionReceiptID);
                                    if (clst != null)
                                    {
                                        clst.BkVfcnSts = 1;
                                        clst.BkVfcnDt = Convert.ToDateTime(master.GeneratedDate);
                                    }
                                }
                            }

                            detail.OwnerTypeId = master.ProductTypeID;
                            detail.VerificationFindingTypeID = 0;
                            if (formcoll["F_BookVerificationFindingID"] != "" && formcoll["F_BookVerificationFindingID"] != null)
                                detail.VerificationFindingTypeID = Convert.ToInt32(formcoll["F_BookVerificationFindingID"]);

                            if (bkvid == 0)
                            {
                                string squery = "SELECT *FROM TransactionBookVerification (nolock) WHERE BookVerificationListId = " + master.BookVerificationListID + "";
                                squery = squery + " and BookEntryDate = '" + detail.BookEntryDate.ToString("yyyy-MM-dd") + "'";
                                squery = squery + " and BookVerificationId <> " + detail.BookVerificationID + "";
                                squery = squery + " and ReferenceCollectionReceiptID = " + detail.ReferenceCollectionReceiptID + "";
                                var query = context.Database.SqlQuery<TransBookVerification>(squery).ToList();

                                if (query.Count != 0)
                                {
                                    status = "Existing";
                                }
                                else
                                {
                                    context.trnbkvfcns.Add(detail);
                                    context.SaveChanges();
                                    status = "Success";

                                }
                            }
                            else
                            {
                                string squery = "SELECT *FROM TransactionBookVerification (nolock) WHERE BookVerificationListId = " + master.BookVerificationListID + " and GeneratedDate ='" + Convert.ToDateTime(master.GeneratedDate) + "'";
                                squery = squery + " and BookEntryDate = " + detail.BookEntryDate.ToString("yyyy-MM-dd") + "";
                                squery = squery + " and BookVerificationId <> " + detail.BookVerificationID + "";
                                var query = context.Database.SqlQuery<TransBookVerification>(squery).ToList();

                                if (query.Count != 0)
                                {
                                    status = "Existing";
                                }
                                else
                                {

                                    context.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                    status = "Success";

                                }
                            }
                            trans1.Commit();
                            bkvfcnlistid = master.BookVerificationListID;

                            //string updqry = "exec pr_Update_Payment_Allocation @paymtlstid = " + master.BookVerificationListID + ",@status =" + master.StatusTypeID;
                            //context.Database.ExecuteSqlCommand(updqry);

                            ViewBag.Message = "Form submitted.";
                            // Response.Redirect("Index");
                            //return RedirectToAction("Index", "ChitSubscriptionManagement");
                            //Response.Write("Update");

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

                    string colagt = Convert.ToString(Session["CollectionAgent"]);

                    int prodtype = Convert.ToInt32(Session["ProductType"]);
                    int routeid = Convert.ToInt32(Session["RouteID"]);
                    int colagtid = 0;
                    if (colagt != "" && colagt != "undefined")
                        colagtid = Convert.ToInt32(colagt);
                    string bkvfby = Convert.ToString(Session["BookVerifiedBy"]);
                    int bkverfby = 0;
                    if (bkvfby != "" && bkvfby != "undefined")
                        bkverfby = Convert.ToInt32(bkvfby);
                    var data = e.pr_Search_Transaction_BookVerification_Details(param.sSearch,
                                                    Convert.ToInt32(Request["iSortCol_0"]),
                                                    Request["sSortDir_0"],
                                                    param.iDisplayStart,
                                                    param.iDisplayStart + param.iDisplayLength,
                                                    totalRowsCount,
                                                    filteredRowsCount, Convert.ToString(cusrid), Convert.ToInt32(cempid), Convert.ToInt32(prodtype), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), routeid, colagtid, bkverfby);

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
                        BookVerfcnGenDt = d.BookVerfcnGenDt,
                        SUBSCRIPTIONCODE = d.SUBSCRIPTIONCODE,
                        COLNAGENTNAME = d.COLNAGENTNAME,
                        ROUTE = d.ROUTENAME,
                        VerifiedByName = d.VerifiedByName,
                        ApprovedByName = d.ApprovedByName,
                        TRANID = d.TRANID.ToString()
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

        #region Get Product Receipt Details
        public JsonResult GetReceiptDtls(string id)
        {
            string[] param = id.Split('~');
            int prodtype = 29;
            int prodid = 0;
            int bvfnlstid = 0;
            int bvid = 0;
            string bvgndt = "";
            if (param.Length > 0)
            {
                if (param[0] != "" && param[0] != null)
                {
                    prodid = Convert.ToInt32(param[0]);
                }
                if (param[1] != "" && param[1] != null)
                {
                    prodtype = Convert.ToInt32(param[1]);
                }
                if (param[2] != "" && param[2] != null)
                {
                    bvfnlstid = Convert.ToInt32(param[2]);
                }
                if (param[3] != "" && param[3] != null)
                {
                    bvgndt = Convert.ToString(param[3]);
                }
                if (param[4] != "" && param[4] != null)
                {
                    bvid = Convert.ToInt32(param[4]);
                }
            }

            string trntypqry = "exec pr_Get_Receipt_Details_BV_Assign @bvlistid=" + bvfnlstid + ",@bvgendt='" + Convert.ToDateTime(bvgndt).ToString("yyyy-MM-dd") + "',@subsid=" + prodid + ", @prodtypid=" + prodtype + ",@bvid=" + bvid;
            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>(trntypqry).ToList();
            //ViewBag.SubsTranCategory = new SelectList(mtqry, "TypeID", "Type").ToList();

            return Json(mtqry, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Product Books Details
        public JsonResult GetBooks(string id)
        {
            string[] param = id.Split('~');
            int prodtype = 29;
            int prodid = 0;
            int bvfnlstid = 0;
            if (param.Length > 0)
            {
                if (param[0] != "" && param[0] != null)
                {
                    prodid = Convert.ToInt32(param[0]);
                }
                if (param[1] != "" && param[1] != null)
                {
                    prodtype = Convert.ToInt32(param[1]);
                }
                if (param[2] != "" && param[2] != null)
                {
                    bvfnlstid = Convert.ToInt32(param[2]);
                }

            }

            string trntypqry = "exec pr_Get_Books_BV_Assign @bvlistid=" + bvfnlstid + ",@subsid=" + prodid + ", @prodtypid=" + prodtype + "";
            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>(trntypqry).ToList();
            //ViewBag.SubsTranCategory = new SelectList(mtqry, "TypeID", "Type").ToList();

            return Json(mtqry, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Product FC Amount for GST 
        public JsonResult GetFCAmt(string id)
        {
            string[] param = id.Split('~');
            int prodtype = 29;
            int subsid = 0;
            if (param != null)
            {
                if (param.Length > 0)
                {
                    if (param[0] != "" && param[0] != "undefined" && param[0] != null)
                    {
                        subsid = Convert.ToInt32(param[0]);
                    }
                    if (param[1] != "" && param[1] != "undefined" && param[1] != null)
                    {
                        prodtype = Convert.ToInt32(param[1]);
                    }
                }
            }

            string trntypqry = "exec pr_Get_FCAmt @usrid='" + Session["CUSRID"].ToString() + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod=" + prodtype + ", @subsid=" + subsid + "";
            var mtqry = context.Database.SqlQuery<decimal>(trntypqry).ToList();
            //ViewBag.SubsTranCategory = new SelectList(mtqry, "TypeID", "Type").ToList();

            return Json(mtqry, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Delete Book Verification List Record  
        //[Authorize(Roles = "TransactionBookVerificationDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check("TransactionBookVerificationList", id, "TransactionBookVerificationList");

            if (temp.Equals("PROCEED"))
            {
                TransactionBookVerificationList trnbkvfclstid = context.trnbkvfcnlsts.Find(Convert.ToInt32(id));

                string dqy = "Delete from TransactionBookVerification Where BookVerificationListID = " + Convert.ToInt32(trnbkvfclstid.BookVerificationListID) + "";
                context.Database.ExecuteSqlCommand(dqy);
                context.trnbkvfcnlsts.Remove(trnbkvfclstid);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion

        #region Delete Book Verification Record  
        //[Authorize(Roles = "TransactionBookVerificationDelete")]
        public void BVDel()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check("TransactionBookVerification", id, "TransactionBookVerification");

            if (temp.Equals("PROCEED"))
            {
                TransBookVerification trnbkvfcn = context.trnbkvfcns.Find(Convert.ToInt32(id));

                context.trnbkvfcns.Remove(trnbkvfcn);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion
    }
}

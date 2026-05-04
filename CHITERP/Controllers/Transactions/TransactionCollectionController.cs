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
using System.Web.Routing;

namespace CHITERP.Controllers.TransactionCollection
{
    public class TransactionCollectionController : Controller
    {
        // GET: TransactionCollection
        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        //[Authorize(Roles = "TransactionCollectionIndex")]
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
            Session["RouteID"] = 0;
            if (string.IsNullOrEmpty(Session["ProductType"] as string))
            {
                if (Request.Form.Get("ProductType") != null && Request.Form.Get("ProductType") != "")
                {
                    Session["ProductType"] = Request.Form.Get("ProductType");
                }
            }
            if (string.IsNullOrEmpty(Session["RouteID"] as string))
            {
                if (Request.Form.Get("RouteID") != null && Request.Form.Get("RouteID") != "")
                {
                    Session["RouteID"] = Request.Form.Get("RouteID");
                }
            }
            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Product', ''").ToList();
            ViewBag.ProductType = new SelectList(mtqry.Where(x => x.Type == "Chit Fund" || x.Type == "Loan"), "TypeID", "Type", Convert.ToInt32(Session["ProductType"])).ToList();
            var amtcdqry4 = context.Database.SqlQuery<CompanyRoute>("select* from CompanyRoute (nolock)").ToList();
            ViewBag.RouteID = new SelectList(amtcdqry4, "RouteID", "RouteName", Convert.ToInt32(Session["RouteID"]));
            
            return View();
        }
        #endregion

        #region Edit
        //[Authorize(Roles = "TransactionCollectionEdit")]
        public void Edit(string id = "0")
        {

            if (id != "")
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/TransactionCollection/Form/" + id);

                //Response.Redirect("/TransactionCollection/Form/?id=" + id);
            }
        }
        #endregion

        #region Form
        //[Authorize(Roles = "TransactionCollectionCreate")]
        public ActionResult Form(string id = "0")
        {

            int TransactionID = 0;
            id = id.Replace("'", "");

            string[] param = id.Split('~');
            string Category = "C";
            string SubsID = "";
            string ProdType = "";
            int subsId = 0;
            int prdtyp = 0;
            if (param.Length > 1)
            {
                //if (param[0] != null && param[0] != "")
                //    Category = param[0];
                if (param[2] != null && param[2] != "")
                {
                    SubsID = param[2];
                    subsId = Convert.ToInt32(SubsID.Trim());
                    ViewBag.F_SubsID = param[2];
                }
                if (param[1] != null && param[1] != "")
                {
                    ProdType = param[1];
                    prdtyp = Convert.ToInt32(ProdType.Trim());
                    ViewBag.F_ProdType = param[1];
                }

                if (param[0] != null && param[0] != "")
                    TransactionID = Convert.ToInt32(param[0]);
            }
            else
            {
                TransactionID = Convert.ToInt32(id);
            }
            Category = "C";
            //if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            if (Convert.ToInt32(Session["compyid"]) == 0)
            {
                
                string refurl = "";
                refurl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToString();
                var routeValues = new RouteValueDictionary { { "returnUrl", refurl } };
                return RedirectToAction("Login", "Account", routeValues);
            }

            TransactionCollectionList tab = new TransactionCollectionList();
                       
            var clist1 = new SelectList(context.clientmasters.Where(d => d.DISPSTATUS == 0), "CLIENTID", "CLIENTNAME").ToList();
            ViewBag.PersonID = new SelectList("");// clist1;


            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Product', ''").ToList();
            ViewBag.ProductTypeID = new SelectList(mtqry.Where(x=> x.Type == "Chit Fund" || x.Type == "Loan"), "TypeID", "Type").ToList();
            if (subsId > 0)
            {
                ViewBag.ProductTypeID = new SelectList(mtqry.Where(x => x.Type == "Chit Fund" || x.Type == "Loan"), "TypeID", "Type", prdtyp).ToList();
            }
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Transaction Type', ''").ToList();
            ViewBag.TransactionType = new SelectList(mtqry, "TypeID", "Type",tab.TransactionTypeTypeID).ToList();
            
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Transaction Mode', ''").ToList();            
            ViewBag.TransactionModeTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();

            var amtcdqry2 = context.Database.SqlQuery<CompanyFundAccount>("Select * From CompanyFundAccount (nolock)").ToList();

            ViewBag.FundAccountID = new SelectList(amtcdqry2, "FundAccountID", "AccountName");

            var amtcdqry3 = context.Database.SqlQuery<CompanyOffice>("Select * From CompanyOffice (nolock)").ToList();
            ViewBag.LinkedOfficeID = new SelectList(amtcdqry3, "officeid", "OfficeName");


            string trntypqry = "exec pr_Get_ProductCategory @usrid='" + Session["CUSRID"].ToString() + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod=29,@trncateg='"+Category+"'";
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>(trntypqry).ToList();
            ViewBag.SubsTranCategory = new SelectList(mtqry, "TypeID", "Type",tab.TransactionCategoryTypeID).ToList();
            if (subsId > 0)
            {
                trntypqry = "exec pr_Get_ProductCategory @usrid='" + Session["CUSRID"].ToString() + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod="+ prdtyp+",@trncateg='" + Category + "'";
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>(trntypqry).ToList();
                ViewBag.SubsTranCategory = new SelectList(mtqry, "TypeID", "Type", tab.TransactionCategoryTypeID).ToList();
            }
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
            ViewBag.StatusTypeID = new SelectList(mtqry.Where(d=>d.Type == "Collected"), "TypeID", "Type").ToList();
            string crole = Convert.ToString(Session["Group"]);
            string cusrid = Convert.ToString(Session["CUSRID"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                cusrid = "";
            }
            trntypqry = "exec pr_Get_Subscription_Info @usrid='" + cusrid + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod=29";
            var subsqry = context.Database.SqlQuery<pr_Get_Subscription_Info_Result>(trntypqry).ToList();
            ViewBag.SubscriptionID = new SelectList(subsqry, "subscriptionid", "subscriptioncode").ToList();
            if (subsId > 0)
            {
                trntypqry = "exec pr_Get_Subscription_Info @usrid='" + cusrid + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod="+prdtyp;
                subsqry = context.Database.SqlQuery<pr_Get_Subscription_Info_Result>(trntypqry).ToList();
                ViewBag.SubscriptionID = new SelectList(subsqry, "subscriptionid", "subscriptioncode",subsId).ToList();
            }
            List<SelectListItem> selTrnCatg = new List<SelectListItem>();
            SelectListItem selTrnCatgItm = new SelectListItem { Text = "Collection", Value = "C", Selected = true };
            selTrnCatg.Add(selTrnCatgItm);
            //selTrnCatgItm = new SelectListItem { Text = "Payment", Value = "P", Selected = false };
            //selTrnCatg.Add(selTrnCatgItm);
            ViewBag.TransactionCategory = selTrnCatg;

            if (TransactionID > 0)
            {
                tab = context.trncolnlst.Find(TransactionID);
                ViewBag.TranID = TransactionID;

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

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Product', ''").ToList();
                //ViewBag.ProductTypeID = new SelectList(mtqry.Where(x => x.Type == "Chit Fund" || x.Type == "Loan"), "TypeID", "Type", tab.ProductTypeID).ToList();
                ViewBag.ProductTypeID = new SelectList(mtqry.Where(x => x.TypeID == tab.ProductTypeID), "TypeID", "Type", tab.ProductTypeID).ToList();

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Transaction Type', ''",tab.TransactionTypeTypeID).ToList();
                ViewBag.TransactionType = new SelectList(mtqry, "TypeID", "Type",tab.TransactionTypeTypeID).ToList();

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Transaction Mode', ''",tab.TransactionModeTypeID).ToList();
                ViewBag.TransactionModeTypeID = new SelectList(mtqry, "TypeID", "Type",tab.TransactionModeTypeID).ToList();

                amtcdqry2 = context.Database.SqlQuery<CompanyFundAccount>("Select * From CompanyFundAccount (nolock)").ToList();

                //ViewBag.FundAccountID = new SelectList(amtcdqry2, "FundAccountID", "AccountName");

                //amtcdqry3 = context.Database.SqlQuery<CompanyOffice>("Select * From CompanyOffice (nolock)").ToList();
                //ViewBag.LinkedOfficeID = new SelectList(amtcdqry3, "officeid", "OfficeName");


                trntypqry = "exec pr_Get_ProductCategory @usrid='" + Session["CUSRID"].ToString() + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod=" + tab.ProductTypeID + ", @trncateg='" + Category + "'";               
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>(trntypqry).ToList();                
                ViewBag.SubsTranCategory = new SelectList(mtqry, "TypeID", "Type",tab.TransactionCategoryTypeID).ToList();
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
                //ViewBag.StatusTypeID = new SelectList(mtqry.Where(x => (x.TypeID >= tab.StatusTypeID && tab.StatusTypeID>=1034) || tab.StatusTypeID<1034), "TypeID", "Type", tab.StatusTypeID).ToList();
                ViewBag.StatusTypeID = new SelectList(mtqry.Where(x=>x.Type=="Collected"||x.TypeID== tab.StatusTypeID), "TypeID", "Type", tab.StatusTypeID).ToList();
                trntypqry = "exec pr_Get_Subscription_Info @usrid='" +cusrid + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod="+tab.ProductTypeID;
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
                        string savenContFlg = "";
                        savenContFlg = formcoll["subcont"];
                        try
                        {


                            string TransactionCategory = "C";// Convert.ToString(formcoll["TransactionCategory"]);
                            if (TransactionCategory == "C")
                            {

                                TransactionCollectionList master = new TransactionCollectionList();
                                Int32 CollectionListID = 0;

                                string collectionListID = Convert.ToString(formcoll["CollectionListID"]);
                                if (collectionListID == "" || collectionListID == null)
                                {
                                    CollectionListID = 0;
                                }
                                else
                                {
                                    CollectionListID = Convert.ToInt32(collectionListID);
                                }

                                if (CollectionListID != 0)
                                {
                                    master = context.trncolnlst.Find(CollectionListID);
                                }
                                
                                //master.CollectionListID = CollectionListID;
                                if (formcoll["ProductTypeID"] == "" || formcoll["ProductTypeID"] == null)
                                    master.ProductTypeID = 0;
                                else
                                    master.ProductTypeID = Convert.ToInt32(formcoll["ProductTypeID"]);
                                if (formcoll["SubscriptionID"] == "" || formcoll["SubscriptionID"] == null)
                                    master.ProductID = 0;
                                else
                                    master.ProductID = Convert.ToInt32(formcoll["SubscriptionID"]);
                                int trantypeid = 0;
                                if (formcoll["TransactionType"] == "" || formcoll["TransactionType"] == null)
                                {
                                    if (master.ProductTypeID==29)
                                    {
                                        ChitSubscription chts = new ChitSubscription();
                                        chts = context.chitsubscriptions.Find(master.ProductID);
                                        trantypeid = Convert.ToInt32(chts.CollectionTypeID);
                                    }
                                    else if (master.ProductTypeID == 30)
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
                                    master.TransactionTypeTypeID= Convert.ToInt32(formcoll["TransactionType"]);
                                if (formcoll["SubsTranCategory"] == "" || formcoll["SubsTranCategory"] == null)
                                    master.TransactionCategoryTypeID = 0;
                                else
                                    master.TransactionCategoryTypeID = Convert.ToInt32(formcoll["SubsTranCategory"]);

                                master.TransactionModeTypeID = Convert.ToInt32(formcoll["TransactionModeTypeID"]);
                                
                                if (formcoll["GeneratedDate"] != "" && formcoll["GeneratedDate"] != null)
                                    master.GeneratedDate = Convert.ToDateTime(formcoll["GeneratedDate"]);
                                else
                                    master.GeneratedDate = DateTime.Now.Date;

                                master.ChequeDate = null;
                                if (formcoll["ChequeDate"] != "" && formcoll["ChequeDate"] != null)
                                    master.ChequeDate = Convert.ToDateTime(formcoll["ChequeDate"]);
                                master.ChequeNo = null;
                                if (formcoll["ChequeNo"] != "" && formcoll["ChequeNo"] != null)
                                    master.ChequeNo = Convert.ToString(formcoll["ChequeNo"]);
                                master.ChequeBankName = null;
                                if (formcoll["ChequeBankName"] != "" && formcoll["ChequeBankName"] != null)
                                    master.ChequeBankName = Convert.ToString(formcoll["ChequeBankName"]);


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


                                //if (CollectionListID == 0)
                                //    master.GeneratedDate = DateTime.Now.Date;
                                master.Remarks = Convert.ToString(formcoll["Remarks"]);
                                master.Installment = null;
                                if (formcoll["Installment"] != "" && formcoll["Installment"] != null)                                    
                                    master.Installment = Convert.ToInt32(formcoll["Installment"]);

                                if (formcoll["GeneratedAmount"] == "" || formcoll["GeneratedAmount"] == null)
                                    master.GeneratedAmount = 0;
                                else
                                    master.GeneratedAmount = Convert.ToDouble(formcoll["GeneratedAmount"]);

                                if (formcoll["StatusTypeID"] == "" || formcoll["StatusTypeID"] == null)
                                    master.StatusTypeID = 0;
                                else
                                    master.StatusTypeID = Convert.ToInt32(formcoll["StatusTypeID"]);




                                if (CollectionListID == 0)
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
                                        master.collectionagent = Session["CUSRID"].ToString();
                                        context.trncolnlst.Add(master);
                                        context.SaveChanges();
                                        if (savenContFlg == "Continue")
                                            status = "Continue";
                                        else
                                            status = "Success";

                                    }

                                }
                                else
                                {
                                    //master.UpdatedBy = Session["CUSRID"].ToString();
                                    //master.UpdatedDt = DateTime.Now;
                                    if(master.collectionagent==null)
                                        master.collectionagent = Session["CUSRID"].ToString();
                                    context.Entry(master).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                    if (savenContFlg == "Continue")
                                        status = "Continue";
                                    else
                                        status = "Success";
                                }
                                trans1.Commit();
                                CollectionListID = master.CollectionListID;

                                string updqry = "exec pr_Update_Collection_Receipt_Allocation @collectionlstid = " + master.CollectionListID + ",@status =" + master.StatusTypeID;                                
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


        #region Get Today Collection Detail
        public string GetTodayCollection(string id)
        {
            int Prodid = 0;
            if (id != "" && id != null)
                Prodid = Convert.ToInt32(id);
            DateTime dt = DateTime.Now.Date;


            using (var e = new FinFusionEntities())
            {
                
                string crole = Convert.ToString(Session["Group"]);
                string cusrid = Convert.ToString(Session["CUSRID"]);
                int cempid = Convert.ToInt32(Session["EMPLID"]);
                if (crole == "SuperAdmin" || crole == "Admin")
                {
                    cusrid = "";
                }


                var tabl = "";

                var count = 0;
                tabl = "<Table id=TODAYCOLLECTION_TDETAIL class='table table-striped table-bordered bootstrap-datatable TODAYCOLLECTIONDETAIL'> <thead><tr><th>Entered By</th><th>Client</th><th>Amt.</th></tr></thead><tbody id='todaycollection_tbody'>";
                string SQRY = "EXEC pr_Get_Today_Collection @indt ='" + Convert.ToDateTime(DateTime.Now.Date).ToString("yyyy-MM-dd") + "', @usrid = '" + cusrid + "', @empid = " + cempid + ", @prod = " + Prodid;
                var query = context.Database.SqlQuery<pr_Get_Today_Collection_Result>(SQRY).ToList();

                foreach (var rslt in query)
                {
                    tabl = tabl + "<tr>";
                    tabl = tabl + "<td class='col-md-1'><input type=text id=EnteredBy value=" + rslt.EnteredBy + " class='form-control EnteredBy' name=EnteredBy maxlength='100' tabindex='105'></td>";
                    tabl = tabl + "<td class='col-md-1'><input type=text id=Client value=" + rslt.Client + " class='form-control Client' name=Client maxlength='150' tabindex='105'></td>";
                    tabl = tabl + "<td class='col-md-1'><input type=text id=SubsCode value=" + rslt.SubsCode + " class='form-control SubsCode' name=SubsCode maxlength='25' tabindex='105'></td>";
                    tabl = tabl + "<td class='col-md-1'><input type=text id=CollectedAmt class='form-control CollectedAmt' name=CollectedAmt value=" + rslt.CollectedAmt + " maxlength='25' tabindex='105'></td>";
                    tabl = tabl + "</tr>";
                    count++;
                }
                tabl = tabl + "</tbody></table>";

                return tabl;

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
                     int routeid = Convert.ToInt32(Session["RouteID"]);
                    var data = e.pr_Search_Transaction_Collection_Details(param.sSearch,
                                                    Convert.ToInt32(Request["iSortCol_0"]),
                                                    Request["sSortDir_0"],
                                                    param.iDisplayStart,
                                                    param.iDisplayStart + param.iDisplayLength,
                                                    totalRowsCount,
                                                    filteredRowsCount, Convert.ToString(cusrid), Convert.ToInt32(cempid), Convert.ToInt32(prodtype), Convert.ToInt32(routeid), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
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
                        CollectionDt = d.CollectionDt,
                        ROUTEDESC = d.ROUTEDESC,
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
            string trancateg = "C";
            if (param.Length>0)
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
            string trancateg = "C";
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
            string trntypqry = "exec pr_Get_Transaction_Dtl @categ='" + trancateg + "',@id=" + Convert.ToInt32(prodid) ;
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
            trancateg = "C";
            string trntypqry = "exec pr_Get_ProductCategory @usrid='" + Session["CUSRID"].ToString() + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod=" + prodtype + ", @trncateg='" + trancateg + "'";
            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>(trntypqry).ToList();
            //ViewBag.SubsTranCategory = new SelectList(mtqry, "TypeID", "Type").ToList();

            return Json(mtqry, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Subscription Default Value Details
        public JsonResult GetProductSubscriptionDefValsDtl(string id)
        {
            string[] param = id.Split('~');
            int prodtype = 29;
            int subsid =0;
            if (param.Length > 0)
            {
                if (param[0] != "" && param[0] != null)
                {
                    subsid = Convert.ToInt32(param[0]);
                }
                if (param[1] != "" && param[1] != null)
                {
                    prodtype = Convert.ToInt32(param[1]);
                }
            }
            string trntypqry = "exec pr_Get_ProductSubsDefValDetl @prod=" + prodtype + ",@subsid=" + subsid + "";
            var mtqry = context.Database.SqlQuery<pr_Get_ProductSubsDefValDetl_Result>(trntypqry).ToList();

            return Json(mtqry, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Delete Record  
        //[Authorize(Roles = "TransactionCollectionDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check("TransactionCollectionList", id, "TransactionCollectionList");

            if (temp.Equals("PROCEED") && id!="" & id != null)
            {
                TransactionCollectionList trancollist = context.trncolnlst.Find(Convert.ToInt32(id));
                string dqy = "Delete A From TransactionCollectionAllocation A join TransactionCollectionReciepts B(nolock) on a.CollectionReceiptID = b.CollectionRecieptID Where b.CollectionListID = " + Convert.ToInt32(trancollist.CollectionListID) +"";
                context.Database.ExecuteSqlCommand(dqy);
                dqy = "Delete from TransactionCollectionReciepts Where CollectionListID = " + Convert.ToInt32(trancollist.CollectionListID) + "";
                context.Database.ExecuteSqlCommand(dqy);
                context.trncolnlst.Remove(trancollist);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion
    }
}
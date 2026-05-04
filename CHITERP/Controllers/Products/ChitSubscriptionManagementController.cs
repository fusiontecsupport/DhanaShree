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
using System.Drawing;
using System.Net;
using System.Data;
using System.Text;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace CHITERP.Controllers.Products
{
    public class ChitSubscriptionManagementController : Controller
    {
        // GET: ChitSubscriptionManagement

        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        //[Authorize(Roles = "ChitSubscriptionManagementIndex")]
        public ActionResult Index()
        {
            
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            string cusrid = Convert.ToString(Session["CUSRID"]);
            string crole = Convert.ToString(Session["Group"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                cusrid = "";
            }
            string eqry = "exec pr_Get_Employee_Dtl @usrid='" + cusrid + "'";
            var empqry = context.Database.SqlQuery<pr_Get_Client_Type_Result>(eqry).ToList();
            
            if (Session["ColAgent"] == null)
            {
                var firstOrDefaultEmp = empqry.OrderBy(x => x.DValue).FirstOrDefault();
                Session["SDATE"] = DateTime.Now.ToString("yyyy-MM-dd");
                Session["EDATE"] = DateTime.Now.ToString("yyyy-MM-dd");
                Session["ColAgent"] = firstOrDefaultEmp.DValue;
            }
            else
            {
                if (Request.Form.Get("from") != null)
                {
                    Session["SDATE"] = Request.Form.Get("from");
                    Session["EDATE"] = Request.Form.Get("to");
                    Session["ColAgent"] = Request.Form.Get("CollectionAgentPersonID");
                }
            }
            
            int colagtid = Convert.ToInt32(Session["ColAgent"]);
            ViewBag.CollectionAgentPersonID = new SelectList(empqry, "DValue", "DText", colagtid).ToList();

            return View();
        }
        #endregion

        #region Edit
        //[Authorize(Roles = "ChitSubscriptionManagementEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/ChitSubscriptionManagement/Form/" + id);

                //Response.Redirect("/ChitSubscriptionManagement/Form/?id=" + id);
            }
        }
        #endregion

        #region Form
        //[Authorize(Roles = "ChitSubscriptionManagementCreate")]
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            ChitSubscription tab = new ChitSubscription();
            ChitTransSS CTS = new ChitTransSS();
           
            int SubscriptionID = 0;

            if (id == "" || id == null || id == "0")
            { SubscriptionID = 0; }
            else { SubscriptionID = Convert.ToInt32(id); }

            var personqry = context.Database.SqlQuery<pr_Chit_Subscription_Organization_Name_Assgn_Result>("Exec pr_Chit_Subscription_Organization_Name_Assgn").ToList();
            ViewBag.FPersonID = new SelectList(personqry, "CLIENTID", "CNAME").ToList();

            var amtcdqry = context.Database.SqlQuery<ChitScheme>("Select * From ChitScheme (nolock)").ToList();
            ViewBag.ChitSchemeID = new SelectList(amtcdqry, "ChitSchemeID", "ChitSchemeName");

            var amtcdqry1 = context.Database.SqlQuery<ChitGroup>("Select * From ChitGroup (nolock)").ToList();            
            ViewBag.ChitGroupID = new SelectList(amtcdqry1, "ChitGroupID", "ChitGroupName");

            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Collection Type', ''").ToList();
            ViewBag.CollectionTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();

            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Interval', ''").ToList();
            ViewBag.CollectionIntervalTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();

            var amtcdqry2 = context.Database.SqlQuery<CompanyFundAccount>("Select * From CompanyFundAccount (nolock)").ToList();

            ViewBag.FundAccountID = new SelectList(amtcdqry2, "FundAccountID", "AccountName");

            var amtcdqry3 = context.Database.SqlQuery<CompanyOffice>("Select * From CompanyOffice (nolock)").ToList();
            ViewBag.LinkedOfficeID = new SelectList(amtcdqry3, "officeid", "OfficeName");

            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Collection Mode', ''").ToList();
            ViewBag.CollectionModeTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Interval', ''").ToList();
            ViewBag.AscertainmentFrequencyTypeId = new SelectList(mtqry, "TypeID", "Type", 775).ToList();

            var amtcdqry4 = context.Database.SqlQuery<CompanyRoute>("select* from CompanyRoute (nolock)").ToList();
            ViewBag.RouteID = new SelectList(amtcdqry4, "RouteID", "RouteName"); 
            ViewBag.CoApplicants = new SelectList("");
            ViewBag.NomineeRelationshipTypeID = new SelectList("");
            var amtcdqry5 = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_Get_Book_PickupDay 0").ToList();
            ViewBag.BookPickupDay = new SelectList(amtcdqry5, "DValue", "DText").ToList();

            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Chit Subscription', 'Status'").ToList();
            ViewBag.StatusTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();
            ViewBag.qrcdsrc = "";
            ViewBag.chk = "hide";
            string cusrid = Convert.ToString(Session["CUSRID"]);
            string crole = Convert.ToString(Session["Group"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                cusrid = "";
            }
            var empqry = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_Get_Employee_Dtl @usrid='"+cusrid+"'").ToList();
            ViewBag.CommisionAgentPersonID = new SelectList(empqry, "DValue", "DText").ToList();
            ViewBag.CommisionAgentPersonID2 = new SelectList(empqry, "DValue", "DText").ToList();
            ViewBag.CollectionAgentPersonID1 = new SelectList(empqry, "DValue", "DText").ToList();
            ViewBag.CollectionAgentPersonID2 = new SelectList(empqry, "DValue", "DText").ToList();
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Book Verification', 'Status'").ToList();
            ViewBag.BookStatusTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();
            if (SubscriptionID > 0)
            {
                tab = context.chitsubscriptions.Find(SubscriptionID);
                CTS.chitsubs = context.chitsubscriptions.Where(x=>x.SubscriptionID==SubscriptionID).ToList();
                CTS.chitschedules = context.Database.SqlQuery<pr_Get_Chit_Collection_Pattern_n_Actuals_Details_Result>("Exec pr_Get_Chit_Collection_Pattern_n_Actuals_Details @ChitSubsID=" + SubscriptionID + ", @usrid='" + cusrid + "'").ToList();
                ViewBag.FPersonID = new SelectList(personqry.Where(x=>x.CLIENTID==tab.PersonID), "CLIENTID", "CNAME", tab.PersonID).ToList();
                ViewBag.QRSubsID = SubscriptionID;
                ViewBag.ProdType = "29";
                amtcdqry = context.Database.SqlQuery<ChitScheme>("Select * From ChitScheme (nolock)").ToList();
                ViewBag.ChitSchemeID = new SelectList(amtcdqry, "ChitSchemeID", "ChitSchemeName", tab.ChitSchemeID);

                amtcdqry1 = context.Database.SqlQuery<ChitGroup>("Select * From ChitGroup (nolock)").ToList();
                ViewBag.ChitGroupID = new SelectList(amtcdqry1, "ChitGroupID", "ChitGroupName", tab.ChitGroupID);

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Collection Type', ''").ToList();
                ViewBag.CollectionTypeID = new SelectList(mtqry, "TypeID", "Type",tab.CollectionTypeID).ToList();

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Interval', ''").ToList();
                ViewBag.CollectionIntervalTypeID = new SelectList(mtqry, "TypeID", "Type",tab.CollectionIntervalTypeID).ToList();

                amtcdqry2 = context.Database.SqlQuery<CompanyFundAccount>("Select * From CompanyFundAccount (nolock)").ToList();

                ViewBag.FundAccountID = new SelectList(amtcdqry2, "FundAccountID", "AccountName",tab.FundAccountID);

                amtcdqry3 = context.Database.SqlQuery<CompanyOffice>("Select * From CompanyOffice (nolock)").ToList();
                ViewBag.LinkedOfficeID = new SelectList(amtcdqry3, "officeid", "OfficeName",tab.LinkedOfficeID);
                
                ViewBag.CommisionAgentPersonID = new SelectList(empqry, "DValue", "DText", tab.CommisionAgentPersonID).ToList();
                ViewBag.CommisionAgentPersonID2 = new SelectList(empqry, "DValue", "DText", tab.CommisionAgentPersonID2).ToList();
                ViewBag.CollectionAgentPersonID1 = new SelectList(empqry, "DValue", "DText", tab.CollectionAgentPersonID1).ToList();
                ViewBag.CollectionAgentPersonID2 = new SelectList(empqry, "DValue", "DText", tab.CollectionAgentPersonID2).ToList();

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Collection Mode', ''").ToList();
                ViewBag.CollectionModeTypeID = new SelectList(mtqry, "TypeID", "Type",tab.CollectionModeTypeID).ToList();
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Interval', ''").ToList();
                ViewBag.AscertainmentFrequencyTypeId = new SelectList(mtqry, "TypeID", "Type", 775).ToList();

                amtcdqry4 = context.Database.SqlQuery<CompanyRoute>("select* from CompanyRoute (nolock)").ToList();
                ViewBag.RouteID = new SelectList(amtcdqry4, "RouteID", "RouteName",tab.RouteID);
                ViewBag.CoApplicants = new SelectList("");
                ViewBag.NomineeRelationshipTypeID = new SelectList("");
                amtcdqry5 = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_Get_Book_PickupDay 0").ToList();
                ViewBag.BookPickupDay = new SelectList(amtcdqry5, "DValue", "DText", tab.BookPickupDay).ToList();
                ChitGroup cg = new ChitGroup();
                cg = context.chitgroups.Find(tab.ChitGroupID);
                ViewBag.PrevGroupTypeID = cg.ChitGroupTypeID;

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Chit Subscription', 'Status'").ToList();
                ViewBag.StatusTypeID = new SelectList(mtqry, "TypeID", "Type", tab.StatusTypeID).ToList();
                //ViewBag.StatusTypeID = new SelectList(mtqry.Where(x => x.TypeID >= tab.StatusTypeID), "TypeID", "Type", tab.StatusTypeID).ToList();
                string imgurl = "~/CSubsQRCode/" + SubscriptionID.ToString() + ".png";
                string imgpath = Server.MapPath("~/CSubsQRCode/" + id.ToString() + ".png");
                //bool chkurl = RemoteFileExists(imgurl);
                bool chkurl = System.IO.File.Exists(imgpath);

                //if (chkurl == false)
                //{
                    GenerateQRCodeFile(SubscriptionID);
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
                CTS = null;
            }

            return View(CTS);
        }
        #endregion

        #region Ledger Report
        //[Authorize(Roles = "ChitSubscriptionManagementCreate")]
        public ActionResult ChitLedgerRpt(string id = "0")
        {
            ViewBag.SubscriptionID = id;
            return View();
        }
        #endregion

        #region QrCode Gneration
        public void GenerateQRCodeFile(int? id = 0)
        {
            if (!System.IO.Directory.Exists(Server.MapPath("~/CSubsQRCode/")))
                Directory.CreateDirectory(Server.MapPath("~/CSubsQRCode/"));

            string barcodePath = Server.MapPath("~/CSubsQRCode/" + id.ToString() + ".png");

            var result = context.Database.SqlQuery<pr_Get_QRCode_Chit_Subscription_Dtl_Result>("exec pr_Get_QRCode_Chit_Subscription_Dtl @usrid='" + Session["CUSRID"].ToString() + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ", @subsid =" + id).ToList();//........procedure  for edit mode details data
            foreach (var rslt in result)
            {
                var subscriptioncode = rslt.subscriptioncode;
                var clienttype = rslt.clienttype;
                var subscriptionid = rslt.subscriptionid;
                var collectionamt = rslt.collectionamt;
                var chitvalue = rslt.chitvalue;
                var clientname = rslt.clientname;
                var clientid = rslt.clientid;

                //string QRContent = subscriptionid + "|" + subscriptioncode + "|" + clienttype + "|" + collectionamt + "|" + chitvalue + "|" + clientname + "|";                
                //string QRContent = "http://localhost:56500/TransCollection/Form?id='0~29~" + subscriptionid + "'";
                //string QRContent = "http://37.17.50.19/dsfinfusion/TransactionCollection/Form?id='0~29~" + subscriptionid.ToString() + "'";
                string burl = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath;
                string QRContent = burl+"/TransactionCollection/Form?id='0~29~" + subscriptionid.ToString() + "'";
                

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
                    //        img.Save(barcodePath, System.Drawing.Imaging.ImageFormat.Png); //Jpeg
                    //    }
                    //}
                    //if (System.IO.File.Exists(barcodePath))
                    //    System.IO.File.Delete(barcodePath);
                    //var url = string.Format("http://chart.apis.google.com/chart?cht=qr&chs={1}x{2}&chl={0}&chld={3}", QRContent, 500, 500,"L");
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

        #region Generate Dividend Payments
        [HttpPost]
        public ActionResult GenerateDividendPayments(FormCollection formcoll)
        {
            string status = "";
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0 || Session["CUSRID"].ToString() == "") { return RedirectToAction("Login", "Account"); }
            else
            {

                using (context = new AppDbContext())
                {
                    using (var trans1 = context.Database.BeginTransaction())
                    {
                        try
                        {
                            string[] boolDivPymtChk = formcoll.GetValues("boolDIVPYMTFLG");
                            string[] instDt = formcoll.GetValues("InsttDt");
                            string[] instNo = formcoll.GetValues("InstNo");
                            string[] divAmt = formcoll.GetValues("Div_Amount");


                            for (int i = 0; i < boolDivPymtChk.Length; i++)
                            {
                                string divchk = boolDivPymtChk[i];
                                if (divchk == "1")
                                {
                                    TransactionPaymentList master = new TransactionPaymentList();
                                    Int32 PaymentListID = 0;

                                    //master.PaymentListID = PaymentListID;
                                    master.ProductTypeTypeID = 29;
                                    if (formcoll["SubscriptionID"] == "" || formcoll["SubscriptionID"] == null)
                                        master.ProductID = 0;
                                    else
                                        master.ProductID = Convert.ToInt32(formcoll["SubscriptionID"]);
                                    int trantypeid = 0;
                                    ChitSubscription chts = new ChitSubscription();
                                    chts = context.chitsubscriptions.Find(master.ProductID);
                                    trantypeid = 342;
                                    master.TransactionTypeTypeID = trantypeid;
                                    master.TransactionCategoryTypeID = 1014;


                                    master.TransactionModeTypeID = 345;

                                    master.Remarks = "Auto Dividend Generation";

                                    if (instDt[i] != "" && instDt[i] != null)
                                        master.GenerratedDate = Convert.ToDateTime(instDt[i]);
                                    else
                                        master.GenerratedDate = DateTime.Now.Date;

                                    if (instNo[i] == "" || instNo[i] == null)
                                        master.Installment = 0;
                                    else
                                        master.Installment = Convert.ToInt32(formcoll["Installment"]);

                                    master.StatusTypeID = 1036;
                                    if (divAmt[i] == "" || divAmt[i] == null)
                                        master.GeneratedAmount = 0;
                                    else
                                        master.GeneratedAmount = Convert.ToDouble(divAmt[i]);

                                    master.ChequeDate = null;
                                    master.ChequeNo = null;
                                    master.ChequeBankName = null;
                                    master.ChequeRealizedDate = null;
                                    master.FundAccountID = null;
                                    master.FundAccountID = chts.FundAccountID;


                                    master.GSTReq = 0;
                                    //if (formcoll["GSTReq"] != null && formcoll["GSTReq"] != "")
                                    //master.GSTReq = Convert.ToInt32(formcoll["GSTReq"]);
                                    master.GSTPerc = 0;
                                    //if (formcoll["GSTPerc"] != "" && formcoll["GSTPerc"] != null)
                                    //master.GSTPerc = Convert.ToDecimal(formcoll["GSTPerc"]);
                                    master.GSTAmt = 0;
                                    //if (formcoll["GSTAmt"] != "" && formcoll["GSTAmt"] != null)
                                    //master.GSTAmt = Convert.ToDecimal(formcoll["GSTAmt"]);
                                    master.GrossAmt = 0;
                                    //if (formcoll["GrossAmt"] != "" && formcoll["GrossAmt"] != null)
                                        //master.GrossAmt = Convert.ToDecimal(formcoll["GrossAmt"]);


                                    context.trnpymtlst.Add(master);
                                    context.SaveChanges();

                                    PaymentListID = master.PaymentListID;

                                    string updqry = "exec pr_Update_Payment_Allocation @paymtlstid = " + master.PaymentListID + ",@status =" + master.StatusTypeID;
                                    context.Database.ExecuteSqlCommand(updqry);
                                }




                            }
                            trans1.Commit();
                            status = "Success";
                        }
                        catch (Exception ex)
                        {
                            var ermsg = ex.Message;
                            //Response.Write(ex.Message.ToString());
                            trans1.Rollback();
                            //Response.Redirect("/Error/AccessDenied");
                            //return RedirectToAction("Index", "ChitSubscriptionManagement");
                            status = "Error";

                        }
                    }
                }
            }
            return Json(status, JsonRequestBehavior.AllowGet);

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
                    string colagt= Convert.ToString(Session["ColAgent"]);
                    if (colagt == "" || colagt == null)
                        colagt = "0";
                    if (crole == "SuperAdmin" || crole == "Admin")
                    {
                        cusrid = "";
                    }

                    var data = e.pr_Search_Product_ChitSubcription(param.sSearch,
                                                    Convert.ToInt32(Request["iSortCol_0"]),
                                                    Request["sSortDir_0"],
                                                    param.iDisplayStart,
                                                    param.iDisplayStart + param.iDisplayLength,
                                                    totalRowsCount,
                                                    filteredRowsCount, Convert.ToString(cusrid),  Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]),
                                                    Convert.ToInt32(colagt),0);
                    var aaData = data.Select(d => new
                    {
                        
                        ClientId = d.ClientId.ToString(),
                        ClientType = d.ClientType,
                        ClientName = d.ClientName,
                        IntervalFreqTypeDesc = d.IntervalFreqTypeDesc,
                        ChitSchemeCode = d.ChitSchemeCode,
                        ChitSchemeName = d.ChitSchemeName,
                        ChitGroupCode = d.ChitGroupCode,
                        ChitGroupName = d.ChitGroupName,
                        ChitValue = d.ChitValue.ToString(),
                        DurationTypeDesc = d.DurationTypeDesc,
                        StatusTypeDesc = d.StatusTypeDesc,
                        SubscriptionCode = d.SubscriptionCode,
                        SubsctiptionID = d.SubscriptionID.ToString(),
                        SubsDate = d.SubsDate,
                        ClientContactNos = d.ClientContactNos,
                        CollectionAgent = d.CollectionAgent
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

                            ChitSubscription master = new ChitSubscription();

                            Int32 ChitSubscriptionID = 0;

                            string ChitSubscriptionid = Convert.ToString(formcoll["SubscriptionID"]);
                            if (ChitSubscriptionid == "" || ChitSubscriptionid == null)
                            {
                                ChitSubscriptionID = 0;
                            }
                            else
                            {
                                ChitSubscriptionID = Convert.ToInt32(ChitSubscriptionid);
                            }
                            
                            //master.ChitSubscriptionCode = "C" + ChitSubscriptionID.ToString();
                            if (ChitSubscriptionID != 0)
                            {
                                master = context.chitsubscriptions.Find(ChitSubscriptionID);
                                //master.ChitSubscriptionCode = "C" + ChitSubscriptionID.ToString();
                            }
                            master.SubscriptionID = ChitSubscriptionID;
                            
                            master.PersonID = Convert.ToInt32(formcoll["PersonID"]);
                            master.ChitGroupID = Convert.ToInt32(formcoll["ChitGroupID"]);
                            int ChitGroupTypeID = 0;
                            ChitGroupTypeID = Convert.ToInt32(formcoll["GroupTypeID"]);
                            int PrevChitGroupID = 0;
                            if(formcoll["edChitGroupID"]!="")
                                PrevChitGroupID = Convert.ToInt32(formcoll["edChitGroupID"]);
                            
                            master.TicketNumber = "1";
                            if (formcoll["TicketNumber"].Trim() != "" && formcoll["TicketNumber"].Trim() != "0" && formcoll["TicketNumber"] != null)
                                master.TicketNumber = Convert.ToString(formcoll["TicketNumber"]);
                            master.LinkedOfficeID = Convert.ToInt32(formcoll["LinkedOfficeID"]);
                            master.FundAccountID = 0;
                            if (formcoll["FundAccountID"] != "" && formcoll["FundAccountID"] != null)
                                master.FundAccountID = Convert.ToInt32(formcoll["FundAccountID"]);

                            master.CommisionAgentPersonID = 0;
                            if (formcoll["CommisionAgentPersonID"] != "" && formcoll["CommisionAgentPersonID"] != null)
                                master.CommisionAgentPersonID = Convert.ToInt32(formcoll["CommisionAgentPersonID"]);
                            master.CommisionAgentPersonID2 = 0;
                            if (formcoll["CommisionAgentPersonID2"] != "" && formcoll["CommisionAgentPersonID2"] != null)
                                master.CommisionAgentPersonID2 = Convert.ToInt32(formcoll["CommisionAgentPersonID2"]);

                            master.CollectionAgentPersonID1 = 0;
                            if (formcoll["CollectionAgentPersonID1"] != "" && formcoll["CollectionAgentPersonID1"] != null)
                                master.CollectionAgentPersonID1 = Convert.ToInt32(formcoll["CollectionAgentPersonID1"]);
                            master.CollectionAgentPersonID2 = 0;
                            if (formcoll["CollectionAgentPersonID2"] != "" && formcoll["CollectionAgentPersonID2"] != null)
                                master.CollectionAgentPersonID2 = Convert.ToInt32(formcoll["CollectionAgentPersonID2"]);

                            master.ChitSchemeID = Convert.ToInt32(formcoll["ChitSchemeID"]);
                            //if (master.ChitSchemeID > 0)
                            //{
                            //    ChitScheme cs = context.chitscheme.Find(master.ChitSchemeID);
                            //    master.ChitSubscriptionCode = cs.ChitSchemeCode;
                            //}
                            //else
                            //    master.ChitSubscriptionCode = "";
                            
                            master.CollectionTypeID = Convert.ToInt32(formcoll["CollectionTypeID"]);
                            master.CollectionModeTypeID = Convert.ToInt32(formcoll["CollectionModeTypeID"]);
                            master.CollectionIntervalTypeID = Convert.ToInt32(formcoll["CollectionIntervalTypeID"]);
                            master.BookPickupDay = Convert.ToInt32(formcoll["BookPickupDay"]);
                            
                            string StartDate = Convert.ToString(formcoll["StartDate"]);
                            master.StartDate = null;
                            if (StartDate != null && StartDate != "")
                                master.StartDate = Convert.ToDateTime(formcoll["StartDate"]);

                            string EndDate = Convert.ToString(formcoll["EndDate"]);
                            master.EndDate = null;
                            if (EndDate != null && EndDate != "")
                                master.EndDate = Convert.ToDateTime(formcoll["EndDate"]);

                            string CommencementTime = Convert.ToString(formcoll["CommencementTime"]);
                            master.CommencementTime = null;
                            if (CommencementTime != null && CommencementTime != "" && CommencementTime != "00:00:00" && CommencementTime != "00:00")
                                master.CommencementTime = Convert.ToString(formcoll["CommencementTime"]);

                            string CommencementDate = Convert.ToString(formcoll["CommencementDate"]);
                            master.CommencementDate = null;
                            if (CommencementDate != null && CommencementDate != "")
                                master.CommencementDate = Convert.ToDateTime(formcoll["CommencementDate"]);

                            if (formcoll["RouteID"] == "" || formcoll["RouteID"] == null)
                                master.RouteID = 0;
                            else
                                master.RouteID = Convert.ToInt32(formcoll["RouteID"]);
                            if (formcoll["TicketPaidNumber"] == "" || formcoll["TicketPaidNumber"] == null)
                                master.TicketPaidNumber = 0;
                            else
                                master.TicketPaidNumber = Convert.ToInt32(formcoll["TicketPaidNumber"]);
                            if (formcoll["StatusTypeID"] == "" || formcoll["StatusTypeID"] == null)
                                master.StatusTypeID = 0;
                            else
                                master.StatusTypeID = Convert.ToInt32(formcoll["StatusTypeID"]);

                            


                            master.OrganizationID = 0;
                            ClientMaster cm = new ClientMaster();
                            cm = context.clientmasters.Find(master.PersonID);                            
                            
                            if  (cm.CLIENTTID==1)
                                master.SubscriberIsPerson = 1;
                            

                            master.StatusTypeID = Convert.ToInt16(formcoll["StatusTypeID"]);

                            if (ChitSubscriptionID == 0)
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
                                    ChitGroup cg = new ChitGroup();
                                    int cgid = 0;
                                    cgid = master.ChitGroupID;
                                    cg = context.chitgroups.Find(cgid);
                                    master.SCNO = Convert.ToInt32(Autonumber.autonum("ChitSubscription", "SCNO", "SCNO <> 0 and ChitSubscriptioncode like '"+ Session["COMPCODE"].ToString().Trim() +"/" + Convert.ToString(cg.ChitGroupCode) + "/%'").ToString());
                                    int scno = Convert.ToInt32(master.SCNO);
                                    string prfx = string.Format("{0:D2}", scno);                                    
                                    
                                    
                                    string aprfx = ""+ Session["COMPCODE"].ToString().Trim() +"/" + cg.ChitGroupCode + "/" + prfx;
                                    master.ChitSubscriptionCode = aprfx.ToString();
                                    context.chitsubscriptions.Add(master);
                                    context.SaveChanges();
                                    ChitSubscriptionID = master.SubscriptionID;
                                    //master.ChitSubscriptionCode = "C" + ChitSubscriptionID.ToString();
                                    //context.Entry(master).State = System.Data.Entity.EntityState.Modified;
                                    //context.SaveChanges();
                                    status = "Success";
                                }

                            }
                            else
                            {
                                //master.UpdatedBy = Session["CUSRID"].ToString();
                                //master.UpdatedDt = DateTime.Now;
                                if(PrevChitGroupID != master.ChitGroupID)
                                {
                                    ChitGroup cg = new ChitGroup();
                                    int cgid = 0;
                                    cgid = master.ChitGroupID;
                                    cg = context.chitgroups.Find(cgid);
                                    master.SCNO = Convert.ToInt32(Autonumber.autonum("ChitSubscription", "SCNO", "SCNO <> 0 and ChitSubscriptioncode like '"+ Session["COMPCODE"].ToString().Trim() +"/" + Convert.ToString(cg.ChitGroupCode) + "/%'").ToString());
                                    int scno = Convert.ToInt32(master.SCNO);
                                    string prfx = string.Format("{0:D2}", scno);


                                    string aprfx = ""+ Session["COMPCODE"].ToString().Trim() +"/" + cg.ChitGroupCode + "/" + prfx;
                                    master.ChitSubscriptionCode = aprfx.ToString();

                                }

                                context.Entry(master).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                status = "Success";
                            }
                            trans1.Commit();
                            ChitSubscriptionID = master.SubscriptionID;
                            

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
        public JsonResult Get_Introduced_by_Name(int id)
        {
            string cqry = "Select b.* from ClientMaster a(nolock) join ClientMaster b(nolock) on a.refclientid = b.clientid  WHERE a.CLIENTID = " + Convert.ToInt32(id);
            var result = context.Database.SqlQuery<ClientMaster>(cqry).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Get_Scheme_Name_Details(int id)
        {
            var result = context.Database.SqlQuery<pr_Chit_Subscription_Scheme_Detail_Assgn_Result>("Exec pr_Chit_Subscription_Scheme_Detail_Assgn @PChitSchemeID = " + Convert.ToInt32(id)).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Get Chit Group Members Count Details
        public JsonResult GetChitGroupMemebersDtl(string id)
        {

            int ID = 0;
            if (id != "null" && id != "") { ID = Convert.ToInt32(id); }

            var result = context.Database.SqlQuery<pr_Get_ChitGroup_MemberDtl_Result>("exec pr_Get_ChitGroup_MemberDtl @ChitGrpid = " + ID + " ").ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Selected Chit Scheme - Chit Groups Details
        public JsonResult GetSchemeChitGroupsDtl(string id)
        {

            int ID = 0;
            if (id != "null" && id != "") { ID = Convert.ToInt32(id); }

            var result = context.Database.SqlQuery<ChitGroup>("Select * From ChitGroup (nolock) Where ChitSchemeID = " + ID + " ").ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

         #region Get Default Chit Subscription Details
        public JsonResult GetDefaultSubsDtl(string id)
        {

            int ID = 0;
            if (id != "null" && id != "") { ID = Convert.ToInt32(id); }

            var result = context.Database.SqlQuery<ChitSubscription>("Select * From ChitSubscripion (nolock) Where SubscriptionID = " + ID + " ").ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Get Schedule & Collection Detail
        public string GetScheduleDtl(string id)
        {
            int SubsID = 0;
            var tabl = "";
            if (id != "" && id != null)
            {
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

                    var data = e.pr_Get_Chit_Collection_Pattern_n_Actuals_Details(SubsID, cusrid);


                    var count = 0;
                    tabl = "<Table id=CHITSCHEDULE_TDETAIL class='table table-striped table-bordered bootstrap-datatable CHITSCHEDULEDETAIL'> <thead><tr><th>Inst.No </th><th>Inst.Dt</th><th>Subs.Amt.</th><th>Div.Amt.</th><th>To be Paid</th><th>Collection Dt</th><th>Colln. Amt</th><th>Balance Actuals</th><th>Collection Sts.</th></tr></thead><tbody id='chitschedule_tbody'>";
                    var query = context.Database.SqlQuery<pr_Get_Chit_Collection_Pattern_n_Actuals_Details_Result>("EXEC pr_Get_Chit_Collection_Pattern_n_Actuals_Details @ChitSubsID =" + SubsID + ", @usrid = '" + cusrid + "'").ToList();
                    foreach (var rslt in query)
                    {
                        tabl = tabl + "<tr>";
                        tabl = tabl + "<td><input style='width:60px;' type=text readonly id=InstNo value='" + rslt.InstNo + "' class='form-control InstNo' name=InstNo maxlength=5 tabindex='105'></td>";
                        tabl = tabl + "<td><input style='width:95px;' type=text readonly id=InsttDt value='" + rslt.InsttDt + "' class='form-control InsttDt' name=InsttDt maxlength=15 tabindex='105'></td>";
                        tabl = tabl + "<td><input style='width:125px;' type=text readonly id=Subs_Amount class='form-control Subs_Amount' name=Subs_Amount value='" + rslt.Subs_Amount + "' maxlength=25 tabindex='105'></td>";
                        tabl = tabl + "<td><input style='width:125px;' type=text readonly id=Div_Amount class='form-control Div_Amount' name=Div_Amount value='" + rslt.Div_Amt + "' maxlength=25 tabindex='105'></td>";
                        tabl = tabl + "<td><input style='width:125px;' type=text readonly id=ToBePaid_Amt class='form-control ToBePaid_Amt' name=ToBePaid_Amt value='" + rslt.ToBePaid_Amt + "' maxlength=25 tabindex='105'></td>";
                        tabl = tabl + "<td><input style='width:95px;' type=text readonly id=collectiondt class='form-control collectiondt' name=collectiondt value='" + rslt.collectiondt + "' maxlength=25 tabindex='105'></td>";
                        tabl = tabl + "<td><input style='width:125px;' type=text readonly id=Credit class='form-control Credit' name=Credit value='" + rslt.Credit + "' maxlength=25 tabindex='105'></td>";
                        tabl = tabl + "<td><input style='width:125px;' type=text readonly id=colbalance class='form-control colbalance' name=Credit value='" + rslt.colbalance + "' maxlength=25 tabindex='105'></td>";
                        tabl = tabl + "<td><input type=text readonly id=CollectionSts class='form-control CollectionSts' name=CollectionSts value='" + rslt.CollectionSts + "' maxlength=25 tabindex='105'></td>";
                        tabl = tabl + "</tr>";
                        count++;
                    }
                    tabl = tabl + "</tbody></table>";



                }

            }
            return tabl;

        }
        #endregion

        #region SSaveData
        [HttpPost]
        public ActionResult SSaveData(FormCollection formcoll)
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

                            ChitSubscription master = new ChitSubscription();

                            Int32 SubscriptionID = 0;

                            string subscriptionId = Convert.ToString(formcoll["SubscriptionID"]);
                            string StartDate = Convert.ToString(formcoll["StartDate"]);
                            if (StartDate == null)
                                StartDate = Convert.ToString(formcoll["CommencementDate"]);

                            string EndDate = Convert.ToString(formcoll["EndDate"]);
                            if (EndDate == null)
                                EndDate = Convert.ToString(formcoll["EndDate"]);

                            string CommencementTime = Convert.ToString(formcoll["CommencementTime"]);
                            if (CommencementTime == null)
                                CommencementTime = Convert.ToString(formcoll["CommencementTime"]);

                            string CommencementDate = Convert.ToString(formcoll["CommencementDate"]);
                            if (CommencementDate == null)
                                CommencementDate = Convert.ToString(formcoll["CommencementDate"]);

                            string ClosureDate = "";
                            string FirstInstalmentDate = "";

                            if (subscriptionId == "" || subscriptionId == null)
                            {
                                SubscriptionID = 0;
                            }
                            else
                            {
                                SubscriptionID = Convert.ToInt32(subscriptionId);
                            }

                            if (SubscriptionID != 0)
                            {
                                master = context.chitsubscriptions.Find(SubscriptionID);
                            }
                            
                            master.PersonID = Convert.ToInt32(formcoll["PersonID"]);
                            master.ChitGroupID = Convert.ToInt32(formcoll["ChitGroupID"]);
                            master.ChitSchemeID = Convert.ToInt32(formcoll["ChitSchemeID"]);
                            master.ChitGroupID = Convert.ToInt32(formcoll["ChitGroupID"]);
                            master.CollectionTypeID = Convert.ToInt32(formcoll["CollectionTypeID"]);
                            master.CollectionModeTypeID = Convert.ToInt32(formcoll["CollectionModeTypeID"]);

                            master.CollectionIntervalTypeID = Convert.ToInt32(formcoll["CollectionIntervalTypeID"]);
                            master.LinkedOfficeID = Convert.ToInt32(formcoll["LinkedOfficeID    "]);
                            master.RouteID = Convert.ToInt32(formcoll["RouteID"]);
                            master.NomineePersonID = Convert.ToInt32(formcoll["NomineePersonID"]);
                            master.NomineeRelationshipTypeID = Convert.ToInt32(formcoll["NomineeRelationshipTypeID"]);
                            master.Witness1PersonID = Convert.ToInt32(formcoll["Witness1PersonID"]);
                            master.Witness2PresonID = Convert.ToInt32(formcoll["Witness2PresonID"]);

                            
                            master.OrganizationID = Convert.ToInt32(formcoll["OrganizationID"]);

                            master.SubscriberIsPerson = 1;
                            master.CommisionAmount = Convert.ToDouble(formcoll["CommisionAmount"]);
                            master.CommencementDate = Convert.ToDateTime(formcoll["CommencementDate"]);
                            if (ClosureDate != "")
                            {
                                master.ClosureDate = Convert.ToDateTime(formcoll["ClosureDate"]);
                            }
                            else
                            {
                                master.ClosureDate = null;
                            }

                            if (FirstInstalmentDate != "")
                            {
                                master.FirstInstalmentDate = Convert.ToDateTime(formcoll["FirstInstalmentDate"]);
                            }
                            else
                            {
                                master.FirstInstalmentDate = null;
                            }

                            master.BookPickupDay = Convert.ToInt32(formcoll["BookPickupDay"]);
                            master.FundAccountID = Convert.ToInt32(formcoll["FundAccountID"]);
                            master.StatusTypeID = Convert.ToInt32(formcoll["StatusTypeID"]);

                            master.AssociatedOrganizationID = Convert.ToInt32(formcoll["AssociatedOrganizationID"]);
                            master.LinkedAddressTypeID = Convert.ToInt32(formcoll["LinkedAddressTypeID"]);
                            if (StartDate !="")
                            {
                                master.StartDate = Convert.ToDateTime(formcoll["StartDate"]);
                            }
                            else
                            {
                                master.StartDate = null;
                            }

                            if (EndDate != "")
                            {
                                master.EndDate = Convert.ToDateTime(formcoll["EndDate"]);
                            }
                            else
                            {
                                master.EndDate = null;
                            }

                            master.ChitSubscriptionCode = formcoll["ChitSubscriptionCode"];
                            master.TallyCode = formcoll["TallyCode"];
                            master.AccountEmployeeID = Convert.ToInt32(formcoll["AccountEmployeeID"]);
                            master.DepositAccountID = Convert.ToInt32(formcoll["DepositAccountID"]);
                            master.TicketPaidNumber = Convert.ToInt32(formcoll["TicketPaidNumber"]);
                            master.CommisionAgentPersonID = Convert.ToInt32(formcoll["CommisionAgentPersonID"]);
                            master.CommisionAgentPersonID2 = Convert.ToInt32(formcoll["CommisionAgentPersonID2"]);

                            master.CollectionAgentPersonID1 = Convert.ToInt32(formcoll["CollectionAgentPersonID1"]);
                            master.CollectionAgentPersonID2 = Convert.ToInt32(formcoll["CollectionAgentPersonID2"]);

                            string cdatetime = "";

                            cdatetime = CommencementDate + " " + CommencementTime;
                            master.CommencementDate = Convert.ToDateTime(cdatetime);

                            if (SubscriptionID == 0)
                            {
                                //string squery = "SELECT *FROM ChitGroup (nolock) WHERE ChitGroupCode ='" + master.ChitGroupCode + "' or ChitGroupName ='" + master.ChitGroupName + "'";
                                //var query = context.Database.SqlQuery<ChitGroup>(squery).ToList();

                                //if (query.Count != 0)
                                //{
                                //    status = "Existing";
                                //}
                                //else
                                //{
                                    context.chitsubscriptions.Add(master);
                                    context.SaveChanges();
                                    status = "Success";

                                //}

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
                            SubscriptionID = master.SubscriptionID;



                            ViewBag.Message = "Form submitted.";
                            // Response.Redirect("Index");
                            //return RedirectToAction("Index", "ChitGroupManagement");
                            //Response.Write("Update");
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            var ermsg = ex.Message;
                            //Response.Write(ex.Message.ToString());
                            trans1.Rollback();
                            //Response.Redirect("/Error/AccessDenied");
                            //return RedirectToAction("Index", "ChitGroupManagement");
                            status = "Error";
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

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




        //#region Chit Ledger Report Output
        //public string GetChitLedgerRpt(string id)
        //{
        //    try
        //    {

        //        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IdentityCon"].ToString()))
        //        {

        //            conn.Open();

        //            SqlCommand cmd = new SqlCommand();
        //            SqlDataAdapter da = new SqlDataAdapter("[dbo].[CustomerAccountLedgerbyPcode]", conn);


        //            da.SelectCommand.CommandType = CommandType.StoredProcedure;
        //            da.SelectCommand.CommandTimeout = 0;
        //            da.SelectCommand.Parameters.Add(new SqlParameter("@Code", SqlDbType.VarChar));
        //            //da.SelectCommand.Parameters["@Code"].Value = "C" + id.Trim();
        //            ChitSubscription cg = new ChitSubscription();
        //            cg = context.chitsubscriptions.Find(Convert.ToInt32(id));
        //            da.SelectCommand.Parameters["@Code"].Value = cg.ChitSubscriptionCode;

        //            DataSet ds = new DataSet();
        //            StringBuilder html0 = new StringBuilder();
        //            StringBuilder html1 = new StringBuilder();
        //            StringBuilder html2 = new StringBuilder();
        //            StringBuilder html3 = new StringBuilder();
        //            StringBuilder sb = new StringBuilder();
        //            da.Fill(ds, "L1");



        //            //Building the Header row.

        //            if (ds.Tables[2].Rows.Count > 0)
        //            {
        //                html0.Append("<table class='table datatable'>");
        //                html0.Append("<tr>");
        //                html0.Append("<th> Client Name</th>");
        //                html0.Append("<td> " + ds.Tables[2].Rows[0][4].ToString() + "</td>");
        //                html0.Append("<th> Chit Group <th>");
        //                html0.Append("<td align='left'> " + ds.Tables[2].Rows[0][2].ToString() + "</td>");
        //                html0.Append("<th> Chit Value <th>");
        //                html0.Append("<td> " + ds.Tables[2].Rows[0][0].ToString() + "</td>");
        //                html0.Append("</tr><tr>");

        //                html0.Append("<th> Address <th>");
        //                html0.Append("<td colspan='3' align='left'> " + ds.Tables[2].Rows[0][5].ToString() + "</td>");
        //                html0.Append("<th> No.of Tickets <th>");
        //                html0.Append("<td> " + ds.Tables[2].Rows[0][1].ToString() + "</td>");

        //                html0.Append("</tr>");
        //                html0.Append("</table>");
        //            }

        //            if (ds.Tables[3].Rows.Count > 0)
        //            {


        //                //Table start.
        //                html1.Append("<table class='table table-striped datatable'>");

        //                //Building the Header row.
        //                html1.Append("<tr>");
        //                foreach (DataColumn column in ds.Tables[3].Columns)
        //                {
        //                    html1.Append("<th>");
        //                    html1.Append(column.ColumnName);
        //                    html1.Append("</th>");
        //                }
        //                html1.Append("</tr>");

        //                //Building the Data rows.
        //                foreach (DataRow row in ds.Tables[3].Rows)
        //                {
        //                    html1.Append("<tr>");
        //                    foreach (DataColumn column in ds.Tables[3].Columns)
        //                    {
        //                        html1.Append("<td>");
        //                        if (column.ColumnName == "Totals")
        //                        {
        //                            html1.Append(Convert.ToDecimal(row[column.ColumnName]).ToString("#,##,###0.00"));
        //                        }
        //                        else
        //                        {
        //                            html1.Append(row[column.ColumnName]);
        //                        }

        //                        html1.Append("</td>");
        //                    }
        //                    html1.Append("</tr>");
        //                }

        //                //Table end.
        //                html1.Append("</table>");

        //                //Append the HTML string to Placeholder.

        //            }


        //            if (ds.Tables[4].Rows.Count > 0)
        //            {


        //                //Table start.
        //                html2.Append("<table class='table table-striped datatable'>");

        //                //Building the Header row.
        //                html2.Append("<tr>");
        //                foreach (DataColumn column in ds.Tables[4].Columns)
        //                {
        //                    html2.Append("<th>");
        //                    html2.Append(column.ColumnName);
        //                    html2.Append("</th>");
        //                }
        //                html2.Append("</tr>");

        //                //Building the Data rows.
        //                foreach (DataRow row in ds.Tables[4].Rows)
        //                {
        //                    html2.Append("<tr>");
        //                    foreach (DataColumn column in ds.Tables[4].Columns)
        //                    {
        //                        html2.Append("<td>");
        //                        if (column.ColumnName == "Value")
        //                        {
        //                            html2.Append(Convert.ToDecimal(row[column.ColumnName]).ToString("#,##,###0.00"));
        //                        }
        //                        else
        //                        {
        //                            html2.Append(row[column.ColumnName]).ToString();
        //                        }
        //                        html2.Append("</td>");
        //                    }
        //                    html2.Append("</tr>");
        //                }

        //                //Table end.
        //                html2.Append("</table>");

        //                //Append the HTML string to Placeholder.

        //            }

        //            if (ds.Tables[5].Rows.Count > 0)
        //            {



        //                //Table start.
        //                html3.Append("<table class='table table-striped datatable'>");

        //                //Building the Header row.
        //                html3.Append("<tr>");
        //                foreach (DataColumn column in ds.Tables[5].Columns)
        //                {
        //                    html3.Append("<th>");
        //                    html3.Append(column.ColumnName);
        //                    html3.Append("</th>");
        //                }
        //                html3.Append("</tr>");

        //                //Building the Data rows.
        //                foreach (DataRow row in ds.Tables[5].Rows)
        //                {
        //                    html3.Append("<tr>");
        //                    foreach (DataColumn column in ds.Tables[5].Columns)
        //                    {
        //                        html3.Append("<td>");
        //                        if (row[column.ColumnName].ToString().Contains("close"))
        //                        {
        //                            html3.Append(row[column.ColumnName]).ToString();
        //                        }
        //                        else
        //                        {
        //                            html3.Append(row[column.ColumnName]).ToString();
        //                        }

        //                        html3.Append("</td>");
        //                    }
        //                    html3.Append("</tr>");
        //                }

        //                //Table end.
        //                html3.Append("</table>");

        //                //Append the HTML string to Placeholder.

        //            }



        //            if (ds.Tables.Count != 0)
        //            {
        //                if (ds.Tables[0].Rows.Count > 0)
        //                {

        //                    //Table start.
        //                    sb.Append("<table   class='table table-striped dataTable ExportDT' id='ExportDT' cellpadding='5' cellspacing='0' style='border: 1px solid #ccc;font-size: 9pt;font-family:Arial'>");

        //                    //Adding HeaderRow.
        //                    sb.Append("<tr>");
        //                    foreach (DataColumn column in ds.Tables[0].Columns)
        //                    {
        //                        sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>" + column.ColumnName + "</th>");
        //                    }
        //                    sb.Append("</tr>");

        //                    var clr = "";
        //                    //Adding DataRow.
        //                    foreach (DataRow row in ds.Tables[0].Rows)
        //                    {
        //                        sb.Append("<tr>");
        //                        foreach (DataColumn column in ds.Tables[0].Columns)
        //                        {

        //                            var clnname = column.ColumnName;
        //                            if (row[column.ColumnName].ToString().Contains("-"))
        //                            {
        //                                sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;color:red;'>" + row[column.ColumnName].ToString() + "</td>");
        //                            }
        //                            else if (row[column.ColumnName].ToString().Contains("Dividend") || row[column.ColumnName].ToString().Contains("Company") || row[column.ColumnName].ToString().Contains("Interest"))
        //                            {
        //                                clr = "red";
        //                                sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;color:" + clr + ";'>" + row[column.ColumnName].ToString() + "</td>");
        //                            }
        //                            else
        //                            {
        //                                if (clnname == "Debit" || clnname == "Credit" || clnname == "Cumilative")
        //                                {
        //                                    sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;color:" + clr + ";'>" + Convert.ToDecimal(row[column.ColumnName]).ToString("#,##,###0.00") + "</td>");
        //                                }
        //                                else
        //                                {
        //                                    sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right'>" + row[column.ColumnName].ToString() + "</td>");
        //                                    clr = "";
        //                                }

        //                            }
        //                        }
        //                        sb.Append("</tr>");
        //                    }

        //                    //Adding DataRow.

        //                    foreach (DataRow row in ds.Tables[1].Rows)
        //                    {
        //                        sb.Append("<tr>");
        //                        foreach (DataColumn column in ds.Tables[1].Columns)
        //                        {
        //                            if (row[column.ColumnName].ToString().Contains("-"))
        //                                sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;background-color:#c5eafa;color:red;'>" + row[column.ColumnName].ToString() + "</td>");
        //                            else if (column.ColumnName == "value" || column.ColumnName == "BalanceTotal" || column.ColumnName == "Total")
        //                            {
        //                                sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;background-color:#c5eafa;'>" + Convert.ToDecimal(row[column.ColumnName]).ToString("#,##,###0.00") + "</td>");
        //                            }
        //                            else
        //                                sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;background-color:#c5eafa;'>" + row[column.ColumnName].ToString() + "</td>");
        //                        }
        //                        sb.Append("</tr>");
        //                    }

        //                    //Table end.
        //                    sb.Append("</table>");

        //                }
        //                //else
        //                //{

        //                //    //Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "Notification", "OpenAfterSave('No Records Found!!')", true);
        //                //}
        //            }
        //            //else
        //            //{
        //            //    ltTable.Text = string.Empty;
        //            //    Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "Notification", "OpenAfterSave('No Records Found!!')", true);
        //            //}

        //            html0 = html0.Append("<table class='table datatable'><tr><td valign='top'>");
        //            html0 = html0.Append(html1);

        //            html0 = html0.Append("</td><td valign='top'>");
        //            html0 = html0.Append(html2);
        //            html0 = html0.Append("</td><td valign='top'>");
        //            html0 = html0.Append(html3);
        //            html0 = html0.Append("</td></tr></table>");
        //            html0 = html0.Append(sb);
        //            return html0.ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string status = "error";
        //        return status;
        //    }

        //}

        //#endregion

        #region Chit Ledger Report Output
        public string GetChitLedgerRpt(string id)
        {
            try
            {

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IdentityCon"].ToString()))
                {

                    conn.Open();

                    SqlCommand cmd = new SqlCommand();
                    //SqlDataAdapter da = new SqlDataAdapter("[dbo].[CustomerAccountLedgerbyPcode_BVFcn]", conn);
                    SqlDataAdapter da = new SqlDataAdapter("[dbo].[CustomerAccountLedgerbyPcode]", conn);


                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.CommandTimeout = 0;
                    da.SelectCommand.Parameters.Add(new SqlParameter("@Code", SqlDbType.VarChar));
                    //da.SelectCommand.Parameters["@Code"].Value = "C" + id.Trim();
                    ChitSubscription cg = new ChitSubscription();
                    cg = context.chitsubscriptions.Find(Convert.ToInt32(id));
                    da.SelectCommand.Parameters["@Code"].Value = cg.ChitSubscriptionCode;

                    DataSet ds = new DataSet();
                    StringBuilder html0 = new StringBuilder();
                    StringBuilder html1 = new StringBuilder();
                    StringBuilder html2 = new StringBuilder();
                    StringBuilder html3 = new StringBuilder();
                    StringBuilder sb = new StringBuilder();
                    da.Fill(ds, "L1");



                    //Building the Header row.

                    if (ds.Tables[2].Rows.Count > 0)
                    {
                        html0.Append("<table class='table datatable'>");
                        html0.Append("<tr>");
                        html0.Append("<th> Client Name</th>");
                        html0.Append("<td> " + ds.Tables[2].Rows[0][4].ToString() + "</td>");
                        html0.Append("<th> Chit Group <th>");
                        html0.Append("<td align='left'> " + ds.Tables[2].Rows[0][2].ToString() + "</td>");
                        html0.Append("<th> Chit Value <th>");
                        html0.Append("<td> " + ds.Tables[2].Rows[0][0].ToString() + "</td>");
                        html0.Append("</tr><tr>");

                        html0.Append("<th> Address <th>");
                        html0.Append("<td colspan='3' align='left'> " + ds.Tables[2].Rows[0][5].ToString() + "</td>");
                        html0.Append("<th> No.of Tickets <th>");
                        html0.Append("<td> " + ds.Tables[2].Rows[0][1].ToString() + "</td>");

                        html0.Append("</tr>");
                        html0.Append("</table>");
                    }

                    if (ds.Tables[3].Rows.Count > 0)
                    {


                        //Table start.
                        html1.Append("<table class='table table-striped datatable'>");

                        //Building the Header row.
                        html1.Append("<tr>");
                        foreach (DataColumn column in ds.Tables[3].Columns)
                        {
                            html1.Append("<th>");
                            html1.Append(column.ColumnName);
                            html1.Append("</th>");
                        }
                        html1.Append("</tr>");

                        //Building the Data rows.
                        foreach (DataRow row in ds.Tables[3].Rows)
                        {
                            html1.Append("<tr>");
                            foreach (DataColumn column in ds.Tables[3].Columns)
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

                        //Table end.
                        html1.Append("</table>");

                        //Append the HTML string to Placeholder.

                    }


                    if (ds.Tables[4].Rows.Count > 0)
                    {


                        //Table start.
                        html2.Append("<table class='table table-striped datatable'>");

                        //Building the Header row.
                        html2.Append("<tr>");
                        foreach (DataColumn column in ds.Tables[4].Columns)
                        {
                            html2.Append("<th>");
                            html2.Append(column.ColumnName);
                            html2.Append("</th>");
                        }
                        html2.Append("</tr>");

                        //Building the Data rows.
                        foreach (DataRow row in ds.Tables[4].Rows)
                        {
                            html2.Append("<tr>");
                            foreach (DataColumn column in ds.Tables[4].Columns)
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

                        //Table end.
                        html2.Append("</table>");

                        //Append the HTML string to Placeholder.

                    }

                    if (ds.Tables[5].Rows.Count > 0)
                    {



                        //Table start.
                        html3.Append("<table class='table table-striped datatable'>");

                        //Building the Header row.
                        html3.Append("<tr>");
                        foreach (DataColumn column in ds.Tables[5].Columns)
                        {
                            html3.Append("<th>");
                            html3.Append(column.ColumnName);
                            html3.Append("</th>");
                        }
                        html3.Append("</tr>");

                        //Building the Data rows.
                        foreach (DataRow row in ds.Tables[5].Rows)
                        {
                            html3.Append("<tr>");
                            foreach (DataColumn column in ds.Tables[5].Columns)
                            {
                                html3.Append("<td>");
                                if (row[column.ColumnName].ToString().Contains("close"))
                                {
                                    html3.Append(row[column.ColumnName]).ToString();
                                }
                                else
                                {
                                    html3.Append(row[column.ColumnName]).ToString();
                                }

                                html3.Append("</td>");
                            }
                            html3.Append("</tr>");
                        }

                        //Table end.
                        html3.Append("</table>");

                        //Append the HTML string to Placeholder.

                    }



                    if (ds.Tables.Count != 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {

                            //Table start.
                            sb.Append("<table   class='table table-striped dataTable LedgerDT' id='LedgerDT' cellpadding='5' cellspacing='0' style='border: 1px solid #ccc;font-size: 9pt;font-family:Arial'>");

                            //Adding HeaderRow.
                            sb.Append("<tr>");
                            foreach (DataColumn column in ds.Tables[0].Columns)
                            {
                                if (column.ColumnName != "BkVfcnSts" && column.ColumnName != "BkVfcnDt" && column.ColumnName != "BkVfcnAmt" && column.ColumnName != "TranID")
                                {
                                    sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>" + column.ColumnName + "</th>");
                                }

                            }
                            sb.Append("</tr><tbody>");

                            var clr = "";
                            //Adding DataRow.
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                sb.Append("<tr>");
                                foreach (DataColumn column in ds.Tables[0].Columns)
                                {
                                    clr = "";
                                    var clnname = column.ColumnName;
                                    var bvsts = "";
                                    var bvdt = DateTime.Now;
                                    var bvamt = 0.00;
                                    var chkbxdisbled = "";
                                    var bvstschkd = "";
                                    var bvinfo = "";
                                    var bvtrnid = "";
                                    var bvtrtyp = "";
                                    bvsts = row["BkVfcnSts"].ToString();
                                    bvtrnid = row["TranID"].ToString();

                                    string[] bvtrn = bvtrnid.Split('~');
                                    if (bvtrn.Length > 1)
                                    {
                                        bvtrtyp = bvtrn[0];
                                        bvtrnid = bvtrn[1];
                                    }
                                    if (!string.IsNullOrEmpty(row["BkVfcnDt"].ToString()))
                                        bvdt = Convert.ToDateTime(row["BkVfcnDt"]);
                                    if (row["Category"].ToString().Contains("Dividend") || row["Category"].ToString().Contains("Prize"))
                                        bvamt = Convert.ToDouble(row["Debit"]);
                                    if (row["Category"].ToString().Contains("Subscription"))
                                        bvamt = Convert.ToDouble(row["Credit"]);
                                    if (bvsts != "0")
                                        bvstschkd = "checked";
                                    if (bvsts == "2")
                                        chkbxdisbled = "disabled";
                                    if (bvsts == "2")
                                        bvinfo = "Already Authorized on " + bvdt.ToString() + " , Passed more than 1 week!!!";
                                    else if (bvsts == "1")
                                        bvinfo = "Already Authorized on " + bvdt.ToString() + " , Within 1 week!!!";
                                    else if (bvsts == "0")
                                        bvinfo = "Pending for Authorization!!!";

                                    if (column.ColumnName != "BkVfcnSts" && column.ColumnName != "BkVfcnDt" && column.ColumnName != "BkVfcnAmt" && column.ColumnName != "TranID")
                                    {
                                        if (row[column.ColumnName].ToString().Contains("-"))
                                        {
                                            sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;color:red;'>" + row[column.ColumnName].ToString() + "</td>");
                                        }
                                        else if (row[column.ColumnName].ToString() == ("Dividend") || row[column.ColumnName].ToString().Contains("Company") || row[column.ColumnName].ToString().Contains("Interest") || row[column.ColumnName].ToString().Contains("Prize"))
                                        {
                                            clr = "red";
                                            //sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;color:" + clr + ";'>" + row[column.ColumnName].ToString() + "</td>");
                                            sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;color:" + clr + ";'>" + row[column.ColumnName].ToString());
                                            if (row[column.ColumnName].ToString() == ("Dividend") || row[column.ColumnName].ToString().Contains("Prize"))
                                            {
                                                sb.Append("<input type='checkbox' " + bvstschkd + " title='" + bvinfo + "' class='bvfcnchkbx' onchange='CheckBVFCn(this);' />");
                                                sb.Append("<input type='text'  class='bvsts hide' value='" + bvsts + "' />");
                                                sb.Append("<input type='text'  class='bvtrtype hide' value='" + bvtrtyp + "' />");
                                                sb.Append("<input type='text'  class='bvtrnid hide' value='" + bvtrnid + "' />");
                                            }
                                            sb.Append("</td>");
                                        }
                                        else if (row[column.ColumnName].ToString().Contains("Subscription") || row[column.ColumnName].ToString().Contains("Cancellation"))
                                        {

                                            //sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;color:" + clr + ";'>" + row[column.ColumnName].ToString() + "</td>");
                                            sb.Append("<td style='width:100px;border: 1px solid #ccc;text-align:right;color:" + clr + ";'>" + row[column.ColumnName].ToString());
                                            sb.Append("<input type='checkbox' " + bvstschkd + " title='" + bvinfo + "' class='bvfcnchkbx' onchange='CheckBVFCn(this);' />");
                                            sb.Append("<input type='text'  class='bvsts hide' value='" + bvsts + "' />");
                                            sb.Append("<input type='text'  class='bvtrtype hide' value='" + bvtrtyp + "' />");
                                            sb.Append("<input type='text'  class='bvtrnid hide' value='" + bvtrnid + "' />");
                                            sb.Append("</td>");
                                        }
                                        else
                                        {
                                            if (row["Category"].ToString().Contains("Dividend"))
                                            {
                                                clr = "red";
                                            }
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
                            sb.Append("</tbody></table>");

                        }
                        //else
                        //{

                        //    //Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "Notification", "OpenAfterSave('No Records Found!!')", true);
                        //}
                    }
                    //else
                    //{
                    //    ltTable.Text = string.Empty;
                    //    Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "Notification", "OpenAfterSave('No Records Found!!')", true);
                    //}

                    html0 = html0.Append("<table class='table datatable'><tr><td valign='top'>");
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

        #region Update Book Verification Staus
        public void UpdateBVSts(string id)
        {
            string[] ids = id.Split('~');
            if (ids.Length > 0)
            {
                string trtyp = Convert.ToString(ids[0]);
                int trnid = Convert.ToInt32(ids[1]);
                string sts = Convert.ToString(ids[2]);

                if (trtyp == "C")
                {
                    if (sts.Contains("Authoization Cancelled!!!"))
                    {
                        string updqry = "Update  TransactionCollectionList Set BkVfcnSts = 0,BkVfcnDt=null,BkVfcnAmt=0 Where CollectionListID = " + trnid;
                        context.Database.ExecuteSqlCommand(updqry);
                    }
                    else
                    {
                        string updqry = "Update  TransactionCollectionList Set BkVfcnSts = 1,BkVfcnDt=getdate(),BkVfcnAmt=GeneratedAmount Where CollectionListID = " + trnid;
                        context.Database.ExecuteSqlCommand(updqry);
                    }
                }
                else if (trtyp == "P")
                {
                    if (sts.Contains("Authoization Cancelled!!!"))
                    {
                        string updqry = "Update  Transactionpaymentlist Set BkVfcnSts = 0,BkVfcnDt=null,BkVfcnAmt=0 Where PaymentListID = " + trnid;
                        context.Database.ExecuteSqlCommand(updqry);
                    }
                    else
                    {
                        string updqry = "Update  Transactionpaymentlist Set BkVfcnSts = 1,BkVfcnDt=getdate(),BkVfcnAmt=GeneratedAmount Where PaymentListID = " + trnid;
                        context.Database.ExecuteSqlCommand(updqry);
                    }
                }


            }
            //int SubsID = 0;
            //var tabl = "";
            //if (id != "" && id != null)
            //{
            //    SubsID = Convert.ToInt32(id);

            //    using (var e = new FinFusionEntities())
            //    {
            //        var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));

            //        var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
            //        string crole = Convert.ToString(Session["Group"]);
            //        string cusrid = Convert.ToString(Session["CUSRID"]);
            //        if (crole == "SuperAdmin" || crole == "Admin")
            //        {
            //            cusrid = "";
            //        }

            //        var data = e.pr_Get_Chit_Collection_Pattern_n_Actuals_Details(SubsID, cusrid);


            //        var count = 0;
            //        tabl = "<Table id=CHITSCHEDULE_TDETAIL class='table table-striped table-bordered bootstrap-datatable CHITSCHEDULEDETAIL'> <thead><tr><th>Inst.No </th><th>Inst.Dt</th><th>Subs.Amt.</th><th>Div.Amt.</th><th>To be Paid</th><th>Collection Dt</th><th>Colln. Amt</th><th>Collection Sts.</th></tr></thead><tbody id='chitschedule_tbody'>";
            //        var query = context.Database.SqlQuery<pr_Get_Chit_Collection_Pattern_n_Actuals_Details_Result>("EXEC pr_Get_Chit_Collection_Pattern_n_Actuals_Details @ChitSubsID =" + SubsID + ", @usrid = '" + cusrid + "'").ToList();
            //        foreach (var rslt in query)
            //        {
            //            tabl = tabl + "<tr>";
            //            tabl = tabl + "<td><input style='width:60px;' type=text readonly id=InstNo value='" + rslt.InstNo + "' class='form-control InstNo' name=InstNo maxlength=5 tabindex='105'></td>";
            //            tabl = tabl + "<td><input style='width:95px;' type=text readonly id=InsttDt value='" + rslt.InsttDt + "' class='form-control InsttDt' name=InsttDt maxlength=15 tabindex='105'></td>";
            //            tabl = tabl + "<td><input style='width:125px;' type=text readonly id=Subs_Amount class='form-control Subs_Amount' name=Subs_Amount value='" + rslt.Subs_Amount + "' maxlength=25 tabindex='105'></td>";
            //            tabl = tabl + "<td><input style='width:125px;' type=text readonly id=Div_Amount class='form-control Div_Amount' name=Div_Amount value='" + rslt.Div_Amt + "' maxlength=25 tabindex='105'></td>";
            //            tabl = tabl + "<td><input style='width:125px;' type=text readonly id=ToBePaid_Amt class='form-control ToBePaid_Amt' name=ToBePaid_Amt value='" + rslt.ToBePaid_Amt + "' maxlength=25 tabindex='105'></td>";
            //            tabl = tabl + "<td><input style='width:95px;' type=text readonly id=collectiondt class='form-control collectiondt' name=collectiondt value='" + rslt.collectiondt + "' maxlength=25 tabindex='105'></td>";
            //            tabl = tabl + "<td><input style='width:125px;' type=text readonly id=Credit class='form-control Credit' name=Credit value='" + rslt.Credit + "' maxlength=25 tabindex='105'></td>";
            //            tabl = tabl + "<td><input style='width:125px;' type=text readonly id=colbalance class='form-control colbalance' name=Credit value=" + rslt.colbalance + " maxlength=25 tabindex='105'></td>";
            //            tabl = tabl + "<td><input type=text readonly id=CollectionSts class='form-control CollectionSts' name=CollectionSts value='" + rslt.CollectionSts + "' maxlength=25 tabindex='105'></td>";
            //            tabl = tabl + "</tr>";
            //            count++;
            //        }
            //        tabl = tabl + "</tbody></table>";



            //    }

            //}
            //return tabl;

        }
        #endregion

        #region Delete Record  
        //[Authorize(Roles = "ChitSubscriptionManagementDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check("ChitSubscription", id, "ChitSubscription");
            
            if (temp.Equals("PROCEED"))
            {
                ChitSubscription chitsubscription = context.chitsubscriptions.Find(Convert.ToInt32(id));
                context.chitsubscriptions.Remove(chitsubscription);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion

    }
}
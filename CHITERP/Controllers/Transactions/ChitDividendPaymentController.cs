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
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace CHITERP.Controllers.Transactions
{
    public class ChitDividendPaymentController : Controller
    {
        // GET: ChitDividendPayment

        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        //[Authorize(Roles = "ChitDividendPaymentIndex")]
        public ActionResult Index()
        {

            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            string cusrid = Convert.ToString(Session["CUSRID"]);
            string crole = Convert.ToString(Session["Group"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                cusrid = "";
            }
            var personqry = context.Database.SqlQuery<pr_Chit_Subscription_Organization_Name_Assgn_Result>("Exec pr_Chit_Subscription_Organization_Name_Assgn").ToList();
            ViewBag.FPersonID = new SelectList(personqry, "CLIENTID", "CNAME").ToList();
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
            if (Request.Form.Get("DateBasedOn") != null && Request.Form.Get("DateBasedOn") != "")
            {
                Session["DateBasedOn"] = Request.Form.Get("DateBasedOn");
            }
            else
                Session["DateBasedOn"] = 1;
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
            if (Request.Form.Get("CommissionAgent") != null && Request.Form.Get("CommissionAgent") != "")
            {
                Session["CommissionAgent"] = Request.Form.Get("CommissionAgent");

            }
            else
                Session["CommissionAgent"] = 0;
            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Product', ''").ToList();
            ViewBag.ProductType = new SelectList(mtqry.Where(x => x.Type == "Chit Fund" || x.Type == "Loan"), "TypeID", "Type", Convert.ToInt32(Session["ProductType"])).ToList();
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_DateBasedOn_Options").ToList();
            ViewBag.DateBasedOn = new SelectList(mtqry, "TypeID", "Type", Convert.ToInt32(Session["DateBasedOn"])).ToList();
            var amtcdqry4 = context.Database.SqlQuery<CompanyRoute>("select* from CompanyRoute (nolock)").ToList();
            ViewBag.RouteID = new SelectList(amtcdqry4, "RouteID", "RouteName", Convert.ToInt32(Session["RouteID"]));
            var empqry = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_Get_Employee_Dtl_Rpt @usrid='" + cusrid + "',@desgnid=0").ToList();
            ViewBag.CollectionAgent = new SelectList(empqry, "DValue", "DText", Convert.ToInt32(Session["CollectionAgent"])).ToList();
            empqry = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_Get_Employee_Dtl_Rpt @usrid='" + cusrid + "',@desgnid=1").ToList();
            ViewBag.CommissionAgent = new SelectList(empqry, "DValue", "DText", Convert.ToInt32(Session["CommissionAgent"])).ToList();
            return View();
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
                    string ClientID = "0";
                    string ProdType = Convert.ToString(Session["ProductType"]);
                    string DateBasedOn = Convert.ToString(Session["DateBasedOn"]);
                    string fromdt = Convert.ToDateTime(Session["SDATE"]).ToString("yyyy-MM-dd");
                    string todt = Convert.ToDateTime(Session["EDATE"]).ToString("yyyy-MM-dd"); ;
                    string route = Convert.ToString(Session["RouteID"]);
                    string colagt = Convert.ToString(Session["CollectionAgent"]);
                    string cmsnagt = Convert.ToString(Session["CommissionAgent"]);


                    int clntid = 0;
                    if (ClientID != "" && ClientID != "undefined")
                        clntid = Convert.ToInt32(ClientID);
                    int routeid = 0;
                    if (route != "" && route != "undefined")
                        routeid = Convert.ToInt32(route);
                    int colagtid = 0;
                    if (colagt != "" && colagt != "undefined")
                        colagtid = Convert.ToInt32(colagt);
                    int cmsnagtid = 0;
                    if (cmsnagt != "" && cmsnagt != "undefined")
                        cmsnagtid = Convert.ToInt32(cmsnagt);
                    int prdtid = 0;
                    if (ProdType != "" && ProdType != "undefined")
                        prdtid = Convert.ToInt32(ProdType);
                    int dtbased = 0;
                    if (DateBasedOn != "" && DateBasedOn != "undefined")
                        dtbased = Convert.ToInt32(DateBasedOn);

                    //var data = e.pr_Search_Product_ChitSubcription(param.sSearch,
                    //                                Convert.ToInt32(Request["iSortCol_0"]),
                    //                                Request["sSortDir_0"],
                    //                                param.iDisplayStart,
                    //                                param.iDisplayStart + param.iDisplayLength,
                    //                                totalRowsCount,
                    //                                filteredRowsCount, Convert.ToString(cusrid), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), dtbased, routeid, clntid, "", colagtid, cmsnagtid, 0);

                    var data = e.pr_Search_Product_ChitSubcription(param.sSearch,
                                                    Convert.ToInt32(Request["iSortCol_0"]),
                                                    Request["sSortDir_0"],
                                                    param.iDisplayStart,
                                                    param.iDisplayStart + param.iDisplayLength,
                                                    totalRowsCount,
                                                    filteredRowsCount, Convert.ToString(cusrid), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]),
                                                    Convert.ToInt32(colagt), 0);
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
                        //SubsDate = d.SubSDate,
                        //SubeDate = d.SubEDate,
                        ClientContactNos = d.ClientContactNos
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
        //[Authorize(Roles = "ChitDividendPaymentCreate")]
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
            var empqry = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_Get_Employee_Dtl @usrid='" + cusrid + "'").ToList();
            ViewBag.CommisionAgentPersonID = new SelectList(empqry, "DValue", "DText").ToList();
            ViewBag.CommisionAgentPersonID2 = new SelectList(empqry, "DValue", "DText").ToList();
            ViewBag.CollectionAgentPersonID1 = new SelectList(empqry, "DValue", "DText").ToList();
            ViewBag.CollectionAgentPersonID2 = new SelectList(empqry, "DValue", "DText").ToList();
            ViewBag.ReviewByEmployeeId = new SelectList(empqry, "DValue", "DText").ToList();
            ViewBag.ApprovedByEmployeeId = new SelectList(empqry, "DValue", "DText").ToList();
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Book Verification', 'Status'").ToList();
            ViewBag.BookStatusTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();
            if (SubscriptionID > 0)
            {
                tab = context.chitsubscriptions.Find(SubscriptionID);
                ViewBag.QRSubsID = SubscriptionID;
                ViewBag.ProdType = "29";
                ViewBag.OwnerID = SubscriptionID;
                ViewBag.OwnerTypeID = "29";
                CTS.chitsubs = context.chitsubscriptions.Where(x => x.SubscriptionID == SubscriptionID).ToList();
                CTS.chitbooklists = context.Database.SqlQuery<pr_Get_Book_List_Result>("Exec pr_Get_Book_List @prodtype =29, @subsid =" + SubscriptionID).ToList();
                string dchkqry = "Exec pr_Get_Chit_Dividend_Schedule_n_Actuals_Details @ChitSubsID=" + SubscriptionID + ", @usrid='" + cusrid + "'";
                CTS.chitdividendschedules = context.Database.SqlQuery<pr_Get_Chit_Dividend_Schedule_n_Actuals_Details_Result>(dchkqry).ToList();
                ViewBag.FPersonID = new SelectList(personqry.Where(x => x.CLIENTID == tab.PersonID), "CLIENTID", "CNAME", tab.PersonID).ToList();

                amtcdqry = context.Database.SqlQuery<ChitScheme>("Select * From ChitScheme (nolock)").ToList();
                ViewBag.ChitSchemeID = new SelectList(amtcdqry.Where(x => x.ChitSchemeID == Convert.ToInt32(tab.ChitSchemeID)), "ChitSchemeID", "ChitSchemeName", tab.ChitSchemeID);

                amtcdqry1 = context.Database.SqlQuery<ChitGroup>("Select * From ChitGroup (nolock)").ToList();
                ViewBag.ChitGroupID = new SelectList(amtcdqry1.Where(x => x.ChitGroupID == tab.ChitGroupID), "ChitGroupID", "ChitGroupName", tab.ChitGroupID);

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Collection Type', ''").ToList();
                ViewBag.CollectionTypeID = new SelectList(mtqry, "TypeID", "Type", tab.CollectionTypeID).ToList();

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Interval', ''").ToList();
                ViewBag.CollectionIntervalTypeID = new SelectList(mtqry, "TypeID", "Type", tab.CollectionIntervalTypeID).ToList();

                amtcdqry2 = context.Database.SqlQuery<CompanyFundAccount>("Select * From CompanyFundAccount (nolock)").ToList();

                ViewBag.FundAccountID = new SelectList(amtcdqry2, "FundAccountID", "AccountName", tab.FundAccountID);

                amtcdqry3 = context.Database.SqlQuery<CompanyOffice>("Select * From CompanyOffice (nolock)").ToList();
                ViewBag.LinkedOfficeID = new SelectList(amtcdqry3, "officeid", "OfficeName", tab.LinkedOfficeID);

                ViewBag.CommisionAgentPersonID = new SelectList(empqry, "DValue", "DText", tab.CommisionAgentPersonID).ToList();
                ViewBag.CommisionAgentPersonID2 = new SelectList(empqry, "DValue", "DText", tab.CommisionAgentPersonID2).ToList();
                ViewBag.CollectionAgentPersonID1 = new SelectList(empqry, "DValue", "DText", tab.CommisionAgentPersonID).ToList();
                ViewBag.CollectionAgentPersonID2 = new SelectList(empqry, "DValue", "DText", tab.CommisionAgentPersonID2).ToList();
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Collection Mode', ''").ToList();
                ViewBag.CollectionModeTypeID = new SelectList(mtqry, "TypeID", "Type", tab.CollectionModeTypeID).ToList();
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Interval', ''").ToList();
                ViewBag.AscertainmentFrequencyTypeId = new SelectList(mtqry, "TypeID", "Type", 775).ToList();

                amtcdqry4 = context.Database.SqlQuery<CompanyRoute>("select* from CompanyRoute (nolock)").ToList();
                ViewBag.RouteID = new SelectList(amtcdqry4, "RouteID", "RouteName", tab.RouteID);
                ViewBag.CoApplicants = new SelectList("");
                ViewBag.NomineeRelationshipTypeID = new SelectList("");
                amtcdqry5 = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_Get_Book_PickupDay 0").ToList();
                ViewBag.BookPickupDay = new SelectList(amtcdqry5, "DValue", "DText", tab.BookPickupDay).ToList();

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Chit Subscription', 'Status'").ToList();
                ViewBag.StatusTypeID = new SelectList(mtqry.Where(x => x.TypeID >= tab.StatusTypeID), "TypeID", "Type", tab.StatusTypeID).ToList();


                string imgurl = "~/CSubsQRCode/" + SubscriptionID.ToString() + ".png";
                string imgpath = Server.MapPath("~/CSubsQRCode/" + id.ToString() + ".png");
                //bool chkurl = RemoteFileExists(imgurl);
                bool chkurl = System.IO.File.Exists(imgpath);


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


        #region Edit
        //[Authorize(Roles = "ChitDividendPaymentEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/ChitDividendPayment/Form/" + id);

                //Response.Redirect("/ChitDividendPayment/Form/?id=" + id);
            }
        }
        #endregion

    }
}
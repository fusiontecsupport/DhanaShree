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
    public class ChitSchemeManagementController : Controller
    {
        // GET: ChitSchemeManagement

        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        //[Authorize(Roles = "ChitSchemeManagementIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

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

                    var data = e.pr_Search_Master_ChitScheme(param.sSearch,
                                                    Convert.ToInt32(Request["iSortCol_0"]),
                                                    Request["sSortDir_0"],
                                                    param.iDisplayStart,
                                                    param.iDisplayStart + param.iDisplayLength,
                                                    totalRowsCount,
                                                    filteredRowsCount, Convert.ToString(cusrid));
                    var aaData = data.Select(d => new
                    {
                        ChitTypeDesc = d.ChitTypeDesc,
                        ChitSchemeCode = d.ChitSchemeCode,
                        ChitSchemeName = d.ChitSchemeName,
                        SubscriptionAmount = d.SubscriptionAmount.ToString(),
                        ChitValue = d.ChitValue.ToString(),
                        DurationTypeDesc =  d.ChitDuration.ToString()+" "+ d.DurationTypeDesc,
                        StatusTypeDesc = d.StatusTypeDesc,                        
                        ChitSchemeID = d.ChitSchemeID.ToString()
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

        #region Edit
        //[Authorize(Roles = "ChitSchemeManagementEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/ChitSchemeManagement/Form/" + id);

                //Response.Redirect("/ChitSchemeManagement/Form/?id=" + id);
            }
        }
        #endregion

        #region Form
        //[Authorize(Roles = "ChitSchemeManagementCreate")]
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            ChitScheme tab = new ChitScheme();            
            ChitSchemePattern tab1 = new ChitSchemePattern();
            ChitSchemeCollection tab2 = new ChitSchemeCollection();

            ChitSchemeList cml = new ChitSchemeList();

            int ChitSchemeID = 0;

            if (id == "" || id == null || id == "0")
            { ChitSchemeID = 0; }
            else { ChitSchemeID = Convert.ToInt32(id); }

            
            
            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'ChitType', ''").ToList();
            ViewBag.ChitTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();
            var amtcdqry = context.Database.SqlQuery<MasterAmountCode>("Select * From MasterAmountCode (nolock)").ToList();
            
            ViewBag.AmountCodeID = new SelectList(amtcdqry, "AmountCodeID", "AmountCodeName");
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Duration', ''").ToList();
            ViewBag.DurationTypeId = new SelectList(mtqry, "TypeID", "Type").ToList();
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Interval', ''").ToList();
            ViewBag.BookVerificationTypeId = new SelectList(mtqry, "TypeID", "Type").ToList();
            ViewBag.AscertainmentFrequencyTypeId = new SelectList(mtqry, "TypeID", "Type",775).ToList();
            //var mcqry = context.Database.SqlQuery<pr_Get_Master_Category_Result>("exec pr_Get_Master_Category 'Ascertainment'").ToList();
            //ViewBag.AscertainmentTypeId = new SelectList(mcqry, "CategoryID", "Category");
            
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Ascertainment', ''").ToList();
            ViewBag.AscertainmentTypeId = new SelectList(mtqry, "TypeID", "Type").ToList();
            ViewBag.CollectionIntervalTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();

            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Chit Scheme', 'Status'").ToList();
            ViewBag.StatusTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();
            


            if (ChitSchemeID > 0)
            {
                tab = context.chitscheme.Find(ChitSchemeID);
                ViewBag.ChitSchemeID=tab.ChitSchemeID.ToString();
                ViewBag.ChitSchemeName=tab.ChitSchemeName;

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'ChitType', ''").ToList();
                ViewBag.ChitTypeID = new SelectList(mtqry, "TypeID", "Type", tab.ChitTypeID).ToList();
                amtcdqry = context.Database.SqlQuery<MasterAmountCode>("Select * From MasterAmountCode (nolock)").ToList();

                ViewBag.AmountCodeID = new SelectList(amtcdqry, "AmountCodeID", "AmountCodeName", tab.AmountCodeID);
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Duration', ''").ToList();
                ViewBag.DurationTypeId = new SelectList(mtqry, "TypeID", "Type", tab.DurationTypeId).ToList();
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Interval', ''").ToList();
                ViewBag.BookVerificationTypeId = new SelectList(mtqry, "TypeID", "Type", tab.BookVerificationTypeId).ToList();
                ViewBag.AscertainmentFrequencyTypeId = new SelectList(mtqry, "TypeID", "Type", tab.AscertainmentFrequencyTypeId).ToList();
                ViewBag.CollectionIntervalTypeID = new SelectList(mtqry, "TypeID", "Type", tab.AscertainmentFrequencyTypeId).ToList();
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Ascertainment', ''").ToList();
                ViewBag.AscertainmentTypeId = new SelectList(mtqry, "TypeID", "Type", tab.AscertainmentTypeId).ToList();
                
                
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Chit Scheme', 'Status'").ToList();
                ViewBag.StatusTypeID = new SelectList(mtqry, "TypeID", "Type", tab.StatusTypeID).ToList();
                
                cml.chitscheme = context.chitscheme.Where(x => x.ChitSchemeID == ChitSchemeID).ToList();
                cml.chitschemecollections = context.chitschemecolletion.Where(x => x.ChitSchemeID == ChitSchemeID).ToList();
                cml.chitschemepatterns = context.chitschemepattern.Where(x => x.ChitSchemeID == ChitSchemeID).ToList();
            }
            else
            {
                
                cml.chitscheme = context.chitscheme.Where(x => x.ChitSchemeID == 0).ToList();
                cml.chitschemecollections = context.chitschemecolletion.Where(x => x.ChitSchemeID == 0).ToList();
                cml.chitschemepatterns = context.chitschemepattern.Where(x => x.ChitSchemeID == 0).ToList();
            }


            return View(cml);
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
                            
                            ChitScheme master = new ChitScheme();

                            Int32 ChitSchemeID = 0;

                            string chkGenSchemePatternFlg = "";

                            string chitschemeid = Convert.ToString(formcoll["ChitSchemeID"]);
                            string chkRegenFlg = Convert.ToString(formcoll["boolReGen"]);
                            if (chitschemeid == "" || chitschemeid == null )
                            {
                                ChitSchemeID = 0;
                                chkGenSchemePatternFlg = "Y";
                            }
                            else
                            {
                                ChitSchemeID = Convert.ToInt32(chitschemeid);
                                chkGenSchemePatternFlg = "N";
                            }
                            if (chkRegenFlg == "1")
                            {
                                chkGenSchemePatternFlg = "Y";
                            }
                            if (ChitSchemeID != 0)
                            {
                                master = context.chitscheme.Find(ChitSchemeID);
                            }
                            master.ChitSchemeID = ChitSchemeID;
                            master.ChitTypeID = Convert.ToInt32(formcoll["ChitTypeID"]);
                            master.AmountCodeID = 1;// Convert.ToInt32(formcoll["AmountCodeID"]);
                            master.ChitSchemeCode = Convert.ToString(formcoll["ChitSchemeCode"]);
                            master.ChitSchemeName = Convert.ToString(formcoll["ChitSchemeName"]);
                            master.ChitValue = Convert.ToDouble(formcoll["ChitValue"]);
                            master.ChitDuration = Convert.ToInt32(formcoll["ChitDuration"]);
                            master.DurationTypeId = Convert.ToInt32(formcoll["DurationTypeId"]);

                            double InitialDiscount = 0;
                            if (formcoll["InitialDiscount"] != "")
                                InitialDiscount = Convert.ToDouble(formcoll["InitialDiscount"]);
                            double InitialDividend = 0;
                            if (formcoll["InitialDividend"] != "")
                                InitialDividend = Convert.ToDouble(formcoll["InitialDividend"]);
                            master.InitialDiscountPerc = InitialDiscount;
                            master.InitialDividendPerc = InitialDividend;
                            if (formcoll["CommissionEmp"] == "" || formcoll["CommissionEmp"] == null)
                                master.CommissionEmp = 0;
                            else
                                master.CommissionEmp = Convert.ToDouble(formcoll["CommissionEmp"]);
                            
                            if (formcoll["ForemanCommision"] == "" || formcoll["ForemanCommision"] == null)
                                master.ForemanCommision = 0;
                            else
                                master.ForemanCommision = Convert.ToDouble(formcoll["ForemanCommision"]);

                            if (formcoll["ForemanPrizeInstalment"] == "" || formcoll["ForemanPrizeInstalment"] == null)
                                master.ForemanPrizeInstalment = 0;
                            else
                                master.ForemanPrizeInstalment = Convert.ToInt32(formcoll["ForemanPrizeInstalment"]);

                            if (formcoll["BidCapAmount"] == "" || formcoll["BidCapAmount"] == null)
                                master.BidCapAmount = 0;
                            else
                                master.BidCapAmount = Convert.ToDouble(formcoll["BidCapAmount"]);

                            if (formcoll["SealingPeriod"] == "" || formcoll["SealingPeriod"] == null)
                                master.SealingPeriod = 0;
                            else
                                master.SealingPeriod = Convert.ToInt32(formcoll["SealingPeriod"]);


                            if (formcoll["BookVerificationTypeId"] == "" || formcoll["BookVerificationTypeId"] == null)
                                master.BookVerificationTypeId = 0;
                            else
                                master.BookVerificationTypeId = Convert.ToInt32(formcoll["BookVerificationTypeId"]);
                            //if (formcoll["AscertainmentTypeId"] == "" || formcoll["AscertainmentTypeId"] == null)
                            //    master.AscertainmentTypeId = 0;
                            //else
                            //    master.AscertainmentTypeId = Convert.ToInt32(formcoll["AscertainmentTypeId"]);

                            //if (formcoll["AscertainmentFrequencyTypeId"] == "" || formcoll["AscertainmentFrequencyTypeId"] == null)
                            //    master.AscertainmentFrequencyTypeId = 0;
                            //else
                            //    master.AscertainmentFrequencyTypeId = Convert.ToInt32(formcoll["AscertainmentFrequencyTypeId"]);
                            master.AscertainmentTypeId = 801;
                            master.AscertainmentFrequencyTypeId = 775;
                            if (formcoll["CommissionNonEmp"] == "" || formcoll["CommissionNonEmp"] == null)
                                master.CommissionNonEmp = 0;
                            else
                                master.CommissionNonEmp = Convert.ToDouble(formcoll["CommissionNonEmp"]);
                            
                            double AverageDiscount = 0;
                            if (formcoll["AverageDiscount"] == "" || formcoll["AverageDiscount"] == null)
                                AverageDiscount = 0;
                            else
                                AverageDiscount = Convert.ToDouble(formcoll["AverageDiscount"]);
                            master.AverageDiscount = AverageDiscount;

                            double AverageDivident = 0;
                            if (formcoll["AverageDivident"] == "" || formcoll["AverageDivident"] == null)
                                AverageDivident = 0;
                            else
                                AverageDivident = Convert.ToDouble(formcoll["AverageDivident"]);

                            
                            master.AverageDivident = AverageDivident;
                            if (formcoll["SubscriptionAmount"] == "" || formcoll["SubscriptionAmount"] == null)
                                master.SubscriptionAmount = 0;
                            else
                                master.SubscriptionAmount = Convert.ToDouble(formcoll["SubscriptionAmount"]);

                            if (formcoll["EligibilityAmount"] == "" || formcoll["EligibilityAmount"] == null)
                                master.EligibilityAmount = 0;
                            else
                                master.EligibilityAmount = Convert.ToDouble(formcoll["EligibilityAmount"]);
                            
                            if (formcoll["SubscriberReturns"] == "" || formcoll["SubscriberReturns"] == null)
                                master.SubscriberReturns = 0;
                            else
                                master.SubscriberReturns = Convert.ToDouble(formcoll["SubscriberReturns"]);

                            
                            master.StatusTypeID = Convert.ToInt16(formcoll["StatusTypeID"]);


                            if (ChitSchemeID == 0)
                            {
                                string squery = "SELECT *FROM ChitScheme (nolock) WHERE ChitSchemeCode ='" + master.ChitSchemeCode+ "' or ChitSchemeName='" + master.ChitSchemeName+ "'";
                                var query = context.Database.SqlQuery<ChitScheme>(squery).ToList();

                                if (query.Count != 0)
                                {
                                    status = "Existing";
                                }
                                else
                                {
                                    //master.CreatedBy = Session["CUSRID"].ToString();
                                    //master.CreatedDt = DateTime.Now;

                                    context.chitscheme.Add(master);
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
                            ChitSchemeID = master.ChitSchemeID;

                            #region Generate Scheme & Collection Pattern <Start>
                            if (chkGenSchemePatternFlg == "Y")
                            {
                                double chitValue = Convert.ToDouble(master.ChitValue);
                                int sealingPeriod = Convert.ToInt32(master.SealingPeriod);
                                int duration = Convert.ToInt32(master.ChitDuration);
                                double initialDividend = 0.0;
                                double initialDiscount = 0.0;
                                double DueAmt = 0.0;
                                int prizeInstall = -1;
                                int numberOfInstallments = 0;
                                try
                                {


                                    var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'ChitType', ''").ToList();
                                    var slistChitType = new SelectList(mtqry.Where(x => x.TypeID == master.ChitTypeID), "TypeID", "Type", master.ChitTypeID).ToList();
                                    var amtcdqry = context.Database.SqlQuery<MasterAmountCode>("Select * From MasterAmountCode (nolock)").ToList();

                                    var slistAmtCd = new SelectList(amtcdqry.Where(x => x.AmountCodeID == master.AmountCodeID), "AmountCodeID", "AmountCodeName", master.AmountCodeID);
                                    mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Duration', ''").ToList();
                                    var slistDurationType = new SelectList(mtqry.Where(x => x.TypeID == master.DurationTypeId), "TypeID", "Type", master.DurationTypeId).ToList();
                                    mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Interval', ''").ToList();
                                    var slistBookVerificationType = new SelectList(mtqry.Where(x => x.TypeID == master.BookVerificationTypeId), "TypeID", "Type", master.BookVerificationTypeId).ToList();
                                    var slistAscertainmentFrequencyType = new SelectList(mtqry.Where(x => x.TypeID == master.AscertainmentFrequencyTypeId), "TypeID", "Type", master.AscertainmentFrequencyTypeId).ToList();
                                    var slistCollectionIntervalType = new SelectList(mtqry, "TypeID", "Type").ToList();
                                    mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Ascertainment', ''").ToList();
                                    var slistAscertainmentType = new SelectList(mtqry.Where(x => x.TypeID == master.AscertainmentTypeId), "TypeID", "Type", master.AscertainmentTypeId).ToList();


                                    mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Chit Scheme', 'Status'").ToList();
                                    var slistStatusType = new SelectList(mtqry.Where(x => x.TypeID == master.StatusTypeID), "TypeID", "Type", master.StatusTypeID).ToList();

                                    SelectListItem slsts = slistStatusType.FirstOrDefault();
                                    SelectListItem slct = slistChitType.FirstOrDefault();
                                    SelectListItem sldurn = slistDurationType.FirstOrDefault();
                                    SelectListItem slfreq = slistAscertainmentFrequencyType.FirstOrDefault();
                                    //SelectListItem slcolintval = slistCollectionIntervalType();

                                    string dqy = "";
                                    //Chit Scheme pattern should be enabled only for UNREG type
                                    if (slct.Text.ToUpper() == "UNREG" && InitialDividend > 0 && InitialDiscount > 0)
                                    {
                                        dqy = "Delete from ChitSchemePattern Where ChitSchemeID = " + master.ChitSchemeID + "";
                                        context.Database.ExecuteSqlCommand(dqy);


                                        numberOfInstallments = GetNoInstallaments(Convert.ToDouble(duration), sldurn.Text.ToUpper());
                                        if (master.SealingPeriod == 0 || master.SealingPeriod == null)
                                        {
                                            //sealingPeriod = Convert.ToInt32(master.SealingPeriod);
                                            sealingPeriod = Convert.ToInt32(numberOfInstallments);
                                        }
                                        initialDividend = Convert.ToDouble(InitialDividend);
                                        initialDiscount = Convert.ToDouble(InitialDiscount);
                                        if (master.ForemanPrizeInstalment > 0)
                                        {
                                            prizeInstall = Convert.ToInt32(master.ForemanPrizeInstalment);
                                        }
                                        double prvDiscPerc = 0.0;
                                        double prvDivdPerc = 0.0;
                                        for (int installment = 1; installment <= numberOfInstallments; installment++)
                                        {

                                            ChitSchemePattern chitSchemePatrn = new ChitSchemePattern();
                                            chitSchemePatrn.PatternID = -1;
                                            chitSchemePatrn.ChitSchemeID = Convert.ToInt32(master.ChitSchemeID);
                                            chitSchemePatrn.ChitInstalmentNo = installment;
                                            //if (installment <= sealingPeriod)
                                            {
                                                if (installment != prizeInstall)
                                                {

                                                    //chitSchemePatrn.DiscountPercentage = InitialDiscount;
                                                    //chitSchemePatrn.DividentPercentage = InitialDividend;
                                                    //chitSchemePatrn.DiscountAmount = Math.Round(((chitValue / 100) * InitialDiscount), 2);
                                                    //chitSchemePatrn.DividentAmount = Math.Round(((chitValue / 100) * InitialDividend), 2);
                                                    if (numberOfInstallments == 20)
                                                    {
                                                        if (installment <= (Convert.ToInt32(sealingPeriod)))
                                                        {
                                                            prvDiscPerc = InitialDiscount;
                                                            chitSchemePatrn.DiscountPercentage = prvDiscPerc;
                                                        }
                                                        else if (installment > (Convert.ToInt32(sealingPeriod)) && installment <= 15)
                                                        {
                                                            if ((prvDiscPerc - 1) > 0)
                                                                prvDiscPerc = prvDiscPerc - 1;
                                                            else
                                                                prvDiscPerc = 0;
                                                            chitSchemePatrn.DiscountPercentage = prvDiscPerc;
                                                        }
                                                        else if (installment > 15)
                                                        {
                                                            if ((prvDiscPerc - 2) > 0)
                                                                prvDiscPerc = prvDiscPerc - 2;
                                                            else
                                                                prvDiscPerc = 0;

                                                            chitSchemePatrn.DiscountPercentage = prvDiscPerc;
                                                        }
                                                        chitSchemePatrn.DiscountAmount = Math.Round(((chitValue / 100) * Convert.ToDouble(chitSchemePatrn.DiscountPercentage)), 2);

                                                        if (installment <= (Convert.ToInt32(sealingPeriod)))
                                                        {
                                                            prvDivdPerc = InitialDividend;
                                                            chitSchemePatrn.DividentPercentage = prvDivdPerc;
                                                        }
                                                        else if (installment > (Convert.ToInt32(sealingPeriod)) && installment <= 15)
                                                        {
                                                            if ((prvDivdPerc - 0.05) > 0)
                                                                prvDivdPerc = prvDivdPerc - 0.05;
                                                            else
                                                                prvDivdPerc = 0;
                                                            chitSchemePatrn.DividentPercentage = prvDivdPerc;
                                                        }
                                                        else if (installment > 15)
                                                        {
                                                            if ((prvDivdPerc - 0.10) > 0)
                                                                prvDivdPerc = prvDivdPerc - 0.10;
                                                            else
                                                                prvDivdPerc = 0;
                                                            chitSchemePatrn.DividentPercentage = prvDivdPerc;
                                                        }
                                                        chitSchemePatrn.DividentAmount = Math.Round(((chitValue / 100) * Convert.ToDouble(chitSchemePatrn.DividentPercentage)), 2);
                                                    }
                                                    else if (numberOfInstallments == 10)
                                                    {
                                                        if (installment == 2)
                                                        {
                                                            prvDiscPerc = InitialDiscount;
                                                            chitSchemePatrn.DiscountPercentage = prvDiscPerc;
                                                        }
                                                        else if (installment > 2 && installment < 4)
                                                        {
                                                            if ((prvDiscPerc - 2) > 0)
                                                                prvDiscPerc = prvDiscPerc - 2;
                                                            else
                                                                prvDiscPerc = 0;
                                                            chitSchemePatrn.DiscountPercentage = prvDiscPerc;
                                                        }
                                                        else if (installment >= 4 && installment < 8)
                                                        {
                                                            if ((prvDiscPerc - 3) > 0)
                                                                prvDiscPerc = prvDiscPerc - 3;
                                                            else
                                                                prvDiscPerc = 0;

                                                            chitSchemePatrn.DiscountPercentage = prvDiscPerc;
                                                        }
                                                        else if (installment >= 8)
                                                        {
                                                            if ((prvDiscPerc - 2) > 0)
                                                                prvDiscPerc = prvDiscPerc - 2;
                                                            else
                                                                prvDiscPerc = 0;

                                                            chitSchemePatrn.DiscountPercentage = prvDiscPerc;
                                                        }
                                                        chitSchemePatrn.DiscountAmount = Math.Round(((chitValue / 100) * Convert.ToDouble(chitSchemePatrn.DiscountPercentage)), 2);

                                                        if (installment == 2)
                                                        {
                                                            prvDivdPerc = InitialDividend;
                                                            chitSchemePatrn.DividentPercentage = prvDivdPerc;
                                                        }
                                                        else if (installment == 3)
                                                        {
                                                            if ((prvDivdPerc - 0.20) > 0)
                                                                prvDivdPerc = prvDivdPerc - 0.20;
                                                            else
                                                                prvDivdPerc = 0;
                                                            chitSchemePatrn.DividentPercentage = prvDivdPerc;
                                                        }
                                                        else if (installment >= 4 && installment < 8)
                                                        {
                                                            if ((InitialDividend - 0.30) > 0)
                                                                prvDivdPerc = prvDivdPerc - 0.30;
                                                            else
                                                                prvDivdPerc = 0;
                                                            chitSchemePatrn.DividentPercentage = prvDivdPerc;
                                                        }
                                                        else if (installment >= 8)
                                                        {
                                                            if ((prvDivdPerc - 0.2) > 0)
                                                                prvDivdPerc = prvDivdPerc - 0.2;
                                                            else
                                                                prvDivdPerc = 0;
                                                            chitSchemePatrn.DividentPercentage = prvDivdPerc;
                                                        }
                                                        chitSchemePatrn.DividentAmount = Math.Round(((chitValue / 100) * Convert.ToDouble(chitSchemePatrn.DividentPercentage)), 2);
                                                    }

                                                }
                                                else
                                                {
                                                    chitSchemePatrn.DiscountPercentage = 0;
                                                    chitSchemePatrn.DividentPercentage = 0;
                                                    chitSchemePatrn.DiscountAmount = 0;
                                                    chitSchemePatrn.DividentAmount = 0;
                                                }
                                                DueAmt = Math.Round(DueAmt + Convert.ToDouble(master.ChitValue) / Convert.ToDouble(master.ChitDuration) - Convert.ToDouble(chitSchemePatrn.DividentAmount), 0);
                                            }
                                            context.chitschemepattern.Add(chitSchemePatrn);
                                            context.SaveChanges();
                                            status = "Success";
                                        }
                                    }

                                    dqy = "Delete from ChitSchemeCollection Where ChitSchemeID = " + master.ChitSchemeID + "";
                                    context.Database.ExecuteSqlCommand(dqy);

                                    foreach (SelectListItem frequency in slistCollectionIntervalType)
                                    {
                                        if (frequency.Value != "-1")
                                        {
                                            ChitSchemeCollection chitSchemeColln = new ChitSchemeCollection();
                                            chitSchemeColln.CollectionIntervalTypeID = Convert.ToInt32(frequency.Value);
                                            chitSchemeColln.ChitSchemeID = Convert.ToInt32(master.ChitSchemeID);
                                            chitSchemeColln.CollectionAmount = GetSuggestedAmount(chitValue, sldurn.Text.ToUpper(), duration, frequency.Text, Convert.ToDouble(DueAmt));
                                            chitSchemeColln.SuggestedAmount = GetSuggestedAmount(chitValue, sldurn.Text.ToUpper(), duration, frequency.Text, 0);
                                            context.chitschemecolletion.Add(chitSchemeColln);
                                            context.SaveChanges();
                                        }
                                    }
                                    CalculateDerivedValues(master.ChitSchemeID);
                                    return Json(status, JsonRequestBehavior.AllowGet);
                                }
                                catch (Exception ex)
                                {
                                    status = "error";
                                    return Json(status, JsonRequestBehavior.AllowGet);
                                }
                            }
                               
                            #endregion                            

                            ViewBag.Message = "Form submitted.";
                            // Response.Redirect("Index");
                            //return RedirectToAction("Index", "ChitSchemeManagement");
                            //Response.Write("Update");
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            var ermsg = ex.Message;
                            //Response.Write(ex.Message.ToString());
                            trans1.Rollback();
                            //Response.Redirect("/Error/AccessDenied");
                            //return RedirectToAction("Index", "ChitSchemeManagement");
                            status = "Error";
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

        }
        #endregion

        #region Save Scheme Pattern Data
        [HttpPost]
        public ActionResult SaveSPData(FormCollection formcoll)
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
                            
                            ChitScheme master = new ChitScheme();

                            Int32 ChitSchemeID = 0;

                            string chitschemeid = Convert.ToString(formcoll["ChitSchemeID"]);
                            if (chitschemeid == "" || chitschemeid == null)
                            {
                                ChitSchemeID = 0;
                            }
                            else
                            {
                                ChitSchemeID = Convert.ToInt32(chitschemeid);
                            }

                            if (ChitSchemeID != 0)
                            {
                                master = context.chitscheme.Find(ChitSchemeID);
                            }
                            
                            string[] PatternID = formcoll.GetValues("PatternID");
                            string[] ChitInstalmentNo = formcoll.GetValues("ChitInstalmentNo");
                            string[] DiscountPercentage = formcoll.GetValues("DiscountPercentage");
                            string[] DiscountAmount = formcoll.GetValues("DiscountAmount");
                            string[] DividentPercentage = formcoll.GetValues("DividentPercentage");
                            string[] DividentAmount = formcoll.GetValues("DividentAmount");
                            int reccnt = 0;
                            reccnt = ChitInstalmentNo.Length;
                            //if((reccnt==20 || reccnt == 10) && master.ChitSchemeID>0)
                            //{
                            //    for (int installment = 0; installment < reccnt; installment++)
                            //    {
                            //        ChitSchemePattern chitSchemePatrn = new ChitSchemePattern();

                            //        if (Convert.ToInt32(PatternID[installment]) > 0)
                            //        {
                            //            chitSchemePatrn = context.chitschemepattern.Find(Convert.ToInt32(PatternID[installment]));
                            //        }
                            //        chitSchemePatrn.PatternID = Convert.ToInt32(PatternID[installment]);
                            //        chitSchemePatrn.ChitSchemeID = Convert.ToInt32(master.ChitSchemeID);
                            //        chitSchemePatrn.ChitInstalmentNo = Convert.ToInt32(ChitInstalmentNo[installment]);
                            //        if ((DiscountPercentage[installment] == "" || DiscountPercentage[installment] == null)&& installment == 0)
                            //            chitSchemePatrn.DiscountPercentage = 0;
                            //        else
                            //        {
                            //            chitSchemePatrn.DiscountPercentage =Convert.ToDouble(DiscountPercentage[installment]);
                            //            //if(reccnt==20)
                            //            //{
                            //            //    if (installment < (Convert.ToInt32(master.SealingPeriod) - 1))
                            //            //    {
                            //            //        chitSchemePatrn.DiscountPercentage = Convert.ToDouble(DiscountPercentage[installment]);
                            //            //    }
                            //            //    else if (installment >= (Convert.ToInt32(master.SealingPeriod) - 1) && installment <= 15)
                            //            //    {
                            //            //        chitSchemePatrn.DiscountPercentage = Convert.ToDouble(DiscountPercentage[installment - 1]) - 1;
                            //            //    }
                            //            //    else if (installment > 15)
                            //            //    {
                            //            //        chitSchemePatrn.DiscountPercentage = Convert.ToDouble(DiscountPercentage[installment - 1]) - 2;
                            //            //    }
                            //            //}
                            //            //else if (reccnt == 10)
                            //            //    {
                            //            //    if (installment == 0)
                            //            //    {
                            //            //        chitSchemePatrn.DiscountPercentage = Convert.ToDouble(DiscountPercentage[installment]);
                            //            //    }
                            //            //    else if (installment > 0 && installment <= 4)
                            //            //    {
                            //            //        chitSchemePatrn.DiscountPercentage = Convert.ToDouble(DiscountPercentage[installment - 1]) - 2;
                            //            //    }
                            //            //    else if (installment > 4 && installment <8)
                            //            //    {
                            //            //        chitSchemePatrn.DiscountPercentage = Convert.ToDouble(DiscountPercentage[installment - 1]) - 3;
                            //            //    }
                            //            //    else if (installment >=8 )
                            //            //    {
                            //            //        chitSchemePatrn.DiscountPercentage = Convert.ToDouble(DiscountPercentage[installment - 1]) - 2;
                            //            //    }
                            //            //}

                            //        }

                            //        chitSchemePatrn.DiscountAmount = Math.Round(Convert.ToDouble(master.ChitValue) * Convert.ToDouble(chitSchemePatrn.DiscountPercentage) / 100.0, 2);
                            //        if ((DividentPercentage[installment] == "" || DividentPercentage[installment] == null) && installment == 0)
                            //            chitSchemePatrn.DividentPercentage= 0;
                            //        else
                            //        {
                            //            chitSchemePatrn.DividentPercentage = Convert.ToDouble(DividentPercentage[installment]);
                            //            //if (reccnt == 20)
                            //            //{
                            //            //    if (installment < (Convert.ToInt32(master.SealingPeriod) - 1))
                            //            //    {
                            //            //        chitSchemePatrn.DividentPercentage = Convert.ToDouble(DividentPercentage[installment]);
                            //            //    }
                            //            //    else if (installment >= (Convert.ToInt32(master.SealingPeriod) - 1) && installment <= 15)
                            //            //    {
                            //            //        chitSchemePatrn.DividentPercentage = Convert.ToDouble(DividentPercentage[installment - 1]) - 0.05;
                            //            //    }
                            //            //    else if (installment > 15)
                            //            //    {
                            //            //        chitSchemePatrn.DividentPercentage = Convert.ToDouble(DividentPercentage[installment - 1]) - .10;
                            //            //    }
                            //            //}
                            //            //else if (reccnt == 10)
                            //            //{
                            //            //    if (installment == 0 )
                            //            //    {
                            //            //        chitSchemePatrn.DividentPercentage = Convert.ToDouble(DividentPercentage[installment]);
                            //            //    }
                            //            //    else if (installment >= 0 && installment <= 4)
                            //            //    {
                            //            //        chitSchemePatrn.DividentPercentage = Convert.ToDouble(DividentPercentage[installment - 1]) ;
                            //            //    }
                            //            //    else if (installment > 4 && installment < 8)
                            //            //    {
                            //            //        chitSchemePatrn.DividentPercentage = Convert.ToDouble(DividentPercentage[1]) - 1;
                            //            //    }
                            //            //    else if (installment >= 8)
                            //            //    {
                            //            //        chitSchemePatrn.DividentPercentage = 0;// Convert.ToDouble(DividentPercentage[installment - 1]) - 1;
                            //            //    }
                            //            //}

                            //        }

                            //        chitSchemePatrn.DividentAmount = Math.Round(Convert.ToDouble(master.ChitValue) * Convert.ToDouble(chitSchemePatrn.DividentPercentage) / 100.0, 2);
                                    
                            //        if (Convert.ToInt32(PatternID[installment]) == 0)
                            //        {
                            //            context.chitschemepattern.Add(chitSchemePatrn);
                            //        }
                            //        else
                            //        {
                            //            context.Entry(chitSchemePatrn).State = System.Data.Entity.EntityState.Modified;
                            //        }
                            //        context.SaveChanges();
                            //        status = "Success";
                            //    }
                            //    CalculateDerivedValues(master.ChitSchemeID);
                            //    trans1.Commit();
                            //}
                            //else
                            if (reccnt>0 && master.ChitSchemeID>0)
                            {
                                for (int installment = 0; installment < reccnt; installment++)
                                { 
                                    ChitSchemePattern chitSchemePatrn = new ChitSchemePattern();

                                    if (Convert.ToInt32(PatternID[installment]) > 0)
                                    {
                                        chitSchemePatrn = context.chitschemepattern.Find(Convert.ToInt32(PatternID[installment]));
                                    }
                                    chitSchemePatrn.PatternID = Convert.ToInt32(PatternID[installment]);
                                    chitSchemePatrn.ChitSchemeID = Convert.ToInt32(master.ChitSchemeID);
                                    chitSchemePatrn.ChitInstalmentNo = Convert.ToInt32(ChitInstalmentNo[installment]);
                                    if (DiscountPercentage[installment] == "" || DiscountPercentage[installment] == null)
                                        chitSchemePatrn.DiscountPercentage = 0;
                                    else
                                        chitSchemePatrn.DiscountPercentage = Convert.ToDouble(DiscountPercentage[installment]);

                                    if (DiscountAmount[installment] == "" || DiscountAmount[installment] == null)
                                        chitSchemePatrn.DiscountAmount = 0;
                                    else
                                        chitSchemePatrn.DiscountAmount = Convert.ToDouble(DiscountAmount[installment]);
                                    if (DividentPercentage[installment] == "" || DividentPercentage[installment] == null)
                                        chitSchemePatrn.DividentPercentage = 0;
                                    else
                                        chitSchemePatrn.DividentPercentage = Convert.ToDouble(DividentPercentage[installment]);
                                    if (DividentAmount[installment] == "" || DividentAmount[installment] == null)
                                        chitSchemePatrn.DividentAmount = 0;
                                    else
                                        chitSchemePatrn.DividentAmount = Convert.ToDouble(DividentAmount[installment]);
                                    if (Convert.ToInt32(PatternID[installment]) == 0)
                                    {
                                        context.chitschemepattern.Add(chitSchemePatrn);
                                    }
                                    else
                                    {  context.Entry(chitSchemePatrn).State = System.Data.Entity.EntityState.Modified;
                                    }
                                    context.SaveChanges();
                                    status = "Success";
                                }
                                CalculateDerivedValues(master.ChitSchemeID);
                                trans1.Commit();
                            }
                            else
                            {
                                status="error";
                            }
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }  
                        catch (Exception ex)
                        {
                            var ermsg = ex.Message;
                            //Response.Write(ex.Message.ToString());
                            trans1.Rollback();
                            //Response.Redirect("/Error/AccessDenied");
                            //return RedirectToAction("Index", "ChitSchemeManagement");
                            status = "Error";
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

        }
        #endregion

        #region Save Collection Pattern Data
        [HttpPost]
        public ActionResult SaveCPData(FormCollection formcoll)
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
                            
                            ChitScheme master = new ChitScheme();

                            Int32 ChitSchemeID = 0;

                            string chitschemeid = Convert.ToString(formcoll["ChitSchemeID"]);
                            if (chitschemeid == "" || chitschemeid == null)
                            {
                                ChitSchemeID = 0;
                            }
                            else
                            {
                                ChitSchemeID = Convert.ToInt32(chitschemeid);
                            }

                            if (ChitSchemeID != 0)
                            {
                                master = context.chitscheme.Find(ChitSchemeID);
                            }

                            string[] ChitSchemeCollectionID = formcoll.GetValues("ChitSchemeCollectionID");
                            string[] CollectionIntervalTypeID = formcoll.GetValues("chitschemecollections[0].CollectionIntervalTypeID");                            
                            string[] SuggestedAmount = formcoll.GetValues("SuggestedAmount");                            
                            string[] CollectionAmount = formcoll.GetValues("CollectionAmount");
                            int reccnt = 0;
                            reccnt = ChitSchemeCollectionID.Length;
                            if (reccnt>0 && master.ChitSchemeID>0)
                            {
                                for (int icnt = 0; icnt < reccnt; icnt++)
                                { 
                                    ChitSchemeCollection chitSchemeColln = new ChitSchemeCollection();
                                    if(Convert.ToInt32(ChitSchemeCollectionID[icnt])>0)
                                        chitSchemeColln=context.chitschemecolletion.Find(Convert.ToInt32(ChitSchemeCollectionID[icnt]));
                                    chitSchemeColln.CollectionIntervalTypeID = Convert.ToInt32(CollectionIntervalTypeID[icnt]);
                                    chitSchemeColln.ChitSchemeID = Convert.ToInt32(master.ChitSchemeID);
                                    chitSchemeColln.CollectionAmount = Convert.ToDouble(CollectionAmount[icnt]);
                                    chitSchemeColln.SuggestedAmount = Convert.ToDouble(SuggestedAmount[icnt]);
                                    
                                    if (Convert.ToInt32(ChitSchemeCollectionID[icnt]) == 0)
                                    {
                                        context.chitschemecolletion.Add(chitSchemeColln);
                                    }
                                    else
                                    {  
                                        context.Entry(chitSchemeColln).State = System.Data.Entity.EntityState.Modified;
                                    }

                                    context.SaveChanges();
                                    status = "Success";
                                }
                                CalculateDerivedValues(master.ChitSchemeID);
                                trans1.Commit();
                                return Json(status, JsonRequestBehavior.AllowGet);
                            }


                            
                            ViewBag.Message = "Form submitted.";
                            // Response.Redirect("Index");
                            //return RedirectToAction("Index", "ChitSchemeManagement");
                            //Response.Write("Update");
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            var ermsg = ex.Message;
                            trans1.Rollback();
                            status = "Error";
                            return Json(status, JsonRequestBehavior.AllowGet);
                        } 
                        
                    }
                }
            }

        }
        #endregion

        #region Delete Record  
        //[Authorize(Roles = "ChitSchemeManagementDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check(fld, id, "ChitScheme");            
            if (temp.Equals("PROCEED"))
            {
                string dqy = "Delete from ChitSchemePattern Where ChitSchemeID = " + Convert.ToInt32(id) + "";
                context.Database.ExecuteSqlCommand(dqy);
                dqy = "Delete from ChitSchemeCollection Where ChitSchemeID = " + Convert.ToInt32(id) + "";
                context.Database.ExecuteSqlCommand(dqy);
                ChitScheme chitscheme = context.chitscheme.Find(Convert.ToInt32(id));
                context.chitscheme.Remove(chitscheme);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion

        public int GetNoInstallaments(double duration, string durationType)
        {
            double numberOfInstallments;
            try
            {
                durationType = durationType.Trim().ToUpper();
                if (durationType == "DAYS")
                {
                    numberOfInstallments = Math.Ceiling(Convert.ToDouble(duration / 30));
                }
                else if (durationType == "WEEKS")
                {
                    numberOfInstallments = Math.Ceiling(Convert.ToDouble(((duration * 7) / 30)));
                }
                else if (durationType == "YEARS")
                {
                    numberOfInstallments = Convert.ToDouble(duration * 12);
                }
                else
                {
                    numberOfInstallments = duration;
                }
                return Convert.ToInt32(numberOfInstallments);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double GetSuggestedAmount(double SchemeValue, string DurationType,double duration, string frequencyType, double DueAmt)
        {
            double suggestedAmount = 0;
            double chitValue;
            string durationType;
            try
            {
                if(DueAmt>0)
                    chitValue = Convert.ToDouble(DueAmt);
                else
                    chitValue = Convert.ToDouble(SchemeValue);
                durationType = DurationType.Trim().ToUpper();

                frequencyType = frequencyType.Trim().ToUpper();

                if (frequencyType == "DAILY")
                {
                    if (durationType == "DAYS")
                    {
                        suggestedAmount = Math.Round(chitValue / duration, 2);
                    }
                    else if (durationType == "WEEKS")
                    {
                        suggestedAmount = Math.Round(chitValue / (duration * 7), 2);
                    }
                    else if (durationType == "YEARS")
                    {
                        suggestedAmount = Math.Round(chitValue / (duration * 365), 2);
                    }
                    else
                    {
                        suggestedAmount = Math.Round(chitValue / (duration * 30), 2);
                    }
                }
                else if (frequencyType == "WEEKLY")
                {
                    if (durationType == "DAYS")
                    {
                        suggestedAmount = Math.Round(chitValue / (duration / 7), 2);
                    }
                    else if (durationType == "WEEKS")
                    {
                        suggestedAmount = Math.Round(chitValue / duration, 2);
                    }
                    else if (durationType == "YEARS")
                    {
                        suggestedAmount = Math.Round(chitValue / (duration * 52), 2);
                    }
                    else
                    {
                        suggestedAmount = Math.Round(chitValue / ((duration * 30) / 7), 2);
                    }
                }
                else if (frequencyType == "MONTHLY")
                {
                    if (durationType == "DAYS")
                    {
                        suggestedAmount = Math.Round(chitValue / (duration / 30), 2);
                    }
                    else if (durationType == "WEEKS")
                    {
                        suggestedAmount = Math.Round(chitValue / ((duration * 7) / 30), 2);
                    }
                    else if (durationType == "YEARS")
                    {
                        suggestedAmount = Math.Round(chitValue / (duration * 12), 2);
                    }
                    else
                    {
                        suggestedAmount = Math.Round(chitValue / duration, 2);
                    }
                }
                return suggestedAmount;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CalculateDerivedValues(int chitSchemeId)
        {
            ChitScheme csmaster = new ChitScheme(); 
            csmaster = context.chitscheme.Find(chitSchemeId);
            ChitSchemeList cml = new ChitSchemeList();
            cml.chitscheme = context.chitscheme.Where(x => x.ChitSchemeID == chitSchemeId).ToList();
            cml.chitschemecollections = context.chitschemecolletion.Where(x => x.ChitSchemeID == chitSchemeId && x.CollectionIntervalTypeID == csmaster.AscertainmentFrequencyTypeId).ToList();
            cml.chitschemepatterns = context.chitschemepattern.Where(x => x.ChitSchemeID == chitSchemeId).ToList();
            if (cml.chitschemepatterns.Count > 0)
            {

                double enteredDiscountPercent = context.chitschemepattern.Where(x => x.ChitSchemeID == chitSchemeId).Select(x => x.DiscountPercentage ?? 0).Sum();
                double enteredDividentPercent = context.chitschemepattern.Where(x => x.ChitSchemeID == chitSchemeId).Select(x => x.DividentPercentage ?? 0).Sum();

                double enteredDiscountAmount = context.chitschemepattern.Where(x => x.ChitSchemeID == chitSchemeId).Select(x => x.DiscountAmount ?? 0).Sum();

                double enteredDividentAmount = context.chitschemepattern.Where(x => x.ChitSchemeID == chitSchemeId).Select(x => x.DividentAmount ?? 0).Sum(); 
                
                double enteredDiscountInstallment = context.chitschemepattern.Where(x => x.ChitSchemeID == chitSchemeId).Count();
                double enteredDividentInstallment = context.chitschemepattern.Where(x => x.ChitSchemeID == chitSchemeId).Count();
                
                
                if (enteredDiscountInstallment > 0)
                {
                    double averageDiscountPercent = Math.Round(enteredDiscountPercent / enteredDiscountInstallment, 2);
                    double averageDiscountAmount = Math.Round(enteredDiscountAmount / enteredDiscountInstallment, 2);
                    ViewBag.AverageDiscount = averageDiscountPercent.ToString();
                    ViewBag.AverageDiscountAmount = averageDiscountAmount.ToString();
                    
                    csmaster.AverageDiscount = Convert.ToDouble(averageDiscountPercent);                    

                }
                else
                {
                    ViewBag.AverageDiscount = string.Empty;
                    ViewBag.AverageDiscountAmount =  string.Empty;
                }

                if (enteredDividentInstallment > 0)
                {
                    double averageDividentPercent = Math.Round(enteredDividentPercent / enteredDividentInstallment, 2);
                    double averageDividentAmount = Math.Round(enteredDividentAmount / enteredDividentInstallment, 2);
                    ViewBag.AverageDivident = averageDividentPercent.ToString();
                    csmaster.AverageDivident = Convert.ToDouble(averageDividentPercent);
                    ViewBag.AverageDividentAmount = averageDividentAmount.ToString();
                }
                else
                {
                    ViewBag.AverageDivident = string.Empty;
                    ViewBag.AverageDividentAmount = string.Empty;
                }

               
            }
            
            
            cml.chitschemecollections.Where(x => x.CollectionIntervalTypeID == csmaster.AscertainmentFrequencyTypeId);
            ViewBag.SubscriptionAmount = string.Empty;
            ViewBag.SubscriptionAmount = cml.chitschemecollections[0].SuggestedAmount.ToString();
            csmaster.SubscriptionAmount= Convert.ToDouble(cml.chitschemecollections[0].SuggestedAmount);
            
            context.Entry(csmaster).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }

    }
}
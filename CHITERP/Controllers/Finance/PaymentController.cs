using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CHITERP.Data;
//using SACT_ERP.Context;
using CHITERP.Helper;
using CHITERP.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Controllers.Accounts
{

    [Authorize]
    public class PaymentController : Controller
    {
        // GET: Payment
        AppDbContext context = new AppDbContext();
        FinFusionEntities db = new FinFusionEntities();

        [Authorize(Roles = "PaymentIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0 || Session["CUSRID"].ToString() == "") { return RedirectToAction("Login", "Account"); }
            if (string.IsNullOrEmpty(Session["SDATE"] as string))
            {

                Session["SDATE"] = DateTime.Now.ToString("dd-MM-yyyy");
                Session["EDATE"] = DateTime.Now.ToString("dd-MM-yyyy");
            }
            else
            {
                if (Request.Form.Get("from") != null)
                {
                    Session["SDATE"] = Request.Form.Get("from");
                    Session["EDATE"] = Request.Form.Get("to");
                }
            }
            DateTime sd = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;
            DateTime ed = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;
            if (Request.Form.Get("LinkedOfficeID") != null && Request.Form.Get("LinkedOfficeID") != "")
            {
                Session["LinkedOfficeID"] = Request.Form.Get("LinkedOfficeID");
            }
            else
                Session["LinkedOfficeID"] = "0";
            return View();
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            int ofcid = Convert.ToInt32(Session["LinkedOfficeID"]);
            string crole = Convert.ToString(Session["Group"]);
            string cusrid = Convert.ToString(Session["CUSRID"]);
            int cempid = Convert.ToInt32(Session["EMPLID"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                cusrid = "";
            }

            using (var e = new FinFusionEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Payment(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]), cusrid, ofcid);
                var aaData = data.Select(d => new { TRANDATE = d.TRANDATE, TRANDNO = d.TRANDNO, TRANREFNAME = d.TRANREFNAME, TRANMODE = d.TRANMODE, TRAN_BANK_REF_NO = d.TRAN_BANK_REF_NO, TRAN_BANK_REF_DATE = d.TRAN_BANK_REF_DATE, TRANNAMT = d.TRANNAMT.ToString(), DISPSTATUS = d.DISPSTATUS, TRANMID = d.TRANMID.ToString() }).ToList();

                return Json(new
                {
                    //sEcho = param.sEcho,
                    data = aaData
                    //iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    //iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);


            }
        }

        [Authorize(Roles = "PaymentCreate")]
        public ActionResult GSTForm(string id = "0")
        {

            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0 || Session["CUSRID"].ToString() == "") { return RedirectToAction("Login", "Account"); }

            context.Database.ExecuteSqlCommand("Delete From TMP_PAYMENT_DETAIL Where KUSRID = '" + Session["CUSRID"].ToString() + "'");

            int ids = 0; int etype = 0;
            if (id.Contains("-"))
            {
                var param = id.Split('-'); etype = 1;
                ids = Convert.ToInt32(param[0]);
            }
            else
            {
                ids = Convert.ToInt32(id);
            }
            ViewBag.id = ids;
            TransactionMaster tab = new TransactionMaster();
            Transaction_List vm = new Transaction_List();

            ViewBag.TRANMODE = new SelectList(context.Fin_PaymentModes.Where(x => x.DISPSTATUS == 0), "FINPYMTMODEID", "FINPYMTMODEDESC");

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItemsts = new SelectListItem { Text = "IN BOOKS", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItemsts);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            ViewBag.ITEMQID = new SelectList(context.journal_entry_type_masters.Where(m => m.DISPSTATUS == 0 && m.JETID == 1).OrderBy(m => m.JETID), "JETID", "JETDESC");
            var amtcdqry3 = context.Database.SqlQuery<CompanyOffice>("Select * From CompanyOffice (nolock)").ToList();
            ViewBag.TRANREFID = new SelectList(amtcdqry3, "officeid", "OfficeName");

            var ahdqry = context.Database.SqlQuery<AccountHeadMaster>("Select * From AccountHeadMaster (nolock) where dispstatus = 0").ToList();
            ViewBag.TRANDREFID = new SelectList(ahdqry, "ACHEADID", "ACHEADDESC");
            if (ids != 0)
            {
                tab = context.transactionmasters.Find(ids);//-----------find id

                ViewBag.CATEAID = new SelectList(context.Database.SqlQuery<VW_CUSTOMER_LOCATION_CBX_ASSGN>("select CATEAID,LOCTDESC from VW_CUSTOMER_LOCATION_CBX_ASSGN Where CATEID = " + tab.TRANMID), "CATEAID", "LOCTDESC", tab.CATEAID);

                ViewBag.TRANMODE = new SelectList(context.Fin_PaymentModes.Where(x => x.DISPSTATUS == 0), "FINPYMTMODEID", "FINPYMTMODEDESC", tab.TRANMODE);

                if (etype == 1)
                {
                    tab.TRANMID = 0;
                    TransactionDetail tabs = new TransactionDetail();
                    tabs.TRANDID = 0;
                }

                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 0)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "IN BOOKS", Value = "0", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
                else if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "IN BOOKS", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
                amtcdqry3 = context.Database.SqlQuery<CompanyOffice>("Select * From CompanyOffice (nolock)").ToList();
                ViewBag.TRANREFID = new SelectList(amtcdqry3, "officeid", "OfficeName", tab.LinkedOfficeID);
                vm.masterdata = context.transactionmasters.Where(det => det.TRANMID == ids).ToList();
                vm.detaildata = context.transactiondetails.Where(det => det.TRANMID == ids).ToList();

            }
            return View(vm);
        }

        public ActionResult BForm(string id)/*BATCH*/
        {
            var param = id.Split(';');
            //var ldgrdesc = param[0];
            var ldgrid = 0;// Convert.ToInt32(param[0]);
            var trandid = 0;// Convert.ToInt32(param[1]);
            var atrandamt = "";
            var aldgramt = "";
            var rowid = 0;
            //dec trandamt = 0;

            context.Database.ExecuteSqlCommand("DELETE FROM TMP_NEW_RECEIPT_BILL_DETAIL WHERE KUSRID='" + Session["CUSRID"] + "'");

            if (param[0] != "") { ldgrid = Convert.ToInt32(param[0]); } else { ldgrid = 0; }
            if (param[1] != "") { trandid = Convert.ToInt32(param[1]); } else { trandid = 0; }
            if (param[2] != "") { atrandamt = param[2]; } else { atrandamt = "0"; }
            if (param[3] != "") { aldgramt = param[3]; } else { aldgramt = "0"; }
            if (param[4] != "") { rowid = Convert.ToInt32(param[4]); } else { rowid = 0; }

            atrandamt = atrandamt.Replace("~", ".");
            aldgramt = aldgramt.Replace("~", ".");

            //if (trandamt != "") { trandamt = Convert.ToDecimal(atrandamt); }
            Transaction_List vm = new Transaction_List();
            TransactionReceiptDetail tab = new TransactionReceiptDetail();
            Tmp_Receipt_Detail rtab = new Tmp_Receipt_Detail();

            ViewBag.id = trandid;
            ViewBag.V_ROWID = rowid;

            var cateid = 0;
            var zsql = db.Database.SqlQuery<AccountHeadMaster>("Select * From AccountHeadMaster Where ACHEADID = " + ldgrid).ToList();
            if (zsql.Count() > 0)
            {
                ViewBag.LDGRDESC = zsql[0].ACHEADDESC;
                ViewBag.TRANDREFID = zsql[0].CATEID;
                ViewBag.OAMT = atrandamt;
                ViewBag.LDGRAMT = aldgramt;
                cateid = zsql[0].CATEID;
            }

            ViewBag.RTID = new SelectList(context.receipt_type_details.Where(x => x.RTID > 0).OrderBy(x => x.RTDESC), "RTID", "RTDESC");
            ViewBag.TRANMID = new SelectList("");

            var sql = db.Database.SqlQuery<int>("Select TRANRID From Transaction_Receipt_Detail  Where TRANDID = " + trandid).ToList();
            if (sql.Count() > 0)
            {
                ViewBag.TRANMID = "";// new SelectList(context.Database.SqlQuery<pr_Customer_Wise_Pending_Invoice_Detail_Flx_Assgn_Result>("pr_Customer_Wise_Pending_Invoice_Detail_Flx_Assgn @PCATEID=" + Convert.ToInt32(cateid) + ",@PCOMPYID=" + Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"])), "TRANMID", "TRANDNO");
                vm.receipt_detail_data = context.Database.SqlQuery<TransactionReceiptDetail>("Select * From Transaction_Receipt_Detail Where TRANDID=" + trandid + "").ToList();
            }
            return View(vm);
        }

        //public ActionResult BForm(string id)/*BATCH*/
        //{
        //    var param = id.Split(';');
        //    //var ldgrdesc = param[0];
        //    var ldgrid = 0;// Convert.ToInt32(param[0]);
        //    var trandid = 0;// Convert.ToInt32(param[1]);
        //    var atrandamt = "";
        //    var rowid = 0;
        //    //dec trandamt = 0;

        //    context.Database.ExecuteSqlCommand("DELETE FROM TMP_NEW_PAYMENT_BILL_DETAIL WHERE KUSRID='" + Session["CUSRID"] + "'");

        //    if (param[0] != "") { ldgrid = Convert.ToInt32(param[0]); } else { ldgrid = 0; }
        //    if (param[1] != "") { trandid = Convert.ToInt32(param[1]); } else { trandid = 0; }
        //    if (param[2] != "") { atrandamt = param[2]; } else { atrandamt = "0"; }
        //    if (param[3] != "") { rowid = Convert.ToInt32(param[3]); } else { rowid = 0; }

        //    atrandamt = atrandamt.Replace("~", ".");

        //    //if (trandamt != "") { trandamt = Convert.ToDecimal(atrandamt); }
        //    Transaction_List vm = new Transaction_List();
        //    Transaction_Receipt_Detail tab = new Transaction_Receipt_Detail();
        //    Tmp_Payment_Detail rtab = new Tmp_Payment_Detail();

        //    ViewBag.id = trandid;
        //    ViewBag.V_ROWID = rowid;

        //    var cateid = 0;
        //    var zsql = db.Database.SqlQuery<AccountHeadMaster>("Select * From AccountHeadMaster Where ACHEADID = " + ldgrid).ToList();
        //    if (zsql.Count() > 0)
        //    {
        //        ViewBag.LDGRDESC = zsql[0].ACHEADDESC;
        //        ViewBag.TRANDREFID = zsql[0].CATEID;
        //        ViewBag.LDGRAMT = atrandamt;
        //        cateid = zsql[0].CATEID;
        //    }

        //    ViewBag.RTID = new SelectList(context.receipt_type_details.Where(x => x.RTID > 0).OrderBy(x => x.RTDESC), "RTID", "RTDESC");
        //    ViewBag.TRANMID = new SelectList("");

        //    var sql = db.Database.SqlQuery<int>("Select TRANRID From Transaction_Receipt_Detail  Where TRANDID = " + trandid).ToList();
        //    if (sql.Count() > 0)
        //    {
        //        ViewBag.TRANMID = new SelectList(context.Database.SqlQuery<pr_Customer_Wise_Pending_Invoice_Detail_Flx_Assgn_Result>("pr_Customer_Wise_Pending_Invoice_Detail_Flx_Assgn @PCATEID=" + Convert.ToInt32(cateid) + ",@PCOMPYID=" + Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"])), "TRANMID", "TRANDNO");
        //        vm.receipt_detail_data = context.Database.SqlQuery<Transaction_Receipt_Detail>("Select * From Transaction_Receipt_Detail Where TRANDID=" + trandid + "").ToList();
        //    }
        //    return View(vm);
        //}

        [Authorize]
        public ActionResult gstsavedata(FormCollection F_Form)
        {
            var F_TMP = Convert.ToInt32(Request.Form.Get("TMPSAVE"));
            string status = "";
            using (AppDbContext context = new AppDbContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        TransactionMaster TranMasts = new TransactionMaster();
                        TransactionDetail TranDtls = new TransactionDetail();
                        Int32 TRANMID = Convert.ToInt32(F_Form["masterdata[0].TRANMID"]);
                        Int32 TRANDID = 0;
                        string DELIDS = "";

                        var redirect = "";

                        if (TRANMID != 0)//Getting Primary id in Edit mode
                        {
                            TranMasts = context.transactionmasters.Find(TRANMID);

                        }
                        TranMasts.COMPYID = Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]);
                        TranMasts.SDPTID = 1;
                        TranMasts.REGSTRID = 22;
                        TranMasts.TRANBTYPE = 0;// Convert.ToInt16(F_Form["TRANBTYPE"]);
                        TranMasts.TRANTID = 0;// Convert.ToInt32(F_Form["masterdata[0].TRANTID"]);
                        TranMasts.TRANDATE = Convert.ToDateTime(F_Form["TRANDATE"]);
                        TranMasts.TRANREFID = Convert.ToInt32(F_Form["masterdata[0].TRANREFID"]);
                        TranMasts.LinkedOfficeID = Convert.ToInt32(F_Form["masterdata[0].LinkedOfficeID"]);
                        TranMasts.TRANREFNAME = F_Form["masterdata[0].TRANREFNAME"].ToString();
                        TranMasts.TRANREFADDR = "";// F_Form["masterdata[0].TRANREFADDR"].ToString();
                        TranMasts.TRANSDATE = TranMasts.TRANDATE;
                        TranMasts.TRANEDATE = TranMasts.TRANDATE;
                        TranMasts.TRANREFDATE = TranMasts.TRANDATE;
                        TranMasts.TRANREFNO = "";

                        TranMasts.TRANDCTHRU = "";
                        TranMasts.TRANEDESC = "";
                        TranMasts.TRANDTERM = "";
                        TranMasts.TRANPTERM = "";
                        TranMasts.TRANMTERM = "";//;
                        TranMasts.TRANNARTN = "";

                        TranMasts.TRANIDESC = "";
                        TranMasts.TRANADESC = "";
                        TranMasts.TRANPOREFNO = "";
                        TranMasts.TRANLRREFNO = "";//;
                        TranMasts.TRANREMRKS = F_Form["masterdata[0].TRANREMRKS"].ToString();
                        TranMasts.TRANLMID = 0;
                        TranMasts.TRANFTYPE = 0;

                        TranMasts.LMUSRID = "";
                        TranMasts.DISPSTATUS = Convert.ToInt16(F_Form["DISPSTATUS"]);
                        TranMasts.PRCSDATE = DateTime.Now;
                        TranMasts.QSTATUS = 0;
                        TranMasts.CATEAID = 0;
                        TranMasts.LOCTID = 0;
                        TranMasts.DLOCTID = 0;
                        TranMasts.TRANDELIADDR = "";
                        TranMasts.TRANQTYPE = 0;
                        TranMasts.TRANGAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANGAMT"]);
                        TranMasts.TRANNAMT = Convert.ToDecimal(F_Form["masterdata[0].TRANNAMT"]);
                        TranMasts.TRANCGSTAMT = 0;
                        TranMasts.TRANSGSTAMT = 0;
                        TranMasts.TRANIGSTAMT = 0;
                        TranMasts.TRANAMTWRDS = AmtInWrd.ConvertNumbertoWords(F_Form["masterdata[0].TRANGAMT"]);
                        TranMasts.CURNID = 0;
                        TranMasts.TRANCRATE = 0;

                        TranMasts.TRANCHK4 = 0;

                        TranMasts.TRANBCDTYPE = 0;
                        TranMasts.TRANSWSTYPE = 0;

                        TranMasts.TRANMODE = Convert.ToInt32(F_Form["TRANMODE"]);
                        //TranMasts.TRAN_BANK_REF_DATE = Convert.ToDateTime(DateTime.Now.Date);
                        if (Convert.ToInt32(F_Form["TRANMODE"]) != 2)
                        {
                            TranMasts.TRAN_BANK_NAME = F_Form["masterdata[0].TRAN_BANK_NAME"].ToString();
                            TranMasts.TRAN_BANK_REF_NO = F_Form["masterdata[0].TRAN_BANK_REF_NO"].ToString();
                            TranMasts.TRAN_BANK_REF_DATE = Convert.ToDateTime(F_Form["TRAN_BANK_REF_DATE"]);
                        }


                        if (TRANMID == 0)
                        {
                            TranMasts.TRANNO = Convert.ToInt16(Autonumber.transactiomaster_autonum("transactionmaster", "TRANNO", "REGSTRID = 22 and tranbtype = 0 and COMPYID=" + Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) + "").ToString());
                            int ano = TranMasts.TRANNO;
                            string prfx = string.Format("{0:D5}", ano);
                            string yrdesc = Session["GYrDesc"].ToString();
                            string ayrdesc = yrdesc.Substring(2, 2) + "-" + yrdesc.Substring(9, 2);
                            string aprfx = Session["COMPCODE"].ToString().Trim() + "/P/" + ayrdesc + "/" + prfx;
                            TranMasts.CUSRID = Session["CUSRID"].ToString();
                            TranMasts.TRANDNO = aprfx.ToString();
                            context.transactionmasters.Add(TranMasts);
                            context.SaveChanges(); redirect = "GSTform/" + TranMasts.TRANMID;

                            TRANMID = TranMasts.TRANMID;
                        }
                        else
                        {
                            TranMasts.LMUSRID = Session["CUSRID"].ToString();
                            context.Entry(TranMasts).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges(); redirect = "Index";
                            //Response.Write("Update <hr>");
                        }


                        //.......................transaction detail....................//
                        string[] A_TRANDID = F_Form.GetValues("TRANDID");
                        string[] TRANDREFNAME = F_Form.GetValues("TRANDREFNAME");/*Invoice no*/
                        string[] TRANDREFID = F_Form.GetValues("TRANDREFID");/* Invoice master id*/
                        if (TRANDREFID == null)
                            TRANDREFID = F_Form.GetValues("detaildata[0].TRANDREFID");
                        string[] TRANDGAMT = F_Form.GetValues("TRANDGAMT");
                        string[] TRANDAAMT = F_Form.GetValues("TRANDAAMT");
                        string[] TRAND_TOT_COST = F_Form.GetValues("TRAND_TOT_COST");
                        string[] TRANDCTYPE = F_Form.GetValues("ACHEADTYPE");
                        string[] A_ITEMQID = F_Form.GetValues("A_ITEMQID");

                        string booltype = "true";// F_Form.GetValues("booltype");
                        string boolSTFDIDS = "true";// F_Form.GetValues("booltype");


                        for (int count = 0; count < TRANDREFNAME.Count(); count++)
                        {
                            if (boolSTFDIDS == "true")
                            {

                                TRANDID = Convert.ToInt32(A_TRANDID[count]);
                                var boolSTFDID = Convert.ToString(boolSTFDIDS[count]);
                                if (TRANDID != 0 && boolSTFDIDS == "true")
                                {
                                    TranDtls = context.transactiondetails.Find(TRANDID);
                                }
                                TranDtls.TRANMID = TranMasts.TRANMID;
                                TranDtls.TRANDGREFID = 0;
                                TranDtls.MTRLID = 0;
                                TranDtls.TRANDREFID = Convert.ToInt32(TRANDREFID[count]);
                                TranDtls.TRANDCTYPE = Convert.ToInt16(TRANDCTYPE[count]);
                                TranDtls.TRANDREFNAME = TRANDREFNAME[count].ToString();
                                TranDtls.TRANDREFNO = "";
                                TranDtls.TRANDCREFNO = "";
                                TranDtls.TRAND_ODESC = "";
                                TranDtls.TRANDQTY = 0;
                                TranDtls.TRANAQTY = 0;
                                TranDtls.TRANRQTY = 0;
                                TranDtls.UNITID = 0;
                                TranDtls.TRANDRATE = 0;
                                TranDtls.TRANDLRATE = 0;
                                TranDtls.TRANDARATE = 0;
                                TranDtls.TRANDAMT = Convert.ToDecimal(TRANDGAMT[count]);
                                TranDtls.TRANDGAMT = Convert.ToDecimal(TRANDGAMT[count]);
                                TranDtls.TRANDNAMT = Convert.ToDecimal(TRANDGAMT[count]);
                                TranDtls.TRANDAAMT = 0;
                                if (TRANDAAMT[count] != "")
                                    TranDtls.TRANDAAMT = Convert.ToDecimal(TRANDAAMT[count]);

                                TranDtls.TRANDDISCEXPRN = 0;
                                TranDtls.TRANDDISCAMT = 0;
                                TranDtls.TRANDCGSTEXPRN = 0;
                                TranDtls.TRANDCGSTAMT = 0;
                                TranDtls.TRANDSGSTEXPRN = 0;
                                TranDtls.TRANDSGSTAMT = 0;
                                TranDtls.TRANDIGSTEXPRN = 0;
                                TranDtls.TRANDIGSTAMT = 0;
                                TranDtls.HSNID = 0;

                                TranDtls.TRAND_EXCESS_QTY = 0;
                                TranDtls.TRAND_SHORT_QTY = 0;
                                TranDtls.TRAND_DAMAGE_QTY = 0;

                                TranDtls.TRAND_TOT_COST = Convert.ToDecimal(TRAND_TOT_COST[count]);

                                TranDtls.ITEMQID = Convert.ToInt32(A_ITEMQID[count]);

                                TranDtls.TRANDAID = Convert.ToInt32(TRANDREFID[count]);
                                TranDtls.TRANDCID = 0;
                                TranDtls.DISPSTATUS = TranMasts.DISPSTATUS;
                                TranDtls.PRCSDATE = System.DateTime.Now;


                                if (Convert.ToInt32(TRANDID) == 0)
                                {
                                    context.transactiondetails.Add(TranDtls);
                                    context.SaveChanges();
                                    TRANDID = TranDtls.TRANDID;
                                    redirect = "Dform/" + TranMasts.TRANMID;
                                }
                                else
                                {
                                    context.Entry(TranDtls).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                }
                                DELIDS = DELIDS + "," + TRANDID.ToString();

                            }
                        }

                        //..............delete transaction master factor record

                        context.Database.ExecuteSqlCommand("DELETE FROM transactiondetail  WHERE tranmid=" + TRANMID + " and  TRANDID NOT IN(" + DELIDS.Substring(1) + ")");
                        //Response.Write(DELIDS.Substring(1));
                        //Response.Redirect("Index");

                        trans.Commit();
                        //if (TranMasts.TRANMID > 0)
                        //{
                        //    Payment_Detail_Update(TranMasts.TRANMID);
                        //    //Update_IBPO_Approval_Over_Due_Amount_Update_Detail(TranMasts.TRANMID);

                        //}
                        status = "Success";
                        return Json(status, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        var aa = ex.Message;
                        trans.Rollback();
                        //Response.Redirect("/Error/SavepointErr");
                        status = "Error";
                        return Json(status, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        } //...........end

        public void Payment_Detail_Update(int rtranmid)
        {
            try
            {
                var Query = context.Database.SqlQuery<int>("select TRANMID From TransactionDetail where TRANMID = " + rtranmid).ToList();
                if (Query.Count() >= 0)
                {
                    context.Database.ExecuteSqlCommand("pr_Transaction_Payment_Detail @PTRANMID = " + rtranmid + ", @PKUSRID = '" + Session["CUSRID"].ToString() + "'");
                    context.Database.ExecuteSqlCommand("pr_Receipt_Master_TranRefName_Update_Assgn @PTRANMID = " + rtranmid);
                    //Response.Write("Saved Successfully");
                }
            }
            catch (Exception e)
            {
                Response.Write(e.Message);
            }
        }

        [Authorize]
        public void B_Tmp_Savedata(FormCollection sfrm)//......batch save
        {
            int TRANDID = Convert.ToInt32(sfrm["V_TRANDID"]);
            int TRANDREFID = Convert.ToInt32(sfrm["V_TRANDREFID"]);
            string TRANDREFNAME = sfrm["LDGRDESC"];
            int TRANDRID = 0;
            int btype = 0;
            string S_DELIDS = "";

            Tmp_Payment_Detail Tmp_Payment_Detail = new Tmp_Payment_Detail();
            //int TRANDMID = Convert.ToInt32(sfrm["V_TRANDMID"]);
            //    int TRANDID = Convert.ToInt32(sfrm["V_TRANDID"]);

            string[] T_RTID = sfrm.GetValues("RTID");
            string[] T_TRANMID = sfrm.GetValues("RTRANMID");
            string[] TRANRREFNO = sfrm.GetValues("TRANRREFNO");
            string[] TRANRREFDATE = sfrm.GetValues("TRANRREFDATE");
            string[] TRANRAMT = sfrm.GetValues("TRANRAMT");
            string[] TRANR_BAL_AMT = sfrm.GetValues("TRANBALAMT");
            string booltype = "true";// sfrm.GetValues("booltype");

            for (int row = 0; row < T_RTID.Count(); row++)
            {
                //var DL_TRANDPID = Convert.ToInt32(T_TRANSDID[row]);
                var bools = booltype;// booltype[row];
                if (bools == "true")
                {
                    TRANDRID = 0;// Convert.ToInt32(T_TRANSDID[row]);

                    //if (TRANDPID != 0)
                    //{
                    //    TransactionProductSerialDetail = context.transactionproductserialdetails.Find(TRANDPID);
                    //}

                    switch (Convert.ToInt32(T_RTID[row]))
                    {
                        case 1:
                            if (TRANRREFNO[row].ToString().Contains("OPF")) { btype = 2; }
                            if (TRANRREFNO[row].ToString().Contains("IB")) { btype = 3; }
                            if (TRANRREFNO[row].ToString().Contains("PI")) { btype = 4; }
                            break;
                        case 2:
                            btype = 1;//Invoice
                            break;
                        case 3:
                            btype = 0;//New Ref
                            break;
                        case 4:
                            btype = 0;//On Account
                            break;
                        default:
                            btype = 0;//Invoice
                            break;
                    }
                    Tmp_Payment_Detail.KUSRID = Session["CUSRID"].ToString();
                    Tmp_Payment_Detail.TRANDREFID = Convert.ToInt32(TRANDREFID);
                    Tmp_Payment_Detail.TRANRREFNAME = TRANDREFNAME.ToString();
                    Tmp_Payment_Detail.RTID = Convert.ToInt32(T_RTID[row]);
                    Tmp_Payment_Detail.TRANMID = Convert.ToInt32(T_TRANMID[row]);

                    Tmp_Payment_Detail.TRANRREFNO = TRANRREFNO[row].ToString();
                    Tmp_Payment_Detail.TRANRREFDATE = Convert.ToDateTime(TRANRREFDATE[row]);
                    Tmp_Payment_Detail.TRANRAMT = Convert.ToDecimal(TRANRAMT[row]);
                    Tmp_Payment_Detail.TRANR_BAL_AMT = Convert.ToDecimal(TRANR_BAL_AMT[row]);
                    Tmp_Payment_Detail.BTYPE = Convert.ToInt32(btype);

                    if (TRANDRID == 0)
                    {
                        context.tmp_payment_details.Add(Tmp_Payment_Detail);
                        context.SaveChanges();
                        //TRANDPID = TransactionProductSerialDetail.TRANDPID;

                        //context.Database.ExecuteSqlCommand("UPDATE TransactionProductSerialDetail SET TRANBQTY=" + (Convert.ToDecimal(BQTY[row]) - Convert.ToDecimal(TRANPQTY[row])) + ",TRANIQTY=" + (Convert.ToDecimal(TRANIQTY[row]) + Convert.ToDecimal(TRANPQTY[row])) + " WHERE TRANDPID=" + Convert.ToInt32(ATRANDPID[row]) + "");
                    }
                    //else
                    //{
                    //    context.Entry(TransactionProductSerialDetail).State = System.Data.Entity.EntityState.Modified;
                    //    context.SaveChanges();
                    //}
                }
                //else
                //{
                //    if (DL_TRANDPID != 0)
                //    {
                //        // Response.Write((Convert.ToDecimal(AQTY[row]) - Convert.ToDecimal(BQTY[row])) + "//../iqty/" + (Convert.ToDecimal(TRANIQTY[row]) + (Convert.ToDecimal(TRANPQTY[row]))));
                //        context.Database.ExecuteSqlCommand("UPDATE TransactionProductSerialDetail SET TRANBQTY=" + (Convert.ToDecimal(BQTY[row])) + ",TRANIQTY=" + (Convert.ToDecimal(TRANIQTY[row]) - (Convert.ToDecimal(TRANPQTY[row]))) + " WHERE TRANDPID=" + Convert.ToInt32(ATRANDPID[row]) + "");
                //        context.Database.ExecuteSqlCommand("DELETE FROM TransactionProductSerialDetail  WHERE TRANDID=" + TRANDID + " and  TRANDPID  IN(" + DL_TRANDPID + ")");
                //    }
                //}
                //S_DELIDS = S_DELIDS + "," + TRANDPID.ToString();
            }
            // context.Database.ExecuteSqlCommand("DELETE FROM TransactionDetailProductSerial  WHERE TRANDID=" + TRANDID + " and  TRANDPID NOT IN(" + S_DELIDS.Substring(1) + ")");
            Response.Write("saved");
        }

        [Authorize]
        public void B_Org_Savedata(FormCollection sfrm)//......batch save
        {
            int TRANDID = Convert.ToInt32(sfrm["V_TRANDID"]);
            int TRANDREFID = Convert.ToInt32(sfrm["V_TRANDREFID"]);
            string TRANDREFNAME = sfrm["LDGRDESC"];
            int TRANRID = 0;
            int btype = 0;
            string S_DELIDS = "";

            TransactionReceiptDetail TranReceptDtls = new TransactionReceiptDetail();

            string[] T_TRANRID = sfrm.GetValues("TRANRID");
            string[] T_RTID = sfrm.GetValues("ARTID");
            string[] T_TRANMID = sfrm.GetValues("RTRANMID");
            string[] TRANRREFNO = sfrm.GetValues("TRANRREFNO");
            string[] TRANRREFDATE = sfrm.GetValues("TRANRREFDATE");
            string[] TRANRAMT = sfrm.GetValues("TRANRAMT");
            string[] TRANR_BAL_AMT = sfrm.GetValues("TRANBALAMT");

            string booltype = "true";// sfrm.GetValues("booltype");

            for (int row = 0; row < T_TRANRID.Count(); row++)
            {
                var bools = booltype;// booltype[row];
                if (bools == "true")
                {
                    TRANRID = Convert.ToInt32(T_TRANRID[row]);

                    if (TRANRID != 0)
                    {
                        TranReceptDtls = context.transactionreceiptdetails.Find(TRANRID);
                    }

                    switch (Convert.ToInt32(T_RTID[row]))
                    {
                        case 1:
                            if (TRANRREFNO[row].ToString().Contains("OPF")) { btype = 2; }
                            if (TRANRREFNO[row].ToString().Contains("IB")) { btype = 3; }
                            if (TRANRREFNO[row].ToString().Contains("PI")) { btype = 4; }
                            break;
                        case 2:
                            btype = 1;//Invoice
                            break;
                        case 3:
                            btype = 0;//New Ref
                            break;
                        case 4:
                            btype = 0;//On Account
                            break;
                        default:
                            btype = 0;//Invoice
                            break;
                    }

                    TranReceptDtls.TRANDID = TRANDID;
                    TranReceptDtls.TRANDREFID = Convert.ToInt32(TRANDREFID);
                    TranReceptDtls.TRANRREFNAME = TRANDREFNAME.ToString();
                    TranReceptDtls.RTID = Convert.ToInt32(T_RTID[row]);
                    TranReceptDtls.TRANMID = Convert.ToInt32(T_TRANMID[row]);

                    TranReceptDtls.TRANRREFNO = TRANRREFNO[row].ToString();
                    TranReceptDtls.TRANRREFDATE = Convert.ToDateTime(TRANRREFDATE[row]);
                    TranReceptDtls.TRANRAMT = Convert.ToDecimal(TRANRAMT[row]);
                    TranReceptDtls.TRANR_BAL_AMT = Convert.ToDecimal(TRANR_BAL_AMT[row]);
                    TranReceptDtls.BTYPE = Convert.ToInt32(btype);

                    if (TRANRID == 0)
                    {
                        context.transactionreceiptdetails.Add(TranReceptDtls);
                        context.SaveChanges();
                    }
                    else
                    {
                        context.Entry(TranReceptDtls).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                }

            }
            // context.Database.ExecuteSqlCommand("DELETE FROM TransactionDetailProductSerial  WHERE TRANDID=" + TRANDID + " and  TRANDPID NOT IN(" + S_DELIDS.Substring(1) + ")");
            Response.Write("saved");
        }


        public JsonResult AutoLedgerMaster(string term)
        {
            var result = (from r in context.accountheadmasters
                          where r.ACHEADDESC.ToLower().Contains(term.ToLower())
                          //select new { r.ACHEADDESC, r.ACHEADID, r.ACHEADGID }).Where(x => x.ACHEADGID == 1 || x.ACHEADGID == 2).OrderBy(x => x.ACHEADDESC).Distinct();
                          select new { r.ACHEADDESC, r.ACHEADID, r.ACHEADGID }).OrderBy(x => x.ACHEADDESC).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLedgerDetail(int id)
        {
            var result = (from r in context.accountheadmasters
                          where r.ACHEADID == id
                          select new { r.ACHEADDESC, r.ACHEADID, r.CATEID, r.ACHEADTYPE, r.DISPSTATUS }).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.ACHEADDESC).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AutoLedgerDetail(string term)
        {
            var result = (from r in context.accountheadmasters
                          where r.ACHEADDESC.ToLower().Contains(term.ToLower())
                          select new { r.ACHEADDESC, r.ACHEADID, r.CATEID, r.ACHEADTYPE, r.DISPSTATUS }).Where(x => x.DISPSTATUS == 0).OrderBy(x => x.ACHEADDESC).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerOutstanding(string ids)
        {
            var cateid = 0;
            var cateaid = 0;
            var param = ids.Split('~');
            if (param[0] != "") { cateid = Convert.ToInt32(param[0]); } else { cateid = 0; }
            if (param[1] != "") { cateaid = Convert.ToInt32(param[1]); } else { cateaid = 0; }
            var result = "";// context.Database.SqlQuery<pr_IBPO_CUSTOMER_OVER_DUE_DETAIL_ASSGN_C02_Result>("EXEC pr_IBPO_CUSTOMER_OVER_DUE_DETAIL_ASSGN_C02 @PCateID = " + cateid + ", @PCateAID = " + cateaid).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBillTypeDescription(string ids)
        {

            var result = context.Database.SqlQuery<Receipt_Type_Detail>("Select RTID, RTDESC From Receipt_Type_Detail Order by RTID").ToList();
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetInvoiceNoDetail(string ids)
        {

            var compyid = Session["COMPYID"].ToString();
            var cusrid = Session["CUSRID"].ToString();
            var cateid = 0;
            var rtid = 0;

            var param = ids.Split(';');
            if (param[0] != "") { cateid = Convert.ToInt32(param[0]); } else { cateid = 0; }
            if (param[1] != "") { rtid = Convert.ToInt32(param[1]); } else { rtid = 0; }


            var result = "";// context.Database.SqlQuery<pr_New_Receipt_Customer_Wise_Pending_Invoice_Detail_Flx_Assgn_Result>("pr_New_Receipt_Customer_Wise_Pending_Invoice_Detail_Flx_Assgn @PKUSRID = '" + cusrid + "', @PCATEID =" + cateid + ", @RTID = " + rtid).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInvoiceDetail(string ids)
        {
            //var result = context.Database.SqlQuery<pr_Transaction_Master_ID_Assgn_Result>("pr_Transaction_Master_ID_Assgn @PTranMID = " + id).ToList();
            var cateid = 0;
            var rtid = 0;
            var tranmid = 0;
            var tranno = "";
            var btype = 0;

            var param = ids.Split(';');
            if (param[0] != "") { cateid = Convert.ToInt32(param[0]); } else { cateid = 0; }
            if (param[1] != "") { rtid = Convert.ToInt32(param[1]); } else { rtid = 0; }
            if (param[2] != "") { tranmid = Convert.ToInt32(param[2]); } else { tranmid = 0; }
            if (param[3] != "") { tranno = param[3]; } else { tranno = ""; }

            tranno = tranno.Replace("~", "/");

            switch (rtid)
            {
                case 1:
                    if (tranno.Contains("OPF")) { btype = 2; }
                    if (tranno.Contains("IB")) { btype = 3; }
                    if (tranno.Contains("PI")) { btype = 4; }
                    break;
                case 2:
                    btype = 1;//Invoice
                    break;
                case 3:
                    btype = 0;//New Ref
                    break;
                case 4:
                    btype = 0;//On Account
                    break;
                default:
                    btype = 0;//Invoice
                    break;
            }

            var result = "";// context.Database.SqlQuery<pr_New_Receipt_Customer_Wise_Pending_Invoice_Wise_Detail_Flx_Assgn_Result>("pr_New_Receipt_Customer_Wise_Pending_Invoice_Wise_Detail_Flx_Assgn @PCATEID =" + cateid + ",@PTRANMID=" + Convert.ToInt32(tranmid) + ", @RTID = " + rtid + ", @PBTYPE = " + btype).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void TMP_INSERT_DEL()
        {

            context.Database.ExecuteSqlCommand("DELETE FROM TMP_NEW_PAYMENT_BILL_DETAIL WHERE KUSRID='" + Session["CUSRID"] + "'");
            Response.Write("SAV");
        }

        public void TMP_INSERT(string ids)
        {
            var cusrid = Session["CUSRID"].ToString();
            var rtypedesc = "";
            var rtid = 0;
            var tranmid = 0;
            var tranno = "";
            var btype = 0;
            var ramt = "";
            var zramt = 0.00;

            var param = ids.Split(';');
            if (param[0] != "") { rtid = Convert.ToInt32(param[0]); } else { rtid = 0; }
            if (param[1] != "") { tranmid = Convert.ToInt32(param[1]); } else { tranmid = 0; }
            if (param[2] != "") { tranno = param[2]; } else { tranno = ""; }
            if (param[3] != "") { ramt = param[3]; } else { ramt = "0"; }
            ramt = ramt.Replace("~", ".");
            tranno = tranno.Replace("~", "/");
            zramt = Convert.ToDouble(ramt);

            switch (rtid)
            {
                case 1:
                    if (tranno.Contains("OPF")) { btype = 2; }
                    if (tranno.Contains("IB")) { btype = 3; }
                    if (tranno.Contains("PI")) { btype = 4; }
                    break;
                case 2:
                    btype = 1;//Invoice
                    break;
                case 3:
                    btype = 0;//New Ref
                    break;
                case 4:
                    btype = 0;//On Account
                    break;
                default:
                    btype = 0;//Invoice
                    break;
            }

            context.Database.ExecuteSqlCommand("INSERT INTO TMP_NEW_PAYMENT_BILL_DETAIL(KUSRID,RTYPE,BTYPE,RID,RAMT) VALUES('" + cusrid + "'," + Convert.ToInt32(rtid) + "," + Convert.ToInt32(btype) + "," + Convert.ToInt32(tranmid) + "," + Convert.ToDecimal(zramt) + ")");
            Response.Write("SAV");
        }

        [Authorize(Roles = "PaymentDelete")]
        public void Del()//..index delete
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = TD_DEL.delete_check1(fld, id, "TRANSACTIONMASTER");
            if (temp.Equals("PROCEED"))
            {
                context.Database.ExecuteSqlCommand("USP_Receipt_Over_Due_Amount_Delete_Update @PRTranMId = " + Convert.ToInt32(id));

                TransactionMaster TransactionMaster = context.transactionmasters.Find(Convert.ToInt32(id));
                context.transactionmasters.Remove(TransactionMaster);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }

        //..........................Printview...
        [Authorize(Roles = "PaymentPrint")]
        public void PrintView(int? id = 0)
        {

            //  ........delete TMPRPT...//

            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = InsertToTMP("TMPRPT_IDS", "OPTNSTR", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;



                // cryRpt.Load(Server.MapPath("~/") + "//Reports//RPT_0302.rpt");


                //........Get TRANPCOUNT...//
                var Query = context.Database.SqlQuery<int>("select TRANPCOUNT from transactionmaster where TRANMID=" + id).ToList();
                var PCNT = 0;

                if (Query.Count() != 0) { PCNT = Query[0]; }
                var TRANPCOUNT = ++PCNT;
                // Response.Write(++PCNT);
                // Response.End();

                context.Database.ExecuteSqlCommand("UPDATE transactionmaster SET TRANPCOUNT=" + TRANPCOUNT + " WHERE TRANMID=" + id);

                var strPath = ConfigurationManager.AppSettings["Reporturl"];

                cryRpt.Load(strPath + "\\PaymentDetail.rpt");

                cryRpt.RecordSelectionFormula = "{VW_PAYMENT_DETAIL_RPT.TRANMID} =" + id + " And {VW_PAYMENT_DETAIL_RPT.KUSRID} = '" + Session["CUSRID"] + "'";

                String constring = ConfigurationManager.ConnectionStrings["IdentityCon"].ConnectionString;
                SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
                crConnectionInfo.ServerName = stringbuilder.DataSource;
                crConnectionInfo.DatabaseName = stringbuilder.InitialCatalog;
                crConnectionInfo.UserID = stringbuilder.UserID;
                crConnectionInfo.Password = stringbuilder.Password;

                CrTables = cryRpt.Database.Tables;
                foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
                {
                    crtableLogoninfo = CrTable.LogOnInfo;
                    crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                    CrTable.ApplyLogOnInfo(crtableLogoninfo);
                }


                cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                //cryRpt.PrintToPrinter(1,false,0,0);
                cryRpt.Dispose(); cryRpt.Close();
            }

        }
        //end

        public static string InsertToTMP(String table_name, String table_fld1, int table_fld2, string cusrid)
        {

            String temp = "";
            using (var context = new AppDbContext())
            {


                //........addng value to TMPRPT....//
                TMPRPT_IDS RPTIDS = new TMPRPT_IDS();
                RPTIDS.KUSRID = cusrid;
                RPTIDS.OPTNSTR = table_fld1;
                RPTIDS.RPTID = Convert.ToInt32(table_fld2);

                context.TMPRPT_IDS.Add(RPTIDS);
                context.SaveChanges();//...End

                temp = "Successfully Added";
            }

            return temp;
        }

        public class TD_DEL
        {
            AppDbContext context = new AppDbContext();

            public static string delete_check1(string fname, string pid, string tabname)
            {
                var TmpPrcdStatus = "PROCEED";
                var TmpRCount = 0; var Tmpcount = 0;
                using (var context = new AppDbContext())
                {
                    //var s = context.Database.SqlQuery<Soft_Table_Delete_Detail>("select * from SOFT_TABLE_DELETE_DETAIL where OPTNSTR= '" + fname + "' and TABNAME='" + tabname + "' Order by TABDID");
                    var s = context.Database.SqlQuery<Soft_Table_Delete_Detail>("select * from SOFT_TABLE_DELETE_DETAIL where OPTNSTR= '" + fname + "' Order by TABDID");
                    var ss = s;


                    foreach (var sss in ss)
                    {
                        var Tablename = sss.TABNAME;
                        //var m = Tablename;
                        //return m;
                        var fieldname = sss.PFLDNAME;

                        var condstr = sss.DCONDTNSTR + pid;

                        var Dispdesc = sss.DISPDESC;

                        TmpRCount = recordCount(Tablename, fieldname, condstr);
                        //Tmpcount = recordCount_det("TRANSACTIONDETAIL", "TRANDID", "TRANDCID=" + pid + "");



                        if (TmpRCount > 0)
                        {
                            TmpPrcdStatus = Dispdesc; break;
                        }
                        //if (Tmpcount > 0)
                        //{
                        //    TmpPrcdStatus = "Selected Record Referred in Other  Transaction"; break;
                        //}
                        //  return TmpPrcdStatus;
                    }
                    return TmpPrcdStatus;
                }
            }


            public static int recordCount_det(string Tablename, string fieldname, string condstr)
            {
                AppDbContext context = new AppDbContext();



                if (condstr.Trim().Length > 0)
                {


                    var d = context.Database.SqlQuery<Int32>("select Count(" + fieldname + ") As Rcount from " + Tablename + " where " + condstr + " Group by " + fieldname).ToList();
                    // return d.Count();
                    var count = d.Count();
                    if (count != 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;

                    }

                }

                return 2;

            }
            public static int recordCount(string Tablename, string fieldname, string condstr)
            {
                AppDbContext context = new AppDbContext();



                if (condstr.Trim().Length > 0)
                {


                    var d = context.Database.SqlQuery<Int32>("select Count(" + fieldname + ") As Rcount from " + Tablename + " where " + condstr + " Group by " + fieldname).ToList();
                    // return d.Count();
                    var count = d.Count();
                    if (count != 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;

                    }

                }

                return 2;

            }

        }
    }
}
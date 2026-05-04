using CHITERP.Data;
using CHITERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Controllers.Products
{
    public class ArrearChitSubscriptionController : Controller
    {
        // GET: ArrearSubscription
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
                    string colagt = Convert.ToString(Session["ColAgent"]);
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
                                                    filteredRowsCount, Convert.ToString(cusrid), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]),
                                                    Convert.ToInt32(colagt), 1);
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
    }
}
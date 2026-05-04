using CHITERP.Data;
using CHITERP.Models;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Controllers.Reports
{
    public class DailyCollectionPlanRptController : Controller
    {
        // GET: DailyCollectionPlanRpt
        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        public ActionResult Index()
        {
            string cusrid = Convert.ToString(Session["CUSRID"]);
            string crole = Convert.ToString(Session["Group"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                cusrid = "";
            }
            var personqry = context.Database.SqlQuery<pr_Chit_Subscription_Organization_Name_Assgn_Result>("Exec pr_Chit_Subscription_Organization_Name_Assgn").ToList();
            ViewBag.FPersonID = new SelectList(personqry, "CLIENTID", "CNAME").ToList();
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
            if (string.IsNullOrEmpty(Session["CollectionAgent"] as string))
            {
                if (Request.Form.Get("CollectionAgent") != null && Request.Form.Get("CollectionAgent") != "")
                {
                    Session["CollectionAgent"] = Request.Form.Get("CollectionAgent");
                }
            }
            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Product', ''").ToList();
            ViewBag.ProductType = new SelectList(mtqry.Where(x => x.Type == "Chit Fund" || x.Type == "Loan"), "TypeID", "Type", Convert.ToInt32(Session["ProductType"])).ToList();
            var amtcdqry4 = context.Database.SqlQuery<CompanyRoute>("select* from CompanyRoute (nolock)").ToList();
            ViewBag.RouteID = new SelectList(amtcdqry4, "RouteID", "RouteName", Convert.ToInt32(Session["RouteID"]));
            var empqry = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_Get_Employee_Dtl_Rpt @usrid='" + cusrid + "'").ToList();
            ViewBag.CollectionAgent = new SelectList(empqry, "DValue", "DText", Convert.ToInt32(Session["CollectionAgent"])).ToList();
            return View();
        }


        public void DailyCollectionPlanPDFRpt()
        {
            string ClientID = Request.Form.Get("FPersonID");
            string ProdType = Request.Form.Get("ProductType");
            string fromdt = Convert.ToDateTime(Request.Form.Get("from")).ToString("yyyy-MM-dd");
            string todt = Convert.ToDateTime(Request.Form.Get("to")).ToString("yyyy-MM-dd"); ;
            string route = Request.Form.Get("RouteID");
            string colagt = Request.Form.Get("CollectionAgent");

            string cusrid = Convert.ToString(Session["CUSRID"]);
            string crole = Convert.ToString(Session["Group"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                cusrid = "";
            }

            int clntid = 0;
            if (ClientID != "" && ClientID != "undefined")
                clntid = Convert.ToInt32(ClientID);
            int routeid = 0;
            if (route != "" && route != "undefined")
                routeid = Convert.ToInt32(route);
            int colagtid = 0;
            if (colagt != "" && colagt != "undefined")
                colagtid = Convert.ToInt32(colagt);
            int prdtid = 0;
            if (ProdType != "" && ProdType != "undefined")
                prdtid = Convert.ToInt32(ProdType);

            int compyid = Convert.ToInt32(Session["compyid"]);

            string query = "";

            query = "exec [pr_Get_Chit_Daily_Collection_Plan] @ProductTypeID = " + prdtid;
            query = query + ", @RouteID = " + routeid;
            query = query + ", @CollectionAgentID = " + colagtid;
            query = query + ", @FrDt = '" + fromdt + "'";
            query = query + ", @ToDt = '" + todt + "'";

            String constring = ConfigurationManager.ConnectionStrings["IdentityCon"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

            ReportDocument cryRpt = new ReportDocument();
            TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
            TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
            ConnectionInfo crConnectionInfo = new ConnectionInfo();
            CrystalDecisions.CrystalReports.Engine.Tables CrTables;
            string rptname = ConfigurationManager.AppSettings["Reporturl"] + "DailyCollectionPlan.RPT";
            cryRpt.Load(rptname);


            cryRpt.SetParameterValue("@ProductTypeID", prdtid);
            cryRpt.SetParameterValue("@RouteID", routeid);
            cryRpt.SetParameterValue("@CollectionAgentID", colagtid);
            cryRpt.SetParameterValue("@FrDt", fromdt);
            cryRpt.SetParameterValue("@ToDt", todt);

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
    }
}
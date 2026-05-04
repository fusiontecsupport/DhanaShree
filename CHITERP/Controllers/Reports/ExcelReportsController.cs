using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Web.Mvc;
using CHITERP.Models;
using ClosedXML.Excel;
using CHITERP.Data;
using System.Linq;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace CHITERP.Controllers.Reports
{
    public class ExcelReportsController : Controller
    {
        // GET: ExcelReports

        #region context 
        AppDbContext context = new AppDbContext();
        #endregion
        public ActionResult PartyAcDtlRptIndex()
        {

            var personqry = context.Database.SqlQuery<pr_Chit_Subscription_Organization_Name_Assgn_Result>("Exec pr_Chit_Subscription_Organization_Name_Assgn").ToList();
            ViewBag.FPersonID = new SelectList(personqry, "CLIENTID", "CNAME").ToList();

            return View();
        }

        public void GeneratePartyAcctDetailsReport()
        {
            string ClientID = Request.Form.Get("FPersonID");
            int clntid = 0;
            if (ClientID!="" && ClientID!="undefined")
                clntid = Convert.ToInt32(ClientID);
            
            int compyid = Convert.ToInt32(Session["compyid"]);
            string filename = "PartyACDtl_" + ClientID.ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            filename = filename + ".xlsx";
            string query = "";

            query = "exec pr_update_active_custaccount_ledgers @custid = '" + ClientID + "'";
            context.Database.ExecuteSqlCommand(query);

            query = "exec [pr_Customer_Wise_Account_Detail_Assgn] @PCustID = " + clntid ;

            DataTable dt0 = new DataTable();
            DataTable dt = new DataTable();

            String constring = ConfigurationManager.ConnectionStrings["IdentityCon"].ConnectionString;
            SqlConnection con = new SqlConnection(constring);

            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds, "Party_Accounts_Summary");
            //da.Fill(dt);
            con.Close();

            using (XLWorkbook wb = new XLWorkbook())
            {
                //dt.TableName = "ClientAccountsLedgersRpt";
                //wb.Worksheets.Add(dt);
                dt0 = ds.Tables[0];
                dt = ds.Tables[1];
                dt0.TableName = "PartyInfo";
                wb.Worksheets.Add(dt0);
                dt.TableName = "ClientAccountsLedgersRpt";
                wb.Worksheets.Add(dt);


                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                wb.Style.Border = null;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= " + filename);// Collection_Details.xlsx");

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    //Response.End();
                }
            }
        }

        public void GeneratePartyAcctDetailsReportPDF()
        {
            string ClientID = Request.Form.Get("FPersonID");
            int clntid = 0;
            if (ClientID != "" && ClientID != "undefined")
                clntid = Convert.ToInt32(ClientID);

            int compyid = Convert.ToInt32(Session["compyid"]);
            string filename = "PartyACDtl_" + ClientID.ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            filename = filename + ".xlsx";
            string query = "";

            query = "exec pr_update_active_custaccount_ledgers @custid = '" + ClientID + "'";
            context.Database.ExecuteSqlCommand(query);

            query = "exec [pr_Customer_Wise_Account_Detail_Assgn] @PCustID = " + clntid;
            String constring = ConfigurationManager.ConnectionStrings["IdentityCon"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

            ReportDocument cryRpt = new ReportDocument();
            TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
            TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
            ConnectionInfo crConnectionInfo = new ConnectionInfo();
            CrystalDecisions.CrystalReports.Engine.Tables CrTables;

            string rptpath = ConfigurationManager.AppSettings["Reporturl"];
            string rptname = "CustomerAccountsSummary.rpt";
            
            if (System.IO.File.Exists(rptpath + "\\" + rptname))
            {
                Console.Write(rptpath + "\\" + rptname);
                cryRpt.Load(rptpath + "\\" + rptname);


                cryRpt.SetParameterValue("@PCustID", ClientID);
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
}
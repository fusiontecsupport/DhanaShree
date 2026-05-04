using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System;
using System.Linq;
using System.Web.Mvc;
using CHITERP.Data;
using CHITERP.Models;
using ClosedXML.Excel;

namespace CHITERP.Controllers.Reports
{
    public class DayBookLedgerRptController : Controller
    {
        // GET: DayBookLedgerRpt

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


        public void DayBookLedgerXLRpt()
        {
            string ClientID = Request.Form.Get("FPersonID");
            string ProdType = Request.Form.Get("ProductType");
            string fromdt = Convert.ToDateTime(Request.Form.Get("from")).ToString("yyyy-MM-dd");
            string todt = Convert.ToDateTime(Request.Form.Get("to")).ToString("yyyy-MM-dd"); 
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
            string filename = "DayBookLedgerRpt_" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            filename = filename + ".xlsx";
            string query = "";


            query = "exec [CustomerAccountLedgerbyPcode_Daybook] @usrid ='" + cusrid + "' , @FromDate = '" + fromdt + "',";
            query = query + " @ToDate = '" + todt + "', @product=" + prdtid + ", @customer = " + clntid + ",";
            query = query + " @account = '', @collectionagent=" + colagtid + ", @routeid = " + routeid;

            DataTable dt0 = new DataTable();
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();

            String constring = ConfigurationManager.ConnectionStrings["IdentityCon"].ConnectionString;
            SqlConnection con = new SqlConnection(constring);

            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds, "DayBookLedger");
            //da.Fill(dt);
            con.Close();




            using (XLWorkbook wb = new XLWorkbook())
            {
                
                
                dt0 = ds.Tables[0];
                dt0.TableName = "FilterDetails";
                wb.Worksheets.Add(dt0);                
                dt1 = ds.Tables[2];
                dt1.TableName = "DatewiseSummary";
                wb.Worksheets.Add(dt1);                
                dt2 = ds.Tables[3];
                dt2.TableName = "CollectionAgentwiseSummary";
                wb.Worksheets.Add(dt2);
                dt = ds.Tables[1];
                dt.TableName = "DayBookLedgerDetailsReport";
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

    }
}
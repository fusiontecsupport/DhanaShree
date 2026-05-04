using CHITERP.Data;
using CHITERP.Models;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Controllers.Reports
{
    public class ChitBusinessReportController : Controller
    {
        // GET: BusinessReport
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


        public void ChitBusinessXLRpt()
        {
            string ProdType = Request.Form.Get("ProductType");
            string fromdt = Convert.ToDateTime(Request.Form.Get("from")).ToString("yyyy-MM-dd");
            string todt = Convert.ToDateTime(Request.Form.Get("to")).ToString("yyyy-MM-dd"); ;
            //string route = Request.Form.Get("RouteID");
            //string colagt = Request.Form.Get("CollectionAgent");

            //string cusrid = Convert.ToString(Session["CUSRID"]);
            //string crole = Convert.ToString(Session["Group"]);
            //if (crole == "SuperAdmin" || crole == "Admin")
            //{
            //    cusrid = "";
            //}


            int prdtid = 0;
            if (ProdType != "" && ProdType != "undefined")
                prdtid = Convert.ToInt32(ProdType);
            prdtid = 29;
            int compyid = Convert.ToInt32(Session["compyid"]);
            string filename = "BusinessRpt_"+DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            filename = filename + ".xlsx";
            string query = "";


            query = "exec [Chit_Business_Report] @product=" + prdtid + " , @FrDate = '" + fromdt + "',";
            query = query + " @ToDate = '" + todt + "'";


            DataTable dt0 = new DataTable();
            DataTable dt1 = new DataTable();

            String constring = ConfigurationManager.ConnectionStrings["IdentityCon"].ConnectionString;
            SqlConnection con = new SqlConnection(constring);

            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds, "ChitBusinessReport");
            //da.Fill(dt);
            con.Close();




            using (XLWorkbook wb = new XLWorkbook())
            {
                dt0 = ds.Tables[0];
                dt0.TableName = "FilterDetails";
                wb.Worksheets.Add(dt0);
                dt1 = ds.Tables[1];
                dt1.TableName = "ChitBusinessReport";
                wb.Worksheets.Add(dt1);


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

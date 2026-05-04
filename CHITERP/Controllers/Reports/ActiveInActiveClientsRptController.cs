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
    public class ActiveInActiveClientsRptController : Controller
    {
        // GET: ActiveInActiveClientsRpt
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
            if (Request.Form.Get("AcctStatus") != null && Request.Form.Get("AcctStatus") != "")
            {
                Session["AcctStatus"] = Request.Form.Get("AcctStatus");
            }
            else
                Session["AcctStatus"] = 1;
            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Product', ''").ToList();
            ViewBag.ProductType = new SelectList(mtqry.Where(x => x.Type == "Chit Fund" || x.Type == "Loan"), "TypeID", "Type", Convert.ToInt32(Session["ProductType"])).ToList();
            var amtcdqry4 = context.Database.SqlQuery<CompanyRoute>("select* from CompanyRoute (nolock)").ToList();
            ViewBag.RouteID = new SelectList(amtcdqry4, "RouteID", "RouteName", Convert.ToInt32(Session["RouteID"]));
            var empqry = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_Get_Employee_Dtl_Rpt @usrid='" + cusrid + "'").ToList();
            ViewBag.CollectionAgent = new SelectList(empqry, "DValue", "DText", Convert.ToInt32(Session["CollectionAgent"])).ToList();
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Account_Status").ToList();
            ViewBag.AcctStatus = new SelectList(mtqry, "TypeID", "Type", Convert.ToInt32(Session["AcctStatus"])).ToList();
            var amtcdqry3 = context.Database.SqlQuery<CompanyOffice>("Select * From CompanyOffice (nolock) ");// where officeid = " + Convert.ToInt32(Session["LOfficeID"])).ToList();
            ViewBag.LinkedOfficeID = new SelectList(amtcdqry3, "officeid", "OfficeName");
            return View();
        }


        public void ActiveInactiveClientsXLRpt()
        {
            
            string ProdType = Request.Form.Get("ProductType");
            string AcctStatus = Request.Form.Get("AcctStatus");
            string Routid = Request.Form.Get("RouteID");
            string officeid = Request.Form.Get("LinkedOfficeID");
            //string fromdt = Convert.ToDateTime(Request.Form.Get("from")).ToString("yyyy-MM-dd");
            //string todt = Convert.ToDateTime(Request.Form.Get("to")).ToString("yyyy-MM-dd"); ;
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

            int compyid = Convert.ToInt32(Session["compyid"]);
            string filename = "ActInactStsRpt_"+DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            filename = filename + ".xlsx";
            string query = "";
            string crole = Convert.ToString(Session["Group"]);
            string cusrid = Convert.ToString(Session["CUSRID"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                //cusrid = "";
            }
            int acsts = 0;
            if (AcctStatus != null)
                acsts = Convert.ToInt32(AcctStatus);

            int routeid = 0;
            if (Routid != null && Routid != "")
                routeid = Convert.ToInt32(Routid);
            int ofcid = 0;
            if (officeid != "" && officeid != "undefined")
                ofcid = Convert.ToInt32(officeid);

            query = "exec [Active_Clients_Summary] @product=" + prdtid + "";
            if (acsts == 1)
                query = "exec [Active_Clients_Summary] @product=" + prdtid + ", @usrid='" + cusrid + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@routeid=" + routeid + ",@officeid=" + ofcid;
            else
                query = "exec [InActive_Clients_Summary] @product=" + prdtid + ", @usrid='" + cusrid + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@routeid=" + routeid + ",@officeid=" + ofcid;


            DataTable dt0 = new DataTable();
            DataTable dt1 = new DataTable();

            String constring = ConfigurationManager.ConnectionStrings["IdentityCon"].ConnectionString;
            SqlConnection con = new SqlConnection(constring);

            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            if (acsts == 1)
                da.Fill(ds, "ActiveClientsAcSummary");
            else
                da.Fill(ds, "InActiveClientsAcSummary");
            //da.Fill(dt);
            con.Close();




            using (XLWorkbook wb = new XLWorkbook())
            {
                dt0 = ds.Tables[0];
                dt0.TableName = "FilterDetails";
                wb.Worksheets.Add(dt0);
                dt1 = ds.Tables[1];
                if (acsts == 1)
                    dt1.TableName = "ActiveClientsAcSummary";
                else
                    dt1.TableName = "InActiveClientsAcSummary";
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
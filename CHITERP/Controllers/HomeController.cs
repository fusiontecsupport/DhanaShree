using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CHITERP.Data;
using CHITERP.Models;


namespace CHITERP.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext context = new AppDbContext();
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            if (Request.Form.Get("from") != null)
            {
                Session["DBSDATE"] = Request.Form.Get("from");
                Session["DBEDATE"] = Request.Form.Get("to");
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<string> AddUser()
        {
            AppUser user;
            AppUserStore Store = new AppUserStore(new AppDbContext());
            AppUserManager userManager = new AppUserManager(Store);
            user = new AppUser
            {
                UserName = "TestUser",
                Email = "TestUser@test.com"
            };

            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return result.Errors.First();
            }
            return "User Added";
        }

        #region Get Dashboard Information   
        public JsonResult GetDashboardDtl(string id)
        {

            string crole = Convert.ToString(Session["Group"]);
            string cusr = Convert.ToString(Session["CUSRID"]);
            DateTime fdt, tdt;
            fdt = DateTime.Now.Date;
            tdt = DateTime.Now.Date;
            int cempid = 0;
            int lempid = 0;
            lempid = Convert.ToInt32(Session["EMPLID"]);
            
            var ids = id.Split('~');
            if (ids.Length > 0)
            {
                if (ids[0].ToString() != "")
                { cusr = Convert.ToString(ids[0]); }
                else { cusr = Convert.ToString(Session["CUSRID"]); }

                if (ids[1].ToString() != "")
                { cempid = Convert.ToInt32(ids[1]); }
                else { cempid = Convert.ToInt32(Session["EMPLID"]); }

                if (ids[2].ToString() != "")
                { fdt = Convert.ToDateTime(ids[2]); }
                else { fdt = Convert.ToDateTime(Session["DBSDATE"]); }

                if (ids[3].ToString() != "")
                { tdt = Convert.ToDateTime(ids[3]); }
                else { tdt = Convert.ToDateTime(Session["DBEDATE"]); }


            }




            int compyid = 0;
            compyid = Convert.ToInt32(Session["compyid"]);

            //if ((crole == "SuperAdmin" || crole == "Admin") && (lempid == cempid))
            //{
            //    cusr = "";
            //}
            //else
            //{
            //    var rsql = context.Database.SqlQuery<AspNetUser>("select * from aspnetusers (nolock) Where brnchid = '" + cempid + "'").ToList();
            //    if (rsql.Count > 0)
            //    {
            //        cusr = rsql[0].UserName.ToLower();
            //    }
            //    else
            //    {
            //        cusr = Session["CUSRID"].ToString();
            //    }
            //}

            int trc = 9999999;
            int frc = 999999;
            int srn = 1;
            int ern = 9999999;

            DateTime sdate = Convert.ToDateTime(Session["FSDATE"]).Date;
            DateTime edate = Convert.ToDateTime(Session["FEDATE"]).Date;
            int TaskStatusId = Convert.ToInt32(Session["TASKSTATUS"]);

            string squery = "exec [pr_db_monthwise_summary] @usrid='" + cusr+"',";
            squery += "@empid=" + cempid + ",";
            squery += "@fdt='" + fdt.ToString("yyyy-MM-dd") + "',";
            squery += "@tdt='" + tdt.ToString("yyyy-MM-dd") + "',@prod=0";


            var result = context.Database.SqlQuery<pr_db_monthwise_summary_Result>(squery).ToList();

           
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

        public string DBTodayCollectionDtl(string id)
        {
            string crole = Convert.ToString(Session["Group"]);
            string cusr = Convert.ToString(Session["CUSRID"]);
            DateTime fdt, tdt;
            fdt = DateTime.Now.Date;
            tdt = DateTime.Now.Date;
            int cempid = 0;
            int lempid = 0;
            lempid = Convert.ToInt32(Session["EMPLID"]);

            var ids = id.Split('~');
            if (ids.Length > 0)
            {
                if (ids[0].ToString() != "")
                { cusr = Convert.ToString(ids[0]); }
                else { cusr = Convert.ToString(Session["CUSRID"]); }

                if (ids[1].ToString() != "")
                { cempid = Convert.ToInt32(ids[1]); }
                else { cempid = Convert.ToInt32(Session["EMPLID"]); }

                if (ids[2].ToString() != "")
                { fdt = Convert.ToDateTime(ids[2]); }
                else { fdt = Convert.ToDateTime(Session["DBSDATE"]); }

                if (ids[3].ToString() != "")
                { tdt = Convert.ToDateTime(ids[3]); }
                else { tdt = Convert.ToDateTime(Session["DBEDATE"]); }


            }




            int compyid = 0;
            compyid = Convert.ToInt32(Session["compyid"]);
            try
            {

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IdentityCon"].ToString()))
                {

                    conn.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataAdapter da = new SqlDataAdapter("[dbo].[pr_db_today_Summary]", conn);

                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.CommandTimeout = 0;
                    da.SelectCommand.Parameters.Add(new SqlParameter("@usrid", SqlDbType.VarChar));
                    da.SelectCommand.Parameters.Add(new SqlParameter("@empid", SqlDbType.Int));
                    da.SelectCommand.Parameters.Add(new SqlParameter("@fdt", SqlDbType.DateTime));
                    da.SelectCommand.Parameters.Add(new SqlParameter("@tdt", SqlDbType.DateTime));
                    da.SelectCommand.Parameters.Add(new SqlParameter("@prod", SqlDbType.Int));

                    da.SelectCommand.Parameters["@usrid"].Value = cusr.Trim();
                    da.SelectCommand.Parameters["@empid"].Value = cempid;
                    da.SelectCommand.Parameters["@fdt"].Value = fdt.ToString("yyyy-MM-dd");
                    da.SelectCommand.Parameters["@tdt"].Value = tdt.ToString("yyyy-MM-dd");
                    da.SelectCommand.Parameters["@prod"].Value = 0;

                    DataSet ds = new DataSet();
                    StringBuilder html0 = new StringBuilder();
                    StringBuilder html1 = new StringBuilder();
                    StringBuilder html2 = new StringBuilder();
                    StringBuilder html3 = new StringBuilder();
                    StringBuilder sb = new StringBuilder();
                    da.Fill(ds, "DBDtl");

                    ////Building the Header row.
                    int tblcnt = 0;
                    while (tblcnt<ds.Tables.Count)
                    {
                        if (ds.Tables[tblcnt].Rows.Count > 0)
                        {
                            //Table start.
                            html1.Append("<br/><br/><table class='table table-striped table-bordered datatable'>");

                            //Building the Header row.
                            html1.Append("<tr>");

                            int tblcolcnt = 0;
                            foreach (DataColumn column in ds.Tables[tblcnt].Columns)
                            {
                                if(tblcolcnt>0)
                                { 
                                html1.Append("<th>");
                                html1.Append(column.ColumnName);
                                html1.Append("</th>");
                                }
                                else
                                {
                                    html1.Append("<th colspan='" + (ds.Tables[tblcnt].Columns.Count-1).ToString() + "' class='bg-success'>");
                                    html1.Append(ds.Tables[tblcnt].Rows[0][column.ColumnName].ToString());                                     html1.Append("</th></tr><tr>");
                                }
                                tblcolcnt++;
                            }
                            html1.Append("</tr>");

                            //Building the Data rows.
                            foreach (DataRow row in ds.Tables[tblcnt].Rows)
                            {
                                html1.Append("<tr>");
                                tblcolcnt = 0;
                                foreach (DataColumn column in ds.Tables[tblcnt].Columns)
                                {
                                    if (tblcolcnt > 0)
                                    {
                                        html1.Append("<td>");
                                        //if (column.ColumnName == "Totals")
                                        //{
                                        //    html1.Append(Convert.ToDecimal(row[column.ColumnName]).ToString("#,##,###0.00"));
                                        //}
                                        //else
                                        //{
                                        html1.Append(row[column.ColumnName]);
                                        //}
                                    
                                    html1.Append("</td>");
                                    }
                                    tblcolcnt++;
                                }
                                html1.Append("</tr>");
                            }

                            //Table end.
                            html1.Append("</table>");
                            tblcnt++;
                            //Append the HTML string to Placeholder.

                        }

                    }



                    return html1.ToString();
                }
            }
            catch (Exception ex)
            {
                string status = "error";
                return status;
            }
            return "";
        }
    }
}
using CHITERP.Helper;
using CHITERP.Data;
using CHITERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data.SqlClient;
using System.Configuration;

namespace CHITERP.Controllers.Reports
{

    public class FinLedgerStatementRptController : Controller
    {
        // GET: FinLedgerStatementRpt
        AppDbContext context = new AppDbContext();
        FinFusionEntities db = new FinFusionEntities();

        [Authorize(Roles = "FinLedgerStatementRptIndex")]
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

            if (Request.Form.Get("LinkedOfficeID") != null && Request.Form.Get("LinkedOfficeID") != "")
            {
                Session["LinkedOfficeID"] = Request.Form.Get("LinkedOfficeID");
            }
            else
                Session["LinkedOfficeID"] = "0";

            if (Request.Form.Get("RptType") != null && Request.Form.Get("RptType") != "")
            {
                Session["RptType"] = Request.Form.Get("RptType");
            }
            else
                Session["RptType"] = "0";
            int linkofcid = 0;

            linkofcid = Convert.ToInt32(Session["LinkedOfficeID"]);
            //ViewBag.CATEID = new SelectList(context.clientmasters.OrderBy(x => x.CLIENTNAME), "CLIENTID", "CLIENTNAME");
            string cusrid = Convert.ToString(Session["CUSRID"]);
            string crole = Convert.ToString(Session["Group"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                cusrid = "";
            }
            var result = context.Database.SqlQuery<pr_Roles_Wise_Customer_Name_Assgn_Result>("EXEC pr_Roles_Wise_Customer_Name_Assgn @CUSRID = '" + cusrid + "', @PEmplId =" + Convert.ToInt32(Session["EMPLID"]) + "").ToList();
            ViewBag.CATEID = new SelectList(result, "CATEID", "CLIENTNAME").ToList();
            ViewBag.MTRLGID = new SelectList("");// (context.itemgroupmasters.OrderBy(x => x.ITEMGDESC), "ITEMGID", "ITEMGDESC");
            ViewBag.MTRLID = new SelectList("");// context.ItemMaster.OrderBy(x => x.ITEMDESC), "ITEMID", "ITEMDESC");
            var amtcdqry3 = context.Database.SqlQuery<CompanyOffice>("Select * From CompanyOffice (nolock)").ToList();
            ViewBag.LinkedOfficeID = new SelectList(amtcdqry3, "officeid", "OfficeName", linkofcid);
            var rpttypqry = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_Get_Fin_Rpt_Type").ToList();
            ViewBag.RptType = new SelectList(rpttypqry, "DValue", "DText", Convert.ToInt32(Session["RptType"])).ToList();

            return View();
        }

        public JsonResult GetCustomerList(string ids)
        {
            var param = ids.Split('~');
            var roleid = 0;// Convert.ToInt32(param[0]);
            var emplid = 0;// Convert.ToInt32(param[1]);
            var regnid = 0;// Convert.ToInt32(param[2]);

            //if (param[0] != "") { roleid = Convert.ToInt32(param[0]); } else { roleid = 0; }
            //if (param[1] != "") { emplid = Convert.ToInt32(param[1]); } else { emplid = 0; }
            //if (param[2] != "") { regnid = Convert.ToInt32(param[2]); } else { regnid = 0; }
            string cusrid = Convert.ToString(Session["CUSRID"]);
            string crole = Convert.ToString(Session["Group"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                cusrid = "";
            }
            var result = context.Database.SqlQuery<pr_Roles_Wise_Customer_Name_Assgn_Result>("EXEC pr_Roles_Wise_Customer_Name_Assgn @CUSRID = '" + cusrid + "', @PEmplId =" + Convert.ToInt32(Session["EMPLID"]) + "").ToList();

            //var result = (from r in context.clientmasters
            //              select new { r.CATEID, r.CATENAME }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }//...end

        public void showrpt()
        {


            var rpttype = Convert.ToInt32(Request.Form.Get("RptType"));
            var ofcid = Request.Form.Get("LinkedOfficeID");

            var afromDate = Request.Form.Get("from").Split('-');
            var fromDate = afromDate[2] + "-" + afromDate[1] + "-" + afromDate[0];
            string cusrid = Convert.ToString(Session["CUSRID"]);
            string crole = Convert.ToString(Session["Group"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                cusrid = "";
            }
            var atoDate = Request.Form.Get("to").Split('-');
            var toDate = atoDate[2] + "-" + atoDate[1] + "-" + atoDate[0];

            var strPath = ConfigurationManager.AppSettings["Reporturl"];

            var rptURL = ""; var rptquery = ""; var strHead = "";

            var SDATE = afromDate[0] + "-" + afromDate[1] + "-" + afromDate[2];
            var EDATE = atoDate[0] + "-" + atoDate[1] + "-" + atoDate[2];


            var date1 = Convert.ToDateTime(SDATE).ToString("dd-MMM-yyyy");
            var date2 = Convert.ToDateTime(EDATE).ToString("dd-MMM-yyyy");
            var gentype = Convert.ToInt32(Request.Form.Get("gentype"));
            var sql = "INSERT INTO TMPRPT_IDS(KUSRID, OPTNSTR, RPTID)";
            switch (rpttype)
            {
                case 0:
                    context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE OPTNSTR='DBK' AND KUSRID='" + Session["CUSRID"].ToString() + "'");

                    switch (gentype)
                    {
                        case 0:
                            strHead = "Day Book Statement From " + Convert.ToDateTime(SDATE).ToString("dd-MMM-yyyy") + " Till " + Convert.ToDateTime(EDATE).ToString("dd-MMM-yyyy") + "";
                            if (Request.Form.Get("LDGRID") == "" || Request.Form.Get("LDGRID") == null)
                            {
                                sql = sql + " SELECT  '" + Session["CUSRID"].ToString() + "', 'DBK' , ACHEADID AS CATEID FROM ACCOUNTHEADMASTER (NOLOCK) ";
                            }
                            else
                            {
                                sql = sql + " SELECT  '" + Session["CUSRID"].ToString() + "', 'DBK' , ACHEADID AS CATEID FROM ACCOUNTHEADMASTER (NOLOCK) WHERE ACHEADID IN (" + Request.Form.Get("LDGRID") + ")";
                            }
                            context.Database.ExecuteSqlCommand(sql);

                            //context.Database.ExecuteSqlCommand("pr_Customer_SOA_Consolidated_Rpt_Assgn @PSDate = '" + date1 + "', @PEDate = '" + date2 + "', @PKUsrID  = '" + Session["CUSRID"].ToString() + "'");

                            //rptURL = strPath + "\\LedgerStatement.Rpt";
                            rptURL = strPath + "\\DayBookRpt.Rpt";
                            //rptquery = "{VW_SOA_RPT.KUSRID}='" + Session["CUSRID"].ToString() + "'";
                            break;
                        case 1:

                        case 2:
                            break;
                    }
                    break;
                case 1:
                    context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE OPTNSTR='LDGR' AND KUSRID='" + Session["CUSRID"].ToString() + "'");

                    switch (gentype)
                    {
                        case 0:
                            strHead = "Ledger Statement From " + Convert.ToDateTime(SDATE).ToString("dd-MMM-yyyy") + " Till " + Convert.ToDateTime(EDATE).ToString("dd-MMM-yyyy") + "";
                            if (Request.Form.Get("LDGRID") == "" || Request.Form.Get("LDGRID") == null)
                            {
                                sql = sql + " SELECT  '" + Session["CUSRID"].ToString() + "', 'LDGR' , ACHEADID AS CATEID FROM ACCOUNTHEADMASTER (NOLOCK) ";
                            }
                            else
                            {
                                sql = sql + " SELECT  '" + Session["CUSRID"].ToString() + "', 'LDGR' , ACHEADID AS CATEID FROM ACCOUNTHEADMASTER (NOLOCK) WHERE ACHEADID IN (" + Request.Form.Get("LDGRID") + ")";
                            }
                            context.Database.ExecuteSqlCommand(sql);

                            //context.Database.ExecuteSqlCommand("pr_Customer_SOA_Consolidated_Rpt_Assgn @PSDate = '" + date1 + "', @PEDate = '" + date2 + "', @PKUsrID  = '" + Session["CUSRID"].ToString() + "'");

                            //rptURL = strPath + "\\LedgerStatement.Rpt";
                            rptURL = strPath + "\\DayBookLedgerRpt.Rpt";
                            //rptquery = "{VW_SOA_RPT.KUSRID}='" + Session["CUSRID"].ToString() + "'";
                            break;
                        case 1:

                        case 2:
                            break;
                    }
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:

                    break;
                default:
                    //rptURL = "E:\\SIMSReports\\GMR_Register_SubCategory_Wise_Detail_Rpt.rpt";
                    break;
            }

            //Response.Write(sql);

            ReportDocument cryRpt = new ReportDocument();

            TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
            TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
            ConnectionInfo crConnectionInfo = new ConnectionInfo();
            Tables CrTables;

            cryRpt.Load(rptURL);

            //if (rpttype == 1)
            {
                ParameterField paramField = new ParameterField();
                ParameterFields parameterField = new ParameterFields();
                ParameterDiscreteValue paramDiscreteValue = new ParameterDiscreteValue();

                paramField.Name = "PSDate";
                paramDiscreteValue.Value = fromDate;
                paramField.CurrentValues.Add(paramDiscreteValue);
                parameterField.Add(paramField);


                paramField.Name = "PEDate";
                paramDiscreteValue.Value = toDate;
                paramField.CurrentValues.Add(paramDiscreteValue);
                parameterField.Add(paramField);


                paramField.Name = "PKUsrID";
                paramDiscreteValue.Value = Session["CUSRID"];
                paramField.CurrentValues.Add(paramDiscreteValue);
                parameterField.Add(paramField);



                paramField.Name = "LinkedOffice";
                paramDiscreteValue.Value = ofcid;
                paramField.CurrentValues.Add(paramDiscreteValue);
                parameterField.Add(paramField);

                paramField.Name = "Cusrid";
                paramDiscreteValue.Value = Session["CUSRID"];
                paramField.CurrentValues.Add(paramDiscreteValue);
                parameterField.Add(paramField);
                //cryRpt.Refresh();
                cryRpt.SetParameterValue("@PSDate", fromDate);
                cryRpt.SetParameterValue("@PEDate", toDate);
                cryRpt.SetParameterValue("@PKUsrID", Session["CUSRID"]);
                cryRpt.SetParameterValue("@LinkedOffice", ofcid);
                cryRpt.SetParameterValue("@Cusrid", cusrid);

            }

            string paramName = "@FHEAD";

            for (int i = 0; i < cryRpt.DataDefinition.FormulaFields.Count; i++)
                if (cryRpt.DataDefinition.FormulaFields[i].FormulaName == "{" + paramName + "}")
                    cryRpt.DataDefinition.FormulaFields[i].Text = "'" + strHead + "'";
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
            if ((Request.Form.Get("prnttype") == "1"))
            {
                cryRpt.ExportToHttpResponse(ExportFormatType.Excel, System.Web.HttpContext.Current.Response, false, "");
            }
            if ((Request.Form.Get("prnttype") == "2"))
            {
                cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
            }

            cryRpt.Close();
            cryRpt.Dispose();

        }

    }
}

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
    public class ChitGroupManagementController : Controller
    {
        // GET: ChitGroupManagement

        #region context 
        AppDbContext context = new AppDbContext();
        #endregion


        #region Index
        //[Authorize(Roles = "ChitGroupManagementIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            return View();
        }
        #endregion

        #region Edit
        //[Authorize(Roles = "ChitGroupManagementEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/ChitGroupManagement/Form/" + id);

                //Response.Redirect("/ChitGroupManagement/Form/?id=" + id);
            }
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

                    var data = e.pr_Search_Master_ChitGroup(param.sSearch,
                                                    Convert.ToInt32(Request["iSortCol_0"]),
                                                    Request["sSortDir_0"],
                                                    param.iDisplayStart,
                                                    param.iDisplayStart + param.iDisplayLength,
                                                    totalRowsCount,
                                                    filteredRowsCount, Convert.ToString(cusrid));
                    var aaData = data.Select(d => new
                    {
                        
                        ChitSchemeCode = d.ChitSchemeCode,
                        ChitSchemeName = d.ChitSchemeName,
                        ChitGroupCode = d.ChitGroupCode,
                        ChitGroupName = d.ChitGroupName,
                        ChitValue = d.ChitValue.ToString(),
                        DurationTypeDesc = d.DurationTypeDesc,
                        MaxGrpMembers = d.MaxGrpMembers.ToString(),
                        CommencementDate = d.CommencementDate.ToString(),
                        StatusTypeDesc = d.StatusTypeDesc,
                        ChitGroupID = d.ChitGroupID.ToString()
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

        #region Form
        //[Authorize(Roles = "ChitGroupManagementCreate")]
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            ChitGroup tab = new ChitGroup();
            ChitGroupRegistered tab1 = new ChitGroupRegistered();
            
            ChitGroupList cgl = new ChitGroupList();

            int ChitGroupID = 0;

            if (id == "" || id == null || id == "0")
            { ChitGroupID = 0; }
            else { 
                ChitGroupID = Convert.ToInt32(id);
                ViewBag.ChitGroupID = id;
            }

            var amtcdqry = context.Database.SqlQuery<ChitScheme>("Select * From ChitScheme (nolock)").ToList();
            ViewBag.ChitSchemeID = new SelectList(amtcdqry, "ChitSchemeID", "ChitSchemeName");
            var coffqry = context.Database.SqlQuery<CompanyOffice>("Select c.* From CompanyOffice c(nolock) join mastertype m(nolock) on c.StatusTypeID = m.typeid and m.type='ACTIVE' ").ToList();
            ViewBag.OfficeID = new SelectList(coffqry, "OfficeID", "OfficeName");



            

            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Chit Group', 'Status'").ToList();
            ViewBag.StatusTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();

            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_ChitGroup_Types").ToList();
            ViewBag.GroupTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();


            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Registrar', ''").ToList();
            ViewBag.RegistrarOfficeTypeId = new SelectList(mtqry, "TypeID", "Type").ToList();

            if (ChitGroupID > 0)
            {
                tab = context.chitgroups.Find(ChitGroupID);
                ViewBag.ChitGroupID = tab.ChitGroupID;
                ViewBag.ChitGroupName = tab.ChitGroupName;
                ViewBag.OfficeID = new SelectList(coffqry.Where(x => x.OfficeID == tab.OfficeID), "OfficeID", "OfficeName", tab.OfficeID);
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Chit Group', 'Status'").ToList();
                ViewBag.StatusTypeID = new SelectList(mtqry, "TypeID", "Type",tab.StatusTypeID).ToList(); //.Where(x => x.TypeID == tab.StatusTypeID)
                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_ChitGroup_Types").ToList();
                ChitGroup cg = new ChitGroup();                
                cg = context.chitgroups.Find( tab.ChitGroupID);
                ViewBag.GroupTypeID = new SelectList(mtqry, "TypeID", "Type", cg.ChitGroupTypeID).ToList();
                amtcdqry = context.Database.SqlQuery<ChitScheme>("Select * From ChitScheme (nolock)").ToList();
                ViewBag.ChitSchemeID = new SelectList(amtcdqry.Where(x => x.ChitSchemeID == tab.ChitSchemeID), "ChitSchemeID", "ChitSchemeName", tab.ChitSchemeID);

                mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Registrar', ''").ToList();
                ViewBag.RegistrarOfficeTypeId = new SelectList(mtqry.Where(x => x.TypeID == tab1.RegistrarOfficeTypeId || tab1.RegistrarOfficeTypeId==null), "TypeID", "Type").ToList();
                cgl.chitgroup = context.chitgroups.Where(x => x.ChitGroupID == ChitGroupID).ToList();
                cgl.chitgroupregistered = context.chitgroupregistereds.Where(x => x.ChitGroupId == ChitGroupID).ToList();                
            }
            else
            {
                cgl.chitgroup = context.chitgroups.Where(x => x.ChitGroupID == 0).ToList();
                cgl.chitgroupregistered = context.chitgroupregistereds.Where(x => x.ChitGroupId == 0).ToList();                
            }


            return View(cgl);
        }
        #endregion

        #region Get Scheme Details
        public JsonResult GetChitSchemeDtl(string id)
        {

            int ID = 0;
            if (id != "null" && id !="") { ID = Convert.ToInt32(id); }

            var result = context.Database.SqlQuery<ChitScheme>("Select *From  ChitScheme(nolock) Where chitschemeid = " + ID + " ").ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Group Subscription Details
        public string GetChitGroupSubscriberDtl(string id)
        {

            int ID = 0;
            if (id != "null" && id != "") { ID = Convert.ToInt32(id); }
            string html = "";
            var result = context.Database.SqlQuery<pr_Get_Chit_Group_Subscribers_Dtl_Result>("Exec pr_Get_Chit_Group_Subscribers_Dtl " + ID + " ").ToList();

            html = html + "<table class='table table-striped table-bordered bootstrap-datatable' id='CHITSUBSCRIBERDETAIL'>";
            html = html + "<thead><tr><th>Ticket#</th><th style='text-align:center'>Name</th><th style='text-align:center'>Status</th></tr> </thead>";
            html = html + "<tbody class='CHITSUBSCRIBERDETAIL_TBODY' id='CHITSUBSCRIBERDETAIL_TBODY'>";
            if (result.Count>0)
            {
                

                foreach (var res in result)
                {

                    var tktnum = res.TktNum.ToString();
                    var name = res.Name.ToString();
                    var sts = res.Sts.ToString();

                    html = html + "<tr><td><input type='text' class='form-control tktnum' id='tktnum' name='tktnum' readonly value='" + tktnum + "'></td>";
                    html = html + "<td><input type='text' class='form-control name' id='name' name='name' readonly value='" + name + "'></td>";
                    html = html + "<td><input type='text' class='form-control sts' id='sts' name='sts' readonly value='" + sts + "'></td>";

                    html = html + "</tr>";
                }
                
            }
            else
            {
                html = html + "<tr><td colspan=3><h5>No Subscribers</h5></td></tr>";
            }
            html = html + "</tbody></table>";
            return html;
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

                            ChitGroup master = new ChitGroup();

                            Int32 ChitGroupID = 0;

                            string chitgroupid = Convert.ToString(formcoll["ChitGroupID"]);
                            string CommencementDate = Convert.ToString(formcoll["CommencementDate"]);
                            
                            if (CommencementDate != null && CommencementDate != "")
                                CommencementDate = Convert.ToString(formcoll["CommencementDate"]);
                            string CommencementTime = Convert.ToString(formcoll["CommencementTime"]);
                            if (CommencementTime != null && CommencementTime != "")
                                CommencementTime = Convert.ToString(formcoll["CommencementTime"]);
                            string FirstInstallmentDate = Convert.ToString(formcoll["FirstInstallmentDate"]);
                            if (FirstInstallmentDate != null && FirstInstallmentDate != "")
                                FirstInstallmentDate = Convert.ToString(formcoll["FirstInstallmentDate"]);
                            string TerminationDate = Convert.ToString(formcoll["TerminationDate"]);
                            if (TerminationDate != null && TerminationDate != "")
                                TerminationDate = Convert.ToString(formcoll["TerminationDate"]);

                            if (chitgroupid == "" || chitgroupid == null)
                            {
                                ChitGroupID = 0;
                            }
                            else
                            {
                                ChitGroupID = Convert.ToInt32(chitgroupid);
                            }

                            if (ChitGroupID != 0)
                            {
                                master = context.chitgroups.Find(ChitGroupID);
                            }
                            master.ChitGroupID = ChitGroupID;
                            master.ChitSchemeID = Convert.ToInt32(formcoll["ChitSchemeID"]);
                            master.OfficeID = Convert.ToInt32(formcoll["OfficeID"]);
                            master.ChitGroupCode = Convert.ToString(formcoll["ChitGroupCode"]);
                            master.ChitGroupName = Convert.ToString(formcoll["ChitGroupName"]);
                            
                            string cdatetime = "";
                            master.CommencementDate = null;
                            cdatetime = CommencementDate;
                            if(CommencementTime!=null && CommencementTime!="")
                                cdatetime = CommencementDate + " " + CommencementTime;
                            if (cdatetime!="" && cdatetime!= null && CommencementDate != null && CommencementTime != null)
                            master.CommencementDate = Convert.ToDateTime(cdatetime);
                            master.FirstInstallmentDate = null;
                            if (FirstInstallmentDate!="" && FirstInstallmentDate!= null)
                            master.FirstInstallmentDate = Convert.ToDateTime(FirstInstallmentDate);
                            master.TerminationDate = null;
                            if (TerminationDate!="" && TerminationDate!= null)
                            master.TerminationDate= Convert.ToDateTime(TerminationDate);
                            
                            master.MaxGrpMembers = Convert.ToInt32(formcoll["MaxGrpMembers"]);

                            
                            master.ChitGroupTypeID = Convert.ToInt16(formcoll["GroupTypeID"]);
                            master.StatusTypeID = Convert.ToInt16(formcoll["StatusTypeID"]);


                            if (ChitGroupID == 0)
                            {
                                string squery = "SELECT *FROM ChitGroup (nolock) WHERE ChitGroupCode ='" + master.ChitGroupCode + "' or ChitGroupName ='" + master.ChitGroupName + "'";
                                var query = context.Database.SqlQuery<ChitGroup>(squery).ToList();

                                if (query.Count != 0)
                                {
                                    status = "Existing";
                                }
                                else
                                {
                                    //master.CreatedBy = Session["CUSRID"].ToString();
                                    //master.CreatedDt = DateTime.Now;

                                    context.chitgroups.Add(master);
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
                            ChitGroupID = master.ChitGroupID;

                            

                            ViewBag.Message = "Form submitted.";
                            // Response.Redirect("Index");
                            //return RedirectToAction("Index", "ChitGroupManagement");
                            //Response.Write("Update");
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            var ermsg = ex.Message;
                            //Response.Write(ex.Message.ToString());
                            trans1.Rollback();
                            //Response.Redirect("/Error/AccessDenied");
                            //return RedirectToAction("Index", "ChitGroupManagement");
                            status = "Error";
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

        }
        #endregion

        #region Save Registration Parameters Data
        [HttpPost]
        public ActionResult SaveRPData(FormCollection formcoll)
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

                            ChitGroup master = new ChitGroup();
                            ChitGroupRegistered tab = new ChitGroupRegistered();

                            Int32 ChitGroupID = 0;

                            string chitgroupid = Convert.ToString(formcoll["rChitGroupID"]);
                            if (chitgroupid == "" || chitgroupid == null)
                            {
                                ChitGroupID = 0;
                            }
                            else
                            {
                                ChitGroupID = Convert.ToInt32(chitgroupid);
                            }

                            if (ChitGroupID != 0)
                            {
                                master = context.chitgroups.Find(ChitGroupID);
                            }

                            string ChitGroupRegisteredId = formcoll["ChitGroupRegisteredId"];
                            string PriorSanctionNo = formcoll["PriorSanctionNo"];
                            string ChitRegistrationNo = formcoll["ChitRegistrationNo"];
                            string RegistrarOfficeTypeId = formcoll["chitgroupregistered[0].RegistrarOfficeTypeId"];
                            if(RegistrarOfficeTypeId==null)
                                RegistrarOfficeTypeId = formcoll["RegistrarOfficeTypeId"];
                            string TotalTicket = formcoll["TotalTicket"];
                            string ChitAgreementNo = formcoll["ChitAgreementNo"];
                            
                            
                            if (master.ChitGroupID > 0)
                            {
                                
                                ChitGroupRegistered chitGroupRegd = new ChitGroupRegistered();
                                if (Convert.ToInt32(ChitGroupRegisteredId) > 0)
                                    chitGroupRegd = context.chitgroupregistereds.Find(Convert.ToInt32(ChitGroupRegisteredId));
                                chitGroupRegd.PriorSanctionNo = Convert.ToString(PriorSanctionNo);
                                chitGroupRegd.ChitGroupId = Convert.ToInt32(master.ChitGroupID);

                                
                                if (TotalTicket == "" || TotalTicket == null)
                                    chitGroupRegd.TotalTicket = 0;
                                else
                                    chitGroupRegd.TotalTicket = Convert.ToInt32(TotalTicket);

                                if (RegistrarOfficeTypeId == "" || RegistrarOfficeTypeId == null)
                                    chitGroupRegd.RegistrarOfficeTypeId = null;
                                else
                                    chitGroupRegd.RegistrarOfficeTypeId = Convert.ToInt32(RegistrarOfficeTypeId);
                                chitGroupRegd.ChitAgreementNo = Convert.ToString(ChitAgreementNo);
                                chitGroupRegd.ChitRegistrationNo = Convert.ToString(ChitRegistrationNo);
                                
                                if (Convert.ToInt32(ChitGroupRegisteredId) == 0)
                                {
                                    context.chitgroupregistereds.Add(chitGroupRegd);
                                }
                                else
                                {
                                    context.Entry(chitGroupRegd).State = System.Data.Entity.EntityState.Modified;
                                }
                                
                                context.SaveChanges();
                                status = "Success";
                                
                                trans1.Commit();
                                return Json(status, JsonRequestBehavior.AllowGet);
                            }
                            
                            ViewBag.Message = "Form submitted.";
                            // Response.Redirect("Index");
                            //return RedirectToAction("Index", "ChitGroupManagement");
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
        //[Authorize(Roles = "ChitGroupManagementDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check(fld, id, "ChitGroup");            
            if (temp.Equals("PROCEED"))
            {
                ChitGroup chitgroup = context.chitgroups.Find(Convert.ToInt32(id));
                context.chitgroups.Remove(chitgroup);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion

    }
}
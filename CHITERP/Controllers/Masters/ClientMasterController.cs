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
using DocumentFormat.OpenXml.Office2010.Excel;

namespace CHITERP.Controllers.Masters
{
    public class ClientMasterController : Controller
    {
        // GET: ClientMaster

        #region contextdeclaration
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        //[Authorize(Roles = "ClientMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            else { return View(); }
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

                    var data = e.pr_Search_Master_ClientMaster(param.sSearch,
                                                    Convert.ToInt32(Request["iSortCol_0"]),
                                                    Request["sSortDir_0"],
                                                    param.iDisplayStart,
                                                    param.iDisplayStart + param.iDisplayLength,
                                                    totalRowsCount,
                                                    filteredRowsCount, Convert.ToString(cusrid));
                    var aaData = data.Select(d => new
                    {
                        CLIENTNAME = d.CLIENTNAME,
                        CLIENTTYPEDESC = d.CLIENTTYPEDESC,
                        CLIENTMOBILENO1 = d.CLIENTMOBILENO1,
                        CLIENTEMAILID = d.CLIENTEMAILID,
                        CLIENTCONTACTPERSON = d.CLIENTCONTACTPERSON,
                        DISPSTATUS = d.DISPSTATUS.ToString(),
                        CLIENTID = d.CLIENTID.ToString()
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
        //[Authorize(Roles = "ClientMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0 || Session["CUSRID"].ToString() == "") { return RedirectToAction("Login", "Account"); }
            else
            {
                
                ClientMaster tab = new ClientMaster();

                var ctqry = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_Get_Client_Type 0").ToList();
                ViewBag.CLIENTTID = new SelectList(ctqry, "DValue", "DText").ToList();
                ctqry = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_get_attachment_type").ToList();
                ViewBag.AttachmentType = new SelectList(ctqry, "DValue", "DText").ToList();
                var clist1 = new SelectList(context.clientmasters.Where(d => d.DISPSTATUS == 0), "CLIENTID", "CLIENTNAME").ToList();
                
                ViewBag.REFCLIENTID = clist1;
                ViewBag.CLIENTSTATEID = new SelectList(context.statemasters.Where(d => d.DISPSTATUS == 0), "STATEID", "STATEDESC");
                //ViewBag.CLIENTCOUNTRYID = new SelectList(context.crmcountrymasters.Where(d => d.DISPSTATUS == 0), "COUNTRYID", "COUNTRYDESC");

                List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
                SelectListItem selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
                selectedDISPSTATUS.Add(selectedItem);
                selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
                selectedDISPSTATUS.Add(selectedItem);
                ViewBag.DISPSTATUS = selectedDISPSTATUS;
                ViewBag.localityid = new SelectList(context.contactaddresslocalitys.Where(x => x.LocalityID > 0).OrderBy(x => x.LocalityName), "LocalityID", "LocalityName");
                var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Nominee Relashionship', ''").ToList();
                ViewBag.Nominee_RelationshipId = new SelectList(mtqry, "TypeID", "Type").ToList();
                ViewBag.hiddentrandid = "0";
                ViewBag.ClientFiles = null;
                if (id != 0 && id != -1)  // IMP
                {
                    ViewBag.hiddentrandid = id;
                    tab = context.clientmasters.Find(id);
                    if (!System.IO.Directory.Exists(Server.MapPath("~/ClientFiles/" + id)))
                        Directory.CreateDirectory(Server.MapPath("~/ClientFiles/" + id));

                    var dir = new DirectoryInfo(Server.MapPath("~/ClientFiles/" + id));

                    var files = dir.EnumerateFiles().Select(f => f.Name);
                    ViewBag.ClientFiles = files;

                    ctqry = context.Database.SqlQuery<pr_Get_Client_Type_Result>("exec pr_Get_Client_Type " +tab.CLIENTTID).ToList();
                    ViewBag.CLIENTTID = new SelectList(ctqry, "DValue", "DText",tab.CLIENTTID).ToList(); //.Where(x=>x.DValue== tab.CLIENTTID)
                    ViewBag.localityid = new SelectList(context.contactaddresslocalitys.Where(x => x.LocalityID > 0).OrderBy(x => x.LocalityName), "LocalityID", "LocalityName",tab.localityid);
                    mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Nominee Relashionship', ''").ToList();
                    ViewBag.Nominee_RelationshipId = new SelectList(mtqry, "TypeID", "Type",tab.Nominee_RelationshipId).ToList();

                    ViewBag.CLIENTSTATEID = new SelectList(context.statemasters.Where(d => d.DISPSTATUS == 0), "STATEID", "STATEDESC", tab.CLIENTSTATEID);
                    var clista = new SelectList(context.clientmasters.Where(d => d.DISPSTATUS == 0), "CLIENTID", "CLIENTNAME",tab.REFCLIENTID).ToList();
                    
                    ViewBag.REFCLIENTID = clista;
                    //ViewBag.CLIENTCOUNTRYID = new SelectList(context.crmcountrymasters.Where(d => d.DISPSTATUS == 0), "COUNTRYID", "COUNTRYDESC", tab.CLIENTCOUNTRYID);

                    List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                    if (Convert.ToInt32(tab.DISPSTATUS) == 0)
                    {
                        SelectListItem selectedItem31 = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        selectedItem31 = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                    }
                    else
                    {
                        SelectListItem selectedItem31 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        selectedItem31 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                    }
                }

                return View(tab);
            }
        }
        #endregion

        #region Save Delete Client Attachment Files
        public void FilesDelete(String id)
        {
            var param = id.Split(';');
            System.IO.File.Delete(Server.MapPath("~/ClientFiles/" + param[0] + "/" + param[1]));
            Response.Redirect(@Url.Action("Form", "ClientMaster") + "/?id=" + param[0]);
        }
        #endregion
        #region SaveData
        [HttpPost]
        public ActionResult SaveData(FormCollection formcoll, HttpPostedFileBase logofilesupload)
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
                            ClientMaster master = new ClientMaster();


                            Int32 CLIENTID = 0;

                            string clientID = Convert.ToString(formcoll["CLIENTID"]);
                            if (clientID == "" || clientID == null)
                            {
                                CLIENTID = 0;
                            }
                            else
                            {
                                CLIENTID = Convert.ToInt32(clientID);
                            }

                            if (CLIENTID != 0)
                            {
                                master = context.clientmasters.Find(CLIENTID);
                            }
                            if (formcoll["REFCLIENTID"] != "")
                            {
                                master.REFCLIENTID = Convert.ToInt32(formcoll["REFCLIENTID"]);
                            }
                            else
                            {
                                master.REFCLIENTID = 0;
                            }
                            master.CLIENTTID = Convert.ToInt32(formcoll["CLIENTTID"]);
                            master.CLIENTCONTACTPERSON = Convert.ToString(formcoll["CLIENTCONTACTPERSON"]);
                            master.CLIENTNAME = Convert.ToString(formcoll["CLIENTNAME"]);
                            master.CLIENTMOBILENO1 = Convert.ToString(formcoll["CLIENTMOBILENO1"]);
                            master.CLIENTMOBILENO2 = Convert.ToString(formcoll["CLIENTMOBILENO2"]);
                            master.CLIENTLANDLINENO1 = Convert.ToString(formcoll["CLIENTLANDLINENO1"]);
                            master.CLIENTLANDLINENO2 = Convert.ToString(formcoll["CLIENTLANDLINENO2"]);
                            master.CLIENTEMAILID = Convert.ToString(formcoll["CLIENTEMAILID"]);
                            master.CLIENTCONTACTDESIGNATION = Convert.ToString(formcoll["CLIENTCONTACTDESIGNATION"]);                            
                            master.CLIENTADDRESS1 = Convert.ToString(formcoll["CLIENTADDRESS1"]);
                            master.CLIENTADDRESS2 = Convert.ToString(formcoll["CLIENTADDRESS2"]);
                            master.CLIENTCITY = Convert.ToString(formcoll["CLIENTCITY"]);
                            master.CLIENTSTATEID = Convert.ToInt32(formcoll["CLIENTSTATEID"]);
                            master.CLIENTCOUNTRYID = Convert.ToInt32(formcoll["CLIENTCOUNTRYID"]);
                            master.CLIENTPINCD = Convert.ToString(formcoll["CLIENTPINCD"]);
                            master.DISPSTATUS = Convert.ToInt16(formcoll["DISPSTATUS"]);
                            master.localityid = 0;
                            if (formcoll["localityid"] != "" && formcoll["localityid"] != null)
                                master.localityid = Convert.ToInt32(formcoll["localityid"]);
                            master.Nominee_RelationshipId = 0;
                            if (formcoll["Nominee_RelationshipId"] != "" && formcoll["Nominee_RelationshipId"] != null)
                                master.Nominee_RelationshipId = Convert.ToInt32(formcoll["Nominee_RelationshipId"]);
                            master.MonthlyIncome_or_Salary = 0;
                            if (formcoll["MonthlyIncome_or_Salary"]!="" && formcoll["MonthlyIncome_or_Salary"]!=null)
                                master.MonthlyIncome_or_Salary = Convert.ToDecimal(formcoll["MonthlyIncome_or_Salary"]);
                            master.Nominee_Name = Convert.ToString(formcoll["Nominee_Name"]);
                            master.Nominee_Address = Convert.ToString(formcoll["Nominee_Address"]);
                            master.Occupation= Convert.ToString(formcoll["Occupation"]);
                            master.OccupationAddress = Convert.ToString(formcoll["OccupationAddress"]);
                            master.Father_or_HusbandName = Convert.ToString(formcoll["Father_or_HusbandName"]);


                            if (CLIENTID == 0)
                            {
                                //string squery = "SELECT *FROM contactmaster(nolock) WHERE CLIENTID='" + master.CLIENTCONTACTPERSON+ "'";
                                //var query = context.Database.SqlQuery<ClientMaster>(squery).ToList();// context.crmsalestaskdetails.SqlQuery(squery).ToList<CRMSalesTaskDetail>();

                                //if (query.Count != 0)
                                //{
                                //    status = "Existing";
                                //}
                                //else
                                {
                                    master.CreatedBy = Session["CUSRID"].ToString();
                                    master.CreatedDt = DateTime.Now;

                                    context.clientmasters.Add(master);                                    
                                    context.SaveChanges();
                                    CLIENTID = master.CLIENTID;
                                    status = "Success";
                                }
                                   
                            }
                            else
                            {
                                master.UpdatedBy = Session["CUSRID"].ToString();
                                master.UpdatedDt = DateTime.Now;

                                context.Entry(master).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                status = "Success";
                            }

                          
                            trans1.Commit();
                            ViewBag.Message = "Form submitted.";
                            // Response.Redirect("Index");
                            //return RedirectToAction("Index", "ClientMaster");
                            //Response.Write("Update");
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            var ermsg = ex.Message;
                            Response.Write(ex.Message.ToString());
                            trans1.Rollback();
                            Response.Redirect("/Error/AccessDenied");
                            //return RedirectToAction("Index", "ClientMaster");
                            status = "Error";
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

        }
        #endregion

        #region Save Client Attachments

        [HttpPost]
        public ActionResult SaveClientFiles(string ids)//FormCollection fileData, HttpPostedFileBase logofilesupload)
        {
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0 || Session["CUSRID"].ToString() == "") { return RedirectToAction("Login", "Account"); } //  Response.Redirect(Url.Action("Login", "Account"));
            else
            {
                string status = "";
                #region Client Files Save
                int CLIENTID = 0;
                string clientid = Convert.ToString(ids);
                try
                {
                    if (clientid != "" && clientid != "0" && clientid != null && clientid != "undefined")
                    { 
                    CLIENTID = Convert.ToInt32(clientid);
                    }
                    if (Request.Files.Count > 0 && CLIENTID > 0) //if (logofilesupload != null && CLIENTID>0)
                    {
                        status = "Success";

                        /*Lopp for multiple files*/
                        /*Geting the file name*/
                        if (!System.IO.Directory.Exists(Server.MapPath("~/ClientFiles/" + CLIENTID)))
                            Directory.CreateDirectory(Server.MapPath("~/ClientFiles/" + CLIENTID));

                        var dir = new DirectoryInfo(Server.MapPath("~/ClientFiles/" + CLIENTID));

                        //string absolutePath = Server.MapPath(Server.MapPath("~/SRFfiles/logo/" + SRFMID));

                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase nfile = files[i];
                            /*Saving the file in server folder*/
                            //if (!System.IO.File.Exists(dir + "/" + nfile.FileName))
                            {
                                nfile.SaveAs(Server.MapPath("~/ClientFiles/" + CLIENTID.ToString() + "/" + nfile.FileName));
                            }

                        }
                        status = "Success";


                    }
                }
                catch (Exception ex)
                {
                    var aa = ex.Message.ToString();
                    //Response.Redirect("Error");
                    status = "FileError";
                }
                #endregion
                return Json(status, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Delete Record  
        //[Authorize(Roles = "ClientMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check(fld, id, "CLIENTMASTER");
            if (temp.Equals("PROCEED"))
            {
                ClientMaster clientMaster = context.clientmasters.Find(Convert.ToInt32(id));
                context.clientmasters.Remove(clientMaster);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion
    }
}
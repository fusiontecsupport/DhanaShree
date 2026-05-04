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
    public class ClientContactMasterController : Controller
    {
        // GET: ClientContactMaster

        #region contextdeclaration
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        //[Authorize(Roles = "ContactMasterIndex")]
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

                    var data = e.pr_Search_Master_ContactMaster(param.sSearch,
                                                    Convert.ToInt32(Request["iSortCol_0"]),
                                                    Request["sSortDir_0"],
                                                    param.iDisplayStart,
                                                    param.iDisplayStart + param.iDisplayLength,
                                                    totalRowsCount,
                                                    filteredRowsCount, Convert.ToString(cusrid));
                    var aaData = data.Select(d => new
                    {
                        ClientName = d.ClientName,
                        CONTACTPERSON = d.CONTACTPERSON,
                        ContactType = d.ContactType,
                        CPMOBILENO1 = d.CPMOBILENO1,
                        CPEMAILID = d.CPEMAILID,                        
                        DISPSTATUS = d.DISPSTATUS.ToString(),
                        CONTID = d.CONTID.ToString()
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
        //[Authorize(Roles = "ContactMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0 || Session["CUSRID"].ToString() == "") { return RedirectToAction("Login", "Account"); }
            else
            {

                ContactMaster tab = new ContactMaster();

                var ctqry = context.Database.SqlQuery<pr_Get_Contact_Type_Result>("exec pr_Get_Contact_Type 0").ToList();
                ViewBag.CONTTID = new SelectList(ctqry, "DValue", "DText").ToList();

                var clist1 = new SelectList(context.clientmasters.Where(d => d.DISPSTATUS == 0), "CLIENTID", "CLIENTNAME").ToList();

                ViewBag.CLIENTID = clist1;
                ViewBag.CPSTATEID = new SelectList(context.statemasters.Where(d => d.DISPSTATUS == 0), "STATEID", "STATEDESC");
                //ViewBag.CLIENTCOUNTRYID = new SelectList(context.crmcountrymasters.Where(d => d.DISPSTATUS == 0), "COUNTRYID", "COUNTRYDESC");

                List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
                SelectListItem selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
                selectedDISPSTATUS.Add(selectedItem);
                selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
                selectedDISPSTATUS.Add(selectedItem);
                ViewBag.DISPSTATUS = selectedDISPSTATUS;

                if (id != 0 && id != -1)  // IMP
                {
                    tab = context.contactmasters.Find(id);


                    ctqry = context.Database.SqlQuery<pr_Get_Contact_Type_Result>("exec pr_Get_Contact_Type " + tab.CONTTID).ToList();
                    ViewBag.CONTTID = new SelectList(ctqry, "DValue", "DText", tab.CONTTID).ToList();


                    ViewBag.CPSTATEID = new SelectList(context.statemasters.Where(d => d.DISPSTATUS == 0), "STATEID", "STATEDESC", tab.CPSTATEID);
                    var clista = new SelectList(context.clientmasters.Where(d => d.DISPSTATUS == 0), "CLIENTID", "CLIENTNAME", tab.CLIENTID).ToList();

                    ViewBag.CLIENTID = clista;
                    //ViewBag.CONTCOUNTRYID = new SelectList(context.crmcountrymasters.Where(d => d.DISPSTATUS == 0), "COUNTRYID", "COUNTRYDESC", tab.CONTCOUNTRYID);


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
                            ContactMaster master = new ContactMaster();


                            Int32 CONTID = 0;

                            string clientContID = Convert.ToString(formcoll["CONTID"]);
                            if (clientContID == "" || clientContID == null)
                            {
                                CONTID = 0;
                            }
                            else
                            {
                                CONTID = Convert.ToInt32(clientContID);
                            }

                            if (CONTID != 0)
                            {
                                master = context.contactmasters.Find(CONTID);
                            }
                            
                            master.CONTTID = Convert.ToInt32(formcoll["CONTTID"]);
                            master.CONTACTPERSON = Convert.ToString(formcoll["CONTACTPERSON"]);
                            master.CLIENTID = Convert.ToInt32(formcoll["CLIENTID"]);
                            master.CPMOBILENO1 = Convert.ToString(formcoll["CPMOBILENO1"]);
                            master.CPMOBILENO2 = Convert.ToString(formcoll["CPMOBILENO2"]);
                            master.CPLANDLINENO1 = Convert.ToString(formcoll["CPLANDLINENO1"]);
                            master.CPLANDLINENO2 = Convert.ToString(formcoll["CPLANDLINENO2"]);
                            master.CPEMAILID = Convert.ToString(formcoll["CPEMAILID"]);
                            master.CPDESIGNATION = Convert.ToString(formcoll["CPDESIGNATION"]);
                            master.CPADDRESS1 = Convert.ToString(formcoll["CPADDRESS1"]);
                            master.CPADDRESS2 = Convert.ToString(formcoll["CPADDRESS2"]);
                            master.CPCITY = Convert.ToString(formcoll["CPCITY"]);
                            master.CPSTATEID = Convert.ToInt32(formcoll["CPSTATEID"]);
                            master.CPCOUNTRYID = Convert.ToInt32(formcoll["CPCOUNTRYID"]);
                            master.CPPINCD = Convert.ToString(formcoll["CPPINCD"]);
                            master.DISPSTATUS = Convert.ToInt16(formcoll["DISPSTATUS"]);



                            if (CONTID == 0)
                            {
                                //string squery = "SELECT *FROM contactmaster(nolock) WHERE contactperson='" + master.CONTACTPERSON+ "'";
                                //var query = context.Database.SqlQuery<ContactMaster>(squery).ToList();// context.crmsalestaskdetails.SqlQuery(squery).ToList<CRMSalesTaskDetail>();

                                //if (query.Count != 0)
                                //{
                                //    status = "Existing";
                                //}
                                //else
                                {
                                    master.CreatedBy = Session["CUSRID"].ToString();
                                    master.CreatedDt = DateTime.Now;

                                    context.contactmasters.Add(master);
                                    context.SaveChanges();
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
                            //return RedirectToAction("Index", "ClientContactMaster");
                            //Response.Write("Update");
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            var ermsg = ex.Message;
                            Response.Write(ex.Message.ToString());
                            trans1.Rollback();
                            Response.Redirect("/Error/AccessDenied");
                            //return RedirectToAction("Index", "ClientContactMaster");
                            status = "Error";
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

        }
        #endregion


        #region Delete Record  
        //[Authorize(Roles = "ContactMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            //String temp = TD_DEL.delete_check1(fld, id, "CONTACTMASTER");
            String temp = "PROCEED";
            if (temp.Equals("PROCEED"))
            {
                ContactMaster contactMaster = context.contactmasters.Find(Convert.ToInt32(id));
                context.contactmasters.Remove(contactMaster);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion
    }
}
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
    public class OfficeManagementController : Controller
    {
        // GET: OfficeManagement

        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        //[Authorize(Roles = "OfficeManagementIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            return View();
        }
        #endregion

        #region GetAjaxDetailsData
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new FinFusionEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Company_OfficeMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount,"");
                var aaData = data.Select(d => new
                {
                    OfficeName = d.OfficeName,
                    TypeDesc = d.TypeDesc,
                    CityName = d.CityName.ToString(),
                    LocalityName = d.LocalityName,
                    DISPSTATUS = d.DISPSTATUS,
                    OfficeID = d.OfficeID.ToString()
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
        #endregion

        //#region Edit
        ////[Authorize(Roles = "OfficeManagementEdit")]
        //public void Edit(int? id = 0)
        //{
        //    if (id > 0)
        //    {
        //        var strPath = ConfigurationManager.AppSettings["BaseURL"];

        //        Response.Redirect("" + strPath + "/FundAccount/Form/" + id);

        //        //Response.Redirect("/FundAccount/Form/?id=" + id);
        //    }
        //}
        //#endregion

        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0 || Session["CUSRID"].ToString() == "") { return RedirectToAction("Login", "Account"); }
            else
            {

                CompanyOffice tab = new CompanyOffice();

                var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Company Office Types', ''").ToList();
                ViewBag.OfficeTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();

                ViewBag.CountryID = new SelectList(context.contactaddresscountrys.Where(d => d.CountryID > 0), "CountryID", "CountryName");
                ViewBag.CityID = new SelectList("");
                ViewBag.LocalityID = new SelectList("");

                List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
                SelectListItem selectedItem = new SelectListItem { Text = "Enabled", Value = "304", Selected = true };
                selectedDISPSTATUS.Add(selectedItem);
                selectedItem = new SelectListItem { Text = "Disabled", Value = "302", Selected = false };
                selectedDISPSTATUS.Add(selectedItem);
                ViewBag.DISPSTATUS = selectedDISPSTATUS;

                if (id != 0 && id != -1)  // IMP
                {
                    tab = context.companyoffices.Find(id);

                    mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Company Office Types', ''").ToList();
                    ViewBag.OfficeTypeID = new SelectList(mtqry, "TypeID", "Type", tab.OfficeTypeID).ToList();

                    ViewBag.CountryID = new SelectList(context.contactaddresscountrys.Where(d => d.CountryID > 0), "CountryID", "CountryName", tab.CountryID);
                    ViewBag.CityID = new SelectList(context.contactaddresscitys.Where(d => d.CountryID == tab.CountryID), "CityID", "CityName", tab.CityID);
                    ViewBag.LocalityID = new SelectList(context.contactaddresslocalitys.Where(d => d.CityID == tab.CityID), "LocalityID", "LocalityName", tab.LocalityID);


                    List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                    if (Convert.ToInt32(tab.StatusTypeID) == 304)
                    {
                        SelectListItem selectedItem31 = new SelectListItem { Text = "Enabled", Value = "304", Selected = true };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        selectedItem31 = new SelectListItem { Text = "Disabled", Value = "302", Selected = false };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                    }
                    else
                    {
                        SelectListItem selectedItem31 = new SelectListItem { Text = "Enabled", Value = "304", Selected = false };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        selectedItem31 = new SelectListItem { Text = "Disabled", Value = "302", Selected = true };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                    }
                }

                return View(tab);
            }
        }

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
                            CompanyOffice master = new CompanyOffice();


                            Int32 OFFICEID = 0;

                            string zofficeid = Convert.ToString(formcoll["OfficeID"]);
                            if (zofficeid == "" || zofficeid == null)
                            {
                                OFFICEID = 0;
                            }
                            else
                            {
                                OFFICEID = Convert.ToInt32(zofficeid);
                            }

                            if (OFFICEID != 0)
                            {
                                master = context.companyoffices.Find(OFFICEID);
                            }
                            master.OfficeTypeID = Convert.ToInt32(formcoll["OfficeTypeID"]);
                            master.OfficeParentID = 0;// Convert.ToString(formcoll["CONTACTPERSON"]);
                            master.OfficeName = Convert.ToString(formcoll["OfficeName"]);
                            master.StatusTypeID = Convert.ToInt32(formcoll["DISPSTATUS"]);
                            master.AddressLine1 = Convert.ToString(formcoll["AddressLine1"]);
                            master.AddressLine2 = Convert.ToString(formcoll["AddressLine2"]);
                            master.PIN = Convert.ToString(formcoll["PIN"]);
                            master.LocalityID = Convert.ToInt32(formcoll["LocalityID"]);
                            master.CityID = Convert.ToInt32(formcoll["CityID"]);
                            master.CountryID = Convert.ToInt32(formcoll["CountryID"]);

                            if (OFFICEID == 0)
                            {
                                {
                                    //master.CreatedBy = Session["CUSRID"].ToString();
                                    //master.CreatedDt = DateTime.Now;

                                    context.companyoffices.Add(master);
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
                            ViewBag.Message = "Form submitted.";
                            // Response.Redirect("Index");
                            //return RedirectToAction("Index", "OfficeManagement");
                            //Response.Write("Update");
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            var ermsg = ex.Message;
                            Response.Write(ex.Message.ToString());
                            trans1.Rollback();
                            Response.Redirect("/Error/AccessDenied");
                            //return RedirectToAction("Index", "OfficeManagement");
                            status = "Error";
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

        }
        #endregion


        public JsonResult Get_City_List(int id)
        {
            var result = context.Database.SqlQuery<ContactAddressCity>("Select * from ContactAddressCity WHERE CountryID = " + Convert.ToInt32(id)).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Get_Location_List(int id)
        {
            var result = context.Database.SqlQuery<ContactAddressLocality>("Select * from ContactAddressLocality WHERE CityID = " + Convert.ToInt32(id)).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

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
                CompanyOffice companyoffice = context.companyoffices.Find(Convert.ToInt32(id));
                context.companyoffices.Remove(companyoffice);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion
    }
}
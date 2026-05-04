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
    public class ContactTypeMasterController : Controller
    {
        // GET: ContactTypeMaster

        #region Context Declaration
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        //[Authorize(Roles = "ContactTypeMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0 || Session["CUSRID"].ToString() == "") { return RedirectToAction("Login", "Account"); }
            else { return View(); }
        }
        #endregion

        #region GetAjaxData
        public ActionResult GetAjaxData(JQueryDataTableParamModel param)
        {
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0 || Session["CUSRID"].ToString() == "") { return RedirectToAction("Login", "Account"); }
            else
            {
                using (var e = new FinFusionEntities())
                {
                    var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                    var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                    var data = e.pr_Search_Master_ContactTypeMaster(param.sSearch,
                                                    Convert.ToInt32(Request["iSortCol_0"]),
                                                    Request["sSortDir_0"],
                                                    param.iDisplayStart,
                                                    param.iDisplayStart + param.iDisplayLength,
                                                    totalRowsCount,
                                                    filteredRowsCount);
                    var aaData = data.Select(d => new
                    {
                        CONTTYPESHRTDESC = d.CONTTYPESHRTDESC,
                        CONTTYPEDESC = d.CONTTYPEDESC,
                        DISPSTATUS = d.DISPSTATUS.ToString(),
                        CreatedBy = d.CreatedBy,
                        CreatedDt = d.CreatedDt,
                        UpdatedBy = d.UpdatedBy,
                        UpdatedDt = d.UpdatedDt,
                        CONTTID = d.CONTTID.ToString()
                    }).ToArray();

                    return Json(new
                    {
                        //sEcho = param.sEcho,
                        data = aaData
                        //iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                        //iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion

        #region Form
        //[Authorize(Roles = "ContactTypeMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0 || Session["CUSRID"].ToString() == "") { return RedirectToAction("Login", "Account"); }
            else
            {
                ContactTypeMaster tab = new ContactTypeMaster();

                List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
                SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
                selectedDISPSTATUS.Add(selectedItem);
                selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
                selectedDISPSTATUS.Add(selectedItem);
                ViewBag.DISPSTATUS = selectedDISPSTATUS;


                if (id == -1)
                    ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";

                if (id != 0 && id != -1)  // IMP
                {
                    tab = context.contacttypemasters.Find(id);

                    List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                    if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                    {
                        SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        selectedItem31 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                    }
                    else
                    {
                        SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        selectedItem31 = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                    }
                }
                return View(tab);
            }
        }
        #endregion  

        #region savedata    
        [HttpPost]
        public ActionResult SaveData(ContactTypeMaster tab)
        {
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0 || Session["CUSRID"].ToString() == "") { return RedirectToAction("Login", "Account"); }
            else
            {
                string status = "";
                if ((tab.CONTTID).ToString() != "0")
                {

                    tab.UpdatedBy = Session["CUSRID"].ToString();
                    tab.UpdatedDt = DateTime.Now;

                    context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    status = "Update";
                    return Json(status, JsonRequestBehavior.AllowGet);

                }
                else
                {

                    var query = context.contacttypemasters.SqlQuery("SELECT *FROM CONTACTTYPEMASTER WHERE CONTTYPESHRTDESC = '" + tab.CONTTYPESHRTDESC + "' AND CONTTYPEDESC = '" + tab.CONTTYPEDESC + "'").ToList<ContactTypeMaster>();

                    if (query.Count != 0)
                    {
                        status = "Existing";
                        return Json(status, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        tab.CreatedBy = Session["CUSRID"].ToString();
                        tab.CreatedDt = DateTime.Now;


                        context.contacttypemasters.Add(tab);
                        context.SaveChanges();

                        status = "Success";
                        return Json(status, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }
        #endregion

        #region Delete Record    
        [Authorize(Roles = "ContactTypeMasterDelete")]
        public void Del()
        {
            //if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0 || Session["CUSRID"].ToString()=="") { return RedirectToAction("Login", "Account"); }
            //else
            //{
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            //String temp = TD_DEL.delete_check1(fld, id, "CONTACTTYPEMASTER");
            String temp = "PROCEED";
            String status = "";
            if (temp.Equals("PROCEED"))
            {
                //ContactTypeMaster contactTypeMaster = context.contacttypemasters.Find(Convert.ToInt32(id));
                //contactTypeMaster.DISPSTATUS = 1;
                //context.Entry(contactTypeMaster).State = System.Data.Entity.EntityState.Modified;
                //context.SaveChanges();
                ContactTypeMaster contactTypeMaster = context.contacttypemasters.Find(Convert.ToInt32(id));
                context.contacttypemasters.Remove(contactTypeMaster);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");

            }
            else
            {
                Response.Write(temp);
                status = Convert.ToString(temp);
            }
            //return Json(status, JsonRequestBehavior.AllowGet);
            //}
        }
        #endregion
    }
}
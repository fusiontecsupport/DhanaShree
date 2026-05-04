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
    //Get Category Master

   
    public class CategoryMasterController :Controller
    {
        #region context 
        AppDbContext context = new AppDbContext();
        #endregion
        #region Index
        [Authorize(Roles = "CategoryMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            return View();
        }
        #endregion

        #region GetAjaxDetailsData
        public JsonResult GetAjaxDetailsData(JQueryDataTableParamModel param)
        {
            using (var e = new FinFusionEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
                var data = e.pr_search_category_master(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);
                // var data = e.pr_search_category_master().ToList();
                var aaData = data.Select(d => new
                {
                    CategoryId = d.CategoryId,
                    Category = d.Category,
                    Classification = d.Classification,
                    ControlName = d.ControlName
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


        #region Edit
        [Authorize(Roles = "CategoryMasterEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/CategoryMaster/Form/" + id);

                //Response.Redirect("/DepartmentMaster/Form/?id=" + id);
            }
        }
        #endregion


        #region Form
         [Authorize(Roles = "CategoryMasterCreate")]
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            CategoryMaster tab = new CategoryMaster();

            int CategoryId = 0;

            //List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            //SelectListItem selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            //selectedDISPSTATUS.Add(selectedItem);
            //selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            //selectedDISPSTATUS.Add(selectedItem);
            //ViewBag.DISPSTATUS = selectedDISPSTATUS;

            if (id != "0")
            {
                CategoryId = Convert.ToInt32(id);

                tab = context.categorymasters.Find(CategoryId);

                //selectedDISPSTATUS = new List<SelectListItem>();
                //if (tab.DISPSTATUS == 0)
                //{
                //    selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
                //    selectedDISPSTATUS.Add(selectedItem);
                //    selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
                //    selectedDISPSTATUS.Add(selectedItem);
                //    ViewBag.DISPSTATUS = selectedDISPSTATUS;
                //}
                //else
                //{
                //    selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                //    selectedDISPSTATUS.Add(selectedItem);
                //    selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                //    selectedDISPSTATUS.Add(selectedItem);
                //    ViewBag.DISPSTATUS = selectedDISPSTATUS;
                //}
            }

            return View(tab);
        }
        #endregion

        #region Save Category Master
        [HttpPost]
        public ActionResult SaveData(FormCollection Fdata)
        {
            using (context = new AppDbContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    string status = "";
                    try
                    {
                        CategoryMaster CategoryMaster = new CategoryMaster();

                        Int32 CATID = 0;

                        string cmid = Convert.ToString(Fdata["CategoryId"]);
                        if (cmid == "" || cmid == null)
                        { CATID = 0; }
                        else
                        { CATID = Convert.ToInt32(cmid); }

                        if (CATID != 0)
                        {
                            CategoryMaster = context.categorymasters.Find(CATID);
                        }

                        CategoryMaster.Category = Convert.ToString(Fdata["Category"]);
                        CategoryMaster.Classification = Convert.ToString(Fdata["Classification"]);
                        CategoryMaster.ControlName = Convert.ToString(Fdata["ControlName"]);

                        //if (Session["CUSRID"] != null)
                        //{ CategoryMaster.CUSRID = Session["CUSRID"].ToString(); }
                        //else { CategoryMaster.CUSRID = ""; }

                        if (CATID == 0)
                        {

                          //  CategoryMaster.PRCSDATE = DateTime.Now;

                            string squery = "SELECT *FROM MasterCategory WHERE  CategoryId = '" + CategoryMaster.CategoryId + "'";
                            var query = context.Database.SqlQuery<CategoryMaster>(squery).ToList();

                            if (query.Count != 0)
                            {
                                status = "Existing";
                            }
                            else
                            {
                                //CategoryMaster.LMUSRID = "";
                                context.categorymasters.Add(CategoryMaster);
                                context.SaveChanges();
                                status = "Success";
                            }
                        }
                        else
                        {
                            //if (Session["CUSRID"] != null)
                            //{ CategoryMaster.LMUSRID = Session["CUSRID"].ToString(); }
                            //else { CategoryMaster.LMUSRID = ""; }

                            //CategoryMaster.PRCSDATE = DateTime.Now;

                            context.Entry(CategoryMaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            status = "Success";
                        }

                        trans.Commit();
                        return Json(status, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        var ermsg = ex.Message;
                        //Response.Write(ex.Message.ToString());
                        trans.Rollback();
                        //Response.Write("Error");
                        status = "Error";
                        //return RedirectToAction("Index", "DepartmentMaster");
                        return Json(status, JsonRequestBehavior.AllowGet);
                        //Response.Redirect("/Error/AccessDenied");
                    }

                }
            }

        }
        #endregion

        #region Delete Department Master 
        [Authorize(Roles = "CategoryMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check(fld, id, "MasterCategory");

            //String temp = "PROCEED";

            if (temp.Equals("PROCEED"))
            {
                CategoryMaster categmaster = context.categorymasters.Find(Convert.ToInt32(id));
                context.categorymasters.Remove(categmaster);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion 
    
    }
}
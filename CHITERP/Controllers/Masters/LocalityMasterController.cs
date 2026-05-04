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
    public class LocalityMasterController : Controller
    {
        // GET: LocalityMaster

        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        //[Authorize(Roles = "LocalityMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            return View();
        }
        #endregion
        #region Edit
        //[Authorize(Roles = "LocalityMasterEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/LocalityMaster/Form/" + id);

                //Response.Redirect("/LocalityMaster/Form/?id=" + id);
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

                    var data = e.pr_Search_Master_LocalityMaster(param.sSearch,
                                                    Convert.ToInt32(Request["iSortCol_0"]),
                                                    Request["sSortDir_0"],
                                                    param.iDisplayStart,
                                                    param.iDisplayStart + param.iDisplayLength,
                                                    totalRowsCount,
                                                    filteredRowsCount, Convert.ToString(cusrid));
                    var aaData = data.Select(d => new
                    {

                        LocalityID = d.LocalityID.ToString(),
                        LocalityName = d.LocalityName,
                        CityID = d.CityID.ToString(),
                        CityName = d.CityName
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
        //[Authorize(Roles = "LocalityMasterCreate")]
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            LocalityMaster tab = new LocalityMaster();

            int LocalityID = 0;

            if (id == "" || id == null || id == "0")
            { LocalityID = 0; }
            else
            {
                LocalityID = Convert.ToInt32(id);

            }

            var amtcdqry = context.Database.SqlQuery<CityMaster>("Select * From ContactAddressCity (nolock) where countryid = 1").ToList();
            ViewBag.CityID = new SelectList(amtcdqry, "CityID", "CityName");

            if (LocalityID > 0)
            {
                tab = context.localityMasters.Find(LocalityID);
                ViewBag.LocalityID = tab.LocalityID;
                ViewBag.LocalityName = tab.LocalityName;
                amtcdqry = context.Database.SqlQuery<CityMaster>("Select * From ContactAddressCity (nolock) where countryid = 1").ToList();
                ViewBag.CityID = new SelectList(amtcdqry.Where(x => x.CityID == tab.CityID), "CityID", "CityName", tab.CityID);


            }
            else
            {
                tab = context.localityMasters.Find(0);
            }


            return View(tab);
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

                            LocalityMaster master = new LocalityMaster();

                            Int32 LocalityID = 0;

                            string cityid = Convert.ToString(formcoll["LocalityID"]);

                            if (cityid == "" || cityid == null)
                            {
                                LocalityID = 0;
                            }
                            else
                            {
                                LocalityID = Convert.ToInt32(cityid);
                            }

                            if (LocalityID != 0)
                            {
                                master = context.localityMasters.Find(LocalityID);
                            }
                            master.LocalityID = LocalityID;
                            master.CityID = Convert.ToInt32(formcoll["CityID"]);
                            master.LocalityName = Convert.ToString(formcoll["LocalityName"]);


                            if (LocalityID == 0)
                            {
                                string squery = "SELECT *FROM CONTACTADDRESSLOCALITY (nolock) WHERE LocalityName ='" + master.LocalityName + "'";
                                var query = context.Database.SqlQuery<LocalityMaster>(squery).ToList();

                                if (query.Count != 0)
                                {
                                    status = "Existing";
                                }
                                else
                                {
                                    //master.CreatedBy = Session["CUSRID"].ToString();
                                    //master.CreatedDt = DateTime.Now;

                                    context.localityMasters.Add(master);
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
                            LocalityID = master.LocalityID;



                            ViewBag.Message = "Form submitted.";
                            // Response.Redirect("Index");
                            //return RedirectToAction("Index", "LocalityMaster");
                            //Response.Write("Update");
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            var ermsg = ex.Message;
                            //Response.Write(ex.Message.ToString());
                            trans1.Rollback();
                            //Response.Redirect("/Error/AccessDenied");
                            //return RedirectToAction("Index", "LocalityMaster");
                            status = "Error";
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

        }
        #endregion

        #region Delete Record  
        //[Authorize(Roles = "LocalityMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check(fld, id, "LocalityMaster");
            if (temp.Equals("PROCEED"))
            {
                LocalityMaster chitgroup = context.localityMasters.Find(Convert.ToInt32(id));
                context.localityMasters.Remove(chitgroup);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion
    }
}
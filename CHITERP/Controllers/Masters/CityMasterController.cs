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
    public class CityMasterController : Controller
    {
        // GET: CityMaster

        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        //[Authorize(Roles = "CityMasterIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            return View();
        }
        #endregion
        #region Edit
        //[Authorize(Roles = "CityMasterEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/CityMaster/Form/" + id);

                //Response.Redirect("/CityMaster/Form/?id=" + id);
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

                    var data = e.pr_Search_Master_CityMaster(param.sSearch,
                                                    Convert.ToInt32(Request["iSortCol_0"]),
                                                    Request["sSortDir_0"],
                                                    param.iDisplayStart,
                                                    param.iDisplayStart + param.iDisplayLength,
                                                    totalRowsCount,
                                                    filteredRowsCount, Convert.ToString(cusrid));
                    var aaData = data.Select(d => new
                    {

                        CityID = d.CityID.ToString(),
                        CityName = d.CityName,
                        CountryID = d.CountryID.ToString(),
                        CountryName = d.CountryName
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
        //[Authorize(Roles = "CityMasterCreate")]
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            CityMaster tab = new CityMaster();

            int CityID = 0;

            if (id == "" || id == null || id == "0")
            { CityID = 0; }
            else
            {
                CityID = Convert.ToInt32(id);

            }

            var amtcdqry = context.Database.SqlQuery<CountryMaster>("Select * From ContactAddressCountry (nolock)").ToList();
            ViewBag.CountryID = new SelectList(amtcdqry, "CountryID", "CountryName", 1);

            if (CityID > 0)
            {
                tab = context.cityMasters.Find(CityID);
                ViewBag.CityID = tab.CityID;
                ViewBag.CityName = tab.CityName;
                amtcdqry = context.Database.SqlQuery<CountryMaster>("Select * From ContactAddressCountry (nolock)").ToList();
                ViewBag.CountryID = new SelectList(amtcdqry.Where(x => x.CountryID == tab.CountryID), "CountryID", "CountryName", tab.CountryID);


            }
            else
            {
                tab = context.cityMasters.Find(0);
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

                            CityMaster master = new CityMaster();

                            Int32 CityID = 0;

                            string cityid = Convert.ToString(formcoll["CityID"]);

                            if (cityid == "" || cityid == null)
                            {
                                CityID = 0;
                            }
                            else
                            {
                                CityID = Convert.ToInt32(cityid);
                            }

                            if (CityID != 0)
                            {
                                master = context.cityMasters.Find(CityID);
                            }
                            master.CityID = CityID;
                            master.CountryID = Convert.ToInt32(formcoll["CountryID"]);
                            master.CityName = Convert.ToString(formcoll["CityName"]);


                            if (CityID == 0)
                            {
                                string squery = "SELECT *FROM CityMaster (nolock) WHERE CityName ='" + master.CityName + "'";
                                var query = context.Database.SqlQuery<CityMaster>(squery).ToList();

                                if (query.Count != 0)
                                {
                                    status = "Existing";
                                }
                                else
                                {
                                    //master.CreatedBy = Session["CUSRID"].ToString();
                                    //master.CreatedDt = DateTime.Now;

                                    context.cityMasters.Add(master);
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
                            CityID = master.CityID;



                            ViewBag.Message = "Form submitted.";
                            // Response.Redirect("Index");
                            //return RedirectToAction("Index", "CityMaster");
                            //Response.Write("Update");
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            var ermsg = ex.Message;
                            //Response.Write(ex.Message.ToString());
                            trans1.Rollback();
                            //Response.Redirect("/Error/AccessDenied");
                            //return RedirectToAction("Index", "CityMaster");
                            status = "Error";
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

        }
        #endregion

        #region Delete Record  
        //[Authorize(Roles = "CityMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check(fld, id, "CityMaster");
            if (temp.Equals("PROCEED"))
            {
                CityMaster chitgroup = context.cityMasters.Find(Convert.ToInt32(id));
                context.cityMasters.Remove(chitgroup);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion
    }
}
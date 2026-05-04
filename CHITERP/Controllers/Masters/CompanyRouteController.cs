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
    public class CompanyRouteController : Controller
    {
        // GET: CompanyRoute
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

                var data = e.pr_Search_Company_Route_Master(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, "");
                var aaData = data.Select(d => new
                {
                    RouteCode = d.RouteCode,
                    RouteName = d.RouteName,
                    DISPSTATUS = d.DISPSTATUS,
                    RouteID = d.RouteID.ToString()
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

                CompanyRoute tab = new CompanyRoute();

                ViewBag.LinkedOfficeID = new SelectList(context.companyoffices.Where(d => d.OfficeID > 0), "OfficeID", "OfficeName");

                List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
                SelectListItem selectedItem = new SelectListItem { Text = "Enabled", Value = "450", Selected = true };
                selectedDISPSTATUS.Add(selectedItem);
                selectedItem = new SelectListItem { Text = "Disabled", Value = "452", Selected = false };
                selectedDISPSTATUS.Add(selectedItem);
                ViewBag.DISPSTATUS = selectedDISPSTATUS;

                if (id != 0 && id != -1)  // IMP
                {
                    tab = context.companyroutes.Find(id);

                    ViewBag.LinkedOfficeID = new SelectList(context.companyoffices.Where(d => d.OfficeID > 0), "OfficeID", "OfficeName", tab.LinkedOfficeID);

                    List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                    if (Convert.ToInt32(tab.StatusTypeID) == 450)
                    {
                        SelectListItem selectedItem31 = new SelectListItem { Text = "Enabled", Value = "450", Selected = true };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        selectedItem31 = new SelectListItem { Text = "Disabled", Value = "452", Selected = false };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                    }
                    else
                    {
                        SelectListItem selectedItem31 = new SelectListItem { Text = "Enabled", Value = "450", Selected = false };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        selectedItem31 = new SelectListItem { Text = "Disabled", Value = "452", Selected = true };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                    }
                }

                return View(tab);
            }
        }

        public ActionResult NForm(int? id = 0)
        {
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0 || Session["CUSRID"].ToString() == "") { return RedirectToAction("Login", "Account"); }
            else
            {

                CompanyRoute tab = new CompanyRoute();
                CompanyRouteList vm = new CompanyRouteList();

                ViewBag.LinkedOfficeID = new SelectList(context.companyoffices.Where(d => d.OfficeID > 0), "OfficeID", "OfficeName");
                ViewBag.LocalityID = new SelectList(context.contactaddresslocalitys.Where(x => x.LocalityID > 0).OrderBy(x => x.LocalityName), "LocalityID", "LocalityName");

                List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
                SelectListItem selectedItem = new SelectListItem { Text = "Enabled", Value = "450", Selected = true };
                selectedDISPSTATUS.Add(selectedItem);
                selectedItem = new SelectListItem { Text = "Disabled", Value = "452", Selected = false };
                selectedDISPSTATUS.Add(selectedItem);
                ViewBag.DISPSTATUS = selectedDISPSTATUS;

                if (id != 0 && id != -1)  // IMP
                {
                    tab = context.companyroutes.Find(id);

                    ViewBag.LinkedOfficeID = new SelectList(context.companyoffices.Where(d => d.OfficeID > 0), "OfficeID", "OfficeName", tab.LinkedOfficeID);

                    List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                    if (Convert.ToInt32(tab.StatusTypeID) == 450)
                    {
                        SelectListItem selectedItem31 = new SelectListItem { Text = "Enabled", Value = "450", Selected = true };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        selectedItem31 = new SelectListItem { Text = "Disabled", Value = "452", Selected = false };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                    }
                    else
                    {
                        SelectListItem selectedItem31 = new SelectListItem { Text = "Enabled", Value = "450", Selected = false };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        selectedItem31 = new SelectListItem { Text = "Disabled", Value = "452", Selected = true };
                        selectedDISPSTATUS1.Add(selectedItem31);
                        ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                    }

                    vm.cr_masterdata = context.companyroutes.Where(d => d.RouteID == id).ToList();
                    vm.crd_detaildata = context.companyroutedetails.Where(d => d.RouteID == id).ToList();
                }

                return View(vm);
            }
        }

        public ActionResult SForm(string id = "0")
        {
            CompanyRouteDetail tab = new CompanyRouteDetail();
            CompanyRouteList vm = new CompanyRouteList();

            ViewBag.LocalityID = new SelectList(context.contactaddresslocalitys.Where(x => x.LocalityID > 0).OrderBy(x => x.LocalityName), "LocalityID", "LocalityName");

            var query = context.Database.SqlQuery<CompanyRoute>("Select * From CompanyRoute Where RouteID = " + Convert.ToInt32(id)).ToList();

            ViewBag.Id = query[0].RouteID;
            ViewBag.RouteCode = query[0].RouteCode;
            ViewBag.RouteName = query[0].RouteName;

            var sql = context.Database.SqlQuery<int>("select RoutelDetailID from CompanyRouteDetail  where RouteID = " + Convert.ToInt32(id)).ToList();
            if (sql.Count() > 0)
            {
                vm.crd_detaildata = context.Database.SqlQuery<CompanyRouteDetail>("Select * from CompanyRouteDetail  where RouteID = " + Convert.ToInt32(id)).ToList();


            }
            return View(vm);
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
                            CompanyRoute master = new CompanyRoute();


                            Int32 ROUTEID = 0;

                            string zrouteid = Convert.ToString(formcoll["RouteID"]);
                            if (zrouteid == "" || zrouteid == null)
                            {
                                ROUTEID = 0;
                            }
                            else
                            {
                                ROUTEID = Convert.ToInt32(zrouteid);
                            }

                            if (ROUTEID != 0)
                            {
                                master = context.companyroutes.Find(ROUTEID);
                            }
                            master.LinkedOfficeID = Convert.ToInt32(formcoll["LinkedOfficeID"]);
                            master.RouteCityID = 0;// Convert.ToString(formcoll["CONTACTPERSON"]);
                            master.SupervisorEmployeeID = 37;// Convert.ToString(formcoll["CONTACTPERSON"]);
                            master.RouteCode = Convert.ToString(formcoll["RouteCode"]);
                            master.RouteName = Convert.ToString(formcoll["RouteName"]);
                            master.StatusTypeID = Convert.ToInt32(formcoll["DISPSTATUS"]);
                            if (ROUTEID == 0)
                            {
                                {
                                    //master.CreatedBy = Session["CUSRID"].ToString();
                                    //master.CreatedDt = DateTime.Now;

                                    context.companyroutes.Add(master);
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
                            return RedirectToAction("Index", "CompanyRoute");
                            //Response.Write("Update");
                            //return Json(status, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            var ermsg = ex.Message;
                            Response.Write(ex.Message.ToString());
                            trans1.Rollback();
                            Response.Redirect("/Error/AccessDenied");
                            return RedirectToAction("Index", "CompanyRoute");
                            //status = "Error";
                            //return Json(status, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

        }
        #endregion

        #region NSaveData 
        [HttpPost]
        public JsonResult NSaveData(FormCollection F_Form)//...Direct
        {
            using (AppDbContext context = new AppDbContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        CompanyRoute CRouteMaster = new CompanyRoute();
                        CompanyRouteDetail CRouteDetail = new CompanyRouteDetail();
                        Int32 RouteID = Convert.ToInt32(F_Form["RouteID"]);
                        Int32 RoutelDetailID = 0;
                        string DELIDS = "";

                        var redirect = "";

                        if (RouteID != 0)//Getting Primary id in Edit mode
                        {
                            CRouteMaster = context.companyroutes.Find(RouteID);

                        }
                        CRouteMaster.LinkedOfficeID = Convert.ToInt32(F_Form["LinkedOfficeID"]);
                        CRouteMaster.RouteCityID = 0;// Convert.ToString(formcoll["CONTACTPERSON"]);
                        CRouteMaster.SupervisorEmployeeID = 37;// Convert.ToString(formcoll["CONTACTPERSON"]);
                        CRouteMaster.RouteCode = F_Form["RouteCode"].ToString();
                        CRouteMaster.RouteName = F_Form["RouteName"].ToString();
                        CRouteMaster.StatusTypeID = Convert.ToInt32(F_Form["DISPSTATUS"]);

                        if (RouteID == 0)
                        {
                            context.companyroutes.Add(CRouteMaster);
                            context.SaveChanges(); redirect = "Dform/" + CRouteMaster.RouteID;
                        }
                        else
                        {
                            context.Entry(CRouteMaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges(); redirect = "Index";
                        }

                        //....................Transaction Detail---------------------------
                        string[] A_RoutelDetailID = F_Form.GetValues("RoutelDetailID");
                        string[] LocalityID = F_Form.GetValues("LocalityID");
                        if (LocalityID == null)
                            LocalityID = F_Form.GetValues("crd_detaildata[0].LocalityID");

                        string boolF_QDAIDS = "true";
                        if (LocalityID != null)
                        {
                            for (int count = 0; count < LocalityID.Count(); count++)
                            {
                                //  Response.Write("trandet" + count + "<br/>");
                                if (boolF_QDAIDS == "true")
                                {
                                    RoutelDetailID = Convert.ToInt32(A_RoutelDetailID[count]);
                                    if (RoutelDetailID != 0)
                                    {

                                        CRouteDetail = context.companyroutedetails.Find(RoutelDetailID);
                                    }
                                    CRouteDetail.RouteID = CRouteMaster.RouteID;
                                    CRouteDetail.LocalityID = Convert.ToInt32(LocalityID[count]);
                                    CRouteDetail.LocalityPriority = count + 1;// CORDDREFNO[count].ToString();
                                    CRouteDetail.StatusTypeID = 450;

                                    if (Convert.ToInt32(RoutelDetailID) == 0)
                                    {
                                        context.companyroutedetails.Add(CRouteDetail);
                                        context.SaveChanges();
                                        RoutelDetailID = CRouteDetail.RoutelDetailID;
                                        redirect = "Dform/" + CRouteDetail.RoutelDetailID;
                                    }
                                    else
                                    {
                                        context.Entry(CRouteDetail).State = System.Data.Entity.EntityState.Modified;
                                        context.SaveChanges();
                                    }
                                    DELIDS = DELIDS + "," + RoutelDetailID.ToString();

                                }

                            }
                        }
                        

                        //..............delete transaction master factor record

                        //.......... terms and condition.......................
                        if (DELIDS !="")
                            context.Database.ExecuteSqlCommand("DELETE FROM CompanyRouteDetail  WHERE RouteID = " + RouteID + " and  RoutelDetailID NOT IN(" + DELIDS.Substring(1) + ")");
                        string status = "Success";
                        
                        //Response.Redirect("DForm/" + TransactionMaster.TRANMID);
                        //Response.Redirect("Index");
                        //  Response.Write("DForm/" + TransactionMaster.TRANMID);
                        trans.Commit();
                        return Json(status, JsonRequestBehavior.AllowGet);

                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        trans.Rollback(); 
                        //Response.Redirect("/Error/SavepointErr");
                        string status = "Error";
                        return Json(status, JsonRequestBehavior.AllowGet);
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
                CompanyRoute companyroute = context.companyroutes.Find(Convert.ToInt32(id));
                context.companyroutes.Remove(companyroute);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion

    }
}
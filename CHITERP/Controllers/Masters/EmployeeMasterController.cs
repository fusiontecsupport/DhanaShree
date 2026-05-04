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
    public class EmployeeMasterController : Controller
    {
        // GET: EmployeeMaster

        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        //[Authorize(Roles = "EmployeeMasterIndex")]
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

                var data = e.pr_Search_Masters_Employee(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);
                var aaData = data.Select(d => new
                {
                    CATECODE = d.CATECODE,
                    CATENAME = d.CATENAME,
                    DSGNDESC = d.DSGNDESC,
                    DEPTDESC = d.DEPTDESC,
                    CATECPNAME = d.CATECPNAME,
                    CATEPHN3 = d.CATEPHN3,
                    CATEEMAIL = d.CATEEMAIL,
                    DISPSTATUS = d.DISPSTATUS.ToString(),
                    CATEID = d.CATEID.ToString()
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
        #endregion

        #region Edit
        [Authorize(Roles = "EmployeeMasterEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/EmployeeMaster/Form/" + id);

                //Response.Redirect("/EmployeeMaster/Form/?id=" + id);
            }
        }
        #endregion

        #region Form
        [Authorize(Roles = "EmployeeMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            EmployeeMaster tab = new EmployeeMaster();
           
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            tab.CATEID = 0;

            ViewBag.DEPTID = new SelectList(context.departmentmasters, "DEPTID", "DEPTDESC");
            ViewBag.DSGNID = new SelectList(context.designationmasters, "DSGNID", "DSGNDESC");
           
            ViewBag.primary = 0;

            // Employee Route Details - route list and default status list
            ViewBag.RouteList = context.companyroutes.Where(r => r.RouteID > 0).OrderBy(r => r.RouteName).ToList();

            List<SelectListItem> empRouteStatus = new List<SelectListItem>();
            SelectListItem empRouteStatusItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            empRouteStatus.Add(empRouteStatusItem);
            empRouteStatusItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            empRouteStatus.Add(empRouteStatusItem);
            ViewBag.EmpRouteStatus = empRouteStatus;

            // IMP
            if (id == -1)
                ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";


            if (id != 0 && id != -1)  // IMP  
            {

                tab = context.employeemasters.Find(id);

                ViewBag.primary = tab.CATEID;

                ViewBag.DEPTID = new SelectList(context.departmentmasters, "DEPTID", "DEPTDESC", tab.DEPTID);
                ViewBag.DSGNID = new SelectList(context.designationmasters, "DSGNID", "DSGNDESC", tab.DSGNID);
               
                // Load existing route mappings for this employee
                var empRoutes = context.companyrouteemployees.Where(e => e.EmployeeID == tab.CATEID).ToList();
                if (empRoutes.Count > 0)
                {
                    ViewBag.EmpRouteDetails = empRoutes;
                }
            
                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem3 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem3);
                    selectedItem3 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem3);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
            }
            return View(tab);
        }
        #endregion

        #region Save Employee details Master
        [HttpPost]
        public ActionResult SaveData(EmployeeMaster tab)
        {
            string status = "";
            using (context = new AppDbContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        
                        tab.PRCSDATE = DateTime.Now;

                        tab.CATETID = 1;
                        tab.REGNID = 0;
                        tab.LOCTID = 0;
                        tab.CATESTATUS = 0;

                        tab.CATEDOB = null;
                        tab.CATEDOJ = null;
                        tab.CATEDOC = null;
                        tab.CATEDOR = null;

                        if (tab.CUSRID == "" || tab.CUSRID == null)
                        {
                            if (Session["CUSRID"] != null)
                            {
                                tab.CUSRID = Session["CUSRID"].ToString();
                            }
                            else { tab.CUSRID = "0"; }
                        }
                        tab.CATECODE = "E" + tab.CATEID.ToString();


                        if ((tab.CATEID).ToString() != "0" || tab.CATEID != 0)
                        {
                            
                            tab.LMUSRID = Session["CUSRID"].ToString(); 
                            context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            status = "Success";

                        }
                        else
                        {
                            string squery = "SELECT *FROM EMPLOYEEMASTER  WHERE CATENAME='" + tab.CATENAME + "' AND DEPTID=" + tab.DEPTID + " AND DSGNID = " + tab.DSGNID + "";                            
                            var query = context.Database.SqlQuery<EmployeeMaster>(squery).ToList();

                            if (query.Count != 0)
                            {
                                status = "Existing";
                                //return Json(status, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                tab.LMUSRID = "";                                
                                context.employeemasters.Add(tab);
                                context.SaveChanges();
                                tab.CATECODE = "E" + tab.CATEID.ToString();
                                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                status = "Success";
                                //return Json(status, JsonRequestBehavior.AllowGet);
                            }
                        }
                        // Save Employee Route Details when employee is saved successfully
                        if (status == "Success")
                        {
                            int employeeId = tab.CATEID;

                            string[] aRouteEmployeeId = Request.Form.GetValues("RouteEmployeeID");
                            string[] routeIds = Request.Form.GetValues("RouteID");
                            string[] routeStatusTypeIds = Request.Form.GetValues("RouteStatusTypeID");

                            if (routeIds != null && aRouteEmployeeId != null && routeStatusTypeIds != null)
                            {
                                string delIds = "";

                                int nextRouteEmployeeId = 1;
                                if (context.companyrouteemployees.Any())
                                {
                                    nextRouteEmployeeId = context.companyrouteemployees.Max(x => x.RouteEmployeeID) + 1;
                                }

                                for (int i = 0; i < routeIds.Length; i++)
                                {
                                    int routeId;
                                    if (!int.TryParse(routeIds[i], out routeId) || routeId == 0)
                                    {
                                        continue;
                                    }

                                    int routeEmployeeId = 0;
                                    int.TryParse(aRouteEmployeeId[i], out routeEmployeeId);

                                    int statusTypeId;
                                    if (!int.TryParse(routeStatusTypeIds[i], out statusTypeId))
                                    {
                                        statusTypeId = 0;
                                    }

                                    CompanyRouteEmployee routeDetail;
                                    bool isNew = false;
                                    if (routeEmployeeId != 0)
                                    {
                                        routeDetail = context.companyrouteemployees.Find(routeEmployeeId);
                                        if (routeDetail == null)
                                        {
                                            routeDetail = new CompanyRouteEmployee();
                                            routeDetail.RouteEmployeeID = nextRouteEmployeeId++;
                                            context.companyrouteemployees.Add(routeDetail);
                                            isNew = true;
                                        }
                                    }
                                    else
                                    {
                                        routeDetail = new CompanyRouteEmployee();
                                        routeDetail.RouteEmployeeID = nextRouteEmployeeId++;
                                        context.companyrouteemployees.Add(routeDetail);
                                        isNew = true;
                                    }

                                    routeDetail.RouteID = routeId;
                                    routeDetail.EmployeeID = employeeId;
                                    if (isNew)
                                    {
                                        routeDetail.StartDate = DateTime.Now;
                                        routeDetail.EndDate = DateTime.Now;
                                    }
                                    routeDetail.StatusTypeID = statusTypeId;

                                    context.SaveChanges();

                                    delIds = delIds + "," + routeDetail.RouteEmployeeID.ToString();
                                }

                                if (delIds != "" && employeeId != 0)
                                {
                                    context.Database.ExecuteSqlCommand("DELETE FROM CompanyRouteEmployee WHERE EmployeeID = " + employeeId + " AND RouteEmployeeID NOT IN(" + delIds.Substring(1) + ")");
                                }
                            }
                        }
                        trans.Commit();
                        return Json(status, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        status = "Error";
                        string Message = ex.Message.ToString();
                        return Json(status, JsonRequestBehavior.AllowGet);
                        //Response.Write("Sorry!! An Error Occurred.... ");
                    }
                }
            }
            //return Json(status, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Delete Employee Master
        [Authorize(Roles = "EmployeeMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check(fld, id, "EmployeeMaster");

            //String temp = "PROCEED";

            if (temp.Equals("PROCEED"))
            {
                EmployeeMaster employeemaster = context.employeemasters.Find(Convert.ToInt32(id));
                context.employeemasters.Remove(employeemaster);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }
        #endregion 
       
    }
}
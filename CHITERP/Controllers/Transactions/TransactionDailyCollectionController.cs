using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CHITERP.Data;
using CHITERP.Models;

namespace CHITERP.Controllers.TransactionDailyCollection
{
    public class TransactionDailyCollectionController : Controller
    {
        private readonly AppDbContext context = new AppDbContext();

        // GET: TransactionDailyCollection
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            // Reuse the existing Transaction Collection index page, but mark source as daily bulk
            return RedirectToAction("Index", "TransactionCollection", new { src = "daily" });
        }

        // GET: TransactionDailyCollection/Form
        public ActionResult Form()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0)
            {
                string refurl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToString();
                var routeValues = new RouteValueDictionary { { "returnUrl", refurl } };
                return RedirectToAction("Login", "Account", routeValues);
            }

            TransactionCollectionList tab = null;

            var mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Product', ''").ToList();
            ViewBag.ProductTypeID = new SelectList(mtqry.Where(x => x.Type == "Chit Fund" || x.Type == "Loan"), "TypeID", "Type").ToList();

            // Transaction modes for the grid (Mode column)
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Transaction Mode', ''").ToList();
            ViewBag.TransactionModeTypeID = new SelectList(mtqry, "TypeID", "Type").ToList();

            // Sub transaction categories for header (default for Collection)
            string category = "C";
            string crole = Convert.ToString(Session["Group"]);
            string cusrid = Convert.ToString(Session["CUSRID"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                cusrid = "";
            }
            string trntypqry = "exec pr_Get_ProductCategory @usrid='" + cusrid + "',@empid=" + Convert.ToInt32(Session["EMPLID"]) + ",@prod=29,@trncateg='" + category + "'";
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>(trntypqry).ToList();
            ViewBag.SubsTranCategory = new SelectList(mtqry, "TypeID", "Type").ToList();

            // Transaction category header (Collection only)
            List<SelectListItem> selTrnCatg = new List<SelectListItem>();
            SelectListItem selTrnCatgItm = new SelectListItem { Text = "Collection", Value = "C", Selected = true };
            selTrnCatg.Add(selTrnCatgItm);
            ViewBag.TransactionCategory = selTrnCatg;

            // Status list (default Collected)
            mtqry = context.Database.SqlQuery<pr_Get_Master_Types_Result>("exec pr_Get_Master_Types 'Transaction Management', 'Status'").ToList();
            ViewBag.StatusTypeID = new SelectList(mtqry.Where(d => d.Type == "Collected"), "TypeID", "Type").ToList();

            // Assigned to employee list (Employee Master)
            var empList = context.employeemasters.Where(e => e.DISPSTATUS == 0).OrderBy(e => e.CATENAME).ToList();
            ViewBag.AssignedToEmployeeID = new SelectList(empList, "CATEID", "CATENAME").ToList();

            // Route list will be loaded dynamically based on selected employee
            ViewBag.RouteID = new List<SelectListItem>();

            ViewBag.Title = "Daily Bulk Collection Entry";

            return View(tab);
        }

        public JsonResult GetActiveSubscriptions(int prod, int? empId = null, int? routeId = null)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0)
            {
                return Json(new object[] { }, JsonRequestBehavior.AllowGet);
            }

            string crole = Convert.ToString(Session["Group"]);
            string cusrid = Convert.ToString(Session["CUSRID"]);
            if (crole == "SuperAdmin" || crole == "Admin")
            {
                cusrid = "";
            }

            int empIdToUse = empId ?? Convert.ToInt32(Session["EMPLID"]);

            // For bulk add: if a Route is selected, we want to filter only by route, not by employee.
            // So we clear usrId and empId so the procedure returns all subs, then we apply route filter below.
            if (routeId.HasValue && routeId.Value > 0)
            {
                cusrid = "";
                empIdToUse = 0;
            }

            string trntypqry = "exec pr_Get_Subscription_Info @usrid='" + cusrid + "',@empid=" + empIdToUse + ",@prod=" + prod;
            var subsqry = context.Database.SqlQuery<pr_Get_Subscription_Info_Result>(trntypqry).ToList();

            var subsIds = subsqry
                .Where(s => s.subscriptionid.HasValue)
                .Select(s => s.subscriptionid.Value)
                .ToList();

            if (subsIds.Count > 0)
            {
                if (prod == 29) // Chit Fund
                {
                    var allowedStatusIds = new[] { 881, 884 };

                    var activeArrearSubs = context.chitsubscriptions
                        .Where(c => subsIds.Contains(c.SubscriptionID) && allowedStatusIds.Contains(c.StatusTypeID))
                        .Select(c => c.SubscriptionID)
                        .ToList();

                    subsqry = subsqry
                        .Where(s => s.subscriptionid.HasValue && activeArrearSubs.Contains(s.subscriptionid.Value))
                        .ToList();
                }
                else if (prod == 30) // Loan
                {
                    var allowedStatusIds = new[] { 968, 974 };

                    var activeArrearSubs = context.loansubscription
                        .Where(l => subsIds.Contains(l.LoanSubscriptionID) && allowedStatusIds.Contains(l.StatusTypeID))
                        .Select(l => l.LoanSubscriptionID)
                        .ToList();

                    subsqry = subsqry
                        .Where(s => s.subscriptionid.HasValue && activeArrearSubs.Contains(s.subscriptionid.Value))
                        .ToList();
                }
            }

            // If a RouteID is selected, further restrict subscriptions to that route
            if (routeId.HasValue && routeId.Value > 0)
            {
                int selRouteId = routeId.Value;

                if (prod == 29) // Chit Fund
                {
                    var routeSubs = context.chitsubscriptions
                        .Where(c => c.RouteID == selRouteId)
                        .Select(c => c.SubscriptionID)
                        .ToList();

                    subsqry = subsqry
                        .Where(s => s.subscriptionid.HasValue && routeSubs.Contains(s.subscriptionid.Value))
                        .ToList();
                }
                else if (prod == 30) // Loan
                {
                    var routeSubs = context.loansubscription
                        .Where(l => l.RouteID == selRouteId)
                        .Select(l => l.LoanSubscriptionID)
                        .ToList();

                    subsqry = subsqry
                        .Where(s => s.subscriptionid.HasValue && routeSubs.Contains(s.subscriptionid.Value))
                        .ToList();
                }
            }

            var result = subsqry.Select(x => new
            {
                SubscriptionID = x.subscriptionid,
                SubscriptionCode = x.subscriptioncode
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeRoutes(int empId)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0)
            {
                return Json(new object[] { }, JsonRequestBehavior.AllowGet);
            }

            var routeIds = context.companyrouteemployees
                .Where(r => r.EmployeeID == empId && r.StatusTypeID == 0)
                .Select(r => r.RouteID)
                .Distinct()
                .ToList();

            if (routeIds.Count == 0)
            {
                return Json(new object[] { }, JsonRequestBehavior.AllowGet);
            }

            var routes = context.companyroutes
                .Where(r => routeIds.Contains(r.RouteID))
                .OrderBy(r => r.RouteName)
                .Select(r => new { RouteID = r.RouteID, RouteName = r.RouteName })
                .ToList();

            return Json(routes, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveBulk(FormCollection formcoll)
        {
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0 || Session["CUSRID"].ToString() == "")
            {
                return RedirectToAction("Login", "Account");
            }

            string status = "";

            using (var db = new AppDbContext())
            {
                using (var trans1 = db.Database.BeginTransaction())
                {
                    try
                    {
                        string bulkItems = Convert.ToString(formcoll["BulkItems"]);
                        if (string.IsNullOrWhiteSpace(bulkItems))
                        {
                            status = "Error";
                            return Json(status, JsonRequestBehavior.AllowGet);
                        }

                        int productTypeId = 0;
                        if (!string.IsNullOrEmpty(formcoll["ProductTypeID"]))
                        {
                            productTypeId = Convert.ToInt32(formcoll["ProductTypeID"]);
                        }

                        int subsTranCategory = 0;
                        if (!string.IsNullOrEmpty(formcoll["SubsTranCategory"]))
                        {
                            subsTranCategory = Convert.ToInt32(formcoll["SubsTranCategory"]);
                        }

                        DateTime generatedDate = DateTime.Now.Date;
                        if (!string.IsNullOrEmpty(formcoll["GeneratedDate"]))
                        {
                            generatedDate = Convert.ToDateTime(formcoll["GeneratedDate"]);
                        }

                        int statusTypeId = 0;
                        if (!string.IsNullOrEmpty(formcoll["StatusTypeID"]))
                        {
                            statusTypeId = Convert.ToInt32(formcoll["StatusTypeID"]);
                        }

                        string remarks = Convert.ToString(formcoll["Remarks"]);

                        int assignedToEmployeeId = 0;
                        if (!string.IsNullOrEmpty(formcoll["AssignedToEmployeeID"]))
                        {
                            assignedToEmployeeId = Convert.ToInt32(formcoll["AssignedToEmployeeID"]);
                        }

                        int headerRouteId = 0;
                        if (!string.IsNullOrEmpty(formcoll["RouteID"]))
                        {
                            headerRouteId = Convert.ToInt32(formcoll["RouteID"]);
                        }

                        string cusrid = Session["CUSRID"].ToString();

                        string[] rows = bulkItems.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var row in rows)
                        {
                            string[] parts = row.Split('~');
                            if (parts.Length != 3)
                            {
                                continue;
                            }

                            int subscriptionId = 0;
                            int modeId = 0;
                            double amount = 0;

                            if (!int.TryParse(parts[0], out subscriptionId))
                            {
                                continue;
                            }
                            if (!int.TryParse(parts[1], out modeId))
                            {
                                continue;
                            }
                            if (!double.TryParse(parts[2], out amount))
                            {
                                continue;
                            }
                            if (amount <= 0)
                            {
                                continue;
                            }

                            TransactionCollectionList master = new TransactionCollectionList();

                            master.ProductTypeID = productTypeId;
                            master.ProductID = subscriptionId;

                            if (assignedToEmployeeId > 0)
                            {
                                master.AssignedToEmployeeID = assignedToEmployeeId;
                            }

                            if (headerRouteId > 0)
                            {
                                master.RouteID = headerRouteId;
                            }

                            int trantypeid = 0;
                            if (master.ProductTypeID == 29)
                            {
                                ChitSubscription chts = db.chitsubscriptions.Find(master.ProductID);
                                if (chts != null)
                                {
                                    trantypeid = Convert.ToInt32(chts.CollectionTypeID);
                                }
                            }
                            else if (master.ProductTypeID == 30)
                            {
                                LoanSubscription ls = db.loansubscription.Find(master.ProductID);
                                if (ls != null)
                                {
                                    trantypeid = Convert.ToInt32(ls.CollectionType);
                                }
                            }
                            else
                            {
                                trantypeid = 342;
                            }
                            master.TransactionTypeTypeID = trantypeid;

                            master.TransactionCategoryTypeID = subsTranCategory;
                            master.TransactionModeTypeID = modeId;
                            master.GeneratedDate = generatedDate;

                            master.GSTReq = 0;
                            master.GSTPerc = 0;
                            master.GSTAmt = 0;
                            master.GrossAmt = 0;

                            master.Remarks = remarks;
                            master.Installment = null;

                            master.GeneratedAmount = amount;
                            master.StatusTypeID = statusTypeId;

                            master.collectionagent = cusrid;

                            db.trncolnlst.Add(master);
                            db.SaveChanges();

                            string updqry = "exec pr_Update_Collection_Receipt_Allocation @collectionlstid = " + master.CollectionListID + ",@status =" + master.StatusTypeID;
                            db.Database.ExecuteSqlCommand(updqry);
                        }

                        trans1.Commit();
                        status = "Success";
                        return Json(status, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception)
                    {
                        trans1.Rollback();
                        status = "Error";
                        return Json(status, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }
    }
}

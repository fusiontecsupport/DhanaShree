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

namespace CHITERP.Controllers.Products
{
    public class LoanRequestManagementController : Controller
    {
        // GET: LoanRequestManagement

        #region context 
        AppDbContext context = new AppDbContext();
        #endregion

        #region Index
        //[Authorize(Roles = "LoanRequestManagementIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            return View();
        }
        #endregion

        #region Edit
        //[Authorize(Roles = "LoanRequestManagementEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/LoanRequestManagement/Form/" + id);

                //Response.Redirect("/LoanRequestManagement/Form/?id=" + id);
            }
        }
        #endregion

        #region Form
        //[Authorize(Roles = "LoanRequestManagementCreate")]
        public ActionResult Form(string id = "0")
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            LoanRequest tab = new LoanRequest();

            int LoanRequestID = 0;

            if (id == "" || id == null || id == "0")
            { LoanRequestID = 0; }
            else { LoanRequestID = Convert.ToInt32(id); }

            ViewBag.OrganizationID = new SelectList("");
            ViewBag.PersonID = new SelectList("");

            ViewBag.LoanSchemeID = new SelectList("");
            ViewBag.LoanSecuredTypeId = new SelectList("");
            ViewBag.DisbursementModeTypeID = new SelectList("");
            ViewBag.RepaymentModeTypeID = new SelectList("");

            ViewBag.DeductionTypeId = new SelectList("");
            ViewBag.TermTypeID = new SelectList("");
            ViewBag.LinkedOfficeID = new SelectList("");
            ViewBag.CommissionAgentPersonID = new SelectList("");
            ViewBag.AccountEmployeeID = new SelectList("");

            List<SelectListItem> StatusType = new List<SelectListItem>();
            SelectListItem selectedSType = new SelectListItem { Text = "Active", Value = "0", Selected = true };
            StatusType.Add(selectedSType);
            selectedSType = new SelectListItem { Text = "InActive", Value = "1", Selected = false };
            StatusType.Add(selectedSType);
            ViewBag.StatusTypeID = StatusType;

            if (LoanRequestID > 0)
            {
                tab = context.loanrequest.Find(LoanRequestID);

                StatusType = new List<SelectListItem>();
                if (tab.StatusTypeID == 0)
                {
                    selectedSType = new SelectListItem { Text = "Active", Value = "0", Selected = true };
                    StatusType.Add(selectedSType);
                    selectedSType = new SelectListItem { Text = "InActive", Value = "1", Selected = false };
                    StatusType.Add(selectedSType);
                    ViewBag.StatusTypeID = StatusType;
                }
                else
                {
                    selectedSType = new SelectListItem { Text = "Active", Value = "0", Selected = false };
                    StatusType.Add(selectedSType);
                    selectedSType = new SelectListItem { Text = "InActive", Value = "1", Selected = true };
                    StatusType.Add(selectedSType);
                    ViewBag.StatusTypeID = StatusType;
                }                
            }
            else
            {
                tab = null;
            }

            return View(tab);
        }
        #endregion

        #region Loan Request SaveData
        public ActionResult SaveData(FormCollection fdata)
        {
            string status = "";
            try
            {

            }
            catch (Exception ex)
            {

            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
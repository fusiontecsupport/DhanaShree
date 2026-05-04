using CHITERP.Data;
using CHITERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Controllers.Masters
{
    public class AccountHeadMasterController : Controller
    {
        #region context 
        AppDbContext context = new AppDbContext();
        #endregion
        // GET: /AccountHeadMaster/
        [Authorize(Roles = "AccountHeadMasterIndex")]
        public ActionResult Index()
        {
            return View(context.accountheadmasters.ToList());//Loading Grid
        }
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new FinFusionEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
                var data = e.pr_SearchAccountHeadMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);
                var aaData = data.Select(d => new { ACHEADCODE = d.ACHEADCODE, ACHEADGDESC = d.ACHEADGDESC, ACHEADDESC = d.ACHEADDESC, DISPSTATUS = d.DISPSTATUS.ToString(), ACHEADID = d.ACHEADID.ToString() }).ToArray();
                return Json(new
                {
                    //sEcho = param.sEcho,
                    data = aaData
                    //iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    //iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //-------------Initializing Form-------------//
        [Authorize(Roles = "AccountHeadMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            AccountHeadMaster tab = new AccountHeadMaster();
            ViewBag.ACHEADGID = new SelectList(context.accountgroupmasters, "ACHEADGID", "ACHEADGDESC");

            //bill wise
            List<SelectListItem> selectedACHEADTYPE = new List<SelectListItem>();
            SelectListItem selectedATypeItem = new SelectListItem { Text = "No", Value = "1", Selected = true };
            selectedACHEADTYPE.Add(selectedATypeItem);
            selectedATypeItem = new SelectListItem { Text = "Yes", Value = "2", Selected = false };
            selectedACHEADTYPE.Add(selectedATypeItem);
            ViewBag.ACHEADTYPE = selectedACHEADTYPE;

            //display status
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            tab.ACHEADID = 0;
            // IMP
            if (id == -1)
                ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";
            if (id != 0 && id != -1)  // IMP
            {
                tab = context.accountheadmasters.Find(id);
                ViewBag.ACHEADGID = new SelectList(context.accountgroupmasters, "ACHEADGID", "ACHEADGDESC", tab.ACHEADGID);

                List<SelectListItem> selectedACHEADTYPE1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.ACHEADTYPE) == 1)
                {
                    SelectListItem selectedAType1 = new SelectListItem { Text = "No", Value = "1", Selected = true };
                    selectedACHEADTYPE1.Add(selectedAType1);
                    selectedAType1 = new SelectListItem { Text = "Yes", Value = "2", Selected = false };
                    selectedACHEADTYPE1.Add(selectedAType1);
                    ViewBag.ACHEADTYPE = selectedACHEADTYPE1;
                }
                else
                {
                    SelectListItem selectedAType1 = new SelectListItem { Text = "No", Value = "1", Selected = false };
                    selectedACHEADTYPE1.Add(selectedAType1);
                    selectedAType1 = new SelectListItem { Text = "Yes", Value = "2", Selected = true };
                    selectedACHEADTYPE1.Add(selectedAType1);
                    ViewBag.ACHEADTYPE = selectedACHEADTYPE1;
                }

                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem1 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem1);
                    selectedItem1 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem1);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
            }
            return View(tab);
        }//--End of Form
         //-----------------Imsert or Modify data------------------//
        #region SaveData
        [HttpPost]
        public ActionResult savedata(AccountHeadMaster tab)
        {


            tab.PRCSDATE = DateTime.Now;
            tab.ACHEADTYPE = 0;
            var s = tab.ACHEADDESC;//...ProperCase
            s = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());//end
            tab.ACHEADDESC = s;
            if ((tab.ACHEADID).ToString() != "0")
            {
                tab.LMUSRID = Session["CUSRID"].ToString();
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                tab.CUSRID = Session["CUSRID"].ToString();
                tab.LMUSRID = "";
                context.accountheadmasters.Add(tab);
                context.SaveChanges();
            }

            string status = "Success";
            return Json(status, JsonRequestBehavior.AllowGet);

        }//---------End  
        #endregion
        //-----------------------------Delete Record---//
        [Authorize(Roles = "AccountHeadMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            AccountHeadMaster accountheadmasters = context.accountheadmasters.Find(Convert.ToInt32(id));
            context.accountheadmasters.Remove(accountheadmasters);
            context.SaveChanges();
            Response.Write("Deleted Successfully ...");
        }
    }//--End of class
}
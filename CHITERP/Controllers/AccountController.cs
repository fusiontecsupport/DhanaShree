using CHITERP.Models;
using CHITERP.Data;
using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Data.Entity.Validation;

namespace CHITERP.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account


        AppDbContext _db = new AppDbContext();

        public ApplicationSignInManager _signInManager;
        public AppUserManager _userManager;
        public AppRoleManager _roleManager;

        public AccountController()
        {
        }

        
        public AccountController(AppUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }




        public AppRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<AppRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public AppUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
     

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
      


        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            Response.Cookies.Clear();
            AppDbContext context = new AppDbContext();

            
            

            Session["UserIdentfier"] = null;

            Session["DEPTID"] = "";
            Session["DEPTNAME"] = "";
            Session["CUSRID"] = "";
            Session["EMPLNAME"] = "";
            Session["EMPLID"] = "";
            Session["REGNID"] = "";
            //Session["F_BRNCHID"] = "";
            //Session["F_BRNCHNAME"] = "";
            //Session["F_DBRNCHID"] = "";

            Session["SMTPNAME"] = "";
            Session["SMTPPORT"] = "";
            Session["POP3NAME"] = "";
            Session["POP3PORT"] = "";

            Session["CUSTVIEW"] = "";

            Session["USEREMAIL"] = "";
            Session["EMAILPWD"] = "";

            Session["F_DEPTNAME"] = "";
            Session["BRNCHCTYPE"] = "";
            Session["COMPID"] = "";
            //Session["S_BRNCHID"] = "";
            Session["Group"] = "";
            Session["COMPNAME"] = "";
            Session["COMP_LUT_NO"] = "";
            Session["grntranrefid"] = "0";

            Session["F_PROD_LINE_SPEED"] = "0";
            Session["F_MESSAGE_HEIGHT"] = "0";
            Session["F_LINE_HEIGHT"] = "0";
            Session["F_MESSAGE_WIDTH"] = "0";

            Session["POLICYNO"] = "";
            Session["POLICYDATE"] = "";

            Session["Overdue"] = "0";
            Session["amcduedays"] = "45";
            Session["amcenddate"] = DateTime.Now.ToString("dd/MM/yyyy");
            Session["amcrate"] = "0";

            Session["DB_COMPYID"] = "";
            Session["DB_MONTHID"] = "0";
            Session["GMapsAPIKey"] = "";

            Session["EmplImageFileName"] = "noImg.png";

            ViewBag.COMPID = new SelectList(context.companymasters, "COMPID", "COMPNAME");
            Session["LDATE"] = DateTime.Now.ToString("dd-MM-yyyy");
            Session["GYrDesc"] = (DateTime.Now.Year - 1) + " - " + (DateTime.Now.Year);
            //ViewBag.COMPYID = new SelectList(context.VW_ACCOUNTING_YEAR_DETAIL_ASSGN.OrderByDescending(m => m.YRDESC), "COMPYID", "YRDESC");

            //Response.AddHeader("Cache-Control", "no-cache, no-store, max-age=0, must-revalidate");
            //Response.AddHeader("Expires", "Fri, 01 Jan 1990 00:00:00 GMT");
            //Response.AddHeader("Pragma", "no-cache");
            
            if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
                returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);

            if (!string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnUrl = returnUrl;
            }
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            AppDbContext context = new AppDbContext();


            ViewBag.COMPID = new SelectList(context.companymasters, "COMPID", "COMPNAME");
            ViewBag.COMPYID = new SelectList(context.VW_ACCOUNTING_YEAR_DETAIL_ASSGN.OrderByDescending(m => m.YRDESC), "COMPYID", "YRDESC");

            const string roleName = "Admin";
            var role = new AppRole(roleName, roleName);
            var roleresult = await RoleManager.CreateAsync(role);

            var userchk = context.Database.SqlQuery<int>("Select CateId From View_User_Diable_Chk_For_Login Where UserName = '" + model.UserName + "' And DispStatus = 0").ToList();
            if (userchk.Count > 0)
            {
                //code start
                if (ModelState.IsValid)
                {
                    var user = await UserManager.FindAsync(model.UserName, model.Password);

                    var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: true);

                    switch (result)
                    {
                        case SignInStatus.Success:
                            var userroles = await UserManager.GetRolesAsync(user.Id);
                            context.Database.ExecuteSqlCommand("Update AspNetUsers Set NPassword = '" + model.Password + "' where id ='" + user.Id + "'");

                            Session["UserIdentfier"] = user.EmpId;

                            Session["compyid"] = model.COMPYID;
                            Session["CUSRID"] = model.UserName.ToLower();
                            Session["COMPID"] = model.COMPID;
                            Session["EMPLNAME"] = user.FirstName;
                            Session["EMPLID"] = user.EmpId;
                            //Signature Value
                            Session["SIGNATURE"] = user.Signature;
                            //Session["F_BRNCHID"] = "2";// user.EmpId;
                            //Session["F_BRNCHNAME"] = "HO";// user.FirstName;
                            //Session["F_DBRNCHID"] = "2";// user.DEmpId;
                            //Session["F_DEPTNAME"] = "ADMIN";// user.DeptName;
                            //Session["S_BRNCHID"] = "0";// user.EmpId;
                            Session["BRNCHCTYPE"] = "0";// brnchctype[0];// model.BRNCHCTYPE;
                            Session["DEPTID"] = "2";// deptid[0];
                            Session["DEPTNAME"] = "ADMIN";// deptdesc[0];
                            Session["COMPNAME"] = "FinFusion";
                            var compdtl = context.Database.SqlQuery<CompanyMaster>("Select * From CompanyMaster (nolock) where  DispStatus = 0").ToList();
                            Session["COMPNAME"] = "FinFusion";
                            if (compdtl.Count > 0)
                            {
                                Session["COMPNAME"] = compdtl[0].COMPNAME.ToString();
                                Session["COMPCODE"] = compdtl[0].COMPCODE.ToString();
                            }
                            Session["grntranrefid"] = "0";

                            Session["USEREMAIL"] = user.Email;
                            Session["EMAILPWD"] = user.EPassword;


                            DateTime atd = Convert.ToDateTime(DateTime.Now).Date;
                            Session["AttendanceDate"] = atd.ToString("yyyy-MM-dd");


                            Session["CUSTVIEW"] = "admin";


                            var filenameextn = user.EmpId.ToString() + ".jpg";
                            var absolutePath = Server.MapPath("~/Content/EmployeeImages/") + filenameextn;// @Url.Content("~/Content/EmployeeImages/" + filename + ".jpg");
                            if (System.IO.File.Exists(absolutePath))
                            {
                                Session["EmplImageFileName"] = filenameextn;
                            }
                            else
                            {
                                Session["EmplImageFileName"] = "noImg.png";
                            }



                            Session["LDATE"] = Request.Form.Get("LDATE"); var COMPID = Request.Form.Get("COMPID");
                            DateTime TmpDate = Convert.ToDateTime(Request.Form.Get("LDATE")).Date;
                            var LMNTH = TmpDate.Month; var LYR = TmpDate.Year; var PFYear = 0; var PTYear = 0; var PFDATE = ""; var PTDATE = ""; var GYrDesc = "";

                            if (LMNTH >= 4)
                            {
                                PFYear = LYR;
                                PTYear = LYR + 1;
                                PFDATE = "01/04/" + PFYear; PTDATE = "31/03/" + PTYear;
                                GYrDesc = PFYear + " - " + PTYear;


                            }
                            else
                            {
                                PFYear = LYR - 1;
                                PTYear = LYR;
                                PFDATE = "01/04/" + PFYear; PTDATE = "31/03/" + PTYear;
                                GYrDesc = PFYear + " - " + PTYear;
                            }


                            DateTime sd = Convert.ToDateTime(PFDATE).Date;
                            DateTime startDate = new DateTime(sd.Year, 4, 1);

                            DateTime sd1 = DateTime.Now.Date;
                            DateTime startDate1 = new DateTime(sd1.Year, sd1.Month, sd1.Day);

                            Session["SDATE"] = sd1.ToString("yyyy-MM-dd");
                            Session["EDATE"] = sd1.ToString("yyyy-MM-dd");

                            DateTime ed = Convert.ToDateTime(PTDATE).Date;
                            DateTime endDate = new DateTime(ed.Year, 3, 31);


                            Session["FINSDATE"] = sd.ToString("yyyy-MM-dd");
                            Session["FINEDATE"] = ed.ToString("yyyy-MM-dd");


                            Session["DBSDATE"] = sd.ToString("yyyy-MM-dd");
                            Session["DBEDATE"] = ed.ToString("yyyy-MM-dd");



                            Session["GMapsAPIKey"] = ConfigurationManager.AppSettings["GMapsAPIKey"];

                            List<int> Max_YrId = new List<int>();
                            var ACCYR_QRY = context.Database.SqlQuery<PR_ACCOUNTINGYEAR_ID_CHK_Result>("PR_ACCOUNTINGYEAR_ID_CHK @PFYear=" + PFYear + ",@PTYear=" + PTYear + "").ToList();
                            if (ACCYR_QRY.Count == 0)
                            {
                                context.Database.ExecuteSqlCommand("INSERT INTO AccountingYear (  YrDesc, FDate, TDate, CUSRID, PRCSDATE ) VALUES  ( '" + GYrDesc + "', '" + Convert.ToDateTime(PFDATE).ToString("MM/dd/yyyy") + "', '" + Convert.ToDateTime(PTDATE).ToString("MM/dd/yyyy") + "', '" + Session["CUSRID"] + "', '" + DateTime.Now.ToString("MM-dd-yyyy") + "')");
                                Max_YrId = context.Database.SqlQuery<Int32>("SELECT MAX(YRID) FROM AccountingYear").ToList();
                            }
                            else
                            {
                                var ROW = ACCYR_QRY.Count - 1;
                                Max_YrId.Add(ACCYR_QRY[ROW].YRID);
                            }

                            var GCID = context.Database.SqlQuery<int>("select isnull(MAX(COMPYID),0) from CompanyAccountingDetail").ToList();

                            if(COMPID==""|| COMPID == null) { COMPID ="1"; }

                            string sqry = "Exec PR_COMPANYACCOUNTINGDETAIL_ID_CHK @PCompId=" + Convert.ToInt32(COMPID) + ",@PYrId=" + Convert.ToInt32(Max_YrId[0]) + "";
                            var COMPDTL_QRY = context.Database.SqlQuery<PR_COMPANYACCOUNTINGDETAIL_ID_CHK_Result>(sqry).ToList();
                            if (COMPDTL_QRY.Count == 0 || GCID[0] == 0)
                            {
                                context.Database.ExecuteSqlCommand("INSERT INTO CompanyAccountingDetail ( CompId, YrId,  CUSRID, PRCSDATE ) VALUES  ( " + COMPID + ", " + Convert.ToInt32(Max_YrId[0]) + ",  '" + Session["CUSRID"] + "', '" + DateTime.Now.ToString("MM-dd-yyyy") + "')");
                                // GCID = context.Database.SqlQuery<int>("select MAX(COMPYID) from CompanyAccountingDetail").ToList();
                                System.Web.HttpContext.Current.Session["compyid"] = Convert.ToInt32(GCID[0] + 1);
                            }
                            else
                            {
                                System.Web.HttpContext.Current.Session["compyid"] = Convert.ToInt32(COMPDTL_QRY[0].COMPYID);
                            }

                            Session["GYrDesc"] = GYrDesc;

                            //SMTP
                            var smtpsql = context.Database.SqlQuery<CompanyMaster>("select * from CompanyMaster").ToList();
                            if (smtpsql.Count != 0)
                            {
                                Session["SMTPNAME"] = "";
                                Session["SMTPPORT"] = "";
                                Session["POP3NAME"] = "";
                                Session["POP3PORT"] = "";
                                Session["COMP_LUT_NO"] = "";
                                if (smtpsql[0].COMP_SMTP != null)
                                    Session["SMTPNAME"] = smtpsql[0].COMP_SMTP.ToString();
                                if (smtpsql[0].COMP_SMTP_PORT != null)
                                    Session["SMTPPORT"] = smtpsql[0].COMP_SMTP_PORT.ToString();
                                if (smtpsql[0].COMP_POP3 != null)
                                    Session["POP3NAME"] = smtpsql[0].COMP_POP3.ToString();
                                if (smtpsql[0].COMP_POP3_PORT != null)
                                    Session["POP3PORT"] = smtpsql[0].COMP_POP3_PORT.ToString();
                                if (smtpsql[0].COMP_LUT_NO != null)
                                    Session["COMP_LUT_NO"] = smtpsql[0].COMP_LUT_NO.ToString();
                            }

                            //SMTP

                            //var sql = context.Database.SqlQuery<int>("select GroupId from ApplicationUserGroups inner join AspNetUsers on AspNetUsers.Id=ApplicationUserGroups.UserId where AspNetUsers.UserName='" + model.UserName + "'").ToList();

                            //if (sql[0].Equals(1)) { Session["Group"] = "Admin"; }
                            //if (sql[0].Equals(2)) { Session["Group"] = "SuperAdmin"; }
                            //if (sql[0].Equals(4)) { Session["Group"] = "Users"; }
                            //if (sql[0].Equals(3)) { Session["Group"] = "Manager"; }

                            var sql = context.Database.SqlQuery<VW_USER_DETAILS>("select * from VW_USER_DETAILS Where UserName='" + model.UserName + "'").ToList();
                            if (sql.Count == 0)
                            {
                                Session["Group"] = "";
                            }
                            else
                            {
                                if (sql.Count > 1)
                                { Session["Group"] = sql[1].GroupName; }
                                else
                                { Session["Group"] = sql[0].GroupName; }

                            }
                            var aa = Session["EMPLID"].ToString();
                            var emplid = 0;
                            if (aa != "") { emplid = Convert.ToInt32(Session["EMPLID"]); }
                            var rsql = context.Database.SqlQuery<EmployeeMaster>("select * from EmployeeMaster Where CATEID = '" + emplid + "'").ToList();
                            if (rsql.Count > 0)
                            {
                                Session["REGNID"] = rsql[0].REGNID;

                            }
                            else
                            {
                                Session["REGNID"] = "0";
                            }

                            //var rsqly = context.Database.SqlQuery<EmployeeYardMappingDetail>("Select *From EMPLOYEEYARDMAPPINGDETAIL Where EMPID = '" + emplid + "'").ToList();
                            //if (rsqly.Count > 0)
                            //{
                            //    Session["YRDID"] = rsqly[0].YRDID;
                            //}
                            //{
                            //    Session["YRDID"] = "0";
                            //}

                            //

                            Session["EXCLPATH"] = "D:\\KGK_EXCEL\\" + Session["CUSRID"];

                            context.Database.ExecuteSqlCommand("delete from menurolemaster where Roles='" + model.UserName + "'");
                            context.Database.ExecuteSqlCommand("EXEC pr_USER_MENU_DETAIL_ASSGN @PKUSRID='" + model.UserName + "'");

                            return RedirectToLocal(returnUrl);
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            return View(model);
                    }


                }
                else
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { x.Key, x.Value.Errors })
                        .ToArray();

                    return View(model);
                }
                //code end
            }
            else

            {
                var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { x.Key, x.Value.Errors })
                        .ToArray();
                ModelState.AddModelError("", "Invalid username or password.");
            }
            return View(model);

        }



        //
        // POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);
            //AuthenticationManager.SignOut();
            
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            Response.Cookies.Clear();
            return RedirectToAction("Login", "Account");
        }

      
        private ActionResult RedirectToLocal(string returnUrl)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(returnUrl, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (result)
            {
                return Redirect(returnUrl);
            }
            else
                return RedirectToAction("Index", "Home");
        }

       
        public ActionResult Index()
        {
            var users = _db.Users.OrderBy(X => X.UserName);
            var model = new List<AccountViewModel.EditUserViewModel>();
            foreach (var user in users)
            {
                var u = new AccountViewModel.EditUserViewModel(user);
                // if(u.UserName==Session["CUSRID"].ToString())
                model.Add(u);
            }
            return View(model);
        }



        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.EmpId = new SelectList(_db.employeemasters.OrderBy(x => x.CATENAME), "CATEID", "CATENAME").ToList();
            
            return View();
        }


        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = model.GetUser();
                user.Avatar = "";
                user.DeptId = 0;
                user.DeptName = "";
                user.Designation = ""; 
                try
                {
                    var store = new UserStore<AppUser, AppRole, string, AppUserLogin, AppUserRole, AppUserClaim>(new AppDbContext());
                    AppUserManager userManager = new AppUserManager(store);

                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { x.Key, x.Value.Errors })
                        .ToArray();



                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToAction("Index", "Home");
                    }
                    AddErrors(result);
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);

                            ModelState.AddModelError("", ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }

            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
            if (user != null)
            {
                ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: false, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        

        ////
        //// POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
        //            ViewBag.Link = callbackUrl;
        //            return View("DisplayEmail");
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.UserName);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                ViewBag.Link = callbackUrl;
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        //    [Authorize(Roles = "Admin, CanEditUser")]
        //public ActionResult UserGroups(string id)
        //{
        //    var user = _db.Users.First(u => u.UserName == id);
        //    var model = new AccountViewModel.SelectUserGroupsViewModel(user);
        //    return View(model);
        //}

        


        //// [Authorize(Roles = "Admin, CanEditRole, CanEditGroup, User")]
        //public ActionResult UserPermissions(string id)
        //{
        //    var user = _db.Users.First(u => u.UserName == id);
        //    var model = new AccountViewModel.UserPermissionsViewModel(user);
        //    return View(model);
        //}


        // [Authorize(Roles = "Admin, CanEditUser")]
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //  [Authorize(Roles = "Admin, CanEditUser")]
        public async Task<ActionResult> Manage(AccountViewModel.ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        //string cname = ViewBag.ReturnUrl;
                        //var zname = cname.Split('/');
                        //var xname = model.NPassword;// zname[3];

                        //_db.Database.ExecuteSqlCommand("Update AspNetUsers Set NPassword = '" + model.NewPassword + "', EPassword  = '" + model.EPassword + "' WHERE UserName = '" + xname + "'");
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public ActionResult ResetUserPassword(ResetPasswordMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ResetPasswordMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ResetPasswordMessageId.SetPasswordSuccess ? "Your password has been reset successfully."
                : message == ResetPasswordMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ResetPasswordMessageId.Error ? "An error has occurred."
                : "";
            var username = Request.Url.Segments.Last();// Request.QueryString["id"];
            var user = UserManager.FindByName(username);
            ViewBag.HasLocalPassword = HasPassword(username);
            ViewBag.ReturnUrl = Url.Action("ResetPassword");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin, SuperAdmin, CanEditUser")]
        public async Task<ActionResult> ResetUserPassword(AccountViewModel.ResetPasswordUserViewModel model)
        {

            // string token = await UserManager.GeneratePasswordResetTokenAsync(user);
            //    return await userManager.ResetPasswordAsync(user, token, password);
            if (ModelState.IsValid)
            {
                var username = Request.Url.Segments.Last();// Request.QueryString["id"];

                var user = UserManager.FindByName(username);

                await UserManager.RemovePasswordAsync(user.Id);
                model.NPassword = model.NewPassword;
                await UserManager.AddPasswordAsync(user.Id, model.NewPassword);
                return RedirectToAction("ResetUserPassword", new { Message = ResetPasswordMessageId.SetPasswordSuccess });
            }
            return View(model);
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetUserPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new AppUser { UserName = model.UserName, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }



        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPassword(string username)
        {
            var user = UserManager.FindByName(username);
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }
        public enum ResetPasswordMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }


        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

               

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        #region EmployeeDetDesgDetails
        public JsonResult EmployeeDetDesgDetails(string id = "0")
        {
            AppDbContext contex = new AppDbContext();
            int EmpId = 0;
            if (id != "" && id != "0")
            {
                EmpId = Convert.ToInt32(id);
            }
            string squery = "Select *from VW_EMPLOYEE_DETAILS(nolock) Where CATEID=" + EmpId + "";

            var data = contex.Database.SqlQuery<Vw_Employee_Details>(squery).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
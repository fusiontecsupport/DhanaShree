using CHITERP.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static CHITERP.Models.AccountViewModel;

namespace CHITERP.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        AppDbContext _db = new AppDbContext();

        public UserController()
        {
        }

        public UserController(AppUserManager userManager,
            AppRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private AppUserManager _userManager;
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

        // Add the Group Manager (NOTE: only access through the public
        // Property, not by the instance variable!)
        private AppGroupManager _groupManager;
        public AppGroupManager GroupManager
        {
            get
            {
                return _groupManager ?? new AppGroupManager();
            }
            private set
            {
                _groupManager = value;
            }
        }

        private AppRoleManager _roleManager;
        public AppRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext()
                    .Get<AppRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }


        public async Task<ActionResult> Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View(await UserManager.Users.ToListAsync());
        }


        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            // Show the groups the user belongs to:
            var userGroups = await this.GroupManager.GetUserGroupsAsync(id);
            ViewBag.GroupNames = userGroups.Select(u => u.Name).ToList();
            return View(user);
        }


        public ActionResult Create()
        {
            List<EmployeeMaster> emp = new List<EmployeeMaster>();
            var emp1 = _db.employeemasters.Where(x => x.DISPSTATUS == 0).ToList();   //_db.employeemasters.OrderBy(x => x.CATENAME), "CATEID", "CATENAME").ToList()

            List<SelectListItem> item8 = new List<SelectListItem>();
            foreach (var c in emp1)
            {
                item8.Add(new SelectListItem
                {
                    Text = c.CATENAME,
                    Value = c.CATEID.ToString()
                });
            }
                        
            ViewBag.EmpId = item8;

            //ViewBag.EmpId = new SelectList(_db.employeemasters.OrderBy(x => x.CATENAME), "CATEID", "CATENAME").ToList();
            ViewBag.DeptId = new SelectList(_db.departmentmasters.OrderBy(x=>x.DEPTDESC),"DEPTID","DEPTDESC").ToList();
            

            // Show a list of available groups:
            ViewBag.GroupsList =
                new SelectList(this.GroupManager.Groups, "Id", "Name");
            return View();
        }

        public ActionResult ManageUser()
        {
            var uname = Session["CUSRID"].ToString();
            var users = _db.Users.Where(X => X.UserName == uname).OrderBy(X => X.UserName);
            var model = new List<AccountViewModel.EditUserViewModel>();
            foreach (var user in users)
            {
                var u = new AccountViewModel.EditUserViewModel(user);
                // if(u.UserName==Session["CUSRID"].ToString())
                model.Add(u);
            }
            return View(model);
        }
        // [Authorize(Roles = "UserPasswordChange")]
        public ActionResult ManageUsers()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            var uname = Session["CUSRID"].ToString();
            var users = _db.Users.OrderBy(X => X.UserName);
            if (Session["CUSRID"].ToString().ToLower() != "admin" && Session["Group"].ToString().ToLower() != "admin" && Session["Group"].ToString().ToLower() != "superadmin")
                users = _db.Users.Where(X => X.UserName == uname).OrderBy(X => X.UserName);
            var model = new List<AccountViewModel.EditUserViewModel>();
            foreach (var user in users)
            {
                var u = new AccountViewModel.EditUserViewModel(user);
                
                model.Add(u);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel,
            params string[] selectedGroups)
        {
            if (ModelState.IsValid)
            {
                var user = userViewModel.GetUser();
                user.Avatar = "";
                user.DeptId = 0;
                user.DeptName = "";
                user.Designation = "";
                var adminresult = await UserManager
                    .CreateAsync(user, userViewModel.Password);

                //Add User to the selected Groups 
                if (adminresult.Succeeded)
                {
                    if (selectedGroups != null)
                    {
                        selectedGroups = selectedGroups ?? new string[] { };
                        await this.GroupManager
                            .SetUserGroupsAsync(user.Id, selectedGroups);
                    }
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Groups = new SelectList(
                await RoleManager.Roles.ToListAsync(), "Id", "Name");
            return View();
        }


        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Display a list of available Groups:
            var allGroups = this.GroupManager.Groups;
            
            var userGroups = await this.GroupManager.GetUserGroupsAsync(id);
            //ViewBag.GroupsList =new SelectList(this.GroupManager.Groups, "Id", "Name");

            var model = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                RolesList = new List<SelectListItem>(),
                GroupsList = new List<SelectListItem>()
            };

            foreach (var group in allGroups)
            {
                var listItem = new SelectListItem()
                {
                    //Group = new SelectListGroup {  Disabled = false, Name= group.Name },
                    Text = group.Name,
                    Value = group.Id,
                    Selected = userGroups.Any(g => g.Id == group.Id)
                };
                
                if (listItem != null)
                    model.GroupsList.Add(listItem);
             
            }
            if (model.GroupsList == null)
                model.GroupsList = new SelectList(this.GroupManager.Groups, "Id", "Name").ToList();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Username,Id,FirstName,LastName,Email")] EditUserViewModel editUser,
            params string[] selectedGroups)
        {
            var errors = ModelState
                       .Where(x => x.Value.Errors.Count > 0)
                       .Select(x => new { x.Key, x.Value.Errors })
                       .ToArray();
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                // Update the User:
                user.UserName = editUser.UserName;
                user.FirstName= editUser.FirstName;
                user.LastName= editUser.LastName;
                user.Email = editUser.Email;
                await this.UserManager.UpdateAsync(user);

                // Update the Groups:
                selectedGroups = selectedGroups ?? new string[] { };
                await this.GroupManager.SetUserGroupsAsync(user.Id, selectedGroups);
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }


        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                // Remove all the User Group references:
                await this.GroupManager.ClearUserGroupsAsync(id);

                // Then Delete the User:
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
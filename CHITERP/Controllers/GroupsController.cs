using CHITERP.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace CHITERP.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class GroupsController : Controller
    {
        private AppDbContext db = new AppDbContext();

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
        private AppUserManager _userManager;
        public AppUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext()
                    .Get<AppUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            return View(this.GroupManager.Groups.ToList());
        }


        public async Task<ActionResult> Details(string id)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationGroups applicationgroup =
                await this.GroupManager.Groups.FirstOrDefaultAsync(g => g.Id == id);
            if (applicationgroup == null)
            {
                return HttpNotFound();
            }
            var groupRoles = this.GroupManager.GetGroupRoles(applicationgroup.Id);
            string[] RoleNames = groupRoles.Select(p => p.Name).ToArray();
            ViewBag.RolesList = RoleNames;
            ViewBag.RolesCount = RoleNames.Count();
            return View(applicationgroup);
        }


        public ActionResult Create()
        {
            //Get a SelectList of Roles to choose from in the View:
            ViewBag.RolesList = new SelectList(
                this.RoleManager.Roles.ToList(), "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "Name,Description")] ApplicationGroups applicationgroup, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                // Create the new Group:
                var result = await this.GroupManager.CreateGroupAsync(applicationgroup);
                if (result.Succeeded)
                {
                    selectedRoles = selectedRoles ?? new string[] { };

                    // Add the roles selected:
                    int rolescount = Convert.ToInt32(selectedRoles.Length);
                    if (rolescount < 50)
                        await this.GroupManager.SetGroupRolesAsync(applicationgroup.Id, 0, selectedRoles);
                    else
                    {
                        for (int i = 0; i < rolescount - 1; i = i + 50)
                        {
                            int arsz = 50;
                            if ((rolescount - i) < 50)
                                arsz = rolescount - i;
                            var sel1 = new string[arsz];
                            Array.Copy(selectedRoles, i, sel1, 0, arsz);
                            await this.GroupManager.SetGroupRolesAsync(applicationgroup.Id, i, sel1);
                        }
                    }
                }
                return RedirectToAction("Index");
            }

            // Otherwise, start over:
            ViewBag.RoleId = new SelectList(
                this.RoleManager.Roles.ToList(), "Id", "Name");
            return View(applicationgroup);
        }


        public async Task<ActionResult> Edit(string id)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationGroups applicationgroup = await this.GroupManager.FindByIdAsync(id);
            if (applicationgroup == null)
            {
                return HttpNotFound();
            }

            // Get a list, not a DbSet or queryable:
            var allRoles = await this.RoleManager.Roles.ToListAsync();
            var allUsers = await this.UserManager.Users.ToListAsync();
            var groupRoles = await this.GroupManager.GetGroupRolesAsync(id);
            var groupUsers = await this.GroupManager.GetGroupUsersAsync(id);

            var model = new GroupViewModel()
            {
                Id = applicationgroup.Id,
                Name = applicationgroup.Name,
                Description = applicationgroup.Description
            };

            // load the roles/Roles for selection in the form:
            foreach (var Role in allRoles)
            {
                var listItem = new SelectListItem()
                {
                    Text = Role.Name,
                    Value = Role.Id,
                    Selected = groupRoles.Any(g => g.Id == Role.Id)
                };
                model.RolesList.Add(listItem);
            }
            // load the roles/Roles for selection in the form:
            foreach (var usr in allUsers)
            {
                var usrlistItem = new SelectListItem()
                {
                    Text = usr.UserName,
                    Value = usr.Id,
                    Selected = groupUsers.Any(g => g.Id == usr.Id)
                };
                model.UsersList.Add(usrlistItem);
            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,Name,Description")] GroupViewModel model, params string[] selectedRoles)
        {
            var group = await this.GroupManager.FindByIdAsync(model.Id);
            if (group == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                group.Name = model.Name;
                group.Description = model.Description;
                await this.GroupManager.UpdateGroupAsync(group);

                selectedRoles = selectedRoles ?? new string[] { };
                
                
                int rolescount = Convert.ToInt32(selectedRoles.Length);
                if (rolescount < 50)
                    await this.GroupManager.SetGroupRolesAsync(group.Id, 0, selectedRoles);
                else
                {
                    for (int i = 0; i < rolescount - 1; i = i + 50)
                    {
                        int arsz = 50;
                        if ((rolescount - i) < 50)
                            arsz = rolescount - i;
                        var sel1 = new string[arsz];
                        Array.Copy(selectedRoles, i, sel1, 0, arsz);
                        await this.GroupManager.SetGroupRolesAsync(group.Id, i, sel1);
                    }
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }


        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationGroups applicationgroup = await this.GroupManager.FindByIdAsync(id);
            if (applicationgroup == null)
            {
                return HttpNotFound();
            }
            return View(applicationgroup);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationGroups applicationgroup = await this.GroupManager.FindByIdAsync(id);
            await this.GroupManager.DeleteGroupAsync(id);
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
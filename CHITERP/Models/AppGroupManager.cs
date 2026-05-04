using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CHITERP.Models
{
    public class AppGroupManager
    {
        private AppGroupStore _groupStore;
        private AppDbContext _db;
        private AppUserManager _userManager;
        private AppRoleManager _roleManager;

        public AppGroupManager()
        {
            _db = HttpContext.Current
                .GetOwinContext().Get<AppDbContext>();
            _userManager = HttpContext.Current
                .GetOwinContext().GetUserManager<AppUserManager>();
            _roleManager = HttpContext.Current
                .GetOwinContext().Get<AppRoleManager>();
            _groupStore = new AppGroupStore(_db);
        }


        public IQueryable<ApplicationGroups> Groups
        {
            get
            {
                return _groupStore.Groups;
            }
        }


        public async Task<IdentityResult> CreateGroupAsync(ApplicationGroups group)
        {
            await _groupStore.CreateAsync(group);
            return IdentityResult.Success;
        }


        public IdentityResult CreateGroup(ApplicationGroups group)
        {
            _groupStore.Create(group);
            return IdentityResult.Success;
        }


        public IdentityResult SetGroupRoles(string groupId, params string[] roleNames)
        {
            // Clear all the roles associated with this group:
            var thisGroup = this.FindById(groupId);
            thisGroup.ApplicationRoles.Clear();
            _db.SaveChanges();

            // Add the new roles passed in:
            var newRoles = _roleManager.Roles.Where(r => roleNames.Any(n => n == r.Name));
            foreach (var role in newRoles)
            {
                thisGroup.ApplicationRoles.Add(new ApplicationGroupRole { ApplicationGroupId = groupId, ApplicationRoleId = role.Id });
            }
            _db.SaveChanges();

            // Reset the roles for all affected users:
            foreach (var groupUser in thisGroup.ApplicationUsers)
            {
                this.RefreshUserGroupRoles(groupUser.ApplicationUserId);
            }
            return IdentityResult.Success;
        }


        public async Task<IdentityResult> SetGroupRolesAsync(string groupId, int itr, params string[] roleNames)
        {
            // Clear all the roles associated with this group:
            var thisGroup = await this.FindByIdAsync(groupId);
            if (itr == 0 && thisGroup.ApplicationRoles.Count > 0)
            {
                thisGroup.ApplicationRoles.Clear(); 
                await _db.SaveChangesAsync();
            }
            

            // Add the new roles passed in:
            var newRoles = _roleManager.Roles.Where(r => roleNames.Any(n => n == r.Name));
            foreach (var role in newRoles)
            {
                thisGroup.ApplicationRoles.Add(new ApplicationGroupRole { ApplicationGroupId = groupId, ApplicationRoleId = role.Id });
            }
            await _db.SaveChangesAsync();

            // Reset the roles for all affected users:
            foreach (var groupUser in thisGroup.ApplicationUsers)
            {
                await this.RefreshUserGroupRolesAsync(groupUser.ApplicationUserId);
            }
            return IdentityResult.Success;
        }


        public async Task<IdentityResult> SetUserGroupsAsync(string userId, params string[] groupIds)
        {
            // Clear current group membership:
            var currentGroups = await this.GetUserGroupsAsync(userId);
            foreach (var group in currentGroups)
            {
                group.ApplicationUsers
                    .Remove(group.ApplicationUsers.FirstOrDefault(gr => gr.ApplicationUserId == userId));
            }
            await _db.SaveChangesAsync();

            // Add the user to the new groups:
            foreach (string groupId in groupIds)
            {
                var newGroup = await this.FindByIdAsync(groupId);
                newGroup.ApplicationUsers.Add(new ApplicationUserGroup { ApplicationUserId = userId, ApplicationGroupId = groupId });
            }
            await _db.SaveChangesAsync();

            await this.RefreshUserGroupRolesAsync(userId);
            return IdentityResult.Success;
        }


        public IdentityResult SetUserGroups(string userId, params string[] groupIds)
        {
            // Clear current group membership:
            var currentGroups = this.GetUserGroups(userId);
            foreach (var group in currentGroups)
            {
                group.ApplicationUsers
                    .Remove(group.ApplicationUsers.FirstOrDefault(gr => gr.ApplicationUserId == userId));
            }
            _db.SaveChanges();

            // Add the user to the new groups:
            foreach (string groupId in groupIds)
            {
                var newGroup = this.FindById(groupId);
                newGroup.ApplicationUsers.Add(new ApplicationUserGroup { ApplicationUserId = userId, ApplicationGroupId = groupId });
            }
            _db.SaveChanges();

            this.RefreshUserGroupRoles(userId);
            return IdentityResult.Success;
        }


        public IdentityResult RefreshUserGroupRoles(string userId)
        {
            var user = _userManager.FindById(userId);
            if (user == null)
            {
                throw new ArgumentNullException("User");
            }
            // Remove user from previous roles:
            var oldUserRoles = _userManager.GetRoles(userId);
            if (oldUserRoles.Count > 0)
            {
                _userManager.RemoveFromRoles(userId, oldUserRoles.ToArray());
            }

            // Find teh roles this user is entitled to from group membership:
            var newGroupRoles = this.GetUserGroupRoles(userId);

            // Get the damn role names:
            var allRoles = _roleManager.Roles.ToList();
            var addTheseRoles = allRoles.Where(r => newGroupRoles.Any(gr => gr.ApplicationRoleId == r.Id));
            var roleNames = addTheseRoles.Select(n => n.Name).ToArray();

            // Add the user to the proper roles
            _userManager.AddToRoles(userId, roleNames);

            return IdentityResult.Success;
        }


        public async Task<IdentityResult> RefreshUserGroupRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentNullException("User");
            }
            // Remove user from previous roles:
            var oldUserRoles = await _userManager.GetRolesAsync(userId);
            if (oldUserRoles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(userId, oldUserRoles.ToArray());
            }

            // Find the roles this user is entitled to from group membership:
            var newGroupRoles = await this.GetUserGroupRolesAsync(userId);

            // Get the damn role names:
            var allRoles = await _roleManager.Roles.ToListAsync();
            var addTheseRoles = allRoles.Where(r => newGroupRoles.Any(gr => gr.ApplicationRoleId == r.Id));
            var roleNames = addTheseRoles.Select(n => n.Name).ToArray();

            // Add the user to the proper roles
            await _userManager.AddToRolesAsync(userId, roleNames);

            return IdentityResult.Success;
        }


        public async Task<IdentityResult> DeleteGroupAsync(string groupId)
        {
            var group = await this.FindByIdAsync(groupId);
            if (group == null)
            {
                throw new ArgumentNullException("User");
            }

            var currentGroupMembers = (await this.GetGroupUsersAsync(groupId)).ToList();
            // remove the roles from the group:
            group.ApplicationRoles.Clear();

            // Remove all the users:
            group.ApplicationUsers.Clear();

            // Remove the group itself:
            _db.ApplicationGroups.Remove(group);

            await _db.SaveChangesAsync();

            // Reset all the user roles:
            foreach (var user in currentGroupMembers)
            {
                await this.RefreshUserGroupRolesAsync(user.Id);
            }
            return IdentityResult.Success;
        }


        public IdentityResult DeleteGroup(string groupId)
        {
            var group = this.FindById(groupId);
            if (group == null)
            {
                throw new ArgumentNullException("User");
            }

            var currentGroupMembers = this.GetGroupUsers(groupId).ToList();
            // remove the roles from the group:
            group.ApplicationRoles.Clear();

            // Remove all the users:
            group.ApplicationUsers.Clear();

            // Remove the group itself:
            _db.ApplicationGroups.Remove(group);

            _db.SaveChanges();

            // Reset all the user roles:
            foreach (var user in currentGroupMembers)
            {
                this.RefreshUserGroupRoles(user.Id);
            }
            return IdentityResult.Success;
        }


        public async Task<IdentityResult> UpdateGroupAsync(ApplicationGroups group)
        {
            await _groupStore.UpdateAsync(group);
            foreach (var groupUser in group.ApplicationUsers)
            {
                await this.RefreshUserGroupRolesAsync(groupUser.ApplicationUserId);
            }
            return IdentityResult.Success;
        }


        public IdentityResult UpdateGroup(ApplicationGroups group)
        {
            _groupStore.Update(group);
            foreach (var groupUser in group.ApplicationUsers)
            {
                this.RefreshUserGroupRoles(groupUser.ApplicationUserId);
            }
            return IdentityResult.Success;
        }


        public IdentityResult ClearUserGroups(string userId)
        {
            return this.SetUserGroups(userId, new string[] { });
        }


        public async Task<IdentityResult> ClearUserGroupsAsync(string userId)
        {
            return await this.SetUserGroupsAsync(userId, new string[] { });
        }


        public async Task<IEnumerable<ApplicationGroups>> GetUserGroupsAsync(string userId)
        {
            var result = new List<ApplicationGroups>();
            var userGroups = (from g in this.Groups
                              where g.ApplicationUsers.Any(u => u.ApplicationUserId == userId)
                              select g).ToListAsync();
            return await userGroups;
        }


        public IEnumerable<ApplicationGroups> GetUserGroups(string userId)
        {
            var result = new List<ApplicationGroups>();
            var userGroups = (from g in this.Groups
                              where g.ApplicationUsers.Any(u => u.ApplicationUserId == userId)
                              select g).ToList();
            return userGroups;
        }


        public async Task<IEnumerable<AppRole>> GetGroupRolesAsync(string groupId)
        {
            var grp = await _db.ApplicationGroups.FirstOrDefaultAsync(g => g.Id == groupId);
            var roles = await _roleManager.Roles.ToListAsync();
            var groupRoles = (from r in roles
                              where grp.ApplicationRoles.Any(ap => ap.ApplicationRoleId == r.Id)
                              select r).ToList();
            return (IEnumerable<AppRole>)groupRoles;
        }


        public IEnumerable<AppRole> GetGroupRoles(string groupId)
        {
            var grp = _db.ApplicationGroups.FirstOrDefault(g => g.Id == groupId);
            var roles = _roleManager.Roles.ToList();
            var groupRoles = from r in roles
                             where grp.ApplicationRoles.Any(ap => ap.ApplicationRoleId == r.Id)
                             select r;
            return (IEnumerable<AppRole>)groupRoles;
        }


        public IEnumerable<AppUser> GetGroupUsers(string groupId)
        {
            var group = this.FindById(groupId);
            var users = new List<AppUser>();
            foreach (var groupUser in group.ApplicationUsers)
            {
                var user = _db.Users.Find(groupUser.ApplicationUserId);
                users.Add(user);
            }
            return users;
        }


        public async Task<IEnumerable<AppUser>> GetGroupUsersAsync(string groupId)
        {
            var group = await this.FindByIdAsync(groupId);
            var users = new List<AppUser>();
            foreach (var groupUser in group.ApplicationUsers)
            {
                var user = await _db.Users
                    .FirstOrDefaultAsync(u => u.Id == groupUser.ApplicationUserId);
                users.Add(user);
            }
            return users;
        }


        public IEnumerable<ApplicationGroupRole> GetUserGroupRoles(string userId)
        {
            var userGroups = this.GetUserGroups(userId);
            var userGroupRoles = new List<ApplicationGroupRole>();
            foreach (var group in userGroups)
            {
                userGroupRoles.AddRange(group.ApplicationRoles.ToArray());
            }
            return userGroupRoles;
        }


        public async Task<IEnumerable<ApplicationGroupRole>> GetUserGroupRolesAsync(string userId)
        {
            var userGroups = await this.GetUserGroupsAsync(userId);
            var userGroupRoles = new List<ApplicationGroupRole>();
            foreach (var group in userGroups)
            {
                userGroupRoles.AddRange(group.ApplicationRoles.ToArray());
            }
            return userGroupRoles;
        }


        public async Task<ApplicationGroups> FindByIdAsync(string id)
        {
            return await _groupStore.FindByIdAsync(id);
        }


        public ApplicationGroups FindById(string id)
        {
            return _groupStore.FindById(id);
        }
    }
}
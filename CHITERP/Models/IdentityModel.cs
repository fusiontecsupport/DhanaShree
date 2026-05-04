using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using CHITERP.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Web.Mvc;

namespace CHITERP.Models
{
    public class IdentityModel
    {
    }
    public class AppUserLogin : IdentityUserLogin<string> { }
    public class AppUserClaim : IdentityUserClaim<string> { }
    public class AppUserRole : IdentityUserRole<string> { }



    public class GroupViewModel
    {
        public GroupViewModel()
        {
            this.UsersList = new List<SelectListItem>();
            this.RolesList = new List<SelectListItem>();
        }
        [Required(AllowEmptyStrings = false)]
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<SelectListItem> UsersList { get; set; }
        public ICollection<SelectListItem> RolesList { get; set; }
    }


    // Must be expressed in terms of our custom UserRole:
    public class AppRole : IdentityRole<string, AppUserRole>
    {
        public AppRole()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public AppRole(string name, string description)
            : this()
        {
            this.Name = name;
            this.Description= description;
        }

        // Add any custom Role properties/code here
        public Nullable<Int16> RMenuGroupOrder { get; set; }
        public string RControllerName { get; set; }
        public string RMenuType { get; set; }
        public string Description { get; set; }
        public string RMenuIndex { get; set; }
        public Nullable<int> ModuleID { get; set; }
        public string RImageClassName { get; set; }
        

    }
    
    public class ApplicationGroups
    {
        public ApplicationGroups()
        {
            this.Id = Guid.NewGuid().ToString();
            //this.ApplicationRoles = new List<ApplicationGroupRole>();
            //this.ApplicationUsers = new List<ApplicationUserGroup>();
        }

        public ApplicationGroups(string name)
            : this()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Name = name;
        }

        public ApplicationGroups(string name, string description)
            : this(name)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Name = name;
            this.Description = description;            
        }

        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        
        public string Description { get; set; }
        //public virtual ICollection<ApplicationGroups> Group{ get; set; }
        public virtual ICollection<ApplicationGroupRole> ApplicationRoles { get; set; }
        public virtual ICollection<ApplicationUserGroup> ApplicationUsers { get; set; }
    }


    public class ApplicationUserGroup
    {
        //public string AppUser_Id { get; set; }
       //public string AppUser_Id1 { get; set; }
        
        public string ApplicationUserId { get; set; }
        public string ApplicationGroupId { get; set; }
    }

    public class ApplicationGroupRole
    {
        public string ApplicationGroupId { get; set; }
        public string ApplicationRoleId { get; set; }
    }
}
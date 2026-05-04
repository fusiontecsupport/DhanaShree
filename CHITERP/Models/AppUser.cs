using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace CHITERP.Models
{
    public class AppUser : IdentityUser<string, AppUserLogin,AppUserRole, AppUserClaim>
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public override string Email { get; set; }
        [Required]
        public int EmpId { get; set; }

        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Display(Name = "Department")]
        public int DeptId { get; set; }

        public string DeptName { get; set; }

        public string NPassword { get; set; }
        public string EPassword { get; set; }

        //public string DisplayName { get; set; }

        public string Avatar { get; set; }

        public string Signature { get; set; }

        public virtual ICollection<ApplicationUserGroup> Groups { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(AppUserManager manager)
        {
            var userIdentity = await manager
                .CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }

    }
}
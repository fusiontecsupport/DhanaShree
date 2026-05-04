using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System;
using System.Web.Mvc;

namespace CHITERP.Models
{
    public class AccountViewModel
    {

        public class ManageUserViewModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage =
                "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage =
                "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string NPassword { get; set; }
            public string EPassword { get; set; }
            //[Required]
            [System.Web.Mvc.AllowHtml]
            [Display(Name = "Email Signature")]
            public string Signature { get; set; }
        }


        public class ResetPasswordUserViewModel
        {

            [Required]
            [StringLength(100, ErrorMessage =
                "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage =
                "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string NPassword { get; set; }
            //public string EPassword { get; set; }
            //[Required]
            //[System.Web.Mvc.AllowHtml]
            //[Display(Name = "Email Signature")]
            //public string Signature { get; set; }
        }
        public class EditUserViewModel
        {
            public EditUserViewModel() { }

            // Allow Initialization with an instance of AppUser:
            public EditUserViewModel(AppUser user)
            {
                this.Id= user.Id;
                this.UserName = user.UserName;
                this.FirstName = user.FirstName;
                this.LastName = user.LastName;
                this.Email = user.Email;
                //this.EmpId = user.EmpId;
                //this.Designation = user.Designation;
                //this.NPassword = user.NPassword;
                //this.EPassword = user.EPassword;
                ////this.DBrnchId = user.DBrnchId;
                //this.DeptName = user.DeptName;
                //this.Signature = user.Signature;
                //this.Avatar = user.Avatar;

                this.RolesList = new List<SelectListItem>();
                this.GroupsList = new List<SelectListItem>();


            }

            public string Id { get; set; }
            [Required(AllowEmptyStrings = false)]
            [Display(Name = "User Name")]

            public string UserName { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            public string Email { get; set; }


            //public string NPassword { get; set; }

            //[Required]
            //public int EmpId { get; set; }

            //[Required]
            //public string Designation { get; set; }

            ////[Required]
            ////public int DeptId { get; set; }

            //[Required]
            //public string DeptName { get; set; }
            //public string EPassword { get; set; }
            //// Infologia Code-26-01-2021- Add Email Signature parameter
            ////[Required]
            //[System.Web.Mvc.AllowHtml]
            //[Display(Name = "Email Signature")]
            //public string Signature { get; set; }

            //public string Avatar { get; set; }
            //[Display(Name = "Avatar")]

            //// We will still use this, so leave it here:
            public ICollection<SelectListItem> RolesList { get; set; }

            // Add a GroupsList Property:
            public ICollection<SelectListItem> GroupsList { get; set; }
        }

        // Wrapper for SelectGroupEditorViewModel to select user group membership:
        public class SelectUserGroupsViewModel
        {
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public List<SelectGroupEditorViewModel> Groups { get; set; }

            public SelectUserGroupsViewModel()
            {
                this.Groups = new List<SelectGroupEditorViewModel>();
            }

            //public SelectUserGroupsViewModel(AppUser user)
            //    : this()
            //{
            //    this.UserName = user.UserName;
            //    this.FirstName = user.FirstName;
            //    this.LastName = user.LastName;

            //    var Db = new AppDbContext();

            //    // Add all available groups to the public list:
            //    var allGroups = Db.Groups;
            //    foreach (var role in allGroups)
            //    {
            //        // An EditorViewModel will be used by Editor Template:
            //        var rvm = new SelectGroupEditorViewModel(role);
            //        this.Groups.Add(rvm);
            //    }

            //    // Set the Selected property to true where user is already a member:
            //    foreach (var group in user.Groups)
            //    {
            //        var checkUserRole =
            //            this.Groups.Find(r => r.GroupName == group.Group.Name);
            //        checkUserRole.Selected = true;
            //    }
            //}
        }

    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "UserName")]        
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public int COMPID { get; set; }

        public int COMPYID { get; set; }
        public DateTime LDATE { get; set; }

        public int EmpID { get; set; }

        public string BRNCHNAME { get; set; }

        public Int16 BRNCHCTYPE { get; set; }
        
    }
    public class RegisterViewModel
    {

        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string NPassword { get; set; }

        [Required]
        public int EmpId { get; set; }


        [Display(Name = "Designation")]
        public string Designation { get; set; }

        public int DeptId { get; set; }

        [Display(Name = "Department")]
        public string DeptName { get; set; }


        [Display(Name = "Email Password")]
        public string EPassword { get; set; }
        public string Avatar { get; set; }

        [System.Web.Mvc.AllowHtml]
        [Display(Name = "Email Signature")]
        public string Signature { get; set; }
        public AppUser GetUser()
        {
            var user = new AppUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = this.UserName,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Email = this.Email,
                NPassword = this.NPassword,
                EPassword = this.EPassword,
                EmpId = this.EmpId,
                Designation = this.Designation,
                DeptId = this.DeptId,
                DeptName = this.DeptName,
                Avatar = this.Avatar,
                Signature = this.Signature

            };
            return user;
        }
    }

    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User Name")]

        public string UserName { get; set; }

        [Required]
        [Display(Name = "Email")]

        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "User Name")]

        public string UserName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

   

    public class ResetPasswordViewModel
    {
        [Required]
        [Display(Name = "User Name")]

        public string UserName { get; set; }

        //[Required]
        //[EmailAddress]
        //[Display(Name = "Email")]
        //public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "User Name")]

        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }



    // Used to display a single role with a checkbox, within a list structure:
    public class SelectRoleEditorViewModel
    {
        public SelectRoleEditorViewModel() { }

        // Update this to accept an argument of type AppRole:
        public SelectRoleEditorViewModel(AppRole role)
        {
            this.RoleName = role.Name;

            // Assign the new Descrption property:
            this.Description = role.Description;
        }

        public bool Selected { get; set; }

        [Required]
        public string RoleName { get; set; }

        // Add the new Description property:
        public string Description { get; set; }
    }


    public class RoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayName("Department")]
        public int ModuleID { get; set; }

        public RoleViewModel() { }
        public RoleViewModel(AppRole role)
        {
            this.Name = role.Name;
            this.Description = role.Description;
        }
    }


    public class EditRoleViewModel
    {
        public string OriginalRoleName { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }

        public EditRoleViewModel() { }
        public EditRoleViewModel(AppRole role)
        {
            this.OriginalRoleName = role.Name;
            this.RoleName = role.Name;
            this.Description = role.Description;
        }
    }


    // Wrapper for SelectGroupEditorViewModel to select user group membership:
    public class SelectUserGroupsViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<SelectGroupEditorViewModel> Groups { get; set; }

        public SelectUserGroupsViewModel()
        {
            this.Groups = new List<SelectGroupEditorViewModel>();
        }

        //public SelectUserGroupsViewModel(AppUser user)
        //    : this()
        //{
        //    this.UserName = user.UserName;
        //    this.FirstName = user.FirstName;
        //    this.LastName = user.LastName;
        ////    public List<SelectGroupEditorViewModel> Groups { get; set; }
        
        //var Db = new AppDbContext();

        //    // Add all available groups to the public list:
        //    var allGroups = this.GroupManager.Groups;
        //    foreach (var role in allGroups)
        //    {
        //        // An EditorViewModel will be used by Editor Template:
        //        var rvm = new SelectGroupEditorViewModel(role);
        //        this.Groups.Add(rvm);
        //    }

        //    // Set the Selected property to true where user is already a member:
        //    foreach (var group in user.Groups)
        //    {
        //        var checkUserRole =
        //            this.Groups.Find(r => r.GroupName == group.ApplicationGroupId);
        //        checkUserRole.Selected = true;
        //    }
        //}
    }
 // Used to display a single role group with a checkbox, within a list structure:
        public class SelectGroupEditorViewModel
    { 
    public SelectGroupEditorViewModel() { }
    public SelectGroupEditorViewModel(ApplicationGroups group)
    {
        this.GroupName = group.Name;
        this.GroupId = group.Id;
    }

    public bool Selected { get; set; }

    [Required]
    public string GroupId { get; set; }
    public string GroupName { get; set; }
}



    public class SelectGroupRolesViewModel
    {
        public SelectGroupRolesViewModel()
        {
            this.Roles = new List<SelectRoleEditorViewModel>();
        }


        // Enable initialization with an instance of AppUser:
        public SelectGroupRolesViewModel(ApplicationGroups Appgroup)
            : this()
        {
            this.GroupId = Appgroup.Id;
            this.GroupName = Appgroup.Name;

            var Db = new AppDbContext();

            // Add all available roles to the list of EditorViewModels:
            var allRoles = Db.Roles.OrderBy(x => x.Name);
            foreach (var role in allRoles)
            {
                // An EditorViewModel will be used by Editor Template:
                var rvm = new SelectRoleEditorViewModel(role);
                this.Roles.Add(rvm);
            }

            // Set the Selected property to true for those roles for 
            // which the current user is a member:
            foreach (var groupRole in Appgroup.ApplicationRoles)
            {
                var checkGroupRole =
                    this.Roles.Find(r => r.RoleName == groupRole.ApplicationRoleId);
                checkGroupRole.Selected = true;
            }
        }

        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public List<SelectRoleEditorViewModel> Roles { get; set; }
    }


    public class UserPermissionsViewModel
    {
        public UserPermissionsViewModel()
        {
            this.Roles = new List<RoleViewModel>();
        }


        // Enable initialization with an instance of AppUser:
        //public UserPermissionsViewModel(AppUser user)
        //    : this()
        //{
        //    this.UserName = user.UserName;
        //    this.FirstName = user.FirstName;
        //    this.LastName = user.LastName;
        //    foreach (var role in user.Roles)
        //    {
        //        var appRole = (AppRole)role.Role;
        //        var pvm = new RoleViewModel(appRole);
        //        this.Roles.Add(pvm);
        //    }
        //}

        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<RoleViewModel> Roles { get; set; }
    }

}
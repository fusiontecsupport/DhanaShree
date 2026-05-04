using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CHITERP.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace CHITERP
{
    public class IdentityConfig
    {
    }

    

    public class AppUserStore : UserStore<AppUser, AppRole, string, AppUserLogin, AppUserRole, AppUserClaim>, IUserStore<AppUser, string>, IDisposable
    {
        public AppUserStore()
            : this(new AppDbContext())
        {
            
            base.DisposeContext = true;
        }
        public AppUserStore(AppDbContext  context) : base(context)
        {
        }
    }

    public class AppRoleStore    : RoleStore<AppRole, string, AppUserRole>,    IQueryableRoleStore<AppRole, string>,    IRoleStore<AppRole, string>, IDisposable
    {
        public AppRoleStore()
            : base(new AppDbContext())
        {
            base.DisposeContext = true;
        }

        public AppRoleStore(AppDbContext context)
            : base(context)
        {
        }
    }

    //public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<AppDbContext> 
    //{
    //    protected override void Seed(AppDbContext context) 
    //    {
    //        InitializeIdentityForEF(context);
    //        base.Seed(context);
    //    }
        
    //    //Create User=admin with password=123456 in the Admin role        
        
    //}
  
    //public class AppUserManager : UserManager<AppUser, string>
    //{
    //    public UserValidator<AppUser> UserValidator { get; private set; }

    //    private PasswordValidator PasswordValidator;

    //    public AppUserManager(IUserStore<AppUser, string> store)
    //        : base(store)
    //    {
    //    }

    //    public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options,
    //        IOwinContext context)
    //    {
    //        var manager = new AppUserManager(new UserStore<AppUser, AppRole, string, AppUserLogin, AppUserRole, AppUserClaim>(context.Get<AppDbContext>()));
    //        // Configure validation logic for usernames
    //        manager.UserValidator = new UserValidator<AppUser>(manager)
    //        {
    //            AllowOnlyAlphanumericUserNames = false,
    //            RequireUniqueEmail = true
    //        };
    //        // Configure validation logic for passwords
    //        manager.PasswordValidator = new PasswordValidator
    //        {
    //            RequiredLength = 6,
    //            RequireNonLetterOrDigit = true,
    //            RequireDigit = true,
    //            RequireLowercase = true,
    //            RequireUppercase = true,
    //        };
    //        // Configure user lockout defaults
    //        manager.UserLockoutEnabledByDefault = true;
    //        manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
    //        manager.MaxFailedAccessAttemptsBeforeLockout = 5;
    //        // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
    //        // You can write your own provider and plug in here.
    //        manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
    //        {
    //            MessageFormat = "Your security code is: {0}"
    //        });
    //        manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
    //        {
    //            Subject = "SecurityCode",
    //            BodyFormat = "Your security code is {0}"
    //        });
    //        manager.EmailService = new EmailService();
    //        manager.SmsService = new SmsService();
    //        var dataProtectionProvider = options.DataProtectionProvider;
    //        if (dataProtectionProvider != null)
    //        {
    //            manager.UserTokenProvider =
    //                new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
    //        }
    //        return manager;
    //    }
    //}
    public class AppUserManager : UserManager<AppUser,string>
    {
        public AppUserManager(IUserStore<AppUser,string> store) : base(store)
        {
        }
        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
        {
            var store = new UserStore<AppUser, AppRole, string, AppUserLogin, AppUserRole, AppUserClaim>(context.Get<AppDbContext>());            
            var manager = new AppUserManager(store);
            manager.DefaultAccountLockoutTimeSpan=TimeSpan.FromMinutes(30);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            //user.LockoutEnabled = true;
            //user.LockoutEndDateUtc = DateTime.UtcNow.AddMinutes(42);
            //await userManager.UpdateAsync(user);

            return manager;
        }
    }


    public class AppRoleManager : RoleManager<AppRole>
    {
        public AppRoleManager(IRoleStore<AppRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static AppRoleManager Create(IdentityFactoryOptions<AppRoleManager> options, IOwinContext context)
        {
            var appRoleManager = new AppRoleManager(new AppRoleStore(context.Get<AppDbContext>()));

            return appRoleManager;
        }
    }

    public class ApplicationSignInManager : SignInManager<AppUser, string>
    {

        public ApplicationSignInManager(AppUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {

        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<AppUserManager>(), context.Authentication);
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }

        Task IIdentityMessageService.SendAsync(IdentityMessage message)
        {
            throw new NotImplementedException();
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
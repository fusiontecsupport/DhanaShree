using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using CHITERP.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace CHITERP
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and role manager to use a single instance per request
            app.CreatePerOwinContext<AppDbContext>(AppDbContext.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.CreatePerOwinContext<AppRoleManager>(AppRoleManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),                
                LogoutPath = new PathString("/Account/LogOff"),
                ExpireTimeSpan = new System.TimeSpan(1, 0, 0),
                SlidingExpiration = true,
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<AppUserManager, AppUser, string>(
                        validateInterval: TimeSpan.FromMinutes(60),
                        regenerateIdentityCallback: (manager, user) => user.GenerateUserIdentityAsync(manager),
                            // Need to add THIS line because we added the third type argument (int) above:
                            getUserIdCallback: (claim) => claim.GetUserId())
                }
                
                
            });
            SetPageCaching();


            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            //// Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            //app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            //// Enables the application to remember the second login verification factor such as phone or email.
            //// Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            //// This is similar to the RememberMe option when you log in.
            //app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(
            //    clientId: "",
            //    clientSecret: "");

        }

        public static void SetPageCaching()
        {
            //Used for setting/disabling page caching
            HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(24));
            HttpContext.Current.Response.Cache.SetValidUntilExpires(true);//false
            HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            //HttpContext.Current.Response.Cache.SetNoStore(); to disable
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Public); //nocache
            TimeSpan ts = new TimeSpan(24, 0, 0);
            HttpContext.Current.Response.Cache.SetMaxAge(ts);
            //HttpContext.Current.Response.ContentType = "image/jpeg"; // specific content
            HttpContext.Current.Response.Cache.SetSlidingExpiration(true);
        }


    }
}
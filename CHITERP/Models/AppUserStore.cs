using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    
    public class AppUserStore
       : UserStore<AppUser, AppRole, string,
           AppUserLogin, AppUserRole,
           AppUserClaim>, IUserStore<AppUser, string>,
       IDisposable
    {
        public AppUserStore()
            : this(new AppDbContext())
        {
            base.DisposeContext = true;
        }

        public AppUserStore(AppDbContext context)
            : base(context)
        {
        }
    }
}
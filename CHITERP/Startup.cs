using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using CHITERP.Models;

[assembly: OwinStartup(typeof(CHITERP.Startup))]

namespace CHITERP
{
    public partial class Startup { 
        public void Configuration(IAppBuilder app) 
        {

            ConfigureAuth(app); 
        } 
    }
}
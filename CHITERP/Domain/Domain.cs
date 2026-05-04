using CHITERP.Models;
using CHITERP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;


namespace CHITERP.Models
{
    public class MenuNavData
    {
        private readonly AppDbContext context = new AppDbContext();
        public IEnumerable<MenuNavbar> navbarItems()
        {

            //" + Session["CUSRID"] + "
            //var uname = HttpSessionStateBase["CUSRID"].ToString();
            var amenu = new List<MenuNavbar>();
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0) { return amenu.ToList(); }

            var query = context.Database.SqlQuery<MenuRoleMaster>("select * from MenuRoleMaster where Roles='" + System.Web.HttpContext.Current.Session["CUSRID"].ToString() + "'");
            foreach (var data in query)
            {
               
                amenu.Add(new MenuNavbar
                {
                    MenuGId = Convert.ToInt32(data.MenuGId),
                    MenuGIndex = Convert.ToInt32(data.MenuGIndex),
                    LinkText = data.LinkText,
                    ActionName = data.ActionName,
                    ControllerName = data.ControllerName,
                    username = System.Web.HttpContext.Current.Session["CUSRID"].ToString(),// "admin",
                    imageClass = data.ImageClassName,
                    activeli = "active",
                    action = data.ActionName,
                    estatus = true
                });
            }

            return amenu.ToList();
        }



        public IEnumerable<MenuNavbar> itemsPerUser(string controller, string action, string userName)
        {

            IEnumerable<MenuNavbar> items = navbarItems();

            //IEnumerable<User> usersNav = users();

            var navbar = items.Where(p => p.ControllerName == controller && p.action == action).Select(c => { c.activeli = "active"; return c; }).ToList();

            navbar = (from nav in items
                      where nav.username.ToLower() == userName.ToLower()

                      select new MenuNavbar
                      {
                          MenuGId = nav.MenuGId,
                          MenuGIndex = nav.MenuGIndex,
                          LinkText = nav.LinkText,
                          ControllerName = nav.ControllerName,
                          ActionName = nav.ActionName,
                          imageClass = nav.imageClass,
                          estatus = nav.estatus,
                          activeli = nav.activeli
                      }).ToList();

            

            return navbar.ToList();
        }

    }
}
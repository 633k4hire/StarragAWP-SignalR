using Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_App_Master.Account;
using Web_App_Master.Models;
using static Notification.NotificationSystem;

namespace Web_App_Master
{
    public partial class _Default : Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterAsyncTask(new PageAsyncTask( Global.RefreshAssetCacheAsync));
            if (!User.IsInRole("Admins")|| !User.IsInRole("superadmin"))
            {
                Response.Redirect("/Account/AssetView.aspx");
            }
            
        }

        protected void reateRoleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var a = roleManager.Create(new IdentityRole("superadmin"));
                var b = roleManager.Create(new IdentityRole("Admins"));
                var c = roleManager.Create(new IdentityRole("Users"));
                var d = roleManager.Create(new IdentityRole("Customers"));

                Page.SiteMaster().ShowError("Roles Created");


            }
            catch
            {
                Page.SiteMaster().ShowError("Could Not Create Role.");

            }
        }
    }
}
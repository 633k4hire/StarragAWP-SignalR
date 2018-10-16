using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master.Account
{
    public partial class PrintBarcodes : System.Web.UI.Page
    {       
        public string userid { get {
                var r = ".bl"; //barcode handler
                var choice = Session["IsTextLabel"] as string;
                if (choice!=null)
                {
                    var check = Convert.ToBoolean(choice);
                    if (check)
                    {
                        r = ".l"; //labelhandler
                    }
                }
                try
                {
                    var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    // Require the user to have a confirmed email before they can log on.
                    var user = manager.FindByName(User.Identity.Name);
                    r = user.Id + r;
                }
                catch { }
               
                return r; } }
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {                
                AssetCheckList.DataSource = (from a in AssetController.GetAllAssets() orderby a.AssetNumber select a.AssetNumber).ToList();
                AssetCheckList.DataBind();
            }
        }

        protected void SelectAllBtn_Click(object sender, EventArgs e)
        {
            foreach (ListItem item in AssetCheckList.Items)
            {
                item.Selected = true;
            }

        }

        protected void PreviewBtn_Click(object sender, EventArgs e)
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();        
            // Require the user to have a confirmed email before they can log on.
            var user = manager.FindByName(User.Identity.Name);
            if (user != null)
            {
                var id = user.Id;
                var list = new List<string>();
                foreach (ListItem item in AssetCheckList.Items)
                {
                    if (item.Selected == true)
                    {
                        list.Add(item.Text);
                    }
                }
                Application["CurrentPrintList" + id] = list;
            }
            else
            {
                var id = "nouser";
                var list = new List<string>();
                foreach (ListItem item in AssetCheckList.Items)
                {
                    if (item.Selected == true)
                    {
                        list.Add(item.Text);
                    }
                }
                Application["CurrentPrintList" + id] = list;
            }
            
        }

        protected void hiddenModeSwitch_Click(object sender, EventArgs e)
        {
            var choice = Session["IsTextLabel"] as string;
            var check = Convert.ToBoolean(choice);
            if(check)
            {
                Session["IsTextLabel"] = "false";
            }
            else
            {
                Session["IsTextLabel"] = "true";
            }
        }
    }
}
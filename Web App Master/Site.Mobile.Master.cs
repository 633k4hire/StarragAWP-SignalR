using System;

namespace Web_App_Master
{
    public partial class Site_Mobile : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect(SwitcherAuto.AutoLinkUrl);
        }
    }
}
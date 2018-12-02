using System;

namespace Web_App_Master
{
    public partial class items : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("/Account/AssetView.aspx");
        }
    }
}
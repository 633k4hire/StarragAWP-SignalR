using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master
{
    public partial class Map : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["MapView"] = "default";
                Session["MapBool"] = true;
            }
        }

        protected void hiddenModeSwitch_Click(object sender, EventArgs e)
        {
            if ((bool)Session["MapBool"])
            {
                Session["MapBool"] = false;
            }
            else
            {
                Session["MapBool"] = true;

            }
        }
    }
}
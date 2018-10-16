using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master.Account.Templates
{
    public partial class icon_view : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void prepareImg()
        {
        }
        protected string prepareAjax(string num, string isout)
        {
            if (isout.ToLower() == "true")
            {
                string str = "AjaxAddCheckin('";
                str = str + num;
                str = str + "');"; /*ToggleDropDown('CheckinDropDown');*/
                return str;
            }
            else
            {
                string str = "AjaxAddCheckout('";
                str = str + num;
                str = str + "');"; /*ToggleDropDown('CheckoutDropDown');*/
                return str;
            }
        }
        protected void RenameInputBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
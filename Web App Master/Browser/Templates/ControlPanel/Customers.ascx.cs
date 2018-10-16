using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master.Browser.Templates.ControlPanel
{
    public partial class Customers : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CustomersRepeater.DataSource = Global.Library.Settings.Customers.OrderBy((w)=>w.CompanyName);
            CustomersRepeater.DataBind();
        }

        protected void DeleteCustomerBtn_Command(object sender, CommandEventArgs e)
        {
            try
            {
                var customer = (from a in Global.Library.Settings.Customers where a.CompanyName == (string)e.CommandName && a.Postal == (string)e.CommandArgument select a).FirstOrDefault();
                Global.Library.Settings.Customers.Remove(customer);
            }
            catch { }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            var query = avSearchString.Text.ToUpper();
            try
            {
                CustomersRepeater.DataSource = (from c in Global.Library.Settings.Customers where c.CompanyName.ToUpper().Contains(query) || c.Address.ToUpper().Contains(query) || c.Address2.ToUpper().Contains(query) || c.City.ToUpper().Contains(query) || c.Postal.ToUpper().Contains(query) || c.NickName.ToUpper().Contains(query) || c.Attn.ToUpper().Contains(query) || c.Country.ToUpper().Contains(query) || c.State.ToUpper().Contains(query) select c).ToList().OrderBy((w) => w.CompanyName);
                CustomersRepeater.DataBind();
                CustomerUpdatePanel.Update();
            }
            catch { }
        }

        protected void ViewAll_Click(object sender, EventArgs e)
        {

        }
    }
}
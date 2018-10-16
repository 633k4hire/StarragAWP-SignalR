using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master.Pages
{
    public partial class Customers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindCustomers(Global.Library.Settings.Customers);
        }

        protected void BindCustomers(List<Customer> customers)
        {
            CustomersRepeater.DataSource = customers.OrderBy((w) => w.CompanyName);
            CustomersRepeater.DataBind();
            //CustomerUpdatePanel.Update();
        }

        protected void CustomersRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }

        protected void CustomersRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        protected void DeleteCustomerBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
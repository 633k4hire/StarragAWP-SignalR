using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master.Browser.Templates.ControlPanel
{
    public partial class Transactions : System.Web.UI.UserControl
    {
        public string TID { get; set; }
        public string OrderNumber { get; set; }
        public string Email { get; set; }
        public string Date { get; set; }
        public Customer Address { get; set; }
        public string ShipToName { get; set; }
        public string Name { get; set; }
        public string TransactionID = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            //load application variable with current target view
            TransactionRepeater.DataSource = Pull.Transactions();
            TransactionRepeater.DataBind();
        }

        protected void TransactionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        protected void DeleteTransactionBtn_Command(object sender, CommandEventArgs e)
        {

        }

        protected void TransactionItemRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }

        protected void TransactionRepeater_ItemDataBound1(object sender, RepeaterItemEventArgs e)
        {

        }
    }
}
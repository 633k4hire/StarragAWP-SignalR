using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master.Account
{
    public partial class UserCheckout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TID= Request.QueryString["tid"];
                if (TID != null)
                {
                    var transaction = Pull.Transaction(TID);
                    OrderNumber = transaction.Comment;
                    Email = transaction.Email.Email;
                    Name = transaction.Name;
                    Date = transaction.Date.ToString();
                    Address = transaction.Customer;
                    FillDefaultShipper(Address);
                    BindRepeater(transaction);
                }
            }
        }

        private void BindRepeater(PendingTransaction transaction)
        {
            try
            {
                List<Asset> assets = new List<Asset>();
                foreach (string item in transaction.Assets)
                {
                    var ass = (from a in Pull.Assets() where a.AssetNumber == item select a).ToList();
                    assets.Add(ass[0]);
                }
                ReceiptRepeater.DataSource = assets;
                ReceiptRepeater.DataBind();
            }
            catch { }
        }

        public string TID { get; set; }
        public string OrderNumber { get; set; }
        public string Email { get; set; }
        public string Date { get; set; }
        public Customer Address { get; set; }
        public string Name { get; set; }
        private void FillDefaultShipper(Customer cust)
        {
            SprCompany.Value = cust.CompanyName;
            SprAddr.Value = cust.Address;
            SprAddr2.Value = cust.Address2;
            SprCty.Value = cust.City;
            SprState.Value = cust.State;
            SprPostal.Value = cust.Postal;
            SprCountry.Value = cust.Country;
            SprName.Value = cust.Attn;
            SprEmail.Value = cust.Email;
            SprPhone.Value = cust.Phone;
        }

    }

}
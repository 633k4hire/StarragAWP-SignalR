using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master.Account
{
    public partial class ShoppingCart : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            //this.SiteMaster().OnPanelUpdate += Checkout_OnPanelUpdate;
        }
        private void Checkout_OnPanelUpdate(object sender, UpdateRequestEvent e)
        {
            FinalCheckoutRepeater.DataBind();
            OutCartUpdatePanel.Update();
        }


        public string Customer = null;
        public string Shipper = null;
        public string Engineer = null;
        public string Ordernumber = null;
        public List<Asset> CheckoutItems = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            // ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(ContinueToCheckOutBtn); 
            
          
            Button btn = (Button)this.Master.FindControl("UpdateAllCarts");
            AsyncPostBackTrigger Trigger1 = new AsyncPostBackTrigger();
            Trigger1.ControlID = btn.ID;
            Trigger1.EventName = "Click";
            if (OutCartUpdatePanel.Triggers.Count == 1)
                OutCartUpdatePanel.Triggers.Add(Trigger1);

            BindCheckout();
            if (!IsPostBack)
            {
           
                
                    if (!Page.User.Identity.IsAuthenticated)
                    {
                    AnonLabel.Visible = true;
                        AnonUserInput.Visible = true;
                    }
                
                Customer = Session["Customer"] as string;
                Shipper = Session["Shipper"] as string;
                Engineer = Session["Engineer"] as string;
                Ordernumber = Session["Ordernumber"] as string;
                checkout_OrderNumber.Value = Ordernumber;
                //checkout_ServiceEngineer.Text = Engineer;


                try
                {
                    string usershipper = Shipper;
                    var user = Page.User.Identity.Name;
                    var split = user.Split('@');
                    user = split[0];
                    split = user.Split('.');
                    foreach (var s in split)
                    {
                        foreach (var shipper in Global.Library.Settings.ShippingPersons)
                        {
                            if (shipper.Name.ToLower().Contains(s.ToLower()) || shipper.Email.ToLower().Contains(s.ToLower()))
                            {
                                Shipper = shipper.Name;
                            }
                        }
                    }
                }
                catch { }
                //checkout_ShippingPerson.Text = Shipper;

                checkout_ShipTo.Text = Customer;
                checkout_ShipTo.Focus();
                //Response.Cache.SetExpires(DateTime.Now.AddMonths(1));
                //Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
                //Response.Cache.SetValidUntilExpires(true);
            }
        }
        public void BindCheckout()
        {
            var checkout = HttpContext.Current.Session["CheckOut"] as List<Asset>;
            if (checkout == null) checkout = new List<Asset>();
            FinalCheckoutRepeater.DataSource = checkout;
            FinalCheckoutRepeater.DataBind();

            checkout_ShipTo.DataSource = GetShipToNames();
            checkout_ShipTo.DataBind();
        }
        public List<string> GetShipToNames()
        {
            var names = (from ship in Global.Library.Settings.Customers orderby ship.CompanyName select ship.CompanyName + "-" + ship.Address + "-" + ship.Postal).ToList();
            return names;
        }
        public List<string> GetEngineerNames()
        {
            var names = (from ship in Global.Library.Settings.ServiceEngineers orderby ship.Name select ship.Name).ToList();
            return names;
        }
        public List<string> GetShippingNames()
        {
            try
            {
                var names = (from ship in Global.Library.Settings.ShippingPersons orderby ship.Name select ship.Name).ToList();
                return names;
            }
            catch { return new List<string>(); }
        }
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
        private Customer GetCustomAddress()
        {
            Customer cust = new Helpers.Customer();
            cust.CompanyName =SprCompany.Value;
             cust.Address = SprAddr.Value;
            cust.Address2 = SprAddr2.Value;
             cust.City = SprCty.Value;
             cust.State = SprState.Value;
             cust.Postal = SprPostal.Value;
             cust.Country = SprCountry.Value;
            cust.Attn = SprName.Value;
             cust.EmailAddress = new EmailAddress() { Name = SprCompany.Value, Email= SprEmail.Value} ;
            cust.Phone = SprPhone.Value;
            return cust;
        }

        protected void ShowError(string error)
        {
            try
            {
                MessagePlaceHolder.Visible = true;
                ErrorMessage.Text = error;
            }
            catch { }
        }
        protected void ContinueToCheckOutBtn_Click(object sender, EventArgs e)
        {
            try
            {


                if ((Session["CheckOut"]) == null) return;
                if ((Session["CheckOut"] as List<Asset>).Count == 0) return;
                var transaction = new PendingTransaction(Session["CheckOut"] as List<Asset>);
                transaction.Comment = checkout_OrderNumber.Value;
                if (transaction.Email.Email == "")
                {
                    if (AnonUserInput.Text == "")
                    {
                        ShowError("Please enter a Valid email address");
                        return;
                    }
                    if (!EmailHelper.IsValid(AnonUserInput.Text))
                    {
                        ShowError("Please enter a Valid email address");
                        return;
                    }
                    else
                    {
                        transaction.Email = new EmailAddress() { Name = AnonUserInput.Text, Email = AnonUserInput.Text };
                        transaction.Name = AnonUserInput.Text;
                    }
                }
                if (cb_CustomAddress.Checked)
                {
                    transaction.Customer = GetCustomAddress();

                    if (transaction.Customer == null)
                    {
                        ShowError("No customer selected.");
                        return;
                    }
                }
                else
                {
                    if (checkout_ShipTo.SelectedValue == "")
                    {
                        ShowError("No customer selected.");
                        return;
                    }
                    transaction.Customer = (from a in Global.Library.Settings.Customers where checkout_ShipTo.Text.Contains(a.CompanyName) && checkout_ShipTo.Text.Contains(a.Address) select a).ToList().FirstOrDefault();
                    if (transaction.Customer == null)
                    {
                        ShowError("No customer");
                        return;
                    }
                }
                Session["CheckOut"] = new List<Asset>();
                //Session["PendingTransaction"] = transaction;
                Push.Transaction(transaction);
                Push.Alert("Request Confirmed");
                //save to log

                //add notification with transactionID

                var staturl = "http://starrag.azurewebsites.net/Account/OutCart.aspx?pend=" + transaction.TransactionID;
                var rurl = "http://starrag.azurewebsites.net/Account/UserCheckout.aspx?tid=" + transaction.TransactionID;
                var body = "Thank you for placing a tool request<br>Your confirmation number is: " + transaction.TransactionID + "<br> you can view your reciept here <a href='" + rurl + "'></a><br>" + rurl;
                var staticbody = "A new Service Tool Request has been made:<br>Pending Transaction Id number is: " + transaction.TransactionID + "<br> Complete Transaction here <a href='" + staturl + "'></a><br>" + staturl;

                if (!Global.Library.Settings.TESTMODE)
                {
                    try
                    {

                        EmailHelper.SendEmail(transaction.Email.Email, body, "AWP - Order Confirmation:");

                        var turl = "http://starrag-awp.com/Account/OutCart.aspx?pend=" + transaction.TransactionID;
                        var staticBody = "A new order has been placed by " + transaction.Email.Email + "<br>For Customer " + transaction.Customer.CompanyName + "<br>  Transation ID: " + transaction.TransactionID + "<br> you can complete this transaction here <a href=\"" + turl + "\"></a><br>" + turl;
                        var statics = (from a in Global.Library.Settings.StaticEmails select a.Email).ToArray();
                        EmailHelper.SendMassEmail(statics, staticbody, "AWP - New Pending Transaction");
                    }
                    catch (Exception ex)
                    {
                        Context.AddError(ex);
                    }
                }
                //if logged in add transation to recent user transactions

                //redirect
                // Context.Response.Redirect("/Account/UserCheckout.aspx?tid=" + transaction.TransactionID);    
                ApplyChangesButton.Visible = false;
                ShowError("Thank You For Placing an Order");
            }
            catch (Exception exx)
            {
                ShowError("Error Placing Order\r\n"+exx.StackTrace);

            }
        }

        protected void FinalCheckoutRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "delete")
            {
                Asset rem = null;
                foreach (var ass in Session["CheckOut"] as List<Asset>)
                {
                    if (ass.AssetNumber == e.CommandArgument as string)
                    {
                        rem = ass;
                    }
                }
                try
                {
                    (Session["CheckOut"] as List<Asset>).Remove(rem);
                }
                catch { }
                this.UpdateAll();
                BindCheckout();
                OutCartUpdatePanel.Update();
            }
        }
    }
}
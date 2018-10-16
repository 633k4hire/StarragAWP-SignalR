using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master.Account
{
    public partial class Checkout : System.Web.UI.Page
    {
        //Master Event Wire Up
        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.SiteMaster().OnPanelUpdate += Checkout_OnPanelUpdate;
            }
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
        public PendingTransaction Pending = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PreInit += Page_PreInit;
            Button btn = (Button)this.Master.FindControl("UpdateAllCarts");
            AsyncPostBackTrigger Trigger1 = new AsyncPostBackTrigger();
            Trigger1.ControlID = btn.ID;
            Trigger1.EventName = "Click";
            if (OutCartUpdatePanel.Triggers.Count==1)
                OutCartUpdatePanel.Triggers.Add(Trigger1);

            
            if (!IsPostBack)
            {
                Session["IsPending"] = "false";
                if (!User.Identity.IsAuthenticated)
                {
                    Response.Redirect("/Account/Login");
                }
                try
                {
                    var TransactionID = Request.QueryString["pend"];
                    Session["TransactionID"] = TransactionID;
                    if (TransactionID != null)
                    {
                        Pending= Web_App_Master.Pull.Transaction(TransactionID);
                        Session["Pending"] = Pending;
                        Session["IsPending"] = "true";
                    }
                }
                catch (Exception) { }
                if (Pending != null)
                {
                   
                    try
                    {
                        //build asset list
                        var all = AssetController.GetAllAssets();
                        List<Asset> list = new List<Asset>();
                        foreach (var item in Pending.Assets)
                        {
                            list.Add((from a in all where a.AssetNumber == item select a).First());
                        }
                        BindCheckout(list);
                        Session["CheckOut"] = list;
                        //TODO: FIX THIS SHIT ass find of names in shipping persons
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
                    checkout_ShippingPerson.Text = Shipper;
                    if (Pending.Customer!=null)
                    {
                        var existing = (from a in Global.Library.Settings.Customers where Pending.Customer.Address == a.Address select a).ToList();
                        if (existing.Count>0)
                        {
                            Session["Customer"] = existing[0].CompanyName;
                            var item = (checkout_ShipTo.Items.FindByText(existing[0].CompanyName + "-" + existing[0].Address + "-" + existing[0].Postal));
                            var idx = checkout_ShipTo.Items.IndexOf(item);
                            checkout_ShipTo.SelectedIndex = idx;
                        }
                        else
                        {
                            //add to global and save
                            Global.Library.Settings.Customers.Add(Pending.Customer);
                            Push.AppSettings();
                            CustomCustomerPlaceHolder.Visible = true;
                            CustomCustomer.Text = Pending.Customer.CompanyName;
                            checkout_ShipTo.Visible = false;
                        }
                    }
                    checkout_OrderNumber.Value = Pending.Comment;
                    var engineer = (from a in Global.Library.Settings.ServiceEngineers where a.Email == Pending.Name select a).ToList();
                    if (engineer.Count > 0)
                    {
                        checkout_ServiceEngineer.Text = engineer[0].Name;
                            }
                    else
                    {
                        CustomEngineerPlaceHolder.Visible = true;
                        CustomEngineer.Text = Pending.Name;
                        checkout_ServiceEngineer.Visible = false;
                    }
                    
                }
                else
                {
                    BindCheckout();
                    Customer = Session["Customer"] as string;
                    Shipper = Session["Shipper"] as string;
                    Engineer = Session["Engineer"] as string;
                    Ordernumber = Session["Ordernumber"] as string;
                    checkout_OrderNumber.Value = Ordernumber;
                    checkout_ServiceEngineer.Text = Engineer;


                    try
                    {
                        //TODO: FIX THIS SHIT ass find of names in shipping persons
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
                    checkout_ShippingPerson.Text = Shipper;

                    checkout_ShipTo.Text = Customer;
                    checkout_ShipTo.Focus();
                   
                }
            }
            
        }
        public void BindCheckout(List<Asset> datasource=null)
        {
            if (datasource==null)
            {
                var checkout = HttpContext.Current.Session["CheckOut"] as List<Asset>;
                if (checkout == null) checkout = new List<Asset>();
                FinalCheckoutRepeater.DataSource = checkout;
                FinalCheckoutRepeater.DataBind();
            }
            else
            {
                FinalCheckoutRepeater.DataSource = datasource;
                FinalCheckoutRepeater.DataBind();
            }
            

            checkout_ServiceEngineer.DataSource = GetEngineerNames();
            checkout_ServiceEngineer.DataBind();

            checkout_ShippingPerson.DataSource = GetShippingNames();
            checkout_ShippingPerson.DataBind();

            checkout_ShipTo.DataSource = GetShipToNames();
            checkout_ShipTo.DataBind();
        }
        public List<string> GetShipToNames()
        {
            var names = (from ship in Global.Library.Settings.Customers orderby ship.CompanyName select ship.CompanyName+"-"+ship.Address+"-"+ship.Postal).ToList();
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

        protected void ContinueToCheckOutBtn_Click(object sender, EventArgs e)
        {

            Session["Customer"] =( checkout_ShipTo.Text);
           Session["Shipper"] = (checkout_ShippingPerson.Text);
             Session["Engineer"] =(checkout_ServiceEngineer.Text);
           Session["Ordernumber"] = (checkout_OrderNumber.Value);
            if ((Session["CheckOut"]) == null) return;
            if ((Session["CheckOut"] as List<Asset>).Count == 0) return;

            //if it is a pending need to remove from sql now
            if (Session["IsPending"] as string == "true")
            {
                //AssetController.RemoveTransaction(Pending.TransactionID);
                Context.Response.Redirect("/Account/Checkout.aspx?tid="+ Session["TransactionID"] as string);

            }
            else
            {
                Context.Response.Redirect("/Account/Checkout.aspx");

            }
        }

        protected void FinalCheckoutRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName.ToLower()=="delete")
            {
                Asset rem = null;
                foreach(var ass in Session["CheckOut"] as List<Asset>)
                {
                    if (ass.AssetNumber== e.CommandArgument as string)
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

        protected void checkout_ShipTo_TextChanged(object sender, EventArgs e)
        {

        }
    }
   

}
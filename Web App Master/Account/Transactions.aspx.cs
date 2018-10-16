using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master.Account
{
    public partial class Transactions : System.Web.UI.Page
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
            UpdateTransactionRepeater();

            if (!IsPostBack)
            {
                try
                {
                    
                    TransactionID= Request.QueryString["tid"];
                    Session["TransactionID"] = TransactionID;
                    if (TransactionID!=null)
                    {
                        try
                        {
                            var transaction = Web_App_Master.Pull.Transaction(TransactionID);
                            TID = transaction.TransactionID;
                            OrderNumber = transaction.Comment;
                            Email = transaction.Email.Email;
                            Name = transaction.Name;
                            Date = transaction.Date.ToString();
                            Address = transaction.Customer;
                            ShipToName = transaction.Customer.CompanyName;
                            TransactionIdLabel.Text = transaction.Name + " - " + transaction.Date.ToShortDateString();
                            var bindlist = new List<Helpers.Asset>();
                            transaction.Assets.ForEach((asset) =>
                            {
                                bindlist.Add(Pull.Asset(asset));
                            });
                            TransactionItemRepeater.DataSource = bindlist;
                            TransactionItemRepeater.DataBind();
                        }
                        catch { }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            
        }

        private void UpdateTransactionRepeater()
        {
            var transactions = Pull.Transactions().OrderBy(w=>w.Date);
            TransactionRepeater.DataSource = transactions;
            TransactionRepeater.DataBind();
        }
        private void BindTransactionRepeater(string tid = null)
        {
            if (tid != null)
            {
                try
                {
                    var transaction = Web_App_Master.Pull.Transaction(tid);
                    TID = transaction.TransactionID;
                    OrderNumber = transaction.Comment;
                    Email = transaction.Email.Email;
                    Name = transaction.Name;
                    Date = transaction.Date.ToString();
                    Address = transaction.Customer;
                    ShipToName = transaction.Customer.CompanyName;
                    TransactionIdLabel.Text = transaction.Name + " - " + transaction.Date.ToShortDateString();
                    var bindlist = new List<Helpers.Asset>();
                    transaction.Assets.ForEach((asset) =>
                    {
                        bindlist.Add(Pull.Asset(asset));
                    });
                    TransactionItemRepeater.DataSource = bindlist;
                    TransactionItemRepeater.DataBind();
                }
                catch { }
            }

        }
        protected void DeleteTransactionBtn_Command(object sender, CommandEventArgs e)
        {
            try
            {
                if (!AssetController.RemoveTransaction(e.CommandArgument as string))
                {
                    Page.SiteMaster().ShowError("Could Not Delete Transaction");
                }
                else
                {
                    UpdateTransactionRepeater();
                    TransactionUpdatePanel.Update();
                }
            }
            catch {  Page.SiteMaster().ShowError("Could Not Delete Transaction");}
        }

        protected void TransactionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        protected void CompleteTransactionBtn_Command(object sender, CommandEventArgs e)
        {

        }

        protected void TransactionItemRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                var tid = Session["TransactionID"] as string;
                var assetNumber = e.CommandArgument as string;
                var transaction = Pull.Transaction(tid);
                transaction.Assets.Remove(assetNumber);
                AssetController.RemoveTransaction(tid);
                AssetController.PushTransaction(transaction);
                BindTransactionRepeater(tid);
            }
            catch { }
            UpdateTransactionRepeater();
        }
    }
}
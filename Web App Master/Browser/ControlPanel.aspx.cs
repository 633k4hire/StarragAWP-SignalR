using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_App_Master.Models;
using Helpers;
using System.Xml;
using System.IO;
using System.IO.Compression;
using System.Web.UI.HtmlControls;
using static Notification.NotificationSystem;
using Web_App_Master.Account;

namespace Web_App_Master.Browser
{
    
    public partial class ControlPanel : System.Web.UI.Page
    {
        //SUPER BUTTON
        protected void SuperButton_Click(object sender, EventArgs e)
        {
            var sc = SuperButtonCommand.Text;
            var sa = SuperButtonArg.Text;
            if (sc.Contains("role"))
            {
                try
                {
                    var split = sa.StringSplit("-dd-"); 
                    if (sc.Contains("delete"))
                    {

                    }
                    if (sc.Contains("change"))
                    {
                        string name = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(split[0]).UserName;
                        foreach (RepeaterItem i in RolesAndUsersRepeater.Items)
                        {

                            Repeater repeater = (Repeater)i.Controls[1];
                            foreach (RepeaterItem item in repeater.Items)
                            {
                                var changebutton = item.FindControl("ChangeRole") as Button;
                                var temprole = changebutton.CommandName;
                                if (temprole == name)
                                {
                                    var dropdown = item.FindControl("RoleDropDown") as DropDownList;
                                    if (dropdown != null)
                                    {
                                        var selected = dropdown.SelectedValue;

                                        var result = ChangeUserRole(name, selected);
                                        result = RemoveUserFromRole(name, split[1]);

                                        return;
                                    }
                                }

                            }

                        }
                    }
                    if (sc.Contains("copy"))
                    {

                    }
                }
                catch { }
            }
            if (sc.Contains("user"))
            {
                if (sc.Contains("delete"))
                {

                }
                if (sc.Contains("approve"))
                {

                }                
            }
            if (sc.Contains("asset"))
            {
                if (sc.Contains("delete"))
                {
                    var local = Global.AssetCache.Find((x) => x.AssetNumber == sa);
                    if (local != null)
                    {
                        AssetController.DeleteAsset(sa);
                        var assetCache = Application[(Session["guid"] as string)] = Pull.Assets();
                        BindAndUpdateAssets((assetCache as List<Asset>));
                        Page.SiteMaster().UpdateAssetView();
                    }
                }
                if (sc.Contains("edit"))
                {
                    LoadSingleAssetView(Pull.Asset(sa));
                }
                if (sc.Contains("history"))
                {
                    var split = sa.Split(new string[] { "-dd-" }, StringSplitOptions.RemoveEmptyEntries);
                    var tempasset = Pull.Asset(split[0]);
                    var hist = from h in tempasset.History.History where h.DateShippedString == split[1] select h;
                    if (hist.Count() >0)
                    {
                        LoadSingleAssetView(hist.First());
                    }
                    
                }
                if (sc.Contains("delhist"))
                {
                    var split = sa.Split(new string[] { "-dd-" }, StringSplitOptions.RemoveEmptyEntries);
                    var tempasset = Pull.Asset(split[0]);
                    var hist = from h in tempasset.History.History where h.DateShippedString == split[1] select h;
                    if (hist.Count() > 0)
                    {
                        tempasset.History.History.Remove(hist.First());
                        Push.Asset(tempasset);
                        LoadSingleAssetView(tempasset);
                        UpdateStatus("History Item Removed");
                    }
                }
                if (sc.Contains("deldoc"))
                {
                    var split = sa.Split(new string[] { "-dd-" }, StringSplitOptions.RemoveEmptyEntries);
                    var tempasset = Pull.Asset(split[0]);
                    tempasset.Documents.Remove(split[1]);
                    Push.Asset(tempasset);
                    LoadSingleAssetView(tempasset);
                    UpdateStatus("Document Removed");
                }
                if (sc.Contains("delimg"))
                {
                    var split = sa.Split(new string[] { "-dd-" }, StringSplitOptions.RemoveEmptyEntries);
                    var tempasset = Pull.Asset(split[0]);
                    tempasset.Images = tempasset.Images.Replace(split[1]+",","");
                    File.Delete(Server.MapPath("/Account/Images/"+split[0]+"/"+split[1]));
                    Push.Asset(tempasset);
                    LoadSingleAssetView(tempasset);
                    UpdateStatus("Image Removed");
                }
            }
            if (sc.Contains("static"))
            {
                if (sc.Contains("delete"))
                {
                    try
                    {
                        var p = from x in Global.Library.Settings.StaticEmails where x.Email == sa select x;
                        if (p.Count() > 0)
                        {
                            Global.Library.Settings.StaticEmails.Remove(p.First());
                            Push.AppSettings();
                            UpdateStatus(sa + " removed from Static Email List");
                            BindAndUpdatePersonnel(Global.Library.Settings.ShippingPersons, Global.Library.Settings.ServiceEngineers, Global.Library.Settings.StaticEmails);
                        }
                    }
                    catch { UpdateStatus("Error removing " + sa); }
                }
            }
            if (sc.Contains("notice"))
            {
                if (sc.Contains("delete"))
                {
                    try
                    {
                        Global.NoticeSystem.Notices = new NoticeBindinglist((Application["NotificationList"] as List<Notification.NotificationSystem.Notice>));
                        var item = (from n in Global.NoticeSystem.Notices where n.Guid == sa select n).FirstOrDefault();
                        if (item == null)
                            return;
                        Global.NoticeSystem.Notices.Remove(item);
                        Push.NotificationSystem();
                        BindNotifications(Global.NoticeSystem.Notices.ToList());
                        NotificationsViewUpdatePanel.Update();
                        AppRightPanelUpdatePanel.Update();
                        UpdateAllUpdatePanels();
                    }
                    catch
                    { }
                }
            }
            if (sc.Contains("customer"))
            {
                if (sc.Contains("delete"))
                {
                    try
                    {
                        string[] split = sa.Split(new string[] { "-dd-" }, StringSplitOptions.None);
                        var item = (from n in Global.Library.Settings.Customers where split[0].Contains(n.CompanyName) && split[1].Contains(n.Postal) select n).FirstOrDefault();
                        if (item == null)
                            return;
                        Global.Library.Settings.Customers.Remove(item);
                        Push.AppSettings();
                        BindAndUpdateCustomers(Global.Library.Settings.Customers);
                        UpdateStatus(item.CompanyName + " deleted");
                    }
                    catch
                    { UpdateStatus("Error removing Customer"); }
                }
            }
            if (sc.Contains("op"))
            {
                if (sc.Contains("delete"))
                {
                    try
                    {
                        var p = from x in Global.Library.Settings.ShippingPersons where x.Email == sa select x;
                        if (p.Count() > 0)
                        {
                            Global.Library.Settings.ShippingPersons.Remove(p.First());
                            Push.AppSettings();
                            UpdateStatus(sa + " removed from Office Personnel");
                            BindAndUpdatePersonnel(Global.Library.Settings.ShippingPersons, Global.Library.Settings.ServiceEngineers);
                        }
                    }
                    catch { UpdateStatus("Error removing " + sa); }
                }
            }
            if (sc.Contains("fp"))
            {
                if (sc.Contains("delete"))
                {
                    try
                    {
                        var p = from x in Global.Library.Settings.ServiceEngineers where x.Email == sa select x;
                        if (p.Count() > 0)
                        {
                            Global.Library.Settings.ServiceEngineers.Remove(p.First());
                            Push.AppSettings();
                            UpdateStatus(sa + " removed from Field Personnel");
                            BindAndUpdatePersonnel(Global.Library.Settings.ShippingPersons, Global.Library.Settings.ServiceEngineers);
                        }
                    }
                    catch { UpdateStatus("Error removing " + sa); }
                }
            }
            if (sc.Contains("transaction"))
            {
                if (sc.Contains("delete"))
                {
                    AssetController.RemoveTransaction(sa);
                    BindAndUpdateTransactions(Pull.Transactions());
                }
            }
            if (sc.Contains("cust"))
            {
                if (sc.Contains("customer")) return;
                string[] split = sa.Split(new string[] { "-dd-" }, StringSplitOptions.RemoveEmptyEntries);
                if (sc.Contains("deldoc"))
                {
                    var customer = Session["CurrentCustomer"] as Customer;
                    CustomerData cd = Pull.CustomerData(customer.DataGuid);
                    if (cd != null)
                    {
                        cd.Documents.Remove(split[1]);
                        Push.CustomerData(cd);
                        LoadCustomerView(customer);
                    }
                }
                if (sc.Contains("delkit"))
                {
                    var customer = Session["CurrentCustomer"] as Customer;
                    CustomerData cd = Pull.CustomerData(customer.DataGuid);
                    if (cd != null)
                    {
                        var kit = cd.AssetKitHistory.Find((k)=>  k.Guid== split[0]);
                        cd.AssetKitHistory.Remove(kit);
                        Push.CustomerData(cd);
                        LoadCustomerView(customer);
                    }
                }
                
                
            }
            if (sc.Contains("kit"))
            {
                if (sc.Contains("send"))
                {
                    try
                    {
                        var split = sa.Split(new string[] { "-dd-" }, StringSplitOptions.RemoveEmptyEntries);
                        Session["Checkout"] = new List<Asset>();
                        var list = Session["Checkout"] as List<Asset>;
                        var cd = Pull.CustomerData((Session["CurrentCustomer"] as Customer).DataGuid);
                        foreach (var a in cd.AssetKitHistory)
                        {
                            if (split[0] == a.Guid)
                            {
                                foreach (var item in a.Assets)
                                {
                                    list.Add(Pull.Asset(item));
                                }

                            }
                        }
                        Session["Checkout"] = list;
                        Response.Redirect("/Account/Outcart.aspx");
                    }
                    catch { UpdateStatus("Problem"); }
                    //create pending tranasction and send it

                }

            }
        }

        //Developer Action
        protected void DeveloperAction_Click(object sender, EventArgs e)
        {
            Global.AssetCache.ForEach((a)=> 
            {
                try
                {
                    var distinctimages = a.ImageList.DistinctBy(x => x).ToList();
                    if (a.ImageList != distinctimages)
                    {
                        a.ImageList = distinctimages;
                        Push.Asset(a);
                    }
                }
                catch {  }
                
            });
            UpdateStatus("Changed Assest Images");
        }


        protected void UpdateAllUpdatePanels()
        {
            try
            {
                ups_pwd.Value = Global.Library.Settings.UpsAccount.P;
                ups_aln.Value = Global.Library.Settings.UpsAccount.A;
                ups_userid.Value = Global.Library.Settings.UpsAccount.I;
                ups_shippernumber.Value = Global.Library.Settings.UpsAccount.N;
            }
            catch { }
            try
            {
                shipmsgbox.Value = Global.Library.Settings.ShipperNotification;
                checkinmsgbox.Value = Global.Library.Settings.CheckInMessage;
                checkoutmsgbox.Value = Global.Library.Settings.CheckOutMessage;
                notificationmsgbox.Value = Global.Library.Settings.NotificationMessage;
            }
            catch
            {

            }
            AppFooterUpdatePanel.Update();
            AppLeftPanelUpdatePanel.Update();
            //
            AppToolbarUpdatePanel.Update();
            AssetsUpdatePanel.Update();
            CustomerUpdatePanel.Update();
            CertificateUpdatePanel.Update();
            IndividualTransactionUpdatePanel.Update();
            NotificationsViewUpdatePanel.Update();
            PersonnelFieldUpdatePanel.Update();
            PersonnelMainViewUpdatePanel.Update();
            PersonnelOfficeUpdatePanel.Update();
            PersonnelViewMasterUpdatePanel.Update();
            RolesUpdatePanel.Update();
            SettingsViewUpdatePanel.Update();
            TransactionListUpdatePanel.Update();
            UserUpdatePanel.Update();
            AppRightPanelUpdatePanel.Update();          
        }

        protected void BindAndUpdatePersonnel(List<EmailAddress> office=null, List<EmailAddress> field=null, List<EmailAddress> statics=null)
        {
            if (statics != null)
            {
                BindStaticEmails(statics);
            }
            else
            {
                BindStaticEmails(Global.Library.Settings.StaticEmails);
            }
            if (office != null)
            {
                BindOfficePersonnel(office);
            }
            else
            {
                BindOfficePersonnel(Global.Library.Settings.ShippingPersons);
            }
            if (field != null)
            {
                BindFieldPersonnel(field);
            }
            else
            {
                BindFieldPersonnel(Global.Library.Settings.ServiceEngineers);
            }
            PersonnelFieldUpdatePanel.Update();
            PersonnelOfficeUpdatePanel.Update();
            PersonnelMainViewUpdatePanel.Update();
        }

        protected void BindAndUpdateCustomers(List<Customer> customers = null)
        {
            if (customers!=null)
            {
                BindCustomers(customers);
            }
            else
            {
                BindCustomers(Global.Library.Settings.Customers);
            }
            CustomerUpdatePanel.Update();         
        }

        protected void BindAndUpdateTransactions(List<PendingTransaction> transactions = null)
        {
            if (transactions != null)
            {
                BindTransactions(transactions);
            }
            else
            {
                BindTransactions(Pull.Transactions());
            }
            TransactionListUpdatePanel.Update();
        }

        protected void BindAndUpdateNotifications(List<Notification.NotificationSystem.Notice> notices = null)
        {
            if (notices != null)
            {
                BindNotifications(notices);
            }
            else
            {
                BindNotifications(Pull.Notifications());
            }
            NotificationsViewUpdatePanel.Update();
        }

        protected void BindAndUpdateAssets(List<Asset> assets = null)
        {
            if (assets != null)
            {
                BindAssets(assets);
            }
            else
            {
                var assetCache = Application[(Session["guid"] as string)] as List<Asset>;
                BindAssets(assetCache);
            }
            AssetsUpdatePanel.Update();
        }

        protected void BindAndUpdateCertificates(List<CalibrationData> certs = null)
        {
            if (certs != null)
            {
                BindCertificates(certs);
            }
            else
            {
                BindCertificates(Pull.Certificates().Calibrations);
            }
            CertificateUpdatePanel.Update();
        }
        public class FileRepeaterItem
        {           
            public string AssetNumber { get; set; }
            public string FileName { get; set; }
        }
        protected void BindAndUpdateSingleAssetView(Asset asset)
        {
            SingleAssetViewLabel.Text = asset.AssetNumber + ":" + asset.AssetName;

            List<FileRepeaterItem> images = new List<FileRepeaterItem>();
            foreach (var img  in asset.Images.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                images.Add(new FileRepeaterItem() { AssetNumber=asset.AssetNumber, FileName=img });
            }
            SingleAssetViewImageRepeater.DataSource = images;
            SingleAssetViewImageRepeater.DataBind();

            List<FileRepeaterItem> documents = new List<FileRepeaterItem>();
            foreach (var doc in asset.Documents)
            {
                documents.Add(new FileRepeaterItem() { AssetNumber = asset.AssetNumber, FileName = doc });
            }
            SingleAssetViewDocumentRepeater.DataSource = documents;
            SingleAssetViewDocumentRepeater.DataBind();

            SingleAssetViewHistoryRepeater.DataSource = asset.History.History;
            SingleAssetViewHistoryRepeater.DataBind();


            SingleAssetViewUpdatePanel.Update();
        }

        protected void BindAndUpdateCustomer(Customer input)
        {
            if (input == null) return;
            CustomerViewDocumentsLabel.Text = input.CompanyName+": "+input.Address+","+input.Postal;
            CustomerViewCompanyNameLabel.Text = input.CompanyName;
            CustomerViewAddressLabel.Text = input.Address;
            CustomerViewAddress2Label.Text = input.Address2;
            CustomerViewAddress3Label.Text = input.Address3;
            CustomerViewCityLabel.Text = input.City;
            CustomerViewStateLabel.Text = input.State;
            CustomerViewPostalLabel.Text = input.Postal;
            CustomerViewPhoneLabel.Text = input.Phone;
            CustomerViewAttnLabel.Text = input.Attn;
            CustomerViewEmailLabel.Text = input.Email;
            List<Asset> assigned = new List<Asset>();
            List<string> removableassets = new List<string>();
            bool updatecust = false;
                input.CurrentAssignedAssets.ForEach((a) =>
                {

                    if (input.CurrentAssignedAssets.Contains(a))
                    {
                        removableassets.Add(a);
                         updatecust = true;
                    }
                    else
                    {
                        var aa = Global.AssetCache.FindAssetByNumber(a);
                        assigned.Add(aa);
                    }
                });
             
            CustomerViewAssignedAssetRepeater.DataSource = assigned;
            CustomerViewAssignedAssetRepeater.DataBind();

            CustomerData cd = new CustomerData();
            if (input.DataGuid != null)
            {
                if (input.DataGuid == "")
                {
                    Global.Library.Settings.Customers.ForEach((c) =>
                    {
                        if (c.Equals(input))
                        {
                            c.DataGuid = cd.Guid;
                            Push.AppSettings();
                        }
                    });
                    cd.Customer = input;
                    cd.Date = DateTime.Now;
                    Push.CustomerData(cd);
                }
                else
                {
                    cd= Pull.CustomerData(input.DataGuid);
                    if (updatecust)
                    {
                        Global.Library.Settings.Customers.ForEach((c) =>
                        {
                            if (c.Equals(input))
                            {
                                removableassets.ForEach((f) => { c.CurrentAssignedAssets.Remove(f); });
                                Push.AppSettings();
                            }
                        });
                    }
                      
                }
            }
            else
            {
                Global.Library.Settings.Customers.ForEach((c) =>
                {
                    if (c.Equals(input))
                    {
                        c.DataGuid = cd.Guid;
                        Push.AppSettings();
                    }
                });
                cd.Customer = input;
                cd.Date = DateTime.Now;
                Push.CustomerData(cd);
            }
            try
            {
                
                var list = new List<FileRepeaterItem>();
                if (cd == null)
                {
                    cd = new CustomerData();                    
                    cd.Guid = input.DataGuid;                   
                    cd.Customer = input;
                    cd.Date = DateTime.Now;
                    Push.CustomerData(cd);
                }

                cd.Documents.ForEach((d) => {
                    list.Add(new FileRepeaterItem() { AssetNumber = cd.Guid, FileName = d });
                });


                CustomerViewDocumentsRepeater.DataSource = list;
                CustomerViewDocumentsRepeater.DataBind();
            }
            catch
            {
                UpdateStatus("No Customer Data");
            }

            //Asset Kits
            CustomerViewAssetKitsRepeater.DataSource = cd.AssetKitHistory;
            CustomerViewAssetKitsRepeater.DataBind();

            List<FileRepeaterItem> onlist = new List<FileRepeaterItem>();
            cd.OrderNumbers.ForEach((o) => { onlist.Add(new FileRepeaterItem() { FileName = o, AssetNumber = o });});
            CustomerViewOrderNumbersRepeater.DataSource = onlist;
            CustomerViewOrderNumbersRepeater.DataBind();

            CustomerGuid = cd.Guid;
            CustomerViewUpdatePanel.Update();
        }

        public string CustomerGuid { get; set; }
        public string TID { get; set; }
        public string OrderNumber { get; set; }
        public string Email { get; set; }
        public string Date { get; set; }
        public Helpers.Customer Address { get; set; }
        public string ShipToName { get; set; }
        public string Name { get; set; }
        public string TransactionID = "";

        protected void Page_Load(object sender, EventArgs e)
        {
         

            DirTree.SelectedNodeChanged += DirTree_SelectedNodeChanged;
            //dynamically add postpacks
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(SuperButton);
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(DirTree);
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(COPOkBtn);
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(CFPOkBtn);
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(CCOkBtn);
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(CCertOkBtn);
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(CSEOkBtn);
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(CreateAssetModalOkBtn);
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(ControlPanelSaveBtn);
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(ExportXmlBtn);
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(ExportLibraryBtn);
            //ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(CopyUserToRoleBtn);
           // ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(ChangeUserToRoleBtn);



            if (IsPostBack)
            {
               
            }
            

            //determkne last view and load it 
            if (!IsPostBack)
            {
                
                var a = Global.Library.Settings.UpsAccount;
                Pull.Globals();
                a = Global.Library.Settings.UpsAccount;
                TESTMODE_CHECKBOX.Checked = Global.Library.Settings.TESTMODE;

                try
                {
                    ups_pwd.Value = Global.Library.Settings.UpsAccount.P;
                    ups_aln.Value = Global.Library.Settings.UpsAccount.A;
                    ups_userid.Value = Global.Library.Settings.UpsAccount.I;
                    ups_shippernumber.Value = Global.Library.Settings.UpsAccount.N;
                }
                catch { }
                try
                {
                    shipmsgbox.Value = Global.Library.Settings.ShipperNotification;
                    checkinmsgbox.Value = Global.Library.Settings.CheckInMessage;
                    checkoutmsgbox.Value = Global.Library.Settings.CheckOutMessage;
                    notificationmsgbox.Value = Global.Library.Settings.NotificationMessage;
                }
                catch
                {

                }
                
                LoadControlPanelApp();
                var cv = Session["cv"] as string; //get current view 
                switch (cv)
                {
                    case "settings":
                        LoadSettingsView(null);
                        break;
                    case "customers":
                        LoadCustomersView(null);
                        break;
                    case "customer":
                        LoadCustomerView(null);
                        break;
                    case "transactions":
                        LoadTransactionsView(null);
                        break;
                    case "notifications":
                        LoadNoticeView(null);
                        break;
                    case "personnel":
                        LoadPersonnelView(null);
                        break;
                    case "assets":
                        LoadAssetView(null);
                        break;
                   case "certificates":
                        LoadCertificateView(null);
                        break;
                    case null:
                        LoadSettingsView(null);
                        break;
                }
            }
            else
            {
                
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            var query = avSearchString.Text.ToUpper();
            var cv = Session["cv"] as string;
            //REPALCE WITH SESSIONVARIABLE
            switch (cv)
            {
                case "":

                    break;
                case "settings":

                    break;
                case "customers":
                    try
                    {
                        BindCustomers((from c in Global.Library.Settings.Customers where c.CompanyName.ToUpper().Contains(query) || c.Address.ToUpper().Contains(query) || c.Address2.ToUpper().Contains(query) || c.City.ToUpper().Contains(query) || c.Postal.ToUpper().Contains(query) || c.NickName.ToUpper().Contains(query) || c.Attn.ToUpper().Contains(query) || c.Country.ToUpper().Contains(query) || c.State.ToUpper().Contains(query) select c).ToList());
                    }
                    catch { }
                    break;
                case "transactions":
                    var mainlist = (from t in Pull.Transactions() where t.Comment.ToUpper().Contains(query) || t.TransactionID.ToUpper().Contains(query) || t.Name.ToUpper().Contains(query) || t.Email.Email.ToUpper().Contains(query) || t.Email.Name.ToUpper().Contains(query) || t.Date.ToShortDateString().ToUpper().Contains(query) || t.ConfirmationNumber.ToUpper().Contains(query) || t.Customer.CompanyName.ToUpper().Contains(query) select t).ToList();

                    BindTransactions(mainlist);
                    break;
                case "notifications":

                    break;
                case "personnel":

                    break;
                case "assets":
                    var assetlist = (from a in Application[(Session["guid"] as string)] as List<Asset>
                    where 
                                       a.AssetName.ToUpper().Contains(query)
                                    || a.CalibrationCompany.ToUpper().Contains(query)
                                    || a.DateRecievedString.ToUpper().Contains(query)
                                    || a.DateShippedString.ToUpper().Contains(query)
                                    || a.Description.ToUpper().Contains(query)
                                    || a.PersonShipping.ToUpper().Contains(query)
                                    || a.ServiceEngineer.ToUpper().Contains(query)
                                    || a.ShipTo.ToUpper().Contains(query)
                                     select a).ToList();
                    BindAssets(assetlist);
                    break;
                default:
                    break;
            }
            UpdateAllUpdatePanels();
        }

        //LEFT PANEL ACTIONS
        private void DirTree_SelectedNodeChanged(object sender, EventArgs e)
        {
            var tree = sender as TreeView;
            var selected = tree.SelectedNode;
            var parent = selected.Parent;
            var value = tree.SelectedValue;
            if (parent==null)
            {
                LoadSettingsView(null);
                return;
            }
            switch (parent.Value)
            {
                case "Control Panel":
                    //High Level Selected
                    switch (selected.Value as string)
                    {
                        case "GeneralSettings":
                            LoadSettingsView(null);
                            break;
                        case "CustomerNode":
                            LoadCustomersView(null);
                            break;
                        case "TransactionNode":
                            LoadTransactionsView(null);
                            break;
                        case "PersonnelNode":
                            LoadPersonnelView(null);
                            break;
                        case "NoticesNode":
                            LoadNoticeView(null);
                            break;
                        case "AssetsNode":
                            LoadAssetView(null);
                            break;
                        case "CertificatesNode":
                            LoadCertificateView(null);
                            break;
                        default:
                            break;
                    }
                    break;
                case "GeneralSettings":
                    LoadSettingsView(value);
                    break;
                case "CertificatesNode":
                    LoadCertificateView(null);
                    break;

                case "CustomerNode":
                    var customer = from c in Global.Library.Settings.Customers where value.Contains(c.CompanyName) && value.Contains(c.Address) select c;
                    if (customer.Count() >0)
                    {
                        LoadCustomerView(customer.First());
                    }
                    
                    break;
                case "TransactionNode":
                    LoadTransactionsView(value);
                    break;
                case "PersonnelNode":
                    switch (selected.Value)
                    {
                        case "Office Personnel":
                            LoadOfficePersonnelView();
                            break;
                        case "Field Personnel":
                            LoadFieldPersonnelView();
                            break;
                        default:
                            break;
                    }
                    //LoadPersonnelView(value);
                    break;
                case "NoticesNode":
                    LoadNoticeView(value);
                    break;
                case "AssetsNode":
                    LoadSingleAssetView(Pull.Asset(value));
                    break;
                default:
                    break;
            }
        }

        private void LoadBatabaseSettingsView()
        {
            Session["cv"] = "settings-db";
            SettingsMultiview.ActiveViewIndex = 2;
            AppRightPanelMultiView.SetActiveView(SettingsView);
            try
            {
                TESTMODE_CHECKBOX.Checked = Global.Library.Settings.TESTMODE;
                ups_pwd.Value = Global.Library.Settings.UpsAccount.P;
                ups_aln.Value = Global.Library.Settings.UpsAccount.A;
                ups_userid.Value = Global.Library.Settings.UpsAccount.I;
                ups_shippernumber.Value = Global.Library.Settings.UpsAccount.N;
            }
            catch { }
            try
            {
                shipmsgbox.Value = Global.Library.Settings.ShipperNotification;
                checkinmsgbox.Value = Global.Library.Settings.CheckInMessage;
                checkoutmsgbox.Value = Global.Library.Settings.CheckOutMessage;
                notificationmsgbox.Value = Global.Library.Settings.NotificationMessage;
            }
            catch
            {

            }
            AppRightPanelUpdatePanel.Update();
        }

        private void LoadUserSettingsView()
        {
            Session["cv"] = "settings-user";
            BindUsersAndRoles();
            SettingsMultiview.ActiveViewIndex = 1;
            AppRightPanelMultiView.SetActiveView(SettingsView);
            try
            {
                TESTMODE_CHECKBOX.Checked = Global.Library.Settings.TESTMODE;
                ups_pwd.Value = Global.Library.Settings.UpsAccount.P;
                ups_aln.Value = Global.Library.Settings.UpsAccount.A;
                ups_userid.Value = Global.Library.Settings.UpsAccount.I;
                ups_shippernumber.Value = Global.Library.Settings.UpsAccount.N;
            }
            catch { }
            try
            {
                shipmsgbox.Value = Global.Library.Settings.ShipperNotification;
                checkinmsgbox.Value = Global.Library.Settings.CheckInMessage;
                checkoutmsgbox.Value = Global.Library.Settings.CheckOutMessage;
                notificationmsgbox.Value = Global.Library.Settings.NotificationMessage;
            }
            catch
            {

            }
            AppRightPanelUpdatePanel.Update();
        }

        private void LoadFieldPersonnelView()
        {
            Session["cv"] = "fieldpersonnel";
            PersonnelMultiView.ActiveViewIndex = 2;
            PersonnelMainViewUpdatePanel.Update();
            BindFieldPersonnel(Global.Library.Settings.ServiceEngineers);
            //BindOfficePersonnel(Global.Library.Settings.ShippingPersons);
            AppRightPanelMultiView.SetActiveView(PersonnelView);
            PersonnelMultiView.SetActiveView(PersonnelFieldView);
            PersonnelViewMasterUpdatePanel.Update();
            AppRightPanelUpdatePanel.Update();
        }

        private void LoadOfficePersonnelView()
        {
            Session["cv"] = "officepersonnel";
            PersonnelMultiView.ActiveViewIndex = 1;
            PersonnelMainViewUpdatePanel.Update();
            //BindFieldPersonnel(Global.Library.Settings.ServiceEngineers);
            BindOfficePersonnel(Global.Library.Settings.ShippingPersons);
            AppRightPanelMultiView.SetActiveView(PersonnelView);
            PersonnelMultiView.SetActiveView(PersonnelOfficeView);
            PersonnelViewMasterUpdatePanel.Update();
            AppRightPanelUpdatePanel.Update();
        }

        private void LoadControlPanelMainView()
        {
           
        }

        private void LoadSettingsView(string input)
        {
            switch (input)
            {
                case "UserSettings":
                    LoadUserSettingsView();
                    return;
                case "DatabaseSettings":
                    LoadBatabaseSettingsView();
                    return;
                default:
                    break;
            }
            Session["cv"] = "settings";
            //BindUsersAndRoles();
            SettingsMultiview.ActiveViewIndex = 0;
            AppRightPanelMultiView.SetActiveView(SettingsView);
            try
            {
                TESTMODE_CHECKBOX.Checked = Global.Library.Settings.TESTMODE;
                ups_pwd.Value = Global.Library.Settings.UpsAccount.P;
                ups_aln.Value = Global.Library.Settings.UpsAccount.A;
                ups_userid.Value = Global.Library.Settings.UpsAccount.I;
                ups_shippernumber.Value = Global.Library.Settings.UpsAccount.N;
            }
            catch { }
            try
            {
                shipmsgbox.Value = Global.Library.Settings.ShipperNotification;
                checkinmsgbox.Value = Global.Library.Settings.CheckInMessage;
                checkoutmsgbox.Value = Global.Library.Settings.CheckOutMessage;
                notificationmsgbox.Value = Global.Library.Settings.NotificationMessage;
            }
            catch
            {

            }
            AppRightPanelUpdatePanel.Update();
        }

        private void LoadTransactionsView(string input)
        {
            Session["cv"] = "transactions";
            BindTransactions(Pull.Transactions());
            AppRightPanelMultiView.SetActiveView(TransactionsView);
            AppRightPanelUpdatePanel.Update();
        }
        private void LoadCustomersView(string input)
        {
            Session["cv"] = "customers";
            BindCustomers(Global.Library.Settings.Customers);
            AppRightPanelMultiView.SetActiveView(CustomersView);
            AppRightPanelUpdatePanel.Update();
        }

        private void LoadPersonnelView(string input)
        {
            Session["cv"] = "personnel";
            PersonnelMultiView.ActiveViewIndex = 0;
            BindStaticEmails(Global.Library.Settings.StaticEmails);
            BindFieldPersonnel(Global.Library.Settings.ServiceEngineers);
            BindOfficePersonnel(Global.Library.Settings.ShippingPersons);            
            AppRightPanelMultiView.SetActiveView(PersonnelView);
            AppRightPanelUpdatePanel.Update();
        }

        private void LoadNoticeView(string input)
        {
            Session["cv"] = "notifications";
            BindNotifications(Pull.Notifications());
            AppRightPanelMultiView.SetActiveView(NotificationsView);
            AppRightPanelUpdatePanel.Update();
        }

        private void LoadCertificateView(string input)
        {
            Session["cv"] = "certificates";
            BindCertificates(Pull.Certificates().Calibrations);
            AppRightPanelMultiView.SetActiveView(CertificateView);
            AppRightPanelUpdatePanel.Update();
        }

        private void LoadAssetView(string input)
        {
            Session["cv"] = "assets";
            BindAssets(Application[(Session["guid"] as string)] as List<Asset>);
            AppRightPanelMultiView.SetActiveView(AssetsView);
            AppRightPanelUpdatePanel.Update();
        }

        private void LoadSingleAssetView(Asset input)
        {
            Session["cv"] = "asset";
            Session["CreatorAsset"] = input;
            BindAndUpdateSingleAssetView(input);
            AppRightPanelMultiView.SetActiveView(SingleAssetView);
            AppRightPanelUpdatePanel.Update();
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(SingleAssetImageUploadBtn);
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(SingleAssetDocumentFileUploadBtn);
        }

        private void LoadCustomerView(Customer input)
        {
            Session["cv"] = "customer";
            Session["CurrentCustomer"] = input;
            BindAndUpdateCustomer(input);
            AppRightPanelMultiView.SetActiveView(CustomerView);
            AppRightPanelUpdatePanel.Update();
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(CustomerViewDocumentsBtn);
        }              

        private void LoadControlPanelApp()
        {
           

            var rootnode = new TreeNode("Control Panel");

            TreeNode SettingsNode = new TreeNode("Settings");
            SettingsNode.Value = "GeneralSettings";
                //TreeNode GeneralSettingsNode = new TreeNode("General");
                //GeneralSettingsNode.Value = "GeneralSettings";
                //SettingsNode.ChildNodes.Add(GeneralSettingsNode);
                TreeNode UserSettingsNode = new TreeNode("User");
                UserSettingsNode.Value = "UserSettings";
                SettingsNode.ChildNodes.Add(UserSettingsNode);
                TreeNode DbSettingsNode = new TreeNode("Database");
                DbSettingsNode.Value = "DatabaseSettings";
                SettingsNode.ChildNodes.Add(DbSettingsNode);

            TreeNode TransactionNode = new TreeNode("Transactions");
            TransactionNode.Value = "TransactionNode";

            TreeNode CustomerNode = new TreeNode("Customers");
            CustomerNode.Value = "CustomerNode";

            TreeNode PersonnelNode = new TreeNode("Personnel");
            PersonnelNode.Value = "PersonnelNode";

            TreeNode NoticesNode = new TreeNode("Notifications");
            NoticesNode.Value = "NoticesNode";

            TreeNode AssetsNode = new TreeNode("Assets");
            AssetsNode.Value = "AssetsNode";

            TreeNode CertificateNode = new TreeNode("Certificates");
            CertificateNode.Value = "CertificatesNode";

            DirTree.ShowLines = true;
            //Settings

            //transactions
            try
            {
                var ds = Pull.Transactions().OrderBy(w => w.Date).ToList();
                ds.ForEach((t) => {
                    TreeNode node = new TreeNode();
                    node.Text = t.Date.ToShortDateString() + " - " + t.Name;
                    node.Value = t.TransactionID;
                    //node.NavigateUrl = "/Account/OutCart.aspx?pend=" + t.TransactionID;
                   
                    TransactionNode.ChildNodes.Add(node);

                });
            }
            catch
            { }
             
            //Customers
            try
            {               
                var ds = Global.Library.Settings.Customers.OrderBy(w => w.CompanyName).ToList();
                ds.ForEach((t) => {
                    TreeNode node = new TreeNode();
                    node.Text = t.CompanyName;
                    node.ToolTip = t.Address;
                    node.Value = t.CompanyName + "###" + t.Address;
                   
                    CustomerNode.ChildNodes.Add(node);

                });
            }
            catch
            {
                throw;
            }
            //Personnel
            TreeNode OfficeNode = new TreeNode("Office Personnel");
            try
            {
                var ds = Global.Library.Settings.ShippingPersons.OrderBy(w => w.Name).ToList();
                ds.ForEach((t) => {
                    TreeNode node = new TreeNode();
                    node.Text = t.Name;
                    node.ToolTip = t.Email;
                    node.Value = t.Email;
                    OfficeNode.ChildNodes.Add(node);

                });
            }
            catch {throw;}
            TreeNode FieldNode = new TreeNode("Field Personnel");
            try
            {
                var ds = Global.Library.Settings.ServiceEngineers.OrderBy(w => w.Name).ToList();
                ds.ForEach((t) => {
                    TreeNode node = new TreeNode();
                    node.Text = t.Name;
                    node.ToolTip = t.Email;
                    node.Value = t.Email;
                    FieldNode.ChildNodes.Add(node);

                });
            }
            catch { throw; }
            PersonnelNode.ChildNodes.Add(OfficeNode);
            PersonnelNode.ChildNodes.Add(FieldNode);
            //Notices
            try
            {
                var ds = Pull.Notifications().OrderBy(w => w.DaysUntil).ToList();
                ds.ForEach((t) => {
                    TreeNode node = new TreeNode();
                    node.Text = t.DaysUntil+" Days: "+t.Name;
                    node.ToolTip = t.Email;
                    node.Value = t.Guid;
                    NoticesNode.ChildNodes.Add(node);

                });
            }
            catch { throw; }
            //Assets
            try
            {
                var ds = (Application[(Session["guid"] as string)] as List<Asset>).OrderBy(w => w.AssetNumber).ToList();
                ds.ForEach((t) => {
                    TreeNode node = new TreeNode();
                    node.Text = t.AssetNumber + ": " + t.AssetName;
                    node.ToolTip = t.Description;
                    node.Value = t.AssetNumber;
                    AssetsNode.ChildNodes.Add(node);

                });
            }
            catch { throw; }
            //Certificates
            try
            {
                var db = Pull.Certificates();
                var ds = db.Calibrations;
                ds.ForEach((t) => {
                    TreeNode node = new TreeNode();
                    node.Text = t.FileName;
                    node.ToolTip = t.AssetNumber;
                    node.Value = t.FileName;
                    CertificateNode.ChildNodes.Add(node);

                });
            }
            catch { throw; }

            rootnode.ChildNodes.Add(SettingsNode);
            rootnode.ChildNodes.Add(TransactionNode);
            rootnode.ChildNodes.Add(CustomerNode);
            rootnode.ChildNodes.Add(PersonnelNode);
            rootnode.ChildNodes.Add(NoticesNode);
            rootnode.ChildNodes.Add(AssetsNode);
            rootnode.ChildNodes.Add(CertificateNode);


            this.DirTree.Nodes.Add(rootnode);
            DirTree.ExpandDepth = 1;



        }

        public void ToggleLeftPanel()
        {
            var script = @"$(document).ready(function (){ ToggleList();});";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ToggleListScript", script, true);
        }

        public void BindCertificates(List<CalibrationData> list = null)
        {
            CertificateRepeater.DataSource = list;
            CertificateRepeater.DataBind();
            CertificateUpdatePanel.Update();
            AppRightPanelUpdatePanel.Update();
        }

        

        protected void ViewAll_Click(object sender, EventArgs e)
        {

        }
        
        #region Customer Functions
        public void BindCustomers(List<Helpers.Customer> customers)
        {
            Application["CustomerList"] = customers;
            CustomersRepeater.DataSource = customers.OrderBy((w) => w.CompanyName);
            CustomersRepeater.DataBind();
            CustomerUpdatePanel.Update();
        }

        protected void DeleteCustomerBtn_Command(object sender, CommandEventArgs e)
        {
            try
            {
                var customer = (from a in Global.Library.Settings.Customers where a.CompanyName == (string)e.CommandName && a.Postal == (string)e.CommandArgument select a).FirstOrDefault();
                Global.Library.Settings.Customers.Remove(customer);
                Push.AppSettings();
            }
            catch { }
        }

        
        #endregion
        #region Transaction Functions
        public void BindTransactions(List<Helpers.PendingTransaction> trans)
        {
            TransactionRepeater.DataSource = trans;
            TransactionRepeater.DataBind();
            TransactionListUpdatePanel.Update();
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
        #endregion
        #region Personnel Functions
        public void BindStaticEmails(List<Helpers.EmailAddress> statics)
        {
            StaticEmailRepeater.DataSource = statics;
            StaticEmailRepeater.DataBind();            
            PersonnelMainViewUpdatePanel.Update();
        }

        public void BindOfficePersonnel(List<Helpers.EmailAddress> personnel)
        {
            PersonnelOfficeRepeater.DataSource = personnel;
            PersonnelOfficeRepeater.DataBind();
            PersonnelOfficeMainViewRepeater.DataSource = personnel;
            PersonnelOfficeMainViewRepeater.DataBind();            
            PersonnelOfficeUpdatePanel.Update();
            PersonnelMainViewUpdatePanel.Update();

        }

        public void BindFieldPersonnel(List<Helpers.EmailAddress> personnel)
        {
            PersonnelFieldRepeater.DataSource = personnel;
            PersonnelFieldRepeater.DataBind();
            PersonnelFieldMainViewRepeater.DataSource = personnel;
            PersonnelFieldMainViewRepeater.DataBind();           
            PersonnelFieldUpdatePanel.Update();
            PersonnelMainViewUpdatePanel.Update();
        }

        protected void DeleteFieldPersonnelBtn_Command(object sender, CommandEventArgs e)
        {

        }

        protected void DeleteOfficePersonnel_Command(object sender, CommandEventArgs e)
        {

        }
        #endregion
        #region Notification Functions
        public void BindNotifications(List<Notification.NotificationSystem.Notice> notices)
        {
            Application["NotificationList"] = notices;
            NotificationsRepeater.DataSource = notices;
            NotificationsRepeater.DataBind();
            NotificationsViewUpdatePanel.Update();
        }

        protected void DeleteNoticeBtn_Command(object sender, CommandEventArgs e)
        {

        }

        protected void SendNoticeBtn_Command(object sender, CommandEventArgs e)
        {

        }
        #endregion
        #region Asset Functions
        public void BindAssets(List<Helpers.Asset> assets)
        {
            AssetsRepeater.DataSource = assets;
            AssetsRepeater.DataBind();
            AssetsUpdatePanel.Update();
        }

        protected void DeleteAssetBtn_Command(object sender, CommandEventArgs e)
        {

        }
        #endregion
        #region Settings Functions

        private void BindUsersAndRoles()
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            if (!roleManager.RoleExists("NoRole"))
            {
                roleManager.Create(new IdentityRole("NoRole"));
            }
            var nonRoledUsers = (from user in manager.Users where user.Roles.Count == 0 orderby user.Email select user).ToList();
            foreach (var user in nonRoledUsers)
            {
                manager.AddToRole(user.Id, "NoRole");
            }

            var roles = (from role in roleManager.Roles orderby role.Name select role).ToRoleBindingList();

            RolesAndUsersRepeater.DataSource = roles;
            RolesAndUsersRepeater.DataBind();

            var users = (from user in manager.Users orderby user.Email select user).ToUserBindingList();
            UserRepeater.DataSource = users;
            UserRepeater.DataBind();
            RoleDropDown.Items.Clear();
            UserDropDownList.Items.Clear();
            RoleDropDown.DataSource = GetRoleNames();
            UserDropDownList.DataSource = GetUserNames();
            RoleDropDown.DataBind();
            UserDropDownList.DataBind();


            RolesUpdatePanel.Update();
            UserUpdatePanel.Update();

        }
        private void BindAssetAdmin()
        {
            //try
            //{
            //    ups_pwd.Value = Global.Library.Settings.UpsAccount.P;
            //    ups_aln.Value = Global.Library.Settings.UpsAccount.A;
            //    ups_userid.Value = Global.Library.Settings.UpsAccount.I;
            //    ups_shippernumber.Value = Global.Library.Settings.UpsAccount.N;
            //}
            //catch { }
            //try
            //{
            //    shipmsgbox.Value = Global.Library.Settings.ShipperNotification;
            //    checkinmsgbox.Value = Global.Library.Settings.CheckInMessage;
            //    checkoutmsgbox.Value = Global.Library.Settings.CheckOutMessage;
            //    notificationmsgbox.Value = Global.Library.Settings.NotificationMessage;
            //}
            //catch
            //{

            //}
            
        }

        private List<ApplicationUser> GetUsersForRole(string role)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            foreach (var user in manager.Users)
            {
                foreach (var r in user.Roles)
                {
                    if (r.RoleId == role)
                    {
                        users.Add(user);
                    }
                    if (r.UserId == user.Id)
                    {
                        users.Add(user);
                    }
                }
            }
            return users;
        }
        private bool AddUserToRole(string username, string role)
        {
            try
            {
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var user = manager.AddToRole(username, role);
                return true;
            }
            catch
            {

                return false;
            }
        }
        private bool RemoveUserFromRole(string userid, string roleid)
        {
            try
            {
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                /*var irole = roleManager.FindById(role);
               IdentityUserRole remItem=null;
               foreach (var u in irole.Users)
               {
                   if (u.UserId==userid && u.RoleId==role)
                   {
                       remItem = u;
                   }
               }
               var aa = irole.Users.Remove(remItem);
               */
                string rolename = "";
                foreach (var r in roleStore.Roles.ToList())
                {
                    if (r.Id == roleid)
                    { rolename = r.Name; }
                }
                var user = manager.RemoveFromRole(userid, rolename);
                return user.Succeeded;
            }
            catch
            {

                return false;
            }
        }
        private bool RemoveUser(string userid)
        {
            try
            {
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                if (userid == null)
                {
                    return false;
                }

                var user = manager.FindById(userid);
                var logins = user.Logins;
                var rolesForUser = manager.GetRoles(userid);

                using (var transaction = new ApplicationDbContext().Database.BeginTransaction())
                {
                    foreach (var login in logins.ToList())
                    {
                        manager.RemoveLogin(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
                    }

                    if (rolesForUser.Count() > 0)
                    {
                        foreach (var item in rolesForUser.ToList())
                        {
                            // item should be the name of the role
                            var result = manager.RemoveFromRole(user.Id, item);
                        }
                    }

                    manager.Delete(user);
                    transaction.Commit();
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        protected void CreateRoleButton_Click(object sender, EventArgs e)
        {
            try
            {
                var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                //roleManager.Create(new IdentityRole(RoleName.Text));             

                BindAssetAdmin();
            }
            catch
            {
                this.SiteMaster().ShowError("Could Not Create Role.");

            }

        }

        protected void DeleteUser_Command(object sender, CommandEventArgs e)
        {
            var a = e.CommandArgument as string;
            var b = e.CommandName as string;
            RemoveUser(b);
            BindUsersAndRoles();
        }
        protected bool ChangeUserRole(string userid, string role)
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            try
            {
                var user = manager.GetRoles(userid);
                user.ToList().ForEach((r)=> 
                {
                    manager.RemoveFromRole(userid, r);
                });
                if (!manager.IsInRole(userid, role))
                {
                    manager.AddToRole(userid, role);
                    
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        protected void ChangeRole_Command(object sender, CommandEventArgs e)
        {
            var roleId = e.CommandArgument as string;
            var name = e.CommandName as string;
            try
            {
                foreach (RepeaterItem i in RolesAndUsersRepeater.Items)
                {

                    Repeater repeater = (Repeater)i.Controls[1];
                    foreach (RepeaterItem item in repeater.Items)
                    {
                        var changebutton = item.FindControl("ChangeRole") as Button;
                        var temprole = changebutton.CommandName;
                        if (temprole == name)
                        {
                            var dropdown = item.FindControl("RoleDropDown") as DropDownList;
                            if (dropdown != null)
                            {
                                var selected = dropdown.SelectedValue;

                                var result = ChangeUserRole(name, selected);
                                result = RemoveUserFromRole(name, roleId);

                                return;
                            }
                        }

                    }

                }
            }
            catch { }
        }
        protected List<string> GetRoleNames()
        {
            List<string> names = new List<string>();
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            foreach (var role in roleStore.Roles)
            {
                names.Add(role.Name);
            }
            return names;
        }
        protected List<string> GetUserNames()
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var users = (from user in manager.Users orderby user.Email select user.Email).ToList();
            return users;
        }

        protected void RolesAndUsersRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        protected void DeleteFromRole_Command(object sender, CommandEventArgs e)
        {
            var a = e.CommandArgument as string;
            var b = e.CommandName as string;
            RemoveUserFromRole(b, a);
            BindUsersAndRoles();
        }

        protected void CopyRole_Command(object sender, CommandEventArgs e)
        {
            var roleId = e.CommandArgument as string;
            var name = e.CommandName as string;
            try
            {
                foreach (RepeaterItem i in RolesAndUsersRepeater.Items)
                {

                    Repeater repeater = (Repeater)i.Controls[1];
                    foreach (RepeaterItem item in repeater.Items)
                    {
                        var changebutton = item.FindControl("ChangeRole") as Button;
                        var temprole = changebutton.CommandName;
                        if (temprole == name)
                        {
                            var dropdown = item.FindControl("RoleDropDown") as DropDownList;
                            if (dropdown != null)
                            {
                                var selected = dropdown.SelectedValue;
                                ChangeUserRole(name, selected);
                                return;
                            }
                        }

                    }

                }
            }
            catch { }
            BindUsersAndRoles();
        }

        protected void PullSQL_Click(object sender, EventArgs e)
        {
            Global.AssetCache = Pull.Assets();
            Application[(Session["guid"] as string)] = Global.AssetCache;
            //PopNotify("Complete", "Library Pulled From SQL");

        }

        protected void DeleteSQL_Click(object sender, EventArgs e)
        {
            SQL_Request req = new SQL_Request().OpenConnection();
            try
            {


                //request all assets
                req.GetAssets(false);
                if (req.Tag != null)
                {
                    var cloud = req.Tag as List<Asset>;

                    foreach (var asset in cloud)
                    {
                        try
                        {
                            req.DeleteAsset(asset.AssetNumber, false);
                        }
                        catch
                        { //PopNotify("Error", "Error Deleting SQL Library");
                        }
                    }

                    req.CloseConnection();
                    Global.AssetCache = new List<Asset>();
                }
            }
            catch
            {
                //PopNotify("Error", "Error Deleting SQL Library");
            }
            finally
            {
                req.CloseConnection();
                //PopNotify("Complete", "SQL Library Deleted");
            }


        }

        protected void SendSQL_Click(object sender, EventArgs e)
        {
            try
            {
                SQL_Request req = new SQL_Request().OpenConnection();

                //request all assets
                req.GetAssets(false);


                //merge all assets

                //post merged assets as new master DB
                if (req.Tag != null)
                {
                    var cloud = req.Tag as List<Asset>;
                    //upload assets
                    foreach (Asset a in Global.AssetCache)
                    {
                        try
                        {
                            var lookup = cloud.FindAssetByNumber(a.AssetNumber);
                            if (lookup == null)
                            {
                                req.AddAsset(a, false);

                            }
                            else
                            if (lookup.AssetNumber == a.AssetNumber)
                            {
                                req.UpdateAsset(a, false);

                            }
                        }
                        catch
                        { //PopNotify("Error", "Error Pushing Library To SQL");
                        }
                    }
                    //upload settings
                    //upload history
                    //upload notifications
                    //upload Calibrations

                }
                req.CloseConnection();

            }
            catch
            {
                //PopNotify("Error", "Error Pushing Library To SQL");
            }
            //PopNotify("Complete", "Library Pushed To SQL");
        }

        protected void SendSettingsSQL_Click(object sender, EventArgs e)
        {
            AssetController.PushSettings(Global.Library.Settings);
            //PopNotify("Success", "Settings Pushed To SQL");
            
        }

        protected void PullSettings_Click(object sender, EventArgs e)
        {
            Global.Library.Settings = AssetController.GetSettings();
            //PopNotify("Success", "Settings Pulled From SQL");
           
        }

        protected void DeleteSettingsSQL_Click(object sender, EventArgs e)
        {
            try
            {
                SQL_Request req = new SQL_Request().OpenConnection();
                req.SettingsDelete();
                Global.Library.Settings = new Settings();
            }
            catch { }
        }

        protected void UploadLibrary_Click(object sender, EventArgs e)
        {
            try
            {

                var name = Server.MapPath("/Account/library.xml");

                LibraryUploader.PostedFile.SaveAs(name);
                var doc = new XmlDocument();
                using (StreamReader reader = new StreamReader(LibraryUploader.PostedFile.InputStream))
                {
                    doc.LoadXml(reader.ReadToEnd());
                }

                Global.AssetCache = new List<Asset>();
                try
                {
                    XmlNodeList elemList = doc.GetElementsByTagName("Asset");
                    if (elemList.Count == 0)
                    {
                        elemList = doc.GetElementsByTagName("Assets");
                    }
                    foreach (XmlElement elem in elemList)
                    {
                        var a = elem.ToAsset();
                        Global.AssetCache.Add(a);

                    }
                }
                catch
                {
                    //PopNotify("Error", "");
                    Page.SiteMaster().ShowError("Error Adding Assets");

                }
                try
                {
                    if (Global.Library.Settings == null) Global.Library.Settings = new Settings();
                    if (Global.Library.Settings.Customers == null) Global.Library.Settings.Customers = new List<Customer>();
                    XmlNodeList custElemList = doc.GetElementsByTagName("Customer");
                    foreach (XmlElement cust in custElemList)
                    {
                        //create a customer
                        Customer newCustomer = new Customer();
                        newCustomer.AccountNumber = cust.Attributes["AccountNumber"].Value;
                        newCustomer.AccPostalCd = cust.Attributes["AccPostalCd"].Value;
                        newCustomer.Address = cust.Attributes["Address"].Value;
                        newCustomer.Address2 = cust.Attributes["Address2"].Value;
                        newCustomer.Address3 = cust.Attributes["Address3"].Value;
                        newCustomer.Attn = cust.Attributes["Attn"].Value;
                        newCustomer.City = cust.Attributes["City"].Value;
                        newCustomer.CompanyName = cust.Attributes["CompanyName"].Value;
                        newCustomer.ConsInd = cust.Attributes["ConsInd"].Value;
                        newCustomer.Country = cust.Attributes["Country"].Value;
                        newCustomer.EmailAddress = new EmailAddress() { Email = cust.Attributes["Email"].Value };
                        newCustomer.Fax = cust.Attributes["Fax"].Value;
                        newCustomer.LocID = cust.Attributes["LocID"].Value;
                        newCustomer.NickName = cust.Attributes["NickName"].Value;
                        newCustomer.OrderNumber = cust.Attributes["OrderNumber"].Value;
                        newCustomer.PackageWeight = cust.Attributes["PackageWeight"].Value;
                        newCustomer.Phone = cust.Attributes["Phone"].Value;
                        newCustomer.Postal = cust.Attributes["Postal"].Value;
                        newCustomer.ResInd = cust.Attributes["ResInd"].Value;
                        newCustomer.State = cust.Attributes["State"].Value;
                        newCustomer.USPSPOBoxIND = cust.Attributes["USPSPOBoxIND"].Value;
                        Global.Library.Settings.Customers.Add(newCustomer);
                    }
                }
                catch
                {
                    //PopNotify("Error", "");
                    Page.SiteMaster().ShowError("Error adding customers");

                }
                //get engineers
                try
                {
                    XmlNodeList engElemList = doc.GetElementsByTagName("Engineer");
                    foreach (XmlElement eng in engElemList)
                    {
                        //create a customer
                        EmailAddress newCustomer = new EmailAddress();
                        newCustomer.Email = eng.Attributes["Email"].Value;
                        newCustomer.Name = eng.Attributes["Name"].Value;

                        Global.Library.Settings.ServiceEngineers.Add(newCustomer);
                    }
                }
                catch
                {
                    //PopNotify("Error", "");
                    Page.SiteMaster().ShowError("Error adding Engineers");

                }
                try
                {
                    if (Global.Library.Settings.ShippingPersons == null) Global.Library.Settings.ShippingPersons = new List<EmailAddress>();
                    //get shipping personel
                    XmlNodeList shipElemList = doc.GetElementsByTagName("Shipper");
                    foreach (XmlElement eng in shipElemList)
                    {
                        //create a customer
                        EmailAddress newCustomer = new EmailAddress();
                        newCustomer.Email = eng.Attributes["Email"].Value;
                        newCustomer.Name = eng.Attributes["Name"].Value;

                        Global.Library.Settings.ShippingPersons.Add(newCustomer);
                    }
                }
                catch
                {
                    //PopNotify("Error", "");
                    Page.SiteMaster().ShowError("Error adding Shipping Personnel");

                }

                //get settings

                try
                {
                    if (Global.Library.Settings.StaticEmails == null) Global.Library.Settings.StaticEmails = new List<EmailAddress>();
                    //get shipping personel
                    XmlNodeList staticemails = doc.GetElementsByTagName("StaticEmails");
                    foreach (XmlElement staticemail in staticemails)
                    {
                        try
                        {
                            XmlNodeList emails = doc.GetElementsByTagName("StaticEmail");
                            //create a customer
                            EmailAddress newStatic = new EmailAddress();
                            newStatic.Email = staticemail.Attributes["Email"].Value;
                            newStatic.Name = staticemail.Attributes["Name"].Value;

                            Global.Library.Settings.StaticEmails.Add(newStatic);
                        }
                        catch { }

                    }

                    string outmsg = doc.GetElementsByTagName("CheckOutMessage").Item(0).InnerText.Sanitize();
                    Global.Library.Settings.CheckOutMessage = outmsg;

                    string inmsg = doc.GetElementsByTagName("CheckInMessage").Item(0).InnerText.Sanitize();
                    Global.Library.Settings.CheckInMessage = inmsg;

                    string noticemsg = doc.GetElementsByTagName("NotificationMessage").Item(0).InnerText.Sanitize();
                    Global.Library.Settings.NotificationMessage = noticemsg;

                    string shippernotice = doc.GetElementsByTagName("ShipperNotification").Item(0).InnerText.Sanitize();
                    Global.Library.Settings.ShipperNotification = shippernotice;
                }
                catch
                {
                    //PopNotify("Error", "");
                    Page.SiteMaster().ShowError("Error adding Shipping Personnel");

                }

               
            }
            catch
            {
                Page.SiteMaster().ShowError("Problem uploading library");

            }
            Push.AppSettings();
            Push.Library();

        }

        protected void UploadNotices_Click(object sender, EventArgs e)
        {
            var name = Server.MapPath("/App_Data/notice" + DateTime.Now.ToShortDateString().Replace(" ", "").Replace("/", "-") + ".xml");
            NoticeUploader.PostedFile.SaveAs(name);
            var doc = new XmlDocument();
            using (StreamReader reader = new StreamReader(NoticeUploader.PostedFile.InputStream))
            {
                if (reader.EndOfStream) return;
                doc.LoadXml(reader.ReadToEnd());
            }
            //check for nulls
            if (Global.Library.Settings == null) Global.Library.Settings = new Settings();
            if (Global.Library.Settings.Notifications_30_Day == null) Global.Library.Settings.Notifications_30_Day = new List<AssetNotification>();
            if (Global.Library.Settings.Notifications_15_Day == null) Global.Library.Settings.Notifications_15_Day = new List<AssetNotification>();

            XmlNodeList elemList = doc.GetElementsByTagName("Notice");
            foreach (XmlElement elem in elemList)
            {
                try
                {
                    AssetNotification a = elem.ToNotification();
                    EmailNotice notice = new EmailNotice();

                    notice.Scheduled = DateTime.Now.AddDays(30);
                    notice.NoticeType = NoticeType.Checkout;
                    notice.NoticeAction = Global.CheckoutAction;

                    notice.Assets.Add(a.AssetNumber);

                    notice.NoticeControlNumber = a.EmailsCSV;
                    notice.Body = Global.Library.Settings.NotificationMessage;
                    notice.Subject = "Asset Return Reminder";
                    notice.Emails.AddRange(a.Emails);
                    notice.EmailAddress = a.Emails[0];
                    Global.NoticeSystem.Add(notice);

                }
                catch { this.Page.SiteMaster(). ShowError("Problem Adding Timed Notice"); }
            }
            Push.NotificationSystem();
        }

        protected void UploadHistory_Click(object sender, EventArgs e)
        {
            try
            {
                var name = Server.MapPath("/App_Data/history" + DateTime.Now.ToShortDateString().Replace(" ", "").Replace("/", "-") + ".xml");
                HistoryUploader.PostedFile.SaveAs(name);
                var doc = new XmlDocument();
                using (StreamReader reader = new StreamReader(HistoryUploader.PostedFile.InputStream))
                {
                    if (reader.EndOfStream) return;
                    doc.LoadXml(reader.ReadToEnd());
                }
                //check for nulls
                if (Global.AssetCache == null)
                {
                    //PopNotify("Error", "No Library loaded");
                    return;
                }
                if (Global.AssetCache.Count == 0)
                {
                    //PopNotify("Error", "No Assests In Library");
                    return;
                }

                XmlNodeList elemList = doc.GetElementsByTagName("HistoryItems");
                foreach (XmlElement elem in elemList)
                {
                    try
                    {
                        var assetnumber = elem.GetAttribute("AssetNumber").Sanitize();
                        var currentAsset = Global.AssetCache.FindAssetByNumber(assetnumber);
                        foreach (XmlElement subelem in elem.GetElementsByTagName("Asset"))
                        {
                            try
                            {
                                Asset a = subelem.ToAsset();
                                a.IsHistoryItem = true;
                                currentAsset.History.History.Add(a);
                            }
                            catch
                            {
                                //PopNotify("Error", "Adding History Item To Library Asset");
                                return;
                            }
                        }
                    }
                    catch
                    {
                        //PopNotify("Error", "Loading HistoryItems");
                        return;
                    }

                }
                try
                {

                    //PopNotify("File Upload Complete", "History file upload complete");
                }
                catch { }
            }
            catch
            {
                //PopNotify("Error", "History file upload Error");

            }
        }

        #endregion

        protected void ExportLibraryBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GC.Collect();
                var libraryname = Server.MapPath("/Zip/lib.xml");
                var noticename = Server.MapPath("/Zip/notice.xml");
                var settingname = Server.MapPath("/Zip/settings.xml");
                Global.Library.Settings.SerializeToXmlFile(Global.Library.Settings, settingname);
                Global.Library.SerializeToXmlFile(Global.Library, libraryname);
                Global.NoticeSystem.SerializeToXmlFile(Global.NoticeSystem, noticename);
                var temp = "/Zip/" + Guid.NewGuid().ToString() + ".zip";
                temp = Server.MapPath(temp);
                var files = new List<string>() { libraryname, noticename, settingname };

                var images = (from i in Directory.GetFiles(Server.MapPath("/Account/Images")) select i).ToList();
                var labels = (from i in Directory.GetFiles(Server.MapPath("/Account/Labels")) select i).ToList();
                var packing = (from i in Directory.GetFiles(Server.MapPath("/Account/PackingLists")) select i).ToList();
                var PdfFiles = (from i in Directory.GetFiles(Server.MapPath("/Account/PdfFiles")) select i).ToList();
                var Receiving = (from i in Directory.GetFiles(Server.MapPath("/Account/Receiving")) select i).ToList();
                var templates = (from i in Directory.GetFiles(Server.MapPath("/Account/Templates")) select i).ToList();

                bool result = false;
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                        {
                            foreach (var file in files)
                            {
                                var name = file;
                                archive.CreateEntryFromFile(file, Path.GetFileName(name));
                            }
                            foreach (var file in images)
                            {
                                var name = file;
                                archive.CreateEntryFromFile(file, "Images/" + Path.GetFileName(name));
                            }
                            foreach (var file in labels)
                            {
                                var name = file;
                                archive.CreateEntryFromFile(file, "Labels/" + Path.GetFileName(name));
                            }
                            foreach (var file in packing)
                            {
                                var name = file;
                                archive.CreateEntryFromFile(file, "PackingSlips/" + Path.GetFileName(name));
                            }
                            foreach (var file in PdfFiles)
                            {
                                var name = file;
                                archive.CreateEntryFromFile(file, "PdfFiles/" + Path.GetFileName(name));
                            }
                            foreach (var file in Receiving)
                            {
                                var name = file;
                                archive.CreateEntryFromFile(file, "Receiving/" + Path.GetFileName(name));
                            }
                            foreach (var file in templates)
                            {
                                var name = file;
                                archive.CreateEntryFromFile(file, "Templates/" + Path.GetFileName(name));
                            }

                        }

                        using (var fileStream = new FileStream(temp, FileMode.Create))
                        {
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            memoryStream.CopyTo(fileStream);
                        }
                    }
                    result = true;
                }
                catch { result = false; }
                if (!result)
                {
                    GC.Collect();
                    GC.Collect();
                    Page.SiteMaster().ShowError("Problem exporting Library file...");
                }
                else
                {
                    GC.Collect();
                    GC.Collect();
                    string fileName = String.Empty;

                    string filePath = String.Empty;

                    filePath = temp;

                    fileName = System.IO.Path.GetFileName(filePath);
                    try
                    {
                        HttpContext.Current.Response.ClearContent();

                        HttpContext.Current.Response.ClearHeaders();

                        HttpContext.Current.Response.AddHeader("Content-Disposition", "inline; filename=" + fileName);

                        HttpContext.Current.Response.ContentType = "application/zip";

                        HttpContext.Current.Response.WriteFile(filePath);
                        GC.Collect();
                        HttpContext.Current.Response.End();
                    }
                    catch { GC.Collect(); }
                }
            }
            catch
            {
                Page.SiteMaster().ShowError("Problem exporting Library file...");
                GC.Collect();
            }

        }

        protected void ImportLibraryBtn_Click(object sender, EventArgs e)
        {
            
        }

        protected void SaveUpsAcctBtn_Click(object sender, EventArgs e)
        {
            if (Global.Library.Settings.UpsAccount == null)
            { Global.Library.Settings.UpsAccount = new UPSaccount(); }
            Global.Library.Settings.UpsAccount.P = ups_pwd.Value;
            Global.Library.Settings.UpsAccount.A = ups_aln.Value;
            Global.Library.Settings.UpsAccount.I = ups_userid.Value;
            Global.Library.Settings.UpsAccount.N = ups_shippernumber.Value;
            Push.AppSettings();
            UpdateAssetAdmin();
        }

        private void UpdateAssetAdmin()
        {
            UpdateAllUpdatePanels();
        }

        protected void ControlPanelSaveBtn_Click(object sender, EventArgs e)
        {
            var cv = Session["cv"] as string; //get current view 
            switch (cv)
            {
                case "settings":
                   
                    Global.Library.Settings.TESTMODE = TESTMODE_CHECKBOX.Checked;
                    break;
                case "customers":
                    break;
                case "settings-user":
                    Global.Library.Settings.NotificationMessage = notificationmsgbox.Value;
                    Global.Library.Settings.CheckOutMessage = checkoutmsgbox.Value;
                    Global.Library.Settings.CheckInMessage = checkinmsgbox.Value;
                    Global.Library.Settings.ShipperNotification = shipmsgbox.Value;
                    if (Global.Library.Settings.UpsAccount == null)
                    { Global.Library.Settings.UpsAccount = new UPSaccount(); }
                    Global.Library.Settings.UpsAccount.P = ups_pwd.Value;
                    Global.Library.Settings.UpsAccount.A = ups_aln.Value;
                    Global.Library.Settings.UpsAccount.I = ups_userid.Value;
                    Global.Library.Settings.UpsAccount.N = ups_shippernumber.Value;
                    break;
                case "settings-db":
                    Global.Library.Settings.NotificationMessage = notificationmsgbox.Value;
                    Global.Library.Settings.CheckOutMessage = checkoutmsgbox.Value;
                    Global.Library.Settings.CheckInMessage = checkinmsgbox.Value;
                    Global.Library.Settings.ShipperNotification = shipmsgbox.Value;
                    if (Global.Library.Settings.UpsAccount == null)
                    { Global.Library.Settings.UpsAccount = new UPSaccount(); }
                    Global.Library.Settings.UpsAccount.P = ups_pwd.Value;
                    Global.Library.Settings.UpsAccount.A = ups_aln.Value;
                    Global.Library.Settings.UpsAccount.I = ups_userid.Value;
                    Global.Library.Settings.UpsAccount.N = ups_shippernumber.Value;
                    break;
                case "transactions":
                    break;
                case "notifications":

                    break;
                case "personnel":

                    break;
                case "assets":

                    break;
                default:
                    break;
            }
            Push.AppSettings();
            UpdateAllUpdatePanels();
           
        }

        protected void ControlPanelCreateBtn_Click(object sender, EventArgs e)
        {
            var cv = Session["cv"] as string; //get current view 
            switch (cv)
            {
                case "settings":

                    break;
                case "customers":
                    break;
                case "transactions":
                    break;
                case "notifications":

                    break;
                case "personnel":

                    break;
                case "assets":

                    break;
                default:
                    break;
            }
            Push.AppSettings();
            Push.Library();
        }

        protected void ModalOk_Click(object sender, EventArgs e)
        {

        }

        protected void ModalCancel_Click(object sender, EventArgs e)
        {

        }

        protected void COPOkBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool isedit = Convert.ToBoolean(IsOPEdit.Value);
                if (!isedit)
                {
                    var local = Global.Library.Settings.ShippingPersons.Find((x) => x.Email == COPEmailTextBox.Value);
                    if (local != null)
                    {
                        UpdateStatus(COPEmailTextBox.Value + " already exists");
                        return;
                    }
                    var name = COPNameTextBox.Value;
                    var email = COPEmailTextBox.Value;
                    EmailAddress NEWSTATIC = new EmailAddress() { Name = name, Email = email };
                    Global.Library.Settings.ShippingPersons.Add(NEWSTATIC);
                }
                else
                {
                    var local = Global.Library.Settings.ShippingPersons.Find((x) => x.Email == COPEmailTextBox.Value);
                    local.Email = COPEmailTextBox.Value;
                    local.Name = COPNameTextBox.Value;
                }
                Push.AppSettings();
                UpdateStatus("Personnel added: " + COPEmailTextBox.Value);
                BindAndUpdatePersonnel(Global.Library.Settings.ShippingPersons, Global.Library.Settings.ServiceEngineers);
            }
            catch { UpdateStatus("Error adding Personnel"); }
        }

        protected void CFPOkBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool isedit = Convert.ToBoolean(IsFPEdit.Value);
                if (!isedit)
                {
                    var local = Global.Library.Settings.ServiceEngineers.Find((x) => x.Email == CFPEmailTextBox.Value);
                    if (local != null)
                    {
                        UpdateStatus(CFPEmailTextBox.Value + " already exists");
                        return;
                    }
                    var name = CFPNameTextBox.Value;
                    var email = CFPEmailTextBox.Value;
                    EmailAddress NEWSTATIC = new EmailAddress() { Name = name, Email = email };
                    Global.Library.Settings.ServiceEngineers.Add(NEWSTATIC);
                }
                else
                {
                    var local = Global.Library.Settings.ServiceEngineers.Find((x) => x.Email == CFPEmailTextBox.Value);
                    local.Email = CFPEmailTextBox.Value;
                    local.Name = CFPNameTextBox.Value;
                }
                Push.AppSettings();
                UpdateStatus("Field Personnel added: " + CFPEmailTextBox.Value);
                BindAndUpdatePersonnel(Global.Library.Settings.ShippingPersons, Global.Library.Settings.ServiceEngineers);
            }
            catch { UpdateStatus("Error adding Field Personnel"); }        
        }

        protected void CCOkBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Customer cust = new Customer()
                {
                    CompanyName = CustCompany.Value,
                    Address = CustAddr.Value,
                    Address2 = CustAddr2.Value,
                    Postal = CustPostal.Value,
                    City = CustCty.Value,
                    Country = CustCountry.Value,
                    Attn = CustName.Value,
                    State = CustState.Value,
                    Phone = CustPhone.Value,
                    EmailAddress = new EmailAddress() { Name = CustName.Value, Email = CustEmail.Value }

                };
                Global.Library.Settings.Customers.Add(cust);
                Push.AppSettings();
                UpdateStatus("Customer added: " + cust.CompanyName);
                BindAndUpdateCustomers(Global.Library.Settings.Customers);
            }
            catch { UpdateStatus("Error Creating Static Email"); }
        }

        protected void ExportXmlBtn_Click(object sender, EventArgs e)
        {
            string xml = "";
            var name = Guid.NewGuid().ToString();
            var path = Server.MapPath( "/Upload/Temp/" + name + ".xml");
            try
            {
               xml =  Global.Library.SerializeToXmlString(Global.Library);
            }
            catch { }
            try
            {
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.ContentType = "text/xml";
                Response.AddHeader("content-disposition", "attachment;filename=Library.xml");
                Response.Write(xml);
                Response.End();

            }
            catch {
                //this.Page.SiteMaster().ShowError("Error Exporting Library Xml");
            }

        }

        protected void CSEOkBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool isedit = Convert.ToBoolean(IsStaticEdit.Value);
                if (!isedit)
                {
                    var local = Global.Library.Settings.StaticEmails.Find((x) => x.Email == CSEEmailTextBox.Value);
                    if (local != null)
                    {
                        UpdateStatus(CSEEmailTextBox.Value + " already exists");
                        return;
                    }
                    var name = CSENameTextBox.Value;
                    var email = CSEEmailTextBox.Value;
                    EmailAddress NEWSTATIC = new EmailAddress() { Name = name, Email = email };
                    Global.Library.Settings.StaticEmails.Add(NEWSTATIC);
                }
                else
                {
                    var local = Global.Library.Settings.StaticEmails.Find((x) => x.Email == CSEEmailTextBox.Value);
                    local.Email = CSEEmailTextBox.Value;
                    local.Name = CSENameTextBox.Value;
                }
                Push.AppSettings();
                UpdateStatus("Static Email added: " + CSEEmailTextBox.Value);
                BindAndUpdatePersonnel();
            }
            catch { UpdateStatus("Error Creating Static Email"); }
        }

        protected void CreateAssetModalOkBtn_Click(object sender, EventArgs e)
        {

        }

        protected void UploadImageBtn_Click(object sender, EventArgs e)
        {

        }


        private void BuildCertsFromDirectory(string dir = "/Account/Certificates/")
        {
            List<CalibrationData> certs = new List<CalibrationData>();
            var collection = Directory.GetFiles(Server.MapPath(dir));
            foreach (var item in collection)
            {
                try
                {
                    CalibrationData cd = new CalibrationData();
                    cd.FilePath = "/Account/Certificates/" + Path.GetFileName(item);
                    cd.CalibrationCompany = Path.GetFileName(item);
                    cd.Description = Path.GetFileName(item);
                    cd.ImagesPath = "/Account/Certificates/";
                    cd.SchedulePeriod = "6";
                    Global.Library.Certificates.Calibrations.Add(cd);
                }
                catch
                { }
            }
            Push.Certificates();

            LoadCertificateView(null);
        }
        protected void CCertOkBtn_Click(object sender, EventArgs e)
        {
        

        }

        protected void SortAssetImages_Click(object sender, EventArgs e)
        {
            foreach (var asset in Pull.Assets())
            {
                var images = asset.Images.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!Directory.Exists(Server.MapPath("/Account/Images/"+asset.AssetNumber)))
                {
                    Directory.CreateDirectory(Server.MapPath("/Account/Images/" + asset.AssetNumber));
                   
                }
                try
                {
                    foreach (var img in images)
                    {
                        if (!File.Exists(Server.MapPath(("/Account/Images/" + asset.AssetNumber + "/" + Path.GetFileName(img)))))
                        {
                            if (File.Exists(Server.MapPath("/Account/Images/" + Path.GetFileName(img) )))
                                File.Copy(Server.MapPath("/Account/Images/" + Path.GetFileName(img) ), Server.MapPath(("/Account/Images/" + asset.AssetNumber + "/" + Path.GetFileName(img))));
                        }
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatus("Error Sorting Images: "+ex.Message);
                    return;
                }
                UpdateStatus("Success Sorting Images: ");
            }

        }
        protected void UpdateStatus(string status)
        {
            FooterStatusLabel.Text = status;
            AppFooterUpdatePanel.Update();
        }

        protected void SingleAssetImageUploadBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (SingleAssetImageFileUpload.PostedFile != null)
                {
                    var asset = Session["CreatorAsset"] as Asset;                   
                    if (!Directory.Exists(Server.MapPath("/Account/Images/" + asset.AssetNumber)))
                    {
                        Directory.CreateDirectory(Server.MapPath("/Account/Images/" + asset.AssetNumber));
                    }
                    var filename = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                    var ext = Path.GetExtension(SingleAssetImageFileUpload.FileName);
                    SingleAssetImageFileUpload.SaveAs(Server.MapPath("/Account/Images/" + asset.AssetNumber + "/" + filename + ext));
                    //AssetImgBox.ImageUrl = "/Account/Images/" + asset.AssetNumber + "/" + filename + ext;
                    asset.Images += filename + ext + ",";
                    Session["CreatorAsset"] = asset;
                    Push.Asset(asset);
                    LoadSingleAssetView(asset);
                    UpdateStatus("Image uploaded");
                    //ImagePlaceHolder.Visible = true;
                }
            }
            catch
            {
                UpdateStatus("Problem uploading image");
            }

        }

        protected void SingleAssetDocumentFileUploadBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (SingleAssetDocumentFileUpload.PostedFile != null)
                {
                    var asset = Session["CreatorAsset"] as Asset;
                    if (!Directory.Exists(Server.MapPath("/Account/Images/" + asset.AssetNumber+"/Documents")))
                    {
                        Directory.CreateDirectory(Server.MapPath("/Account/Images/" + asset.AssetNumber + "/Documents"));
                    }
                    var filename = SingleAssetDocumentFileUpload.FileName;
                    SingleAssetDocumentFileUpload.SaveAs(Server.MapPath("/Account/Images/" + asset.AssetNumber+"/Documents/" + filename ));
                    //AssetImgBox.ImageUrl = "/Account/Images/" + asset.AssetNumber + "/" + filename + ext;
                    asset.Documents.Add("/Account/Images/" + asset.AssetNumber + "/Documents/" + filename);
                    Session["CreatorAsset"] = asset;
                    Push.Asset(asset);
                    LoadSingleAssetView(asset);
                    UpdateStatus("Document uploaded");
                    //ImagePlaceHolder.Visible = true;
                }
            }
            catch
            {
                UpdateStatus("Problem uploading image");
            }
        }

        protected void CustomerViewDocumentsBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (CustomerViewDocumentsFileUpload.PostedFile != null)
                {
                    var customer = Session["CurrentCustomer"] as Customer;
                    CustomerData cd = new CustomerData();
                    if (customer.DataGuid!=null)
                    {
                        if (customer.DataGuid=="")
                        {         
                            Global.Library.Settings.Customers.ForEach((c) => 
                            {
                                if (c.Equals(customer))
                                {
                                    c.DataGuid = cd.Guid;
                                    Push.AppSettings();
                                }
                            });
                            cd.Customer = customer;
                            cd.Date = DateTime.Now;
                        }
                        else
                        {
                            cd = Pull.CustomerData(customer.DataGuid);
                        }
                    }
                    else
                    {
                        Global.Library.Settings.Customers.ForEach((c) =>
                        {
                            if (c.Equals(customer))
                            {
                                c.DataGuid = cd.Guid;
                                Push.AppSettings();
                            }
                        });
                        cd.Customer = customer;
                        cd.Date = DateTime.Now;
                    }
                    if (cd==null)
                    {
                        cd = new CustomerData();
                    }
                    if (!Directory.Exists(Server.MapPath("/Account/CustomerData/" )))
                    {
                        Directory.CreateDirectory(Server.MapPath("/Account/CustomerData/"));
                    }
                    if (!Directory.Exists(Server.MapPath("/Account/CustomerData/" + customer.DataGuid)))
                    {
                        Directory.CreateDirectory(Server.MapPath("/Account/CustomerData/" + customer.DataGuid));
                    }
                    var filename = CustomerViewDocumentsFileUpload.FileName;
                    CustomerViewDocumentsFileUpload.SaveAs(Server.MapPath("/Account/CustomerData/" + customer.DataGuid + "/" + filename));
                    cd.Documents.Add("/Account/CustomerData/" + customer.DataGuid + "/" + filename);
                    Session["CurrentCustomer"] = customer;

                    Push.CustomerData(cd);
                    LoadCustomerView(customer);
                    UpdateStatus("Document uploaded");
                }
            }
            catch
            {
                UpdateStatus("Problem uploading image");
            }

            LoadCustomerView(Session["CurrentCustomer"] as Customer);
        }

        protected void RoleDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void UserDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void CopyUserToRoleBtn_Click(object sender, EventArgs e)
        {
            BindUsersAndRoles();
        }

        protected void ChangeUserToRoleBtn_Click(object sender, EventArgs e)
        {
            var user = UserDropDownList.SelectedItem.Text;
            var role = RoleDropDown.SelectedItem.Text;
           // BindUsersAndRoles();
            var name = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindByName(user);
            
            ChangeUserRole(name.Id,role);
            BindUsersAndRoles();
            AppRightPanelUpdatePanel.Update();
        }
    }
}
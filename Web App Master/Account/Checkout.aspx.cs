using Helpers;
using ShippingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using static Notification.NotificationSystem;

namespace Web_App_Master.Account
{

    public partial class Checkout1 : System.Web.UI.Page
    {
        public void SaveToUserPersistantLog()
        {
            try
            {
                var ud = Session["PersistingUserData"] as Data.UserData;
                string pdf = Session["CombinedPdf"] as string;
                Data.Attachment a = new Data.Attachment();
                a.Email = ud.Email;
                a.Name = ud.Name;
                a.Files.Add(new Data.FileReference(Session["Ordernumber"] as string, pdf));
                ud.Attachments.Add(a);
                ud.Log.Add(new Data.LogEntry("Checked Out Order #" + Session["Ordernumber"] as string, Session["Ordernumber"] as string));
               
                SettingsDBData db = new SettingsDBData();
                db.Appname = ud.Guid;
                db.XmlData = ud.SerializeToXmlString(ud);

                Push.Setting(db);
            }
            catch { }
        }
        public void NotifyCheckoutEmail(List<Asset> assets)
        {
            if(Global.Library.Settings.TESTMODE==false)
            {
                try
                {
                    EmailHelper.SendCheckOutNoticeAsync(assets, Global.Library.Settings.CheckOutMessage);
                    Global.LogEntry(DateTime.Now.ToString() + " User:" + Page.User.Identity.Name + ": " + "Checkout Email Sent ");
                }
                catch { ShowError("Problem Sending Emails"); }
                
               
             
            }
           
        }
       
        public string Customer = null;
        public string Shipper = null;
        public string Engineer = null;
        public string Ordernumber = null;
       
        private void DataBindShipping(string customer, string shipper, string engineer, string ordernumber)
        {
            try
            {
                Customer cust=null;
                //var bb = Global.Library;
                foreach (var c in Global.Library.Settings.Customers)
                {
                    if (customer.Contains(c.CompanyName) && customer.Contains(c.Address))
                    {
                        if (c.CompanyName != "" && c.Address != "")
                            cust = c;
                    }
                }
                if (cust != null)
                {
                    ToCompany.Value = cust.CompanyName;
                    ToAddr.Value = cust.Address;
                    ToAddr2.Value = cust.Address2;
                    ToCty.Value = cust.City;
                    ToState.Value = cust.State;
                    ToPostal.Value = cust.Postal;
                    ToCountry.Value = cust.Country;
                    ToName.Value = cust.Attn;
                    ToEmail.Value = cust.Email;
                    ToPhone.Value = cust.Phone;
                }
                Session["CheckOutCustomer"] = cust;
            }
            catch { }
            var starrag = "Starrag USA Inc.";
            Customer defaultshipper = (from temp in Global.Library.Settings.Customers where temp.CompanyName == starrag select temp).ToList().FirstOrDefault();
            FillDefaultShipper(defaultshipper);
            AddressUpdatePanel.Update();
        }
        private void DataBindShipping(Customer cust, string shipper, string engineer, string ordernumber)
        {
            try
            {
                
                if (cust != null)
                {
                    ToCompany.Value = cust.CompanyName;
                    ToAddr.Value = cust.Address;
                    ToAddr2.Value = cust.Address2;
                    ToCty.Value = cust.City;
                    ToState.Value = cust.State;
                    ToPostal.Value = cust.Postal;
                    ToCountry.Value = cust.Country;
                    ToName.Value = cust.Attn;
                    ToEmail.Value = cust.Email;
                    ToPhone.Value = cust.Phone;
                }
                Session["CheckOutCustomer"] = cust;
            }
            catch { }
            var starrag = "Starrag USA Inc.";
            Customer defaultshipper = (from temp in Global.Library.Settings.Customers where temp.CompanyName == starrag select temp).ToList().FirstOrDefault();
            FillDefaultShipper(defaultshipper);
            AddressUpdatePanel.Update();
        }

        protected bool Finalized = false;
        protected void FinalizeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (ShippingMethodDropDownList.Text == "")
                {
                    ShowError("Please Select A Shipping Option");
                    return;
                }
                var checkoutdata = Session["CheckOutData"] as CheckOutData;
                checkoutdata = UpdateCheckOutData(checkoutdata);
                Session["CheckOutDataTemp"] = checkoutdata.Clone() as CheckOutData;
                if (checkoutdata.CheckOutItems == null) return;

                if (checkoutdata.CheckOutItems.Count > 0)
                {
                    if (ShippingMethodDropDownList.SelectedValue == "00-NoLabel")
                    {
                        Finalized = true;
                        PackingSlipViewBtn.Enabled = true;

                        CreatePackingSlip();

                        //UpdateCost();
                        // UpdateArrival();
                        // UpdateUpsLabel();

                        PackingSlipOnly();

                        //notice
                        try
                        {
                            EmailNotice notice = new EmailNotice();

                            notice.Scheduled = DateTime.Now.AddDays(30);
                            notice.NoticeType = NoticeType.Checkout;
                            notice.NoticeAction = Global.CheckoutAction;
                            foreach (var asset in checkoutdata.CheckOutItems)
                            {
                                notice.Assets.Add(asset.AssetNumber);
                            }
                            notice.NoticeControlNumber = checkoutdata.CheckOutItems[0].OrderNumber;
                            notice.Body = Global.Library.Settings.CheckOutMessage;
                            notice.Subject = "Asset Return Reminder";
                            var engineer = (from d in Global.Library.Settings.ServiceEngineers where d.Name == Session["Engineer"] as string select d).FirstOrDefault();
                            var statics = (from d in Global.Library.Settings.StaticEmails select d).ToList();
                            if (engineer == null)
                            {
                                engineer = new EmailAddress();
                            }
                            notice.Emails.Add(engineer);
                            notice.Emails.AddRange(statics);
                            notice.EmailAddress = engineer;
                            Global.NoticeSystem.Add(notice);
                            Push.NotificationSystem();
                        }
                        catch { ShowError("Problem Adding Timed Notice"); }

                        SaveToUserPersistantLog();



                        FinalizeAssets(Session["Checkout"] as List<Asset>);

                        Session["Checkout"] = new List<Asset>();


                        but_OK.Enabled = false;

                        FinalizeHolder.Visible = false;

                        LeavePlaceHolder.Visible = true;

                        CheckoutView.ActiveViewIndex = 1; // goto report
                    }
                    else
                    {
                        if (checkoutdata.Package.Weight == null) { ShowError("No Package Weight."); return; }
                        if (checkoutdata.From.AddressLine[0] != "" && checkoutdata.To.AddressLine[0] != "" && checkoutdata.Shipper.AddressLine[0] != "" && checkoutdata.Package.Weight != "")
                        {
                            var ud = Session["SessionUserData"] as Data.UserData;
                            ud.Log.Add(new Data.LogEntry("Order#" + checkoutdata.CheckOutItems.FirstOrDefault().OrderNumber, Session["CombinedPdf"] as string));
                            Finalized = true;
                            PackingSlipViewBtn.Enabled = true;

                            CreatePackingSlip();

                            UpdateCost();
                            UpdateArrival();
                            UpdateUpsLabel();

                            try
                            {
                                EmailNotice notice = new EmailNotice();

                                notice.Scheduled = DateTime.Now.AddDays(30);
                                notice.NoticeType = NoticeType.Checkout;
                                notice.NoticeAction = Global.CheckoutAction;
                                foreach (var asset in checkoutdata.CheckOutItems)
                                {
                                    notice.Assets.Add(asset.AssetNumber);
                                }
                                notice.NoticeControlNumber = checkoutdata.CheckOutItems[0].OrderNumber;
                                notice.Body = Global.Library.Settings.CheckOutMessage;
                                notice.Subject = "Asset Return Reminder";
                                var engineer = (from d in Global.Library.Settings.ServiceEngineers where d.Name == Session["Engineer"] as string select d).FirstOrDefault();
                                var statics = (from d in Global.Library.Settings.StaticEmails select d).ToList();
                                if (engineer == null)
                                {
                                    engineer = new EmailAddress();
                                }
                                notice.Emails.Add(engineer);
                                notice.Emails.AddRange(statics);
                                notice.EmailAddress = engineer;
                                Global.NoticeSystem.Add(notice);
                                Push.NotificationSystem();
                            }
                            catch { ShowError("Problem Adding Timed Notice"); }
                            SaveToUserPersistantLog();
                            FinalizeAssets(Session["Checkout"] as List<Asset>);
                            Session["Checkout"] = new List<Asset>();

                            but_OK.Enabled = false;
                            FinalizeHolder.Visible = false;
                            LeavePlaceHolder.Visible = true;


                            CheckoutView.ActiveViewIndex = 1; // goto report

                        }
                        else
                        {
                            ShowError("Please fill in form completely.");
                        }
                    }
                }
                else
                {
                    ShowError("There are no items in Check Out");
                }
                try
                {
                    if (Session["IsPending"] as string == "true")
                    {
                        AssetController.RemoveTransaction(Session["TransactionID"] as string);
                    }
                }
                catch (Exception) { }


                try
                {
                    var existingCust = from c in Global.Library.Settings.Customers where c.CompanyName == ToCompany.Value && c.Address == ToAddr.Value select c;
                    if (existingCust.Count() ==0)
                    {
                        var newcust = GetCustomAddress();
                        Global.Library.Settings.Customers.Add(newcust);
                        Push.AppSettings();
                    }
                }
                catch (Exception exx) {
                    ShowError("Could not add new Customer to List\r\n"+exx.StackTrace);
                }

                //Force all clients to update
                this.HubContext<App_Start.SignalRHubs.ClientHub>().Clients.All.assetCacheChanged();
                ShowError("Checkout Succeeded\r\n");
            }
            catch ( Exception exx) { ShowError("Checkout failed\r\n"+exx.StackTrace); }
        }

        private Customer GetCustomAddress()
        {
            Customer cust = new Helpers.Customer();
            cust.CompanyName = ToCompany.Value;
            cust.Address = ToAddr.Value;
            cust.Address2 = ToAddr2.Value;
            cust.City = ToCty.Value;
            cust.State = ToState.Value;
            cust.Postal = ToPostal.Value;
            cust.Country = ToCountry.Value;
            cust.Attn = ToName.Value;
            cust.EmailAddress = new EmailAddress() { Name = ToCompany.Value, Email = ToEmail.Value };
            cust.Phone = ToPhone.Value;
            return cust;
        }


        private void FinalizeAssets(List<Asset> assets)
        {
            var customer = Session["CheckOutCustomer"] as Customer;
            CustomerData cd = new CustomerData();
            if (customer.DataGuid == null)
            {
                Global.Library.Settings.Customers.ForEach((c) =>
                {
                    if (c.Address.Contains(customer.Address) && c.Postal.Contains(customer.Postal))
                    {
                        try
                        {
                            if (cd == null)
                            {
                                cd = new CustomerData();
                                cd.Guid = c.DataGuid;
                            }
                            c.DataGuid = cd.Guid;
                            c.CurrentAssignedAssets.Clear();
                            assets.ForEach((x) => { c.CurrentAssignedAssets.Add(x.AssetNumber); });
                            Push.AppSettings();
                            Push.CustomerData(cd);
                        }
                        catch
                        {

                        }
                    }
                });
            }
            if (customer.DataGuid != "")
            {
                cd = Pull.CustomerData(customer.DataGuid);
                Global.Library.Settings.Customers.ForEach((c) =>
                {
                    if (c.Address.Contains(customer.Address) && c.Postal.Contains(customer.Postal))
                    {
                        try
                        {
                            if (cd == null)
                            {
                                cd = new CustomerData();
                                cd.Guid = c.DataGuid;
                            }
                            c.DataGuid = cd.Guid;
                            c.CurrentAssignedAssets.Clear();
                            assets.ForEach((x) => { c.CurrentAssignedAssets.Add(x.AssetNumber); });
                            Push.AppSettings();
                            Push.CustomerData(cd);
                        }
                        catch
                        {

                        }
                    }
                });
            }
            else
            {
                customer.DataGuid = cd.Guid;
                Global.Library.Settings.Customers.ForEach((c) =>
                {
                    if (c.Address.Contains(customer.Address) && c.Postal.Contains(customer.Postal))
                    {
                        c.DataGuid = cd.Guid;
                        c.CurrentAssignedAssets.Clear();
                        assets.ForEach((x) => { c.CurrentAssignedAssets.Add(x.AssetNumber); });
                        Push.AppSettings();
                    }
                });
                cd.Customer = customer;
                cd.Date = DateTime.Now;
            }
            var on = Session["Ordernumber"] as string;
            if (on != null)
            {
                if (cd.OrderNumbers == null)
                    cd.OrderNumbers = new List<string>();
                cd.OrderNumbers.Add(on);
            }

            List<Asset> emaillist = new List<Asset>();

            AssetKit assetKit = new AssetKit();

            foreach (var A in assets)
            {
                assetKit.Assets.Add(A.AssetNumber);
                A.ShipTo = ToCompany.Value;
                A.ServiceEngineer = Session["Engineer"] as string;
                try
                {
                    var t = Session["Ordernumber"] as string;
                    if (t!=null)
                    {
                        A.OrderNumber = t;
                    }
                    
                }
                catch {
                    ShowError("Could Not Add Order Number To Asset: " + A.AssetNumber);
                }
                A.DateShipped = DateTime.Now;
                A.PackingSlip = Session["LastPackingSlip"] as string;
                A.PersonShipping = Session["Shipper"] as string;
                A.UpsLabel= Session["ShippingLabelPdf"] as string;
                if (A.UpsLabel==null || A.UpsLabel == "")
                {
                    A.Documents.Add(Session["LastPackingSlip"] as string);
                    cd.Documents.Add(Session["LastPackingSlip"] as string);
                }
                else
                {
                    A.Documents.Add(Session["CombinedPdf"] as string);
                    cd.Documents.Add(Session["CombinedPdf"] as string);
                }
                if (Session["ShippingLabelPdf"] != null)
                {
                    A.Documents.Add(Session["ShippingLabelPdf"] as string);
                    cd.Documents.Add(Session["ShippingLabelPdf"] as string);
                }
                
                A.IsOut = true;
                Asset rem=null;
                foreach (var a in Global.AssetCache)
                {
                    if (a.AssetNumber==A.AssetNumber)
                    {
                        rem = a;
                    }
                }
                if (rem != null)
                {
                    Global.AssetCache.Remove(rem);
                    Global.AssetCache.Add(A);
                }
                emaillist.Add(A);     

                //UPDATE ASSET IN SQL
                //AssetController.UpdateAsset(A.Clone() as Asset);
                Push.Asset(A);
            }
            if (cd.AssetKitHistory == null)
            { cd.AssetKitHistory = new List<AssetKit>();}
            cd.AssetKitHistory = new List<AssetKit>(); cd.AssetKitHistory.Add(assetKit);
            Push.CustomerData(cd);
            var aaa = from aa in Global.Library.Settings.Customers where aa.DataGuid == cd.Guid select aa;
            NotifyCheckoutEmail(emaillist);
            if (aaa.Count() ==0)
            {
                Global.Library.Settings.Customers.ForEach((c) =>
                {
                    if (c.Address.Contains(customer.Address) && c.Postal.Contains(customer.Postal))
                    {
                        c.DataGuid = cd.Guid;
                        c.CurrentAssignedAssets.Clear();
                        assets.ForEach((x) => { c.CurrentAssignedAssets.Add(x.AssetNumber); });
                        Push.AppSettings();
                    }
                });
            }
            Push.Alert((Session["Ordernumber"] as string) + ": Checked Out");
            Session["Customer"] = null;
            Session["Shipper"] = null;
            Session["Engineer"] = null;
            Session["Ordernumber"] = null;
            Session["checkout_success"] = "true";


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
        public PendingTransaction Pending = null;

        private void DataBindPacking()
        {

        }
        private void DataBindShippingLabel()
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["IsPending"] = "false";
            try
            {
                var TransactionID = Request.QueryString["tid"];
                Session["TransactionID"] = TransactionID;
                if (TransactionID != null)
                {
                    Pending = Web_App_Master.Pull.Transaction(TransactionID);
                    Session["Pending"] = Pending;
                    Session["IsPending"] = "true";
                }
            }
            catch (Exception) { }

            if (!Page.User.Identity.IsAuthenticated)
            {
                Response.Redirect("/Account/Login");
            }
            var checkoutdata = Session["CheckOutData"] as CheckOutData;
            if (checkoutdata == null) checkoutdata = new CheckOutData();
            var CheckoutItems = Session["Checkout"] as List<Asset>;
            checkoutdata.CheckOutItems = CheckoutItems;
            Customer = Session["Customer"] as string;
            Shipper = Session["Shipper"] as string;
            Engineer = Session["Engineer"] as string;
            Ordernumber = Session["Ordernumber"] as string;
            if (IsPostBack)
            {

                DataBindShipping(GetCustomAddress(), Shipper, Engineer, Ordernumber);
            }

            if (!IsPostBack)
            {
                Session["ups_remake"] = "false";
                Session["checkout_success"] = "false";
                Session["ups_success"] = "false";
                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(RemakeLabelsBtn);
                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(checkout_ShipTo);
                checkout_ShipTo.DataSource = GetShipToNames();
                checkout_ShipTo.DataBind();

                but_OK.Enabled = true;
                DataBindShipping(Customer, Shipper, Engineer, Ordernumber);

                var preferShip = Session["PreferedShipMethod"] as string;
                if (preferShip == null)
                {
                    ShippingMethodDropDownList.Text= "No Label";
                    //ShippingMethodDropDownList.SelectedIndex = 3;
                    Session["PreferedShipMethod"] = "NoLabel";
                }
                else
                {
                        
                        ShippingMethodDropDownList.Text = preferShip;
                    
                }
                decimal TotalWeight = 0;
                if (CheckoutItems != null)
                {
                    foreach (var asset in CheckoutItems)
                    {
                        TotalWeight += asset.weight;
                    }
                    PkgWeight.Value = TotalWeight.ToString();
                    checkoutdata.Package.Weight = TotalWeight.ToString();
                }
                
            }
            MessagePlaceHolder.Visible = false;



            UpdateCheckOutData(checkoutdata );
        }

        private CheckOutData UpdateCheckOutData(CheckOutData checkoutdata)
        {
            //Code
            checkoutdata.Code = checkoutdata.ParseUpsCodeString(Session["PreferedShipMethod"] as string);
            if (checkoutdata.Package.Weight == null )
            {
                checkoutdata.Package.Weight = "1";
                PkgWeight.Value = "1";
            }
            //pkg
            decimal TotalWeight = 0;
            var CheckoutItems = Session["Checkout"] as List<Asset>;
            if (CheckoutItems != null)
            {              

            }
            //package
            string userinputweight = PkgWeight.Value;
            try
            {
                TotalWeight = Convert.ToDecimal(userinputweight);
                if (TotalWeight==0)
                {
                    TotalWeight = 1;
                    PkgWeight.Value = "1";
                }
            }
            catch { }         
            
            try
            {
                checkoutdata.Package.Weight = TotalWeight.ToString();
                checkoutdata.Package.H = Convert.ToInt32(string.IsNullOrEmpty(PkgHeight.Value) ? "0" : PkgHeight.Value);
                checkoutdata.Package.L = Convert.ToInt32(string.IsNullOrEmpty(PkgLength.Value) ? "0" : PkgHeight.Value);
                checkoutdata.Package.W = Convert.ToInt32(string.IsNullOrEmpty(PkgWidth.Value) ? "0" : PkgHeight.Value);
                checkoutdata.Package.PackType = UPS_PackagingType.CustomerSupplied;
                checkoutdata.Package.reference = PkgReference.Value;
                checkoutdata.Package.reference2 = PkgReference2.Value;
            }
            catch { }


            //From
            checkoutdata.From.AddressLine = new string[] { SprAddr.Value, SprAddr2.Value };
            checkoutdata.From.Name = SprCompany.Value;
            checkoutdata.From.AttentionName = SprName.Value;
            checkoutdata.From.City = SprCty.Value;
            checkoutdata.From.State = SprState.Value;
            checkoutdata.From.Postal = SprPostal.Value;
            checkoutdata.From.Country = SprCountry.Value;
            checkoutdata.From.Phone = "18595345201";

            //To
            checkoutdata.To.AddressLine = new string[] { ToAddr.Value, ToAddr2.Value };
            checkoutdata.To.Name = ToCompany.Value;
            checkoutdata.To.AttentionName = ToName.Value;
            checkoutdata.To.City = ToCty.Value;
            checkoutdata.To.State = ToState.Value;
            checkoutdata.To.Postal = ToPostal.Value;
            checkoutdata.To.Country = ToCountry.Value;
            checkoutdata.To.Phone = "18595345201";

            checkoutdata.Shipper = checkoutdata.From;
            //save
            Session["CheckOutData"] = checkoutdata;
            return checkoutdata;
        }

        protected void ShippingViewBtn_Click(object sender, EventArgs e)
        {
            CheckoutView.ActiveViewIndex = 0;
            QuietLinks();
        }

        protected void PackingSlipViewBtn_Click(object sender, EventArgs e)
        {
            CheckoutView.ActiveViewIndex = 1;
            var filename = Session["CombinedPdf"] as string;
            if (filename != null)
                PF.Src = filename;
            QuietLinks();
        }

        protected void ShippingLabelBtn_Click(object sender, EventArgs e)
        {
          
            CheckoutView.ActiveViewIndex = 2;
            DataBindShippingLabel();
            QuietLinks();
        }

        protected void SaveShipperBtn_Click(object sender, EventArgs e)
        {

        }

        protected void CombineShipAndPack()
        {
            try
            {
                var date = DateTime.Now.ToShortDateString();
                date = date.Replace("/","-");
                var filename =date+"-"+ TrackingNumberLabel.Text + ".pdf";
                Session["CombinedPdf"] = PF.Src = "/Account/CheckOutPdf/" + filename; ;
                var dest = Server.MapPath("/Account/CheckOutPdf/" + filename);
                var pack = Session["LastPackingSlip"] as string;
                var ship = Session["ShippingLabelPdf"] as string;
                if (!Directory.Exists(Server.MapPath("/Account/CheckOutPdf/")))
                {
                    Directory.CreateDirectory(Server.MapPath("/Account/CheckOutPdf/"));
                }

                string[] combine = new string[2] { Server.MapPath(pack), Server.MapPath(ship) };

                var result = Pdf.Merge(combine.ToList(), dest, false);
                if (result)
                {
                    PF.Src = "/Account/CheckOutPdf/" + filename;
                    PdfUpdatePanel.Update();
                }
                if ((Session["ups_remake"] as string) == "true")
                {
                    string redirect = "<script>window.open('" + "/Account/CheckOutPdf/" + filename + "');</script>";
                    // Response.Write(redirect);
                    PdfUpdatePanel.Update();
                }
            }
            catch { }

        }

        protected void PackingSlipOnly()
        {
            try
            {
                var filename = Guid.NewGuid().ToString() + ".pdf";
                Session["CombinedPdf"] = PF.Src = "/Account/PackingLists/" + filename; ;
                var dest = Server.MapPath("/Account/PackingLists/" + filename);
                var pack = Session["LastPackingSlip"] as string;
                if (!Directory.Exists(Server.MapPath("/Account/PackingLists/")))
                {
                    Directory.CreateDirectory(Server.MapPath("/Account/PackingLists/"));
                }
                File.Copy(Server.MapPath(pack),dest);
               // string[] combine = new string[2] { Server.MapPath(pack), Server.MapPath(ship) };

               // var result = Pdf.Merge(combine.ToList(), dest);
                //if (result)
               // {
                    PF.Src = "/Account/PackingLists/" + filename;

               // }
            }
            catch { }

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
        protected bool ValidateUpsAcct(UPSaccount acct)
        {
            if (acct.P==null || acct.A == null || acct.I == null || acct.N == null )
            { return false; }
            if (acct.P == "" || acct.A == "" || acct.I == "" || acct.N == "")
            { return false; }
            return true;
        }
        private string GetInnerUpsAcctError(UPSaccount acct)
        {
            string ret = "";
            if (acct.P == null)
            { ret+= "<No Password>"; }
            if (acct.A == null)
            { ret += "<No Account Number>"; }
            if (acct.I == null)
            { ret += "<No UserID>"; }
            if (acct.N == null)
            { ret += "<No Shipper Number>"; }
            if (acct.P == "")
            { ret += "<No Password>"; }
            if (acct.A == "")
            { ret += "<No Account Number>"; }
            if (acct.I == "")
            { ret += "<No UserID>"; }
            if (acct.N == "")
            { ret += "<No Shipper Number>"; }
            return ret;
        }
        protected void UpdateCost()
        {
            try
            {
                var checkoutdata = Session["CheckOutData"] as CheckOutData;
                checkoutdata = UpdateCheckOutData(checkoutdata);

                if (checkoutdata != null)
                {
                    if (checkoutdata.Package.Weight == null) { ShowError("No Package Weight."); return; }
                    if (checkoutdata.From.AddressLine[0] != "" && checkoutdata.To.AddressLine[0] != "" && checkoutdata.Shipper.AddressLine[0] != "" && checkoutdata.Package.Weight != "")
                    {
                        //if (Global.Library.Settings.TESTMODE)
                        //{
                        //    checkoutdata.Pickup = DateTime.Now.AddHours(-8).AddDays(1);
                        //}
                        //Check UPS Account
                        if (!ValidateUpsAcct(Global.Library.Settings.UpsAccount))
                        { ShowError("Please check your Shipper Account Settings in Admin Panel: "+GetInnerUpsAcctError(Global.Library.Settings.UpsAccount)); return; }
                        UPS ups = new UPS(Global.Library.Settings.UpsAccount, true);
                        ups.OnException += Ups_OnException;
                        ups.OnRateReturn += Ups_OnRateReturn;
                        ups.SoapExceptionListener += Ups_SoapExceptionListener;
                        var result = ups.GetRateAsync(checkoutdata.From, checkoutdata.To, checkoutdata.From, checkoutdata.Code, checkoutdata.Package);
                        if (result == null)
                        {
                            //error
                        }
                    }
                    else
                    {
                        ShowError("Please fill in form completely.");
                    }
                }
            }
            catch
            {
              
            }
        }
        
        protected void CreatePackingSlip()
        {
            var cd = Session["CheckOutData"] as CheckOutData;
            //get filename
            string n = DateTime.Now.ToString().Replace("/", "-");
            n = n.Replace(":", "-");
            string info = n + " " + (Session["Ordernumber"] as string);
            info = info.Replace("\\", "").Replace(":", "").Replace("*", "").Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "").Replace(" ", "_");
            string path = "/Account/PackingLists/" + info + ".pdf";
            PackingSlipLink.Text = path;
            Session["LastPackingSlip"] = path;
            path = Server.MapPath(path);
            
            //To
            string to = cd.To.Name + Environment.NewLine;
            foreach(var line in cd.To.AddressLine)
            {
                to += line + Environment.NewLine;
            }
            to += cd.To.City + Environment.NewLine;
            to += cd.To.State + ", "+ cd.To.Postal + Environment.NewLine;
            to += cd.To.Country + Environment.NewLine;
            //From
            string from = cd.From.Name + Environment.NewLine;
            foreach (var line in cd.From.AddressLine)
            {
                from += line + Environment.NewLine;
            }
            from += cd.From.City + Environment.NewLine;
            from += cd.From.State + ", " + cd.From.Postal + Environment.NewLine;
            from += cd.From.Country + Environment.NewLine;
            //Define Keys

            var headerKeys = new Dictionary<string, string>() {
            {"ShipDate", cd.Pickup.ToShortDateString()},
            {"Tracking", TrackingNumberLabel.Text},
            {"Signature", Session["Shipper"] as string},
            {"Attn", cd.To.AttentionName},
            {"Po", ""},
            {"To", to},
            {"From", from},
            {"Weight","Weight:"+ cd.Package.Weight},
            {"Ordernumber","Order #:"+ Session["Ordernumber"] as string} };

            var keys = new Dictionary<string, string>();
            headerKeys.ToList().ForEach(x => keys.Add(x.Key, x.Value));
            int i = 1;
            foreach (var asset in cd.CheckOutItems)
            {
                if (i>13)
                {
                    //create templated pdf packingslip with default stamping action
                    Pdf.PackingSlip p = new Pdf.PackingSlip(new Templates.StarragPackingSlipTemplate());
                    p.CreateAndFill(path, keys, p.StampAction);
                    //fill new page with new assets
                    keys = new Dictionary<string, string>();
                    headerKeys.ToList().ForEach(x => keys.Add(x.Key, x.Value));
                    i = 1;
                    path = path.Replace(".pdf", "")+i.ToString()+".pdf";
                }
                keys.Add("Desc" + i.ToString(), asset.AssetName);
                keys.Add("PartNo" + i.ToString(), asset.AssetNumber);
                keys.Add("Qty" + i.ToString(), "1");
                ++i;
            }

            //create templated pdf packingslip with default stamping action
            Pdf.PackingSlip ps = new Pdf.PackingSlip(new Templates.StarragPackingSlipTemplate());
            var result = ps.CreateAndFill(path, keys, ps.StampAction);
            var bbbb = false;

        }
        protected void UpdateArrival()
        {
            try
            {
                var checkoutdata = Session["CheckOutData"] as CheckOutData;
                checkoutdata = UpdateCheckOutData(checkoutdata);

                if (checkoutdata != null)
                {
                    if (checkoutdata.Package.Weight == null) { ShowError("No Package Weight."); return; }
                    if (checkoutdata.From.AddressLine[0]!=""&& checkoutdata.To.AddressLine[0] != "" && checkoutdata.Shipper.AddressLine[0] != "" && checkoutdata.Package.Weight != "")
                    {
                        if (!ValidateUpsAcct(Global.Library.Settings.UpsAccount))
                        { ShowError("Please check your Shipper Account Settings in Admin Panel:" + GetInnerUpsAcctError(Global.Library.Settings.UpsAccount)); return; }
                        UPS ups = new UPS(Global.Library.Settings.UpsAccount, true);
                        ups.OnException += Ups_OnException;
                        ups.OnRateReturn += Ups_OnRateReturn;
                        ups.OnTransitReturn += Ups_OnTransitReturn;
                        var result = ups.GetTransitAsync(checkoutdata.From, checkoutdata.To, checkoutdata.Pickup, checkoutdata.Package.Weight);
                        if (result == null)
                        {
                            //error
                        }
                    }
                    else
                    {
                        ShowError("Please fill in form completely.");
                    }
                }
            }
            catch
            {
                
            }
        }
        protected void UpdateUpsLabel()
        {
            try
            {
                var checkoutdata = Session["CheckOutData"] as CheckOutData;
                checkoutdata = UpdateCheckOutData(checkoutdata);

                if (checkoutdata != null)
                {
                    if (checkoutdata.Package.Weight == null) { ShowError("No Package Weight."); return; }
                    if (checkoutdata.From.AddressLine.Count() != 0 && checkoutdata.To.AddressLine.Count() != 0 && checkoutdata.Shipper.AddressLine.Count() != 0 && checkoutdata.Package.Weight != "")
                    {
                        if (!ValidateUpsAcct(Global.Library.Settings.UpsAccount))
                        { ShowError("Please check your Shipper Account Settings in Admin Panel:" + GetInnerUpsAcctError(Global.Library.Settings.UpsAccount)); return; }
                        UPS ups = new UPS(Global.Library.Settings.UpsAccount,true );
                        ups.OnException += Ups_OnException;
                        ups.OnRateReturn += Ups_OnRateReturn;
                        ups.OnShipReturn += Ups_OnShipReturn;
                        var result = ups.GetShipAsync(checkoutdata.From, checkoutdata.To, checkoutdata.From, checkoutdata.Code, checkoutdata.Package, checkoutdata.Package.reference, checkoutdata.Package.reference2);
                        if (result == null)
                        {
                            ShowError("Problem Creating Label");
                        }
                    }
                    else
                    {
                        ShowError("Please fill in form completely.");
                    }
                }
            }
            catch
            {
                ShowError("Problem Getting Shipping Label");
            }
        }
        protected void QuietLinks()
        {
            Quiet(ShipLink);
            //Quiet(LabelLink);
            Quiet(ReportLink);
        }
        protected void Shake(HtmlGenericControl control)
        {
            var a = control.Attributes["class"];
            a += " mif-ani-flash";
            control.Attributes["class"] = a ;
        }
        protected void Quiet(HtmlGenericControl control)
        {
            var a = control.Attributes["class"];
            a = a.Replace("mif-ani-flash", "").Replace("mif-ani-slow", "");
            control.Attributes["class"] = a;
        }

        protected void RefreshCost_Click(object sender, EventArgs e)
        {
            UpdateCost();
        }

        private void Ups_OnTransitReturn(object sender, UPS.ReturnTransitEvent e)
        {
            
            
            Transit.RESPONSE r = e.Response;

            var code = ShippingMethodDropDownList.Text;

            string sum = "************************\r\n";
            try
            {
                code = (code.Split('-'))[1];
            }
            catch { }
            foreach (var unit in r.Summary)
            {
                var cd = unit.Service.Description.Replace("UPS ", "").Replace(" ","");
                if (cd == code)
                {
                    try
                    {
                        System.Globalization.CultureInfo provider = new System.Globalization.CultureInfo("en-US");
                        var d = DateTime.ParseExact(unit.EstimatedArrival.Arrival.Date, "yyyyMMdd", provider);
                        ArrivalDate.Value = d.ToShortDateString();
                        string dn = "PM";
                        int a = Convert.ToInt32(unit.EstimatedArrival.Arrival.Time);

                        if (a <= 119999)
                            dn = "AM";
                        else
                            a = a - 120000;
                        var t = a.ToString();
                        t = t.Insert(2, ":");
                        t = t.Insert(5, ":");
                        ArrivalTime.Value = t + " " + dn;
                    }
                    catch { }
                    return;
                }
                sum += unit.Service.Description +
                    "\r\nBusiness Days: "
                    + unit.EstimatedArrival.BusinessDaysInTransit +
                    "\r\nArrival Date: " + unit.EstimatedArrival.Arrival.Date + " @ " + unit.EstimatedArrival.Arrival.Time + "\r\n************************\r\n";
            }
           
        }
        private void Ups_OnShipReturn(object sender, UPS.ReturnShipEvent e)
        {
            try
            {
                var img = e.Response.Label;
                using (MemoryStream ms = new MemoryStream())
                {
                    //img.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    Session["LastShippingLabel"] = "/Account/Labels/" + e.Response.TrackingNumber + ".gif";
                    var filename = Server.MapPath("/Account/Labels/" + e.Response.TrackingNumber + ".gif");
                    PkgTracking.Value = TrackingNumberLabel.Text = e.Response.TrackingNumber;
                    Session["LastTrackingNumber"] = e.Response.TrackingNumber;

                    var shortname = "/Account/Labels/" + e.Response.TrackingNumber + ".gif";
                    try
                    {
                        img.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    catch { }
                    try
                    {
                        Session["ShippingLabelPdf"] = "/Account/Labels/" + e.Response.TrackingNumber + ".pdf";
                        var dest = Server.MapPath("/Account/Labels/" + e.Response.TrackingNumber + ".pdf");
                        Pdf.ImageToPdf(img, dest);
                        img.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    catch { }
                    UpsLabelImgBox.Attributes["height"] = "100%";
                    try
                    {

                        imgcontainer.Style["height"] = "100%";
                    }
                    catch { }
                    UpsLabelImgBox.Alt = DateTime.Now.ToString();
                    UpsLabelImgBox.Src = shortname;
                    Session["ShippingLabelFileName"] = shortname;

                    //create pdf
                    if ((Session["ups_remake"] as string) == "true")
                    {
                        Finalized = true;
                    }
                    if (Finalized)
                    {
                        CombineShipAndPack();
                        Session["ups_success"] = "true";
                    }
                }
            }
            catch (Exception ex) {
                Session["ups_success"] = "false";
                ShowError("Could not Create Label\r\n" + ex.StackTrace);
            }

        }
        private void Ups_OnRateReturn(object sender, UPS.ReturnRateEvent e)
        {
            try
            {
                EstimatedCost.Value = e.Response.Negotiated;
                if (e.Response.Negotiated==null)
                {
                    EstimatedCost.Value = e.Response.Price;
                }
            }
            catch { }
        }
        private void Ups_OnException(object sender, UPS.ExceptionOccured e)
        {
            try
            {
                Session["ups_success"] = "false";
                var exx = e.Exception.InnerException as System.Web.Services.Protocols.SoapException;
                var msg = exx.Detail.ChildNodes[0].ChildNodes[0].ChildNodes[1].InnerText;
                ShowError("A problem has occured, please check that all forms are filled in correctly and try again<br>"+msg);
                
            }
            catch { }
        }
        private void Ups_SoapExceptionListener(object sender, UPS.SoapExceptionOccured e)
        {
            Session["ups_success"] = "false";
            ShowError(e.Exception.Detail.InnerText + e.Exception.Detail.InnerXml);
        }


        protected void RefreshArrival_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateArrival();
            }
            catch {

                ShowError("Problem Updating Arrival");
            }
        }

        protected void ShippingMethodDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            Session["PreferedShipMethod"] = ShippingMethodDropDownList.SelectedValue;
            if (ShippingMethodDropDownList.SelectedValue != "00-NoLabel")
            {
                UpdateCost();
            UpdateArrival();

            }
            
        }

        protected void GetUpsLabel_Click(object sender, EventArgs e)
        {
            UpdateUpsLabel();
        }
        public List<string> GetShipToNames()
        {
            var names = (from ship in Global.Library.Settings.Customers orderby ship.CompanyName select ship.CompanyName + "-" + ship.Address + "-" + ship.Postal).ToList();
            return names;
        }
        protected void checkout_ShipTo_TextChanged(object sender, EventArgs e)
        {
            Customer cust = (from temp in Global.Library.Settings.Customers where checkout_ShipTo.Text.Contains(temp.CompanyName) && checkout_ShipTo.Text.Contains(temp.Address) select temp).ToList().FirstOrDefault();
            
            if (cust != null)
            {
                ToCompany.Value = cust.CompanyName;
                ToAddr.Value = cust.Address;
                ToAddr2.Value = cust.Address2;
                ToCty.Value = cust.City;
                ToState.Value = cust.State;
                ToPostal.Value = cust.Postal;
                ToCountry.Value = cust.Country;
                ToName.Value = cust.Attn;
                ToEmail.Value = cust.Email;
                ToPhone.Value = cust.Phone;
            }
            Session["CheckOutCustomer"] = cust;
        }

        protected void RemakeLabelsBtn_Click(object sender, EventArgs e)
        {
            if (PF.Src=="/Account/Pdfs/blank.pdf")
            {
                var lastPdf = Session["CombinedPdf"] as string;
                if (lastPdf==null)
                {
                    Session["ups_remake"] = "true";
                    var lastCheckout = Session["CheckOutDataTemp"] as CheckOutData;
                    Session["CheckOutData"] = lastCheckout;
                    if (lastCheckout==null)
                    {
                        //no checkout data at all
                        ShowError("No CheckoutData");
                        return;
                    }
                    var t = Session["ups_success"] as string;
                    var success = Convert.ToBoolean(t);
                    CreatePackingSlip();
                    UpdateUpsLabel();
                    UpdateAssetsWithNewDocument();
                    return;

                }
                if (lastPdf=="")
                {
                    Session["ups_remake"] = "true";
                    var lastCheckout = Session["CheckOutDataTemp"] as CheckOutData;
                    Session["CheckOutData"] = lastCheckout;
                    if (lastCheckout == null)
                    {
                        //no checkout data at all
                        ShowError("No CheckoutData");
                        return;
                    }
                    var t = Session["ups_success"] as string;
                    var success = Convert.ToBoolean(t);
                    CreatePackingSlip();
                    UpdateUpsLabel();
                    UpdateAssetsWithNewDocument();
                    return;
                }
            }
            else
            {
                var lastPdf = Session["CombinedPdf"] as string;
                if (lastPdf == null)
                {
                    Session["ups_remake"] = "true";
                    var lastCheckout = Session["CheckOutDataTemp"] as CheckOutData;
                    Session["CheckOutData"] = lastCheckout;
                    if (lastCheckout == null)
                    {
                        //no checkout data at all
                        ShowError("No CheckoutData");
                        return;
                    }
                    var t = Session["ups_success"] as string;
                    var success = Convert.ToBoolean(t);
                    CreatePackingSlip();
                    UpdateUpsLabel();
                    UpdateAssetsWithNewDocument();
                    return;

                }
                if (lastPdf == "")
                {
                    Session["ups_remake"] = "true";
                    var lastCheckout = Session["CheckOutDataTemp"] as CheckOutData;
                    if (lastCheckout == null)
                    {
                        //no checkout data at all
                        ShowError("No CheckoutData");
                        return;
                    }
                    Session["CheckOutData"] = lastCheckout;
                    var t = Session["ups_success"] as string;
                    var success = Convert.ToBoolean(t);
                    CreatePackingSlip();
                    UpdateUpsLabel();
                    UpdateAssetsWithNewDocument();
                    return;
                }
                try
                {
                    var result = new WebClient().DownloadData(Server.MapPath(lastPdf));
                    if (result.Length > 0)
                    {                        
                        PF.Src = lastPdf;
                        PdfUpdatePanel.Update();
                        return;
                    }
                    else
                    {
                        Session["ups_remake"] = "true";
                        var lastCheckout = Session["CheckOutDataTemp"] as CheckOutData;
                        if (lastCheckout == null)
                        {
                            //no checkout data at all
                            ShowError("No CheckoutData");
                            return;
                        }
                        Session["CheckOutData"] = lastCheckout;
                        CreatePackingSlip();
                        UpdateUpsLabel();
                        UpdateAssetsWithNewDocument();
                        return;
                    }
                }
                catch (Exception exx) {
                    if (exx.Message.ToUpper().Contains("COULD NOT FIND FILE"))
                    {
                        Session["ups_remake"] = "true";
                        var lastCheckout = Session["CheckOutDataTemp"] as CheckOutData;
                        if (lastCheckout == null)
                        {
                            //no checkout data at all
                            ShowError("No CheckoutData");
                            return;
                        }
                        Session["CheckOutData"] = lastCheckout;
                        CreatePackingSlip();
                        UpdateUpsLabel();
                        UpdateAssetsWithNewDocument();
                        return;
                    }
                }
              
                //string redirect = "<script>window.open('"+ lastPdf + "');</script>";
                //Response.Write(redirect);

            }

            //Send Data to Asset in lstcheckout




        }
      
        protected void ShowLabelsBtn_Click(object sender, EventArgs e)
        {
            var lastPdf = Session["CombinedPdf"] as string;
            if (lastPdf != null)
            {
                string redirect = "<script>window.open('" + lastPdf + "');</script>";
                Response.Write(redirect);
            }
            else
            {
                Session["ups_remake"] = "true";
                var lastCheckout = Session["CheckOutDataTemp"] as CheckOutData;
                Session["CheckOutData"] = lastCheckout;
                if (lastCheckout == null)
                {
                    //no checkout data at all
                    ShowError("No CheckoutData");
                    return;
                }
                CreatePackingSlip();
                UpdateUpsLabel();
                UpdateAssetsWithNewDocument();
            }

        }

        protected void UpdateAssetsWithNewDocument()
        {
            var lastCheckout = Session["CheckOutDataTemp"] as CheckOutData;
            foreach (var asset  in lastCheckout.CheckOutItems)
            {
                if (!asset.Documents.Contains(Session["CombinedPdf"] as string))
                {
                    asset.IsOut = true;
                    asset.Documents.Add(Session["CombinedPdf"] as string);
                    Push.Asset(asset);
                }
            }
        }
    }
}
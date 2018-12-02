using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Helpers;
using System.Xml;
using System.Linq;
using Web_App_Master.Account;
using System.IO;
using static Web_App_Master.App_Start.SignalRHubs;

namespace Web_App_Master
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(MasterSuperBtn);

            //start polling
           
            if (Context.IsAdmin())
            {
                
            }
            if (Context.IsCustomer())
            {
                if (!IsPostBack)
                {
                    var script = @"$(document).ready(function (){ RunScanner();});";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ScannersScript", script, true);
                }
            }
            if (!IsPostBack)
            {
                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(UploadAssetImg);

                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(ASuperBtn);
                try
                {
                    //var ctrl = RoleLoginViewer.FindControl("ViewChangerBtn");
                    //ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(ctrl);
                    
                }
                catch
                {

                }
               
                BindMenuItems();
                
            }
            this.PreRender += SiteMaster_PreRender;
            if (Page.User.Identity.IsAuthenticated)
            {
                //var ctrl = AssetViewLoggedInUserView.FindControl("av_IsDamaged");
                //ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(ctrl);
                //var ctrl2 = AssetViewLoggedInUserView.FindControl("av_OnHold");
                //ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(ctrl2);
                //var ctrl3 = AssetViewLoggedInUserView.FindControl("av_CalibratedTool");
                //ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(ctrl3);
            }
            //AssetHistoryRepeater.HeaderTemplate = Page.LoadTemplate("/Account/Templates/av_history_header_template.ascx");
            //AssetHistoryRepeater.ItemTemplate = Page.LoadTemplate("/Account/Templates/av_history_template.ascx");
           // AssetHistoryRepeater.FooterTemplate = Page.LoadTemplate("/Account/Templates/av_history_footer_template.ascx");
            var ud = Session["PersistingUserData"] as Data.UserData;
            if (ud != null)
            {
                try
                {
                    if (ud.IsAutoChecked)
                    {
                        var cb = RoleLoginViewer.FindControl("BarcodeCheckBox") as CheckBox;
                        
                        cb.Checked = true;
                    }
                    else
                    {
                        var cb = RoleLoginViewer.FindControl("BarcodeCheckBox") as CheckBox;
                        cb.Checked = false;
                    }
                }
                catch { }
            }

        }

        public event EventHandler<UpdateRequestEvent> OnPanelUpdate;
        public event EventHandler<UpdateRequestEvent> OnAssetViewUpdate;
        protected virtual void UpdateSubscribers(UpdateRequestEvent e)
        {
            try
            {
                EventHandler<UpdateRequestEvent> handler = OnPanelUpdate;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch (Exception ex) { UpdateSubscribers(new UpdateRequestEvent(ex)); }
        }
        protected virtual void UpdateAssetView(UpdateRequestEvent e)
        {
            try
            {
                EventHandler<UpdateRequestEvent> handler = OnAssetViewUpdate;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch (Exception ex) { UpdateSubscribers(new UpdateRequestEvent(ex)); }
        }

        public void ShowError(string message,string title="")
        {
            ErrorBox.Visible = true;
            ErrorLabel.Text = title;
            ErrorMessage.Text = message;
            ErrorModalUpdatePanel.Update();
            
        }
        public void AddMenuNotice(MenuAlert notice)
        {
            var list = Session["Notifications"] as List<MenuAlert>;
            
            list.Add(notice);
            
        }
        public void UpdateAllPanels()
        {
            try
            {
                BindAssetToAssetView();
                AssetHistoryUpdatePanel.Update();
            }
            catch { }
            try
            {
                //BindCalibration();
                //CalibrationUpdatePanel.Update();
            }
            catch { }
            try
            {
                //if (Context.IsAdmin())
                  //  BindCheckin();                
            }
            catch { }
            try {
              //  if (Context.IsAdmin())
                //    BindCheckout();
                
            }
            catch { }
            try
            {
                BindUserNotice();
                var UserNoticeUpdatePanel = RoleLoginViewer.FindControl("UserNoticeUpdatePanel") as UpdatePanel;
                UserNoticeUpdatePanel.Update();
            }
            catch { }
            try
            {
                UpdateSubscribers(new UpdateRequestEvent(this));
            }
            catch { }
           
        }

        private void BindCalibration()
        {
         
        }

        public List<Asset> LocalAssets { get; set; }
        public bool Loggedin { get; set; }
        public XmlDocument LocalXmlLibrary { get; set; }
        public Asset _Asset { get; set; }
        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;
        public Repeater _NoticeRepeater;
        public UpdatePanel _NoticeUpdate;
        public void BindCheckout()
        {
            
            var checkout = Session["CheckOut"] as List<Asset>;
            if (checkout == null) checkout = new List<Asset>();

            //var CheckoutRepeater = RoleLoginViewer.FindRoleControl("CheckoutRepeater", "Admins", "CheckoutUpdatePanel") as Repeater;
            //CheckoutRepeater.DataSource = checkout;
            //CheckoutRepeater.DataBind();
            //var CheckoutUpdatePanel = RoleLoginViewer.FindRoleControl("CheckoutUpdatePanel", "Admins") as UpdatePanel;
            //CheckoutUpdatePanel.Update();
            //var r = CheckoutUpdatePanel.RecursiveFindControl("CheckoutRepeater") as Repeater;
            //r.DataSource = checkout;
            //r.DataBind();
            //CheckoutUpdatePanel.Update();
        }
        public void BindCheckin()
        {
            var checkin = Session["CheckIn"] as List<Asset>;
            if (checkin == null) checkin = new List<Asset>();
            

            var CheckinLiteral= RoleLoginViewer.FindRoleControl("CheckinLiteral", "Admins") as Literal;
            CheckinLiteral.Text = "test";

            //  CheckInRepeater.DataSource = checkin;
            //  CheckInRepeater.DataBind();
            //var CheckInUpdatePanel = RoleLoginViewer.FindRoleControl("CheckInUpdatePanel", "Admins") as UpdatePanel;
            //CheckInUpdatePanel.Update();
        }
        public void BindUserNotice()
        {
            var UserNoticerRepeater = RoleLoginViewer.FindControl("UserNoticerRepeater") as Repeater;
            var loginview = RoleLoginViewer.Controls[0] as LoginView;
            UserNoticerRepeater = loginview.FindControl("UserNoticerRepeater") as Repeater;
            int oldCount = 0;
            try { oldCount = UserNoticerRepeater.Items.Count; }
            catch { }
            var ud = Session["SessionUserData"] as Data.UserData;
            if (ud == null) return;
            int count = ud.Log.Count;
            UserNoticerRepeater.DataSource = ud.Log;
            UserNoticerRepeater.DataBind();
            if (oldCount!=count)
            {
               // NotificationIcon.Shake();
            }
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            
            try
            {
                //BindCheckInOutBoxes();
            }
            catch { }
            _Asset = new Asset();
            Loggedin = false;
            // The code below helps to protect against XSRF attacks
            var requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                // Use the Anti-XSRF token from the cookie
                _antiXsrfTokenValue = requestCookie.Value;
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            else
            {
                // Generate a new Anti-XSRF token and save to the cookie
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = _antiXsrfTokenValue
                };
                if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                {
                    responseCookie.Secure = false;
                }
                Response.Cookies.Set(responseCookie);
            }

            Page.PreLoad += master_Page_PreLoad;
        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set Anti-XSRF token
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
            }
            else
            {
                // Validate the Anti-XSRF token
                if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                    || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
                {
                    throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                }
            }
        }
        protected List<Asset> binddummy()
        {
            return new List<Asset>() { new Asset() { AssetName="None", AssetNumber="0000" } };
        }
        public bool IsAutoChecked = false;
        public void BindMenuItems()
        {
            if (Context.IsAdmin())
            {
               


                


            }
        }


        private void SiteMaster_PreRender(object sender, EventArgs e)
        {
            try
            {
              // UpdateAllPanels();
              if (!IsPostBack)
                {
                   // BindMenuItems();
                }
            }
            catch { }
            
        }

        protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
        {
            Context.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }
        [System.Web.Services.WebMethod]
        public static string search()
        {
            return "worked master";
        }


        public System.Collections.IEnumerable NoticeRepeater_GetData()
        {
            return null;
        }

        protected void NotifyRefreshBtn_Click(object sender, EventArgs e)
        {
        }

        protected void button33_Click(object sender, EventArgs e)
        {
            BindCheckout();
            BindCheckin();
        }

        protected void UpdateAllCarts_Click(object sender, EventArgs e)
        {
            UpdateAllPanels();
        }

        protected void AssetSaveBtn_Click(object sender, EventArgs e)
        {

            var asset = Session["CurrentAsset"] as Asset;
            bool newassetnumber = false;
            var originalassetnumber = asset.AssetNumber;
            try
            {
                //HOLY SHIT YOU NEED TO CHECK INPUT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                if (av_AssetNumber.Value.Length >= Global.Library.Settings.AssetNumberLength)
                    if ((from a in AssetController.GetAllAssets() where a.AssetNumber == av_AssetNumber.Value select a).ToList().Count == 0)
                    { //Unique Number
                        asset.AssetNumber = av_AssetNumber.Value;
                        newassetnumber = true;
                        if (!Directory.Exists(Server.MapPath("/Account/Images/"+asset.AssetNumber)))
                        {
                            Directory.CreateDirectory(Server.MapPath("/Account/Images/" + asset.AssetNumber));
                        }
                        try
                        {
                            foreach(var file in Directory.GetFiles(Server.MapPath("/Account/Images/" + originalassetnumber)))
                            {
                                File.Move(file, Server.MapPath("/Account/Images/" + asset.AssetNumber+"/"+Path.GetFileName(file)));
                            }
                            Directory.Delete(Server.MapPath("/Account/Images/" + originalassetnumber));
                        }
                        catch (Exception exx) {
                            ShowError(exx.StackTrace);
                        }
                    }
                    else
                    {
                        av_AssetNumber.Value = asset.AssetNumber;
                    }
                asset.AssetName = av_AssetName.Value;              

                DateTime tmp;

                var result = DateTime.TryParse(((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("av_DateRecieved")).Value, out tmp);
                if (result)
                    asset.DateRecieved = tmp;
                result = DateTime.TryParse(((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("av_DateShipped")).Value, out tmp);
                if (result)
                    asset.DateShipped = tmp;

                asset.Description = av_Description.Value;
                asset.OrderNumber = ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("av_ServiceOrder")).Value;
                asset.ServiceEngineer = ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("av_ServiceEngineer")).Value;
                asset.ShipTo = ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("av_ShipTo")).Value;
                asset.PersonShipping = ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("av_PersonShipping")).Value;
                try
                {
                    asset.weight = Convert.ToDecimal(((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("av_Weight")).Value);
                }
                catch { }
                asset.IsDamaged=((System.Web.UI.WebControls.CheckBox)AssetViewLoggedInUserView.FindControl("av_IsDamaged")).Checked;
                asset.OnHold=((System.Web.UI.WebControls.CheckBox)AssetViewLoggedInUserView.FindControl("av_OnHold")).Checked ;
                asset.IsCalibrated=((System.Web.UI.HtmlControls.HtmlInputCheckBox)AssetViewLoggedInUserView.FindControl("av_CalibratedTool")).Checked ;
                if (asset.IsCalibrated)
                {
                    
                    //check if cert exists
                    //check if ields are filled in
                    //save new cert to database, or assign cert

                }
            }
            catch
            {
            }
            var test = false;
            Push.Asset(asset);
            //Set Asset Cache
            if (newassetnumber)
            {
                AssetController.DeleteAsset(originalassetnumber);
                var rem = Global.AssetCache.FindAssetByNumber(originalassetnumber);
                Global.AssetCache.Remove(rem);
            }
            AssetViewHeaderLabel.InnerHtml += " - Modified";
            BindAssetToAssetView();
            Push.Alert("Asset Saved");
            OnAssetViewUpdate?.Invoke(this, new UpdateRequestEvent( Global.AssetCache ));


        }

        public void UpdateAssetView()
        {
           OnAssetViewUpdate?.Invoke(this, new UpdateRequestEvent(Global.AssetCache));
        }

        protected void UploadAssetImg_Click(object sender, EventArgs e)
        {
            try
            {
                var filename = AssetImageUploader.FileName;              
                var asset = Session["CurrentAsset"] as Asset;
               if (!Directory.Exists(Server.MapPath("/Account/Images/"+asset.AssetNumber)))
                {
                    Directory.CreateDirectory(Server.MapPath("/Account/Images/"+asset.AssetNumber));
                }
                    
                    var name = Server.MapPath("/Account/Images/" + asset.AssetNumber+"/" + Server.HtmlEncode(filename));
                    asset.Images += Server.HtmlEncode(filename) + ",";
                    AssetImageUploader.PostedFile.SaveAs(name);
                Push.Asset(asset);
                Response.Redirect("/Account/AssetView");
            }
            catch { }
        }

        protected void AssetHistoryRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        protected void HistoryBinderBtn_Click(object sender, EventArgs e)
        {
            BindAssetToAssetView();
        }
        public void UpdateAllAssetView(Asset asset)
        {
            try
            {
                AssetHistoryRepeater.DataSource = asset.History.History;
                AssetHistoryRepeater.DataBind();
                AssetHistoryUpdatePanel.Update();
                AssetModalUpdatePanel.Update();
            }
            catch { }
        }
        public void BindAssetToAssetView(Asset asset = null)
        {
            if (asset == null)
            {
                asset = Session["CurrentAsset"] as Asset;
            }
            else
            {

            }
            AssetImageHolder.ImageUrl = asset.FirstImage;
            AssetImageCountLiteral.Text = "1/" +((asset.Images.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)).Length.ToString());
            AssetImageUpdatePanel.Update();
            if (asset == null) { binddummy(); return; }
            if (asset.History.History.Count==0){ binddummy(); }
            try
            {
                AssetHistoryRepeater.DataSource = asset.History.History;
                AssetHistoryRepeater.DataBind();
                AssetHistoryUpdatePanel.Update();               
            }
            catch { }
            try
            {
               
                CalibrationRepeater.DataSource = asset.CalibrationHistory.Calibrations;
                CalibrationRepeater.DataBind();
                CalibrationUpdatePanel.Update();
            }
            catch { }
            try
            {
                AssetDocumentTree.Nodes.Clear();
                TreeNode rootnode = new TreeNode("Documents");
                TreeNode packnode = new TreeNode("Packing Slips");
                TreeNode Labelnode = new TreeNode("Shipping Labels");
                TreeNode ReceivingNode = new TreeNode("Receiving Slips");
                TreeNode CertificateNode = new TreeNode("Certificates");

                foreach (var doc in asset.Documents)
                {
                    TreeNode n1 = new TreeNode(Path.GetFileName(doc));
                    n1.NavigateUrl = "javascript:doSomething('" + doc + "');";
                    if (doc.Contains("CheckOutPdf"))
                    {
                        packnode.ChildNodes.Add(n1);
                    }else
                    if (doc.Contains("Labels"))
                    {
                        Labelnode.ChildNodes.Add(n1);
                    }
                    else
                    if (doc.Contains("Certificates"))
                    {
                        CertificateNode.ChildNodes.Add(n1);
                    }
                    else
                    if (doc.Contains("Receiving"))
                    {
                        ReceivingNode.ChildNodes.Add(n1);
                    }
                    else
                    if (doc.Contains("PackingLists"))
                    {
                        packnode.ChildNodes.Add(n1);
                    }
                    else
                    {
                        rootnode.ChildNodes.Add(n1);
                    }
                }

                rootnode.ChildNodes.Add(packnode);
                rootnode.ChildNodes.Add(Labelnode);
                rootnode.ChildNodes.Add(ReceivingNode);
                rootnode.ChildNodes.Add(CertificateNode);
                AssetDocumentTree.Nodes.Add(rootnode);
                AssetDocumentTreeUpdatePanel.Update();
                AssetDocumentsViewer.Update();
                AssetModalUpdatePanel.Update();


                //AssetReceivingReportFrame.Src = asset.ReturnReport;
                //AssetShippingReportFrame.Src = asset.UpsLabel;
                //AssetPackingReportFrame.Src = asset.PackingSlip;
            }
            catch { }
            try
            {
                av_AssetName.Value = asset.AssetName;
                av_AssetNumber.Value = asset.AssetNumber;
                av_Description.Value = asset.Description;
                ((System.Web.UI.WebControls.CheckBox)AssetViewLoggedInUserView.FindControl("av_IsDamaged")).Checked = asset.IsDamaged;
                ((System.Web.UI.WebControls.CheckBox)AssetViewLoggedInUserView.FindControl("av_OnHold")).Checked = asset.OnHold;
                ((System.Web.UI.HtmlControls.HtmlInputCheckBox)AssetViewLoggedInUserView.FindControl("av_CalibratedTool")).Checked = asset.IsCalibrated;
                if (asset.IsCalibrated)
                {
                   var ctrl = (System.Web.UI.HtmlControls.HtmlGenericControl)AssetViewLoggedInUserView.FindControl("ExpandedAssetCalibrationInfo");
                    ctrl.Attributes["style"] = "display: normal;";
                    //find assigned certificate
                    var test = Global.Library.Certificates.Calibrations;
                    var cert = from c in Global.Library.Certificates.Calibrations where c.AssetNumber == asset.AssetNumber select c;
                    if (cert.Count()>0)
                    {
                        var cb = cert.First();
                        ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("MasterCertFile")).Value =cb.FileName ;
                        ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("MasterTotalCerts")).Value = "1/"+cert.Count().ToString() ;
                        ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("MasterCertCompany")).Value = cb.CalibrationCompany;
                        ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("MasterCertPeriod")).Value = cb.SchedulePeriod;
                        ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("MasterCertDaysLeft")).Value = cb.DaysLeft;
                        ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("MasterCertGuid")).Value = cb.Guid;
                    }
                    else
                    {

                        ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("MasterCertFile")).Value = "";
                        ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("MasterCertCompany")).Value = "";
                        ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("MasterCertPeriod")).Value = "";
                        ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("MasterCertDaysLeft")).Value = "";
                        ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("MasterCertGuid")).Value = "";
                    }

                }
                else
                {
                    var ctrl = (System.Web.UI.HtmlControls.HtmlGenericControl)AssetViewLoggedInUserView.FindControl("ExpandedAssetCalibrationInfo");
                    ctrl.Attributes["style"] = "display: none;";
                }
                ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("av_DateRecieved")).Value = asset.DateRecievedString;
                ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("av_DateShipped")).Value = asset.DateShippedString;
                ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("av_ServiceOrder")).Value = asset.OrderNumber;
                ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("av_ServiceEngineer")).Value = asset.ServiceEngineer;
                ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("av_ShipTo")).Value = asset.ShipTo;
                ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("av_PersonShipping")).Value = asset.PersonShipping;
                ((System.Web.UI.HtmlControls.HtmlInputText)AssetViewLoggedInUserView.FindControl("av_Weight")).Value = asset.weight.ToString();
                //((System.Web.UI.HtmlControls.HtmlInputCheckBox)AssetViewLoggedInUserView.FindControl("CalibratedToolCheckBox")).Checked = true;

                ((System.Web.UI.WebControls.Button)AssetViewLoggedInUserView.FindControl("MasterCCertOkBtn")).Click += MasterCCertOkBtn_Click;
                //ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(((System.Web.UI.WebControls.Button)AssetViewLoggedInUserView.FindControl("MasterCCertOkBtn")));
                

            }
            catch
            {
            }
            AssetModalUpdatePanel.Update();
        }

        protected void clearCheckIn_Click(object sender, EventArgs e)
        {
            Session["CheckIn"] = new List<Asset>();
        }

        protected void clearCheckout_Click(object sender, EventArgs e)
        {
            Session["CheckOut"] = new List<Asset>();
        }

        protected void UploadAssetCertificateBtn_Click(object sender, EventArgs e)
        {
            if (MasterCertUpload.HasFile)
            {
                var asset = Session["CurrentAsset"] as Asset;
                

                if (MasterCertUpload.PostedFile == null)
                {
                    ShowError("No File Selected");
                }
                try
                {
                    MasterCertUpload.PostedFile.SaveAs(MapPath("/Account/Certificates/" + MasterCertUpload.FileName));

                    //create new
                    CalibrationData cd = new CalibrationData();
                    cd.SchedulePeriod = CalPeriod.Value;
                    cd.AssetNumber = asset.AssetNumber;
                    cd.CalibrationCompany = CalCompany.Value;
                    cd.FilePath = MapPath("/Account/Certificates/" + MasterCertUpload.FileName);
                    Push.ReminderNotice(Global.Library.Settings.StaticEmails.ToArray(), Convert.ToInt32(CalPeriod.Value), Notification.NotificationSystem.NoticeType.Calibration, cd.Guid);
                    Global.Library.Certificates.Calibrations.Add(cd);
                    Push.Certificates();
                    asset.IsCalibrated = true;
                    asset.Documents.Add("/Account/Certificates/" + MasterCertUpload.FileName);
                    Push.Asset(asset);
                }
                catch
                {
                    ShowError("Error Saving Certificate"); return;
                }
               
            }
            else
            {
                ShowError("No File Uplaoded");
            }
           
        }

        protected void SaveCalOptionsBtn_Click(object sender, EventArgs e)
        {
            if(CalPeriod.Value=="" || CalCompany.Value=="")
            {
                ShowError("Please fill in Company and Period data...");
            }
            var asset = Session["Asset"] as Asset;
            asset.CalibrationCompany = CalCompany.Value.Sanitize();
            DateTime tmp = DateTime.Now;
          
            asset.CalibrationPeriod = CalPeriod.Value;
            Push.Asset(asset);
            
        }

        protected void CalibrationBinderBtn_Click(object sender, EventArgs e)
        {
            BindCalibration();
        }

      

        protected void DeleteCalBtn_Command(object sender, CommandEventArgs e)
        {
            var asset = Session["Asset"] as Asset;
            CalibrationData rem = new CalibrationData();
            foreach (var cal in asset.CalibrationHistory.Calibrations)
            {
                if (cal.Guid == e.CommandName )
                {
                    rem = cal;
                }
            }
            try
            {
                asset.CalibrationHistory.Calibrations.Remove(rem);
            }
            catch { }
            Push.Asset(asset);
            BindCalibration();
        }

        protected void CheckInRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        protected void CheckoutRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var asset = e.Item.DataItem as Helpers.Asset;
            var lit = e.Item.Controls[1] as Literal;
            lit.Text = asset.AssetNumber + " - " + asset.AssetName;
        }

        protected void tester_Click(object sender, EventArgs e)
        {
            PendingTransaction p = new PendingTransaction();
            p.Customer = Global.Library.Settings.Customers[1];
            p.Comment = "nopo";
            var isin = (from aa in Global.AssetCache where aa.IsOut = false select aa.AssetNumber).ToList();
            p.Assets = isin;
            Push.Transaction(p);
        }

        protected void ViewChangeBtn_Click(object sender, EventArgs e)
        {
            this.HubContext<ClientHub>().Clients.All.assetCacheChanged();

        }
        //public int CurrentView { get { return MainContentMultiView.ActiveViewIndex; } }

        protected void AssetTabBtn_Click(object sender, EventArgs e)
        {
            AssetModalTabsMultiView.SetActiveView(AssetTabView);
            if (HistoryItemArguments.Text!="")
            {
                var asset = Session["CurrentAsset"] as Asset;
                var ret = AssetController.GetAssetByDateShipped(asset, HistoryItemArguments.Text);
                HistoryItemArguments.Text = "";
                BindAssetToAssetView(ret);
            }
            else
            {
                BindAssetToAssetView();
            }
            
        }

        protected void ReportTabBtn_Click(object sender, EventArgs e)
        {
            AssetModalTabsMultiView.SetActiveView(ReportTabView);
          
            BindAssetToAssetView();

        }

        protected void CalibrationTabBtn_Click(object sender, EventArgs e)
        {
            AssetModalTabsMultiView.SetActiveView(CalibrationTabView);
            BindAssetToAssetView();
        }

        protected void HistoryTabBtn_Click(object sender, EventArgs e)
        {
            AssetModalTabsMultiView.SetActiveView(HistoryTabView);
            BindAssetToAssetView();
        }

        protected void AssetImagePrevBtn_Click(object sender, EventArgs e)
        {
            var asset = Session["Asset"] as Asset;
            var path = "/Account/Images/" + asset.AssetNumber + "/";
            var imglist = asset.Images.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int idx = 0;
            foreach (var img in imglist)
            {
                if (Path.GetFileName(AssetImageHolder.ImageUrl).Contains(Path.GetFileName(img)))
                {
                    break;
                }
                idx++;
            }
            AssetImageCountLiteral.Text = (idx+1).ToString() + "/" + imglist.Length.ToString();
            if (AssetImageHolder.ImageUrl == "")
            {
                AssetImageHolder.ImageUrl = path + Path.GetFileName(imglist[0]);
                return;
            }
            if (idx == -1)
            {
                AssetImageHolder.ImageUrl = path + Path.GetFileName(imglist[0]);

            }
            else
            {
                if (idx > imglist.Length)
                {
                    AssetImageHolder.ImageUrl = path + Path.GetFileName(imglist[0]);
                }
                else
                {

                    if (idx == imglist.Length)
                    {
                        AssetImageHolder.ImageUrl = path + Path.GetFileName(imglist[idx]);

                    }
                    else
                    {
                        if (idx - 1 <0)
                        { return; }
                        AssetImageHolder.ImageUrl = path + Path.GetFileName(imglist[idx - 1]);
                        AssetImageCountLiteral.Text = (idx).ToString() + "/" + imglist.Length.ToString();
                    }
                }
            }
            AssetImageUpdatePanel.Update();
        }

        protected void AssetImageNextBtn_Click(object sender, EventArgs e)
        {

            var asset = Session["Asset"] as Asset;
            var path = "/Account/Images/" + asset.AssetNumber+"/";
           var imglist = asset.Images.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
            int idx = 0;
            foreach (var img in imglist)
            {
                if (Path.GetFileName(AssetImageHolder.ImageUrl).Contains(Path.GetFileName(img)))
                {
                    break;
                }
                idx++;
            }
            AssetImageCountLiteral.Text = (idx + 1).ToString() + "/" + imglist.Length.ToString();
            if (AssetImageHolder.ImageUrl=="")
            {
                AssetImageHolder.ImageUrl = path + Path.GetFileName( imglist[0]);
                return;
            }
            if (idx == -1)
            {
                AssetImageHolder.ImageUrl = path + Path.GetFileName(imglist[0]);

            }
            else
            {
                if (idx > imglist.Length)
                {
                    AssetImageHolder.ImageUrl = path + Path.GetFileName(imglist[0]);
                }
                else
                {

                    if (idx == imglist.Length)
                    {
                        AssetImageHolder.ImageUrl = path + Path.GetFileName(imglist[idx]);

                    }
                    else
                    {
                        if (idx +1 > imglist.Length-1)
                        { return; }
                        AssetImageHolder.ImageUrl = path + Path.GetFileName(imglist[idx + 1]);
                        AssetImageCountLiteral.Text = (idx + 2).ToString() + "/" + imglist.Length.ToString();
                    }
                }
            }
            AssetImageUpdatePanel.Update();
        }

        protected void av_CalibratedTool_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void av_OnHold_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void av_IsDamaged_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void ASuperBtn_Click(object sender, EventArgs e)
        {
            var arg = ASuperBtnArg.Text;
            CurrentAssetDocumentSrc.Src = arg;
            AssetCurrentDocumentLabel.Text = arg;
            AssetDocumentIframeUpdatePanel.Update();
            AssetDocumentsViewer.Update();
        }

        protected void MasterSuperBtn_Click(object sender, EventArgs e)
        {
            var cmd = AppCommand.Text;
            var arg1 = AppArgument.Text;
            var arg2 = AppArgument2.Text;
            this.Page.RegisterAsyncTask(
                new PageAsyncTask(
                    async ()=>
                    { await Global.RefreshAssetCacheAsync(); }));

            //Check to see if asset view is present in contentpanel, if so, fir off page delegate 
            UpdateAssetView(new UpdateRequestEvent(Global.AssetCache));
        }

        protected void MasterCCertOkBtn_Click(object sender, EventArgs e)
        {

        }
    }
    public class UpdateRequestEvent : EventArgs
    {
        public UpdateRequestEvent(object obj = null)
        {
            if (obj != null)
            {
                Data = obj;
            }
        }
        public object Data { get; set; }
    }
    public class ExceptionEvent : EventArgs
    {
        public ExceptionEvent(Exception obj = null)
        {
            if (obj != null)
            {
                Data = obj;
            }
        }
        public Exception Data { get; set; }
    }


}
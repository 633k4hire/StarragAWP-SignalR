using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
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
using static Notification.NotificationSystem;

namespace Web_App_Master
{
    public partial class SiteMaster : MasterPage
    {
        public event EventHandler<UpdateRequestEvent> OnPanelUpdate;
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

        public void ShowError(string message,string title="")
        {
            ErrorLabel.Text = title;
            ErrorMessage.Text = message;
            var script = @"$(document).ready(function (){ HideLoader();ToggleError();});";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "testScript", script, true);
        }

        public void UpdateAllPanels()
        {
            try
            {
                BindHistory();
                AssetHistoryUpdatePanel.Update();
            }
            catch { }
            try
            {
                BindCalibration();
                CalibrationUpdatePanel.Update();
            }
            catch { }
            try
            {
                BindCheckin();
                CheckInUpdatePanel.Update();
            }
            catch { }
            try {
                BindCheckout();
                CheckoutUpdatePanel.Update();
            }
            catch { }
            try
            {
                BindUserNotice();
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
            try
            {
                var asset = Session["CurrentAsset"] as Asset;

                CalibrationRepeater.DataSource = asset.CalibrationHistory.Calibrations;
                CalibrationRepeater.DataBind();
            }
            catch { }
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
            CheckoutRepeater.DataSource = checkout;
            CheckoutRepeater.DataBind();
        }
        public void BindCheckin()
        {
            var checkin = Session["CheckIn"] as List<Asset>;
            if (checkin == null) checkin = new List<Asset>();
            CheckInRepeater.DataSource = checkin;
            CheckInRepeater.DataBind();
        }
        public void BindUserNotice()
        {
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
                    responseCookie.Secure = true;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AssetHistoryRepeater.DataSource = binddummy();
                AssetHistoryRepeater.DataBind();
            }
            this.PreRender += SiteMaster_PreRender;
            AssetHistoryRepeater.HeaderTemplate = Page.LoadTemplate("/Account/Templates/av_history_header_template.ascx");
            AssetHistoryRepeater.ItemTemplate = Page.LoadTemplate("/Account/Templates/av_history_template.ascx");
            AssetHistoryRepeater.FooterTemplate = Page.LoadTemplate("/Account/Templates/av_history_footer_template.ascx");
            var ud = Session["PersistingUserData"] as Data.UserData;
            if (ud != null)
            {
                try
                {
                    if (ud.IsAutoChecked)
                    {
                        var cb = flexer.FindControl("BarcodeCheckBox") as CheckBox;
                        cb.Checked = true;
                    }
                    else
                    {
                        var cb = flexer.FindControl("BarcodeCheckBox") as CheckBox;
                        cb.Checked = false;
                    }
                }
                catch { }
            }

        }

        private void SiteMaster_PreRender(object sender, EventArgs e)
        {
            try
            {
                if (Page.User.Identity.IsAuthenticated)
                {
                    NotificationsHolder.Visible = true;
                }
            }
            catch { }
            UpdateAllPanels();
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
            //HOLY SHIT YOU NEED TO CHECK INPUT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if (av_AssetNumber.Value.Length<=Global.Library.Settings.AssetNumberLength)
                if ((from a in AssetController.GetAllAssets() where a.AssetNumber == av_AssetNumber.Value select a).ToList().Count == 0)
                { //Unique Number
                    asset.AssetNumber=av_AssetNumber.Value;
                }
            asset.AssetName=av_AssetName.Value;

            DateTime tmp;
            var result = DateTime.TryParse(av_DateRecieved.Value, out tmp);
            if (result)
                asset.DateRecieved = tmp;
            result = DateTime.TryParse(av_DateShipped.Value, out tmp);
            if (result)
                asset.DateShipped = tmp;

            asset.Description = av_Description.Value;
            asset.OrderNumber = av_ServiceOrder.Value;
            asset.ServiceEngineer = av_ServiceEngineer.Value;
            asset.ShipTo = av_ShipTo.Value;
            asset.PersonShipping = av_PersonShipping.Value;
            try
            {
                asset.weight = Convert.ToDecimal( av_Weight.Value);
            }
            catch { }

            AssetController.UpdateAsset(asset);
        }

        protected void UploadAssetImg_Click(object sender, EventArgs e)
        {
            try
            {
                var filename = AssetImageUploader.FileName;              
                var asset = Session["CurrentAsset"] as Asset;
               
                    
                    var name = Server.MapPath("/Account/Images/" + Server.HtmlEncode(filename));
                    asset.Images += Server.HtmlEncode(filename) + ",";
                    AssetImageUploader.PostedFile.SaveAs(name);
                
                    Save.Asset(asset);
               
               
            }
            catch { }
        }

        protected void AssetHistoryRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        protected void HistoryBinderBtn_Click(object sender, EventArgs e)
        {
            BindHistory();
        }
        public void BindHistory()
        {
            
            var asset = Session["CurrentAsset"] as Asset;
            if (asset == null) { binddummy(); return; }
            if (asset.History.History.Count==0){ binddummy(); return; }
            try
            {
                AssetHistoryRepeater.DataSource = asset.History.History;
                AssetHistoryRepeater.DataBind();
                AssetHistoryUpdatePanel.Update();
            }
            catch { }
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
            if (CalPeriod.Value == "" || CalCompany.Value == "")
            {
                ShowError("Please fill in Company and Period data...");
            }

            var asset = Session["Asset"] as Asset;
            var ext = Path.GetExtension(CertUpload.FileName);
            if (ext == "")
                ext = ".pdf";
            var file = System.Guid.NewGuid().ToString();
            var dest = Server.MapPath("/Account/Certs/"+ asset.AssetNumber+file+ext);
            using (FileStream fs = new FileStream(dest, FileMode.Create))
            {
                CertUpload.FileContent.CopyTo(fs);
            }
            
            try
            {
                CalibrationData cd = new CalibrationData();
                cd.AssetNumber = asset.AssetNumber;
                cd.CalibrationCompany= CalCompany.Value.Sanitize();
                try {
                   cd.SchedulePeriod=  CalPeriod.Value;
                } catch { }
                cd.ImagePath = "/Account/Certs/" + asset.AssetNumber + file + ext;
               
                asset.CalibrationHistory.Calibrations.Add(cd);                
                Save.Asset(asset);

                //ADD NOTIFICATION
                try
                {
                    EmailNotice notice = new EmailNotice();

                    notice.Scheduled = DateTime.Now.AddDays(30);
                    notice.NoticeType = NoticeType.Calibration;
                    notice.NoticeAction = Global.CalibrationAction;                   
                    notice.NoticeControlNumber = asset.AssetNumber;                   
                    notice.Body = Global.Library.Settings.CalibrationMessage;
                    var statics = (from d in Global.Library.Settings.StaticEmails select d).ToList();
                    EmailAddress person = new EmailAddress();
                    if (statics.Count!=0)
                    {
                        person = statics.FirstOrDefault();
                    }
                    notice.Emails.AddRange(statics);
                    notice.EmailAddress = person;
                    notice.Scheduled = DateTime.Now.AddMonths(Convert.ToInt32(CalPeriod.Value)).AddDays(-14);
                    Global.NoticeSystem.Add(notice);
                    Save.NotificationSystem();
                }
                catch { }

                //register startup script to hide cal uploader
                BindCalibration();
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "HideCalUploader()", true);
            }
            catch
            {

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
            Save.Asset(asset);
            
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
            Save.Asset(asset);
            BindCalibration();
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

}
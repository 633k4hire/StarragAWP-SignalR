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

namespace Web_App_Master.Account.Admin
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        private void UpdateUsersAndRoles()
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


        }
        private void UpdateAssetAdmin()
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
            try
            {
                if (Global.NoticeSystem.Notices != null)
                {
                   
                    Notice30DayRepeater.DataSource = Global.NoticeSystem.Notices;
                    Notice30DayRepeater.DataBind();
                }
            }
            catch
            {
                PopNotify("Error", "Could Not Bind Notices");
            }           
            try
            {
                if (Global.Library.Settings.ShippingPersons != null)
                {
                    var list = (from n in Global.Library.Settings.ShippingPersons orderby n.Name select n).ToList();

                    EmailBindinglist emailList = new EmailBindinglist(list);
                    ShippingPersonRepeater.DataSource = emailList;
                    ShippingPersonRepeater.DataBind();
                }
            }
            catch
            {
                PopNotify("Error", "Could Not Bind Shipping Personnel");
            }
            try
            {
                if (Global.Library.Settings.ServiceEngineers != null)
                {
                    var list = (from n in Global.Library.Settings.ServiceEngineers orderby n.Name select n).ToList();
                    EmailBindinglist emailList = new EmailBindinglist(list);
                    EngineerRepeater.DataSource = emailList;
                    EngineerRepeater.DataBind();
                }
            }
            catch
            {
                PopNotify("Error", "Could Not Bind Engineers");
            }
            BindCustomers();
            BindStaticEmails();
        }
        private void UpdateAssetManager()
        {
            var assets = Pull.Assets().OrderBy(w => w.AssetNumber).ToList();
            AssetRepeater.DataSource = assets;
            AssetRepeater.DataBind();

        }
        protected void BindCustomers()
        {
            try
            {
                if (Global.Library.Settings.Customers != null)
                {
                    var list = (from n in Global.Library.Settings.Customers orderby n.CompanyName select n).ToList();
                    CustomerBindinglist custlist = new CustomerBindinglist(list);
                    CustomerRepeater.DataSource = custlist;
                    CustomerRepeater.DataBind();
                }
            }
            catch
            {
                PopNotify("Error", "Could Not Bind Customers");
            }
        }
        protected void ShowError(string error)
        {
            try
            {
                MessagePlaceHolder.Visible = true;
                ErrorMsg.Text = error;
            }
            catch { }
        }

        protected void Page_Load(object sender, EventArgs e)
        {   if (!IsPostBack)
                {
                UpdateUsersAndRoles();
                UpdateAssetAdmin();
                UpdateAssetManager();
            }
            TestModeLabel.Text = Global.Library.Settings.TESTMODE.ToString();
        }
        
        private List<ApplicationUser> GetUsersForRole(string role)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            foreach(var user in manager.Users)
            {
                foreach(var r in user.Roles)
                {
                    if (r.RoleId==role)
                    {
                        users.Add(user);
                    }
                    if (r.UserId==user.Id)
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
                    if (r.Id==roleid)
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
               
                UpdateAssetAdmin();
            }
            catch {
                ShowError("Could Not Create Role.");
               
            }
           
        }

        protected void DeleteUser_Command(object sender, CommandEventArgs e)
        {
            var a = e.CommandArgument as string;
            var b = e.CommandName as string;
            RemoveUser(b);
            UpdateUsersAndRoles();
        }
        protected bool ChangeUserRole(string userid, string role)
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            try
            {
                if (!manager.IsInRole(userid, role))
                {
                    manager.AddToRole(userid, role);
                    UpdateUsersAndRoles();
                }
                return true;
            }
            catch {
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
            foreach(var role in roleStore.Roles)
            {
                names.Add(role.Name);
            }
            return names;
        }

        protected void RolesAndUsersRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        protected void DeleteFromRole_Command(object sender, CommandEventArgs e)
        {
            var a = e.CommandArgument as string;
            var b = e.CommandName as string;
            RemoveUserFromRole(b, a);
            UpdateUsersAndRoles();
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
            UpdateUsersAndRoles();
        }

        protected void UsersAndRolesBtn_Click(object sender, EventArgs e)
        {
            AdminMultiView.ActiveViewIndex = 0;
            UpdateUsersAndRoles();
        }

        protected void AssetAdminBtn_Click(object sender, EventArgs e)
        {
            AdminMultiView.ActiveViewIndex = 1;
            UpdateAssetAdmin();
        }

        protected void CustomerManager_Click(object sender, EventArgs e)
        {
            AdminMultiView.ActiveViewIndex = 3;
            BindCustomers();
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
                    if (elemList.Count==0)
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

                UpdateAssetAdmin();
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
            var name = Server.MapPath("/App_Data/notice"+DateTime.Now.ToShortDateString().Replace(" ","").Replace("/", "-") + ".xml");
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
                catch { ShowError("Problem Adding Timed Notice"); }
            }
            Push.NotificationSystem();
            UpdateAssetAdmin();
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

        protected void PopNotify(string caption, string content)
        {
            var script = @"
            <script type='text/javascript'> 
            function Completed() {
                        $.Notify({
                            caption:'"+caption + @"',
                            content: '" + content + @"'
                        });
            };
            Completed();
            </script>
            ";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", script);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "Completed();", true);
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
                        catch { //PopNotify("Error", "Error Pushing Library To SQL");
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

        protected void PullSQL_Click(object sender, EventArgs e)
        {
            try
            {
                SQL_Request req = new SQL_Request().OpenConnection();

                //request all assets
                req.GetAssets(true);
                if (req.Tag != null)
                {
                    //pull assets
                    Global.AssetCache = req.Tag as List<Asset>;
                    //pull settings
                    //pull notifications
                    //pull calibrations
                    //pull history
                }
            }
            catch
            {
                //PopNotify("Error", "Error Pulling Library From SQL");
            }
            //PopNotify("Complete", "Library Pulled From SQL");
            UpdateAssetAdmin();
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
                        catch { //PopNotify("Error", "Error Deleting SQL Library");
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

        protected void SaveAllChangesBtn_Click(object sender, EventArgs e)
        {
            Push.Library();
            Push.AppSettings();
            UpdateUsersAndRoles();
            UpdateAssetAdmin();
        }

        protected void DeleteNotice30DayBtn_Command(object sender, CommandEventArgs e)
        {
            try
            {
                var rem = (from n in Global.NoticeSystem.Notices where n.Guid == e.CommandArgument as string select n).FirstOrDefault();
                if (rem != null)
                {
                    var idx = Global.NoticeSystem.Notices.IndexOf(rem);
                    Global.NoticeSystem.Notices.RemoveAt(idx);
                   
                }
                UpdateAssetAdmin();
            }
            catch {

                try
                {
                    var rem = (from n in Global.NoticeSystem.Notices where n.Guid == e.CommandArgument as string select n).FirstOrDefault();
                    if (rem != null)
                    {
                        var idx = Global.NoticeSystem.Notices.IndexOf(rem);
                        Global.NoticeSystem.Notices.RemoveAt(idx);
                       
                    }
                    UpdateAssetAdmin();
                }
                catch
                {

                    return;
                }
            }
            Push.NotificationSystem();
        }

        protected void SendNotice30DayBtn_Command(object sender, CommandEventArgs e)
        {
            var rem = (from n in Global.NoticeSystem.Notices where n.Guid == e.CommandArgument as string select n).FirstOrDefault();
            //send then remove notice
            if (rem != null)
            {
                EmailHelper.SendNotificationSystemNotice(rem);
                
                     Global.NoticeSystem.Notices.Remove(rem);
               
               
            }
            UpdateAssetAdmin();
        }

        protected void DeleteNotice15DayBtn_Command(object sender, CommandEventArgs e)
        {
            UpdateAssetAdmin();
        }

        protected void SendNotice15DayBtn_Command(object sender, CommandEventArgs e)
        {
            UpdateAssetAdmin();
        }

        protected void DeleteShippingPersonBtn_Command(object sender, CommandEventArgs e)
        {
            try
            {
                var email = e.CommandName;
                EmailAddress em = null;
                foreach (var n in Global.Library.Settings.ShippingPersons)
                {
                    if (n.Email == email)
                    { em = n; }
                }
                Global.Library.Settings.ShippingPersons.Remove(em);
                Push.AppSettings();
                UpdateAssetAdmin();
            }
            catch { Page.SiteMaster().ShowError("Problem romoving Shipping Personnel"); }
        }

        protected void DeleteEngineer_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "DeleteEngineer")
            {
                try
                {
                    var name = e.CommandArgument as string;
                    EmailAddress em = null;
                    foreach (var n in Global.Library.Settings.ServiceEngineers)
                    {
                        if (n.Name == name)
                        { em = n; }
                    }
                    Global.Library.Settings.ServiceEngineers.Remove(em);
                    Push.AppSettings();
                    UpdateAssetAdmin();
                }
                catch { Page.SiteMaster().ShowError("Problem romoving Shipping Personnel"); }

            }
            UpdateAssetAdmin();
        }

        protected void CustomerRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteCustomer")
            {
                try
                {
                    var split = (e.CommandArgument as string).Split('|');
                    var name = split[0];
                    var addy = split[1];
                    Customer em = null;
                    foreach (var n in Global.Library.Settings.Customers)
                    {
                        if (n.CompanyName == name && n.Address == addy)
                        { em = n; }
                    }
                    Global.Library.Settings.Customers.Remove(em);
                    Push.AppSettings();
                    UpdateAssetAdmin();
                }
                catch { Page.SiteMaster().ShowError("Problem romoving Shipping Personnel"); }

            }
            UpdateAssetAdmin();
        }

        protected void SendSettingsSQL_Click(object sender, EventArgs e)
        {           
            AssetController.PushSettings(Global.Library.Settings);
            //PopNotify("Success", "Settings Pushed To SQL");
            UpdateAssetAdmin();
        }

        protected void PullSettings_Click(object sender, EventArgs e)
        {
            Global.Library.Settings = AssetController.GetSettings();
            //PopNotify("Success", "Settings Pulled From SQL");
            UpdateAssetAdmin();
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

        protected void AssetPeopleManager_Click(object sender, EventArgs e)
        {

            AdminMultiView.ActiveViewIndex = 2;
            UpdateAssetAdmin();
        }

        protected void NotificationManagerBtn_Click(object sender, EventArgs e)
        {
            AdminMultiView.ActiveViewIndex = 4;
            UpdateAssetAdmin();
        }

        protected void SaveCheckOutMsgBtn_Click(object sender, EventArgs e)
        {
            var a = checkoutmsgbox.Value;
            Global.Library.Settings.CheckOutMessage = a;
            Push.AppSettings();
        }

        protected void SaveCheckInMsgBtn_Click(object sender, EventArgs e)
        {
            Global.Library.Settings.CheckInMessage = checkinmsgbox.Value;
            Push.AppSettings();
        }

        protected void SaveNoticMsgBtn_Click(object sender, EventArgs e)
        {
            Global.Library.Settings.NotificationMessage = notificationmsgbox.Value;
            Push.AppSettings();
        }

        protected void SaveShipperMsgBtn_Click(object sender, EventArgs e)
        {
            Global.Library.Settings.ShipperNotification = shipmsgbox.Value;
            Push.AppSettings();
        }
        private string AppendDir(string dir, string file)
        {
            return dir + file;
        }
        protected void ExportLibrary_Click(object sender, EventArgs e)
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
                                var name =  file;
                                archive.CreateEntryFromFile(file,"Images/"+ Path.GetFileName(name));
                            }
                            foreach (var file in labels)
                            {
                                var name = file;
                                archive.CreateEntryFromFile(file,"Labels/" +  Path.GetFileName(name));
                            }
                            foreach (var file in packing)
                            {
                                var name = file;
                                archive.CreateEntryFromFile(file,"PackingSlips/" +  Path.GetFileName(name));
                            }
                            foreach (var file in PdfFiles )
                            {
                                var name =  file;
                                archive.CreateEntryFromFile(file,"PdfFiles/" + Path.GetFileName(name));
                            }
                            foreach (var file in Receiving)
                            {
                                var name = file;
                                archive.CreateEntryFromFile(file, "Receiving/" + Path.GetFileName(name));
                            }
                            foreach (var file in templates)
                            {
                                var name =  file;
                                archive.CreateEntryFromFile(file, "Templates/" +Path.GetFileName(name));
                            }

                        }
                       
                        using (var fileStream = new FileStream(temp, FileMode.Create))
                        {
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            memoryStream.CopyTo(fileStream);
                        }
                    }
                    result =  true;
                }
                catch { result =  false; }
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

        protected void CreateShipper_Click(object sender, EventArgs e)
        {
            EmailAddress email = new EmailAddress();
            email.Name = ShipperNameInput.Text;
            email.Email = ShipperEmialInput.Text;
            Global.Library.Settings.ShippingPersons.Add(email);
            Push.AppSettings();
        }

        protected void CreateEngineerBtn_Click(object sender, EventArgs e)
        {
            EmailAddress email = new EmailAddress();
            email.Name = EngineerNameInput.Text;
            email.Email = EngineerEmailInput.Text;
            Global.Library.Settings.ServiceEngineers.Add(email);
            Push.AppSettings();
        }

        protected void CreateCustomerBtn_Click(object sender, EventArgs e)
        {
            Customer cust = new Customer();
            cust.CompanyName = SprCompany.Text;
            cust.Address = SprAddr.Text;
            cust.Address2 = SprAddr2.Text;
            cust.Attn = SprName.Text;
            cust.City = SprCompany.Text;
            cust.State = SprState.Text;
            cust.Postal = SprPostal.Text;
            cust.Country = SprCountry.Text;
            cust.EmailAddress = new EmailAddress() { Email = SprEmail.Text, Name= cust.Attn };
            cust.Phone = SprPhone.Text;
            Global.Library.Settings.Customers.Add(cust);
            Push.AppSettings();
        }

        protected void AddStatic_Click(object sender, EventArgs e)
        {
            AddStaticPlaceHolder.Visible = true;
            staticupdatepanel.Update();
        }


        protected void DeleteStatic_Command(object sender, CommandEventArgs e)
        {
            try
            {
                var idx = Global.Library.Settings.StaticEmails.FindIndex(x => x.Email == (string)e.CommandArgument);
                Global.Library.Settings.StaticEmails.RemoveAt(idx);
            }
            catch (Exception)
            {
                
            }
        }
        public void BindStaticEmails()
        {
            try
            {
                StaticEmailRepeater.DataSource = Global.Library.Settings.StaticEmails;
                StaticEmailRepeater.DataBind();
            }
            catch { }
        }
        protected void AddStaticSubmit_Click(object sender, EventArgs e)
        {
            var controls = staticupdatepanel.Controls[0].Controls[1].Controls;
            var a = 0;
            var name = (controls[1] as HtmlInputText).Value;
            var email = (controls[3] as HtmlInputText).Value;
            
            EmailAddress stat = new EmailAddress() { Name=name, Email=email };
            if (EmailHelper.IsValid(stat))
            {
                Global.Library.Settings.StaticEmails.Add(stat);
                Push.AppSettings();
                BindStaticEmails();
                staticupdatepanel.Update();
            }
            else
            {
                ShowError("Email is not valid");
            }
            
        }

        protected void ChangeTestModeBtn_Click(object sender, EventArgs e)
        {
            Global.Library.Settings.TESTMODE = !Global.Library.Settings.TESTMODE;
            Push.AppSettings();
            TestModeLabel.Text = Global.Library.Settings.TESTMODE.ToString();

        }

        protected void AssetManagerBtn_Click(object sender, EventArgs e)
        {
            AdminMultiView.ActiveViewIndex = 5;
        }

        protected void AddCalibBtn_Command(object sender, CommandEventArgs e)
        {

        }

        protected void DeleteAssetBtn_Command(object sender, CommandEventArgs e)
        {
          
        }
    }
    
}
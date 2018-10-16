using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using ShippingAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using Web_App_Master;
using Web_App_Master.Models;

namespace Helpers
{

    public class CustomerBindinglist : BindingList<Customer>
    {
        public CustomerBindinglist() { this.AllowEdit = AllowNew = AllowRemove = true; }
        public CustomerBindinglist(List<Customer> notices)
        {
            this.AllowEdit = AllowNew = AllowRemove = true;
            foreach (var notice in notices)
            {
                this.Add(notice);
            }
        }
    }

    public class AssetNoticeBindinglist : BindingList<AssetNotification>
    {
        public AssetNoticeBindinglist() { this.AllowEdit = AllowNew = AllowRemove = true; }
        public AssetNoticeBindinglist(List<AssetNotification> notices)
        {
            this.AllowEdit = AllowNew = AllowRemove = true;
            foreach (var notice in notices)
            {
                this.Add(notice);
            }
        }
    }
  
    public class EmailBindinglist : BindingList<EmailAddress>
    {
        public EmailBindinglist() { this.AllowEdit = AllowNew = AllowRemove = true; }
        public EmailBindinglist(List<EmailAddress> emails)
        {
            this.AllowEdit = AllowNew = AllowRemove = true;
            foreach (var email in emails)
            {
                this.Add(email);
            }
        }
    }

    public class UserBindinglist : BindingList<BindableUserData>
    {
        public UserBindinglist() { }
        public UserBindinglist(List<ApplicationUser> users)
        {
            foreach(var user in users)
            {
                this.Add(new BindableUserData() { IdentityUserRole = new IdentityUserRole() { UserId = user.Id, RoleId = user.Email } });
            }
        }
    }
    
    public class RoleBindinglist: BindingList<BindableRoleData>
    {
        public RoleBindinglist()
        {
            this.AddingNew += RoleBindinglist_AddingNew;
        }
        public RoleBindinglist(List<IdentityRole> roles)
        {
            this.AddingNew += RoleBindinglist_AddingNew;
            foreach(var role in roles)
            {
                this.Add(new BindableRoleData( role));
            }
        }

        private void RoleBindinglist_AddingNew(object sender, AddingNewEventArgs e)
        {
            var data = e.NewObject as BindableRoleData;
            data.RoleIndex = (sender as RoleBindinglist).Count.ToString();
        }
    }
 
    public class BindableRoleData
    {
        public BindableRoleData() { }
        public BindableRoleData(IdentityRole role)
        {
            this.iRole = role;
        }
        public IdentityRole iRole { get; set; }
        public string RoleName { get
            {
                return iRole.Name;
            }
            }
        public string RoleIndex { get; set; }
        public List<BindableUserData> RoleUsers {
            get
            {
                List<BindableUserData> users = new List<BindableUserData>();

                foreach(var user in iRole.Users)
                {
                    BindableUserData ud = new BindableUserData(user);
                    users.Add(ud);
                }
                return users;
            }
        }

    }
   
    public class BindableUserData
    {
        public BindableUserData()
        {
           
        }
        public BindableUserData( IdentityUserRole iur)
        {
            this.IdentityUserRole = iur;
        }
        public IdentityUserRole IdentityUserRole { get; set; }
        public string UserId { get
            {
                var manager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                return (from user in manager.Users where user.Id==this.IdentityUserRole.UserId select user ).FirstOrDefault().Id; 
            }
        }
        public string UserName
        {
            get
            {
                var manager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                return (from user in manager.Users where user.Id == this.IdentityUserRole.UserId select user).FirstOrDefault().Email;
            }
        }
        public string RoleId { get { return this.IdentityUserRole.RoleId; } }
    }
  
    
    public class CheckOutData: ICloneable
    {
        public UPScode ParseUpsCodeString(string code)
        {
            try
            {
                var a = code.Split('-')[0];
                var ret = (UPScode)Enum.Parse(typeof(UPScode), a);
                return ret;
            }
            catch { return  UPScode.Ground; }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Address From = new Address();
        public Address To = new Address();
        public Address Shipper = new Address();
        public UPScode Code = UPScode.Ground;
        public Package Package = new Package();
        public DateTime Pickup = DateTime.Now;
        public DateTime Arrival = DateTime.Now.AddDays(3);
        public string Reference1 = "";
        public string Reference2 = "";
        public List<Asset> CheckOutItems;
    }    
    [Serializable]
    public class SqlSetting: Serializers.Serializer<SqlSetting>
    {        
        public SqlSetting()
        {
            this.XmlData = Type = "";
            this.Guid = System.Guid.NewGuid().ToString();
        }
        public SqlSetting(System.Guid guid)
        {
            this.XmlData = Type = "";
            this.Guid = guid.ToString();
        }
        [XmlElement]
        public string XmlData { get; set; }
        [XmlElement]
        public string Type { get; set; }
        [XmlElement]
        public string Guid { get; set; }
    }
    [Serializable]
    public class MenuAlert : Serializers.Serializer<MenuAlert>
    {
        public MenuAlert()
        {
            this.XmlData = Text = Name = "";
            this.Guid = System.Guid.NewGuid().ToString();
        }
        public MenuAlert(string name)
        {
            Name = name;
            this.XmlData = Text = "";
            this.Guid = System.Guid.NewGuid().ToString();
        }
        public MenuAlert(string name, string text)
        {
            Name = name;
            Text = text;
            this.XmlData = "";
            this.Guid = System.Guid.NewGuid().ToString();
        }
        public MenuAlert(string name, string text, string xml)
        {
            Name = name;
            Text = text;
            this.XmlData = xml;
            this.Guid = System.Guid.NewGuid().ToString();
        }
        public MenuAlert(System.Guid guid)
        {
            this.XmlData = Text = Name = "";
            this.Guid = guid.ToString();
        }
        [XmlElement]
        public DateTime Created = DateTime.Now;
        [XmlElement]
        public string XmlData { get; set; }
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public string Guid { get; set; }
        [XmlElement]
        public string Text { get; set; }
    }
    [Serializable]
    public class TransationDBData : SettingsDBData
    {
        public TransationDBData(PendingTransaction transaction)
        {
            this.XmlData = transaction.SerializeToXmlString(transaction);
            this.TransactionID = transaction.TransactionID;
            this.Appname = "AWP_STARRAG_US";
        }
        public TransationDBData()
        {
            Appname = XmlData = TransactionID = "";
        }        
        [XmlElement]
        public string TransactionID { get; set; }
       
    }
    [Serializable]
    public class SettingsDBData:Serializers.Serializer<SettingsDBData>
    {
        public SettingsDBData()
        {
            Appname = XmlData = XmlData2 = XmlData3 = XmlData4 = XmlData5="";
        }
        [XmlElement]
        public string Appname { get; set; }
        [XmlElement]
        public string XmlData { get; set; }
        [XmlElement]
        public string XmlData2 { get; set; }
        [XmlElement]
        public string XmlData3 { get; set; }
        [XmlElement]
        public string XmlData4 { get; set; }
        [XmlElement]
        public string XmlData5 { get; set; }
    }
    [Serializable]//xml
    public class CalibrationLibrary : Serializers.Serializer<CalibrationLibrary>
    {
        [XmlElement("Calibrations")]
        public List<CalibrationData> Calibrations = new List<CalibrationData>();
        [XmlIgnore]
        public object Tag;

    }

    [Serializable]//xml
    public class CalibrationData : Serializers.Serializer<CalibrationData>, ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public CalibrationData()
        {
            LastCalibration = DateTime.Now;
            AssetNumber = "";
            m_SchedulePeriod = "12";
            CalibrationCompany ="";
            m_IsCalibrated = true;
                 History = new List<string>();
            ScheduledCalibrationDate = LastCalibration.AddMonths(Convert.ToInt32(m_SchedulePeriod));
        }
        
        public bool CheckCalibration()
        {

            int result = DateTime.Compare(ScheduledCalibrationDate, DateTime.Now);
            int result2 = DateTime.Compare(DateTime.Now, ScheduledCalibrationDate);
            DaysUntilNextCalibration = (ScheduledCalibrationDate - DateTime.Now).TotalDays;
            if (DaysUntilNextCalibration <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        [XmlElement("LastCalibration")]
        public DateTime LastCalibration { get; set; }
        [XmlElement("ScheduledCalibrationDate")]
        public DateTime ScheduledCalibrationDate { get; set; }
        [XmlAttribute("AssetNumber")]
        public string AssetNumber { get; set; }
        [XmlElement]
        public string m_guid = System.Guid.NewGuid().ToString();
        [XmlElement]
        public string Guid { get { return m_guid; } set { m_guid = value; } }
        [XmlIgnore]
        private string m_SchedulePeriod = "12";
        [XmlElement("SchedulePeriod")]
        public string SchedulePeriod
        {
            get { return m_SchedulePeriod; }
            set
            {
                m_SchedulePeriod = value;
                ScheduledCalibrationDate = LastCalibration.AddMonths(Convert.ToInt32(m_SchedulePeriod));
                CheckCalibration();
            }
        }
        [XmlElement("CalibrationCompany")]
        public string CalibrationCompany { get; set; }
        [XmlIgnore]
        public bool m_IsCalibrated = true;
        [XmlElement("IsCalibrated")]
        public bool IsCalibrated
        {
            get { return CheckCalibration(); }
            set { m_IsCalibrated = value; }
        }
        [XmlIgnore]
        public string DaysLeft { get { return DaysUntilNextCalibration.ToString(); } }
        [XmlIgnore]
        public object Tag;
        [XmlIgnore]
        public object Tag2;
        [XmlIgnore]
        private double m_days;
        [XmlElement("DaysUntilNextCalibration")]
        public double DaysUntilNextCalibration
        {
            get
            {
                m_days = (ScheduledCalibrationDate - DateTime.Now).TotalDays;
                if (m_days <= 0)
                {
                    IsCalibrated = false;
                }
                else
                {
                    IsCalibrated = true;
                }

                return m_days;
            }
            set { m_days = value; }
        }
        [XmlElement("ImagePath")]
        public string ImagesPath { get; set; }

        [XmlElement("History")]
        public List<string> History { get; set; }
        public string FirstImage { get
            {
                try
                {
                    var files = Directory.GetFiles(HttpContext.Current.Server.MapPath(ImagesPath));
                    return files[0];
                }
                catch { return "/images/transparent.png"; }
            } }
        [XmlElement]
        public string Description = "";
        [XmlElement]
        public string FilePath = "";
        public string FileName
        {
            get
            {
                try
                {
                    return Path.GetFileName(FilePath);
                }
                catch (Exception)
                {
                    return "";
                }
            } }
    }

    [Serializable]//xml
    public class UPSaccount:Serializers.Serializer<UPSaccount>
    {
        public void Write(string path)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(stream, this);
                }
            }
            catch { }
        }
        public static UPSaccount Read(string path)
        {
            UPSaccount obj = new UPSaccount();
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(path,
                                          FileMode.Open,
                                          FileAccess.Read,
                                          FileShare.Read);
                obj = (UPSaccount)formatter.Deserialize(stream);
                stream.Close();
            }
            catch { }
            return obj;
        }
        [XmlElement("P")]
        public string P;
        [XmlElement("A")]
        public string A;
        [XmlElement("I")]
        public string I;
        [XmlElement("N")]
        public string N;
    }    

    [Serializable]//xml
    public class Email:Serializers.Serializer<Email>
    {
        [XmlElement("Address")]
        public EmailAddress Address;
        [XmlElement("Subject")]
        public string Subject;
        [XmlElement("Body")]
        public string Body;
        [XmlElement("Attachments")]
        public object Attachments;
        [XmlElement("Time")]
        public DateTime Time;
    }

    [Serializable]//xml
    public class EmailAddress:Serializers.Serializer<EmailAddress>
    {
        public static EmailAddress Create(string email)
        {

            EmailAddress e = new EmailAddress();
            e.Email = email;
            if (!e.Email.Contains("@"))
            {
                e.Email = "servicetools.starrag@gmail.com";
            }
            try
            {
                if (email.Contains("@"))
                {
                    var temp = email.Split('@');
                    e.Name = temp[0];
                    if (temp[0].Contains("."))
                    {
                        e.Name = temp[0].Replace(".", " ");
                    }
                }
            }
            catch
            {
                e.Name = e.Email;
            }

            return e;
        }
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Email")]
        public string Email { get; set; }
    }

    [Serializable]//xml
    public class Customer:Serializers.Serializer<Customer>
    {
        public Customer()
        {
            DataGuid = "";
           CompanyName = "";
            Attn = "";
        Address = "";
        Address2 = "";
         Address3 = "";
         City = "";
        State = "";
            NickName = "";
            ResInd = "";
            LocID = "";
            ConsInd = "";
            AccountNumber = "";
            AccPostalCd = "";
            USPSPOBoxIND = "";
        Postal = "";
         Country = "";
         PackageWeight = "";
         Phone = "";
         Fax = "";
         EmailAddress = new EmailAddress();
         OrderNumbers = new List<string>();
        OrderNumber = "";
         CurrentAssignedAssets = new List<string>();
        Tag = null;

    }
        public static Customer Create(string coName, string attn, string address, string phone, string fax, EmailAddress email)
        {
            Customer c = new Customer();
            c.CompanyName = coName;
            c.Attn = attn;
            c.Address = address;
            c.Phone = phone;
            c.Fax = fax;
            c.EmailAddress = email;
            return c;
        }
        [XmlElement("CompanyName")]
        public string CompanyName { get; set; }
        [XmlElement("Attn")]
        public string Attn { get; set; }
        [XmlElement("Address")]
        public string Address { get; set; }
        [XmlElement("Address2")]
        public string Address2 { get; set; }
        [XmlElement("Address3")]
        public string Address3 { get; set; }
        [XmlElement("City")]
        public string City { get; set; }
        [XmlElement("State")]
        public string State { get; set; }
        [XmlElement("NickName")]
        public string NickName { get; set; }
        [XmlElement("ResInd")]
        public string ResInd { get; set; }
        [XmlElement("LocID")]
        public string LocID { get; set; }
        [XmlElement("ConsInd")]
        public string ConsInd { get; set; }
        [XmlElement("AccountNumber")]
        public string AccountNumber { get; set; }
        [XmlElement("AccPostalCd")]
        public string AccPostalCd { get; set; }
        [XmlElement("USPSPOBoxIND")]
        public string USPSPOBoxIND { get; set; }
        [XmlElement("Postal")]
        public string Postal { get; set; }
        [XmlElement("Country")]
        public string Country { get; set; }
        [XmlElement("PackageWeight")]
        public string PackageWeight { get; set; }
        [XmlElement("Phone")]
        public string Phone { get; set; }
        [XmlElement("Fax")]
        public string Fax { get; set; }
        [XmlElement("EmailAddress")]
        public EmailAddress EmailAddress { get; set; }
        [XmlElement("Email")]
        public string Email { get { return this.EmailAddress.Email; } }
        [XmlElement("OrderNumbers")]
        public List<string> OrderNumbers { get; set; }
        [XmlElement("OrderNumber")]
        public string OrderNumber { get; set; }
        [XmlElement("CurrentAssignedAssets")]
        public List<string> CurrentAssignedAssets { get; set; }
        [XmlElement("DataGuid")]
        public string DataGuid { get; set; }
        [XmlIgnore]
        public object Tag { get; set; }
       
    }

    [Serializable]//xml
    public class Engineer:Serializers.Serializer<Engineer>
    {
        public Engineer()
        { }
        public static Engineer Create(string name, string address, string phone, EmailAddress email)
        {
            Engineer e = new Engineer();
            e.Name = name;
            e.Address = address;
            e.Phone = phone;
            e.Email = email;
            return e;
        }
        [XmlElement("Name")]
        public string Name = "";
        [XmlElement("Address")]
        public string Address = "";
        [XmlElement("Phone")]
        public string Phone = "";
        [XmlElement("Email")]
        public EmailAddress Email = new EmailAddress();
        [XmlIgnore]
        public object Tag = null;
    }

    [Serializable]//xml
    public class NotificationLibrary:Serializers.Serializer<NotificationLibrary>
    {
        public NotificationLibrary()
        {

        }
        [XmlElement("ThrityDayNotifications")]
        public List<AssetNotification> ThrityDayNotifications = new List<AssetNotification>();
        [XmlElement("FifteenDayNotifications")]
        public List<AssetNotification> FifteenDayNotifications = new List<AssetNotification>();
    }

    [Serializable]//xml
    public class Backup:Serializers.Serializer<Backup>
    {
        public Backup()
        {
        }
        [XmlAttribute("LastBackup")]
        public DateTime LastBackup;
        [XmlElement("Tag")]
        public object Tag;
        [XmlElement("Tag2")]
        public object Tag2;
    }

    [Serializable]//xml
    public class AssetNotification:Serializers.Serializer<AssetNotification>
    {
        public static AssetNotification Create(string assetNumber, List<EmailAddress> emails, bool isNotified = false)
        {
            AssetNotification n = new AssetNotification();
            n.Emails = new List<EmailAddress>();
            n.Emails.AddRange(emails);
            n.AssetNumber = assetNumber;
            n.Time = DateTime.Now;
            n.LastNotified = DateTime.Now;
            n.IsNotified = isNotified;
            return n;
        }
        public AssetNotification()
        {
            AssetNumber = "";
            Emails = new List<EmailAddress>();
        }
        [XmlElement("Emails")]
        public List<EmailAddress> Emails { get; set; }
        [XmlElement("AssetNumber")]
        public string AssetNumber { get; set; }
        [XmlElement("Time")]
        public DateTime Time { get; set; }
        [XmlElement("LastNotified")]
        public DateTime LastNotified { get; set; }
        [XmlElement("IsNotified")]
        public bool IsNotified { get; set; }
        [XmlElement("Is30Day")]
        public bool Is30Day { get; set; }
        [XmlElement("Is15Day")]
        public bool Is15Day { get; set; }
        [XmlIgnore]
        public object Tag { get; set; }
        [XmlElement("EmailsCSV")]
        public string EmailsCSV { get {
                string ret = "";
                foreach(var email in Emails)
                {
                    ret += email.Name + ", ";
                }
                return ret;
            }
        }

    }


    [Serializable]//xml
    public class Settings: Serializers.Serializer<Settings>, ICloneable
    {
        public static Settings GetInstance()
        {
            return new Settings();
        }
        public Settings()
        {
            
        }
        public Settings Deserialize(string xml)
        {
            return this.DeserializeFromXmlString<Settings>(xml);
        }
        public string Serialize()
        {
            return this.SerializeToXmlString(this);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        [XmlAttribute("LibraryPath")]
        public string LibraryPath;

        [XmlAttribute("AssetsPath")]
        public string AssetsPath;

        [XmlAttribute("TimeStamp")]
        public string TimeStamp = DateTime.Now.ToString();
        
        [XmlElement("Notifications_30_Day")]
        public List<AssetNotification> Notifications_30_Day = new List<AssetNotification>();

        [XmlElement("Notifications_15_Day")]
        public List<AssetNotification> Notifications_15_Day = new List<AssetNotification>();

        [XmlElement("NotSentEmails")]
        public List<Email> NotSentEmails = new List<Email>();

        [XmlElement("ServiceEngineers")]
        public List<EmailAddress> ServiceEngineers = new List<EmailAddress>();

        [XmlElement("Customers")]
        public List<Customer> Customers;

        [XmlElement("StaticEmails")]
        public List<EmailAddress> StaticEmails;

        [XmlElement("ShippingPersons")]
        public List<EmailAddress> ShippingPersons;

        [XmlElement("CheckOutMessage")]
        public string CheckOutMessage = "";

        [XmlElement("CheckInMessage")]
        public string CheckInMessage = "";

        [XmlElement("NotificationMessage")]
        public string NotificationMessage = "";       

        [XmlElement("ShipperNotification")]
        public string ShipperNotification = "";

        [XmlElement("UpsAccount")]
        public UPSaccount UpsAccount = new UPSaccount();

        [XmlElement]
        public bool TESTMODE = false;

        [XmlElement]
        public int AssetNumberLength = 4;
        [XmlElement]
        public string SenderEmail = "provider.service.secure@gmail.com";
        [XmlElement]
        public string SenderPassword = "@Service2";
        [XmlElement]
        public string SenderHost = "smtp.gmail.com";
        [XmlElement]
        public int SenderPort = 587;
        [XmlElement]
        public string CalibrationMessage = @"Hello <name>,<NL><NL>
                Calibrated Tool, <serviceTool>, is out of calibration <NL> 
                Please send tool for calibration.<NL><NL>
                Kind Regards,<NL><NL>
                Support Team
                ";
        [XmlElement]
        public List<string> AssetNumbers = new List<string>();

    }

    [Serializable]//xml
    public class Asset : Serializers.Serializer<Asset>
    {
        public Asset Deserialize(string xml)
        {
            return this.DeserializeFromXmlString<Asset>(xml);
        }
        public string Serialize()
        {
            return this.SerializeToXmlString(this);
        }
        public static Asset Create(string assetName = "", string assetNumber = "")
        {
            
            Asset ass = new Asset().Init();            
            ass.AssetName = assetName;
            ass.AssetNumber = assetNumber;
            return ass;
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public Asset()
        {
            Init();
        }
        private Asset Init()
        {
            Images = "";
            IsOut = false;
            weight = 0;
            History = new AssetHistory();
            AssetNumber = "0000";
            OrderNumber = "0";
            AssetName = PackingSlip =UpsLabel = ReturnReport  = ShipTo = ServiceEngineer= PersonShipping=  Barcode = Description= BarcodeImage= CalibrationCompany= CalibrationPeriod=   "";
            IsOut = IsDamaged = OnHold = false;
            CalibrationHistory = new CalibrationLibrary();
            Documents = new List<string>();
            return this;
        }
        [XmlElement]
        public string FirstImage{ get
            {
                if (this.Images=="")
                {
                    return "/Images/transparent.png";
                }
                var img = Images.Split(',')[0].Replace(",,,", "<@#$>").Replace(",", "").Replace("<@#$>", ",").Replace("Images", "").Replace("\\", "");
                img = "/Account/Images/"+AssetNumber+"/" + img;
                return img;
            }
        }
        [XmlElement]
        public string Images { get; set; }
        [XmlElement]
        public string AssetName { get; set; }
        [XmlElement]
        public string OrderNumber { get; set; }
        [XmlElement]
        public string ShipTo { get; set; }
        [XmlElement]
        public DateTime DateShipped { get; set; }
        [XmlElement]
        public DateTime LastCalibrated { get; set; }
        [XmlElement]
        public DateTime DateRecieved { get; set; }
        [XmlElement]
        public string DateRecievedString { get { return DateRecieved.ToString(); } }
        [XmlElement]
        public string LastCalibratedString { get { return LastCalibrated.ToString(); } }
        [XmlElement]
        public string DateShippedString { get { return DateShipped.ToString(); } }
        [XmlIgnore]
        public string DateShippedTicks { get { return DateShipped.Ticks.ToString(); } }
        [XmlElement]
        public string ServiceEngineer { get; set; }
        [XmlElement]
        public string PersonShipping { get; set; }
        [XmlElement]
        public string Barcode { get; set; }
        [XmlElement]
        public string AssetNumber { get; set; }// "0000";
        [XmlElement]
        public string Description { get; set; }
        [XmlElement("BarcodeImage")]
        public string BarcodeImage  { get; set; }
        [XmlElement("IsOut")]
        public bool IsOut { get; set; }
        [XmlElement("IsDamaged")]
        public bool IsDamaged { get; set; }
        [XmlElement("IsCalibrated")]
        public bool IsCalibrated { get; set; }
        [XmlElement("CalibrationCompany")]
        public string CalibrationCompany { get; set; }
        [XmlElement("CalibrationPeriod")]
        public string CalibrationPeriod { get; set; }
        [XmlElement("CalibrationHistory")]
        public CalibrationLibrary CalibrationHistory { get; set; }
        [XmlElement("OnHold")]
        public bool OnHold { get; set; }
        [XmlElement("weight")]
        public decimal weight { get; set; }
        [XmlElement("PackingSlip")]
        public string PackingSlip { get; set; }
        [XmlElement("UpsLabel")]
        public string UpsLabel { get; set; }
        [XmlElement("ReturnReport")]
        public string ReturnReport { get; set; }
        [XmlElement("History")]
        public AssetHistory History { get; set; }
        [XmlElement]
        public bool IsHistoryItem { get; set; }
        [XmlIgnore]
        public string Color
        {
            get
            {
                if (IsOut)
                    return "bg-sg-amber";
                if (IsDamaged)
                    return "bg-sg-red";
                if (OnHold)
                    return "bg-sg-violet";
               
                return "bg-sg-blue";
            }
        }
        [XmlElement]
        public List<string> Documents { get; set; }
        [XmlIgnore]
        public List<string> ImageList {
            get
            {
                return Images.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            set
            {
                Images = "";
                foreach (var img in value)
                {
                    Images += img + ",";
                }
            }
        }

    }

    [Serializable]
    public class AssetHistory:Serializers.Serializer<AssetHistory>
    {
        public AssetHistory Deserialize(string xml)
        {
            return this.DeserializeFromXmlString<AssetHistory>(xml);
        }
        public string Serialize()
        {
            return this.SerializeToXmlString(this);
        }
        public AssetHistory()
        {
            History = new List<Asset>();
        }
       [XmlElement]
       public List<Asset> History { get; set; }
    }
    [Serializable]//xml
    public class DataStore:Serializers.Serializer<DataStore>, ICloneable
    {
        [XmlElement]
        public Settings Settings = new Settings();
        [XmlElement]
        public List<Asset> Assets = new List<Asset>();
        [XmlElement]
        public Notification.NotificationSystem NotificationSystem = new Notification.NotificationSystem();
        [XmlElement]
        public string Name;
        [XmlElement]
        public string Filename;
        [XmlIgnore]
        public object Tag;       
        public DataStore()
        {
            Settings = new Settings();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        [XmlElement]
        public CalibrationLibrary Certificates = new CalibrationLibrary();
    }
    [Serializable]    
    public class EmailNotice :  Notification.NotificationSystem.Notice
    {
        public static EmailNotice Create(List<string> assets, List<EmailAddress> emails,string body, string subject)
        {
            EmailNotice n = new EmailNotice();
            n.Emails = new List<EmailAddress>();
            n.Emails.AddRange(emails);
            n.Assets = assets;
            n.Body = body;
            n.Subject = subject;
            return n;
        }
        public EmailNotice()
        {
            Body = ""; Subject = "";
            Emails = new List<EmailAddress>();
            Assets = new List<string>();
        }
        [XmlElement("Emails")]
        public List<EmailAddress> Emails { get; set; }
        [XmlElement]
        public List<string> Assets = new List<string>();
        [XmlElement]
        public string Body { get; set; }
        [XmlElement]
        public string Subject { get; set; }
        [XmlIgnore]
        public object Tag { get; set; }
        [XmlElement("EmailsCSV")]
        public string EmailsCSV
        {
            get
            {
                string ret = "";
                foreach (var email in Emails)
                {
                    ret += email.Name + ", ";
                }
                return ret;
            }
        }

    }  
    [Serializable]    
    public class PendingTransaction : Serializers.Serializer<PendingTransaction>
    {
        public static PendingTransaction Create()
        {
            PendingTransaction n = new PendingTransaction();
            
            return n;
        }
        public PendingTransaction()
        {
            TransactionID = Guid.NewGuid().ToString();
            Name = HttpContext.Current.User.Identity.Name;
            var list = from a in Global.Library.Settings.ServiceEngineers where a.Email == HttpContext.Current.User.Identity.Name select a;
            if (list.Count()==0)
            {
                Email = new EmailAddress() { Name = HttpContext.Current.User.Identity.Name, Email = HttpContext.Current.User.Identity.Name };
            }else
            {
                Email = list.First();
            }
            Comment = "";
            Customer = new Customer();
            Assets = new List<string>();
            ConfirmationNumber = "";
            Date = DateTime.Now;
        }
        public PendingTransaction( List<Asset> assets)
        {
            TransactionID = Guid.NewGuid().ToString();
            ConfirmationNumber = "";
            Comment = "";
            Name = HttpContext.Current.User.Identity.Name;
            var list = from a in Global.Library.Settings.ServiceEngineers where a.Email == HttpContext.Current.User.Identity.Name select a;
            if (list.Count() == 0)
            {
                Email = new EmailAddress() { Name = HttpContext.Current.User.Identity.Name, Email = HttpContext.Current.User.Identity.Name };
            }
            else
            {
                Email = list.First();
            }
            Customer = new Customer();
            this.Assets = (from a in assets select a.AssetNumber).ToList();
            Date = DateTime.Now;
        }
        [XmlElement]
        public EmailAddress Email { get; set; }
        [XmlElement]
        public DateTime Date { get; set; }

        //THIS IS NOT SERIALIZING
        [XmlElement]
        public List<string> Assets = new List<string>();

        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public string TransactionID { get; set; }
        [XmlElement]
        public string ConfirmationNumber { get; set; }
        [XmlElement]
        public string Comment { get; set; }
        [XmlElement]
        public Customer Customer { get; set; }
        [XmlIgnore]
        public object Tag { get; set; }       

    }
    [Serializable]    
    public class CustomerData : Serializers.Serializer<CustomerData>
    {
        public static CustomerData Create()
        {
            CustomerData n = new CustomerData();
            
            return n;
        }

        public CustomerData()
        {
            Guid = System.Guid.NewGuid().ToString();
            Data = HttpContext.Current.User.Identity.Name;
            var list = from a in Global.Library.Settings.ServiceEngineers where a.Email == HttpContext.Current.User.Identity.Name select a;
            OrderNumbers = new List<string>();
            Comment = "";
            Customer = new Customer();
            Assets = new List<string>();
            Documents = new List<string>();
            Date = DateTime.Now;
        }

        public CustomerData( List<Asset> assets)
        {
            Guid = System.Guid.NewGuid().ToString();
            Documents = new List<string>();
            Comment = "";
            Data = HttpContext.Current.User.Identity.Name;
            Customer = new Customer();
            this.Assets = (from a in assets select a.AssetNumber).ToList();
            Date = DateTime.Now;
            OrderNumbers = new List<string>();
        }
        
        [XmlElement]
        public DateTime Date { get; set; }

        [XmlElement]
        public List<string> Assets = new List<string>();

        [XmlElement]
        public string Data { get; set; }

        [XmlElement]
        public string Guid { get; set; }

        [XmlElement]
        public List<string> Documents { get; set; }

        [XmlElement]
        public string Comment { get; set; }      

        [XmlElement]
        public Customer Customer { get; set; }

        [XmlElement]
        public List<AssetKit> AssetKitHistory { get; set; }

        [XmlElement]
        public List<string> OrderNumbers { get; set; }

        [XmlIgnore]
        public object Tag { get; set; }       
    }
    [Serializable]
    public class AssetKit : Serializers.Serializer<AssetKit>
    {
        public AssetKit()
        {
            Guid = System.Guid.NewGuid().ToString();
            CustomerDataGuid = "";
            Assets = new List<string>();
        }

        [XmlElement]
        public string Guid { get; set; }
       
        [XmlElement]
        public List<string> Assets { get; set; }

        [XmlElement]
        public string CustomerDataGuid { get; set; }

        [XmlIgnore]
        public string AssetsString { get
            {
                string ret = "";
                Assets.ForEach((asset) => { ret += asset + ","; });
                return ret;
            }
        }

    }

    [Serializable]
    public class PollItems : Serializers.Serializer<PollItems>
    {
        public PollItems()
        {
            Guid = System.Guid.NewGuid().ToString();      
        }

        [XmlElement]
        public string Guid { get; set; }

        [XmlElement]
        public List<Asset> InOutItems { get; set; }
        [XmlElement]
        public List<PendingTransaction> Transactions { get; set; }
        [XmlElement]
        public List<MenuAlert> Notices { get; set; }

    }

}
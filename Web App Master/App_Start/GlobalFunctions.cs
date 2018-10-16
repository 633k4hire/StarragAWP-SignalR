using Helpers;
using Notification;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Web_App_Master.Account;
using Web_App_Master.App_Start;

namespace Web_App_Master
{
    //MAKE ALL OF THESE ASYNC
    public class Push
    {
        public static bool Asset(Asset asset)
        {
            try
            {
                var local = Global.AssetCache.Find((x) => x.AssetNumber == asset.AssetNumber);
                if (local != null)
                {
                    local = asset.Clone() as Asset;
                    var local2 = Global.AssetCache.Find((x) => x.AssetNumber == asset.AssetNumber);
                    Global.AssetCache.Update(asset);
                    return AssetController.UpdateAsset(asset);
                }
                else
                {
                    Global.AssetCache.Add(asset);
                    return AssetController.AddNewAsset(asset);
                }
               
                
            }
            catch { return false; }
        }
        public static Task<bool> SettingAsync(SettingsDBData data)
        {
            return Task.Run(() =>
            {
                try
                {
                    return AssetController.PushSetting(data);

                }
                catch { return false; }
            });
        }
        public static Task<bool> GlobalsAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    return AssetController.PushSettings(Global.Library.Settings);

                }
                catch { return false; }
            });
        }
        public static Task<bool> LibraryAsync()
        {
            return Task.Run(() =>
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

                    }
                    req.CloseConnection();
                    return true;
                }
                catch { return false; }
            });
        }
        public static Task<bool> NotificationAsync(NotificationSystem.Notice notice)
        {
            return Task.Run(() =>
            {
                try
                {
                    SettingsDBData db = new SettingsDBData();
                    db.Appname = notice.Guid;
                    db.XmlData = notice.SerializeToXmlString(notice);
                    return AssetController.PushSetting(db);
                }
                catch { return false; }
            });
        }
        public static Task<bool> NotificationSystemAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    SettingsDBData db = new SettingsDBData();
                    db.Appname = Global.NoticeSystem.Guid;
                    db.XmlData = Global.NoticeSystem.SerializeToXmlString(Global.NoticeSystem);
                    return AssetController.PushSetting(db);
                }
                catch { return false; }
            });
        }

        public static bool Setting(SettingsDBData data)
        {
            try
            {
                return AssetController.PushSetting(data);
               
            }
            catch { return false; }
        }
        public static bool AppSettings()
        {
            try
            {
                 return AssetController.PushSettings(Global.Library.Settings);
               
            }
            catch { return false; }
        }
        public static bool Library()
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

                }
                req.CloseConnection();
                return true;
            }
            catch
            {
                return false;
            }

            
        }
        public static bool Notification(NotificationSystem.Notice notice)
        {
            try
            {
                SettingsDBData db = new SettingsDBData();
                db.Appname = notice.Guid;
                db.XmlData = notice.SerializeToXmlString(notice);
                return AssetController.PushSetting(db);               
            }
            catch { return false; }
        }
        public static bool NotificationSystem()
        {
            try
            {
                SettingsDBData db = new SettingsDBData();
                db.Appname = Global.NoticeSystem.Guid;
                db.XmlData = Global.NoticeSystem.SerializeToXmlString(Global.NoticeSystem);
                return AssetController.PushSetting(db);
            }
            catch {
                //Global.NoticeSystem.Notices = new Notification.NotificationSystem.NoticeBindinglist();
                return false; }
            
        }
        public static bool Transaction(PendingTransaction transaction)
        {
            return AssetController.PushTransaction(transaction);
        }
        public static bool Alert(string text)
        {
            
            try
            {
                var list = HttpContext.Current.Session["Notifications"] as List<MenuAlert>;
                MenuAlert n = new MenuAlert();
                n.Name = HttpContext.Current.User.Identity.Name;
                n.Text = text;
                list.Add(n);
                HttpContext.Current.Session["Notifications"] = list;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool Alert(MenuAlert alert)
        {

            try
            {
                var list = HttpContext.Current.Session["Notifications"] as  List<MenuAlert>;               
                list.Add(alert);
                HttpContext.Current.Session["Notifications"] = list;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool Certificates(string app_name = "AWP_Certificate_System")
        {
            try
            {
                SettingsDBData db = new SettingsDBData();
                db.Appname = app_name;
                db.XmlData = Global.Library.Certificates.SerializeToXmlString(Global.Library.Certificates);
                return AssetController.PushSetting(db);
            }
            catch
            {
                //Global.NoticeSystem.Notices = new Notification.NotificationSystem.NoticeBindinglist();
                return false;
            }

        }
        public static bool CustomerData(CustomerData data,  string app_name = "AWP_CustomerData_System")
        {
            try
            {
                SettingsDBData db = new SettingsDBData();
                db.Appname = data.Guid;
                db.XmlData = data.SerializeToXmlString(data);
                return AssetController.PushSetting(db);
            }
            catch
            {
                //Global.NoticeSystem.Notices = new Notification.NotificationSystem.NoticeBindinglist();
                return false;
            }

        }
        public static bool MasterLog(MasterLog log, string appname = "AWP_Master_Log")
        {
            try
            {
                var xml = log.SerializeToXmlString(log);
                return Push.Setting(new SettingsDBData() { Appname = appname, XmlData = xml });                
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
    public class Pull
    {
        public static CalibrationLibrary Certificates(string  app_name = "AWP_Certificate_System")
        {
            try
            {
                var db = AssetController.GetSetting(app_name);
                if (db==null)
                {
                    Global.Library.Certificates = new CalibrationLibrary();
                    Push.Certificates();
                }
                Global.Library.Certificates = new CalibrationLibrary().DeserializeFromXmlString<CalibrationLibrary>(db.XmlData);
                return Global.Library.Certificates;
            }
            catch { Global.Library.Certificates = new CalibrationLibrary(); return null; }
        }
        public static Asset Asset(string num)
        {
            try
            {
                var a = AssetController.GetAsset(num);
                Global.AssetCache.ForEach((t)=> { if (t.AssetNumber == num) { t = a; } }) ;
                return a;
            }
            catch { return null; }
        }
        public static List<Asset> Assets()
        {
            try
            {
                var a = AssetController.GetAllAssets().OrderBy(w => w.AssetNumber).ToList();
                Global.AssetCache = a;
                return a;
               
            }
            catch { return new List<Helpers.Asset>(); }
        }
        public static async Task<List<Asset>> AssetsAsync()
        {
            var a = await AssetController.GetAllAssetsAsync();
                Global.AssetCache = a;
            return a;
        }
        public static SettingsDBData Setting(string guid)
        {
            try
            {
                return AssetController.GetSetting(guid);               
            }
            catch { return null; }
        }
        public static bool Globals()
        {
            try
            {
                Global.Library.Settings = AssetController.GetSettings();
                return true;
            }
            catch
            {
                Global.Library.Settings = new Settings();
                return false;
            }
        }
        public static bool Library()
        {
            try
            {
                var a = Global.AssetCache = AssetController.GetAllAssets();
                Global.AssetCache = a;
                return true;
            }
            catch { Global.Library = new DataStore(); return false; }
        }
        public static bool NotificationSystem(string app_name= "AWP_Notification_System")
        {
            try
            {
                var db = AssetController.GetSetting(app_name);
                Global.NoticeSystem = new NotificationSystem().DeserializeFromXmlString<NotificationSystem>(db.XmlData);
                                
                return true;
            }
            catch { Global.NoticeSystem = new NotificationSystem("AWP_Notification_System"); return false; }
        }
        public static List<NotificationSystem.Notice> Notifications(string app_name = "AWP_Notification_System")
        {
            try
            {
                var db = AssetController.GetSetting(app_name);
                Global.NoticeSystem = new NotificationSystem().DeserializeFromXmlString<NotificationSystem>(db.XmlData);
                return Global.NoticeSystem.Notices.ToList();
                
            }
            catch { Global.NoticeSystem = new NotificationSystem("AWP_Notification_System"); return null; }
        }
        public static NotificationSystem.Notice Notification(string guid)
        {
            try
            {
                var db = AssetController.GetSetting(guid);
                var notice = new NotificationSystem.Notice().DeserializeFromXmlString<NotificationSystem.Notice>(db.XmlData);
                return notice;
            }
            catch { return null; }
        }
        public static NotificationSystem.Notice Notification(NotificationSystem.Notice notice)
        {
            try
            {
                
                var db = AssetController.GetSetting(notice.Guid);
                var n = new NotificationSystem.Notice().DeserializeFromXmlString<NotificationSystem.Notice>(db.XmlData);
                return n;
            }
            catch { return null; }
        }
        public static List<PendingTransaction> Transactions(string AppName = "AWP_STARRAG_US")
        {
            return AssetController.GetAllTransactions(AppName);
        }
        public static PendingTransaction Transaction(string transactionID, string AppName = "AWP_STARRAG_US")
        {
            return AssetController.PullTransaction(transactionID, AppName);
        }
        public static List<MenuAlert> Alerts()
        {            
            return HttpContext.Current.Session["Notifications"] as List<MenuAlert>;
        }
        public static CustomerData CustomerData(string CustomerDataGuid, string AppName = "AWP_CustomerData_System")
        {
            CustomerData cd = null;
            try
            {
                var db = AssetController.GetSetting(CustomerDataGuid);
                if (db == null)
                {                    
                    return cd;
                }
                cd = new CustomerData().DeserializeFromXmlString<CustomerData>(db.XmlData);
            }
            catch { return cd; }
            return cd;
        }
        public static MasterLog MasterLog(string appname = "AWP_Master_Log")
        {            
            var set =Pull.Setting(appname);
            if (set==null)
            {
                Push.MasterLog(Global._MasterLog);
                return Global._MasterLog;
            }
            var ml = new MasterLog().DeserializeFromXmlString<MasterLog>(set.XmlData);
            return ml;
        }
    }
    public class Add
    {

    }
    public class Find
    {

    }
    public class Remove
    {

    }
    public static class ExportLibrary
    {
        // GO BACK AND GET THE REAL ONE R USE THE NEW XML ONE
        public static BackgroundWorker Worker;
        public static void Export(string filename="/library.xml")
        {
            Worker = new BackgroundWorker();
            Worker.DoWork += Worker_DoWork;
            Worker.WorkerReportsProgress = true;
            Worker.WorkerSupportsCancellation = true;
            Global.Library.Name = filename;
            Worker.RunWorkerAsync(Global.Library);

        }
        private static void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            var assetslibrary = e.Argument as DataStore;
            var assets = assetslibrary.Assets;
            var length = assets.Count;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            StringBuilder builder = new StringBuilder();
            builder.Capacity = 5000;
            int i = 0;
            using (XmlWriter writer = XmlWriter.Create(builder, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("AssetList");
                foreach (var s in assets)
                {
                    if (e.Cancel == true)
                    {
                        return;
                    }
                    try
                    {
                        AddAssetToXml(writer, s);
                    }
                    catch { }
                    ++i;
                }
                writer.WriteEndElement();

                writer.WriteEndDocument();
            }
            using (var sr = File.OpenWrite(assetslibrary.Name))
            using (StreamWriter s = new StreamWriter(sr))
            {
                builder.ToString();
                s.WriteLine(builder.ToString());
            }
        }

        private static void AddAssetToXml(XmlWriter writer, Asset s)
        {
            try
            {
                writer.WriteStartElement("Asset");
                try
                {
                    writer.WriteAttributeString("Name", s.AssetName);
                }
                catch { }

                writer.WriteStartElement("AssetNumber");
                try
                {
                    writer.WriteString(s.AssetNumber);
                }
                catch { }
                writer.WriteEndElement();

                writer.WriteStartElement("Description");

                try
                {
                    if (s.Description.Contains("\n"))
                    {
                        s.Description = s.Description.Replace("\n", " ");
                    }
                    writer.WriteString(s.Description);
                }
                catch { }
                writer.WriteEndElement();

                writer.WriteStartElement("BarcodeImage");
                try
                {
                    writer.WriteString(s.BarcodeImage);
                }
                catch { }
                writer.WriteEndElement();

                writer.WriteStartElement("Images");
                try
                {
                    
                        writer.WriteStartElement("Image");
                        writer.WriteString(s.Images);
                        writer.WriteEndElement();
                    
                }
                catch (Exception ex) { }
                writer.WriteEndElement();

                writer.WriteStartElement("DateRecieved");
                try
                {
                    writer.WriteString(s.DateRecieved.ToString());

                }
                catch { }
                writer.WriteEndElement();

                writer.WriteStartElement("DateShipped");
                try
                {
                    writer.WriteString(s.DateShipped.ToString());
                }
                catch { }
                writer.WriteEndElement();

                writer.WriteStartElement("IsOut");
                try
                {
                    writer.WriteString(s.IsOut.ToString());
                }
                catch { }
                writer.WriteEndElement();

                writer.WriteStartElement("OrderNumber");
                try
                {
                    writer.WriteString(s.OrderNumber.ToString());
                }
                catch { }
                writer.WriteEndElement();

                writer.WriteStartElement("PersonShipping");
                try
                {
                    writer.WriteString(s.PersonShipping);
                }
                catch { }
                writer.WriteEndElement();

                writer.WriteStartElement("ServiceEngineer");
                try
                {
                    writer.WriteString(s.ServiceEngineer);
                }
                catch { }
                writer.WriteEndElement();

                writer.WriteStartElement("ShipTo");
                try
                {
                    writer.WriteString(s.ShipTo);
                }
                catch { }
                writer.WriteEndElement();

                writer.WriteStartElement("Weight");
                try
                {
                    writer.WriteString(s.weight.ToString());
                }
                catch { }
                writer.WriteEndElement();

                writer.WriteStartElement("OnHold");
                try
                {
                    writer.WriteString(s.OnHold.ToString());
                }
                catch { }
                writer.WriteEndElement();

                writer.WriteStartElement("IsDamaged");
                try
                {
                    writer.WriteString(s.IsDamaged.ToString());
                }
                catch { }
                writer.WriteEndElement();


                writer.WriteStartElement("ShippingInformation");

                
                    try
                    {
                        writer.WriteStartElement("PackingSlip");
                        writer.WriteString(s.PackingSlip);
                        writer.WriteEndElement();
                    }
                    catch { }
                    try
                    {
                        writer.WriteStartElement("UPSlabel");
                        writer.WriteString(s.UpsLabel);
                        writer.WriteEndElement();
                    }
                    catch { }
                    try
                    {
                        writer.WriteStartElement("ReturnReport");
                        writer.WriteString(s.ReturnReport);
                        writer.WriteEndElement();
                    }
                    catch { }
                
             
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
            catch (Exception ex)
            {

            }
        }

        public static void AddCookie(string name, string value, int expireAfter=3, bool secure = false, bool shareable=false)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Secure = secure;
            cookie.Shareable = shareable;
            cookie.Expires = DateTime.Now.AddDays(expireAfter);
            cookie.Value = value;

            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        public static HttpCookieCollection GetCookies()
        {
            return HttpContext.Current.Request.Cookies;
        }
        public static HttpCookie GetCookie(string name)
        {
            return HttpContext.Current.Request.Cookies[name];
        }
    }
}
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using Web_App_Master;
using Web_App_Master.Models;
using Microsoft.AspNet.Identity;
using static Web_App_Master.Models.Data_Models;
using Microsoft.AspNet.SignalR;
using static Web_App_Master.App_Start.SignalRHubs;
using Microsoft.AspNet.SignalR.Hubs;
using System.Drawing;

namespace Helpers
{
    public static class Extensions
    {
        //images
        public static System.Drawing.Image Resize(this System.Drawing.Image img, int width, int height)
        {
            System.Drawing.Image _img = new Bitmap(width, height);
            try
            {
                Graphics graphics = Graphics.FromImage(_img);

                graphics.DrawImage(img, 0, 0, width, height);

                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

                graphics.Dispose();

                img.Dispose();
            }
            catch { }

            return _img;
        }

        public static byte[] ToArray(this System.Drawing.Image img, System.Drawing.Imaging.ImageFormat format = null)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                if (format != null)
                {
                    img.Save(stream, format);
                }
                else
                {
                    img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                }
                stream.Close();
                byteArray = stream.ToArray();
            }
            return byteArray;
        }

        public static System.Drawing.Image FromArray(this byte[] bytes)
        {
            System.Drawing.Image image = null;
            try
            {

                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    image = System.Drawing.Image.FromStream(ms);
                }
                return image;
            }
            catch
            {
                return image;
            }
        }

        public static System.Drawing.Image FromArray(this string base64)
        {
            System.Drawing.Image image = null;
            try
            {

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64)))
                {
                    image = System.Drawing.Image.FromStream(ms);
                }
                return image;
            }
            catch
            {
                return image;
            }
        }
        //end images

        public static ClientData FindById(this HashSet<ClientData> data, string id)
        {
            return (from c in ClientHandler.ClientDatas where c.Id == id select c).FirstOrDefault();

        }

        public static string ClientIP(this Page page)
        {
            return HttpContext.Current.Request.UserHostAddress;
        }


        public static IHubContext HubContext<T>(this UserControl control) where T : IHub
        {
            return GlobalHost.ConnectionManager.GetHubContext<T>();
        }

        public static IHubContext HubContext<T>(this Page page) where T : IHub
        {
            return GlobalHost.ConnectionManager.GetHubContext<T>();
        }


        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            return source.Where(element => knownKeys.Add(keySelector(element)));
        }
        public static List<Asset> Update(this List<Asset> source, Asset asset, bool push=false)
        {
            try
            {
                source.ForEach((aa) =>
                {
                    if (aa.AssetNumber == asset.AssetNumber)
                    {
                        aa = asset;
                        if (push)
                            Push.Asset(asset);
                    }
                });
            }
            catch { }
            return source;
        }

        public static bool IsAdmin(this HttpContext context)
        {
            if (context.User.IsInRole("Admins") || context.User.IsInRole("superadmin"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public static bool IsCustomer(this HttpContext context)
        {
            if (context.User.IsInRole("Customers"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public static bool IsUser(this HttpContext context)
        {
            if (context.User.IsInRole("Users"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public static Control RecursiveFindControl(this Control parent, string idToFind)
        {
            foreach (Control child in parent.Controls)
            {
                if (child.ID == idToFind)
                {
                    return child;
                }
                else
                {
                    Control control = RecursiveFindControl(child, idToFind);
                    if (control != null)
                    {
                        return control;
                    }
                }
            }
            return null;
        }
        public static Control FindRoleControl(this LoginView lv, string controlName, string role, string parentControl = null)
        {
            int idx = 0;
            foreach (RoleGroup r in lv.RoleGroups)
            {
                if (r.Roles.Contains(role))
                {
                    break;
                }
                ++idx;
            }
            Control c = null;
            ITemplate template = lv.RoleGroups[0].ContentTemplate;
            if (template != null)
            {
                Control container = new Control();
                template.InstantiateIn(container);

                if (parentControl != null)
                {
                    var parent = container.RecursiveFindControl(parentControl);
                    c = parent.RecursiveFindControl(controlName);
                }
                else
                {
                    c = container.RecursiveFindControl(controlName);
                }

            }
            return c;
        }
        public static void Shake(this HtmlGenericControl control, string animation = " mif-ani-flash")
        {
            var a = control.Attributes["class"];
            a += animation;
            control.Attributes["class"] = a;
        }
        public static void Quiet(this HtmlGenericControl control, string animation = " mif-ani-flash")
        {
            var a = control.Attributes["class"];
            a = a.Replace(animation, "");
            control.Attributes["class"] = a;
        }
        public static IEnumerable<List<T>> SplitList<T>(this List<T> list, int nSize = 30)
        {
            if (list.Count <= 30)
            { return new List<List<T>>() { list }; }
            try
            {
                var tmp = new T[list.Count];
                list.CopyTo(tmp);
                var temp = new List<T>(tmp);
                var ret = new List<List<T>>();
                int count = 0;
                do
                {
                    var page = temp.Take(nSize).ToList();
                    foreach (var item in page)
                    {
                        temp.Remove(item);
                    }
                    ret.Add(page);
                    count += page.Count;
                } while (count < list.Count);
                return ret;
            }
            catch {
                return new List<List<T>>();
            }
        }
        public static SiteMaster SiteMaster(this Page page)
        {
            return page.Master as SiteMaster;
        }
        public static void UpdateAll(this Page page)
        {
            (page.Master as SiteMaster).UpdateAllPanels();
        }

        public static List<Asset> ImportXmlLibraryFile(string filename)
        {

            var doc = new XmlDocument();
            using (StreamReader reader = new StreamReader(filename))
            {
                doc.LoadXml(reader.ReadToEnd());
            }


            Global.AssetCache = new List<Asset>();

            XmlNodeList elemList = doc.GetElementsByTagName("Asset");
            foreach (XmlElement asset in elemList)
            {
                Asset a = new Asset();
                try
                {

                    a.AssetName = asset.GetAttribute("Name").Sanitize();
                }
                catch { }
                try
                {
                    a.AssetNumber = asset.SelectSingleNode("AssetNumber").InnerText.Sanitize();
                }
                catch { }
                try
                {
                    a.DateRecieved = DateTime.Parse(asset.SelectSingleNode("DateRecieved").InnerText.Sanitize());
                }
                catch { }
                try
                {
                    a.DateShipped = DateTime.Parse(asset.SelectSingleNode("DateShipped").InnerText.Sanitize());
                }
                catch { }
                try
                {
                    a.Description = asset.SelectSingleNode("Description").InnerText.Sanitize();
                }
                catch { }
                try
                {
                    a.OrderNumber = asset.SelectSingleNode("OrderNumber").InnerText.Sanitize();
                }
                catch { }
                try
                {
                    a.PackingSlip = asset.SelectSingleNode("PackingSlip").InnerText.Sanitize();
                }
                catch { }
                try
                {
                    a.PersonShipping = asset.SelectSingleNode("PersonShipping").InnerText.Sanitize();
                }
                catch { }
                try
                {
                    a.ReturnReport = asset.SelectSingleNode("ReturnReport").InnerText.Sanitize();
                }
                catch { }
                try
                {
                    a.ServiceEngineer = asset.SelectSingleNode("ServiceEngineer").InnerText.Sanitize();
                }
                catch { }
                try
                {
                    a.ShipTo = asset.SelectSingleNode("ShipTo").InnerText.Sanitize();
                }
                catch { }
                try
                {
                    a.UpsLabel = asset.SelectSingleNode("UpsLabel").InnerText.Sanitize();
                }
                catch { }
                try
                {
                    a.weight = Convert.ToDecimal(asset.SelectSingleNode("Weight").InnerText.Sanitize());
                }
                catch { }
                try
                {
                    a.IsOut = Convert.ToBoolean(asset.SelectSingleNode("IsOut").InnerText.Sanitize());
                }
                catch { }
                try
                {
                    a.OnHold = Convert.ToBoolean(asset.SelectSingleNode("OnHold").InnerText.Sanitize());
                }
                catch { }
                try
                {
                    a.IsDamaged = Convert.ToBoolean(asset.SelectSingleNode("IsDamaged").InnerText.Sanitize());
                }
                catch { }


                try
                {
                    XmlNodeList imglist = asset.GetElementsByTagName("Image");

                    foreach (XmlElement el in imglist)
                    {
                        a.Images += "" + el.InnerText + ",";
                    }
                }
                catch { }

                Global.AssetCache.Add(a);

            }

            return Global.AssetCache;
        }
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }

        }

        public static string Sanitize(this string str)
        {
            if (str == null)
                str = "";
            return str.Replace("\"", "").Replace("'", "");
        }
        public static string SanitizeHTML(this string str)
        {
            if (str == null)
                str = "";
            return str.Replace("<", "").Replace(">", "").Replace("*", "").Replace(":", "").Replace("|", "");
        }
        public static Asset FindAssetByNumber(this List<Asset> assets, string assetNumber)
        {
            return (from x in assets
                    where x.AssetNumber == assetNumber
                    select x).FirstOrDefault();
        }
        public static Asset FindAssetByName(this List<Asset> assets, string assetName)
        {
            return (from x in assets
                    where x.AssetName == assetName
                    select x).FirstOrDefault();
        }
        public static List<Asset> FindAssetsByNumber(this List<Asset> assets, string assetNumber)
        {
            return (from x in assets
                    where x.AssetNumber == assetNumber
                    select x).ToList();
        }

        public static RoleBindinglist ToRoleBindingList(this IEnumerable<IdentityRole> list)
        {
            return new RoleBindinglist(list.ToList());
        }
        public static UserBindinglist ToUserBindingList(this IEnumerable<ApplicationUser> list)
        {
            return new UserBindinglist(list.ToList());
        }
        public static AssetNoticeBindinglist ToNoticeBindingList(this IEnumerable<AssetNotification> list)
        {
            return new AssetNoticeBindinglist(list.ToList());
        }
        public static Asset ToAsset(this XmlElement asset)
        {
            Asset a = new Asset();
            try
            {

                a.AssetName = asset.GetAttribute("Name").Sanitize();
            }
            catch { }
            try
            {
                a.AssetNumber = asset.SelectSingleNode("AssetNumber").InnerText.Sanitize();
            }
            catch { }
            try
            {
                a.DateRecieved = DateTime.Parse(asset.SelectSingleNode("DateRecieved").InnerText.Sanitize());
            }
            catch { }
            try
            {
                a.DateShipped = DateTime.Parse(asset.SelectSingleNode("DateShipped").InnerText.Sanitize());
            }
            catch { }
            try
            {
                a.Description = asset.SelectSingleNode("Description").InnerText.Sanitize();
            }
            catch { }
            try
            {
                a.OrderNumber = asset.SelectSingleNode("OrderNumber").InnerText.Sanitize();
            }
            catch { }
            try
            {
                a.PackingSlip = asset.SelectSingleNode("PackingSlip").InnerText.Sanitize();
            }
            catch { }
            try
            {
                a.PersonShipping = asset.SelectSingleNode("PersonShipping").InnerText.Sanitize();
            }
            catch { }
            try
            {
                a.ReturnReport = asset.SelectSingleNode("ReturnReport").InnerText.Sanitize();
            }
            catch { }
            try
            {
                a.ServiceEngineer = asset.SelectSingleNode("ServiceEngineer").InnerText.Sanitize();
            }
            catch { }
            try
            {
                a.ShipTo = asset.SelectSingleNode("ShipTo").InnerText.Sanitize();
            }
            catch { }
            try
            {
                a.UpsLabel = asset.SelectSingleNode("UpsLabel").InnerText.Sanitize();
            }
            catch { }
            try
            {
                a.weight = Convert.ToDecimal(asset.SelectSingleNode("Weight").InnerText.Sanitize());
            }
            catch { }
            try
            {
                a.IsOut = Convert.ToBoolean(asset.SelectSingleNode("IsOut").InnerText.Sanitize());
            }
            catch { }
            try
            {
                a.OnHold = Convert.ToBoolean(asset.SelectSingleNode("OnHold").InnerText.Sanitize());
            }
            catch { }
            try
            {
                a.IsDamaged = Convert.ToBoolean(asset.SelectSingleNode("IsDamaged").InnerText.Sanitize());
            }
            catch { }
            try
            {
                XmlNodeList imglist = asset.GetElementsByTagName("Image");

                foreach (XmlElement el in imglist)
                {
                    a.Images += "" + el.InnerText + ",";
                }
            }
            catch { }
            return a;
        }
        public static AssetNotification ToNotification(this XmlElement asset)
        {
            AssetNotification a = new AssetNotification();
            try
            {
                a.AssetNumber = asset.GetAttribute("AssetNumber").Sanitize();
                a.IsNotified = Convert.ToBoolean(asset.GetAttribute("IsNotified").Sanitize());
                a.LastNotified = DateTime.Parse(asset.GetAttribute("LastNotified").Sanitize());
                a.Time = DateTime.Parse(asset.GetAttribute("Time").Sanitize());
                var n30 = asset.GetAttribute("Is30day").Sanitize();
                var n15 = asset.GetAttribute("Is15Day").Sanitize();
                a.Is30Day = Convert.ToBoolean(n30);
                a.Is15Day = Convert.ToBoolean(n15);
                XmlNodeList elemList = asset.GetElementsByTagName("Email");
                foreach (XmlElement elem in elemList)
                {
                    EmailAddress em = new EmailAddress();
                    em.Email = asset.GetAttribute("Email").Sanitize();
                    em.Name = asset.GetAttribute("Name").Sanitize();
                    a.Emails.Add(em);
                }
            }
            catch { }


            return a;
        }
        public static CustomerBindinglist ToCustomerBindingList(this IEnumerable<Customer> list)
        {
            return new CustomerBindinglist(list.ToList());

        }

        public static List<Asset> RemoveDuplicateAssets(this List<Asset> list)
        {
            list = list.GroupBy(x => x.AssetNumber).Select(x => x.First()).ToList();
            return list;
        }


        //public static IEnumerable<IGrouping<TKey, TSource>> RemoveDupliactes<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        //{
        //    return new IEnumerable<IGrouping<TKey, TSource>>();
        //}

        //LOGGING
        public static bool AddToLog(this HttpContext context, string msg)
        {
            try
            {
                var manager = context.GetOwinContext().GetUserManager<ApplicationUserManager>();               
                // Require the user to have a confirmed email before they can log on.
                var user = manager.FindByName(context.User.Identity.Name);
                var UserData = context.Session["SessionUserData"] as Data.UserData;
                UserData.Guid = user.Id;


                UserData.Email = user.Email;
                UserData.Name = user.UserName;
                UserData.Log.Add(new Data.LogEntry(msg));               
                try
                {
                    var db = Web_App_Master.Pull.Setting(user.Id);
                    if (db != null)
                    {
                        Data.UserData tt;
                        context.Session["PersistingUserData"] = tt = new Data.UserData().DeserializeFromXmlString<Data.UserData>(db.XmlData) as Data.UserData;
                        tt.Log.Add(new Data.LogEntry(msg));
                        

                        Helpers.SettingsDBData d = new Helpers.SettingsDBData();
                        d.Appname = user.Id;
                        d.XmlData = UserData.SerializeToXmlString(tt);
                        Push.Setting(d);
                    }
                    else
                    {
                        var nn = new Data.UserData();
                        nn.Guid = user.Id;
                        nn.Email = user.Email;
                        nn.Name = user.UserName;
                        nn.Log.Add(new Data.LogEntry(msg));
                        context.Session["PersistingUserData"] = nn;
                        Helpers.SettingsDBData d = new Helpers.SettingsDBData();
                        d.Appname = user.Id;
                        d.XmlData = UserData.SerializeToXmlString(nn);
                        Push.Setting(d);
                    }
                }
                catch
                {
                    try
                    {
                        var u = UserData.Clone() as Data.UserData;
                        u.Log.Add(new Data.LogEntry(msg));
                        context.Session["PersistingUserData"] = u;

                        Helpers.SettingsDBData db = new Helpers.SettingsDBData();
                        db.Appname = user.Id;
                        db.XmlData = UserData.SerializeToXmlString(u);
                        Push.Setting(db);
                    }
                    catch { }
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        public static void AddUserNotice(this Page page, string notice, string xml=null)
        {
           var list =  page.Session["Notifications"] as List<MenuAlert>;
            if (list==null)
            {
                list = new List<MenuAlert>();
            }           
            MenuAlert n = new MenuAlert();
            n.Name = page.User.Identity.Name;
            n.Text = notice;
            if (xml !=null)
            {
                n.XmlData = xml;
            }
            list.Add(n);
            page.Session["Notifications"] = list;
        }

        public static string Terminate(this string str, string end = "/")
        {
            if (!str.EndsWith(end)) str += "/";
            return str;
        }
        public static string Map(this string input)
        {
            return HttpContext.Current.Server.MapPath(input);
        }
        /// <summary>
        /// Split string using string instead of Character
        /// </summary>
        /// <param name="input"></param>
        /// <param name="splitString"> Deliminator Value</param>
        /// <returns></returns>
        public static string[] StringSplit(this string input, string deliminator, bool removeBlanks=true)
        {
            if (removeBlanks)
                return input.Split(new string[] {deliminator }, StringSplitOptions.RemoveEmptyEntries);
            else
                return input.Split(new string[] { deliminator }, StringSplitOptions.None);
        }
    }

}
namespace System.Collections.Generic
{
    public class SizedList<T> : ConcurrentQueue<T>
    {
        private readonly object syncObject = new object();

        public int Size { get; private set; }

        public SizedList(int size)
        {
            Size = size;
        }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            lock (syncObject)
            {
                while (base.Count > Size)
                {
                    T outObj;
                    base.TryDequeue(out outObj);
                }
            }
        }
    }
}
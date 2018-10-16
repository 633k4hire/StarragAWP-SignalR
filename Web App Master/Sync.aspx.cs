using Sync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master
{
    public partial class Sync : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            


            if (PushUpload.HasFile)
            {
                string path ="/Upload/Temp/"+ Guid.NewGuid().ToString()+".xml";
                PushUpload.PostedFile.SaveAs(path);
                var ds = new Helpers.DataStore().DeserializeFromXmlFile<Helpers.DataStore>(path);
                File.Delete(path);
                Global.Library.Assets = ds.Assets;
                Web_App_Master.Push.Library();
            }
            try
            {
                if (Request.PathInfo=="/Pull" || Request.PathInfo=="/Push")
                {                  
              
                    var stream = this.Request.InputStream;
                    byte[] bytes = new byte[] { };
                    stream.Position = 0;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        ms.Position = 0;
                        bytes = ms.ToArray();
                    }
                    var xml = Encoding.UTF8.GetString(bytes);

                    if (Request.PathInfo == "/Pull")
                    {
                        Pull(xml);
                    }
                    if (Request.PathInfo == "/Push")
                    {
                        Push(xml);
                    }



                  

                }


            }
            catch {

            }

            


        }
      
        public  void Push(string xml)
        {
            var query = Request.QueryString["t"];
            
            var blank = XmlHelper.BlankXml("Error");
            if (query != null)
            { 
                try
                {
                    switch (query)
                    {
                        case "asset":
                            var asset = new Helpers.Asset().DeserializeFromXmlString<Helpers.Asset>(xml);                           
                            Web_App_Master.Push.Asset(asset); 
                            
                            break;
                        case "notice":
                            var notice = new Notification.NotificationSystem.Notice().DeserializeFromXmlString<Notification.NotificationSystem.Notice>(xml);
                            Web_App_Master.Push.Notification(notice);
                            break;
                        case "settings":
                            var settings = new Helpers.Settings().DeserializeFromXmlString<Helpers.Settings>(xml);
                            Global.Library.Settings = settings;
                            Web_App_Master.Push.LibrarySettings();

                            break;
                        case "datastore":
                            var aa = 0;
                            var ds = new Helpers.DataStore().DeserializeFromXmlString<Helpers.DataStore>(xml);
                            Global.Library.Assets = ds.Assets;
                            Web_App_Master.Push.Library();
                            break;
                        default:
                            break;
                    }

                   



                    blank = XmlHelper.BlankXml("Success");
                }
                catch { }
            }
            var utf8 = Encoding.UTF8.GetBytes(blank);
            try
            {
                Response.ClearContent();

                Response.ClearHeaders();

                Response.AddHeader("Content-Disposition", "inline; filename=" + "update.xml");

                Response.ContentType = "text/xml";

                Response.BinaryWrite(utf8);

                Response.Flush(); // Sends all currently buffered output to the client.
                Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
                HttpContext.Current.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.

            }
            catch
            {
            }

        }
        
        public  void Pull(string xml)
        {try
            {
  
                var ret = Global.Library.SerializeToXmlString(Global.Library);
            var utf8 = Encoding.UTF8.GetBytes(ret);
            
                Response.ClearContent();

                Response.ClearHeaders();

                Response.AddHeader("Content-Disposition", "inline; filename=" + "update.xml");

                Response.ContentType = "text/xml";

                Response.BinaryWrite(utf8);

                Response.Flush(); // Sends all currently buffered output to the client.
                Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
                HttpContext.Current.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.

            }
            catch
            {
            }
        }
    }
    
}
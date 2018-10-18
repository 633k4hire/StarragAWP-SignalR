using Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Web_App_Master;

namespace Web_App_Master
{
    public  class EmailHelper
    {
        public static bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public static bool IsValid(EmailAddress emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress.Email);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public static Task<bool> SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(SendEmail(message.Destination,message.Body,message.Subject));
        }
        public static  bool SendEmail(string email,string body = "", string subject = "")
        {
            

                MailMessage msg = new MailMessage();
                msg.Subject = subject;
                msg.IsBodyHtml = true;
                msg.BodyEncoding = Encoding.ASCII;
                msg.Body = body;
                msg.From = new MailAddress(Global.Library.Settings.SenderEmail);
                msg.To.Add(email);
                string username = Global.Library.Settings.SenderEmail;  //email address or domain user for exchange authentication
                string password = Global.Library.Settings.SenderPassword;  //password
                SmtpClient mClient = new SmtpClient();
                mClient.Host = Global.Library.Settings.SenderHost;
                mClient.Port = Global.Library.Settings.SenderPort;
                mClient.UseDefaultCredentials = false;
                mClient.Credentials = new NetworkCredential(username, password);
                mClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                mClient.Timeout = 100000;
                mClient.EnableSsl = true;
                try
                {
                    mClient.Send(msg);
                return true;
                }
                catch (Exception ex)
                {
                Push.Alert("Email Fail:" + ex.Message);
                return false;
                }
          
        }
        public static void SendEmailAsync(string email,string body = "", string subject = "")
        {


            new System.Threading.Thread(() =>
            {

                MailMessage msg = new MailMessage();
                msg.Subject = subject;
                msg.IsBodyHtml = true;
                msg.BodyEncoding = Encoding.ASCII;
                msg.Body = body;
                msg.From = new MailAddress("provider.service.secure@gmail.com");
                msg.To.Add(email);
                string username = "provider.service.secure@gmail.com";  //email address or domain user for exchange authentication
                string password = "@Service1";  //password
                SmtpClient mClient = new SmtpClient();
                mClient.Host = "smtp.gmail.com";
                mClient.Port = 587;
                mClient.UseDefaultCredentials = false;
                mClient.Credentials = new NetworkCredential(username, password);
                mClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                mClient.Timeout = 100000;
                mClient.EnableSsl = true;
                try
                {
                    mClient.Send(msg);
                }
                catch (Exception ex)
                {
                    Push.Alert("Email Fail:" + ex.Message);
                }
            }).Start();
        }
        public static void SendMassEmail(string[] emails, string body = "", string subject = "")
        {


            
                MailMessage msg = new MailMessage();
                msg.Subject = subject;
                msg.IsBodyHtml = true;
                msg.BodyEncoding = Encoding.ASCII;
                msg.Body = body;
                msg.From = new MailAddress("provider.service.secure@gmail.com");
                foreach (var email in emails)
                {
                    msg.To.Add(email);
                }
                string username = "provider.service.secure@gmail.com";  //email address or domain user for exchange authentication
                string password = "@Service1";  //password
                SmtpClient mClient = new SmtpClient();
                mClient.Host = "smtp.gmail.com";
                mClient.Port = 587;
                mClient.UseDefaultCredentials = false;
                mClient.Credentials = new NetworkCredential(username, password);
                mClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                mClient.Timeout = 100000;
                mClient.EnableSsl = true;
                try
                {
                    mClient.Send(msg);
                }
                catch (Exception ex)
                {
                Push.Alert("Email Fail:" + ex.Message);
            }
         
        }
        public static void SendMassEmailAsync(string[] emails, string body = "", string subject = "")
        {


            new System.Threading.Thread(() =>
            {

                MailMessage msg = new MailMessage();
                msg.Subject = subject;
                msg.IsBodyHtml = true;
                msg.BodyEncoding = Encoding.ASCII;
                msg.Body = body;
                msg.From = new MailAddress("provider.service.secure@gmail.com");
                foreach(var email in emails )
                {
                    msg.To.Add(email);
                }                
                string username = "provider.service.secure@gmail.com";  //email address or domain user for exchange authentication
                string password = "@Service1";  //password
                SmtpClient mClient = new SmtpClient();
                mClient.Host = "smtp.gmail.com";
                mClient.Port = 587;
                mClient.UseDefaultCredentials = false;
                mClient.Credentials = new NetworkCredential(username, password);
                mClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                mClient.Timeout = 100000;
                mClient.EnableSsl = true;
                try
                {
                    mClient.Send(msg);
                }
                catch (Exception ex)
                {
                    Push.Alert("Email Fail:" + ex.Message);
                }
            }).Start();
        }
        public static bool SendCheckOutNotice(List<Asset> assets, string Body = "")
        {
            try
            {
                var engineer = (from d in Global.Library.Settings.ServiceEngineers where d.Name == assets[0].ServiceEngineer select d).FirstOrDefault();
                var statics = (from d in Global.Library.Settings.StaticEmails select d.Email).ToList();


                var emaillist = new List<string>();
                //emaillist.Add(shipper.Email);
                emaillist.Add(engineer.Email);
                emaillist.AddRange(statics);



                Body = Body.Replace("<name>", assets[0].ServiceEngineer);
                var serviceToolString = "";
                foreach (var item in assets)
                {
                    serviceToolString += "(" + item.AssetName + ")";
                }
                Body = Body.Replace("<customer>", assets[0].ShipTo);
                Body = Body.Replace("<serviceTool>", serviceToolString);
                Body = Body.Replace("<serviceOrder>", assets[0].OrderNumber.ToString());
                Body = Body.Replace("<dateAssigned>", assets[0].DateShipped.ToString());
                Body = Body.Replace("<NL>", "<br />");
                MailMessage msg = new MailMessage();
                msg.Subject = "Asset Alert:: " + assets[0].AssetName + " :: " + DateTime.Now.ToString();
                msg.IsBodyHtml = true;
                msg.BodyEncoding = Encoding.ASCII;
                msg.Body = Body;
                msg.From = new MailAddress("provider.service.secure@gmail.com");
                foreach (var email in emaillist)
                {
                    msg.To.Add(email);
                }
                string username = "provider.service.secure@gmail.com";  //email address or domain user for exchange authentication
                string password = "@Service1";  //password
                SmtpClient mClient = new SmtpClient();
                mClient.Host = "smtp.gmail.com";
                mClient.Port = 587;
                mClient.UseDefaultCredentials = false;
                mClient.Credentials = new NetworkCredential(username, password);
                mClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                mClient.Timeout = 100000;
                mClient.EnableSsl = true;
                try
                {
                    mClient.Send(msg);
                }
                catch (Exception ex)
                {
                    Push.Alert("Email Fail:" + ex.Message);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Push.Alert("Email Fail:" + ex.Message);
                return false;
            }
        }

        public static void SendCheckOutNoticeAsync(List<Asset> assets, string Body = "")
        {
            var engineer = (from d in Global.Library.Settings.ServiceEngineers where d.Name == assets[0].ServiceEngineer select d).FirstOrDefault();
            var statics = (from d in Global.Library.Settings.StaticEmails  select d.Email).ToList();
       

            var emaillist = new List<string>();
            //emaillist.Add(shipper.Email);
            emaillist.Add(engineer.Email);
            emaillist.AddRange(statics);
            
            new System.Threading.Thread(() =>
            {

                Body = Body.Replace("<name>", assets[0].ServiceEngineer);
                var serviceToolString = "";
                foreach(var item in assets)
                {
                    serviceToolString +="("+ item.AssetName + ")";
                }
                Body = Body.Replace("<customer>", assets[0].ShipTo);
                Body = Body.Replace("<serviceTool>", serviceToolString);
                Body = Body.Replace("<serviceOrder>", assets[0].OrderNumber.ToString());
                Body = Body.Replace("<dateAssigned>", assets[0].DateShipped.ToString());
                Body = Body.Replace("<NL>", "<br />");
                MailMessage msg = new MailMessage();
                msg.Subject = "Asset Alert:: " + assets[0].AssetName + " :: " + DateTime.Now.ToString();
                msg.IsBodyHtml = true;
                msg.BodyEncoding = Encoding.ASCII;
                msg.Body = Body;
                msg.From = new MailAddress("provider.service.secure@gmail.com");
                foreach (var email in emaillist)
                {
                    msg.To.Add(email);
                }
                string username = "provider.service.secure@gmail.com";  //email address or domain user for exchange authentication
                string password = "@Service1";  //password
                SmtpClient mClient = new SmtpClient();
                mClient.Host = "smtp.gmail.com";
                mClient.Port = 587;
                mClient.UseDefaultCredentials = false;
                mClient.Credentials = new NetworkCredential(username, password);
                mClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                mClient.Timeout = 100000;
                mClient.EnableSsl = true;
                try
                {
                    mClient.Send(msg);
                }
                catch (Exception ex)
                {
                    Push.Alert("Email Fail:"+ex.Message);
                }
            }).Start();
        }
        public static void SendNotificationSystemNotice(Notification.NotificationSystem.Notice notice)
        {
            if (notice.NoticeType== Notification.NotificationSystem.NoticeType.Checkout)
            {
                if (Global.Library.Settings.TESTMODE) return;
                if (notice is EmailNotice )
                {
                    var en = notice as EmailNotice;
                    string Body = en.Body;
                    new System.Threading.Thread(() =>
                    {

                        Body = Body.Replace("<name>", en.EmailAddress.Name);
                        var serviceToolString = " < br />";
                        foreach (var item in en.Assets)
                        {
                            serviceToolString += "("+ item + ")";
                        }

                        Body = Body.Replace("<serviceTool>", serviceToolString);
                        Body = Body.Replace("<serviceOrder>", en.NoticeControlNumber);
                        Body = Body.Replace("<dateAssigned>", en.Created.ToShortDateString());
                        Body = Body.Replace("<NL>", "<br />");
                        MailMessage msg = new MailMessage();
                        msg.Subject = "Return Request:: Job# " + en.NoticeControlNumber + " :: " + DateTime.Now.ToString();
                        msg.IsBodyHtml = true;
                        msg.BodyEncoding = Encoding.ASCII;
                        msg.Body = Body;
                        msg.From = new MailAddress(Global.Library.Settings.SenderEmail);
                        foreach (var email in en.Emails)
                        {
                            msg.To.Add(email.Email);
                        }
                        string username = Global.Library.Settings.SenderEmail;  //email address or domain user for exchange authentication
                        string password = Global.Library.Settings.SenderPassword;  //password
                        SmtpClient mClient = new SmtpClient();
                        mClient.Host = Global.Library.Settings.SenderHost;
                        mClient.Port = Global.Library.Settings.SenderPort;
                        mClient.UseDefaultCredentials = false;
                        mClient.Credentials = new NetworkCredential(username, password);
                        mClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        mClient.Timeout = 100000;
                        mClient.EnableSsl = true;
                        try
                        {
                            mClient.Send(msg);
                        }
                        catch (Exception ex)
                        {
                            Push.Alert("Email Fail:" + ex.Message);
                        }
                    }).Start();
                }
               
            }                      

        }
    }
}



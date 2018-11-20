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
        //Helpers
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

        //GMAIL
        public static  bool SendGmail(string email,string body = "", string subject = "")
        {
            try
            {
                GmailApi.GmailHelper gh = new GmailApi.GmailHelper(GmailApi.GmailHelper.Credential64);
                gh.Send(new string[] { email }, subject, body);
                gh.Dispose();
                return true;
            }
            catch (Exception)
            {

                return false;
            }          
        }

        public static Task<bool> SendGmailAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(SendGmail(message.Destination, message.Body, message.Subject));
        }

        //SendGrid
        public static bool SendGridEmail(string email, string body = "", string subject = "")
        {
            return SendGridApi.Send(new string[] { email }, subject, body);
        }
        public static bool SendGridEmail(string[] emails, string body = "", string subject = "")
        {
            return SendGridApi.Send(emails , subject, body);
        }
        public static Task<bool> SendGridEmailAsync(IdentityMessage message)
        {
            return Task.FromResult(SendGridEmail(message.Destination, message.Body, message.Subject));
        }public static Task<bool> SendGridEmailAsync(string[] emails, string body = "", string subject = "")
        {
            return Task.FromResult(SendGridEmail(emails, body, subject));
        }

        public static bool SendMassGmail(string[] emails, string body = "", string subject = "")
        {
            try
            {
                GmailApi.GmailHelper gh = new GmailApi.GmailHelper(GmailApi.GmailHelper.Credential64);
                gh.Send(emails, subject, body);
                gh.Dispose();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public static Task<bool> SendMassGmailAsync(string[] emails, string body = "", string subject = "")
        {
            return Task.FromResult(SendMassGmail(emails, body, subject));
        }

        public static bool SendCheckOutNotice(List<Asset> assets, string Body = "")
        {
            try
            {
                var engineer = (from d in Global.Library.Settings.ServiceEngineers where d.Name == assets[0].ServiceEngineer select d).FirstOrDefault();

                var statics = (from d in Global.Library.Settings.StaticEmails select d.Email).ToList();
                
                var emaillist = new List<string>();
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
                                
                var Subject = "Asset Alert:: " + assets[0].AssetName + " :: " + DateTime.Now.ToString();

                return SendGridEmail(emaillist.ToArray(), Body, Subject);
                
            }
            catch (Exception ex)
            {
                Push.Alert("Email Fail:" + ex.Message);
                return false;
            }
        }

        public static Task<bool> SendCheckOutNoticeAsync(List<Asset> assets, string Body = "")
        {
            return Task.FromResult(SendCheckOutNotice(assets, Body));
        }

        public static bool SendNotificationSystemNotice(Notification.NotificationSystem.Notice notice)
        {
            try
            {


                if (notice.NoticeType == Notification.NotificationSystem.NoticeType.Checkout)
                {
                    if (Global.Library.Settings.TESTMODE) return false;
                    if (notice is EmailNotice)
                    {
                        var en = notice as EmailNotice;
                        string Body = en.Body;


                        Body = Body.Replace("<name>", en.EmailAddress.Name);
                        var serviceToolString = " < br />";
                        foreach (var item in en.Assets)
                        {
                            serviceToolString += "(" + item + ")";
                        }

                        Body = Body.Replace("<serviceTool>", serviceToolString);
                        Body = Body.Replace("<customer>", en.Data);
                        Body = Body.Replace("<serviceOrder>", en.NoticeControlNumber);
                        Body = Body.Replace("<dateAssigned>", en.Created.ToShortDateString());
                        Body = Body.Replace("<NL>", "<br />");


                        var Subject = "Return Request:: Job# " + en.NoticeControlNumber + " :: " + DateTime.Now.ToString();

                        List<String> emails = new List<string>();
                        foreach (var email in en.Emails)
                        {
                            emails.Add(email.Email);
                        }

                        return SendGridEmail(emails.ToArray(), Body, Subject);
                    }

                }

                return false;
            }
            catch (Exception ex)
            {
                Push.Alert("Email Fail:" + ex.Message);
                return false;
            }

        }

        public static Task<bool> SendNotificationSystemNoticeAsync(Notification.NotificationSystem.Notice notice)
        {
            return Task.FromResult(SendNotificationSystemNotice(notice));
        }

        



    }
}



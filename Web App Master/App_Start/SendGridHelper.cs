using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Web_App_Master
{
    public class SendGridApi
    {
        private static readonly string _ApiKey = "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";

        public SendGridApi()
        {
            
        }

        public static async Task<Response> SendAsync(string[] recips, string subject, string htmlBody)
        {
            try
            {
                var msg = new SendGridMessage();

                msg.SetFrom(new EmailAddress("Support@Starrag-Awp.com", "Starrag Support Team"));

                var recipients = new List<EmailAddress>();
                foreach (var recip in recips)
                {
                    recipients.Add(new EmailAddress(recip));
                }
                msg.AddTos(recipients);

                msg.SetSubject(subject);

                //msg.AddContent(MimeType.Text, "Hello World plain text!");
                msg.AddContent(MimeType.Html, htmlBody);


                var client = new SendGridClient(_ApiKey);
                var aa = client.UrlPath;
                return await client.SendEmailAsync(msg);
                
                
            }
            catch 
            {
                return null;
            }
           
            
        }

        public static bool Send(string[] recips, string subject, string htmlBody)
        {
            try
            {
                var msg = new SendGridMessage();

                msg.SetFrom(new EmailAddress("Support@Starrag-Awp.com", "Starrag Support Team"));

                var recipients = new List<EmailAddress>();
                foreach (var recip in recips)
                {
                    recipients.Add(new EmailAddress(recip));
                }
                msg.AddTos(recipients);

                msg.SetSubject(subject);

                //msg.AddContent(MimeType.Text, "Hello World plain text!");
                msg.AddContent(MimeType.Html, htmlBody);

                
                var client = new SendGridClient(_ApiKey);
                var aa = client.UrlPath;
                var response = Task.FromResult(client.SendEmailAsync(msg));
                response.Wait();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

    }
}
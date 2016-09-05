using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailSenderLibrary
{
    public class SendEmail
    {

        public static bool Send(EmailConfiguration config)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                MailMessage mail = new MailMessage();

                client.Port = config.Port;
                client.Host = config.Host;
                client.Credentials = new NetworkCredential(config.UserName, config.Password);

                mail.From = new MailAddress(config.From);
                mail.To.Add(new MailAddress(config.To));
                mail.Subject = config.Subject;
                mail.Body = config.Body;

                if (config.IsHtmlText == true)
                {
                    string tempName = "D:/Robin/Sample Projects/EmailSendingTest/EmailSendingTest/myPage.html";
                    StreamReader sReader = new StreamReader(tempName);
                    string htmlContent = sReader.ReadToEnd();
                    htmlContent = htmlContent.Replace("[myName]", "Robin Halder");
                    mail.Body = htmlContent;

                    AlternateView htv = AlternateView.CreateAlternateViewFromString(mail.Body, null, "text/html");

                    //LinkedResource pic1 = new LinkedResource("D:/Robin/Sample Projects/EmailSendingTest/EmailSendingTest/Images/a.jpg");
                    //pic1.ContentId = "pic1";
                    //htv.LinkedResources.Add(pic1);
                    foreach (var item in config.EmbeddedResources)
                    {
                        htv.LinkedResources.Add(item);
                    }
                    mail.AlternateViews.Add(htv);
                }

                client.Send(mail);
                Console.WriteLine("Done!");
                Console.ReadLine();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
            return false;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {  
            //EmailConfiguration configEmail = new EmailConfiguration();

            //configEmail.From = "robin.halder@enosisbd.com";
            //configEmail.To = "halder.cse.kuet@hotmail.com";
            //configEmail.UserName = "robin.halder@enosisbd.com";
            //configEmail.Password = "Rhalder123";
            //configEmail.Subject = "Another test 3 subject";
            //configEmail.Body = "Test completed";
            //configEmail.Port = 587;
            //configEmail.Host = "smtp.enosisbd.com";
            //configEmail.IsHtmlText = true;

            //configEmail.EmbeddedResources = new List<LinkedResource>();

            //var embeddedResource = new LinkedResource("D:/Robin/Sample Projects/EmailSendingTest/EmailSendingTest/Images/a.jpg");
            //embeddedResource.ContentId = "pic1";
            //configEmail.EmbeddedResources.Add(embeddedResource);


            //SendEmail.Send(configEmail);

             Dictionary<string,string> dic = new Dictionary<string,string>();

             dic.Add("a",null);
             dic.Add("b", "r");

             foreach (var item in dic)
             {
                 Console.WriteLine(item.Value);
             }
             Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using System.Configuration;

namespace EmailSendingTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "smtp.enosisbd.com";

                string tempName = "D:/Robin/Sample Projects/EmailSendingTest/EmailSendingTest/myPage.html";
                StreamReader sReader = new StreamReader(tempName);

                string htmlContent = sReader.ReadToEnd();
                htmlContent = htmlContent.Replace("[myName]","Robin Halder");


                
                client.Credentials = new NetworkCredential("robin.halder@enosisbd.com", "Rhalder123");
                MailMessage mm = new MailMessage("robin.halder@enosisbd.com", "otheruse2mine@gmail.com", "test", htmlContent);

                AlternateView htv = AlternateView.CreateAlternateViewFromString(mm.Body, null, "text/html");
                mm.AlternateViews.Add(htv);
                client.Send(mm);

                MessageBox.Show("Sent sucessfully!"); 

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

       
    }
}

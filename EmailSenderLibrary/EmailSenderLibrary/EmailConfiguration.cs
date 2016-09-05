using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailSenderLibrary
{
    public class EmailConfiguration 
    {
        public string To { get; set; }
        public string From { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string HtmlLocation { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
        public bool IsHtmlText { get; set; }
        public List<LinkedResource> EmbeddedResources { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class User
    {
        public int UserID { get; set; }
        public int ClientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public string Type { get; set; }
    }
}
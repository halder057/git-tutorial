using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiNotificationCommon.Utilities;

namespace ZhiNotificationCommon.Models
{
    public class UserNotificationObject
    {
        public long UserNotificationAssocID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public long NotificationID { get; set; }
        public int ApplicationID { get; set; }
        public string Text { get; set; }
        public string NotificationDetails { get; set; }
        public DateTime GenerationTime { get; set; }
    }
}

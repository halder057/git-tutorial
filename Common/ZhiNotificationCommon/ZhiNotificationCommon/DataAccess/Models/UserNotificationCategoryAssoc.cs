using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class UserNotificationCategoryAssoc
    {
        public long ID { get; set; }
        public int UserID { get; set; }
        public int NotificationCategoryID { get; set; }
        public virtual NotificationCategory NotificationCategory { get; set; }
    }
} 
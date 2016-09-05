using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotification.Models
{
    public class ChangeViewModel
    {
        public List<Change> Changes { get; set; }
        public List<NotificationCategory> NotificationCategories { get; set; }
    }
}
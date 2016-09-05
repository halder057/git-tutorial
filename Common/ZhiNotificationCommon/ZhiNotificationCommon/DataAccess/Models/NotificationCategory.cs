using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ZhiNotificationCommon.DataAccess.Models
{
    public class NotificationCategory
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool? Mandatory { get; set; }
        public bool? AutoApproval { get; set; }
        public int ApplicationID { get; set; }

        public virtual ICollection<UserNotificationCategoryAssoc> UserNotificationCategories { get; set; }
        public virtual ICollection<Change> Changes { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<NotificationTemplateMessage> NotificationTemplateMessages { get; set; }
    }
}
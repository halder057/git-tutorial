using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class NotificationTemplateMessage
    {
        public int ID { get; set; }
        public int NotificationCategoryID { get; set; }
        public string Message { get; set; }
        public bool? Active { get; set; }
        public DateTime? LastUpdateTime { get; set; }

        public virtual NotificationCategory NotificationCategory { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class NotificationEvent
    {
        public int ID { get; set; }
        public string EventName { get; set; }

        public virtual ICollection<NotificationModuleNotificationEventAssoc> NotificationModuleNotificationEventAssoc { get; set; }
    }
}

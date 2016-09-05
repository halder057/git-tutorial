using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class NotificationModule
    {
        public int ID { get; set; }
        public string Modulename { get; set; }
        public virtual ICollection<NotificationError> NotificatioError { get; set; }
        public virtual ICollection<NotificationModuleNotificationEventAssoc> NotificationModuleNotificationEventAssoc { get; set; }
    }
}

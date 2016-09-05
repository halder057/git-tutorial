using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class NotificationModuleNotificationEventAssoc
    {
        public int ID { get; set; }
        public int ModuleID { get; set; }
        public int EventID { get; set; }

        public virtual NotificationModule NotificationModule { get; set; }
        public virtual NotificationEvent NotificationEvent { get; set; }
    }
}

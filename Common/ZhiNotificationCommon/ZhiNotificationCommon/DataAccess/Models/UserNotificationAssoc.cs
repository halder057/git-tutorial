using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class UserNotificationAssoc
    {
        public long ID { get; set; }
        public int UserID { get; set; }
        public long NotificationID { get; set; }
        public int? Status { get; set; }
        public int ApplicationID { get; set; }

        public virtual NotificationStatus NotificationStatus { get; set; }

        public virtual Notification Notification { get; set; }
        public virtual ICollection<DeliveryLog> DeliveryLog { get; set; }
    }
}

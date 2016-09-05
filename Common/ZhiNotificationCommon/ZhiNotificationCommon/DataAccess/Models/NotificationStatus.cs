using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class NotificationStatus
    {
        public int ID { get; set; }
        public string Value { get; set; }

        public virtual ICollection<UserNotificationAssoc> UserNotificationAssocs { get; set; }
    }
}
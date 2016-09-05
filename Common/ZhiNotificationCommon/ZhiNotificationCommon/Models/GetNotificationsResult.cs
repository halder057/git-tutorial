using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.Models
{
    public class UserNotificationAndDeliveryDetails
    {
        public List<UserNotificationObject> UserNotifications { get; set; }
        public List<UserNotificationDeliveryObject> UserNotificationDeliveries { get; set; }
    }
}

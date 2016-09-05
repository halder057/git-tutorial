using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.Models
{
    public class UserNotificationDeliveryObject
    {
        public int ApplicationID { get; set; }
        public int UserID { get; set; }
        public int? DeliveryMethod { get; set; }
        public int? DeliveryFrequency { get; set; }
        public int? ContentVolume { get; set; }
    }
}

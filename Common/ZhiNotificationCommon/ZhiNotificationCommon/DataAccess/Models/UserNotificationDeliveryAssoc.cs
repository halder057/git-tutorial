using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class UserNotificationDeliveryAssoc
    {
        public long ID { get; set; }
        public int UserID { get; set; }
        public Nullable<int> DeliveryMethod { get; set; }
        public Nullable<int> DeliveryFrequency { get; set; }
        public Nullable<int> ContentVolume { get; set; }
        public int ApplicationID { get; set; }
        public DateTime? LastDeliveryTimestamp { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class DeliveryLog
    {
        public long ID { get; set; }
        public long UserNotificationAssocID { get; set; }
        public int DeliveryFrequencyID { get; set; }
        public DateTime Timestamp { get; set; }
        public virtual UserNotificationAssoc UserNotificationAssoc { get; set; }
        public virtual DeliveryFrequency DeliveryFrequency { get; set; }
    }
}

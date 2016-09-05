using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class SubscriptionLog
    {
        public int ID { get; set; }
        public int SubscriptionLevelID { get; set; }
        public int RowID { get; set; }
        public int SubscriptionChangeTypeID{get; set;}
        public DateTime Timestamp { get; set; }
        public virtual SubscriptionLevel SubscriptionLevel { get; set; }
        public virtual SubscriptionChangeType SubscriptionChangeType { get; set; }
    }
}

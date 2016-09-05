using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class SubscriptionLevel
    {
        public int ID { get; set; }
        public string Level { get; set; }
        public virtual ICollection<SubscriptionLog> SubscriptionLog { get; set; }
    }
}

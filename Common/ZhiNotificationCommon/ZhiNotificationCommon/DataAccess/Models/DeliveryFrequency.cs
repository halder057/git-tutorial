using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class DeliveryFrequency
    {
        public int ID { get; set; }
        public string Value { get; set; }
        public virtual ICollection<DeliveryLog> DeliveryLog { get; set; }
    }
}

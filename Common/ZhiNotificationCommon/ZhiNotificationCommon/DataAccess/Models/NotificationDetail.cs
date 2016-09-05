using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class NotificationDetail
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public virtual ICollection<ChangeNotificationDetailAssoc> ChangeNotificationDetailAssocs { get; set; }
    }
}

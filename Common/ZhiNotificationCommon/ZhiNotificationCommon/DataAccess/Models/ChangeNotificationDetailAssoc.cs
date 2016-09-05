using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class ChangeNotificationDetailAssoc
    {
        public long ID { get; set; }
        public long ChangeID { get; set; }
        public long NotificationDetailID { get; set; }

        public virtual Change Change { get; set; }
        public virtual NotificationDetail NotificationDetail { get; set; }
    }
}

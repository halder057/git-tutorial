using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.Models
{
    public class ApproveChangesAndGenerateNotificationsResult
    {
        public long ID { get; set; }
        public int NotificationCategoryID { get; set; }
        public long ChangeID { get; set; }
    }
}

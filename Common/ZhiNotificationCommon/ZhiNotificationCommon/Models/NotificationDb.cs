using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.Models
{
    public class NotificationDb
    {
        public long NotificationID { get; set; }
        public int NotificationCategoryID { get; set; }
        public long ChangeID { get; set; }
    }
}

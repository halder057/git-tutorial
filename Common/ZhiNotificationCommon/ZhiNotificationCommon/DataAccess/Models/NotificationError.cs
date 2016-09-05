using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class NotificationError
    {
        public long ID { get; set; }
        public int ModuleID { get; set; }
        public string ErrorText { get; set; }
        public string StackTrace { get; set; }
        public DateTime OccurrenceTime { get; set; }

        public virtual NotificationModule NotificationModule { get; set; }
    }
}

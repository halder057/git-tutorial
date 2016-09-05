using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class Notification
    {
        public long ID { get; set; }
        public int NotificationCategoryID { get; set; }
        public string Text { get; set; }
        public DateTime GenerationTime { get; set; }
        public string NotificationDetails { get; set; }
        public long? ChangeID { get; set; }
        public int? McoId { get; set; }
        public string IndicationAbbreviation { get; set; }
        public string DrugName { get; set; }

        public virtual ICollection<UserNotificationAssoc> UserNotificationAssocs { get; set; }

        public virtual NotificationCategory NotificationCategory { get; set; }
        public virtual Change Change { get; set; }

    }
}

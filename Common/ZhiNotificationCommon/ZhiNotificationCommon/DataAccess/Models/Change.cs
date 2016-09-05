using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
        public class Change
        {
            public long ID { get; set; }
            public int NotificationCategoryID { get; set; }
            public string ChangedFieldName { get; set; }
            public string PreviousValue { get; set; }
            public string CurrentValue { get; set; }
            public string NotificationMessage { get; set; }
            public DateTime? GenerationTime { get; set; }
            public int? ChangedBy { get; set; }
            public int? ApprovedBy { get; set; }
            public int? Status { get; set; }
            public string NotificationDetails { get; set; }
            public int? McoId { get; set; }
            public string IndicationAbbreviation { get; set; }
            public string DrugName { get; set; }

            public virtual NotificationCategory NotificationCategory { get; set; }
            public virtual ChangeStatus ChangeStatus { get; set; }
            public virtual ICollection<Notification> Notifications { get; set; }
            public virtual ICollection<ChangeNotificationDetailAssoc> ChangeNotificationDetailAssocs { get; set; }
            public virtual ICollection<ChangeApprovalLog> ChangeApprovalLog { get; set; }
            public virtual ICollection<ScanLog> ScanLog { get; set; }
        }
}

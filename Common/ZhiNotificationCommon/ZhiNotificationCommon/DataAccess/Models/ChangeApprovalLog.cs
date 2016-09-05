using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class ChangeApprovalLog
    {
        public long ID { get; set; }
        public int UserID { get; set; }
        public long ChangeID { get; set; }
        public int ChangeApprovalChangeTypeID { get; set; }
        public DateTime Timestamp { get; set; }
        public virtual Change Change { get; set; }
        public virtual ChangeApprovalChangeType ChangeApprovalChangeType { get; set; }
    }
}

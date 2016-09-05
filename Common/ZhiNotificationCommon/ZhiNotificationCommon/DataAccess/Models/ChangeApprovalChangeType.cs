using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class ChangeApprovalChangeType
    {
        public int ID { get; set; }
        public string ChangeType { get; set; }
        public virtual ICollection<ChangeApprovalLog> ChangeApprovalLog { get; set; }
    }
}

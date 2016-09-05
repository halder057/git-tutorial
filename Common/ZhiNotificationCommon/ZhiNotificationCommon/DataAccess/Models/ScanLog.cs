using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class ScanLog
    {
        public long ID { get; set; }
        public int LogID { get; set; }
        public int RowID { get; set; }
        public int? MCOID { get; set; }
        public long ChangeID { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime ProcessingTimestamp { get; set; }
        public virtual Change Change { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiPattChangeScanner.DataAccessLayer.Models
{
    public class ScanHistory
    {
        public long ID { get; set; }
        public int? ApplicationID { get; set; }
        public int LogID { get; set; }
        public DateTime ProcessingTimeStamp { get; set; }
    }
}

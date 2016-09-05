using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class UserIndicationAssoc
    {
        public long ID { get; set; }
        public int UserID { get; set; }
        public int IndicationID { get; set; }
    }
}

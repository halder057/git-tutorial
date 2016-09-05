using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class NotificationColumn
    {
        public int ID { get; set; }
        public string DisplayName { get; set; }
        public string ColumnName { get; set; }
        public string TableName { get; set; }
        public string Indication { get; set; }
    }
}

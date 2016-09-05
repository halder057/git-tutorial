using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class NotificationEmailTemplateColumnOrder
    {
        public int ID { get; set; }
        public string FieldName { get; set; }
        public int AppearanceOrder { get; set; }
        public int SortOrder { get; set; }
    }
}

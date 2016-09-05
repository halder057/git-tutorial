using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiPattChangeScanner.Models
{
    public class PattChangesDetailsTemplates
    {
        public List<ChangeObject> Changes { get; set; }
        public List<ChangeDetailObject> ChangeDetails { get; set; }
        public List<NotificationTemplateMessageObject> NotificationTemplateMessages { get; set; }
    }

    public class ChangeObject
    {
        public int? LogId { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string NotificationMessage { get; set; }
        public int? NotificationCategoryID { get; set; }
    }

    public class ChangeDetailObject
    {
        public int? LogId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class NotificationTemplateMessageObject
    {
        public int NotificationCategoryID { get; set; }
        public string Message { get; set; }
    }
}

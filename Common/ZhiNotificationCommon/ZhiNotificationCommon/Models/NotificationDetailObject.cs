using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZhiNotificationCommon.Models
{

    public class NotificationDetailObject
    {
        public long? ChangeID { get; set; }
        public string PlanName { get; set; }
        public string NotificationMessage { get; set; }
        public DateTime? ChangeGenerationTime { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string TA { get; set; }
        public string Drug { get; set; }
        public DateTime GenerationTime { get; set; }
        public Dictionary<string, string> DetailProperties { get; set; }
    }

    public class FilteredChangesByPlanObject
    {
        public string PlanName { get; set; }
        public List<FilteredDetailPropertiesByDateObject> FilteredChanges { get; set; }
    }

    public class FilteredDetailPropertiesByDateObject
    {
        public string DateString { get; set; }
        public List<Dictionary<string, string>> DetailProperties { get; set; }
    }
}
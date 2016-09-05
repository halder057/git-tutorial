using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZhiNotification.Models
{
    public class NotificationDeliveryLogViewModel
    {
        public string UserName { get; set; }
        public string NotificationText { get; set; }
        public string DeliveryFrequencyValue { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
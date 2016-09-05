using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotification.Models
{
    public class PageViewModel
    {
        public List<Application> Applications { get; set; }
        public Dictionary<Int32, Int32> Preferences { get; set; }
        public List<SubscriptionClient> SubscriptionClients { get; set; }
    }
}
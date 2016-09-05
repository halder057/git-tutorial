using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZhiNotification.Models
{
    public class SubscriptionSubmissionModel
    {
        public int ApplicationID { get; set; }
        public List<NotificationCategorySubscription> NotificationCategorySubscriptions { get; set; }
        public List<DeliveryMethodSubscription> DeliveryMethos { get; set; }
        public List<IndicationSubscription> IndicationSubscriptions { get; set; }
        public List<CategoryWithColumns> CategoriesWithColumns { get; set; }
        public int DeliveryFrequency { get; set; }
        public int ContentVolume { get; set; }
        public List<int> UserIds { get; set; }
    }
}
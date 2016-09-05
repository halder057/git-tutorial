using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotification.Models
{
    public class SubscriptionViewModel
    {
        public int ApplicationID { get; set; }
        public List<NotificationCategorySubscription> NotificationCategorySubscriptions { get; set; }
        public List<IndicationSubscription> IndicationSubscriptions { get; set; }
        public List<DeliveryMethodSubscription> DeliveryMethodsSubscriptions { get; set; }
        public List<ContentVolumeSubscription> ContentVolumeSubscriptions { get; set; }
        public List<DeliveryFrequencySubscription> DeliveryFrequencySubscriptions { get; set; }
        public List<CategoryWithColumns> CategoriesWithColumns { get; set; }


        public SubscriptionViewModel()
        {
            this.NotificationCategorySubscriptions = new List<NotificationCategorySubscription>();
            this.IndicationSubscriptions = new List<IndicationSubscription>();
            this.DeliveryMethodsSubscriptions = new List<DeliveryMethodSubscription>();
            this.ContentVolumeSubscriptions = new List<ContentVolumeSubscription>();
            this.DeliveryFrequencySubscriptions = new List<DeliveryFrequencySubscription>();
            this.CategoriesWithColumns = new List<CategoryWithColumns>();
        }
    }

    public class CategoryFilterCriteria
    {
        public int ApplicationId { get; set; }
        public int UserId { get; set; }
        public List<IndicationSubscription> SubscribedIndications { get; set; }
        public List<CategoryWithColumns> SelectedCategoryColumns { get; set; }
        public CategoryFilterCriteria()
        {
            this.SubscribedIndications = new List<IndicationSubscription>();
            this.SelectedCategoryColumns = new List<CategoryWithColumns>();
        }
    }


    public class NotificationCategorySubscription
    {
        public int NotificationCategoryID { get; set; }
        public string NotificationCategoryName { get; set; }
        public bool IsSubscribed { get; set; }
        public bool IsMandatory { get; set; }
    }
    public class IndicationSubscription
    {
        public int IndicationID { get; set; }
        public string IndicationName { get; set; }
        public string IndicationAbbreviation { get; set; }
        public bool IsSubscribed { get; set; }
    }
    public class DeliveryMethodSubscription
    {
        public int ID { get; set; }
        public string Value { get; set; }
        public bool IsChecked { get; set; }
    }
    public class ContentVolumeSubscription
    {
        public int ID { get; set; }
        public string Value { get; set; }
        public bool IsChecked { get; set; }
    }
    public class DeliveryFrequencySubscription
    {
        public int ID { get; set; }
        public string Value { get; set; }
        public bool IsChecked { get; set; }
    }
    public class SubscriptionClient
    {
        public string ClientName { get; set; }
        public List<SubscriptionUser> SubscriptionUsers { get; set; }
        public SubscriptionClient()
        {
            this.SubscriptionUsers = new List<SubscriptionUser>();
        }
    }
    public class SubscriptionUser
    {
        public string UserName { get; set; }
        public int UserId { get; set; }
    }

    public class CategoryWithColumns
    {
        public int NotificationCategoryID { get; set; }
        public string NotificationCategoryName { get; set; }
        public bool IsSubscribed { get; set; }
        public List<Column> Columns { get; set; }
        public CategoryWithColumns()
        {
            this.Columns = new List<Column>();
        }
    }

    public class Column
    {
        public int ColumnID { get; set; }
        public string ColumnName { get; set; }
        public bool IsSubscribed { get; set; }
    }
}

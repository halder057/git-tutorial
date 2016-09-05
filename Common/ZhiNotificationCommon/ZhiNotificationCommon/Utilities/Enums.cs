using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.Utilities
{
    public enum PreferenceType
    {
        Application = 1
    }

    public enum Page
    {
        PendingChanes = 1,
        Subscription = 2,
        Log = 3
    }

    public enum Application
    {
        PATT = 1
    }

    public enum ChangeStatus
    {
        Pending = 1,
        Approved = 2,
        Discarded = 3
    }

    public enum NotificationStatus
    {
        Pending = 1,
        Delivered = 2,
        Acknowledged = 3
    }

    public enum DeliveryMethod
    {
        Push = 1,
        Email = 2,
        Text = 3
    }

    public enum DeliveryFrequency
    {
        RealTime = 1,
        Daily = 2,
        Weekly = 3,
        Monthly = 4
    }

    public enum Module
    {
        ChangeScannerApplication = 1,
        NotificationApplication = 2,
        DeliveryApplication = 3
    }

    public enum ContentVolume
    {
        Single = 1,
        Bundle = 2
    }

    public enum BrontoEmailType
    {
        Transactional,
        Marketing,
        Triggered,
        Test
    }
    public enum BrontoMessage
    {
        PattUpdates
    }
}

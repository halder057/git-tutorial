using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotification.Models
{
    public class ErrorModel
    {
        public List<NotificationError> Errors { get; set; }
    }
}
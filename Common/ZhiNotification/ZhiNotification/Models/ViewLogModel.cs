using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZhiNotificationCommon.Models;
using ZhiNotificationCommon.DataAccess.Models;


namespace ZhiNotification.Models
{
    public class ViewLogModel
    {
        public List<Application> Applications { get; set; }
        public List<Module> Modules { get; set; }

    }
}
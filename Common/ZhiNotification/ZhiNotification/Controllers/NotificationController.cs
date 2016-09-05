using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ZhiNotification.Hub;
using ZhiNotificationCommon.Models;
using ZhiNotificationCommon.Utilities;

namespace ZhiNotification.Controllers
{
    public class NotificationController : ApiController
    {
        [HttpPost]
        public bool NotifyAppUser(int appId, int userId, List<UserNotificationObject> notifications)
        {
            bool bRet = false;
            try
            {
                NotificationHub.NotifyAppUser(appId, userId, notifications);
                bRet = true;
            }
            catch (Exception ex)
            {
                
                int moduleId = (int)ZhiNotificationCommon.Utilities.Module.NotificationApplication; // moduleId = 3
                string message = ZhiNotificationCommon.Utilities.Constants.ErrorWhileNotifyingUser; // ErrorWhileNotifyingUser = "Error While Notifying User"
                ZhiNotificationCommon.Utilities.LoggerHelper.WriteLogThroughCommonLogger(moduleId, message, ex);
            }
            return bRet;
        }

    }
}

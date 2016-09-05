using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using ZhiNotification.Models;
using ZhiNotificationCommon.Models;

namespace ZhiNotification.Hub
{
    [HubName("NotificationHub")]
    public class NotificationHub : Microsoft.AspNet.SignalR.Hub
    {
        private static Dictionary<NotificationUser, string> _applicationUserConnectionIds = new Dictionary<NotificationUser, string>();

        public override System.Threading.Tasks.Task OnConnected()
        {
            var appIdstr = Context.QueryString["ApplicationID"];
            var userIdstr = Context.QueryString["UserID"];
            int appId, userId;

            if (int.TryParse(appIdstr, out appId))
                if (int.TryParse(userIdstr, out userId))
                    _applicationUserConnectionIds[new NotificationUser { ApplicationID = appId, UserID = userId }] = Context.ConnectionId;

            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var connId = Context.ConnectionId;
            if (_applicationUserConnectionIds.ContainsValue(connId))
                _applicationUserConnectionIds
                    .Remove(_applicationUserConnectionIds.First(f => f.Value == connId).Key);
            return base.OnDisconnected(stopCalled);
        }

        public static void NotifyAppUser(int appId, int userId, List<UserNotificationObject> notifications)
        {
            var notificationHubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            var appUser = new NotificationUser
            {
                ApplicationID = appId,
                UserID = userId
            };
            if (_applicationUserConnectionIds.ContainsKey(appUser))
                notificationHubContext.Clients.Client(_applicationUserConnectionIds[appUser]).AddNotifications(notifications);
        }
    }
}
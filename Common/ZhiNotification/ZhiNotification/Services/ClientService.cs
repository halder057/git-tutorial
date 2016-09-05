using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZhiNotificationCommon.DataAccess.Models;
using ZhiNotificationCommon.DataAccess;

namespace ZhiNotification.Services
{
    public class ClientService
    {
        public static Client GetClient(int clientId)
        {
            return ClientDataAccess.GetClient(clientId);
        }

        public static List<Client> GetClients()
        {
            return ClientDataAccess.GetClients();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiNotificationCommon.DataAccess.DatabaseContexts;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess
{
    public class ClientDataAccess
    {
        public static List<Client> GetClients(bool activeOnly = true)
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.Clients.Where(x => !activeOnly || x.Active).OrderBy(x => x.Name.Trim()).ToList();
            }
        }

        public static Client GetClient(int clientId, bool activeOnly = true)
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.Clients.FirstOrDefault(x => (!activeOnly || x.Active) && (x.ClientID == clientId));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data;
using System.Data.SqlClient;
using ZhiNotificationCommon.DataAccess.DatabaseContexts;
using ZhiNotificationCommon.DataAccess.Models;
using ZhiNotificationCommon.Models;
using ZhiNotificationCommon.Services;

namespace ZhiNotificationCommon.DataAccess
{
    public class LogDataAccess
    {
        public static List<NotificationModule> GetNotificationModules()
        {
            using (var dbcontext = new NotificationContext())
            {
                return dbcontext.NotificationModule.ToList();
            }
        }
        public static List<NotificationEvent> GetNotificationEvents()
        {
            using (var dbcontext = new NotificationContext())
            {
                return dbcontext.NotificationEvent.ToList();
            }
        }
        public static List<NotificationModuleNotificationEventAssoc> GetNotificationModuleEventAssoc()
        {
            using (var dbcontext = new NotificationContext())
            {
                return dbcontext.NotificationModuleNotificationEventAssoc
                    .Include(x => x.NotificationModule)
                    .Include(x => x.NotificationEvent)
                    .ToList();
            }
        }

        public static List<NotificationError> GetNotificationErrors(int moduleId)
        {
            using (var dbcontext = new NotificationContext())
            {
                return dbcontext.NotificationError
                    .Where(ne => ne.ModuleID == moduleId)
                    .ToList();
            }
        }
        public static void WriteNotificationDeliveryLog(List<UserDeliveryLogDetail> userDeliveryLogDetails)
        {
            try
            {
                using (var dbcontext = new NotificationContext())
                {
                    foreach (var userDeliveryLogDetail in userDeliveryLogDetails)
                    {
                        foreach (var userNotificationAssocId in userDeliveryLogDetail.UserNotificationAssocIds)
                        {
                            var deliveryLog = new DeliveryLog();
                            deliveryLog.DeliveryFrequencyID = userDeliveryLogDetail.DeliveryFrequencyId;
                            deliveryLog.Timestamp = DateTime.UtcNow;
                            deliveryLog.UserNotificationAssocID = userNotificationAssocId;
                            dbcontext.DeliveryLogs.Add(deliveryLog);
                        }
                    }
                    dbcontext.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<DeliveryLog> GetNotificationDeliveryLog()
        {
            using (var dbcontext = new NotificationContext())
            {
                return dbcontext.DeliveryLogs.Include(x => x.UserNotificationAssoc).Include(y=> y.UserNotificationAssoc.Notification).Include(z => z.DeliveryFrequency).ToList() ;
            }
        }
    }
}

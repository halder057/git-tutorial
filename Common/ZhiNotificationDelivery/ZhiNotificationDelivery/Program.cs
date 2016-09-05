using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ZhiNotificationCommon.Services;
using ZhiNotificationCommon.Utilities;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace ZhiNotificationDelivery
{
    public class Program
    {
        /// <summary>
        ///   Upon getting called, this program will do the followings:
        ///   1. Determine deliverable notifications
        ///   2. Determine users for those notifications
        ///   3. Deliver notifications through email
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            //Log4net init
            log4net.GlobalContext.Properties["LogFolderPath"] = ConfigurationManager.AppSettings["LogFolderPath"];
            log4net.Config.BasicConfigurator.Configure();
            CommonLogger.Instance.LogInfo("Notification Delivery Process Started.");

            Arguments arguments = new Arguments(args);

            bool isRealTime = (arguments[ZhiNotificationCommon.Utilities.Constants.RealTimeNotificationDeliveryCommandLineFlag] != null);

            if (isRealTime)
            {
                CommonLogger.Instance.LogInfo("Only realtime notifications will be processed and delivered.");
            }
            else
            {
                CommonLogger.Instance.LogInfo("Only scheduled notifications will be processed and delivered.");
            }

            try
            {           
                if (!NotificationService.DeliverPendingEmailNotifications(isRealTime))
                {
                    CommonLogger.Instance.LogError("An error occurred while trying to deliver notifications.");
                }
                else
                {
                    CommonLogger.Instance.LogInfo("Notification Delivery Process Successfully Completed.");
                }
            }
            catch (Exception ex)
            {
                int moduleId = (int)ZhiNotificationCommon.Utilities.Module.DeliveryApplication;               
                LoggerHelper.WriteLogThroughCommonLogger(moduleId, "An error occurred while trying to deliver notifications.", ex);             
            }
        }
    }
}

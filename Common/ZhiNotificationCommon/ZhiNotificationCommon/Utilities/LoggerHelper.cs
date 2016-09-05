using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.Utilities
{
    public class LoggerHelper
    {
        public static void WriteLogThroughCommonLogger(int moduleId, string message, Exception ex)
        {
            log4net.LogicalThreadContext.Properties["ModuleID"] = moduleId;
            CommonLogger.Instance.LogError(message, ex);
        }
    }
}

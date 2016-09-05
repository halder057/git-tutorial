using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace ZhiNotificationCommon.Utilities
{
    public class CommonLogger
    {
        private ILog log;
        private static readonly CommonLogger _instance = new CommonLogger();
        private CommonLogger()
        {
            log = log4net.LogManager.GetLogger(typeof(CommonLogger));
        }
        public static CommonLogger Instance
        {
            get
            {
                return _instance;
            }
        }
        public void LogError(string message, Exception exception = null)
        {
            try
            {
                
                if (exception != null)
                    log.Error(message, exception);
                else
                    log.Error(message);
            }
            catch( Exception ex)
            {

            }
        }
        public void LogInfo(string message, Exception exception = null)
        {
            if (exception != null)
                log.Info(message, exception);
            else
                log.Info(message);
        }
        public void LogWarning(string message, Exception exception = null)
        {
            if (exception != null)
                log.Warn(message, exception);
            else
                log.Warn(message);
        }

    }
}

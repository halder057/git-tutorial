using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiPattChangeScanner.Utilities;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace ZhiPattChangeScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            //Log4net init
            log4net.GlobalContext.Properties["LogFolderPath"] = ConfigurationManager.AppSettings["LogFolderPath"];
            log4net.Config.BasicConfigurator.Configure();

            Processor processor = new Processor();
            processor.Process(Logger.Instance);
        }
    }
}

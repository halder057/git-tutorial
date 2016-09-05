using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SpiderDashboard.DataAccessLayer.Models;
using SpiderDashboard.DataAccessLayer.DataAccess;

namespace SpiderDashboard.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (SpiderDashboard.DataAccessLayer.Models.User.UserID.HasValue)
            {
                ViewBag.Flag = "Hello " + SpiderDashboard.DataAccessLayer.Models.User.WindowsName;
            }

            //var alldata = FileWatchHistoryDataAccess.GetFileHistoryAndFileSubscriptionDetails("PFM Legacy", "Abraxane", 1640, Convert.ToDateTime("2014-07-30 15:12:48.000"));
            var analyst = SubscribingDocDataAccess.GetAnalysts();

            return View(analyst);
        }

        public JsonResult GetDrugsAndFileWatchIds(string analyst)
        {
            var drugs = SubscribingDocDataAccess.GetDrugs(analyst);
            var fileWatchIds = SubscribingDocDataAccess.GetFileWatchIds(analyst);
            return this.Json(new { Drugs = drugs, FileWatchIds = fileWatchIds }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFileWatchIds(string analyst, string drug)
        {
            var fileWatchIds = SubscribingDocDataAccess.GetFileWatchIds(analyst, drug);
            return this.Json(new { FileWatchIds = fileWatchIds }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFileWatchHistoryAndSubscriptionData(string analyst, string drug, int filewatchid, string date)
        {
            int dateValue;
            DateTime lastDateValue = DateTime.Now;

            if (int.TryParse(date, out dateValue))
            {
                dateValue = (dateValue * -1);
                var now = DateTime.Now;
                lastDateValue = now.AddDays(dateValue);
            }

            var historyAndSubscriptionData = FileWatchHistoryDataAccess.GetFileHistoryAndFileSubscriptionDetails(analyst, drug, filewatchid, lastDateValue);
            return this.Json(new { HistoryAndSubscriptionData = historyAndSubscriptionData }, JsonRequestBehavior.AllowGet);
        }
        //public FileResult DisplayPDF()
        //{
        //    return File("C:/Users/robin/Downloads/Actinic Keratoses Treatments (1).pdf", "application/pdf");
        //}

    }
}
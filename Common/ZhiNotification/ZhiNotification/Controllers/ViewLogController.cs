using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZhiNotification.Services;
using ZhiNotification.Models;
using ZhiNotification.Utilities;
using ZhiNotificationCommon.DataAccess;

namespace ZhiNotification.Controllers
{
    public class ViewLogController : Controller
    {
        public ActionResult ViewLogs()
        {
            ViewLogService service = new ViewLogService();
            ViewLogModel data = service.GetModuleAndEventData(User.Identity.Name);
            ViewBag.SelectedMenuItem = MenuItem.Log;
            return View("ViewLogs", data);
        }
        [HttpPost]
        public ActionResult GetLogData(int moduleId,int eventId)
        {
            if (eventId == 1)
            {
                return GetErrors(moduleId);
            }
            else return View();
        }

        public ActionResult GetErrors(int moduleId = -1)
        {
            ErrorModel data = new ErrorModel();
            ViewLogService vlService = new ViewLogService();
            data = vlService.GetErrors(moduleId);
            return PartialView("Errors", data);
        }

        public ActionResult GetNotificationDeliveryLogs()
        {
            NotificationManagementService notificationManagementService = new NotificationManagementService();
            var notificationDeliveryLogs = notificationManagementService.GetNotificationDeliveryLogs();

            return View(notificationDeliveryLogs);
        }
    }
}
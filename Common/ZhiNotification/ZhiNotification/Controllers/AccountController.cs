using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ZhiNotification.Models;
using ZhiNotification.Services;
using ZhiNotificationCommon.DataAccess.Models;
using ZhiNotification.Utilities;
using ZhiNotification.MembershipInfrastructure;

namespace ZhiNotification.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                AccountService service = new AccountService();
                User user = service.GetUser(User.Identity.Name);

                return RedirectToAction("ManageSubscriptions", "NotificationManagement");
            }
            return View();
        }
        public ActionResult Login()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            AccountService service = new AccountService();

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("ManageSubscriptions", "NotificationManagement");
            }

            string username = model.UserName;
            string password = model.Password;
            User user = service.GetUser(username, password);

            if (user != null && user.Type.Equals(RoleNames.AuthorizedSuperAdmin))
            {
                FormsAuthentication.SetAuthCookie(username, false);
                return RedirectToAction("ManageSubscriptions", "NotificationManagement");
            }
            else if (user == null)
            {
                ModelState.AddModelError("", Constants.InvalidCredential);
                return View("Index");
            }
            else
            {
                ModelState.AddModelError("", Constants.InsufficientPrivilege);
                return View("Index");
            }
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}
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
using ZhiNotification.ActionFilters;

namespace ZhiNotification.Controllers
{
    [CustomAuthorize]
    public class NotificationManagementController : Controller
    {
        [CustomAuthorize(Roles = RoleNames.AuthorizedSuperAdmin)]
        public ActionResult ManageChanges()
        {
            NotificationManagementService service = new NotificationManagementService();
            PageViewModel data = service.GetManageChangeData(User.Identity.Name);
            ViewBag.SelectedMenuItem = MenuItem.Changes;
            return View("ManageChanges", data);
        }

        [CustomAuthorize(Roles = RoleNames.AuthorizedSuperAdmin)]
        [HttpPost]
        public ActionResult GetChanges(int applicationID = -1)
        {
            NotificationManagementService notyManagementService = new NotificationManagementService();
            ChangeViewModel data = notyManagementService.GetChangeViewModel(applicationID);

            int pageID = (int)ZhiNotificationCommon.Utilities.Page.PendingChanes;
            int prefType = (int)ZhiNotificationCommon.Utilities.PreferenceType.Application;
            Dictionary<int, int> preference = new Dictionary<int, int>();
            preference.Add(prefType, applicationID);
            UpdatePreference(pageID, preference);

            return PartialView("Changes", data);
        }

        public ActionResult ApproveChanges(ChangeModel approvedData)
        {
            NotificationManagementService notyManagementService = new NotificationManagementService();
            AccountService service = new AccountService();
            User loggedIn = service.GetUser(User.Identity.Name);
            bool sucess = notyManagementService.ApproveChangesAndGenerateNotification(approvedData, loggedIn.UserID);

            if (sucess)
            {
                ChangeViewModel data = notyManagementService.GetChangeViewModel(approvedData.ApplicationId);
                return PartialView("Changes", data);
            }
            else
            {
                return Json(new { error = true, message = Constants.ChangesCouldNotBeApproved });
            }
        }

        public ActionResult DiscardChanges(ChangeModel discardedData)
        {
            NotificationManagementService notyManagementService = new NotificationManagementService();
            AccountService service = new AccountService();
            User loggedIn = service.GetUser(User.Identity.Name);
            bool sucess = notyManagementService.DiscardChanges(discardedData, loggedIn.UserID);

            if (sucess)
            {
                ChangeViewModel data = notyManagementService.GetChangeViewModel(discardedData.ApplicationId);
                return PartialView("Changes", data);
            }
            else
            {
                return Json(new { error = true, message = Constants.ChangesCouldNotBeDiscarded });
            }
        }

        public ActionResult ManageSubscriptions()
        {
            NotificationManagementService service = new NotificationManagementService();
            PageViewModel data = service.GetManageSubscriptionData(User.Identity.Name);
            ViewBag.SelectedMenuItem = MenuItem.Subscription;
            return View("ManageSubscriptions", data);
        }

        [HttpPost]
        public ActionResult GetSubscriptions(bool forApplicationChange, int applicationID = -1, int userID = -1)
        {
            AccountService service = new AccountService();
            NotificationManagementService notyManagementService = new NotificationManagementService();

            User loggedIn = service.GetUser(User.Identity.Name);

            SubscriptionViewModel data = new SubscriptionViewModel();


            bool isSuperAdmin = Roles.IsUserInRole(RoleNames.SuperAdmin);
            bool isClientAdmin = Roles.IsUserInRole(RoleNames.Admin);

            bool isAnyAdmin = isSuperAdmin || isClientAdmin;

            if (forApplicationChange && isAnyAdmin)
            {
                data.ContentVolumeSubscriptions = new List<ContentVolumeSubscription>();
                data.DeliveryFrequencySubscriptions = new List<DeliveryFrequencySubscription>();
                data.DeliveryMethodsSubscriptions = new List<DeliveryMethodSubscription>();
                data.IndicationSubscriptions = new List<IndicationSubscription>();
                data.NotificationCategorySubscriptions = new List<NotificationCategorySubscription>();

            }
            else
            {
                if (userID == -1)
                {
                    userID = loggedIn.UserID;
                }
                data = notyManagementService.GetSubscriptionData(applicationID, userID);
            }

            int pageID = (int)ZhiNotificationCommon.Utilities.Page.Subscription;
            int prefType = (int)ZhiNotificationCommon.Utilities.PreferenceType.Application;
            Dictionary<int, int> preference = new Dictionary<int, int>();
            preference.Add(prefType, applicationID);
            UpdatePreference(pageID, preference);

            data.ApplicationID = applicationID;
            return PartialView("Subscriptions", data);
        }

        [HttpPost]
        public JsonResult GetCategoriesWithColumns(CategoryFilterCriteria criteria)
        {

            NotificationManagementService notyManagementService = new NotificationManagementService();

            if (criteria.UserId == -1)
            {
                AccountService service = new AccountService();
                User loggedIn = service.GetUser(User.Identity.Name);

                return Json(notyManagementService.GetNotificationCategoryWithColumns(
                criteria.ApplicationId,
                loggedIn.UserID,
                criteria.SubscribedIndications,
                null,
                criteria.SelectedCategoryColumns));
            }
            else
            {
                return Json(notyManagementService.GetNotificationCategoryWithColumns(
                criteria.ApplicationId,
                criteria.UserId,
                criteria.SubscribedIndications,
                null,
                criteria.SelectedCategoryColumns));
            }
        }

        [HttpPost]
        public JsonResult SaveSubscriptions(SubscriptionSubmissionModel subscription)
        {
            AccountService service = new AccountService();
            User loggedIn = service.GetUser(User.Identity.Name);

            List<int> userIds = new List<int>();
            if ((Roles.IsUserInRole(RoleNames.SuperAdmin) || Roles.IsUserInRole(RoleNames.Admin)) && subscription.UserIds != null)
            {
                userIds = subscription.UserIds;
            }
            else
            {
                userIds.Add(loggedIn.UserID);
            }

            NotificationManagementService notyManagementService = new NotificationManagementService();
            bool success = notyManagementService.SaveSubscriptionData(userIds, subscription);
            JsonResult result = null;
            if (success)
            {
                result = Json(new { error = false, message = Constants.SubscriptionsSuccessfullySaved });
            }
            else
            {
                result = Json(new { error = true, message = Constants.SubscriptionsCouldNotBeSaved });
            }
            return result;
        }

        public ActionResult GetSubscribedUsers(int notificationCategoryID, long changeId)
        {
            NotificationManagementService notyManagementService = new NotificationManagementService();
            List<User> subscribedUsers = notyManagementService.GetSubscribedUsers(notificationCategoryID, changeId);

            return PartialView("SubscribedUsers", subscribedUsers);

        }

        [HttpPost]
        public void UpdatePreference(int pageID, Dictionary<int, int> preferences)
        {
            NotificationManagementService notyManagementService = new NotificationManagementService();
            notyManagementService.UpdatePreference(User.Identity.Name, pageID, preferences);
        }
    }
}

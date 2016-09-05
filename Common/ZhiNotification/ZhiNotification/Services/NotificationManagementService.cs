using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZhiNotification.Controllers;
using ZhiNotificationCommon.DataAccess;
using ZhiNotificationCommon.DataAccess.Models;
using ZhiNotification.Models;
using ZhiNotificationCommon.Models;
using ZhiNotificationCommon.Services;
using System.Diagnostics;
using System.Configuration;
using System.Data;
using ZhiNotification.Utilities;
using System.Web.Security;
using ZhiNotification.MembershipInfrastructure;

namespace ZhiNotification.Services
{
    public class NotificationManagementService
    {
        public PageViewModel GetManageChangeData(string username)
        {
            AccountService accService = new AccountService();
            User loggedIn = accService.GetUser(username);
            PageViewModel data = new PageViewModel();
            data.Applications = GetApplications();
            data.Preferences = GetPreferences(loggedIn.UserID, (int)ZhiNotificationCommon.Utilities.Page.PendingChanes);
            return data;
        }

        public PageViewModel GetManageSubscriptionData(string username)
        {
            PageViewModel data = new PageViewModel();
            List<SubscriptionClient> subscriptionClients = new List<SubscriptionClient>();

            AccountService accService = new AccountService();
            User loggedIn = accService.GetUser(username);
            bool isSuperAdmin = Roles.IsUserInRole(RoleNames.SuperAdmin);
            bool isClientAdmin = Roles.IsUserInRole(RoleNames.Admin);

            if (isSuperAdmin || isClientAdmin)
            {
                List<Client> clients = new List<Client>();
                if (isClientAdmin)
                    clients.Add(ClientService.GetClient(loggedIn.ClientID));
                else
                    clients = ClientService.GetClients();

                if (clients != null)
                {
                    var clientUsers = AccountDataAccess.GetUsers(clients.Select(x => x.ClientID).ToList());
                    foreach (var client in clients)
                    {
                        var subscriptionClient = new SubscriptionClient();
                        subscriptionClient.ClientName = client.Name.Trim();
                        var users = clientUsers.Where(x => x.ClientID == client.ClientID);
                        foreach (var user in users)
                        {
                            subscriptionClient.SubscriptionUsers.Add(new SubscriptionUser
                            {
                                UserId = user.UserID,
                                UserName = string.Format("{0} - ({1})", user.FirstName.Trim(), user.UserName.Trim()).Trim()
                            });
                        }
                        subscriptionClient.SubscriptionUsers = subscriptionClient.SubscriptionUsers.OrderBy(x => x.UserName).ToList();
                        subscriptionClients.Add(subscriptionClient);
                    }
                }
            }

            data.SubscriptionClients = subscriptionClients;
            data.Applications = GetApplications();
            data.Preferences = GetPreferences(loggedIn.UserID, (int)ZhiNotificationCommon.Utilities.Page.Subscription);
            return data;
        }

        public Dictionary<int, int> GetPreferences(int userID, int pageID)
        {
            List<UserPreference> prefs = CommonDataAccess.GetPreferences(userID, pageID);
            Dictionary<int, int> dict = new Dictionary<int, int>();
            foreach (UserPreference pref in prefs)
            {
                dict.Add(pref.PreferenceTypeID, pref.PreferenceValue);
            }
            return dict;
        }
        public void UpdatePreference(string username, int pageID, Dictionary<int, int> preferences)
        {
            AccountService service = new AccountService();
            User loggedIn = service.GetUser(username);
            CommonDataAccess.UpdatePreference(loggedIn.UserID, pageID, preferences);
        }

        public List<Application> GetApplications()
        {
            return CommonDataAccess.GetApplications();
        }
        public ChangeViewModel GetChangeViewModel(int applicationID)
        {
            ChangeViewModel data = new ChangeViewModel();
            data.NotificationCategories = NotificationManagementDataAccess.GetNotificationCategories(applicationID);
            data.Changes = GetChanges(applicationID);
            return data;
        }

        public bool DiscardChanges(ChangeModel data, int userID)
        {
            try
            {
                List<Notification> notificationList = new List<Notification>();
                List<Change> changeList = new List<Change>();
                changeList = NotificationManagementDataAccess.GetChanges(data.ChangeIds);
                foreach (Change changeObject in changeList)
                {
                    changeObject.Status = (int)ZhiNotificationCommon.Utilities.ChangeStatus.Discarded;
                    changeObject.ApprovedBy = userID;
                }
                NotificationManagementDataAccess.UpdateChanges(changeList);

                return true;
            }
            catch (Exception e)
            {
                
                int moduleId = (int)ZhiNotificationCommon.Utilities.Module.NotificationApplication; // moduleId = 3
                string message = ZhiNotificationCommon.Utilities.Constants.ErrorWhileUpdatingChangelist; // ErrorWhileUpdatingChangelist = "Error While Updating Changelist"
                ZhiNotificationCommon.Utilities.LoggerHelper.WriteLogThroughCommonLogger(moduleId, message, e);

                return false;
            }
        }

        public bool ApproveChangesAndGenerateNotification(ChangeModel data, int userId)
        {

            try
            {
                List<Notification> notificationList = new List<Notification>();

                DataTable changeIdsDataTable = GenerateDataTableOfIDs("udt_BigIntegerID", data.ChangeIds);

                var generatedNotifications = NotificationManagementDataAccess.ApproveChangesAndGenerateNotifications(changeIdsDataTable, userId);

                List<NotificationDb> notificationDbs = new List<NotificationDb>();
                foreach (var notification in generatedNotifications)
                {
                    NotificationDb obj = new NotificationDb();
                    obj.ChangeID = (long)notification.ChangeID;
                    obj.NotificationCategoryID = notification.NotificationCategoryID;
                    obj.NotificationID = notification.ID;
                    notificationDbs.Add(obj);
                }

                //Generate exclution list data table param for SP
                List<ExcludedUserDb> excludedUsers = new List<ExcludedUserDb>();
                if (data.ExcludedUsers != null)
                {
                    foreach (ExcludedUser eu in data.ExcludedUsers)
                    {
                        long changeID = eu.ChangeId;
                        if (eu.UserIds != null)
                        {
                            List<int> userIds = eu.UserIds;
                            foreach (int id in userIds)
                            {
                                ExcludedUserDb obj = new ExcludedUserDb();
                                obj.ChangeID = changeID;
                                obj.UserID = id;
                                excludedUsers.Add(obj);
                            }
                        }
                    }
                }


                bool success = NotificationManagementDataAccess.GeneratePattUserNotificationAssociations(notificationDbs, excludedUsers, data.ApplicationId);

                if (success)
                {
                    //string path = ConfigurationManager.AppSettings["deliveryapplicationpath"];
                    //ProcessStartInfo procStartInfo = new ProcessStartInfo(path, string.Format(" --{0}", ZhiNotificationCommon.Utilities.Constants.RealTimeNotificationDeliveryCommandLineFlag));
                    //procStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //Process proc = new Process();
                    //proc.StartInfo = procStartInfo;
                    //proc.Start();
                    NotificationService.DeliverPendingEmailNotifications(true);
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {               
                int moduleId = (int)ZhiNotificationCommon.Utilities.Module.NotificationApplication; // moduleId = 3
                string message = ZhiNotificationCommon.Utilities.Constants.ErrorWhileGeneratingNotification;
                ZhiNotificationCommon.Utilities.LoggerHelper.WriteLogThroughCommonLogger(moduleId, message, ex);
                return false;
            }

        }

        private List<NotificationDb> CastToNotificationTableTypeObject(List<Notification> notificationList)
        {
            return notificationList.Select(s => new NotificationDb
            {
                NotificationID = s.ID,
                NotificationCategoryID = s.NotificationCategoryID,
            }).ToList();
        }

        public SubscriptionViewModel GetSubscriptionData(int applicationId, int userId)
        {
            SubscriptionViewModel returnData = new SubscriptionViewModel();
            //View models
            List<DeliveryFrequencySubscription> dfs = new List<DeliveryFrequencySubscription>();
            List<DeliveryMethodSubscription> dms = new List<DeliveryMethodSubscription>();
            List<ContentVolumeSubscription> cvs = new List<ContentVolumeSubscription>();
            //Data models
            List<DeliveryFrequency> df = NotificationManagementDataAccess.GetDeliveryFrequencies();
            List<DeliveryMethod> dm = NotificationManagementDataAccess.GetDeliveryMethods();
            List<ContentVolume> cv = NotificationManagementDataAccess.GetContentVolumes();
            List<UserNotificationDeliveryAssoc> unda = NotificationManagementDataAccess.GetUserNotificationDeliveryAssocs(applicationId, userId);

            //Collect indications

            List<UserIndication> userAllIndications = NotificationManagementDataAccess.GetUserIndications(userId);
            List<UserIndicationAssoc> userIndicationAssocs = NotificationManagementDataAccess.GetUserIndicationAssociations(userId);
            List<IndicationSubscription> indicationSubscriptions = userAllIndications.Select(s => new IndicationSubscription
            {
                IndicationID = s.IndicationID,
                IndicationName = s.Name,
                IndicationAbbreviation = s.Abbreviation,
                IsSubscribed = userIndicationAssocs.FirstOrDefault(x => x.IndicationID == s.IndicationID) != null
            }).ToList();

            returnData.IndicationSubscriptions = indicationSubscriptions;



            //Collect Delivery Method
            foreach (DeliveryMethod item in dm)
            {
                DeliveryMethodSubscription subscription = new DeliveryMethodSubscription();
                subscription.ID = item.ID;
                subscription.Value = item.Value;
                subscription.IsChecked = (unda.FirstOrDefault(x => x.DeliveryMethod == item.ID) != null);
                dms.Add(subscription);
            }


            //Collect Delivery Frequency
            foreach (DeliveryFrequency item in df)
            {
                DeliveryFrequencySubscription subscription = new DeliveryFrequencySubscription();
                subscription.ID = item.ID;
                subscription.Value = item.Value;
                subscription.IsChecked = (unda.FirstOrDefault(x => x.DeliveryFrequency == item.ID) != null);
                dfs.Add(subscription);
            }

            foreach (ContentVolume item in cv)
            {
                ContentVolumeSubscription subscription = new ContentVolumeSubscription();
                subscription.ID = item.ID;
                subscription.Value = item.Value;
                subscription.IsChecked = (unda.FirstOrDefault(x => x.ContentVolume == item.ID) != null);
                cvs.Add(subscription);
            }

            returnData.NotificationCategorySubscriptions = GetNotificationCategorySubscriptions(applicationId, userId);
            returnData.CategoriesWithColumns = GetNotificationCategoryColumnSubscriptions(applicationId,
                indicationSubscriptions.Where(x => x.IsSubscribed).ToList(),
                userId);
            returnData.ContentVolumeSubscriptions = cvs;
            returnData.DeliveryFrequencySubscriptions = dfs;
            returnData.DeliveryMethodsSubscriptions = dms;
            returnData.ApplicationID = applicationId;
            return returnData;

        }

        public List<CategoryWithColumns> GetNotificationCategoryWithColumns(int applicationId, int userId,
            List<IndicationSubscription> subscribedIndications,
            List<UserNotificationCategoryColumnAssoc> subscribedNotyCategoryColumns = null,
            List<CategoryWithColumns> selectedCategoryColumns = null)
        {
            List<CategoryWithColumns> categoryColumns = new List<CategoryWithColumns>();
            var notyCategories = NotificationManagementDataAccess.GetNotificationCategories(applicationId);
            var indicationDataTable = GenerateIndicationDataTable("indicationDataTable", subscribedIndications.Select(x => x.IndicationAbbreviation).ToList());
            var notyColumns = NotificationManagementDataAccess.GetNotificationColumns(applicationId, userId, indicationDataTable);
            var notyCategoryColumnAssocs = NotificationManagementDataAccess.GetNotificationCategoryNotificationColumnAssocs(applicationId);

            foreach (var categoryItem in notyCategories)
            {
                CategoryWithColumns category = new CategoryWithColumns();
                category.NotificationCategoryID = categoryItem.ID;
                category.NotificationCategoryName = categoryItem.Name;
                category.IsSubscribed = false;
                category.Columns = new List<Column>();

                if (subscribedNotyCategoryColumns != null)
                {
                    //initial loading
                    category.IsSubscribed = subscribedNotyCategoryColumns
                        .FirstOrDefault(x => x.NotificationCategoryID == categoryItem.ID) != null;
                }
                else if (selectedCategoryColumns != null)
                {
                    //indication selection change
                    category.IsSubscribed = selectedCategoryColumns
                        .FirstOrDefault(x => x.NotificationCategoryID == categoryItem.ID) != null;
                }

                var columnsUnderCategory = notyColumns.Where(x => notyCategoryColumnAssocs
                    .FirstOrDefault(y => y.NotificationCategoryID == categoryItem.ID && y.NotificationColumnID == x.ID) != null)
                    .ToList();

                if (subscribedIndications.Count > 0)
                {
                    foreach (var columnItem in columnsUnderCategory)
                    {
                        if (columnItem.Indication.Equals(ZhiNotificationCommon.Utilities.Constants.AllIndicationsText, StringComparison.InvariantCultureIgnoreCase)
                            || subscribedIndications
                            .FirstOrDefault(si => si.IndicationAbbreviation.Equals(columnItem.Indication, StringComparison.InvariantCultureIgnoreCase)) != null)
                        {
                            Column column = new Column();
                            column.ColumnID = columnItem.ID;
                            column.ColumnName = columnItem.DisplayName;
                            column.IsSubscribed = false;

                            if (subscribedNotyCategoryColumns != null)
                            {
                                //initial loading
                                column.IsSubscribed = subscribedNotyCategoryColumns
                                    .FirstOrDefault(sncc => sncc.NotificationCategoryID == categoryItem.ID && sncc.NotificationColumnID == columnItem.ID) != null;
                            }
                            else if (selectedCategoryColumns != null)
                            {
                                //indication selection change
                                column.IsSubscribed = selectedCategoryColumns
                                    .FirstOrDefault(x => x.NotificationCategoryID == categoryItem.ID
                                        && x.Columns.FirstOrDefault(y => y.ColumnID == columnItem.ID) != null) != null;
                            }

                            category.Columns.Add(column);
                        }
                    }
                }

                categoryColumns.Add(category);
            }

            return categoryColumns;
        }

        private List<CategoryWithColumns> GetNotificationCategoryColumnSubscriptions(int applicationId,
            List<IndicationSubscription> indicationSubscriptions,
            int? userId = null)
        {
            List<CategoryWithColumns> list = new List<CategoryWithColumns>();

            if (userId.HasValue)
            {
                List<UserNotificationCategoryColumnAssoc> existingNotyColumnSubscriptions = new List<UserNotificationCategoryColumnAssoc>();
                existingNotyColumnSubscriptions = NotificationManagementDataAccess.GetUserNotificationCategoryColumnAssociations(applicationId, userId.Value);
                list.AddRange(GetNotificationCategoryWithColumns(applicationId, userId.Value, indicationSubscriptions, existingNotyColumnSubscriptions, null));
            }
            return list;
        }

        private List<NotificationCategorySubscription> GetNotificationCategorySubscriptions(int applicationID, int? userID = null)
        {

            //For return data
            List<NotificationCategorySubscription> list = new List<NotificationCategorySubscription>();

            List<NotificationCategory> notificationCategories = NotificationManagementDataAccess.GetNotificationCategories(applicationID);
            List<UserNotificationCategoryAssoc> existingSubscriptions = new List<UserNotificationCategoryAssoc>();

            if (userID.HasValue)
            {
                existingSubscriptions = NotificationManagementDataAccess.GetUserNotificationCategoryAssociations(applicationID, userID.Value);
            }

            foreach (NotificationCategory item in notificationCategories)
            {
                NotificationCategorySubscription subscription = new NotificationCategorySubscription();
                subscription.NotificationCategoryID = item.ID;
                subscription.NotificationCategoryName = item.Name;
                subscription.IsMandatory = item.Mandatory.HasValue && item.Mandatory.Value;

                subscription.IsSubscribed = subscription.IsMandatory ||
                    (existingSubscriptions.FirstOrDefault(x => x.NotificationCategoryID == item.ID) != null);

                list.Add(subscription);
            }


            return list;
        }

        public bool SaveSubscriptionData(List<int> userIds, SubscriptionSubmissionModel subscription)
        {
            bool bRet = false;
            try
            {
                var userIdsDataTable = GenerateDataTableOfIDs("UserID", userIds);

                var notyCategoryNotyColumnDataTable = GenerateNotificationCategoryNotificationColumnDataTable(subscription.CategoriesWithColumns);

                var deliveryMethodIdsDataTable = GenerateDataTableOfIDs("DeliveryMethodID", subscription.DeliveryMethos
                    .Where(x => x.IsChecked).Select(x => x.ID).ToList());

                var indicationIdsDataTable = GenerateDataTableOfIDs("udt_IndicationID", subscription.IndicationSubscriptions
                    .Where(x => x.IsSubscribed).Select(x => x.IndicationID).ToList());

                return NotificationManagementDataAccess.SaveNotificationSubscriptions(Constants.NotificationSubscriptionsSavingCommandText,
                    userIdsDataTable,
                    notyCategoryNotyColumnDataTable,
                    deliveryMethodIdsDataTable,
                    indicationIdsDataTable,
                    subscription.ApplicationID,
                    subscription.DeliveryFrequency,
                    subscription.ContentVolume);
            }
            catch (Exception ex)
            {
                
                int moduleId = (int)ZhiNotificationCommon.Utilities.Module.NotificationApplication; // moduleId = 3
                string message = ZhiNotificationCommon.Utilities.Constants.ErrorWhileSavingSubscription;
                ZhiNotificationCommon.Utilities.LoggerHelper.WriteLogThroughCommonLogger(moduleId, message, ex);
            }
            return bRet;
        }

        public List<Change> GetChanges(int applicationID)
        {
            try
            {
                return NotificationManagementDataAccess.GetChanges(applicationID);
            }
            catch (Exception e)
            {
                
                int moduleId = (int)ZhiNotificationCommon.Utilities.Module.NotificationApplication; // moduleId = 3
                string message = ZhiNotificationCommon.Utilities.Constants.ErrorWhileGettingChanges;
                ZhiNotificationCommon.Utilities.LoggerHelper.WriteLogThroughCommonLogger(moduleId, message, e);
            }
            return new List<Change>();
        }

        private DataTable GenerateDataTableOfIDs<T>(string tableName, List<T> idsToInsert)
        {
            using (DataTable dt = new DataTable(tableName))
            {
                dt.Columns.Add("ID", typeof(T));
                idsToInsert.ForEach(f => dt.Rows.Add(f));
                return dt;
            }
        }

        private DataTable GenerateIndicationDataTable(string tableName, List<string> indications)
        {
            using (DataTable dt = new DataTable(tableName))
            {
                dt.Columns.Add("Indication", typeof(string));
                indications.ForEach(f => dt.Rows.Add(f));
                return dt;
            }
        }

        private DataTable GenerateNotificationCategoryNotificationColumnDataTable(List<CategoryWithColumns> categoryColumns)
        {
            using (DataTable dt = new DataTable("udt_NotificationCategoryNotificationColumn"))
            {
                dt.Columns.Add("NotificationCategoryID", typeof(int));
                dt.Columns.Add("NotificationColumnID", typeof(int));
                foreach (var categoryItem in categoryColumns)
                {
                    if (categoryItem.IsSubscribed)
                    {
                        foreach (var columnItem in categoryItem.Columns)
                        {
                            if (columnItem.IsSubscribed)
                            {
                                dt.Rows.Add(categoryItem.NotificationCategoryID, columnItem.ColumnID);
                            }
                        }
                    }
                }
                return dt;
            }
        }

        public List<User> GetSubscribedUsers(int notificationCategoryID, long changeId)
        {
            try
            {
                return NotificationManagementDataAccess.GetSubscribedUsers(notificationCategoryID, changeId);
            }
            catch (Exception e)
            {
                
                int moduleId = (int)ZhiNotificationCommon.Utilities.Module.NotificationApplication; // moduleId = 3
                string message = ZhiNotificationCommon.Utilities.Constants.ErrorWhileGettingSubscribedUsers;
                ZhiNotificationCommon.Utilities.LoggerHelper.WriteLogThroughCommonLogger(moduleId, message, e);
            }
            return new List<User>();
        }

        public List<NotificationDeliveryLogViewModel> GetNotificationDeliveryLogs()
        {
            var notificationDeliveryLogs = LogDataAccess.GetNotificationDeliveryLog();
            var notificationDeliveryLogViewModels = new List<NotificationDeliveryLogViewModel>();            
            var userIds = new List<int>();
            var users = new List<User>();

            notificationDeliveryLogs.ForEach(x => userIds.Add(x.UserNotificationAssoc.UserID));
            users = AccountDataAccess.GetUsersById(userIds);
           
            foreach (var item in notificationDeliveryLogs)
            {
                var currentUser = users.FirstOrDefault(x => x.UserID == item.UserNotificationAssoc.UserID);
                var notificationDeliveryLogViewModel = new NotificationDeliveryLogViewModel();
                notificationDeliveryLogViewModel.DeliveryFrequencyValue = item.DeliveryFrequency.Value;
                notificationDeliveryLogViewModel.NotificationText = item.UserNotificationAssoc.Notification.Text;
                if (currentUser != null)
                    notificationDeliveryLogViewModel.UserName = string.Format("{0} {1}", currentUser.FirstName, currentUser.LastName);
                notificationDeliveryLogViewModel.TimeStamp = item.Timestamp;
                notificationDeliveryLogViewModels.Add(notificationDeliveryLogViewModel);
            }

            return notificationDeliveryLogViewModels;
        }
    }
}

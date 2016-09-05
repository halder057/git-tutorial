using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ZhiNotificationCommon.DataAccess.DatabaseContexts;
using ZhiNotificationCommon.DataAccess.Models;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using ZhiNotificationCommon.Models;

namespace ZhiNotificationCommon.DataAccess
{
    public class NotificationManagementDataAccess
    {
        public static List<ContentVolume> GetContentVolumes()
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.ContentVolumes.ToList();
            }
        }

        public static List<DeliveryFrequency> GetDeliveryFrequencies()
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.DeliveryFrequencies.ToList();
            }
        }

        public static List<DeliveryMethod> GetDeliveryMethods()
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.DeliveryMethods.ToList();
            }
        }

        public static List<UserNotificationDeliveryAssoc> GetUserNotificationDeliveryAssocs(int applicationID, int userID)
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.UserNotificationDeliveryAssocs
                    .Where(und => und.UserID == userID && und.ApplicationID == applicationID)
                    .ToList();
            }
        }

        public static List<NotificationCategory> GetNotificationCategories(int applicationID)
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.NotificationCategories.Where(nc => nc.ApplicationID == applicationID).ToList();
            }
        }

        public static List<NotificationColumn> GetNotificationColumns(int applicationId, int userId, DataTable indicationDataTable)
        {
            using (var dbContext = new NotificationContext())
            {
                var user = dbContext.Users.FirstOrDefault(x => x.UserID == userId);
                if (user != null)
                {
                    var client = dbContext.Clients.FirstOrDefault(x => x.ClientID == user.ClientID);
                    if (client != null)
                    {
                        var clientName = client.Name;

                        var appNotyColumnIds = GetNotificationCategoryNotificationColumnAssocs(applicationId)
                        .Select(x => x.NotificationColumnID)
                        .ToList();
                        var appNotyColumns = dbContext.NotificationColumns.Where(x => appNotyColumnIds.Contains(x.ID)).ToList();

                        foreach (var item in ZhiNotificationCommon.Utilities.Constants.AliasToDisplayNames.Keys)
                        {
                            var customColumnNames = GetCustomColumnNames(item, clientName, indicationDataTable);
                            var displayName = ZhiNotificationCommon.Utilities.Constants.AliasToDisplayNames[item];

                            var columnsToBeExcludedForCurrentAlias = appNotyColumns.Where(x => x.DisplayName.Equals(displayName, StringComparison.InvariantCultureIgnoreCase)
                                && customColumnNames.FirstOrDefault(y => y.Equals(x.ColumnName, StringComparison.InvariantCultureIgnoreCase)) == null).ToList();

                            columnsToBeExcludedForCurrentAlias.ForEach(x => appNotyColumns.Remove(x));
                        }

                        return appNotyColumns;
                    }
                }

            }
            return new List<NotificationColumn>();
        }

        public static List<NotificationCategoryNotificationColumnAssoc> GetNotificationCategoryNotificationColumnAssocs(int applicationId)
        {
            using (var dbContext = new NotificationContext())
            {
                var appNotificationCategoryIds = dbContext.NotificationCategories
                    .Where(nc => nc.ApplicationID == applicationId)
                    .Select(n => n.ID)
                    .ToList();

                var appNotyCategoryColumnAssocs = dbContext.NotificationCategoryNotificationColumnAssocs
                    .Where(ncnca => appNotificationCategoryIds.Contains(ncnca.NotificationCategoryID))
                    .ToList();

                return appNotyCategoryColumnAssocs;
            }
        }

        public static List<UserNotificationCategoryAssoc> GetUserNotificationCategoryAssociations(int applicationID, int userID)
        {
            using (var dbContext = new NotificationContext())
            {
                var userAssocs = dbContext.UserNotificationCategoryAssocs
                    .Include(unc => unc.NotificationCategory)
                    .Where(unc => unc.NotificationCategory.ApplicationID == applicationID && unc.UserID == userID)
                    .ToList();
                return userAssocs;
            }
        }

        public static List<UserNotificationCategoryColumnAssoc> GetUserNotificationCategoryColumnAssociations(int applicationId, int userId)
        {
            using (var dbContext = new NotificationContext())
            {
                var userNotyCategoryColumnAssocs = dbContext.UserNotificationCategoryColumnAssocs
                    .Where(uncca => uncca.UserID == userId)
                    .ToList();

                var appNotificationCategoryIds = dbContext.NotificationCategories
                    .Where(nc => nc.ApplicationID == applicationId)
                    .Select(n => n.ID)
                    .ToList();

                var appUserNotyCategoryColumnAssocs = userNotyCategoryColumnAssocs
                    .Where(uncca => appNotificationCategoryIds
                        .FirstOrDefault(anci => anci == uncca.NotificationCategoryID) != null).ToList();
                return userNotyCategoryColumnAssocs;
            }
        }

        public static List<UserIndicationAssoc> GetUserIndicationAssociations(int userId)
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.UserIndicationAssocs.Where(x => x.UserID == userId).ToList();
            }
        }

        public static List<Change> GetChanges(int applicationID, bool includeCategory = true, bool includeStatus = true)
        {
            using (var dbContext = new NotificationContext())
            {
                IQueryable<Change> changes = dbContext.Changes;
                if (includeCategory)
                {
                    changes = changes.Include(c => c.NotificationCategory);
                }
                if (includeStatus)
                {
                    changes = changes.Include(c => c.ChangeStatus);
                }
                var resultList = changes.Where(c => c.NotificationCategory.ApplicationID == applicationID && c.NotificationCategory.AutoApproval == false).ToList();
                return resultList;
            }
        }

        public static bool SetUserNotificationsAsDelivered(List<long> userNotyAssocIds)
        {
            using (var dbContext = new NotificationContext())
            {
                bool bRet = false;
                foreach (var item in userNotyAssocIds)
                {
                    var dbItem = dbContext.UserNotificationAssocs.FirstOrDefault(una => una.ID == item);
                    if (dbItem != null)
                        dbItem.Status = (int)Utilities.NotificationStatus.Delivered;
                }
                dbContext.SaveChanges();
                bRet = true;
                return bRet;
            }
        }

        public static bool UpdateUserNotificationDeliveryTimestamp(List<long> userNotyDeliveryAssocIds)
        {
            using (var dbContext = new NotificationContext())
            {
                bool bRet = false;
                foreach (var item in userNotyDeliveryAssocIds)
                {
                    var dbItem = dbContext.UserNotificationDeliveryAssocs.FirstOrDefault(unda => unda.ID == item);
                    if (dbItem != null)
                        dbItem.LastDeliveryTimestamp = DateTime.Now;
                }
                dbContext.SaveChanges();
                bRet = true;
                return bRet;
            }
        }

        public static List<Change> GetChanges(List<long> ids)
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.Changes.Where(c => ids.Contains(c.ID)).ToList();
            }
        }

        public static List<Notification> InsertNotifications(List<Notification> notifications)
        {
            using (var dbContext = new NotificationContext())
            {
                notifications.ForEach(n => dbContext.Notifications.Add(n));
                dbContext.SaveChanges();
                return notifications;
            }
        }

        public static void UpdateChanges(List<Change> updatedChanges)
        {
            using (var dbContext = new NotificationContext())
            {
                updatedChanges.ForEach(uc => dbContext.Entry(dbContext.Changes.Find(uc.ID)).CurrentValues.SetValues(uc));
                dbContext.SaveChanges();
            }
        }

        public static List<UserIndication> GetUserIndications(int userId)
        {
            List<UserIndication> userIndications = new List<UserIndication>();

            using (var dbContext = new NotificationContext())
            {
                dbContext.Database.Initialize(force: false);
                var cmd = dbContext.Database.Connection.CreateCommand();
                cmd.CommandText = Utilities.Constants.UserWiseIndicationsRetrievalCommandText;
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter userIdParam = new SqlParameter("@UserID", userId);
                cmd.Parameters.Add(userIdParam);

                dbContext.Database.Connection.Open();
                var reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    userIndications = ((IObjectContextAdapter)dbContext)
                        .ObjectContext
                        .Translate<UserIndication>(reader).ToList();
                }
            }
            return userIndications;
        }

        public static bool SaveNotificationSubscriptions(string commandText,
            DataTable userIds,
            DataTable notificationCategoryNotificationColumns,
            DataTable deliveryMethodIds,
            DataTable indicationIds,
            int applicationId,
            int? deliveryFrequency = null,
            int? contentVolume = null)
        {
            using (var dbContext = new NotificationContext())
            {
                try
                {
                    dbContext.Database.Initialize(force: false);
                    var cmd = dbContext.Database.Connection.CreateCommand();
                    cmd.CommandText = commandText;
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter userIdsparam = new SqlParameter("@UserIDs", userIds);
                    SqlParameter notyCategoryIdsParam = new SqlParameter("@NotificationCategoryNotificationColumns", notificationCategoryNotificationColumns);
                    SqlParameter deliveryMethodIdsParam = new SqlParameter("@DeliveryMethodIDs", deliveryMethodIds);
                    SqlParameter indicationIdsParam = new SqlParameter("@IndicationIDs", indicationIds);
                    SqlParameter appIdParam = new SqlParameter("@AppID", applicationId);
                    cmd.Parameters.Add(userIdsparam);
                    cmd.Parameters.Add(notyCategoryIdsParam);
                    cmd.Parameters.Add(deliveryMethodIdsParam);
                    cmd.Parameters.Add(indicationIdsParam);
                    cmd.Parameters.Add(appIdParam);

                    if (deliveryFrequency.HasValue)
                    {
                        SqlParameter deliveryFrequencyParam = new SqlParameter("@DeliveryFrequency", deliveryFrequency.Value);
                        cmd.Parameters.Add(deliveryFrequencyParam);
                    }

                    if (contentVolume.HasValue)
                    {
                        SqlParameter contentVolumeParam = new SqlParameter("@ContentVolume", contentVolume.Value);
                        cmd.Parameters.Add(contentVolumeParam);
                    }

                    dbContext.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();
                    return true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public static List<User> GetSubscribedUsers(int notificationCategoryID, long changeId)
        {
            using (var dbContext = new NotificationContext())
            {
                List<User> filteredUsers = new List<User>();
                try
                {
                    dbContext.Database.Initialize(force: false);
                    var cmd = dbContext.Database.Connection.CreateCommand();
                    cmd.CommandText = "[usp_GetPattUsersAssociatedWithChange]";
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter changeIdParam = new SqlParameter("@ChangeID", changeId);
                    SqlParameter notificationCategoryIdParam = new SqlParameter("@NotificationCategoryID", notificationCategoryID);

                    cmd.Parameters.Add(changeIdParam);
                    cmd.Parameters.Add(notificationCategoryIdParam);

                    dbContext.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();

                    if (reader != null)
                    {
                        filteredUsers = ((IObjectContextAdapter)dbContext)
                            .ObjectContext
                            .Translate<User>(reader).ToList();
                    }

                }
                catch (Exception)
                {
                    throw;
                }
                return filteredUsers;
            }
        }

        public static List<UserNotificationAssoc> GetUserNotificationAsocs(int? status = null)
        {
            using (var dbContext = new NotificationContext())
            {
                try
                {
                    IQueryable<UserNotificationAssoc> unaIQueryable = dbContext.UserNotificationAssocs
                        .Include(una => una.Notification.Change.ChangeNotificationDetailAssocs.Select(x => x.NotificationDetail))
                        .Where(una => una.Status == null || una.Status == status);
                    List<UserNotificationAssoc> resultList = unaIQueryable.ToList();
                    return resultList;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public static bool GeneratePattUserNotificationAssociations(List<NotificationDb> notifications, List<ExcludedUserDb> excludedUsers, int applicationId)
        {
            using (var notyContext = new NotificationContext())
            {
                bool bRet = false;

                notyContext.Database.Initialize(force: false);
                var cmd = notyContext.Database.Connection.CreateCommand();
                cmd.CommandText = Utilities.Constants.PattNotificationAssociationGenerationCommandText;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Utilities.Constants.ConnectionTimeoutInSeconds;

                var pattNotyDrugIndicationDataTable = GetPattNotyDrugIndicationsDataTable(notifications);

                SqlParameter statusParam = new SqlParameter("@PattNotifications", pattNotyDrugIndicationDataTable);
                cmd.Parameters.Add(statusParam);

                var excludedUserDataTable = GetExcludedUsersDataTable(excludedUsers);
                cmd.Parameters.Add(new SqlParameter("@ExcludedUsers", excludedUserDataTable));

                SqlParameter appIdParam = new SqlParameter("@ApplicationID", applicationId);
                cmd.Parameters.Add(appIdParam);

                notyContext.Database.Connection.Open();
                var reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    bRet = true;
                }
                return bRet;
            }
        }

        public static List<ApproveChangesAndGenerateNotificationsResult> ApproveChangesAndGenerateNotifications(DataTable changeIds, int userId)
        {
            List<ApproveChangesAndGenerateNotificationsResult> result = new List<ApproveChangesAndGenerateNotificationsResult>();

            using (var notyContext = new NotificationContext())
            {
                notyContext.Database.Initialize(force: false);
                var cmd = notyContext.Database.Connection.CreateCommand();
                cmd.CommandText = Utilities.Constants.ApproveChangesAndGenerateNotificationsCommandText;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Utilities.Constants.ConnectionTimeoutInSeconds;

                SqlParameter statusParam = new SqlParameter("@ChangeIDs", changeIds);
                cmd.Parameters.Add(statusParam);

                cmd.Parameters.Add(new SqlParameter("@ApprovedBy", userId));

                notyContext.Database.Connection.Open();
                var reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    result = ((IObjectContextAdapter)notyContext)
                            .ObjectContext
                            .Translate<ApproveChangesAndGenerateNotificationsResult>(reader).ToList();
                }
            }

            return result;
        }

        public static List<User> GetUsers(List<int> userIds)
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.Users
                     .Where(x => userIds.Contains(x.UserID)).ToList();
            }
        }

        public static List<UserNotificationDeliveryAssoc> GetDeliveryAssocs(List<int> userIds)
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.UserNotificationDeliveryAssocs
                    .Where(x => userIds.Contains(x.UserID))
                    .ToList();
            }
        }

        public static List<NotificationTemplateMessage> GetNotificationTemplateMessages(List<int> categoryIds)
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.NotificationTemplateMessages
                    .Where(x => x.Active == true && categoryIds.Contains(x.NotificationCategoryID))
                    .GroupBy(x => x.NotificationCategoryID)
                    .Select(g => g.FirstOrDefault()).ToList();
            }
        }

        public static List<string> GetCustomColumnNames(string fieldName, string clientName, DataTable indicationDataTable)
        {
            var customColumnNames = new List<string>();

            using (var dbContext = new NotificationContext())
            {
                try
                {
                    dbContext.Database.Initialize(force: false);
                    var cmd = dbContext.Database.Connection.CreateCommand();
                    cmd.CommandText = "[usp_GetCustomNotificationColumnNames]";
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter aliasNameParam = new SqlParameter("@aliasName", fieldName);
                    SqlParameter clientNameParam = new SqlParameter("@client", clientName);
                    SqlParameter indicationDataTableParam = new SqlParameter("@indicationAbbreviations", indicationDataTable);

                    cmd.Parameters.Add(aliasNameParam);
                    cmd.Parameters.Add(clientNameParam);
                    cmd.Parameters.Add(indicationDataTableParam);

                    dbContext.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();

                    if (reader != null)
                    {
                        customColumnNames = ((IObjectContextAdapter)dbContext)
                            .ObjectContext
                            .Translate<string>(reader).ToList();
                    }

                }
                catch (Exception)
                {
                    throw;
                }
            }
            return customColumnNames;
        }

        public static List<PlanDetailNotAvailableMessage> GetPlanDetailNotAvailableMessages()
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.PlanDetailNotAvailableMessages.ToList();
            }
        }

        public static List<NotificationEmailTemplateColumnOrder> GetNotificationEmailTemplateColumnOrders()
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.NotificationEmailTemplateColumnOrders.ToList();
            }
        }

        public static List<EligibleDomain> GetEligibleDomains()
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.EligibleDomains.ToList();
            }
        }

        public static List<EligibleEmailAddress> GetEligibleEmailAddresses()
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.EligibleEmailAddresses.ToList();
            }
        }

        private static DataTable GetExcludedUsersDataTable(List<ExcludedUserDb> excludedUsers)
        {
            using (DataTable dt = new DataTable("ExcludedUser"))
            {
                dt.Columns.Add("ChangeID", typeof(long));
                dt.Columns.Add("UserID", typeof(int));
                excludedUsers.ForEach(f => dt.Rows.Add(f.ChangeID, f.UserID));
                return dt;
            }
        }

        private static DataTable GetPattNotyDrugIndicationsDataTable(List<NotificationDb> pattNotifications)
        {
            using (DataTable dt = new DataTable("PattNotification"))
            {
                dt.Columns.Add("NotificationID", typeof(long));
                dt.Columns.Add("NotificationCategoryID", typeof(int));
                dt.Columns.Add("ChangeID", typeof(long));
                pattNotifications.ForEach(f => dt.Rows.Add(f.NotificationID, f.NotificationCategoryID, f.ChangeID));
                return dt;
            }
        }
    }
}

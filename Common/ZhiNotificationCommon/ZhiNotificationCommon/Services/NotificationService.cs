using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiNotificationCommon.DataAccess.Models;
using ZhiNotificationCommon.DataAccess.Mappings;
using ZhiNotificationCommon.DataAccess.DatabaseContexts;
using ZhiNotificationCommon.Models;
using ZhiNotificationCommon.Utilities;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using ZhiNotificationCommon.DataAccess;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Configuration;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;
using System.Web;

namespace ZhiNotificationCommon.Services
{
    public class NotificationService
    {
        #region Core Functionalities
        public static bool DeliverPendingEmailNotifications(bool isRealTime = false)
        {

            var appIds = DataAccess.CommonDataAccess.GetApplications().Select(x => x.AppID);
            foreach (var item in appIds)
            {
                switch (item)
                {
                    case (int)ZhiNotificationCommon.Utilities.Application.PATT:
                        var deliveryResult = DeliverPattEmailNotifications(item, isRealTime);
                        if (!deliveryResult.Status)
                        {
                            return false;
                        }
                        else
                        {
                            if (!SetUserNotificationsAsDelivered(deliveryResult.UserNotificationAssociationIds))
                            {
                                return false;
                            }
                            else if (!UpdateEmailDeliveryTimestamp(deliveryResult.UserNotificationDeliveryAssociationIds))
                            {
                                return false;
                            }
                            SaveNotificationDeliveryLog(deliveryResult.UserDeliveryLogDetails);
                        }
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        private static NotificationDeliveryResult DeliverPattEmailNotifications(int appId, bool isRealTime = false)
        {
            NotificationDeliveryResult result = new NotificationDeliveryResult();
            result.Status = true;

            List<long> userNotificationAssocIds = new List<long>();
            List<long> userNotificationDeliveryAssocIds = new List<long>();
            result.UserDeliveryLogDetails = new List<UserDeliveryLogDetail>();

            try
            {
                List<UserNotificationAssoc> userNotificationAsocs = NotificationManagementDataAccess.
                    GetUserNotificationAsocs((int)ZhiNotificationCommon.Utilities.NotificationStatus.Pending);
                //Get distinct users
                List<int> userIds = userNotificationAsocs.Select(x => x.UserID).Distinct().ToList();

                List<User> users = NotificationManagementDataAccess.GetUsers(userIds);
                List<UserNotificationDeliveryAssoc> pattNotificationDeliveries = NotificationManagementDataAccess
                    .GetDeliveryAssocs(userIds);

                foreach (User user in users)
                {
                    var currentUserFilteredAssocs = userNotificationAsocs.Where(x => x.UserID == user.UserID).ToList();

                    List<NotificationDetailObject> filterableList = GetNotificationDetails(currentUserFilteredAssocs);

                    filterableList = filterableList
                        .OrderBy(o => o.DetailProperties["Drug"])
                        .ThenBy(o => o.DetailProperties["TA"])
                        .ThenBy(o => o.DetailProperties[Constants.FiledNameProperty])
                        .ToList();

                    filterableList = GetFilteredCombinedNotifications(filterableList);

                    var userDeliveryDetails = pattNotificationDeliveries
                        .Where(x => x.ApplicationID == appId && x.UserID == user.UserID && x.DeliveryMethod == (int)ZhiNotificationCommon.Utilities.DeliveryMethod.Email)
                        .FirstOrDefault();
                    var userDetails = user;
                    var userNotifications = filterableList;

                    if (userDetails != null && userDeliveryDetails != null && userNotifications != null && userNotifications.Count > 0)
                    {
                        if (ShouldDeliverEmailNotificationsToUser(userDeliveryDetails, isRealTime)
                            && IsUserEmailEligibleForDelivery(userDetails.EmailAddress))
                        {
                            if (ComposeAndSendPattNotificationEmail(userDetails, userDeliveryDetails, userNotifications, currentUserFilteredAssocs))
                            {
                                userNotificationAssocIds.AddRange(currentUserFilteredAssocs.Select(x => x.ID));
                                userNotificationDeliveryAssocIds.Add(userDeliveryDetails.ID);

                                result.UserDeliveryLogDetails.Add(new UserDeliveryLogDetail
                                {
                                    UserId = user.UserID,
                                    DeliveryFrequencyId = userDeliveryDetails.DeliveryFrequency.Value,
                                    UserNotificationAssocIds = currentUserFilteredAssocs.Select(x => x.ID).ToList()
                                });
                            }
                            else
                            {
                                result.Status = false;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result.Status = false;
            }
            result.UserNotificationDeliveryAssociationIds = userNotificationDeliveryAssocIds;
            result.UserNotificationAssociationIds = userNotificationAssocIds;
            return result;
        }

        private static bool ShouldDeliverEmailNotificationsToUser(UserNotificationDeliveryAssoc userDeliveryDetails, bool isRealTime = false)
        {
            if (isRealTime)
            {
                return userDeliveryDetails.DeliveryFrequency == (int)ZhiNotificationCommon.Utilities.DeliveryFrequency.RealTime;
            }
            else if (userDeliveryDetails.DeliveryFrequency == (int)ZhiNotificationCommon.Utilities.DeliveryFrequency.RealTime)
            {
                return false;
            }

            if (!userDeliveryDetails.LastDeliveryTimestamp.HasValue)
            {
                return true;
            }

            switch (userDeliveryDetails.DeliveryFrequency)
            {
                case (int)ZhiNotificationCommon.Utilities.DeliveryFrequency.Daily:
                    return (Math.Round((DateTime.Now - userDeliveryDetails.LastDeliveryTimestamp.Value).TotalHours) >= 24);
                case (int)ZhiNotificationCommon.Utilities.DeliveryFrequency.Weekly:
                    return (Math.Round((DateTime.Now - userDeliveryDetails.LastDeliveryTimestamp.Value).TotalHours) >= (24 * 7)) &&
                        (DateTime.Now.DayOfWeek == userDeliveryDetails.LastDeliveryTimestamp.Value.DayOfWeek);
                case (int)ZhiNotificationCommon.Utilities.DeliveryFrequency.Monthly:
                    return (Math.Round((DateTime.Now - userDeliveryDetails.LastDeliveryTimestamp.Value).TotalHours) >= (24 * 30)) &&
                        (DateTime.Now.Day == userDeliveryDetails.LastDeliveryTimestamp.Value.Day);
                default:
                    return false;
            }
        }

        private static bool IsUserEmailEligibleForDelivery(string email)
        {
            bool bRet = false;
            try
            {
                MailAddress mailAddress = new MailAddress(email);
                string host = mailAddress.Host;

                var eligibleEmailAddresses = NotificationManagementDataAccess.GetEligibleEmailAddresses();
                if (eligibleEmailAddresses.FirstOrDefault(x => x.EmailAddress.Equals(Constants.DomainEmailEligibilityWildCard)) != null
                    || eligibleEmailAddresses.FirstOrDefault(x => string.Equals(x.EmailAddress, email, StringComparison.InvariantCultureIgnoreCase)) != null)
                {
                    bRet = true;
                }
                else
                {
                    var eligibleDomains = NotificationManagementDataAccess.GetEligibleDomains();
                    if (eligibleDomains.FirstOrDefault(x => x.Domain.Equals(Constants.DomainEmailEligibilityWildCard)) != null
                        || eligibleDomains.FirstOrDefault(x => string.Equals(x.Domain, host, StringComparison.InvariantCultureIgnoreCase)) != null)
                    {
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                //most probable cause, malformed or empty email address
            }
            return bRet;
        }

        private static bool ComposeAndSendPattNotificationEmail(
            User userDetails,
            UserNotificationDeliveryAssoc userDeliveryDetails,
            List<NotificationDetailObject> deliverableUserNotifications,
            List<UserNotificationAssoc> currentUserFilteredAssocs)
        {
            bool bRet = false;
            try
            {
                if (deliverableUserNotifications.Count > 0)
                {
                    string email = userDetails.EmailAddress;
                    string firstName = userDetails.FirstName;
                    List<BrontoContent> brontoContents = new List<BrontoContent>();
                    if (userDeliveryDetails.ContentVolume == (int)Utilities.ContentVolume.Bundle
                        || userDeliveryDetails.DeliveryFrequency == (int)Utilities.DeliveryFrequency.RealTime)
                    {
                        List<FilteredChangesByPlanObject> viewModelList = GetFilteredChangesByPlan(
                            currentUserFilteredAssocs,
                            deliverableUserNotifications,
                            userDetails.UserID);

                        string notificationDetail = GetNotificationDetailsEmailHtmlBody(viewModelList);

                        //Generate notificationDetailsPlainText
                        string notificationDetailsPlainText = GetNotificationDetailsText(viewModelList);

                        brontoContents.Clear();
                        brontoContents.Add(new BrontoContent { Name = "FirstName", Type = Constants.BrontoEmailContentType_Html, Content = firstName });
                        brontoContents.Add(new BrontoContent { Name = "Summary", Type = Constants.BrontoEmailContentType_Html, Content = string.Empty });
                        brontoContents.Add(new BrontoContent { Name = "PdfDetail", Type = Constants.BrontoEmailContentType_Html, Content = notificationDetail });

                        brontoContents.Add(new BrontoContent { Name = "FirstNameText", Type = Constants.BrontoEmailContentType_Text, Content = firstName });
                        brontoContents.Add(new BrontoContent { Name = "SummaryText", Type = Constants.BrontoEmailContentType_Text, Content = notificationDetailsPlainText });
                        brontoContents.Add(new BrontoContent { Name = "PdfDetailText", Type = Constants.BrontoEmailContentType_Text, Content = string.Empty });

                        if (EmailHelper.SendEmailWithBronto(email, BrontoMessage.PattUpdates, brontoContents, BrontoEmailType.Transactional).errors != null)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        foreach (var duItem in deliverableUserNotifications)
                        {
                            brontoContents.Clear();
                            //Get PDF Link
                            string pdfDetail = "";

                            //Generate Summary
                            var summaryText = duItem.NotificationMessage.Replace("#-#", " - ");
                            var summaryHtml = summaryText;
                            var link = GetDocumentLinkFromNotificationMessage(duItem.NotificationMessage);
                            if (!link.Equals(Utilities.Constants.NoDocumentAvailable))
                            {
                                var messagePart = RemoveDocumentLinkFromNotificationMessage(duItem.NotificationMessage);
                                summaryHtml = messagePart + "- <a href='" + link + "'> PA Policy Document</a>";
                            }

                            brontoContents.Clear();
                            brontoContents.Add(new BrontoContent { Name = "FirstName", Type = Constants.BrontoEmailContentType_Html, Content = firstName });
                            brontoContents.Add(new BrontoContent { Name = "Summary", Type = Constants.BrontoEmailContentType_Html, Content = summaryHtml });
                            brontoContents.Add(new BrontoContent { Name = "PdfDetail", Type = Constants.BrontoEmailContentType_Html, Content = pdfDetail });

                            brontoContents.Add(new BrontoContent { Name = "FirstNameText", Type = Constants.BrontoEmailContentType_Text, Content = firstName });
                            brontoContents.Add(new BrontoContent { Name = "SummaryText", Type = Constants.BrontoEmailContentType_Text, Content = summaryText });
                            brontoContents.Add(new BrontoContent { Name = "PdfDetailText", Type = Constants.BrontoEmailContentType_Text, Content = pdfDetail });

                            if (EmailHelper.SendEmailWithBronto(email, BrontoMessage.PattUpdates, brontoContents, BrontoEmailType.Transactional).errors != null)
                            {
                                return false;
                            }
                        }
                    }
                }
                bRet = true;
            }
            catch (Exception e)
            {
                throw;
            }
            return bRet;
        }
        public static bool SetUserNotificationsAsDelivered(List<long> userNotyAssocIds)
        {
            bool bRet = false;
            try
            {
                DataAccess.NotificationManagementDataAccess.SetUserNotificationsAsDelivered(userNotyAssocIds);
                bRet = true;
            }
            catch (Exception ex)
            {

            }
            return bRet;
        }

        public static bool UpdateEmailDeliveryTimestamp(List<long> userNotyDeliveryAssocIds)
        {
            bool bRet = false;
            try
            {
                DataAccess.NotificationManagementDataAccess.UpdateUserNotificationDeliveryTimestamp(userNotyDeliveryAssocIds);
                bRet = true;
            }
            catch (Exception ex)
            {

            }
            return bRet;
        }

        #endregion

        #region Secondary Functionalities
        public static string GetDocumentLinkFromNotificationMessage(string currentMessage)
        {
            string[] messageParts = currentMessage.Split(new string[] { "#-#" }, StringSplitOptions.None);
            string fileLink = GeneratePlanDetailUrl(messageParts[1].Trim());
            return fileLink;
        }
        public static string RemoveDocumentLinkFromNotificationMessage(string currentMessage)
        {
            string[] messageParts = currentMessage.Split(new string[] { "#-#" }, StringSplitOptions.None);
            return messageParts[0].Trim();
        }

        private static string GeneratePlanDetailUrl(string fileLink)
        {
            string baseurl = ConfigurationManager.AppSettings["plandetaildownloadbaseurl"];
            if (!fileLink.Equals(Utilities.Constants.NoDocumentAvailable))
            {
                fileLink = Regex.Replace(fileLink, Regex.Escape(Constants.PlanDetailLinkSuffixToBeRemoved), string.Empty, RegexOptions.IgnoreCase)
                    .Replace("\\", "/");
                fileLink = string.Format("{0}{1}", baseurl.Trim(), fileLink.Trim());
            }
            return fileLink;
        }

        private static string GetNotificationDetailsText(List<FilteredChangesByPlanObject> filteredChangesByPlan)
        {
            StringBuilder summaryBuilder = new StringBuilder();
            foreach (FilteredChangesByPlanObject item in filteredChangesByPlan)
            {
                summaryBuilder.Append(string.Format("For '{0}'\r\n", item.PlanName));
                var datelist = item.FilteredChanges;

                foreach (FilteredDetailPropertiesByDateObject item2 in datelist)
                {
                    summaryBuilder.Append(string.Format("On '{0}', the following data changed\r\n", item2.DateString));

                    foreach (var item3 in item2.DetailProperties)
                    {
                        StringBuilder singleChangeBuilder = new StringBuilder();
                        bool hasLink = item3[Constants.IsDocumentAvailable].Equals(Constants.DocumentAvailable);
                        foreach (KeyValuePair<string, string> property in item3)
                        {
                            if (property.Key.Equals(Constants.PlanDetailLinkPropery))
                            {
                                if (hasLink)
                                {
                                    singleChangeBuilder.Append(string.Format("You can get the details from below link:\n{0}.", property.Value));
                                }
                            }
                            else
                            {
                                singleChangeBuilder.Append(string.Format("{0} = {1}. ", property.Key, property.Value));
                            }
                        }
                        summaryBuilder.Append(string.Format("{0}\r\n", singleChangeBuilder.ToString()));
                    }
                    summaryBuilder.Append("\r\n");
                }
                summaryBuilder.Append("\r\n");
            }
            return summaryBuilder.ToString();
        }

        private static List<NotificationDetailObject> GetNotificationDetails(List<UserNotificationAssoc> userNotificationAssocList)
        {
            List<NotificationDetailObject> filterableList = new List<NotificationDetailObject>();
            try
            {

                //Plan Detail NotAvailable Messages 
                List<PlanDetailNotAvailableMessage> planDetailNotAvailableMessages = NotificationManagementDataAccess.GetPlanDetailNotAvailableMessages();               
                Dictionary<string, string> FieldNameToMessage = new Dictionary<string, string>();
                foreach (PlanDetailNotAvailableMessage item in planDetailNotAvailableMessages)
                {
                    FieldNameToMessage.Add(item.FieldName, item.Message);
                }

                //Appearance Sort order
                List<NotificationEmailTemplateColumnOrder> notificationEmailTemplateColumnOrders =
                    NotificationManagementDataAccess.GetNotificationEmailTemplateColumnOrders();
                Dictionary<string, int> appearanceOrderDic = new Dictionary<string, int>();
                foreach (NotificationEmailTemplateColumnOrder item in notificationEmailTemplateColumnOrders)
                {
                    appearanceOrderDic.Add(item.FieldName, item.AppearanceOrder);
                }
                var sortedAppearanceOrder = appearanceOrderDic.OrderBy(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                sortedAppearanceOrder.Add(Constants.IsDocumentAvailable, 0);

                //Change table values
                string[] changeTableDetails = Constants.CommonNotificationTemplateFields.ToArray();

                //Get the templates
                List<int> notifcationCategoryIds = userNotificationAssocList
                    .Select(x => x.Notification.NotificationCategoryID)
                    .Distinct().ToList();
                List<NotificationTemplateMessage> templates = NotificationManagementDataAccess
                    .GetNotificationTemplateMessages(notifcationCategoryIds);

                //Extract required properties
                Dictionary<int, List<string>> categoryToProperties = new Dictionary<int, List<string>>();
                foreach (NotificationTemplateMessage template in templates)
                {
                    string templateMessage = template.Message;
                    Regex rgx = new Regex("<%\\w+%>");
                    MatchCollection matches = rgx.Matches(templateMessage);
                    List<string> properties = new List<string>();
                    foreach (Match item in matches)
                    {
                        string field = item.ToString().Replace("<%", "").Replace("%>", "").Trim();
                        properties.Add(field);
                    }
                    categoryToProperties.Add(template.NotificationCategoryID, properties);
                }
                //Generate detail property-value pair for every change
                List<Notification> notifications = userNotificationAssocList.Select(x => x.Notification).Distinct().ToList();

                foreach (Notification notification in notifications)
                {
                    Change change = notification.Change;
                    string planName = "";

                    Dictionary<string, string> properyToValue = new Dictionary<string, string>();
                    properyToValue.Add(Constants.FieldName, change.ChangedFieldName);
                    properyToValue.Add(Constants.OldValue, change.PreviousValue);
                    properyToValue.Add(Constants.NewValue, change.CurrentValue);

                    if (change.ChangeNotificationDetailAssocs != null)
                    {
                        List<NotificationDetail> notificationDetails = change
                            .ChangeNotificationDetailAssocs
                            .Select(x => x.NotificationDetail).ToList();

                        if (categoryToProperties.ContainsKey(change.NotificationCategoryID))
                        {
                            List<string> properties = categoryToProperties[change.NotificationCategoryID];
                            foreach (string property in properties)
                            {
                                if (changeTableDetails.Contains(property))
                                {
                                    continue;
                                }
                                List<string> values = notificationDetails
                                    .Where(x => x.Name.Equals(property))
                                    .Select(x => x.Value)
                                    .ToList();
                                string value = String.Join(",", values);
                                if (property.Equals(Constants.PlanNameProperty))
                                {
                                    planName = value;
                                }
                                if (property.Equals(Constants.PlanDetailLinkPropery))
                                {

                                    if (!value.Equals(Utilities.Constants.NoDocumentAvailable))
                                    {
                                        value = GeneratePlanDetailUrl(value);
                                    }
                                }
                                if (property.Equals(Constants.TherapAreaProperty))
                                {
                                    if (string.IsNullOrWhiteSpace(value))
                                    {
                                        value = notificationDetails
                                    .Where(x => x.Name.Equals(Utilities.Constants.SecondaryIndicationProperty))
                                    .Select(x => x.Value).FirstOrDefault();
                                    }
                                }
                                properyToValue.Add(property, value);
                            }
                        }

                        NotificationDetailObject obj = new NotificationDetailObject();
                        obj.ChangeID = notification.ChangeID;
                        obj.NotificationMessage = notification.Text;
                        obj.GenerationTime = notification.GenerationTime;
                        obj.ChangeGenerationTime = notification.Change.GenerationTime;
                        obj.DetailProperties = properyToValue;
                        obj.PlanName = planName;
                        obj.DetailProperties.Remove(Constants.PlanNameProperty);

                        //Update no plan detail available message
                        var link = obj.DetailProperties[Constants.PlanDetailLinkPropery];
                        var filedName = obj.DetailProperties[Constants.FiledNameProperty];
                        filedName = filedName.Trim();

                        if (link.Equals(Utilities.Constants.NoDocumentAvailable))
                        {

                            obj.DetailProperties.Add(Constants.IsDocumentAvailable, Constants.NoDocumentAvailable);

                            if (FieldNameToMessage.ContainsKey(filedName))
                            {
                                obj.DetailProperties[Constants.PlanDetailLinkPropery] = FieldNameToMessage[filedName];
                            }
                        }
                        else
                        {

                            obj.DetailProperties.Add(Constants.IsDocumentAvailable, Constants.DocumentAvailable);
                        }

                        //Sort Detail Properties

                        obj.DetailProperties = obj.DetailProperties
                            .OrderBy(x => sortedAppearanceOrder[x.Key])
                            .ToDictionary(pair => pair.Key, pair => pair.Value);

                        obj.Drug = obj.DetailProperties[Constants.Drug];
                        obj.OldValue = obj.DetailProperties[Constants.OldValue];
                        obj.NewValue = obj.DetailProperties[Constants.NewValue];
                        obj.FieldName = obj.DetailProperties[Constants.FieldName];
                        obj.TA = obj.DetailProperties[Constants.TA];
                        obj.DetailProperties.Remove(Constants.PlanName);

                        filterableList.Add(obj);
                    }
                }
            }
            catch (Exception e)
            {
            }
            return filterableList;
        }
        public static List<NotificationDetailObject> GetFilteredCombinedNotifications(List<NotificationDetailObject> filterableList)
        {

            var tempData = (from A in filterableList
                            join B in filterableList
                            on new { A.PlanName, A.FieldName, A.TA, A.Drug } equals new { B.PlanName, B.FieldName, B.TA, B.Drug }
                            where ((A.ChangeID == filterableList
                            .Where(x => x.PlanName == A.PlanName && x.FieldName == A.FieldName && x.TA == A.TA && x.Drug == A.Drug)
                            .Select(x => x.ChangeID).Min())
                                    && (B.ChangeID == filterableList
                                    .Where(x => x.PlanName == B.PlanName && x.FieldName == B.FieldName && x.TA == B.TA && x.Drug == B.Drug)
                                    .Select(x => x.ChangeID).Max())
                                )
                            select new
                            {
                                ChangeID = B.ChangeID,
                                NotificationMessage = B.NotificationMessage,
                                GenerationTime = B.GenerationTime,
                                PlanName = B.PlanName,
                                Drug = B.Drug,
                                FieldName = B.FieldName,
                                TA = B.TA,
                                OldValue = A.DetailProperties[Constants.OldValue],
                                DetailProperties = B.DetailProperties

                            }).ToList();

            tempData.ForEach(x => x.DetailProperties[Constants.OldValue] = x.OldValue);

            var filteredTempData = tempData.Where(x => (x.DetailProperties[Constants.OldValue] != x.DetailProperties[Constants.NewValue])
                || (string.IsNullOrWhiteSpace(x.DetailProperties[Constants.OldValue]) && !string.IsNullOrWhiteSpace(x.DetailProperties[Constants.NewValue]))
                || (!string.IsNullOrWhiteSpace(x.DetailProperties[Constants.OldValue]) && string.IsNullOrWhiteSpace(x.DetailProperties[Constants.NewValue]))
                ).DistinctBy(x => x.ChangeID).ToList();

            var notificationDetails = filteredTempData.Select(x => new NotificationDetailObject
            {
                ChangeID = x.ChangeID,
                NotificationMessage = x.NotificationMessage,
                GenerationTime = x.GenerationTime,
                DetailProperties = x.DetailProperties,
                PlanName = x.PlanName,
                Drug = x.Drug,
                FieldName = x.FieldName,
                TA = x.TA

            }).ToList();

            return notificationDetails;
        }
        private static List<FilteredChangesByPlanObject> GetFilteredChangesByPlan(
            List<UserNotificationAssoc> userList,
            List<NotificationDetailObject> filterableList,
            int userId)
        {
            List<FilteredChangesByPlanObject> viewModelList = new List<FilteredChangesByPlanObject>();
            List<UserNotificationAssoc> filteredAsocs = userList.Where(x => x.UserID == userId).ToList();
            List<long> changeIds = filteredAsocs.Select(x => x.Notification.ChangeID.Value).ToList();
            List<NotificationDetailObject> filteredByChangeId = filterableList.Where(x => changeIds.Contains(x.ChangeID.Value)).ToList();

            //For bundle
            List<string> planNames = filteredByChangeId.Select(x => x.PlanName).Distinct().ToList();
            foreach (string planName in planNames)
            {
                List<FilteredDetailPropertiesByDateObject> changesByDate = new List<FilteredDetailPropertiesByDateObject>();
                List<NotificationDetailObject> filteredByPlanName = filteredByChangeId.Where(x => x.PlanName.Equals(planName)).ToList();
                List<string> dates = filteredByPlanName.Select(x => x.GenerationTime.ToShortDateString()).Distinct().ToList();

                foreach (string date in dates)
                {
                    List<NotificationDetailObject> filteredByDate = filteredByPlanName
                        .Where(x => x.GenerationTime.ToShortDateString().Equals(date))
                        .ToList();

                    FilteredDetailPropertiesByDateObject changeBydateModel = new FilteredDetailPropertiesByDateObject();
                    changeBydateModel.DateString = date;
                    changeBydateModel.DetailProperties = filteredByDate.Select(x => x.DetailProperties).ToList();

                    changesByDate.Add(changeBydateModel);
                }
                FilteredChangesByPlanObject obj = new FilteredChangesByPlanObject();
                obj.PlanName = planName;
                obj.FilteredChanges = changesByDate;
                viewModelList.Add(obj);
            }
            return viewModelList;
        }

        private static string GetNotificationDetailsEmailHtmlBody(object notificationDetailsModel)
        {
            string notificationDetailEmailTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates\\NotificationDetailsEmailTemplate.cshtml");
            var templateService = new TemplateService();
            return templateService.Parse(File.ReadAllText(notificationDetailEmailTemplatePath), notificationDetailsModel, null, null);
        }

        private static void SaveNotificationDeliveryLog(List<UserDeliveryLogDetail> userDeliveryLogDetails)
        {
            try
            {
                LogDataAccess.WriteNotificationDeliveryLog(userDeliveryLogDetails);
            }
            catch (Exception ex)
            {
                int moduleId = (int)ZhiNotificationCommon.Utilities.Module.DeliveryApplication;
                ZhiNotificationCommon.Utilities.LoggerHelper.WriteLogThroughCommonLogger(moduleId ,"An error occured while saving notification delivery log" , ex);
            }
        }
        #endregion

    }

    public class NotificationDeliveryResult
    {
        public bool Status { get; set; }
        public List<long> UserNotificationAssociationIds { get; set; }
        public List<long> UserNotificationDeliveryAssociationIds { get; set; }
        public List<UserDeliveryLogDetail> UserDeliveryLogDetails { get; set; }
    }


    public class UserDeliveryLogDetail
    {
        public int UserId { get; set; }
        public List<long> UserNotificationAssocIds { get; set; }
        public int DeliveryFrequencyId { get; set; }
    }
}

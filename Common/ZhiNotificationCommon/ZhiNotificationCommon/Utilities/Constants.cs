using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.Utilities
{
    public class Constants
    {
        public const string BrontoEmailContentType_Text = "text";
        public const string BrontoEmailContentType_Html = "html";
        public const string DeliveryRecipientType_Contact = "contact";
        public const string PattNotificationAssociationGenerationCommandText = "usp_GeneratePattUserNotificationAssociations";
        public const string ApproveChangesAndGenerateNotificationsCommandText = "usp_ApproveChangesAndGenerateNotifications";
        public const string UserWiseIndicationsRetrievalCommandText = "usp_GetUserIndications";
        public const string NotificationsDeliveryDetailsRetrievalCommandText = "usp_GetNotifications";
        public static int[] SelectedAppIds = { 1, 2, 3, 6, 7 };
        public static readonly List<string> CommonNotificationTemplateFields = new List<string>() { "FieldName", "OldValue", "NewValue" };
        public const string NoDocumentAvailable = "PA Policy Document Not Applicable";
        public const string PlanNameProperty = "PlanName";
        public const string FiledNameProperty = "FieldName";
        public const string PlanDetailLinkPropery = "PlanDetailLink";
        public const string IsDocumentAvailable = "IsDocumentAvailable";
        public const string DocumentAvailable = "DocumentAvailable";
        public const string SecondaryIndicationProperty = "SecondaryTA";
        public const string TherapAreaProperty = "TA";
        public const string PlanDetailLinkSuffixToBeRemoved = @"\\tzgsrv03\PFM Files\";
        public const string RealTimeNotificationDeliveryCommandLineFlag = "realtime";
        public const string CustomColumnNameRetrievalFunctionName = "CWP.dbo.GetCustomColumnName";
        public const string AllIndicationsText = "ALL";
        public const string HealthPlanManagementFieldName = "Health plan Management";
        public const string OldValue = "OldValue";
        public const string NewValue = "NewValue";
        public const string FieldName = "FieldName";
        public const string TA = "TA";
        public const string Drug = "Drug";
        public const string PlanName = "PlanName";
        public const string DomainEmailEligibilityWildCard = "*";
        public const int ConnectionTimeoutInSeconds = 1800;
        public static readonly Dictionary<string, string> AliasToDisplayNames = new Dictionary<string, string> { 
        { "Healthplan Management" , "Health plan Management"}
        };
        public const string ErrorWhileNotifyingUser = "Error While Notifying User";
        public const string ErrorWhileUpdatingChangelist = "Error While Updating Changelist";
        public const string ErrorWhileGeneratingNotification = "Error While Generating Notification";
        public const string ErrorWhileSavingSubscription = "Error While Saving Subscription";
        public const string ErrorWhileGettingChanges = "Error While Getting Changes";
        public const string ErrorWhileGettingSubscribedUsers = "Error While Getting Subscribed Users";

        public const string ErrorWhileSendingEmail = "Error While Sending email";
    }
}

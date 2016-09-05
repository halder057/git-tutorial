using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity;
using ZhiNotificationCommon.DataAccess.Models;
using ZhiNotificationCommon.DataAccess.Mappings;
using System.Data.Entity.Infrastructure;

namespace ZhiNotificationCommon.DataAccess.DatabaseContexts
{
    public class NotificationContext : DbContext
    {
        static NotificationContext()
        {
            Database.SetInitializer<NotificationContext>(null);
        }
        public NotificationContext()
            : base("Name=ENO_Notification")
        {
            this.Configuration.LazyLoadingEnabled = false;
            
            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            objectContext.CommandTimeout = Utilities.Constants.ConnectionTimeoutInSeconds;
        }
        public DbSet<Application> Applications { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<NotificationCategory> NotificationCategories { get; set; }
        public DbSet<UserNotificationCategoryAssoc> UserNotificationCategoryAssocs { get; set; }
        public DbSet<ContentVolume> ContentVolumes { get; set; }
        public DbSet<DeliveryFrequency> DeliveryFrequencies { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<UserNotificationDeliveryAssoc> UserNotificationDeliveryAssocs { get; set; }
        public DbSet<Change> Changes { get; set; }
        public DbSet<ChangeStatus> ChangeStatuses { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationStatus> NotificationStatuses { get; set; }
        public DbSet<UserNotificationAssoc> UserNotificationAssocs { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<PreferenceType> PreferenceTypes { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<NotificationTemplateMessage> NotificationTemplateMessages { get; set; }
        public DbSet<NotificationDetail> NotificationDetails { get; set; }
        public DbSet<ChangeNotificationDetailAssoc> ChangeNotificationDetailAssocs { get; set; }
        public DbSet<UserIndicationAssoc> UserIndicationAssocs { get; set; }
        public DbSet<NotificationColumn> NotificationColumns { get; set; }
        public DbSet<NotificationCategoryNotificationColumnAssoc> NotificationCategoryNotificationColumnAssocs { get; set; }
        public DbSet<UserNotificationCategoryColumnAssoc> UserNotificationCategoryColumnAssocs { get; set; }
        public DbSet<PlanDetailNotAvailableMessage> PlanDetailNotAvailableMessages { get; set; }
        public DbSet<NotificationEmailTemplateColumnOrder> NotificationEmailTemplateColumnOrders { get; set; }
        public DbSet<EligibleDomain> EligibleDomains { get; set; }
        public DbSet<EligibleEmailAddress> EligibleEmailAddresses { get; set; }
        public DbSet<NotificationModule> NotificationModule { get; set; }
        public DbSet<NotificationEvent> NotificationEvent { get; set; }
        public DbSet<NotificationModuleNotificationEventAssoc> NotificationModuleNotificationEventAssoc { get; set; }
        public DbSet<NotificationError> NotificationError { get; set; }
        public DbSet<DeliveryLog> DeliveryLogs { get; set; }
        public DbSet<SubscriptionLevel> SubscriptionLevel { get; set; }
        public DbSet<SubscriptionChangeType> SubscriptionChangeType { get; set; }
        public DbSet<ChangeApprovalChangeType> ChangeApprovalChangeType { get; set; }
        public DbSet<SubscriptionLog> SubscriptionLog { get; set; }
        public DbSet<ChangeApprovalLog> ChangeApprovalLog { get; set; }
        public DbSet<ScanLog> ScanLog { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.Add(new ApplicationMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new NotificationCategoryMap());
            modelBuilder.Configurations.Add(new UserNotificationCategoryAssocMap());
            modelBuilder.Configurations.Add(new ContentVolumeMap());
            modelBuilder.Configurations.Add(new DeliveryFrequencyMap());
            modelBuilder.Configurations.Add(new DeliveryMethodMap());
            modelBuilder.Configurations.Add(new UserNotificationDeliveryAssocMap());
            modelBuilder.Configurations.Add(new ChangeMap());
            modelBuilder.Configurations.Add(new ChangeStatusMap());
            modelBuilder.Configurations.Add(new NotificationMap());
            modelBuilder.Configurations.Add(new NotificationStatusMap());
            modelBuilder.Configurations.Add(new UserNotificationAssocMap());
            modelBuilder.Configurations.Add(new ClientMap());
            modelBuilder.Configurations.Add(new PageMap());
            modelBuilder.Configurations.Add(new PreferenceTypeMap());
            modelBuilder.Configurations.Add(new UserPreferenceMap());
            modelBuilder.Configurations.Add(new NotificationTemplateMessageMap());
            modelBuilder.Configurations.Add(new NotificationDetailMap());
            modelBuilder.Configurations.Add(new ChangeNotificationDetailAssocMap());
            modelBuilder.Configurations.Add(new UserIndicationAssocMap());
            modelBuilder.Configurations.Add(new NotificationColumnMap());
            modelBuilder.Configurations.Add(new NotificationCategoryNotificationColumnAssocMap());
            modelBuilder.Configurations.Add(new UserNotificationCategoryColumnAssocMap());
            modelBuilder.Configurations.Add(new PlanDetailNotAvailableMessageMap());
            modelBuilder.Configurations.Add(new NotificationEmailTemplateColumnOrderMap());
            modelBuilder.Configurations.Add(new EligibleDomainMap());
            modelBuilder.Configurations.Add(new EligibleEmailAddressMap());

            modelBuilder.Configurations.Add(new NotificationModuleMap());
            modelBuilder.Configurations.Add(new NotificationEventMap());
            modelBuilder.Configurations.Add(new NotificationModuleNotificationEventAssocMap());
            modelBuilder.Configurations.Add(new NotificationErrorMap());
            modelBuilder.Configurations.Add(new DeliveryLogMap());
            modelBuilder.Configurations.Add(new SubscriptionLevelMap());
            modelBuilder.Configurations.Add(new SubscriptionChangeTypeMap());
            modelBuilder.Configurations.Add(new ChangeApprovalChangeTypeMap());
            modelBuilder.Configurations.Add(new SubscriptionLogMap());
            modelBuilder.Configurations.Add(new ChangeApprovalLogMap());
            modelBuilder.Configurations.Add(new ScanLogMap());
        }
    }
}

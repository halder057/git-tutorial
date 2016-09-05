using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;
namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class NotificationCategoryMap : EntityTypeConfiguration<NotificationCategory>
    {
        public NotificationCategoryMap()
        {
            this.HasKey(x => x.ID);
            this.ToTable("NotificationCategory");

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.Name).HasColumnName("Name");
            this.Property(x => x.Mandatory).HasColumnName("Mandatory");
            this.Property(x => x.AutoApproval).HasColumnName("AutoApproval");
            this.Property(x => x.ApplicationID).HasColumnName("ApplicationID");

            this.HasMany(nc => nc.UserNotificationCategories)
                .WithRequired(unc => unc.NotificationCategory);
            
            this.HasMany(nc => nc.Changes)
                .WithRequired(c => c.NotificationCategory);

            this.HasMany(nc => nc.Notifications)
                .WithRequired(n => n.NotificationCategory);

            this.HasMany(nc => nc.NotificationTemplateMessages)
                .WithRequired(nt => nt.NotificationCategory);
        }
    }
}
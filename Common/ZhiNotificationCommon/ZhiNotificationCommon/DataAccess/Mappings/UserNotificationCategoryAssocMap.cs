using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class UserNotificationCategoryAssocMap : EntityTypeConfiguration<UserNotificationCategoryAssoc>
    {
        public UserNotificationCategoryAssocMap()
        {
            this.HasKey(x => x.ID);
            this.ToTable("UserNotificationCategoryAssoc");

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.UserID).HasColumnName("UserID");
            this.Property(x => x.NotificationCategoryID).HasColumnName("NotificationCategoryID");


            this.HasRequired(unc => unc.NotificationCategory)
                .WithMany(nc => nc.UserNotificationCategories)
                .HasForeignKey(unc => unc.NotificationCategoryID);
        }
    }
}
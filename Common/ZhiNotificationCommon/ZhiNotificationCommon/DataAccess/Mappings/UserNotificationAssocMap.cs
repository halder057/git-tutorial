using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class UserNotificationAssocMap : EntityTypeConfiguration<UserNotificationAssoc>
    {
        public UserNotificationAssocMap()
        {
            this.ToTable("UserNotificationAssoc");
            this.HasKey(una => una.ID);

            this.Property(una => una.ID).HasColumnName("ID");
            this.Property(una => una.UserID).HasColumnName("UserID");
            this.Property(una => una.NotificationID).HasColumnName("NotificationID");
            this.Property(una => una.Status).HasColumnName("Status");
            this.Property(una => una.ApplicationID).HasColumnName("ApplicationID");

            this.HasOptional(una => una.NotificationStatus)
                .WithMany(ns => ns.UserNotificationAssocs)
                .HasForeignKey(una => una.Status);

            this.HasRequired(una => una.Notification)
                    .WithMany(n => n.UserNotificationAssocs)
                    .HasForeignKey(una => una.NotificationID);
        }
    }
}
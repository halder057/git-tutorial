using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration;

using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class NotificationMap : EntityTypeConfiguration<Notification>
    {
        public NotificationMap()
        {
            this.ToTable("Notification");
            this.HasKey(n => n.ID);

            this.Property(n => n.ID).HasColumnName("ID");
            this.Property(n => n.NotificationCategoryID).HasColumnName("NotificationCategoryID");
            this.Property(n => n.Text).HasColumnName("Text");
            this.Property(n => n.GenerationTime).HasColumnName("GenerationTime");
            this.Property(n => n.NotificationDetails).HasColumnName("NotificationDetails");
            this.Property(n => n.ChangeID).HasColumnName("ChangeID");
            this.Property(n => n.McoId).HasColumnName("MCOID");
            this.Property(n => n.IndicationAbbreviation).HasColumnName("Indication");
            this.Property(n => n.DrugName).HasColumnName("Drug_Name");
            
            this.HasMany(n => n.UserNotificationAssocs)
                .WithRequired(una => una.Notification);

            this.HasRequired(n => n.NotificationCategory)
                .WithMany(nc => nc.Notifications)
                .HasForeignKey(n => n.NotificationCategoryID);

            this.HasOptional(n => n.Change)
                .WithMany(n => n.Notifications)
                .HasForeignKey(n => n.ChangeID);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class NotificationStatusMap : EntityTypeConfiguration<NotificationStatus>
    {
        public NotificationStatusMap()
        {
            this.ToTable("NotificationStatus");
            this.HasKey(ns => ns.ID);

            this.Property(ns => ns.ID).HasColumnName("ID");
            this.Property(ns => ns.Value).HasColumnName("Value");

            this.HasMany(u => u.UserNotificationAssocs)
                .WithOptional(u => u.NotificationStatus);
        }
    }
}
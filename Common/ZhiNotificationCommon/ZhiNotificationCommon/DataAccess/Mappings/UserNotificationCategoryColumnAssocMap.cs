﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class UserNotificationCategoryColumnAssocMap : EntityTypeConfiguration<UserNotificationCategoryColumnAssoc>
    {
        public UserNotificationCategoryColumnAssocMap()
        {
            this.ToTable("UserNotificationCategoryColumnAssoc");
            this.HasKey(x => x.ID);

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.UserID).HasColumnName("UserID");
            this.Property(x => x.NotificationCategoryID).HasColumnName("NotificationCategoryID");
            this.Property(x => x.NotificationColumnID).HasColumnName("NotificationColumnID");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class UserNotificationDeliveryAssocMap : EntityTypeConfiguration<UserNotificationDeliveryAssoc>
    {
        public UserNotificationDeliveryAssocMap()
        {
            this.HasKey(x => x.ID);
            this.ToTable("UserNotificationDeliveryAssoc");

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.UserID).HasColumnName("UserID");
            this.Property(x => x.DeliveryMethod).HasColumnName("DeliveryMethod");
            this.Property(x => x.DeliveryFrequency).HasColumnName("DeliveryFrequency");
            this.Property(x => x.ContentVolume).HasColumnName("ContentVolume");
            this.Property(x => x.ApplicationID).HasColumnName("ApplicationID");
            this.Property(x => x.LastDeliveryTimestamp).HasColumnName("LastDeliveryTimestamp");
        }
    }
}

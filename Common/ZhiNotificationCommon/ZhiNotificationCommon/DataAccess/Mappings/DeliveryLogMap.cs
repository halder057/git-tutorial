using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    class DeliveryLogMap : EntityTypeConfiguration<DeliveryLog>
    {
        public DeliveryLogMap()
        {
            this.HasKey(x => x.ID);
            this.ToTable("dbo.DeliveryLog");

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.UserNotificationAssocID).HasColumnName("UserNotificationAssocID");
            this.Property(x => x.DeliveryFrequencyID).HasColumnName("DeliveryFrequencyID");
            this.Property(x => x.Timestamp).HasColumnName("Timestamp");

            this.HasRequired(dl => dl.UserNotificationAssoc)
                .WithMany(una => una.DeliveryLog)
                .HasForeignKey(dl => dl.UserNotificationAssocID);

            this.HasRequired(dl => dl.DeliveryFrequency)
                .WithMany(df => df.DeliveryLog)
                .HasForeignKey(dl => dl.DeliveryFrequencyID);
                
        }
    }
}
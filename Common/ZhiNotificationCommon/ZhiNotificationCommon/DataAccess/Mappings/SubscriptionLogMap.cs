using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    class SubscriptionLogMap : EntityTypeConfiguration<SubscriptionLog>
    {
        public SubscriptionLogMap()
        {
            this.HasKey(x => x.ID);
            this.ToTable("dbo.SubscriptionLog");

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.SubscriptionLevelID).HasColumnName("SubscriptionLevelID");
            this.Property(x => x.RowID).HasColumnName("RowID");
            this.Property(x => x.SubscriptionChangeTypeID).HasColumnName("SubscriptionChangeTypeID");
            this.Property(x => x.Timestamp).HasColumnName("Timestamp");

            this.HasRequired(sl => sl.SubscriptionLevel)
                .WithMany(sublevel => sublevel.SubscriptionLog)
                .HasForeignKey(sl => sl.SubscriptionLevelID);

            this.HasRequired(sl => sl.SubscriptionChangeType)
                .WithMany(sct => sct.SubscriptionLog)
                .HasForeignKey(sl => sl.SubscriptionChangeTypeID);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class ChangeNotificationDetailAssocMap : EntityTypeConfiguration<ChangeNotificationDetailAssoc>
    {
        public ChangeNotificationDetailAssocMap()
        {
            this.ToTable("ChangeNotificationDetailAssoc");

            this.HasKey(x => x.ID);

            this.Property(x => x.ChangeID).HasColumnName("ChangeID");
            this.Property(x => x.NotificationDetailID).HasColumnName("NotificationDetailID");

            this.HasRequired(x => x.Change)
                .WithMany(x => x.ChangeNotificationDetailAssocs)
                .HasForeignKey(x => x.ChangeID);

            this.HasRequired(x => x.NotificationDetail)
                .WithMany(x => x.ChangeNotificationDetailAssocs)
                .HasForeignKey(x => x.NotificationDetailID);
        }
    }
}

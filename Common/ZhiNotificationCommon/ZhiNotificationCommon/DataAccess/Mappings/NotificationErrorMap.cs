using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    class NotificationErrorMap : EntityTypeConfiguration<NotificationError>
    {
        public NotificationErrorMap()
        {
            this.HasKey(x => x.ID);
            this.ToTable("dbo.NotificationError");

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.ModuleID).HasColumnName("ModuleID");
            this.Property(x => x.ErrorText).HasColumnName("ErrorText");
            this.Property(x => x.StackTrace).HasColumnName("StackTrace");
            this.Property(x => x.OccurrenceTime).HasColumnName("OccurrenceTime");

            this.HasRequired(ne => ne.NotificationModule)
                .WithMany(nm => nm.NotificatioError)
                .HasForeignKey(ne => ne.ModuleID);
        }
    }
}
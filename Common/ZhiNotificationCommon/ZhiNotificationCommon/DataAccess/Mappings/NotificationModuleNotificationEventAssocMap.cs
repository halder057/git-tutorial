using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    class NotificationModuleNotificationEventAssocMap : EntityTypeConfiguration<NotificationModuleNotificationEventAssoc>
    {
        public NotificationModuleNotificationEventAssocMap()
        {
            this.HasKey(x => x.ID);
            this.ToTable("dbo.NotificationModuleNotificationEventAssoc");

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.ModuleID).HasColumnName("ModuleID");
            this.Property(x => x.EventID).HasColumnName("EventID");

            this.HasRequired(nmnea => nmnea.NotificationEvent)
                .WithMany(ne => ne.NotificationModuleNotificationEventAssoc)
                .HasForeignKey(nmnea => nmnea.EventID);

            this.HasRequired(nmnea => nmnea.NotificationModule)
                .WithMany(nm => nm.NotificationModuleNotificationEventAssoc)
                .HasForeignKey(nmnea => nmnea.ModuleID);
        }
    }
}
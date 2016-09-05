using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    class NotificationEventMap : EntityTypeConfiguration<NotificationEvent>
    {
        public NotificationEventMap()
        {
            this.HasKey(x => x.ID);
            this.ToTable("dbo.NotificationEvent");

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.EventName).HasColumnName("EventName");
        }
    }
}
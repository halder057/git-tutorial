using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class NotificationDetailMap : EntityTypeConfiguration<NotificationDetail>
    {
        public NotificationDetailMap()
        {
            this.ToTable("NotificationDetail");
            this.HasKey(x => x.ID);

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.Name).HasColumnName("Name");
            this.Property(x => x.Value).HasColumnName("Value");
        }
    }
}

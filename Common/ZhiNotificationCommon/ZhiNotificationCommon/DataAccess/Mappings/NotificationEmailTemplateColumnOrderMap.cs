using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    class NotificationEmailTemplateColumnOrderMap: EntityTypeConfiguration<NotificationEmailTemplateColumnOrder>
    {
        public NotificationEmailTemplateColumnOrderMap()
        {
            this.HasKey(x => x.ID);
            this.ToTable("NotificationEmailTemplateColumnOrder");

            this.Property(x => x.FieldName).HasColumnName("FieldName");
            this.Property(x => x.AppearanceOrder).HasColumnName("AppearanceOrder");
            this.Property(x => x.SortOrder).HasColumnName("SortOrder");
        }
    }
}

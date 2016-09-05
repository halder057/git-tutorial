using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class NotificationTemplateMessageMap : EntityTypeConfiguration<NotificationTemplateMessage>
    {
        public NotificationTemplateMessageMap()
        {
            this.ToTable("NotificationTemplateMessage");

            this.HasKey(x => x.ID);

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.NotificationCategoryID).HasColumnName("NotificationCategoryID");
            this.Property(x => x.Message).HasColumnName("Message");
            this.Property(x => x.Active).HasColumnName("Active");
            this.Property(x => x.LastUpdateTime).HasColumnName("LastUpdateTime");

            this.HasRequired(x => x.NotificationCategory)
                .WithMany(x => x.NotificationTemplateMessages)
                .HasForeignKey(x => x.NotificationCategoryID);
        }
    }
}

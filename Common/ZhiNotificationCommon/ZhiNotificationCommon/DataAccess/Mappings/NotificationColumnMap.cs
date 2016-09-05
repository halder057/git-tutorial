using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class NotificationColumnMap : EntityTypeConfiguration<NotificationColumn>
    {
        public NotificationColumnMap()
        {
            this.ToTable("NotificationColumn");

            this.HasKey(x => x.ID);

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.DisplayName).HasColumnName("Display_Name");
            this.Property(x => x.ColumnName).HasColumnName("Column_Name");
            this.Property(x => x.TableName).HasColumnName("Table_Name");
            this.Property(x => x.Indication).HasColumnName("Indication");
        }
    }
}

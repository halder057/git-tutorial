using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    class PlanDetailNotAvailableMessageMap : EntityTypeConfiguration<PlanDetailNotAvailableMessage>
    {
        public PlanDetailNotAvailableMessageMap()
        {
            this.HasKey(x => x.ID);
            this.ToTable("PlanDetailNotAvailableMessage");

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.FieldName).HasColumnName("FieldName");
            this.Property(x => x.Message).HasColumnName("Message");

        }
    }
}

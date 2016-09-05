using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    class ChangeApprovalChangeTypeMap : EntityTypeConfiguration<ChangeApprovalChangeType>
    {
        public ChangeApprovalChangeTypeMap()
        {
            this.HasKey(x => x.ID);
            this.ToTable("dbo.ChangeApprovalChangeType");

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.ChangeType).HasColumnName("ChangeType");
        }
    }
}

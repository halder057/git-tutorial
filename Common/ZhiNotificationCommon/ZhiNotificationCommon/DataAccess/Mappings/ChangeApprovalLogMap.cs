using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    class ChangeApprovalLogMap : EntityTypeConfiguration<ChangeApprovalLog>
    {
        public ChangeApprovalLogMap()
        {
            this.HasKey(x => x.ID);
            this.ToTable("dbo.ChangeApprovalLog");

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.UserID).HasColumnName("UserID");
            this.Property(x => x.ChangeID).HasColumnName("ChangeID");
            this.Property(x => x.ChangeApprovalChangeTypeID).HasColumnName("ChangeApprovalChangeTypeID");
            this.Property(x => x.Timestamp).HasColumnName("Timestamp");

            this.HasRequired(cal => cal.Change)
                .WithMany(cng => cng.ChangeApprovalLog)
                .HasForeignKey(cal => cal.ChangeID);

            this.HasRequired(cal => cal.ChangeApprovalChangeType)
                .WithMany(cact => cact.ChangeApprovalLog)
                .HasForeignKey(cal => cal.ChangeApprovalChangeTypeID);
        }
    }
}
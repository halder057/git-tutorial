using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    class ScanLogMap : EntityTypeConfiguration<ScanLog>
    {
        public ScanLogMap()
        {
            this.HasKey(x => x.ID);
            this.ToTable("dbo.ScanLog");

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.LogID).HasColumnName("LogID");
            this.Property(x => x.RowID).HasColumnName("RowID");
            this.Property(x => x.MCOID).HasColumnName("MCOID");
            this.Property(x => x.ChangeID).HasColumnName("ChangeID");
            this.Property(x => x.UpdateTime).HasColumnName("UpdateTime");
            this.Property(x => x.ProcessingTimestamp).HasColumnName("ProcessingTimestamp");

            this.HasRequired(sl => sl.Change)
                .WithMany(cng => cng.ScanLog)
                .HasForeignKey(sl => sl.ChangeID);
        }
    }
}
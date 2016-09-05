using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiPattChangeScanner.DataAccessLayer.Models;

namespace ZhiPattChangeScanner.DataAccessLayer.Mappings
{
    public class ScanHistoryMap : EntityTypeConfiguration<ScanHistory>
    {
        public ScanHistoryMap()
        {
            this.ToTable("ScanHistory");

            this.HasKey(x => x.ID);

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.ApplicationID).HasColumnName("ApplicationID");
            this.Property(x => x.LogID).HasColumnName("LogID");
            this.Property(x => x.ProcessingTimeStamp).HasColumnName("ProcessingTimeStamp");
        }
    }
}

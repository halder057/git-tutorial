using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class EligibleDomainMap : EntityTypeConfiguration<EligibleDomain>
    {
        public EligibleDomainMap()
        {
            this.ToTable("EligibleDomain");

            this.HasKey(x => x.ID);

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.Domain).HasColumnName("Domain");
        }
    }
}

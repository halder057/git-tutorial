using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class EligibleEmailAddressMap : EntityTypeConfiguration<EligibleEmailAddress>
    {
        public EligibleEmailAddressMap()
        {
            this.ToTable("EligibleEmailAddress");

            this.HasKey(x => x.ID);

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.EmailAddress).HasColumnName("EmailAddress");
        }
    }
}

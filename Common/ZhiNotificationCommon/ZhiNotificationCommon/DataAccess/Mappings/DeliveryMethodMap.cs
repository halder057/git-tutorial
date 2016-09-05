using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class DeliveryMethodMap : EntityTypeConfiguration<DeliveryMethod>
    {
        public DeliveryMethodMap()
        {
            this.HasKey(x => x.ID);
            this.ToTable("DeliveryMethod");

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.Value).HasColumnName("Value");
        }
    }
}

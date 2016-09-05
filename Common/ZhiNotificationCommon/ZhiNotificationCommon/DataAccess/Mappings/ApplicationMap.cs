using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    class ApplicationMap: EntityTypeConfiguration<Application>
    {
        public ApplicationMap()
        {
            this.HasKey(x => x.AppID);
            this.ToTable("zhi.tblApp");

            this.Property(x => x.AppID).HasColumnName("AppID");
            this.Property(x => x.AppName).HasColumnName("AppName");
            this.Property(x => x.AppCode).HasColumnName("AppCode");
        }
    }
}

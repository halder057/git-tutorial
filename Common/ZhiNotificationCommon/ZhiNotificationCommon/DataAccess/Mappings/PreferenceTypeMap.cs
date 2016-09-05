using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class PreferenceTypeMap : EntityTypeConfiguration<PreferenceType>
    {
        public PreferenceTypeMap()
        {
            this.HasKey(x => x.ID);
            this.ToTable("PreferenceType");

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.Value).HasColumnName("Value");

            this.HasMany(p => p.UserPreferences)
                .WithRequired(up => up.PreferenceType)
                .HasForeignKey(up => up.PreferenceTypeID);
        }
    }
}

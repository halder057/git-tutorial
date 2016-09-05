using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class UserPreferenceMap : EntityTypeConfiguration<UserPreference>
    {
        public UserPreferenceMap()
        {
            this.HasKey(x => x.ID);
            this.ToTable("UserPreference");

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.UserID).HasColumnName("UserID");
            this.Property(x => x.PageID).HasColumnName("PageID");
            this.Property(x => x.PreferenceTypeID).HasColumnName("PreferenceTypeID");
            this.Property(x => x.PreferenceValue).HasColumnName("PreferenceValue");

            this.HasRequired(up => up.Page)
                .WithMany(p => p.UserPreferences)
                .HasForeignKey(up => up.PageID);

            this.HasRequired(up => up.PreferenceType)
                .WithMany(pt => pt.UserPreferences)
                .HasForeignKey(up => up.PreferenceTypeID);
        }
    }
}

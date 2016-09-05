using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class PageMap : EntityTypeConfiguration<Page>
    {
        public PageMap()
        {
            this.HasKey(x => x.ID);
            this.ToTable("Page");

            this.Property(x => x.ID).HasColumnName("ID");
            this.Property(x => x.Value).HasColumnName("Value");

            this.HasMany(p => p.UserPreferences)
                .WithRequired(up => up.Page)
                .HasForeignKey(up => up.PageID);
        }
    }
}

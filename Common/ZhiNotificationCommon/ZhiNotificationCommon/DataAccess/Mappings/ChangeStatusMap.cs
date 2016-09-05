using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;
namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class ChangeStatusMap : EntityTypeConfiguration<ChangeStatus>
    {
        public ChangeStatusMap()
        {
            this.ToTable("ChangeStatus");
            this.HasKey(cs => cs.ID);

            this.Property(cs => cs.ID).HasColumnName("ID");
            this.Property(cs => cs.Value).HasColumnName("Value");

            this.HasMany(cs => cs.Changes)
                .WithOptional(pc => pc.ChangeStatus);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class UserIndicationAssocMap : EntityTypeConfiguration<UserIndicationAssoc>
    {
        public UserIndicationAssocMap()
        {
            this.HasKey(x => x.ID);

            this.Property(x => x.IndicationID).HasColumnName("IndicationID");
            this.Property(x => x.UserID).HasColumnName("UserID");
        }
    }
}

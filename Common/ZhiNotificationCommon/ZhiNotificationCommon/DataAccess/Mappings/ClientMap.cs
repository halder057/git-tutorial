using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class ClientMap : EntityTypeConfiguration<Client>
    {
        public ClientMap()
        {
            this.ToTable("Client");
            this.HasKey(x => x.ClientID);

            this.Property(x => x.ClientID).HasColumnName("ClientID");
            this.Property(x => x.ClientAbbrev).HasColumnName("ClientAbbrev");
            this.Property(x => x.Name).HasColumnName("Name");
            this.Property(x => x.Active).HasColumnName("Active");
        }
    }
}

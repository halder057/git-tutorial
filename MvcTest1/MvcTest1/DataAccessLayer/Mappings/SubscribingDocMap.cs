using SpiderDashboard.DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SpiderDashboard.DataAccessLayer.Mappings
{
    public class SubscribingDocMap : EntityTypeConfiguration<SubscribingDoc>
    {
         public SubscribingDocMap()
         {
             this.HasKey(t => t.RecentDocKey);  //primary key
             this.ToTable("vw_SpiderLog_SubscribingDocs"); // table mapping

             //Columns mappings
             this.Property(t => t.DocID).HasColumnName("DocID");
             this.Property(t => t.DrugName).HasColumnName("DrugName");
             this.Property(t => t.MCOID).HasColumnName("MCOID");
             this.Property(t => t.Plan).HasColumnName("Mco");
             this.Property(t => t.DocType).HasColumnName("DocAttributeName");
             this.Property(t => t.Analyst).HasColumnName("UserName");
             this.Property(t => t.FileWatchID).HasColumnName("FileWatchID");
             this.Property(t => t.LastUpdated).HasColumnName("LastUpdateTime");
         }
    }
}
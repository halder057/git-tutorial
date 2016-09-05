using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using ZhiNotificationCommon.DataAccess.Models;
namespace ZhiNotificationCommon.DataAccess.Mappings
{
    public class ChangeMap : EntityTypeConfiguration<Change>
    {
        public ChangeMap()
        {
            this.HasKey(c => c.ID);

            this.Property(c => c.ID).HasColumnName("ID");
            this.Property(c => c.NotificationCategoryID).HasColumnName("NotificationCategoryID");          
            this.Property(c => c.ChangedFieldName).HasColumnName("ChangedFieldName");
            this.Property(c => c.PreviousValue).HasColumnName("PreviousValue");
            this.Property(c => c.CurrentValue).HasColumnName("CurrentValue");
            this.Property(c => c.NotificationMessage).HasColumnName("NotificationMessage");
            this.Property(c => c.GenerationTime).HasColumnName("GenerationTime");
            this.Property(c => c.ChangedBy).HasColumnName("ChangedBy");
            this.Property(c => c.ApprovedBy).HasColumnName("ApprovedBy");
            this.Property(c => c.Status).HasColumnName("Status");
            this.Property(c => c.NotificationDetails).HasColumnName("NotificationDetails");
            this.Property(n => n.McoId).HasColumnName("MCOID");
            this.Property(n => n.IndicationAbbreviation).HasColumnName("Indication");
            this.Property(n => n.DrugName).HasColumnName("Drug_Name");

            this.HasRequired(c => c.NotificationCategory)
                .WithMany(nc => nc.Changes)
                .HasForeignKey(c => c.NotificationCategoryID);

            this.HasOptional(c => c.ChangeStatus)
                .WithMany(cs => cs.Changes)
                .HasForeignKey(c => c.Status);

            this.HasMany(n => n.Notifications)
                .WithOptional(n => n.Change);
        }
    }
}
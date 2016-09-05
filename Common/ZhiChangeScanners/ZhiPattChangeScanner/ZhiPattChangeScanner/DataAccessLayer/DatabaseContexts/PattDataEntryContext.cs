using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity;
using ZhiPattChangeScanner.DataAccessLayer.Models;
using ZhiPattChangeScanner.DataAccessLayer.Mappings;

namespace ZhiPattChangeScanner.DataAccessLayer.DatabaseContexts
{
    public class PattDataEntryContext : DbContext
    {
        static PattDataEntryContext()
        {
            Database.SetInitializer<PattDataEntryContext>(null);
        }

        public PattDataEntryContext()
            : base("Name=PATTDataEntry")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<ScanHistory> ScanHistories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.Add(new ScanHistoryMap());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using SpiderDashboard.DataAccessLayer.Models;
using SpiderDashboard.DataAccessLayer.Mappings;


namespace SpiderDashboard.DataAccessLayer.Contexts
{
    public class SpiderDashboardContext : DbContext
    {
        static SpiderDashboardContext()
        {
           Database.SetInitializer<SpiderDashboardContext>(null);
        }

        public SpiderDashboardContext()
            : base("Name=SpiderDashboardConnectionString") 
        {
            base.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<SubscribingDoc> SubscribingDocs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new SubscribingDocMap());
        }

    }
}
using SpiderDashboard.DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpiderDashboard.DataAccessLayer.Contexts;

namespace SpiderDashboard.DataAccessLayer.DataAccess
{
    public class SubscribingDocDataAccess
    {
        public static List<SubscribingDoc> GetSubscribingDocs()
        {
            using (var dbContext = new SpiderDashboardContext())
            {
                return dbContext.SubscribingDocs.ToList();
            }
        }
        public static List<string> GetAnalysts() 
        {
            using (var dbContext = new SpiderDashboardContext())
            {
                return dbContext.SubscribingDocs.Where(s => s.Analyst != null).Select(s => s.Analyst).Distinct().ToList();
            }
        }
        public static List<string> GetDrugs(string analyst = null) 
        {
            using (var dbContext = new SpiderDashboardContext())
            {
                if (!string.IsNullOrWhiteSpace(analyst))
                {
                    return dbContext.SubscribingDocs.Where(s => s.DrugName != null && s.Analyst == analyst).Select(s => s.DrugName).Distinct().ToList();
                }
                return dbContext.SubscribingDocs.Where(s => s.DrugName != null).Select(s => s.DrugName).Distinct().ToList();
            }
        }

        public static List<int?> GetFileWatchIds(string analyst = null, string drug = null)
        {
            using (var dbContext = new SpiderDashboardContext())
            {
                if ( !string.IsNullOrWhiteSpace(analyst) && !string.IsNullOrWhiteSpace(drug) ) 
                {
                    return dbContext.SubscribingDocs.Where(s => s.FileWatchID != null && s.Analyst == analyst && s.DrugName == drug).Select(s => s.FileWatchID).Distinct().ToList();
                }
                else if (!string.IsNullOrWhiteSpace(analyst))
                {
                    return dbContext.SubscribingDocs.Where(s => s.FileWatchID != null && s.Analyst == analyst).Select(s => s.FileWatchID).Distinct().ToList();
                }
                return dbContext.SubscribingDocs.Where(s => s.FileWatchID != null).Select(s => s.FileWatchID).Distinct().ToList();
            }
        }
        
    }
}
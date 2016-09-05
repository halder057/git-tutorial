using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderDashboard.DataAccessLayer.Models
{
    public class SubscribingDoc
    {
        public string RecentDocKey { get; set; }
        public int DocID { get; set; }
        public string DrugName { get; set; }
        public int MCOID { get; set; }
        public string Plan { get; set; }
        public string DocType { get; set; }
        public string Analyst { get; set; }
        public int? FileWatchID { get; set; }
        public DateTime? LastUpdated { get; set; }

    }
}
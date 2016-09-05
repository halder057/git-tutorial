using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiPattChangeScanner.DataAccessLayer.Models;
using ZhiPattChangeScanner.DataAccessLayer.DatabaseContexts;

namespace ZhiPattChangeScanner.DataAccessLayer.DataAccess
{
    public class ScanHistoryDataAccess
    {
        public static ScanHistory GetLatestScanHistoryEntry(int applicationId)
        {
            using (var dataEntryContext = new PattDataEntryContext())
            {
                return dataEntryContext.ScanHistories.Where(x => x.ApplicationID == applicationId)
                    .OrderByDescending(x => x.LogID).FirstOrDefault();
            }
        }
    }
}

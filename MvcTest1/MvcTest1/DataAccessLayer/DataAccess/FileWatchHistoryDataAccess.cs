using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpiderDashboard.Models;
using SpiderDashboard.DataAccessLayer.Contexts;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace SpiderDashboard.DataAccessLayer.DataAccess
{
    public class FileWatchHistoryDataAccess
    {
        public static FileHistoryAndFileSubscriptionDetails GetFileHistoryAndFileSubscriptionDetails(string analyst = null ,string drugName = null, int? fileWatchId = null ,DateTime? lastDiscoveryDate = null) 
        {
            using (var dbContext = new SpiderDashboardContext())
            {
                FileHistoryAndFileSubscriptionDetails historyAndSubscribingDetails = new FileHistoryAndFileSubscriptionDetails();
                dbContext.Database.Initialize(force: false);
                var cmd = dbContext.Database.Connection.CreateCommand();
                cmd.CommandText = Utilities.Constants.FileHistoryAndFileSubscribingDetailsRetrievalCommandText;
                cmd.CommandType = CommandType.StoredProcedure;

                if (!string.IsNullOrWhiteSpace(analyst)) 
                {
                    SqlParameter analystParam = new SqlParameter("@analyst", analyst);
                    cmd.Parameters.Add(analystParam);
                }
                if (!string.IsNullOrWhiteSpace(drugName)) 
                {
                    SqlParameter drugParam = new SqlParameter("@drugName", drugName);
                    cmd.Parameters.Add(drugParam);
                }
                if (fileWatchId.HasValue) 
                {
                    SqlParameter fileWatchParam = new SqlParameter("@fileWatchId", fileWatchId);
                    cmd.Parameters.Add(fileWatchParam);
                }
                if (lastDiscoveryDate.HasValue) 
                {
                    SqlParameter dateParam = new SqlParameter("@lastDiscoveryDate", lastDiscoveryDate);
                    cmd.Parameters.Add(dateParam);
                }

                dbContext.Database.Connection.Open();
                var reader = cmd.ExecuteReader();

                if (reader != null) 
                {
                    historyAndSubscribingDetails.FileHistoryDetails = ((IObjectContextAdapter)dbContext)
                            .ObjectContext
                            .Translate<FileHistoryDetails>(reader).ToList();

                    reader.NextResult();

                    historyAndSubscribingDetails.FileSubscriptionDetails = ((IObjectContextAdapter)dbContext)
                            .ObjectContext
                            .Translate<FileSubscriptionDetail>(reader).ToList();

                }
                historyAndSubscribingDetails.FileHistoryDetails.ForEach(f => f.FileLocation = "file:" + f.FileLocation.Replace("\\", "/"));
                return historyAndSubscribingDetails;
            }
           
        }

    }
}
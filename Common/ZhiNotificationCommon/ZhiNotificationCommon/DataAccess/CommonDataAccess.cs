using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data;
using System.Data.SqlClient;
using ZhiNotificationCommon.DataAccess.DatabaseContexts;
using ZhiNotificationCommon.DataAccess.Models;
using ZhiNotificationCommon.Models;

namespace ZhiNotificationCommon.DataAccess
{
    public class CommonDataAccess
    {
        public static List<Application> GetApplications()
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.Applications.Where(a => Utilities.Constants.SelectedAppIds.Contains(a.AppID)).ToList();
            }
        }
        public static List<UserPreference> GetPreferences(int userID, int pageID)
        {
            using (var dbContext = new NotificationContext())
            {
                var result = dbContext.UserPreferences
                    .Where(uip => uip.UserID == userID && uip.PageID == pageID);
                return result.ToList();
            }
        }

        public static void UpdatePreference(int userID, int pageID, Dictionary<int, int> preferences)
        {
            using (var dbContext = new NotificationContext())
            {

                foreach (KeyValuePair<int, int> pair in preferences)
                {
                    int prefTypeID = pair.Key;
                    int value = pair.Value;

                    //Delete previous preference, if any
                    var deletable = dbContext.UserPreferences
                        .FirstOrDefault(uip => uip.UserID == userID && uip.PageID == pageID && uip.PreferenceTypeID == prefTypeID);
                    if (deletable != null)
                    {
                        dbContext.UserPreferences.Remove(deletable);
                    }
                    //Insert a preference
                    UserPreference updatedPref = new UserPreference();
                    updatedPref.UserID = userID;
                    updatedPref.PageID = pageID;
                    updatedPref.PreferenceTypeID = prefTypeID;
                    updatedPref.PreferenceValue = value;
                    dbContext.UserPreferences.Add(updatedPref);
                }
                dbContext.SaveChanges();

            }
            
        }
    }
}

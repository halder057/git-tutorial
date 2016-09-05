using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Collections.ObjectModel;
using SpiderDashboard.DataAccessLayer.Models;
using SpiderDashboard.Utilities;

namespace SpiderDashboard.DataAccessLayer.DataAccess
{
    public class UserDataAccess
    {

        public static void IdentifyUser()
        {
            string connectionString = ConfigurationManager.ConnectionStrings[Constants.SpiderConnectionString].ConnectionString;

            Database database = new SqlDatabase(connectionString);
            DbCommand dbCommand = database.GetStoredProcCommand("dbo.usp_Identify_User");

            database.AddInParameter(dbCommand, "@UserName", DbType.String, User.WindowsName);
            database.AddInParameter(dbCommand, "@Domain", DbType.String, User.Domain);
            database.AddOutParameter(dbCommand, "@UserID", DbType.Int32, 4);
            database.AddOutParameter(dbCommand, "@AccessLevel", DbType.Int32, 0);
            database.AddOutParameter(dbCommand, "@Mode", DbType.String, 15);
            database.AddInParameter(dbCommand, "@AppVersion", DbType.String, User.Version);

            database.ExecuteScalar(dbCommand);

            User.UserID = (int)database.GetParameterValue(dbCommand, "@UserID");

        }

    }
}
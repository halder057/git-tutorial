using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace SpiderDashboard.DataAccessLayer.Models
{

    public static class User
    {
        static User()
        {
            DataAccessLayer.DataAccess.UserDataAccess.IdentifyUser();
        }

        private static int? userID = null;
        public static int? UserID { get { if (userID == null) DataAccessLayer.DataAccess.UserDataAccess.IdentifyUser(); return userID; } set { userID = value; } }
        public static string Domain { get { return Environment.UserDomainName; } }
        public static string WindowsName { get { return Environment.UserName; } }
        public static int AccessLevel { get; set; }
        public static DateTime LoginTime { get; set; }
        public static string Version { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }
        public static string Mode { get; set; }



    }


}
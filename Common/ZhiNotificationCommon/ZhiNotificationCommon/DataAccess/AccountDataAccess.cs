using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZhiNotificationCommon.DataAccess.DatabaseContexts;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotificationCommon.DataAccess
{
    public class AccountDataAccess
    {
        public static User GetUser(string username)
        {
            using (var dbContext = new NotificationContext())
            {
                IQueryable<User> users = dbContext.Users;
                return users.FirstOrDefault(u => u.UserName == username);
            }
        }
        public static List<User> GetUsersById(List<int> userIds)
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.Users
                    .Where(u => userIds.Contains(u.UserID))
                    .ToList();
            }
        }
        public static string[] GetAllRoles()
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.Users.Select(n => n.Type).Distinct().ToArray();
            }
        }

        public static string[] GetUsersInRole(string roleName)
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.Users.Where(n => n.Type == roleName).Select(n => n.UserName).ToArray();
            }
        }

        public static User GetUser(string username, string password)
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);
            }
        }

        public static List<User> GetUsers(int clientId, bool activeOnly = true)
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.Users.Where(x => (!activeOnly || x.Active) && (x.ClientID == clientId)).ToList();
            }
        }

        public static List<User> GetUsers(List<int> clientIds, bool activeOnly = true)
        {
            using (var dbContext = new NotificationContext())
            {
                return dbContext.Users.Where(x => (!activeOnly || x.Active) && clientIds.Contains(x.ClientID)).ToList();
            }
        }
    }
}
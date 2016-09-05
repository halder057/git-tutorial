using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZhiNotificationCommon.DataAccess;
using ZhiNotificationCommon.DataAccess.Models;

namespace ZhiNotification.Services
{
    public class AccountService
    {
        public bool IsInRole(User user, string role)
        {
            if (user != null && string.IsNullOrEmpty(role) == false && user.Type.Equals(role, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public User GetUser(string username)
        {
            return AccountDataAccess.GetUser(username);
        }

        public User GetUser(string username, string password)
        {
            return AccountDataAccess.GetUser(username, password);
        }

        public List<User> GetUsers(int clientId)
        {
            return AccountDataAccess.GetUsers(clientId);
        }
    }
}
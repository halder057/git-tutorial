using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using ZhiNotificationCommon.DataAccess;
using ZhiNotification.Services;

namespace ZhiNotification.MembershipInfrastructure
{
    public class CustomRoleProvider : RoleProvider
    {
        #region Declaractions

        private AccountService _accountService;

        #endregion

        public CustomRoleProvider()
        {
            _accountService = new AccountService();
        }

        public override string ApplicationName { get; set; }

        public override bool IsUserInRole(string username, string roleName)
        {
            var user = AccountDataAccess.GetUser(username);
            return _accountService.IsInRole(user, roleName);
        }

        public override string[] GetRolesForUser(string username)
        {
            var user = AccountDataAccess.GetUser(username);
            if (user != null)
            {
                return new[] { user.Type };
            }
            return null;
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            return AccountDataAccess.GetUsersInRole(roleName);
        }

        public override string[] GetAllRoles()
        {
            return AccountDataAccess.GetAllRoles();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }
    }
}
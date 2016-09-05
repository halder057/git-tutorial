using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZhiNotification.MembershipInfrastructure
{
    public class RoleNames
    {
        /// <summary>
        /// Use this to authorize user with annotations.
        /// 
        /// Example : [Authorize(Roles=RoleNames.AuthorizedUser)]
        /// </summary>
        public const string AuthorizedUser = "S,A,U";
        /// <summary>
        /// Use this to authorize user with annotations.
        /// 
        /// Example : [Authorize(Roles=RoleNames.AuthorizedAdmin)]
        /// </summary>
        public const string AuthorizedAdmin = "S,A";
        /// <summary>
        /// Use this to authorize user with annotations.
        /// 
        /// Example : [Authorize(Roles=RoleNames.AuthorizedSuperAdmin)]
        /// </summary>
        public const string AuthorizedSuperAdmin = "S";
        /// <summary>
        /// Use this to check roles against user, using the Roles static class.
        /// 
        /// Example : Roles.IsUserInRole(RoleNames.User)
        /// </summary>
        public const string User = "U";
        /// <summary>
        /// Use this to check roles against user, using the Roles static class.
        /// 
        /// Example : Roles.IsUserInRole(RoleNames.Admin)
        /// </summary>
        public const string Admin = "A";
        /// <summary>
        /// Use this to check roles against user, using the Roles static class.
        /// 
        /// Example : Roles.IsUserInRole(RoleNames.SuperAdmin)
        /// </summary>
        public const string SuperAdmin = "S";
    }
}
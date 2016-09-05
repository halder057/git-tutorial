using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZhiNotification.Models
{
    public class ExcludedUser{
        public long ChangeId { get; set; }
        public List<int> UserIds { get; set; }
    }
    public class ChangeModel
    {
        public List<long> ChangeIds{get;set;}

        public int ApplicationId {get;set;}

        public List<ExcludedUser> ExcludedUsers { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class UserPreference
    {
        public long ID { get; set; }
        public int UserID { get; set; }
        public int PageID { get; set; }
        public int PreferenceTypeID { get; set; }
        public int PreferenceValue { get; set; }

        public virtual Page Page { get; set; }
        public virtual PreferenceType PreferenceType { get; set; }
    }
}

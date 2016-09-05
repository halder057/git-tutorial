using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class Page
    {
        public int ID { get; set; }
        public string Value { get; set; }
        public virtual ICollection<UserPreference> UserPreferences { get; set; }
    }
}

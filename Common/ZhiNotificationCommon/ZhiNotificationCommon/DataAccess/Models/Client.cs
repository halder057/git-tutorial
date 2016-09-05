using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.DataAccess.Models
{
    public class Client
    {
        public int ClientID { get; set; }
        public string Name { get; set; }
        public string ClientAbbrev { get; set; }
        public bool Active { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZhiNotificationCommon.Models
{
    public class Module
    {
        public int moduleId{ get; set; }
        public string moduleName{ get; set; }
        public List<Event> moduleEvents { get; set; }
    }
}
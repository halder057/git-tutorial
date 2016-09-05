using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace ZhiNotificationCommon.Utilities
{
    public class BrontoSettings : ConfigurationSection
    {
        public static BrontoSettings GetBrontoSettings()
        {
            BrontoSettings settings = ConfigurationManager
                .GetSection("BrontoSettings") as BrontoSettings;

            if (settings != null)
                return settings;

            return new BrontoSettings();
        }

        [ConfigurationProperty("ApiKey", DefaultValue = "", IsRequired = false)]
        public string ApiKey
        {
            get { return (string)this["ApiKey"]; }
            set { this["ApiKey"] = value; }
        }

        [ConfigurationProperty("FromName", DefaultValue = "", IsRequired = false)]
        public string FromName
        {
            get { return (string)this["FromName"]; }
            set { this["FromName"] = value; }
        }

        [ConfigurationProperty("FromEmail", DefaultValue = "", IsRequired = false)]
        public string FromEmail
        {
            get { return (string)this["FromEmail"]; }
            set { this["FromEmail"] = value; }
        }
    }
}
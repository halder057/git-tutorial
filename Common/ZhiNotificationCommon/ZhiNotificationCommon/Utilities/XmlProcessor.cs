using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ZhiNotificationCommon.Utilities
{
    public class XmlProcessor
    {
        public static Dictionary<string, string> ConvertNotificationDetailsXmlToDictionary(string xmlContent)
        {
            XElement rootElement = XElement.Parse(xmlContent);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var element in rootElement.Elements())
            {
                string key = "", value = "";
                foreach (var item in element.Elements())
                {
                    if (string.Equals(item.Name.LocalName, "DetailObjectName", StringComparison.InvariantCultureIgnoreCase))
                        key = item.Value;
                    else
                        if (string.Equals(item.Name.LocalName, "DetailObjectValue", StringComparison.InvariantCultureIgnoreCase))
                            value = item.Value;
                }
                dict.Add(key, value);
            }
            return dict;
        }

        public static string ConvertNotificationDetailsDictionaryToXml(Dictionary<string, string> dict, string namespaceName)
        {
            XElement el = new XElement(namespaceName, dict.Select(kv => new XElement(kv.Key, kv.Value)));
            return el.ToString();
        }
    }
}

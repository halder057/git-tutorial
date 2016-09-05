using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZhiNotificationCommon.Utilities
{
    public class Helpers
    {
        public static DataTable GetDataTable<T>(string tableName, List<T> data)
        {
            DataTable dt = new DataTable(tableName);
            var propertyCollection = typeof(T).GetProperties();

            foreach (var item in propertyCollection)
            {
                dt.Columns.Add(item.Name, Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType);
            }

            foreach (var item in data)
            {
                object[] values = new object[propertyCollection.Length];
                for (var i = 0; i < propertyCollection.Length; i += 1)
                {
                    values[i] = item.GetType().GetProperty(propertyCollection[i].Name).GetValue(item, null);
                }
                dt.Rows.Add(values);
            }

            return dt;
        }

        public static string GetWordSeparatedString(string str)
        {
            str = Regex.Replace(str, "(\\B[A-Z])", " $1");
            return str;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace ZhiNotification.Utilities
{
    public static class HtmlHelpers
    {
        public static string MenuItemActive(this HtmlHelper helper, MenuItem menuItem, MenuItem? selectedItem)
        {
            if (selectedItem.HasValue && selectedItem.Value == menuItem)
                return "active";
            return string.Empty;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NotifyMe.Areas.Identity.Pages.Account.Manage
{
    public static class ManageNavPages
    {
        public static string PersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, "PersonalData");

        public static string AccessKeysNavClass(ViewContext viewContext) => PageNavClass(viewContext, "AccessKeys");
        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, "ChangePassword");
        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, "Index");

        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}

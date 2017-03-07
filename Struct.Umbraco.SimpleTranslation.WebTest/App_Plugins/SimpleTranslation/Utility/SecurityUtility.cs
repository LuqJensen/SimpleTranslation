using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models.Membership;
using Umbraco.Web;

namespace Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Utility
{
    public static class SecurityUtility
    {
        public static bool IsEditor(IUser user)
        {
            if (user == null)
                return false;

            //var permissions = user.UserType.Permissions; // TODO: investigate whether Permissions is more suitable for authorization.
            if (user.UserType.Alias == "admin" || user.UserType.Alias == "editor")
            {
                return true;
            }
            return false;
        }
    }
}
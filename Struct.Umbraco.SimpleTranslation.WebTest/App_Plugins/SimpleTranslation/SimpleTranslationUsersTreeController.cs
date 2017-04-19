using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Controllers.Api;
using Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Utility;
using Umbraco.Core.Security;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace Struct.Umbraco.SimpleTranslation.App_Plugins.SimpleTranslation
{
    [Tree(SECTION_ALIAS, TREE_ALIAS, TREE_ROOT_TITLE)]
    [PluginController(PLUGIN_NAME)]
    public class SimpleTranslationUsersTreeController : TreeController
    {
        private const string PLUGIN_NAME = "SimpleTranslation";
        private const string SECTION_ALIAS = "users";
        private const string TREE_ALIAS = "simpletranslation";
        private const string TREE_ROOT_TITLE = "SimpleTranslations";

        private UserSettingsController usc = new UserSettingsController();

        private string CreateRoute(string subnodeAlias, int id)
        {
            return $"{SECTION_ALIAS}/{TREE_ALIAS}/{subnodeAlias}/{id}";
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            if (usc.GetCurrentUserRole() != 1 && !SecurityUtility.IsEditor(UmbracoContext.Security.CurrentUser))
            {
                return null;
            }
            TreeNodeCollection nodes = new TreeNodeCollection();

            if (id == "-1")
            {
                foreach (var user in usc.GetUsers())
                {
                    nodes.Add(CreateTreeNode("nodeId", id, queryStrings, user.UserName, "icon-user", CreateRoute("userSettings", user.Id)));
                }
            }

            return nodes;
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            return new MenuItemCollection();
        }
    }
}
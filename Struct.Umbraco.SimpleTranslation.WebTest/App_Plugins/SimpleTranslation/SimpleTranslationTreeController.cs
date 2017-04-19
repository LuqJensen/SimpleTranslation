using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Utility;
using Umbraco.Core.Security;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Controllers.Api;

namespace Struct.Umbraco.SimpleTranslation.App_Plugins.SimpleTranslation
{
    [Tree(SECTION_ALIAS, TREE_ALIAS, TREE_ROOT_TITLE)]
    [PluginController(PLUGIN_NAME)]
    public class SimpleTranslationTreeController : TreeController
    {
        private const string PLUGIN_NAME = "SimpleTranslation";
        private const string SECTION_ALIAS = "translation";
        private const string TREE_ALIAS = "simpletranslation";
        private const string TREE_ROOT_TITLE = "SimpleTranslation";
        private const string UNREMARKABLE_BUT_NECESSARY_ID = "0";

        private UserSettingsController usc = new UserSettingsController();

        private string CreateRoute(string subnodeAlias)
        {
            return $"{SECTION_ALIAS}/{TREE_ALIAS}/{subnodeAlias}/{UNREMARKABLE_BUT_NECESSARY_ID}";
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            TreeNodeCollection nodes = new TreeNodeCollection();

            if (id == "-1" && usc.GetCurrentUserRole() > 0)
            {
                nodes.Add(CreateTreeNode("1", id, queryStrings, "Translation Tasks", "icon-folder", CreateRoute("tasks")));
                nodes.Add(CreateTreeNode("3", id, queryStrings, "Translatable strings", "icon-folder", CreateRoute("pairs")));

                if (usc.GetCurrentUserRole() == 1 && SecurityUtility.IsEditor(UmbracoContext.Security.CurrentUser))
                {
                    nodes.Add(CreateTreeNode("2", id, queryStrings, "Translation Proposals", "icon-folder", CreateRoute("proposals")));
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
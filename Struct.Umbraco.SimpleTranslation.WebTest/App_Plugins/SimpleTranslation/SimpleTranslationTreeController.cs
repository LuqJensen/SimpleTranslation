using System.Net.Http.Formatting;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace Struct.Umbraco.SimpleTranslation
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

        private string CreateRoute(string subnodeAlias)
        {
            return $"{SECTION_ALIAS}/{TREE_ALIAS}/{subnodeAlias}/{UNREMARKABLE_BUT_NECESSARY_ID}";
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            TreeNodeCollection nodes = new TreeNodeCollection();

            using (var db = DatabaseContext.Database)
            {
                var userRoleHelper = new UserRoleHelper(db);
                var userId = UmbracoContext.Security.GetUserId();

                if (id == "-1" && userRoleHelper.CanUseSimpleTranslation(userId))
                {
                    nodes.Add(CreateTreeNode("1", id, queryStrings, "Translatable strings", "icon-folder", CreateRoute("pairs")));
                    nodes.Add(CreateTreeNode("2", id, queryStrings, "Translation Tasks", "icon-folder", CreateRoute("tasks")));

                    if (userRoleHelper.IsEditor(userId))
                    {
                        nodes.Add(CreateTreeNode("3", id, queryStrings, "Translation Proposals", "icon-folder", CreateRoute("proposals")));
                    }
                }

                return nodes;
            }
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            return new MenuItemCollection();
        }
    }
}
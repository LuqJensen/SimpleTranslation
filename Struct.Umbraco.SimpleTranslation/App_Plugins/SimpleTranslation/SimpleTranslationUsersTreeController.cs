using System.Net.Http.Formatting;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace Struct.Umbraco.SimpleTranslation
{
    [Tree(SECTION_ALIAS, TREE_ALIAS, TREE_ROOT_TITLE)]
    [PluginController(PLUGIN_NAME)]
    public class SimpleTranslationUsersTreeController : TreeController
    {
        private const string PLUGIN_NAME = "SimpleTranslation";
        private const string SECTION_ALIAS = "users";
        private const string TREE_ALIAS = "simpletranslation";
        private const string TREE_ROOT_TITLE = "SimpleTranslations";

        private string CreateRoute(string subnodeAlias, int id)
        {
            return $"{SECTION_ALIAS}/{TREE_ALIAS}/{subnodeAlias}/{id}";
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            TreeNodeCollection nodes = new TreeNodeCollection();
            using (var db = DatabaseContext.Database)
            {
                var userRoleHelper = new UserRoleHelper(db);

                if (id == "-1")
                {
                    foreach (var user in userRoleHelper.GetUsers())
                    {
                        nodes.Add(CreateTreeNode("nodeId", id, queryStrings, user.UserName, "icon-user", CreateRoute("userSettings", user.Id)));
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
using System.Collections.Generic;
using Struct.Umbraco.SimpleTranslation.Models;
using Umbraco.Core.Persistence;

namespace Struct.Umbraco.SimpleTranslation.Utility
{
    public class UserRoleHelper
    {
        private Database _db;

        public UserRoleHelper(Database db)
        {
            _db = db;
        }

        public bool IsEditor(int id)
        {
            return GetUserRole(id) == (int)TranslationRoles.Editor;
        }

        public bool IsTranslator(int id)
        {
            return GetUserRole(id) == (int)TranslationRoles.Translator;
        }

        public bool CanUseSimpleTranslation(int id)
        {
            return GetUserRole(id) > (int)TranslationRoles.None;
        }

        public int GetUserRole(int id)
        {
            var user = _db.FirstOrDefault<UserRole>(new Sql().Select("*").From("dbo.simpleTranslationUserRoles").Where("id=@tag", new
            {
                tag = id
            }));

            return user?.Role ?? 0;
        }

        public List<User> GetUsers()
        {
            var users = _db.Fetch<User>(new Sql().Select("*").From("dbo.umbracoUser"));

            return users;
        }
    }
}
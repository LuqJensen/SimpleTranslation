using System.Collections.Generic;
using System.Web.Http;
using Struct.Umbraco.SimpleTranslation.Models;
using Umbraco.Core.Persistence;
using Umbraco.Web.WebApi;

namespace Struct.Umbraco.SimpleTranslation.Controllers.Api
{
    public class UserSettingsController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public object GetUser(int id)
        {
            var db = DatabaseContext.Database;
            var user = db.FirstOrDefault<UserLanguages>(new Sql().Select("*").From("dbo.umbracoUser").Where("id=@tag", new
            {
                tag = id
            }));
            var languages = db.Fetch<UserLanguage>(new Sql().Select("*").From("dbo.simpleTranslationUserLanguages").Where("id=@tag", new
            {
                tag = id
            }));

            List<int> langs = new List<int>();

            foreach (var v in languages)
            {
                langs.Add(v.LanguageId);
            }
            user.Languages = langs;

            return user;
        }

        [HttpGet]
        public object GetLanguages()
        {
            var db = DatabaseContext.Database;
            var results = db.Fetch<Language>(new Sql().Select("*").From("dbo.umbracoLanguage"));
            return results;
        }

        [HttpGet]
        public object GetRole(int id)
        {
            var userRoleHelper = new UserRoleHelper(DatabaseContext.Database);

            return userRoleHelper.GetUserRole(id);
        }

        [HttpPost]
        public void AddLanguage(int userId, int langId)
        {
            var db = DatabaseContext.Database;

            var existingData = db.FirstOrDefault<UserLanguage>(new Sql("SELECT * FROM dbo.simpleTranslationUserLanguages WHERE id=@tag1 AND languageId=@tag2", new
            {
                tag1 = userId,
                tag2 = langId
            }));

            if (existingData == null)
            {
                var data = new UserLanguage
                {
                    Id = userId,
                    LanguageId = langId
                };
                db.Insert(data);
            }
        }

        [HttpPost]
        public void RemoveLanguage(int userId, int langId)
        {
            var db = DatabaseContext.Database;

            var existingData = db.FirstOrDefault<UserLanguage>(new Sql("SELECT * FROM dbo.simpleTranslationUserLanguages WHERE id=@tag1 AND languageId=@tag2", new
            {
                tag1 = userId,
                tag2 = langId
            }));

            if (existingData != null)
            {
                db.Delete<UserLanguage>(new Sql().Where("pk=@tag", new
                {
                    tag = existingData.PrimaryKey
                }));
            }
        }

        [HttpPost]
        public void SetRole(int userId, int roleId)
        {
            var db = DatabaseContext.Database;
            var existingData = db.FirstOrDefault<UserRole>(new Sql("SELECT * FROM dbo.simpleTranslationUserRoles WHERE id=@tag", new
            {
                tag = userId,
            }));

            if (existingData != null)
            {
                existingData.Role = roleId;
                db.Save(existingData);
            }
            else
            {
                var data = new UserRole()
                {
                    Id = userId,
                    Role = roleId
                };
                db.Insert(data);
            }
        }
    }
}
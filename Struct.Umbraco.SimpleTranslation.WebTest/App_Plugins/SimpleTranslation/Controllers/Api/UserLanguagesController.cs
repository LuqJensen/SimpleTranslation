using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Models;
using Umbraco.Core.Persistence;
using Umbraco.Web.WebApi;

namespace Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Controllers.Api
{
    public class UserLanguagesController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public List<User> GetTranslationUsers()
        {
            var db = DatabaseContext.Database;
            var translators = db.Fetch<User>(new Sql().Select("u.id AS id, u.userName AS userName, u.userType AS userType, t.userTypeName as userTypeName").From("dbo.umbracoUser u INNER JOIN dbo.umbracoUserType t ON u.userType=t.id").Where("userTypeName LIKE '%Translator%'"));

            return translators;
        }

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
    }
}
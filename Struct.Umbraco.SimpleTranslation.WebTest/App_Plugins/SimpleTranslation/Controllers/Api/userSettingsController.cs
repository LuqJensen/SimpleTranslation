using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Struct.Umbraco.SimpleTranslation.Models;
using Umbraco.Core.Persistence;
using Umbraco.Web.WebApi;

namespace Struct.Umbraco.SimpleTranslation.Controllers.Api
{
    public class UserSettingsController : UmbracoAuthorizedApiController
    {
        private Database _db;
        private UserRoleHelper _urh;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db?.Dispose();
            }
            base.Dispose(disposing);
        }

        public UserSettingsController()
        {
            _db = DatabaseContext.Database;
            _urh = new UserRoleHelper(_db);
        }

        [HttpGet]
        public object GetUser(int id)
        {
            var user = _db.FirstOrDefault<UserLanguages>(new Sql().Select("*").From("dbo.umbracoUser").Where("id=@tag", new
            {
                tag = id
            }));
            var languages = _db.Fetch<UserLanguage>(new Sql().Select("*").From("dbo.simpleTranslationUserLanguages").Where("id=@tag", new
            {
                tag = id
            }));

            List<int> langs = new List<int>();

            foreach (var v in languages)
            {
                langs.Add(v.LanguageId);
            }
            user.Languages = langs;

            return new
            {
                user,
                languages = _db.Fetch<Language>(new Sql().Select("*").From("dbo.umbracoLanguage")),
                role = _urh.GetUserRole(id)
            };
        }

        [HttpPost]
        public void AddLanguages(int userId, IEnumerable<int> languages)
        {
            System.Diagnostics.Debug.WriteLine(userId);
            System.Diagnostics.Debug.Write(languages);

            if (!_urh.IsEditor(UmbracoContext.Security.GetUserId()))
                return;
        }

        [HttpPost]
        public void ChangeLanguages(SettingsDTO payload)
        {
            if (!_urh.IsEditor(UmbracoContext.Security.GetUserId()))
                return;

            foreach (var lang in payload.AddLanguages)
            {
                var existingData = _db.FirstOrDefault<UserLanguage>(new Sql().Select("*").From("dbo.simpleTranslationUserLanguages").Where("id=@tag1 AND languageId=@tag2", new
                {
                    tag1 = payload.UserId,
                    tag2 = lang
                }));

                if (existingData == null)
                {
                    var data = new UserLanguage
                    {
                        Id = payload.UserId,
                        LanguageId = lang
                    };
                    _db.Insert(data);
                }
            }

            foreach (var lang in payload.RemoveLanguages)
            {
                _db.Delete<UserLanguage>(new Sql().Where("id=@tag1 AND languageId=@tag2", new
                {
                    tag1 = payload.UserId,
                    tag2 = lang
                }));
            }
        }

        [HttpPost]
        public void SetRole(int userId, int roleId)
        {
            if (!_urh.IsEditor(UmbracoContext.Security.GetUserId()) && UmbracoContext.Security.CurrentUser.UserType.Alias == "admin" && UmbracoContext.Security.CurrentUser.UserType.Alias == "editor")
                return;

            var existingData = _db.FirstOrDefault<UserRole>(new Sql("SELECT * FROM dbo.simpleTranslationUserRoles WHERE id=@tag", new
            {
                tag = userId,
            }));

            if (existingData != null)
            {
                existingData.Role = roleId;
                _db.Save(existingData);
            }
            else
            {
                var data = new UserRole()
                {
                    Id = userId,
                    Role = roleId
                };
                _db.Insert(data);
            }
        }
    }
}
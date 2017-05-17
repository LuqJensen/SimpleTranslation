using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Struct.Umbraco.SimpleTranslation.Models;
using Struct.Umbraco.SimpleTranslation.Utility;
using Struct.Umbraco.SimpleTranslation.ViewModels;
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
        public UserSettingsView GetViewModel(int id)
        {
            var user = _db.FirstOrDefault<User>(new Sql().Select("*").From("dbo.umbracoUser").Where("id=@tag", new
            {
                tag = id
            }));


            var languages = _db.Fetch<UserLanguage>(new Sql().Select("*").From("dbo.simpleTranslationUserLanguages").Where("id=@tag", new
            {
                tag = id
            }));

            var model = new UserSettingsView
            {
                User = user,
                UserLanguages = languages.Select(x => x.LanguageId),
                Languages = _db.Fetch<Language>(new Sql().Select("*").From("dbo.umbracoLanguage")),
                UserRole = _urh.GetUserRole(id),
                Roles = TranslationRole.TranslationRoles
            };

            return model;
        }

        [HttpPost]
        public void SaveSettings(SettingsDTO payload)
        {
            var existingUserRole = _db.FirstOrDefault<UserRole>(new Sql("SELECT * FROM dbo.simpleTranslationUserRoles WHERE id=@tag", new
            {
                tag = payload.UserId
            }));

            if (existingUserRole != null)
            {
                existingUserRole.Role = payload.UserRole;
                _db.Save(existingUserRole);
            }
            else
            {
                var data = new UserRole()
                {
                    Id = payload.UserId,
                    Role = payload.UserRole
                };
                _db.Insert(data);
            }

            _db.Delete<UserLanguage>(new Sql().Where("id=@tag1", new
            {
                tag1 = payload.UserId
            }));

            if (payload.UserLanguages.Any())
            {
                _db.BulkInsertRecords(payload.UserLanguages.Select(x => new UserLanguage
                {
                    Id = payload.UserId,
                    LanguageId = x
                }));
            }
        }
    }
}
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
    public class PairsController : UmbracoAuthorizedApiController
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

        public PairsController()
        {
            _db = DatabaseContext.Database;
            _urh = new UserRoleHelper(_db);
        }

        [HttpGet]
        public PairsView GetViewModel()
        {
            var pairs = _db.Fetch<PairTranslations>(new Sql().Select("*").From("dbo.cmsDictionary"));
            var translations = GetTranslations();
            var tasks = GetTasks();

            foreach (var v in pairs)
            {
                v.TranslationTexts = translations[v.UniqueId].ToDictionary(x => x.LangId, x => x.Value);
                v.TranslationTasks = tasks[v.UniqueId].ToDictionary(x => x.LanguageId, x => true);
            }

            return new PairsView
            {
                Pairs = pairs,
                Languages = GetUserLanguages(),
                IsEditor = _urh.IsEditor(UmbracoContext.Security.GetUserId())
            };
        }

        private ILookup<Guid, TranslationText> GetTranslations()
        {
            return _db.Fetch<TranslationText>(new Sql().Select("*").From("dbo.cmsLanguageText")).ToLookup(x => x.UniqueId, x => x);
        }

        private IEnumerable<Language> GetUserLanguages()
        {
            if (_urh.IsEditor(UmbracoContext.Security.GetUserId()))
            {
                return _db.Fetch<Language>(new Sql().Select("*").From("dbo.umbracoLanguage"));
            }

            return _db.Fetch<Language>(new Sql()
                .Select("l.Id AS id, l.languageCultureName AS languageCultureName")
                .From("dbo.umbracoLanguage l LEFT OUTER JOIN dbo.simpleTranslationUserLanguages u ON l.id=u.languageId")
                .Where("u.id=@tag", new
                {
                    tag = UmbracoContext.Security.GetUserId()
                }));
        }

        private ILookup<Guid, TranslationTask> GetTasks()
        {
            return _db.Fetch<TranslationTask>(new Sql().Select("*").From("dbo.simpleTranslationTasks")).ToLookup(x => x.UniqueId, x => x);
        }

        [HttpPost]
        public void SendToTranslation(IEnumerable<TranslationTask> tasks)
        {
            if (!_urh.IsEditor(UmbracoContext.Security.GetUserId()))
                return;


            var existingData = _db.Fetch<TranslationTask>(new Sql("SELECT * FROM dbo.simpleTranslationTasks WHERE id IN (@tag1) AND languageId IN (@tag2)", new
            {
                tag1 = tasks.Select(x => x.UniqueId).Distinct(),
                tag2 = tasks.Select(x => x.LanguageId).Distinct()
            }));

            var newData = tasks.Where(x => !existingData.Any(y => x.UniqueId == y.UniqueId && x.LanguageId == y.LanguageId));

            if (newData.Any())
            {
                _db.BulkInsertRecords(newData);
            }
        }

        [HttpPost]
        public void SendToTranslationWholeLanguage(int langId)
        {
            if (!_urh.IsEditor(UmbracoContext.Security.GetUserId()))
                return;

            var keys = _db.Fetch<Pair>(new Sql().Select("id").From("dbo.cmsDictionary"));

            var existingData = _db.Fetch<TranslationTask>(new Sql("SELECT * FROM dbo.simpleTranslationTasks WHERE id IN (@tag1) AND languageId=@tag2", new
            {
                tag1 = keys.Select(x => x.UniqueId),
                tag2 = langId
            }));

            var newData = keys.Where(x => !existingData.Any(y => x.UniqueId == y.UniqueId)).Select(x => new TranslationTask
            {
                UniqueId = x.UniqueId,
                LanguageId = langId
            });

            if (newData.Any())
            {
                _db.BulkInsertRecords(newData);
            }
        }
    }
}
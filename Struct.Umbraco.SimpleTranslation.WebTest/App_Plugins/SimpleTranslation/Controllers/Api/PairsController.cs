using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Struct.Umbraco.SimpleTranslation.Models;
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
        public object GetTranslatableKeys()
        {
            var results = _db.Fetch<PairTranslations>(new Sql().Select("*").From("dbo.cmsDictionary"));

            var translations = _db.Fetch<TranslationText>(new Sql().Select("*").From("dbo.cmsLanguageText")).ToLookup(x => x.UniqueId, x => x);

            foreach (var v in results)
            {
                v.TranslationTexts = translations[v.UniqueId].ToDictionary(x => x.LangId, x => x.Value);
            }

            var subNodes = results.Where(x => x.Parent != null).ToLookup(x => x.Parent.Value, x => x);
            var rootNodes = results.Where(x => x.Parent == null);
            var pairs = BuildDictionary(rootNodes, subNodes);

            return new
            {
                pairs,
                role = _urh.GetUserRole(UmbracoContext.Security.GetUserId())
            };
        }

        [HttpGet]
        public object GetMyLanguages()
        {
            if (_urh.IsEditor(UmbracoContext.Security.GetUserId()))
            {
                return _db.Fetch<Language>(new Sql().Select("*").From("dbo.umbracoLanguage"));
            }
            if (_urh.IsTranslator(UmbracoContext.Security.GetUserId()))
            {
                return _db.Fetch<Language>(new Sql().Select("l.Id AS id, l.languageCultureName AS languageCultureName").From("dbo.umbracoLanguage l LEFT OUTER JOIN dbo.simpleTranslationUserLanguages u ON l.id=u.languageId").Where("u.id=@tag", new
                {
                    tag = UmbracoContext.Security.GetUserId()
                }));
            }
            return null;
        }

        [HttpGet]
        public object GetTranslationTasks()
        {
            var tasks = _db.Fetch<TranslationTask>(new Sql().Select("*").From("dbo.simpleTranslationTasks"));

            return tasks;
        }

        [HttpPost]
        public void SendToTranslation(IEnumerable<TranslationTask> tasks)
        {
            if (!_urh.IsEditor(UmbracoContext.Security.GetUserId()))
                return;

            foreach (var t in tasks)
            {
                var existingData = _db.FirstOrDefault<TranslationTask>(new Sql("SELECT * FROM dbo.simpleTranslationTasks WHERE id=@tag1 AND languageId=@tag2", new
                {
                    tag1 = t.UniqueId,
                    tag2 = t.LanguageId
                }));

                if (existingData == null)
                {
                    _db.Insert(t);
                }
            }
        }

        [HttpPost]
        public void SendToTranslationWholeLanguage(int langId)
        {
            if (!_urh.IsEditor(UmbracoContext.Security.GetUserId()))
                return;

            var keys = _db.Fetch<Pair>(new Sql().Select("id").From("dbo.cmsDictionary"));

            foreach (var key in keys)
            {
                var existingData = _db.FirstOrDefault<TranslationTask>(new Sql("SELECT * FROM dbo.simpleTranslationTasks WHERE id=@tag1 AND languageId=@tag2", new
                {
                    tag1 = key.UniqueId,
                    tag2 = langId
                }));

                if (existingData == null)
                {
                    var data = new TranslationTask
                    {
                        UniqueId = key.UniqueId,
                        LanguageId = langId
                    };
                    _db.Insert(data);
                }
            }
        }

        [HttpPost]
        public void CreateProposal(int langId, Guid uniqueId, string value)
        {
            if (!_urh.CanUseSimpleTranslation(UmbracoContext.Security.GetUserId()))
                return;

            _db.Insert("dbo.simpleTranslationProposals", "pk", new TranslationProposal
            {
                LanguageId = langId,
                UniqueId = uniqueId,
                UserId = UmbracoContext.Security.GetUserId(),
                Value = value,
                Timestamp = DateTime.UtcNow
            });
        }

        private Dictionary<Guid, PairTranslations> BuildDictionary(IEnumerable<PairTranslations> rootNodes, ILookup<Guid, PairTranslations> subNodes)
        {
            var nodes = new Dictionary<Guid, PairTranslations>();

            foreach (var v in rootNodes)
            {
                nodes.Add(v.UniqueId, v);
                BuildDictionary(v, subNodes);
            }
            return nodes;
        }

        private void BuildDictionary(PairTranslations currentNode, ILookup<Guid, PairTranslations> subNodes)
        {
            var children = subNodes[currentNode.UniqueId];
            if (children.Any())
            {
                currentNode.Children = new Dictionary<Guid, PairTranslations>();
                foreach (var v in children)
                {
                    currentNode.Children.Add(v.UniqueId, v);
                    BuildDictionary(v, subNodes);
                }
            }
        }
    }
}
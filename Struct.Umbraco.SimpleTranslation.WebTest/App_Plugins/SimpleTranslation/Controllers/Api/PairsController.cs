using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Models;
using Umbraco.Core.Persistence;
using Umbraco.Web;
using Umbraco.Web.WebApi;

namespace Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Controllers.Api
{
    public class PairsController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public object GetTranslatableKeys()
        {
            var db = DatabaseContext.Database;
            var results = db.Fetch<PairTranslations>(new Sql().Select("*").From("dbo.cmsDictionary"));

            var resultsLangText = db.Fetch<TranslationText>(new Sql().Select("*").From("dbo.cmsLanguageText")).ToLookup(x => x.UniqueId, x => x);
            foreach (var v in results)
            {
                v.TranslationTexts = new Dictionary<int, TranslationText>();
                foreach (var x in resultsLangText[v.UniqueId])
                {
                    v.TranslationTexts.Add(x.LangId, x);
                }
            }
            var subNodes = results.Where(x => x.Parent != null).ToLookup(x => x.Parent.Value, x => x);
            var rootNodes = results.Where(x => x.Parent == null);
            return BuildDictionary(rootNodes, subNodes);
        }

        [HttpGet]
        public object GetAllLanguages()
        {
            var db = DatabaseContext.Database;
            var results = db.Fetch<Language>(new Sql().Select("*").From("dbo.umbracoLanguage"));
            return results;
        }

        public object GetTranslatorLanguages()
        {
            var db = DatabaseContext.Database;
            var results = db.Fetch<Language>(new Sql().Select("l.Id AS id, l.languageCultureName AS languageCultureName").From("dbo.umbracoLanguage l LEFT OUTER JOIN dbo.simpleTranslationUserLanguages u ON l.id=u.languageId").Where("u.id=@tag", new
            {
                tag = UmbracoContext.Security.CurrentUser.Id
            }));
            return results;
        }

        [HttpGet]
        public object GetRole()
        {
            var db = DatabaseContext.Database;
            var user = db.FirstOrDefault<UserRole>(new Sql().Select("*").From("dbo.simpleTranslationUserRoles").Where("id=@tag", new
            {
                tag = UmbracoContext.Security.CurrentUser.Id
            }));

            return user?.Role ?? 0;
        }

        [HttpGet]
        public object GetTranslationTasks()
        {
            var db = DatabaseContext.Database;
            var tasks = db.Fetch<TranslationTask>(new Sql().Select("*").From("dbo.simpleTranslationTasks"));

            return tasks;
        }

        [HttpPost]
        public void SendToTranslation(Guid id, int langId)
        {
            var db = DatabaseContext.Database;

            var existingData = db.FirstOrDefault<TranslationTask>(new Sql("SELECT * FROM dbo.simpleTranslationTasks WHERE id=@tag1 AND languageId=@tag2", new
            {
                tag1 = id,
                tag2 = langId
            }));

            if (existingData == null)
            {
                var data = new TranslationTask
                {
                    UniqueId = id,
                    LanguageId = langId
                };
                db.Insert(data);
            }
        }

        [HttpPost]
        public void SendToTranslationWholeLanguage(int langId)
        {
            var db = DatabaseContext.Database;
            var keys = db.Fetch<Pair>(new Sql().Select("id").From("dbo.cmsDictionary"));

            foreach (var key in keys)
            {
                var existingData = db.FirstOrDefault<TranslationTask>(new Sql("SELECT * FROM dbo.simpleTranslationTasks WHERE id=@tag1 AND languageId=@tag2", new
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
                    db.Insert(data);
                }
            }
        }

        [HttpPost]
        public void CreateProposal(int langId, Guid uniqueId, string value)
        {
            var db = DatabaseContext.Database;
            db.Insert("dbo.simpleTranslationProposals", "pk", new TranslationProposal
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
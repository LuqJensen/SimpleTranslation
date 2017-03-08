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
        public object GetLanguages()
        {
            var db = DatabaseContext.Database;
            var results = db.Fetch<Language>(new Sql().Select("*").From("dbo.umbracoLanguage"));
            return results;
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
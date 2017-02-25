﻿using System;
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

        [HttpPost]
        public void SendToTranslation(int id, int langId)
        {
            var db = DatabaseContext.Database;
            var languages = db.Fetch<Language>(new Sql().Select("*").From("dbo.umbracoLanguage"));

            var task = db.FirstOrDefault<Pair>(new Sql("SELECT * FROM dbo.cmsDictionary WHERE pk=@tag", new
            {
                tag = id
            }));

            var existingData = db.FirstOrDefault<TranslationTask>(new Sql("SELECT * FROM dbo.simpleTranslationTasks WHERE id=@tag1 AND languageId=@tag2", new
            {
                tag1 = task.UniqueId,
                tag2 = langId
            }));

            if (existingData == null)
            {
                var data = new TranslationTask
                {
                    UniqueId = task.UniqueId,
                    LanguageId = langId
                };
                db.Insert(data);
            }
        }

        [HttpPost]
        public void SendToTranslationAllLanguages(int id)
        {
            var db = DatabaseContext.Database;
            var languages = db.Fetch<Language>(new Sql().Select("*").From("dbo.umbracoLanguage"));

            var task = db.FirstOrDefault<Pair>(new Sql("SELECT * FROM dbo.cmsDictionary WHERE pk=@tag", new
            {
                tag = id
            }));

            foreach (var language in languages)
            {
                var existingData = db.FirstOrDefault<TranslationTask>(new Sql("SELECT * FROM dbo.simpleTranslationTasks WHERE id=@tag1 AND languageId=@tag2", new
                {
                    tag1 = task.UniqueId,
                    tag2 = language.Id
                }));

                if (existingData == null)
                {
                    var data = new TranslationTask
                    {
                        UniqueId = task.UniqueId,
                        LanguageId = language.Id
                    };
                    db.Insert(data);
                }
            }
        }

        [HttpPost]
        public void SendToTranslationWholeLanguage(int id)
        {
            var db = DatabaseContext.Database;
            var keys = db.Fetch<PairTranslations>(new Sql().Select("*").From("dbo.cmsDictionary"));

            foreach (var key in keys)
            {
                var task = db.FirstOrDefault<Pair>(new Sql("SELECT * FROM dbo.cmsDictionary WHERE pk=@tag", new
                {
                    tag = key.PrimaryKey
                }));

                var existingData = db.FirstOrDefault<TranslationTask>(new Sql("SELECT * FROM dbo.simpleTranslationTasks WHERE id=@tag1 AND languageId=@tag2", new
                {
                    tag1 = task.UniqueId,
                    tag2 = id
                }));

                if (existingData == null)
                {
                    var data = new TranslationTask
                    {
                        UniqueId = task.UniqueId,
                        LanguageId = id
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
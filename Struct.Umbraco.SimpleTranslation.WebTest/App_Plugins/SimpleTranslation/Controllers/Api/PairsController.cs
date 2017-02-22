using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
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

            var languages = db.Fetch<Language>(new Sql().Select("*").From("dbo.umbracoLanguage"));

            var resultsLangText = db.Fetch<TranslationText>(new Sql().Select("*").From("dbo.cmsLanguageText")).ToLookup(x => x.UniqueId, x => x);
            foreach (var v in results)
            {
                v.TranslationTexts = new Dictionary<int, TranslationText>();
                foreach (var lang in languages)
                {
                    bool contains = false;
                    foreach (var x in resultsLangText[v.UniqueId])
                    {
                        if (x.LangId == lang.Id)
                        {
                            contains = true;
                            v.TranslationTexts.Add(x.LangId, x);
                            break;
                        }
                    }
                    if (!contains)
                    {
                        v.TranslationTexts.Add(lang.Id, null);
                    }
                }
//                foreach (var x in resultsLangText[v.UniqueId])
//                {
//                    v.TranslationTexts.Add(x.LangId, x.Value);
//                }
                //                foreach (var language in languages)
                //                {
                //                    foreach (var x in resultsLangText[v.UniqueId])
                //                    {
                //                        if (x.LangId == language.Id)
                //                        {
                //                            v.TranslationTexts resultsLangText[v.UniqueId]
                //                        }
                //                    }
                //                    if
                //                    .Contains()
                //                }
            }
            var subNodes = results.Where(x => x.Parent != null).ToLookup(x => x.Parent.Value, x => x);
            var rootNodes = results.Where(x => x.Parent == null);

            return BuildDictionary(rootNodes, subNodes);
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

        public object GetLanguages()
        {
            var db = DatabaseContext.Database;
            var results = db.Fetch<Language>(new Sql().Select("*").From("dbo.umbracoLanguage"));
            //            var languages = new Dictionary<int, string>();
            //            foreach (var language in results)
            //            {
            //                languages.Add(language.Id, language.LanguageCultureName);

            return results;
        }
    }
}
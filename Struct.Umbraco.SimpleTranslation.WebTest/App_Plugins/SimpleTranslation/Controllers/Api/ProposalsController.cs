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
    public class ProposalsController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public object GetTranslationProposals()
        {
            var db = DatabaseContext.Database;
            var proposals = db.Fetch<TranslationProposal>(new Sql()
                    .Select("p.*, u.userName AS userName, l.languageCultureName AS language")
                    .From("dbo.simpleTranslationProposals p LEFT OUTER JOIN dbo.umbracoLanguage l ON p.languageId=l.id LEFT OUTER JOIN dbo.umbracoUser u ON u.id=p.userId"))
                .ToLookup(x => x.UniqueId, x => x);
            if (!proposals.Any())
            {
                return null;
            }
            var keys = proposals.Select(x => x.Key);
            var results = db.Fetch<PairProposals>(new Sql().Select("*").From("dbo.cmsDictionary").Where("id IN (@tags)", new
            {
                tags = keys
            }));
            foreach (var v in results)
            {
                v.TranslationProposals = proposals[v.UniqueId];
            }
            return results;
        }

        [HttpPost]
        public void AcceptProposal(int id)
        {
            var db = DatabaseContext.Database;
            var proposal = db.FirstOrDefault<TranslationProposal>(new Sql("SELECT * FROM dbo.simpleTranslationProposals WHERE pk=@tag", new
            {
                tag = id
            }));
            var existingData = db.FirstOrDefault<TranslationText>(new Sql("SELECT * FROM dbo.cmsLanguageText WHERE UniqueId=@tag1 AND languageId=@tag2", new
            {
                tag1 = proposal.UniqueId,
                tag2 = proposal.LanguageId
            }));

            if (existingData == null)
            {
                var data = new TranslationText
                {
                    LangId = proposal.LanguageId,
                    UniqueId = proposal.UniqueId,
                    Value = proposal.Value
                };
                db.Insert(data);
            }
            else
            {
                existingData.Value = proposal.Value;
                db.Update(existingData);
            }

            db.Delete<TranslationProposal>(new Sql().Where("id=@tag1 AND languageId=@tag2", new
            {
                tag1 = proposal.UniqueId,
                tag2 = proposal.LanguageId
            }));
        }

        [HttpPost]
        public void RejectProposal(int id)
        {
            var db = DatabaseContext.Database;
            db.Delete<TranslationProposal>(new Sql().Where("pk=@tag", new
            {
                tag = id
            }));
        }
    }
}
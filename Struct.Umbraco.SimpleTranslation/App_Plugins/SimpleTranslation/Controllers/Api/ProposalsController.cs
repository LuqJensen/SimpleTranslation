﻿using System.Linq;
using System.Web.Http;
using Struct.Umbraco.SimpleTranslation.Models;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Web.WebApi;

namespace Struct.Umbraco.SimpleTranslation.Controllers.Api
{
    public class ProposalsController : UmbracoAuthorizedApiController
    {
        private bool CanEdit()
        {
            var userRoleHelper = new UserRoleHelper(DatabaseContext.Database);
            return userRoleHelper.IsEditor(UmbracoContext.Security.GetUserId());
        }

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
            if (!CanEdit())
                return;

            var db = DatabaseContext.Database;
            var proposal = db.FirstOrDefault<TranslationProposal>(new Sql("SELECT * FROM dbo.simpleTranslationProposals WHERE pk=@tag", new
            {
                tag = id
            }));

            var service = ApplicationContext.Services.LocalizationService;
            IDictionaryItem idi = service.GetDictionaryItemById(proposal.UniqueId);
            var translation = idi.Translations.FirstOrDefault(x => x.LanguageId == proposal.LanguageId);

            if (translation == null)
            {
                var translations = idi.Translations.ToList();
                translations.Add(new DictionaryTranslation(service.GetLanguageById(proposal.LanguageId), proposal.Value, proposal.UniqueId));
                idi.Translations = translations;
            }
            else
            {
                translation.Value = proposal.Value;
            }
            service.Save(idi);

            db.Delete<TranslationProposal>(new Sql().Where("id=@tag1 AND languageId=@tag2", new
            {
                tag1 = proposal.UniqueId,
                tag2 = proposal.LanguageId
            }));

            db.Delete<TranslationTask>(new Sql().Where("id=@tag1 AND languageId=@tag2", new
            {
                tag1 = proposal.UniqueId,
                tag2 = proposal.LanguageId
            }));
        }

        [HttpPost]
        public void RejectProposal(int id)
        {
            if (!CanEdit())
                return;

            var db = DatabaseContext.Database;
            db.Delete<TranslationProposal>(new Sql().Where("pk=@tag", new
            {
                tag = id
            }));
        }
    }
}
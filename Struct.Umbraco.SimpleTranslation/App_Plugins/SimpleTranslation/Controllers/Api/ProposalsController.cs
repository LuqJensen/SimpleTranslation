using System;
using System.Linq;
using System.Web.Http;
using Struct.Umbraco.SimpleTranslation.Models;
using Struct.Umbraco.SimpleTranslation.ViewModels;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Web.WebApi;

namespace Struct.Umbraco.SimpleTranslation.Controllers.Api
{
    public class ProposalsController : UmbracoAuthorizedApiController
    {
        private Database _db;
        private UserRoleHelper _urh;

        public ProposalsController()
        {
            _db = DatabaseContext.Database;
            _urh = new UserRoleHelper(_db);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db?.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public ProposalsView GetViewModel()
        {
            var model = new ProposalsView();
            var proposals = _db.Fetch<TranslationProposal>(new Sql()
                    .Select("p.*, u.userName AS userName, l.languageCultureName AS language")
                    .From("dbo.simpleTranslationProposals p LEFT OUTER JOIN dbo.umbracoLanguage l ON p.languageId=l.id LEFT OUTER JOIN dbo.umbracoUser u ON u.id=p.userId"))
                .ToLookup(x => x.UniqueId, x => x);

            if (!proposals.Any())
            {
                return model;
            }

            var keys = proposals.Select(x => x.Key);
            var results = _db.Fetch<PairProposals>(new Sql().Select("*").From("dbo.cmsDictionary").Where("id IN (@tags)", new
            {
                tags = keys
            }));

            foreach (var v in results)
            {
                v.TranslationProposals = proposals[v.UniqueId];
            }

            model.Proposals = results;

            return model;
        }

        [HttpPost]
        public void AcceptProposal(int id)
        {
            if (!_urh.IsEditor(UmbracoContext.Security.GetUserId()))
                return;

            var proposal = _db.FirstOrDefault<TranslationProposal>(new Sql("SELECT * FROM dbo.simpleTranslationProposals WHERE pk=@tag", new
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

            _db.Delete<TranslationProposal>(new Sql().Where("id=@tag1 AND languageId=@tag2", new
            {
                tag1 = proposal.UniqueId,
                tag2 = proposal.LanguageId
            }));

            _db.Delete<TranslationTask>(new Sql().Where("id=@tag1 AND languageId=@tag2", new
            {
                tag1 = proposal.UniqueId,
                tag2 = proposal.LanguageId
            }));
        }

        [HttpPost]
        public void RejectProposal(int id)
        {
            if (!_urh.IsEditor(UmbracoContext.Security.GetUserId()))
                return;

            _db.Delete<TranslationProposal>(new Sql().Where("pk=@tag", new
            {
                tag = id
            }));
        }

        [HttpPost]
        public void CreateProposal(TranslationProposal proposal)
        {
            if (proposal.Value == null || !_urh.CanUseSimpleTranslation(UmbracoContext.Security.GetUserId()))
                return;

            proposal.UserId = UmbracoContext.Security.GetUserId();
            proposal.Timestamp = DateTime.UtcNow;

            _db.Insert("dbo.simpleTranslationProposals", "pk", proposal);
        }
    }
}
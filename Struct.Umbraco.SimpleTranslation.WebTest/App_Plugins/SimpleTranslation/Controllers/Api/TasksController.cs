using System;
using System.Linq;
using System.Web.Http;
using Struct.Umbraco.SimpleTranslation.Models;
using Umbraco.Core.Persistence;
using Umbraco.Web.WebApi;

namespace Struct.Umbraco.SimpleTranslation.Controllers.Api
{
    public class TasksController : UmbracoAuthorizedApiController
    {
        private bool CanDiscard()
        {
            var userRoleHelper = new UserRoleHelper(DatabaseContext.Database);
            return userRoleHelper.IsEditor(UmbracoContext.Security.GetUserId());
        }

        [HttpGet]
        public object GetTranslationTasks()
        {
            var db = DatabaseContext.Database;

            var tasksQuery = new Sql()
                .Select("t.*, l.languageCultureName AS language, d.[key] AS [key]")
                .From("dbo.simpleTranslationTasks t LEFT OUTER JOIN dbo.umbracoLanguage l ON t.languageId=l.id LEFT OUTER JOIN dbo.cmsDictionary d ON t.id=d.id");

            if (!CanDiscard()) // Only translators are limited to languages they are responsible for. Administrators and Editors can view tasks for all languages.
            {
                tasksQuery = tasksQuery.Where("t.languageId IN (select languageId from dbo.simpleTranslationUserLanguages where id=@tag)", new
                {
                    tag = UmbracoContext.Security.GetUserId()
                });
            }

            var tasks = db.Fetch<TranslationTask>(tasksQuery);

            var currentTranslations =
                db.Fetch<TranslationText>(new Sql("select * from dbo.cmsLanguageText where UniqueId in (@ids)", new
                {
                    ids = tasks.Select(x => x.UniqueId).Distinct()
                })).ToLookup(x => x.UniqueId, x => x);


            var latestPersonalProposals = db.Fetch<TranslationProposal>(
                new Sql("select p1.* from dbo.simpleTranslationProposals p1 INNER JOIN" +
                        "(select MAX(pk) AS pk, id, languageId from dbo.simpleTranslationProposals where userId=@userId GROUP BY id, languageId)" +
                        "AS p2 ON p1.pk=p2.pk", new
                {
                    userId = UmbracoContext.Security.GetUserId()
                })).ToDictionary(x => new
            {
                x.UniqueId,
                x.LanguageId
            }, x => x);

            foreach (var v in tasks)
            {
                TranslationProposal p;
                latestPersonalProposals.TryGetValue(new
                {
                    v.UniqueId,
                    v.LanguageId
                }, out p);
                v.LatestPersonalProposal = p;

                v.CurrentTranslations = currentTranslations[v.UniqueId].ToDictionary(x => x.LangId, x => x.Value);
            }

            return new
            {
                tasks,
                canDiscard = CanDiscard(),
                languages = db.Fetch<Language>(new Sql().Select("*").From("dbo.umbracoLanguage"))
            };
        }

        [HttpGet]
        public object GetProposalsForTask(Guid id, int languageId)
        {
            var db = DatabaseContext.Database;
            var latestProposals = db.Fetch<TranslationProposal>(
                new Sql("select p.*, u.userName from dbo.simpleTranslationProposals p left outer join dbo.umbracoUser u on p.userId=u.id where p.id=@tag1 and p.languageId=@tag2", new
                {
                    tag1 = id,
                    tag2 = languageId
                }));

            return latestProposals;
        }

        [HttpPost]
        public void DeleteTask(int id)
        {
            if (!CanDiscard())
                return;

            var db = DatabaseContext.Database;
            db.Delete<TranslationTask>(new Sql().Where("pk=@tag", new
            {
                tag = id
            }));
        }

        [HttpPost]
        public void CreateProposal(TaskProposal proposal)
        {
            var db = DatabaseContext.Database;
            db.Insert("dbo.simpleTranslationProposals", "pk", new TranslationProposal
            {
                LanguageId = proposal.LanguageId,
                UniqueId = proposal.UniqueId,
                UserId = UmbracoContext.Security.GetUserId(),
                Value = proposal.Value,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Models;
using Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Utility;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;

namespace Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Controllers.Api
{
    public class TasksController : UmbracoAuthorizedApiController
    {
        private bool CanDiscard()
        {
            return SecurityUtility.IsEditor(UmbracoContext.Security.CurrentUser);
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
            }

            return new
            {
                tasks,
                canDiscard = CanDiscard()
            };
        }

        [HttpGet]
        public object GetProposalsForTask(Guid id, int languageId)
        {
            var db = DatabaseContext.Database;
            var latestProposals = db.Fetch<TranslationProposal>(new Sql("select * from dbo.simpleTranslationProposals where id=@tag1 and languageId=@tag2", new
            {
                tag1 = id,
                tag2 = languageId
            })).ToList();

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
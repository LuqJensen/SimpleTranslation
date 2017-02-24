using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;

namespace Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Controllers.Api
{
    public class TasksController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public object GetTranslationTasks()
        {
            var db = DatabaseContext.Database;
            var tasks = db.Fetch<TranslationTask>(new Sql()
                    .Select("t.*, l.languageCultureName AS language, d.[key] AS [key]")
                    .From("dbo.simpleTranslationTasks t LEFT OUTER JOIN dbo.umbracoLanguage l ON t.languageId=l.id LEFT OUTER JOIN dbo.cmsDictionary d ON t.id=d.id"))
                .ToList();

            var latestProposals = db.Fetch<TranslationProposal>(
                new Sql("select p1.* from dbo.simpleTranslationProposals p1 INNER JOIN" +
                        "(select MAX(pk) AS pk, id, languageId from dbo.simpleTranslationProposals GROUP BY id, languageId)" +
                        "AS p2 ON p1.pk=p2.pk")).ToDictionary(x => new
            {
                x.UniqueId,
                x.LanguageId
            }, x => x);

            foreach (var v in tasks)
            {
                TranslationProposal p;
                latestProposals.TryGetValue(new
                {
                    v.UniqueId,
                    v.LanguageId
                }, out p);
                v.LatestProposal = p;
            }

            var latestPersonalProposals = db.Fetch<TranslationProposal>(
                new Sql("select p1.* from dbo.simpleTranslationProposals p1 INNER JOIN" +
                        "(select MAX(pk) AS pk, id, languageId from dbo.simpleTranslationProposals where userId=@userId GROUP BY id, languageId)" +
                        "AS p2 ON p1.pk=p2.pk", new
                {
                    userId = UmbracoContext.Security.CurrentUser.Id
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

            return tasks;
        }

        [HttpPost]
        public void DeleteTask(int id)
        {
            var db = DatabaseContext.Database;
            db.Delete<TranslationTask>(new Sql().Where("pk=@tag", new
            {
                tag = id
            }));
        }
    }
}
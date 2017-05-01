using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Struct.Umbraco.SimpleTranslation.Models;
using Struct.Umbraco.SimpleTranslation.ViewModels;
using Umbraco.Core.Persistence;
using Umbraco.Web.WebApi;

namespace Struct.Umbraco.SimpleTranslation.Controllers.Api
{
    public class TasksController : UmbracoAuthorizedApiController
    {
        private Database _db;
        private UserRoleHelper _urh;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db?.Dispose();
            }
            base.Dispose(disposing);
        }

        public TasksController()
        {
            _db = DatabaseContext.Database;
            _urh = new UserRoleHelper(_db);
        }

        [HttpGet]
        public TasksView GetViewModel()
        {
            var model = new TasksView
            {
                Languages = _db.Fetch<Language>(new Sql().Select("*").From("dbo.umbracoLanguage"))
            };

            var tasksQuery = new Sql()
                .Select("t.*, l.languageCultureName AS language, d.[key] AS [key]")
                .From("dbo.simpleTranslationTasks t LEFT OUTER JOIN dbo.umbracoLanguage l ON t.languageId=l.id LEFT OUTER JOIN dbo.cmsDictionary d ON t.id=d.id");

            // Only translators are limited to languages they are responsible for. Administrators and Editors can view tasks for all languages.
            if (_urh.IsTranslator(UmbracoContext.Security.GetUserId()))
            {
                tasksQuery = tasksQuery.Where("t.languageId IN (select languageId from dbo.simpleTranslationUserLanguages where id=@tag)", new
                {
                    tag = UmbracoContext.Security.GetUserId()
                });
            }

            var tasks = _db.Fetch<TranslationTask>(tasksQuery);

            // Skip further processing if there are no tasks.
            if (!tasks.Any())
            {
                return model;
            }

            var currentTranslations =
                _db.Fetch<TranslationText>(new Sql("select * from dbo.cmsLanguageText where UniqueId in (@ids)", new
                {
                    ids = tasks.Select(x => x.UniqueId).Distinct()
                })).ToLookup(x => x.UniqueId, x => x);


            var latestPersonalProposals = _db.Fetch<TranslationProposal>(
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

            model.Tasks = tasks;
            model.IsEditor = _urh.IsEditor(UmbracoContext.Security.GetUserId());

            return model;
        }

        [HttpGet]
        public IEnumerable<TranslationProposal> GetProposalsForTask(Guid id, int languageId)
        {
            var latestProposals = _db.Fetch<TranslationProposal>(
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
            if (!_urh.IsEditor(UmbracoContext.Security.GetUserId()))
                return;

            _db.Delete<TranslationTask>(new Sql().Where("pk=@tag", new
            {
                tag = id
            }));
        }
    }
}
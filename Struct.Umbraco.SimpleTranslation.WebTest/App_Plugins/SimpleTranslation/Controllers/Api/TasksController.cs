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
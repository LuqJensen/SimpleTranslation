using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml;
using System.Xml.Serialization;
using Struct.Umbraco.SimpleTranslation.Models;
using Struct.Umbraco.SimpleTranslation.Utility;
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
        public void ExportTasksToXml(int fromLangId, int toLangId)
        {
            var tasks = _db.Fetch<ExportedTranslationTask>(
                new Sql("SELECT t.*, v.value FROM dbo.simpleTranslationTasks t LEFT OUTER JOIN dbo.cmsLanguageText v ON t.id=v.UniqueId AND v.languageId=@tag1 WHERE t.languageId=@tag2", new
                {
                    tag1 = fromLangId,
                    tag2 = toLangId
                }));

            // Ensure that no values are null or empty as the XmlSerializer will then exclude the tag of the property.
            foreach (var v in tasks)
            {
                v.LocalText = v.LocalText ?? " ";
                v.TranslatedText = v.TranslatedText ?? " ";
            }

            var service = ApplicationContext.Services.LocalizationService;
            var fromLanguage = service.GetLanguageById(fromLangId).CultureName;
            var toLanguage = service.GetLanguageById(toLangId).CultureName;

            // Clear any response data and add header for filetransfer and set content type.
            var response = UmbracoContext.HttpContext.Response;
            response.Clear();
            response.Buffer = true;
            response.AddHeader("content-disposition", $"attachment;filename=SimpleTranslation Tasks {fromLanguage} to {toLanguage} {DateTime.Today.ToString("dd-MM-yy")}.xml");
            response.ContentType = "text/xml";

            // Perform the serialization to the Http outputstream.
            var xs = new XmlSerializer(tasks.GetType());
            xs.Serialize(response.OutputStream, tasks);

            // Force write any remaining data and end the response.
            response.OutputStream.Close();
            response.End();
        }

        private IEnumerable<ExportedTranslationTask> ReadXml(XmlModel model)
        {
            ExportedTranslationTask[] proposals;

            using (var s = new StringReader(model.Value))
            {
                var xs = new XmlSerializer(typeof(ExportedTranslationTask[]));
                proposals = (ExportedTranslationTask[])xs.Deserialize(s);
            }

            var languages = proposals.Select(x => x.LanguageId).Distinct();

            // The feature expects the xml file to contain keys for a single language, as it makes sense in practice.
            if (languages.Count() > 1)
            {
                throw new HttpException("Invalid xml file. Expected file with single language.");
            }

            var languageExists = _db.Fetch<int>("select 1 from dbo.umbracoLanguage where id=@tag", new
            {
                tag = languages.First()
            }).Any();

            if (!languageExists)
            {
                throw new HttpException("Invalid xml file. File contains invalid language.");
            }

            var validKeys = _db.Fetch<int>("select 1 from dbo.cmsDictionary where id IN (@tags)", new
            {
                tags = proposals.Select(x => x.UniqueId).Distinct()
            });

            if (validKeys.Count != proposals.Length)
            {
                throw new HttpException("Invalid xml file. File contains non existing keys.");
            }

            // We dont want empty texts.
            return proposals.Where(x => !string.IsNullOrWhiteSpace(x.TranslatedText));
        }

        [HttpPost]
        public void ImportProposalsFromXml(XmlModel model)
        {
            if (!_urh.IsEditor(UmbracoContext.Security.GetUserId()))
                return;

            IEnumerable<ExportedTranslationTask> proposals = ReadXml(model);
            List<TranslationProposal> data = new List<TranslationProposal>();
            var userId = UmbracoContext.Security.GetUserId();

            foreach (var p in proposals)
            {
                if (string.IsNullOrWhiteSpace(p.TranslatedText))
                    continue;

                data.Add(new TranslationProposal
                {
                    LanguageId = p.LanguageId,
                    UniqueId = p.UniqueId,
                    UserId = userId,
                    Timestamp = DateTime.UtcNow,
                    Value = p.TranslatedText
                });
            }
            if (data.Any())
            {
                _db.BulkInsertRecords(data);
            }
        }

        [HttpPost]
        public void ImportTranslationsFromXml(XmlModel model)
        {
            if (!_urh.IsEditor(UmbracoContext.Security.GetUserId()))
                return;

            IEnumerable<ExportedTranslationTask> translations = ReadXml(model);

            var keys = translations.Select(x => x.UniqueId);
            var languageId = translations.First().LanguageId;

            var existingTranslations = _db.Fetch<LanguageText>("select * from dbo.cmsLanguageText where UniqueId IN (@tags1) AND languageId=@tag2", new
            {
                tags1 = keys,
                tag2 = languageId
            }).Where(x => !string.IsNullOrWhiteSpace(x.Value)).ToDictionary(x => x.UniqueId, x => x);

            var newKeys = translations.Where(x => !existingTranslations.ContainsKey(x.UniqueId)).Select(x => new LanguageText
            {
                LanguageId = languageId,
                UniqueId = x.UniqueId,
                Value = x.TranslatedText
            });

            if (newKeys.Any())
            {
                _db.BulkInsertRecords(newKeys);
            }

            foreach (var translation in translations)
            {
                LanguageText existingTranslation;
                if (!existingTranslations.TryGetValue(translation.UniqueId, out existingTranslation))
                    continue;

                existingTranslation.Value = translation.TranslatedText;
                _db.Update(existingTranslation);
            }


            /*var service = ApplicationContext.Services.LocalizationService;

            foreach (var translation in translations)
            {
                if (string.IsNullOrWhiteSpace(translation.TranslatedText))
                    continue;

                IDictionaryItem idi = service.GetDictionaryItemById(translation.UniqueId);
                var existingTranslation = idi.Translations.FirstOrDefault(x => x.LanguageId == language.Id);

                if (existingTranslation == null)
                {
                    var existingTranslations = idi.Translations.ToList();
                    existingTranslations.Add(new DictionaryTranslation(language, translation.TranslatedText, translation.UniqueId));
                    idi.Translations = existingTranslations;
                }
                else
                {
                    existingTranslation.Value = translation.TranslatedText;
                }
                service.Save(idi);
            }*/


            _db.Delete<TranslationProposal>(new Sql().Where("id IN (@tags1) AND languageId=@tag2", new
            {
                tags1 = keys,
                tag2 = languageId
            }));

            _db.Delete<TranslationTask>(new Sql().Where("id IN (@tags1) AND languageId=@tag2", new
            {
                tags1 = keys,
                tag2 = languageId
            }));
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
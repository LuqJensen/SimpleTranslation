using Struct.Umbraco.SimpleTranslation.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Struct.Umbraco.SimpleTranslation
{
    public class Startup : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var ctx = applicationContext.DatabaseContext;

            using (var db = ctx.Database)
            {
                var schema = new DatabaseSchemaHelper(db, applicationContext.ProfilingLogger.Logger, ctx.SqlSyntax);

                if (!schema.TableExist("simpleTranslationProposals"))
                {
                    schema.CreateTable<TranslationProposal>(false);
                }
                if (!schema.TableExist("simpleTranslationTasks"))
                {
                    schema.CreateTable<TranslationTask>(false);
                }
                if (!schema.TableExist("simpleTranslationUserLanguages"))
                {
                    schema.CreateTable<UserLanguage>(false);
                }
                if (!schema.TableExist("simpleTranslationUserRoles"))
                {
                    schema.CreateTable<UserRole>(false);
                }
            }
        }
    }
}
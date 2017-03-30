﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation
{
    public class Startup : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var ctx = applicationContext.DatabaseContext;
            var schema = new DatabaseSchemaHelper(ctx.Database, applicationContext.ProfilingLogger.Logger, ctx.SqlSyntax);

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
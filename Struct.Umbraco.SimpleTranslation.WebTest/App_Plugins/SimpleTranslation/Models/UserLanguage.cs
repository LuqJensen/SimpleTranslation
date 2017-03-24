using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Models
{
    [TableName("simpleTranslationUserLanguages")]
    [PrimaryKey("pk", autoIncrement = true)]
    public class UserLanguage
    {
        [Column("pk")]
        [PrimaryKeyColumn]
        [JsonProperty("primaryKey")]
        public int PrimaryKey { get; set; }

        [Column("id")]
        [JsonProperty("id")]
        [ForeignKey(typeof(User), Column = "id")]
        public int Id { get; set; }

        [Column("languageId")]
        [JsonProperty("languageId")]
        [ForeignKey(typeof(Language), Column = "id")]
        public int LanguageId { get; set; }
    }
}
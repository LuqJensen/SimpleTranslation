using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Models
{
    [TableName("simpleTranslationTasks")]
    [PrimaryKey("pk", autoIncrement = true)]
    public class TranslationTask
    {
        [Column("pk")]
        [PrimaryKeyColumn]
        [JsonProperty("primaryKey")]
        public int PrimaryKey { get; set; }

        [Column("id")]
        [JsonProperty("id")]
        [ForeignKey(typeof(Pair), Column = "id")]
        public Guid UniqueId { get; set; }

        [Column("languageId")]
        [JsonProperty("languageId")]
        [ForeignKey(typeof(Language), Column = "id")]
        public int LanguageId { get; set; }

        [ResultColumn]
        [JsonProperty("language")]
        public string Language { get; set; }

        [ResultColumn]
        [JsonProperty("key")]
        public string Key { get; set; }
    }
}
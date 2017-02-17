using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Umbraco.Core.Persistence;

namespace Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Models
{
    public class TranslationText
    {
        [Column("pk")]
        [JsonProperty("primaryKey")]
        public int PrimaryKey { get; set; }

        [Column("languageId")]
        [JsonProperty("langId")]
        public int LangId { get; set; }

        [Column("UniqueId")]
        [JsonProperty("id")]
        public Guid UniqueId { get; set; }

        [Column("value")]
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
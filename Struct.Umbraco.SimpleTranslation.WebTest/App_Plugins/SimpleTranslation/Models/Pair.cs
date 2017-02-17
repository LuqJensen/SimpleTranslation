using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Models
{
    public class Pair
    {
        [Column("pk")]
        [JsonProperty("primaryKey")]
        public int PrimaryKey { get; set; }

        [Column("id")]
        [JsonProperty("id")]
        public Guid UniqueId { get; set; }

        [Column("parent")]
        [JsonProperty("parent")]
        public Guid? Parent { get; set; }

        [Column("key")]
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("children")]
        public Dictionary<Guid, Pair> Children { get; set; }

        [JsonProperty("translationTexts")]
        public IEnumerable<TranslationText> TranslationTexts { get; set; }
    }
}
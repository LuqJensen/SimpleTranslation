using System;
using Newtonsoft.Json;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Struct.Umbraco.SimpleTranslation.Models
{
    [TableName("cmsLanguageText")]
    [PrimaryKey("pk", autoIncrement = true)]
    [ExplicitColumns]
    public class TranslationText
    {
        [Column("pk")]
        [PrimaryKeyColumn()]
        [JsonProperty("primaryKey")]
        public int PrimaryKey { get; set; }

        [Column("languageId")]
        [ForeignKey(typeof(Language), Column = "id")]
        [JsonProperty("langId")]
        public int LanguageId { get; set; }

        [Column("UniqueId")]
        [ForeignKey(typeof(Pair), Column = "id")]
        [JsonProperty("id")]
        public Guid UniqueId { get; set; }

        [Column("value")]
        [Length(1000)]
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
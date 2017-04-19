using System;
using Newtonsoft.Json;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Struct.Umbraco.SimpleTranslation.Models
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

        [Ignore]
        [JsonProperty("latestPersonalProposal")]
        public TranslationProposal LatestPersonalProposal { get; set; }

        [Ignore]
        [JsonProperty("latestProposal")]
        public TranslationProposal LatestProposal { get; set; }
    }
}
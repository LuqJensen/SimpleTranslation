using System;
using Newtonsoft.Json;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Struct.Umbraco.SimpleTranslation.Models
{
    [TableName("simpleTranslationProposals")]
    [PrimaryKey("pk", autoIncrement = true)]
    public class TranslationProposal
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

        [Column("userId")]
        [JsonProperty("userId")]
        [ForeignKey(typeof(User), Column = "id")]
        public int UserId { get; set; }

        [Column("value")]
        [JsonProperty("value")]
        public string Value { get; set; }

        [Column("timestamp")]
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [ResultColumn]
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [ResultColumn]
        [JsonProperty("language")]
        public string Language { get; set; }
    }
}
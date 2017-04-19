using System;
using Newtonsoft.Json;
using Umbraco.Core.Persistence;

namespace Struct.Umbraco.SimpleTranslation.Models
{
    [TableName("cmsDictionary")]
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
    }
}
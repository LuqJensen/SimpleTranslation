using Newtonsoft.Json;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Struct.Umbraco.SimpleTranslation.Models
{
    [TableName("simpleTranslationUserRoles")]
    [PrimaryKey("pk", autoIncrement = true)]
    public class UserRole
    {
        [Column("pk")]
        [PrimaryKeyColumn]
        [JsonProperty("primaryKey")]
        public int PrimaryKey { get; set; }

        [Column("id")]
        [JsonProperty("id")]
        public int Id { get; set; }

        [Column("role")]
        [JsonProperty("role")]
        public int Role { get; set; }
    }
}
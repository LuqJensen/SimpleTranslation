using Newtonsoft.Json;
using Umbraco.Core.Persistence;

namespace Struct.Umbraco.SimpleTranslation.Models
{
    [TableName("umbracoUser")]
    public class User
    {
        [Column("id")]
        [JsonProperty("id")]
        public int Id { get; set; }

        [Column("userName")]
        [JsonProperty("userName")]
        public string UserName { get; set; }
    }
}
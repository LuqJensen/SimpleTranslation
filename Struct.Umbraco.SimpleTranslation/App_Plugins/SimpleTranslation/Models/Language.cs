using Newtonsoft.Json;
using Umbraco.Core.Persistence;

namespace Struct.Umbraco.SimpleTranslation.Models
{
    [TableName("umbracoLanguage")]
    public class Language
    {
        [Column("id")]
        [JsonProperty("id")]
        public int Id { get; set; }

        [Column("languageCultureName")]
        [JsonProperty("languageCultureName")]
        public string LanguageCultureName { get; set; }
    }
}
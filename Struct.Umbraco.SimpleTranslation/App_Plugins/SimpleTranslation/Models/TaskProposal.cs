using Newtonsoft.Json;

namespace Struct.Umbraco.SimpleTranslation.Models
{
    public class TaskProposal : TranslationTask
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Struct.Umbraco.SimpleTranslation.Models
{
    public class PairTranslations : Pair
    {
        [JsonProperty("children")]
        public Dictionary<Guid, PairTranslations> Children { get; set; }

        [JsonProperty("translationTexts")]
        public Dictionary<int, string> TranslationTexts { get; set; }

        [JsonProperty("translationTasks")]
        public Dictionary<int, bool> TranslationTasks { get; set; }
    }
}
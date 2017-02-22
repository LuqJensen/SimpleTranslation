using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Models
{
    public class PairTranslations : Pair
    {
        [JsonProperty("children")]
        public Dictionary<Guid, PairTranslations> Children { get; set; }

        [JsonProperty("translationTexts")]
        public Dictionary<int, TranslationText> TranslationTexts { get; set; }
    }
}
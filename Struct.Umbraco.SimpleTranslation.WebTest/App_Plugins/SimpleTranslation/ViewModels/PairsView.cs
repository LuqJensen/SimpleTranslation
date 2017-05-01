using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Struct.Umbraco.SimpleTranslation.Models;

namespace Struct.Umbraco.SimpleTranslation.ViewModels
{
    public class PairsView
    {
        [JsonProperty("pairs")]
        public IEnumerable<PairTranslations> Pairs { get; set; }

        [JsonProperty("isEditor")]
        public bool IsEditor { get; set; }

        [JsonProperty("languages")]
        public IEnumerable<Language> Languages { get; set; }
    }
}
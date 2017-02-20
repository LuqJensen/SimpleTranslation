using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Models
{
    public class PairProposals : Pair
    {
        [JsonProperty("translationProposals")]
        public IEnumerable<TranslationProposal> TranslationProposals { get; set; }
    }
}
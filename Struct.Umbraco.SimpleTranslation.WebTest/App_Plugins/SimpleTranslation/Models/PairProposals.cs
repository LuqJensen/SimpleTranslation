using System.Collections.Generic;
using Newtonsoft.Json;

namespace Struct.Umbraco.SimpleTranslation.Models
{
    public class PairProposals : Pair
    {
        [JsonProperty("translationProposals")]
        public IEnumerable<TranslationProposal> TranslationProposals { get; set; }
    }
}
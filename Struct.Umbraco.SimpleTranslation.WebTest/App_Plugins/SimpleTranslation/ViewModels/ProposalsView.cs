using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Struct.Umbraco.SimpleTranslation.Models;

namespace Struct.Umbraco.SimpleTranslation.ViewModels
{
    public class ProposalsView
    {
        [JsonProperty("proposals")]
        public IEnumerable<PairProposals> Proposals { get; set; }
    }
}
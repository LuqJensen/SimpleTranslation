using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Models
{
    public class PairProposals : Pair
    {
        public IEnumerable<TranslationProposal> TranslationProposals { get; set; }
    }
}
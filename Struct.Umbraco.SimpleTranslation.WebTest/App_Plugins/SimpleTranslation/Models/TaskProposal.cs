using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Models
{
    public class TaskProposal : TranslationTask
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
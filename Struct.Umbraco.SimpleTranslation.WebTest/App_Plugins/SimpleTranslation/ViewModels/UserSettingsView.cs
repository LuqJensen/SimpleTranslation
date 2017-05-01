using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Struct.Umbraco.SimpleTranslation.Models;

namespace Struct.Umbraco.SimpleTranslation.ViewModels
{
    public class UserSettingsView
    {
        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("userLanguages")]
        public IEnumerable<int> UserLanguages { get; set; }

        [JsonProperty("languages")]
        public IEnumerable<Language> Languages { get; set; }

        [JsonProperty("userRole")]
        public int UserRole { get; set; }

        [JsonProperty("roles")]
        public IEnumerable<TranslationRole> Roles { get; set; }
    }
}
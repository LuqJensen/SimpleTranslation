using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Struct.Umbraco.SimpleTranslation.Models
{
    public class SettingsDTO
    {
        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("userRole")]
        public int UserRole { get; set; }

        [JsonProperty("userLanguages")]
        public IEnumerable<int> UserLanguages { get; set; }
    }
}
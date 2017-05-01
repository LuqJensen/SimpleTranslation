using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Struct.Umbraco.SimpleTranslation.Models
{
    public class TranslationRole
    {
        static TranslationRole()
        {
            TranslationRoles = ((TranslationRoles[])Enum.GetValues(typeof(TranslationRoles))).ToList().Select(x => new TranslationRole
            {
                Id = (int)x,
                Title = x.ToString()
            });
        }

        public static IEnumerable<TranslationRole> TranslationRoles { get; private set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Struct.Umbraco.SimpleTranslation.WebTest.App_Plugins.SimpleTranslation.Models
{
    [TableName("umbracoUser")]
    public class User
    {
        [Column("id")]
        [JsonProperty("id")]
        public int Id { get; set; }

        [Column("userName")]
        [JsonProperty("userName")]
        public string UserName { get; set; }
    }
}
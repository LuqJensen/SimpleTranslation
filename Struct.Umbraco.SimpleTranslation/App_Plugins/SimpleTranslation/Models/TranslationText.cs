﻿using System;
using Newtonsoft.Json;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Struct.Umbraco.SimpleTranslation.Models
{
    [TableName("cmsLanguageText")]
    [PrimaryKey("pk", autoIncrement = true)]
    public class TranslationText
    {
        [Column("pk")]
        [PrimaryKeyColumn()]
        [JsonProperty("primaryKey")]
        public int PrimaryKey { get; set; }

        [Column("languageId")]
        [JsonProperty("langId")]
        public int LangId { get; set; }

        [Column("UniqueId")]
        [JsonProperty("id")]
        public Guid UniqueId { get; set; }

        [Column("value")]
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
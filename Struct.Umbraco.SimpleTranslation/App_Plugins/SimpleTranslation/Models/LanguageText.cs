using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Struct.Umbraco.SimpleTranslation.Models
{
    [TableName("cmsLanguageText")]
    [PrimaryKey("pk")]
    [ExplicitColumns]
    public class LanguageText
    {
        [Column("pk")]
        [PrimaryKeyColumn]
        public int PrivateKey { get; set; }

        [Column("languageId")]
        [ForeignKey(typeof(Language), Column = "id")]
        public int LanguageId { get; set; }

        [Column("UniqueId")]
        [ForeignKey(typeof(Pair), Column = "id")]
        public Guid UniqueId { get; set; }

        [Column("value")]
        [Length(1000)]
        public string Value { get; set; }
    }
}
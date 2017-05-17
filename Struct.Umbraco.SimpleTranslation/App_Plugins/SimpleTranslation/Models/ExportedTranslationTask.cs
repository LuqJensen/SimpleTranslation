using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using Umbraco.Core.Persistence;

namespace Struct.Umbraco.SimpleTranslation.Models
{
    public class ExportedTranslationTask
    {
        [Column("id")]
        public Guid UniqueId { get; set; }

        [Column("languageId")]
        public int LanguageId { get; set; }

        [Column("value")]
        public string LocalText { get; set; }

        public string TranslatedText { get; set; }
    }
}
using Umbraco.Core.Persistence;

namespace Struct.Umbraco.SimpleTranslation.Models
{
    public class XmlModel
    {
        [Column("value")]
        public string Value { get; set; }
    }
}
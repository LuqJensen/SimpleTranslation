using System.Text;

namespace Struct.Umbraco.SimpleTranslation.Utility
{
    public sealed class StringWriterWithEncoding : System.IO.StringWriter
    {
        public StringWriterWithEncoding(StringBuilder sb) : base(sb)
        {
            Encoding = Encoding.Unicode;
        }


        public StringWriterWithEncoding(Encoding encoding)
        {
            Encoding = encoding;
        }

        public StringWriterWithEncoding(StringBuilder sb, Encoding encoding) : base(sb)
        {
            Encoding = encoding;
        }

        public override Encoding Encoding { get; }
    }
}
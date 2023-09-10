using System.IO;
using System.Text;

namespace TalentV2.Utils
{
    public class UTF8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8; 
    }
}

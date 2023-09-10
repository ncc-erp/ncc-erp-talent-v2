using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.ModelExtends
{
    public interface IHRSignatureInfo
    {
        public string HRPhone { get; set; }
        public string HRName { get; set; }
        public string HREmail { get; set; }
        public string SignatureContact { get; set; }
    }
}

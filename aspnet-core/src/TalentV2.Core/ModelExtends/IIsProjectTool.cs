using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.ModelExtends
{
    public interface IIsProjectTool
    {
        public bool IsProjectTool{ get; }
        public long? ProjectToolRequestId { get; set; }
    }
}

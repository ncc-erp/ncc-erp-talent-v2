using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Dto
{
    public class GetResultConnectDto
    {
        public bool IsConnected { get; set; }
        public string Message { get; set; }
    }

    public class GetSecurityCodeDto
    {
        public string SecretCode { get; set; }
        public string SecurityCodeHeader { get; set; }
    }
}

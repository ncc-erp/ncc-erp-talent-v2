using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class UpdateFileDto
    {
        public long CVId { get; set; }
        public IFormFile FileUpdate { get; set; }
    }
    public class UpdateFileCVDto : UpdateFileDto { }
    public class UpdateFileAvatarDto : UpdateFileDto { }
}

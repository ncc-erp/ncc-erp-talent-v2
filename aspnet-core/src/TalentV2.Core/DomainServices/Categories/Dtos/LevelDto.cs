using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Categories.Dtos
{
    public class LevelDto
    {
        public long Id { get; set; }
        public string DefaultName { get; set; }
        public string StandardName { get; set; }
        public string ShortName { get; set; }
    }
}

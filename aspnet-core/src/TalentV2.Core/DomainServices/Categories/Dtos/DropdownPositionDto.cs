using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Categories.Dtos
{
    public class DropdownPositionDto
    {
        public long Id { get; set; }
        public string Position { get; set; }
        public List<DropdownSubPositionDto> Items { get; set; }
    }
    public class DropdownSubPositionDto
    {
        public long Id { get; set; }
        public string SubPosition { get; set; }
    }
}

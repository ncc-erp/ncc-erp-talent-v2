using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.ModelExtends
{
    public class DropdownUserInfoDto
    {
        public long Id { get; set; }
        public string Surname { get; set; }
        public string  Name { get; set; }
        public string FullName => Surname+ " " + Name;
        public string Email { get; set; }
        public string UserName { get; set; }
        public string LabelName => FullName + "(" + UserName + ")";
    }
}

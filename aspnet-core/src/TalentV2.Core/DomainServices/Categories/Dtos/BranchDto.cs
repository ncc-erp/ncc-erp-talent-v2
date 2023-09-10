using Abp.AutoMapper;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Entities;

namespace TalentV2.DomainServices.Categories.Dtos
{
    public class BranchDto : CategoryDto
    {
        [ApplySearchAttribute]
        public string DisplayName { get; set; }
        public string ColorCode { get; set; }
        public string Address { get; set; }
    }
    [AutoMapTo(typeof(Branch))]
    public class CreateBranchDto: BranchDto
    {
    }
    [AutoMapTo(typeof(Branch))]
    public class UpdateBranchDto : BranchDto
    {
    }
}

using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TalentV2.Entities.NccCVs;

namespace TalentV2.APIs.NccCVs.APIs.Project.Dto
{
    [AutoMapTo(typeof(TalentV2.Entities.NccCVs.Project))]
    public class ProjectDto : EntityDto<long>
    {
        public string Name { get; set; }
        public ProjectType Type { get; set; }
        public string Technology { get; set; }
        public IEnumerable<TechnologyDto> Technologies { get; set; }
        public string Description { get; set; }
    }
    public class TechnologyDto
    {
        public long Id { get; set; }
        public string TechName { get; set; }
        public string Version { get; set; }
        public Boolean isChecked { get; set; }
    }

}

using Abp.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.DomainServices.NccCVs.GroupSkills;
using TalentV2.DomainServices.NccCVs.GroupSkills.Dtos;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class GroupSkillAppService : TalentV2AppServiceBase
    {
        private readonly IGroupSkillManager _groupSkillManager;
        public GroupSkillAppService(IGroupSkillManager groupSkillManager)
        {
            _groupSkillManager = groupSkillManager;
        }
        [HttpPost]
        public async Task<GridResult<GroupSkillDto>> GetAllPaging(GridParam param)
        {
            var query = _groupSkillManager
                .IQGetAll();
            return await query.GetGridResult(query, param);
        }
        [HttpPost]
        public async Task<GroupSkillDto> Create(GroupSkillDto input)
        {
            return await _groupSkillManager.Create(input);
        }
        [HttpPut]
        public async Task<GroupSkillDto> Update(GroupSkillDto input)
        {
            return await _groupSkillManager.Update(input);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(long Id)
        {
            await _groupSkillManager.Delete(Id);
            return new OkObjectResult("Deleted Successfully!");
        }
        [HttpGet]
        public async Task<List<GroupSkillDto>> GetAll()
        {
            return await _groupSkillManager
                .IQGetAll()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}

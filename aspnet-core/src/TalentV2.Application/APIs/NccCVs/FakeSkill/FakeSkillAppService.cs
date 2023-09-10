using Abp.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.DomainServices.NccCVs.FakeSkills;
using TalentV2.DomainServices.NccCVs.FakeSkills.Dtos;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class FakeSkillAppService : TalentV2AppServiceBase
    {
        private readonly IFakeSkillManager _fakeSkillManager;
        public FakeSkillAppService(IFakeSkillManager fakeSkillManager)
        {
            _fakeSkillManager = fakeSkillManager;
        }
        [HttpPost]
        public async Task<GridResult<FakeSkillDto>> GetAllPaging(GridParam param)
        {
            var query = _fakeSkillManager
                .IQGetAll();
            return await query.GetGridResult(query, param);
        }
        [HttpPost]
        public async Task<FakeSkillDto> Create(FakeSkillDto input)
        {
            return await _fakeSkillManager.Create(input);
        }
        [HttpPut]
        public async Task<FakeSkillDto> Update(FakeSkillDto input)
        {
            return await _fakeSkillManager.Update(input);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(long Id)
        {
            await _fakeSkillManager.Delete(Id);
            return new OkObjectResult("Deleted Successfully!");
        }
        [HttpGet]
        public async Task<List<FakeSkillDto>> GetAll()
        {
            return await _fakeSkillManager
                .IQGetAll()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}

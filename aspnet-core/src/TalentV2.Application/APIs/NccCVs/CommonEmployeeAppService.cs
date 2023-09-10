using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.Entities;
using TalentV2.Entities.NccCVs;

namespace TalentV2.APIs.NccCVs
{
    public class CommonEmployeeAppService : TalentV2AppServiceBase
    {
        [HttpGet]
        public async Task<IActionResult> GetPosition()
        {
            var positions = await WorkScope.GetAll<EmployeePosition>()
                .Select(x => new IdAndNameDto
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .AsNoTracking()
                .ToListAsync();
            return new OkObjectResult(positions);
        }
        [HttpGet]
        public async Task<IActionResult> GetLanguage()
        {
            var languages = await WorkScope.GetAll<EmployeeLanguage>()
                .Select(x => new IdAndNameDto
                {
                    Id = x.Id,
                    Name = x.Name,
                }).AsNoTracking().ToListAsync();
            return new OkObjectResult(languages);
        }
        public async Task<List<IdAndNameDto>> GetCBBGroupSkill()
        {
            return await WorkScope.GetAll<GroupSkill>()
                                       .Select(g => new IdAndNameDto
                                       {
                                           Id = g.Id,
                                           Name = g.Name
                                       })
                                       .AsNoTracking()
                                       .ToListAsync();
        }
        public async Task<dynamic> GetCBBSkillByGroupSkillId(long id)
        {
            return await WorkScope.GetAll<FakeSkill>()
                                  .Where(s => s.GroupSkillId == id)
                                  .Select(s => new 
                                  {
                                      Id = s.Id,
                                      GroupSkillId = s.GroupSkillId,
                                      Name = s.Name
                                  }).ToListAsync();
        }
    }
}

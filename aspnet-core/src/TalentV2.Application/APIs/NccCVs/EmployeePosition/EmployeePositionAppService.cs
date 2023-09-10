using Abp.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.DomainServices.NccCVs.EmployeePositions;
using TalentV2.DomainServices.NccCVs.EmployeePositions.Dtos;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class EmployeePositionAppService : TalentV2AppServiceBase
    {
        private readonly IEmployeePositionManager _EmployeePositionManager;
        public EmployeePositionAppService(IEmployeePositionManager EmployeePositionManager)
        {
            _EmployeePositionManager = EmployeePositionManager;
        }
        [HttpPost]
        public async Task<GridResult<EmployeePositionDto>> GetAllPaging(GridParam param)
        {
            var query = _EmployeePositionManager
                .IQGetAll();
            return await query.GetGridResult(query, param);
        }
        [HttpPost]
        public async Task<EmployeePositionDto> Create(EmployeePositionDto input)
        {
            return await _EmployeePositionManager.Create(input);
        }
        [HttpPut]
        public async Task<EmployeePositionDto> Update(EmployeePositionDto input)
        {
            return await _EmployeePositionManager.Update(input);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(long Id)
        {
            await _EmployeePositionManager.Delete(Id);
            return new OkObjectResult("Deleted Successfully!");
        }
        [HttpGet]
        public async Task<List<EmployeePositionDto>> GetAll()
        {
            return await _EmployeePositionManager
                .IQGetAll()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}

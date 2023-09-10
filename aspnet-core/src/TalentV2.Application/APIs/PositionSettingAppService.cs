using Abp.Authorization;
using Microsoft.AspNetCore.Mvc;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization;
using TalentV2.DomainServices.PositionSettings;
using TalentV2.DomainServices.PositionSettings.Dtos;
using TalentV2.WebServices.InternalServices.LMS.Dtos;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class PositionSettingAppService : TalentV2AppServiceBase
    {
        private readonly IPositionSettingManager _positionSettingManager;
        public PositionSettingAppService(IPositionSettingManager positionSettingManager)
        {
            _positionSettingManager = positionSettingManager;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_PositionSettings_ViewList)]
        public async Task<GridResult<PositionSettingDto>> GetAllPaging(GridParam param)
        {
            var query = _positionSettingManager.IQGetAll();
            return await query.GetGridResult(query, param);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_PositionSettings_Create)]
        public async Task<PositionSettingDto> Create(CreatePositionSettingDto input)
        {
            return await _positionSettingManager.CreatePositionSetting(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_PositionSettings_Edit)]
        public async Task<PositionSettingDto> Update(UpdatePositionSettingDto input)
        {
            return await _positionSettingManager.UpdatePositionSetting(input);  
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_PositionSettings_Delete)]
        public async Task<string> Delete(long id)
        {
            await _positionSettingManager.DeletePositionSetting(id);
            return "Deleted Successfully";
        } 
        [HttpGet]
        public async Task<List<CourseDto>> GetListCourse()
        {
            return await _positionSettingManager.GetListCourse();   
        }
    }
}

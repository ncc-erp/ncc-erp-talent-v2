using Abp.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Categories;
using TalentV2.DomainServices.Categories.Dtos;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class CapabilitySettingAppService : TalentV2AppServiceBase
    {
        private readonly ICategoryManager _categoryManager;
        public CapabilitySettingAppService(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_CapabilitySettings_ViewList)]
        public async Task<GridResult<CapabilitySettingDto>> GetAllPaging(GridParam param)
        {
            var query = _categoryManager
                .IQGetAllCapabilitySetting();
            return await query.GetGridResult(query, param);
        }
        [HttpGet]
        public async Task<List<GetPagingCapabilitySettingDto>> GetAllCapabilitySettings(string capabilityName, UserType? userType, long? subPositionId, bool? fromType)
        {
            var capabilitySettings = await _categoryManager
                .IQGetAllCapabilitySettingsGroupBy(capabilityName, fromType)
                .Where(q => userType.HasValue ? q.UserType == userType.Value : true)
                .Where(q => subPositionId.HasValue ? q.SubPositionId == subPositionId.Value : true)
                .ToListAsync();
            return capabilitySettings;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_CapabilitySettings_Create)]
        public async Task<CapabilitySettingDto> Create(CreateUpdateCapabilitySettingDto input)
        {
            return await _categoryManager.CreateCapabilitySetting(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_CapabilitySettings_Edit)]
        public async Task<CapabilitySettingDto> Update(CreateUpdateCapabilitySettingDto input)
        {
            return await _categoryManager.UpdateCapabilitySetting(input);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_CapabilitySettings_Delete)]
        public async Task<string> Delete(long Id)
        {
            await _categoryManager.DeleteCapabilitySetting(Id);
            return "Deleted Successfully!";
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Pages_CapabilitySettings_Delete)]
        public async Task<string> DeleteGroupCapabiliSettings(UserType userType, long subPositionId)
        {
            await _categoryManager.DeleteGroupCapabilitySettings(userType, subPositionId);
            return "Deleted Successfully";
        }
        [HttpGet]
        public List<CategoryDto> GetUserType()
        {
            return _categoryManager.GetUserType();
        }
        [HttpGet]
        public async Task<List<CapabilitySettingDto>> GetCapabilitiesByUserTypeAndPositionId(UserType userType, long subPositionId)
        {
            return await _categoryManager.GetCapabilitiesByUserTypeAndPositionId(userType, subPositionId);
        }
        [HttpGet]
        public async Task<List<CapabilitySettingDto>> GetRemainCapabilitiesByUserTypeAndPositionId(UserType userType, long subPositionId)
        {
            return await _categoryManager.GetRemainCapabilitiesByUserTypeAndPositionId(userType, subPositionId);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_CapabilitySettings_Clone)]
        public async Task<ResponseCapabilitySettingCloneDto> CapabilitySettingClone(CapabilitySettingCloneDto input)
        {
            var listCapabilitiesSelected = await _categoryManager.GetCapabilitySettingClone(input);
            foreach (var capabilitiesSelected in listCapabilitiesSelected)
            {
                var inputCreateCapabilitySetting = new CreateUpdateCapabilitySettingDto
                {
                    SubPositionId = input.ToSubPositionId,
                    UserType = input.ToUserType,
                    CapabilityId = capabilitiesSelected.CapabilityId.Value,
                    Id = 0
                };
                await _categoryManager.CreateCapabilitySetting(inputCreateCapabilitySetting);
            }

            return new ResponseCapabilitySettingCloneDto
            {
                FromUserType = input.FromUserType,
                FromSubPositionId = input.FromSubPositionId,
                ToUserType = input.ToUserType,
                ToSubPositionId = input.ToSubPositionId,
                ToSubPositionName = _categoryManager.GetSubPositionName(input.ToSubPositionId),
                FromSubPositionName = _categoryManager.GetSubPositionName(input.FromSubPositionId),
            };
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_CapabilitySettings_EditFactor)]
        public async Task<string> UpdateFactor(List<CreateUpdateCapabilitySettingDto> input)
        {
            await _categoryManager.UpdateFactor(input);
            return "Update Successful!";
        }
    }
}

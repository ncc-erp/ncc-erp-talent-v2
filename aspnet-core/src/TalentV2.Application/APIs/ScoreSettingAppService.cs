using Abp.Authorization;
using Microsoft.AspNetCore.Mvc;
using NccCore.Extension;
using NccCore.Paging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.Authorization;
using TalentV2.DomainServices.ScoreSettings;
using TalentV2.DomainServices.ScoreSettings.Dtos;

namespace TalentV2.APIs
{
    [AbpAuthorize(PermissionNames.Pages_ScoreSettings)]
    public class ScoreSettingAppService : TalentV2AppServiceBase
    {
        private readonly ScoreSettingManager _manager;

        public ScoreSettingAppService(ScoreSettingManager manager)
        {
            _manager = manager;
        }

        [AbpAuthorize(PermissionNames.Pages_ScoreSettings_ViewList)]
        [HttpGet]
        public IEnumerable<ScoreSettingDto> GetAll()
        {
            return _manager.IQGetScoreSetting().ToList();
        }

        [AbpAuthorize(PermissionNames.Pages_ScoreSettings_ViewList)]
        [HttpPost]
        public async Task<GridResult<ScoreSettingDto>> GetAllPaging(GridParam param)
        {
            var query = _manager.IQGetScoreSetting();
            if (param.Sort == "userTypeName")
            {
                param.Sort = "userType";
            }
            return await query.GetGridResult(query, param);
        }

        [AbpAuthorize(PermissionNames.Pages_ScoreSettings_ViewList)]
        [HttpGet]
        public IEnumerable<ScoreRangeDto> GetAllRange(GetScoreRangeDto input)
        {
            return _manager.GetScoreRange(input).ToList();
        }

        [AbpAuthorize(PermissionNames.Pages_ScoreSettings_Create)]
        [HttpPost]
        public async Task<ScoreSettingDto> Create(CreateScoreRangeDto input)
        {
            return await _manager.CreateScoreRange(input);
        }

        [AbpAuthorize(PermissionNames.Pages_ScoreSettings_Edit)]
        [HttpPut]
        public async Task<ScoreSettingDto> Update(UpdateScoreRangeDto input)
        {
            return await _manager.UpdateScoreRange(input);
        }

        [AbpAuthorize(PermissionNames.Pages_ScoreSettings_Delete)]
        [HttpDelete]
        public async Task<string> Delete(long id)
        {
            await _manager.DeleteScoreRange(id);
            return "Deleted successful";
        }
    }
}

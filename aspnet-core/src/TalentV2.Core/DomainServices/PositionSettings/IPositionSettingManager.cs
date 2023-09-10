using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.DomainServices.PositionSettings.Dtos;
using TalentV2.WebServices.InternalServices.LMS.Dtos;

namespace TalentV2.DomainServices.PositionSettings
{
    public interface IPositionSettingManager : IDomainService
    {
        Task<List<CourseDto>> GetListCourse();
        Task<PositionSettingDto> CreatePositionSetting(CreatePositionSettingDto input);
        Task DeletePositionSetting(long id);
        Task<PositionSettingDto> UpdatePositionSetting(UpdatePositionSettingDto input);
        IQueryable<PositionSettingDto> IQGetAll();
    }
}

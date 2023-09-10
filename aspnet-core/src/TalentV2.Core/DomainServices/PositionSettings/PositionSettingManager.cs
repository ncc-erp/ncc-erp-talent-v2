using Abp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.DomainServices.PositionSettings.Dtos;
using TalentV2.Entities;
using TalentV2.WebServices.InternalServices.LMS;
using TalentV2.WebServices.InternalServices.LMS.Dtos;

namespace TalentV2.DomainServices.PositionSettings
{
    public class PositionSettingManager : BaseManager, IPositionSettingManager
    {
        private readonly LMSService _lmsService;
        public PositionSettingManager(LMSService lmsService)
        {
            _lmsService = lmsService;
        }
        public async Task<List<CourseDto>> GetListCourse()
        {
            return await _lmsService.GetListCourse();   
        }
        public async Task<PositionSettingDto> CreatePositionSetting(CreatePositionSettingDto input)
        {
            if(WorkScope.GetAll<PositionSetting>()
                .Any(s => s.SubPositionId == input.SubPositionId && s.UserType == input.UserType))
            {
                throw new UserFriendlyException("SubPosition or UserType have already existed!");
            }

            var positionSetting = ObjectMapper.Map<PositionSetting>(input);
            await WorkScope.InsertAsync(positionSetting);

            CurrentUnitOfWork.SaveChanges();
            return IQGetAll()
                .Where(q => q.Id == positionSetting.Id)
                .FirstOrDefault();
        }
        public async Task DeletePositionSetting(long id)
        {
            await WorkScope.DeleteAsync<PositionSetting>(id); 
        }
        public async Task<PositionSettingDto> UpdatePositionSetting(UpdatePositionSettingDto input)
        {
            if (WorkScope.GetAll<PositionSetting>()
                .Any(s => s.Id != input.Id && s.SubPositionId == input.SubPositionId && s.UserType == input.UserType))
            {
                throw new UserFriendlyException("SubPosition or UserType have already existed!");
            }

            var positionSetting = await WorkScope.GetAsync<PositionSetting>(input.Id);
            ObjectMapper.Map<UpdatePositionSettingDto, PositionSetting>(input, positionSetting);
            CurrentUnitOfWork.SaveChanges();
            return IQGetAll()
                .Where(q => q.Id == positionSetting.Id)
                .FirstOrDefault();
        }
        public IQueryable<PositionSettingDto> IQGetAll()
        {
            return WorkScope.GetAll<PositionSetting>()
                .Select(x => new PositionSettingDto
                {
                    Id = x.Id,
                    UserType = x.UserType,
                    DiscordInfo = x.DiscordInfo,
                    IMSInfo = x.IMSInfo,
                    LMSCourseId = x.LMSCourseId,
                    LMSCourseName = x.LMSCourseName,
                    ProjectInfo = x.ProjectInfo,
                    SubPositionId = x.SubPositionId,
                    SubPositionName = x.SubPosition.Name,
                    PositionId = x.SubPosition.PositionId,
                    PositionName = x.SubPosition.Position.Name,
                });
        }
    }
}

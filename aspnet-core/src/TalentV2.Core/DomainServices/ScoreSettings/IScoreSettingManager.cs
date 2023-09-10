using Abp.Domain.Services;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.DomainServices.ScoreSettings.Dtos;

namespace TalentV2.DomainServices.ScoreSettings
{
    internal interface IScoreSettingManager : IDomainService
    {
        public IQueryable<ScoreSettingDto> IQGetScoreSetting();
        public Task<ScoreSettingDto> CreateScoreRange(CreateScoreRangeDto scoreSettingDto);
        public Task<ScoreSettingDto> UpdateScoreRange(UpdateScoreRangeDto scoreSettingDto);
        public Task DeleteScoreRange(long id);
        public IQueryable<ScoreRangeDto> GetScoreRange(GetScoreRangeDto input);
    }
}

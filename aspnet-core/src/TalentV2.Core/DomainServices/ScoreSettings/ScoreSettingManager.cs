using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.Constants.Const;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.ScoreSettings.Dtos;
using TalentV2.Entities;
using TalentV2.Utils;

namespace TalentV2.DomainServices.ScoreSettings
{
    public class ScoreSettingManager : BaseManager, IScoreSettingManager
    {
        public async Task<ScoreSettingDto> CreateScoreRange(CreateScoreRangeDto input)
        {
            ScoreSetting scoreSetting = WorkScope.GetAll<ScoreSetting>()
                .Include(e => e.ScoreRanges)
                .FirstOrDefault(s => s.SubPosition.Id == input.SubPositionId && s.UserType == input.UserType);
            if (scoreSetting == null)
            {
                scoreSetting = new ScoreSetting
                {
                    SubPosition = WorkScope.GetAll<SubPosition>().First(s => s.Id == input.SubPositionId),
                    UserType = input.UserType,
                };
            }
            else
            {
                if (scoreSetting.ScoreRanges.Any(s => !s.IsDeleted && s.Level == (Level)input.Level))
                {
                    throw new UserFriendlyException("This level has been existed!");
                }
            }
            ValidateRange(input.ScoreFrom, input.ScoreTo, scoreSetting.ScoreRanges);
            var scoreRange = new ScoreRange
            {
                ScoreFrom = input.ScoreFrom,
                ScoreTo = input.ScoreTo,
                ScoreSetting = scoreSetting,
                Level = (Level)input.Level
            };
            await WorkScope.InsertAsync(scoreRange);
            return IQGetScoreSetting().Where(s => s.Id == scoreRange.Id).FirstOrDefault();
        }

        public async Task DeleteScoreRange(long id)
        {
            var scoreRange = await WorkScope.GetAsync<ScoreRange>(id);
            await WorkScope.DeleteAsync<ScoreRange>(id);
            var scoreSetting = WorkScope.GetAll<ScoreSetting>()
                          .Where(s => s.Id == scoreRange.ScoreSettingID && !s.IsDeleted)
                          .Include(s => s.ScoreRanges)
                          .FirstOrDefault();
            if (scoreSetting != null && scoreSetting.ScoreRanges.Count == 1)
            {
                await WorkScope.DeleteAsync<ScoreSetting>(scoreSetting.Id);
            }
        }

        public IQueryable<ScoreSettingDto> IQGetScoreSetting()
        {
            return WorkScope.GetAll<ScoreSetting>().Select(s => new ScoreSettingDto
            {
                Id = s.Id,
                UserType = s.UserType,
                UserTypeName = CommonUtils.GetEnumName(s.UserType),
                SubPositionId = s.SubPosition.Id,
                SubPositionName = s.SubPosition.Name,
                ScoreRanges = s.ScoreRanges.Select(s => new ScoreRangeDto
                {
                    ScoreFrom = s.ScoreFrom,
                    ScoreTo = s.ScoreTo,
                    Id = s.Id,
                    Level = s.Level,
                }).OrderBy(s => s.ScoreFrom)
                .ToList()
            });
            ;
        }

        public async Task<ScoreSettingDto> UpdateScoreRange(UpdateScoreRangeDto input)
        {
            var scoreRange = await WorkScope.GetAsync<ScoreRange>(input.ScoreRangeId);
            if (scoreRange == null)
            {
                throw new UserFriendlyException("Score Range have not already existed!");
            }
            var scoreSetting = WorkScope.GetAll<ScoreSetting>()
                .Include(s => s.ScoreRanges)
                .FirstOrDefault(s => s.Id == scoreRange.ScoreSettingID);
            var scoreRanges = scoreSetting.ScoreRanges.Where(s => s.Id != scoreRange.Id);
            ValidateRange(input.ScoreFrom, input.ScoreTo, scoreRanges);
            ObjectMapper.Map(input, scoreRange);
            await WorkScope.UpdateAsync(scoreRange);
            return IQGetScoreSetting().FirstOrDefault(s => s.Id == scoreSetting.Id);
        }

        public IQueryable<ScoreRangeDto> GetScoreRange(GetScoreRangeDto param)
        {
            return WorkScope.GetAll<ScoreSetting>()
                .Where(s => s.UserType == param.UserType)
                .Where(s => s.SubPosition.Id == param.SubPositionId)
                .SelectMany(s => s.ScoreRanges.Select(s => new ScoreRangeDto
                {
                    Id = s.Id,
                    ScoreFrom = s.ScoreFrom,
                    ScoreTo = s.ScoreTo,
                    Level = s.Level,
                }))
                .OrderBy(s => s.ScoreFrom);
        }

        private void ValidateRange(float scoreFrom, float scoreTo, IEnumerable<ScoreRange> scoreRanges)
        {
            if (scoreFrom * 10 % 5 != 0 || scoreTo * 10 % 5 != 0)
            {
                throw new UserFriendlyException("Score must be *.0 or *.5");
            }
            if (scoreFrom >= scoreTo)
            {
                throw new UserFriendlyException("Score From must greater than to Score To");
            }

            if (scoreTo > TalentConstants.MAX_SCORE)
            {
                throw new UserFriendlyException($"Score must be less than {TalentConstants.MAX_SCORE}");
            }
            if (scoreRanges != null && !scoreRanges.All(s => (scoreTo <= s.ScoreFrom || scoreFrom >= s.ScoreTo)))
            {
                throw new UserFriendlyException("Score range has not been overlap");
            }
        }
    }
}

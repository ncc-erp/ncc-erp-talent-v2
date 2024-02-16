using Abp.Dependency;
using TalentV2.DomainServices.Categories.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Services;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.DomainServices.Posts;
using TalentV2.DomainServices.Users.Dtos;
using static TalentV2.DomainServices.Categories.Dtos.CandidateLanguageDto;

namespace TalentV2.DomainServices.Categories
{
    public interface ICategoryManager : IDomainService
    {
        IQueryable<PositionDto> IQGetAllPosition();
        Task<PositionDto> CreatePosition(PositionDto inputPosition);
        Task<PositionDto> UpdatePosition(PositionDto inputPosition);
        Task DeletePosition(long Id);
        IQueryable<SubPositionDto> IQGetAllSubPosition();
        Task<SubPositionDto> CreateSubPosition(SubPositionDto inputSubPosition);
        Task<SubPositionDto> UpdateSubPosition(SubPositionDto inputSubPosition);
        Task DeleteSubPosition(long Id);
        IQueryable<CategoryDto> IQGetAllEducationType();
        Task<CategoryDto> CreateEducationType(EducationTypeDto inputEducationType);
        Task<CategoryDto> UpdateEducationType(EducationTypeDto inputEducationType);
        Task DeleteEducationType(long Id);
        IQueryable<EducationDto> IQGetAllEducation();
        Task<EducationDto> CreateEducation(CreateUpdateEducationDto inputEducation);
        Task<EducationDto> UpdateEducation(CreateUpdateEducationDto inputEducation);
        Task DeleteEducation(long Id);
        IQueryable<CategoryDto> IQGetAllSkills();
        Task<CategoryDto> CreateSkill(SkillDto inputSkill);
        Task<CategoryDto> UpdateSkill(SkillDto inputSkill);
        Task DeleteSkill(long Id);
        IQueryable<CVSourceDto> IQGetAllCVSources();
        Task<CVSourceDto> CreateCVSource(CVSourceDto inputCVSource);
        Task<CVSourceDto> UpdateCVSource(CVSourceDto inputCVSource);
        Task DeleteCVSource(long Id);
        IQueryable<CapabilityDto> IQGetAllCapability();
        Task<CapabilityDto> CreateCapability(CapabilityDto inputCapability);
        Task<CapabilityDto> UpdateCapability(CapabilityDto inputCapability);
        Task DeleteCapability(long Id);
        IQueryable<CapabilitySettingDto> IQGetAllCapabilitySetting();
        Task<CapabilitySettingDto> CreateCapabilitySetting(CreateUpdateCapabilitySettingDto input);
        Task<CapabilitySettingDto> UpdateCapabilitySetting(CreateUpdateCapabilitySettingDto input);
        Task DeleteCapabilitySetting(long Id);
        List<CategoryDto> GetUserType();
        IQueryable<BranchDto> IQGetAllBranches();
        Task<BranchDto> CreateBranch(CreateBranchDto inputBranch);
        Task<BranchDto> UpdateBranch(UpdateBranchDto inputBranch);
        Task DeleteBranch(long Id);
        IQueryable<GetPagingCapabilitySettingDto> IQGetAllCapabilitySettingsGroupBy(string capabilityName = "", bool? fromType = null);
        Task DeleteGroupCapabilitySettings(UserType userType, long subPositionId);
        Task<List<CapabilitySettingDto>> GetCapabilitiesByUserTypeAndPositionId(UserType userType, long subPositionId);
        Task<List<CapabilitySettingDto>> GetRemainCapabilitiesByUserTypeAndPositionId(UserType userType, long subPositionId);
        Task<List<CapabilitySettingDto>> GetCapabilitySettingClone(CapabilitySettingCloneDto inputCapabilitySettingClone);
        string GetSubPositionName(long subPositionId);
        Task UpdateFactor(List<CreateUpdateCapabilitySettingDto> input);

        IQueryable<PostDto> IQGetAllPosts();
        Task<PostDto> CreatePost(CreatePostDto inputPost);
        Task<PostDto> UpdatePost(UpdatePostDto inputPost);
        Task<PostDto> UpdatePostsMetadata(UpdatePostsMetadataDto inputPost);
        Task DeletePost(long postId);
        Task<List<UserReferenceDto>> GetAllRecruitmentUser();
        IQueryable<CandidateLanguageDto> IQGetAllLanguage();
        Task<CandidateLanguageDto> CreateLanguage(CreateLanguageDto inputPost);
        Task<CandidateLanguageDto> UpdateLanguage(UpdateLanguageDto inputPost);
        Task DeleteLanguage(long Id);
    }
}

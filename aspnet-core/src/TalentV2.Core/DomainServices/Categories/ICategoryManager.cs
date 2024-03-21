using Abp.Dependency;
using TalentV2.DomainServices.Categories.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Services;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Users.Dtos;

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
        Task DeleteEducation(long Id);
        IQueryable<CategoryDto> IQGetAllSkills();
        Task<CategoryDto> CreateSkill(SkillDto inputSkill);
        Task<CategoryDto> UpdateSkill(SkillDto inputSkill);
        Task DeleteSkill(long Id);
        IQueryable<CVSourceDto> IQGetAllCVSources();
        Task<CVSourceDto> CreateCVSource(CVSourceDto inputCVSource);
        Task<CVSourceDto> UpdateCVSource(CVSourceDto inputCVSource);
        Task DeleteCVSource(long Id);

        Task DeleteCapabilitySetting(long Id);
        List<CategoryDto> GetUserType();
        IQueryable<BranchDto> IQGetAllBranches();
        Task<BranchDto> CreateBranch(CreateBranchDto inputBranch);
        Task<BranchDto> UpdateBranch(UpdateBranchDto inputBranch);
        Task DeleteBranch(long Id);
        Task DeleteGroupCapabilitySettings(UserType userType, long subPositionId);
        IQueryable<PostDto> IQGetAllPosts();
        Task<PostDto> CreatePost(CreatePostDto inputPost);
        Task<PostDto> UpdatePost(UpdatePostDto inputPost);
        Task DeletePost(long postId);
        Task<List<UserReferenceDto>> GetAllRecruitmentUser();
    }
}

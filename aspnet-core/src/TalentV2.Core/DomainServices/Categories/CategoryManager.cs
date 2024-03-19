using Abp.UI;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Abp.Domain.Entities;
using System.Linq.Expressions;
using System.Reflection;
using NccCore.Extension;
using TalentV2.Authorization.Users;
using Microsoft.AspNetCore.Mvc;
using TalentV2.Authorization.Roles;
using TalentV2.DomainServices.Users.Dtos;

namespace TalentV2.DomainServices.Categories
{
    public class CategoryManager : BaseManager, ICategoryManager
    {
        private readonly IRepository<CapabilitySetting, long> _capabilitySettingRepository;
        private readonly IRepository<Role> _roleRepository;
        public CategoryManager(IRepository<CapabilitySetting, long> capabilitySettingRepository, IRepository<Role> repository)
        {
            _capabilitySettingRepository = capabilitySettingRepository;
            _roleRepository = repository;
        }

        public IQueryable<PositionDto> IQGetAllPosition()
        {
            var qallPositions = from q in WorkScope.GetAll<Position>()
                                select new PositionDto
                                {
                                    Id = q.Id,
                                    Name = q.Name,
                                    Code = q.Code,
                                    ColorCode = q.ColorCode
                                };
            return qallPositions;
        }
        public async Task<PositionDto> CreatePosition(PositionDto inputPosition)
        {
            await CheckDuplicateNameCategory<Position>(inputPosition.Name);
            var position = ObjectMapper.Map<Position>(inputPosition);

            var id = await WorkScope.InsertAndGetIdAsync<Position>(position);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await IQGetAllPosition()
                .Where(q => q.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task<PositionDto> UpdatePosition(PositionDto inputPosition)
        {
            await CheckDuplicateNameCategory<Position>(inputPosition.Name, inputPosition.Id);
            var position = await WorkScope.GetAsync<Position>(inputPosition.Id);
            ObjectMapper.Map<PositionDto, Position>(inputPosition,position);
            await WorkScope.UpdateAsync(position);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await IQGetAllPosition()
                .Where(q => q.Id == position.Id)
                .FirstOrDefaultAsync();
        }
        public async Task DeletePosition(long Id)
        {
            var isExisted = await WorkScope.GetAll<Position>()
                .Where(q => q.Id == Id)
                .Where(q => q.SubPositions.Any(s => !s.IsDeleted))
                .AnyAsync();
            if (isExisted)
                throw new UserFriendlyException($"Not Deleted, Position already exists somewhere");
            var position = await WorkScope.GetAsync<Position>(Id);
            position.IsDeleted = true;

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public IQueryable<SubPositionDto> IQGetAllSubPosition()
        {
            var qallSubPositions = from q in WorkScope.GetAll<SubPosition>()
                                select new SubPositionDto
                                {
                                    Id = q.Id,
                                    Name = q.Name,
                                    ColorCode = q.ColorCode,
                                    PositionId = q.PositionId,
                                    PositionName = q.Position.Name
                                };
            return qallSubPositions;
        }
        public async Task<SubPositionDto> CreateSubPosition(SubPositionDto inputSubPosition)
        {
            await CheckDuplicateNameCategory<SubPosition>(inputSubPosition.Name);
            var subPosition = ObjectMapper.Map<SubPosition>(inputSubPosition);

            var id = await WorkScope.InsertAndGetIdAsync<SubPosition>(subPosition);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await IQGetAllSubPosition()
                .Where(q => q.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task<SubPositionDto> UpdateSubPosition(SubPositionDto inputSubPosition)
        {
            await CheckDuplicateNameCategory<SubPosition>(inputSubPosition.Name, inputSubPosition.Id);
            var subPosition = await WorkScope.GetAsync<SubPosition>(inputSubPosition.Id);
            ObjectMapper.Map<SubPositionDto, SubPosition>(inputSubPosition, subPosition);
            await WorkScope.UpdateAsync(subPosition);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await IQGetAllSubPosition()
                .Where(q => q.Id == subPosition.Id)
                .FirstOrDefaultAsync();
        }
        public async Task DeleteSubPosition(long Id)
        {
            var isExisted = await WorkScope.GetAll<SubPosition>()
                .Where(q => q.Id == Id)
                .Where(q => q.CapabilitySettings.Any(s => !s.IsDeleted) || 
                            q.CVs.Any(s => !s.IsDeleted) ||
                            q.Requests.Any(s => !s.IsDeleted))
                .AnyAsync();
            if (isExisted)
                throw new UserFriendlyException($"Not Deleted, Position already exists somewhere");
            var subPosition = await WorkScope.GetAsync<SubPosition>(Id);
            await WorkScope.DeleteAsync(subPosition);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public IQueryable<CategoryDto> IQGetAllEducationType()
        {
            var qallEducationTypes = from q in WorkScope.GetAll<EducationType>()
                                     select new CategoryDto
                                     {
                                         Id = q.Id,
                                         Name = q.Name,
                                     };
            return qallEducationTypes;
        }
        public async Task<CategoryDto> CreateEducationType(EducationTypeDto inputEducationType)
        {
            await CheckDuplicateNameCategory<EducationType>(inputEducationType.Name);
            var educationType =ObjectMapper.Map<EducationType>(inputEducationType);

            var id = await WorkScope.InsertAndGetIdAsync<EducationType>(educationType);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await IQGetAllEducationType()
                .Where(q => q.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task<CategoryDto> UpdateEducationType(EducationTypeDto inputEducationType)
        {
            await CheckDuplicateNameCategory<EducationType>(inputEducationType.Name, inputEducationType.Id);
            var educationType = await WorkScope.GetAsync<EducationType>(inputEducationType.Id);
            ObjectMapper.Map<EducationTypeDto,EducationType>(inputEducationType,educationType);

            await WorkScope.UpdateAsync(educationType);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await IQGetAllEducationType()
                .Where(q => q.Id == educationType.Id)
                .FirstOrDefaultAsync();
        }
        public async Task DeleteEducationType(long Id)
        {
            var isExisted = await WorkScope.GetAll<EducationType>()
                .Where(q => q.Id == Id)
                .Where(q => q.Educations.Any(s => !s.IsDeleted))
                .AnyAsync();
            if (isExisted)
                throw new UserFriendlyException($"Not Deleted, Education Type already exists somewhere");
            var educationType = await WorkScope.GetAsync<EducationType>(Id);
            educationType.IsDeleted = true;
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public IQueryable<EducationDto> IQGetAllEducation()
        {
            var qallEducations = from q in WorkScope.GetAll<Education>()
                                 select new EducationDto
                                 {
                                     Id = q.Id,
                                     Name = q.Name,
                                     ColorCode = q.ColorCode,
                                     EducationTypeId = q.EducationType.Id,
                                     EducationTypeName = q.EducationType.Name,
                                 };
            return qallEducations;
        }
       
        public async Task DeleteEducation(long Id)
        {
            var isExisted = await WorkScope.GetAll<Education>()
                .Where(q => q.Id == Id)
                .Where(q => q.CVEducations.Any(s => !s.IsDeleted) )
                .AnyAsync();
            if (isExisted)
                throw new UserFriendlyException($"Not Deleted, Education already exists somewhere");
            var education = await WorkScope.GetAsync<Education>(Id);

            education.IsDeleted = true;
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public IQueryable<CategoryDto> IQGetAllSkills()
        {
            var qgetAllSkills = from s in WorkScope.GetAll<Skill>()
                                select new CategoryDto
                                {
                                    Id = s.Id,
                                    Name = s.Name,
                                };
            return qgetAllSkills;
        }
        public async Task<CategoryDto> CreateSkill(SkillDto inputSkill)
        {
            await CheckDuplicateNameCategory<Skill>(inputSkill.Name);
            var skill = ObjectMapper.Map<Skill>(inputSkill);

            var id = await WorkScope.InsertAndGetIdAsync<Skill>(skill);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await IQGetAllSkills()
                .Where(q => q.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task<CategoryDto> UpdateSkill(SkillDto inputSkill)
        {
            await CheckDuplicateNameCategory<Skill>(inputSkill.Name, inputSkill.Id);
            var skill = await WorkScope.GetAsync<Skill>(inputSkill.Id);
            ObjectMapper.Map<SkillDto,Skill>(inputSkill,skill);
            
            await WorkScope.UpdateAsync(skill);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await IQGetAllSkills()
                .Where(q => q.Id == skill.Id)
                .FirstOrDefaultAsync();
        }
        public async Task DeleteSkill(long Id)
        {
            var isExisted = await WorkScope.GetAll<Skill>()
                .Where(q => q.Id == Id)
                .Where(q => q.CVSkills.Any(s => !s.IsDeleted) || q.RequestSkills.Any(s => !s.IsDeleted))
                .AnyAsync();
            if (isExisted)
                throw new UserFriendlyException($"Not Deleted, Skill already exists somewhere");
            var skill = await WorkScope.GetAsync<Skill>(Id);
            skill.IsDeleted = true;
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public IQueryable<CVSourceDto> IQGetAllCVSources()
        {
            var qgetAllCVSources = from cvs in WorkScope.GetAll<CVSource>()
                                   select new CVSourceDto
                                   {
                                       Id = cvs.Id,
                                       Name= cvs.Name,
                                       ReferenceType = cvs.ReferenceType,
                                       ColorCode = cvs.ColorCode,
                                   };
            return qgetAllCVSources;
        }
        public async Task<CVSourceDto> CreateCVSource(CVSourceDto inputCVSource)
        {
            await CheckDuplicateNameCategory<CVSource>(inputCVSource.Name);
            var cvSource = ObjectMapper.Map<CVSource>(inputCVSource);

            var id = await WorkScope.InsertAndGetIdAsync<CVSource>(cvSource);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await IQGetAllCVSources()
                .Where(q => q.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task<CVSourceDto> UpdateCVSource(CVSourceDto inputCVSource)
        {
            await CheckDuplicateNameCategory<CVSource>(inputCVSource.Name, inputCVSource.Id);
            var cvSource = await WorkScope.GetAsync<CVSource>(inputCVSource.Id);
            ObjectMapper.Map<CVSourceDto,CVSource>(inputCVSource,cvSource);

            await WorkScope.UpdateAsync(cvSource);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await IQGetAllCVSources()
                .Where(q => q.Id == cvSource.Id)
                .FirstOrDefaultAsync();
        }
        public async Task DeleteCVSource(long Id)
        {
            var isExisted = await WorkScope.GetAll<CVSource>()
                .Where(q => q.Id == Id)
                .Where(q => q.CVs.Any(s => !s.IsDeleted) )
                .AnyAsync();
            if (isExisted)
                throw new UserFriendlyException($"Not Deleted, CVSource already exists somewhere");
            var cvSource = await WorkScope.GetAsync<CVSource>(Id);
            cvSource.IsDeleted = true;
            await CurrentUnitOfWork.SaveChangesAsync();
        }
      
        public async Task DeleteCapabilitySetting(long Id)
        {
            var capabilitySetting = await WorkScope.GetAsync<CapabilitySetting>(Id);
            capabilitySetting.IsDeleted = true;
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public async Task DeleteGroupCapabilitySettings(UserType userType, long subPositionId)
        {
            var capabilitySettings = await WorkScope.GetAll<CapabilitySetting>()
                .Where(q => q.UserType == userType && q.SubPositionId == subPositionId)
                .ToListAsync();

            await WorkScope.DeleteRangeAsync(capabilitySettings);

            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public List<CategoryDto> GetUserType()
        {
            var userTypes = Enum.GetValues(typeof(UserType))
                .Cast<UserType>()
                .Select(x => new CategoryDto { 
                    Id = (long)x,
                    Name = x.ToString()
                })
                .ToList();
            return userTypes;
        }
        public IQueryable<BranchDto> IQGetAllBranches()
        {
            var branches = from b in WorkScope.GetAll<Branch>()
                           select new BranchDto
                           {
                               DisplayName = b.DisplayName,
                               Id = b.Id,
                               Name = b.Name,
                               ColorCode = b.ColorCode,
                               Address = b.Address
                           };
            return branches;
        }
        public async Task<BranchDto> CreateBranch(CreateBranchDto inputBranch)
        {
            await CheckDuplicateNameCategory<Branch>(inputBranch.Name);
            var branch = ObjectMapper.Map<Branch>(inputBranch);
            var id = await WorkScope.InsertAndGetIdAsync<Branch>(branch);
            await CurrentUnitOfWork.SaveChangesAsync();
            return await IQGetAllBranches()
                .Where(q => q.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task<BranchDto> UpdateBranch(UpdateBranchDto inputBranch)
        {
            await CheckDuplicateNameCategory<Branch>(inputBranch.Name, inputBranch.Id);
            var branch = await WorkScope.GetAsync<Branch>(inputBranch.Id);
            ObjectMapper.Map<UpdateBranchDto,Branch>(inputBranch,branch);
            await WorkScope.UpdateAsync(branch);
            await CurrentUnitOfWork.SaveChangesAsync();
            return await IQGetAllBranches()
                    .Where(q => q.Id == branch.Id)
                    .FirstOrDefaultAsync();
        }
        public async Task DeleteBranch(long Id)
        {
            var isExisted = await WorkScope.GetAll<Branch>()
                .Where(q => q.Id == Id)
                .Where(q => q.Requests.Any(s => !s.IsDeleted)||
                            q.CVs.Any(s => !s.IsDeleted))
                .AnyAsync();
            if (isExisted)
                throw new UserFriendlyException($"Not Deleted, Branch already exists somewhere");
            await WorkScope.DeleteAsync<Branch>(Id);
        }
       

        private async Task CheckDuplicateUrl<IEntity>(string name, long id = default)
            where IEntity : class, IEntity<long>
        {
            name = name.Trim();
            var query = WorkScope.GetAll<IEntity>();
            var param = Expression.Parameter(typeof(IEntity), "x");
            var value = Expression.Property(param, "Url");
            MethodInfo equalsMethod = typeof(string).GetMethod("Equals", new[] { typeof(string) });
            var constant = Expression.Constant(name);
            var body = Expression.Call(value, equalsMethod, constant);
            var exp = Expression.Lambda<Func<IEntity, bool>>(body, param);
            query = query.Where(exp);
            if (id != default)
            {
                var idProperty = Expression.Property(param, "Id");
                var constantId = Expression.Constant(id);
                Expression notEqual = Expression.NotEqual(param, Expression.Constant(null));
                notEqual = Expression.AndAlso(notEqual, Expression.NotEqual(idProperty, constantId));
                var exp2 = Expression.Lambda<Func<IEntity, bool>>(notEqual, param);
                query = query.Where(exp2);
            }
            if(await query.AnyAsync())
            {
                throw new UserFriendlyException($"{name} existed");
            }
        }

        private async Task CheckDuplicateNameCategory<IEntity>(string name, long id = default)
            where IEntity : class, IEntity<long>
        {
            name = name.Trim();
            var query = WorkScope.GetAll<IEntity>();
            var param = Expression.Parameter(typeof(IEntity), "x");
            var value = Expression.Property(param, "Name");
            MethodInfo equalsMethod = typeof(string).GetMethod("Equals", new[] { typeof(string) });
            var constant = Expression.Constant(name);
            var body = Expression.Call(value, equalsMethod, constant);
            var exp = Expression.Lambda<Func<IEntity, bool>>(body, param);
            query = query.Where(exp);
            if (id != default)
            {
                var idProperty = Expression.Property(param, "Id");
                var constantId = Expression.Constant(id);
                Expression notEqual = Expression.NotEqual(param, Expression.Constant(null));
                notEqual = Expression.AndAlso(notEqual, Expression.NotEqual(idProperty, constantId));
                var exp2 = Expression.Lambda<Func<IEntity, bool>>(notEqual, param);
                query = query.Where(exp2);
            }
            if (await query.AnyAsync())
            {
                throw new UserFriendlyException($"{name} existed");
            }
        }
       
        public IQueryable<PostDto> IQGetAllPosts()
        {
            var posts = from b in WorkScope.GetAll<Post>()
                        join u in WorkScope.GetAll<User>() on b.CreatedByUser.Value equals u.Id 
                           select new PostDto
                           {
                               Id = b.Id,
                               PostName = b.PostName,
                               Url = b.Url,
                               CreatedByUser = b.CreatedByUser,
                               PostCreationTime = b.PostCreationTime,
                               Content = b.Content,
                               Metadata = b.Metadata,
                               Type = b.Type,
                               CreatorsName = u.UserName,
                               CreatorsEmailAddess = u.EmailAddress,
                               ApplyNumber = b.ApplyNumber
                           };
            return posts;
        }
        public async Task<PostDto> CreatePost(CreatePostDto inputPost)
        {
            if (!inputPost.CreatedByUser.HasValue || inputPost.CreatedByUser.Value == 0)
                inputPost.CreatedByUser = AbpSession.UserId;
            await CheckDuplicateUrl<Post>(inputPost.Url);
            var post = ObjectMapper.Map<Post>(inputPost);
            var id = await WorkScope.InsertAndGetIdAsync<Post>(post);
            await CurrentUnitOfWork.SaveChangesAsync();
            return await IQGetAllPosts()
                .Where(q => q.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task<PostDto> UpdatePost(UpdatePostDto inputPost)
        {
            if (!inputPost.CreatedByUser.HasValue || inputPost.CreatedByUser.Value == 0)
                inputPost.CreatedByUser = AbpSession.UserId;
            await CheckDuplicateUrl<Post>(inputPost.Url);
            var post = await WorkScope.GetAsync<Post>(inputPost.Id);
            ObjectMapper.Map<UpdatePostDto, Post>(inputPost, post);
            await WorkScope.UpdateAsync(post);
            await CurrentUnitOfWork.SaveChangesAsync();
            return await IQGetAllPosts()
                    .Where(q => q.Id == post.Id)
                    .FirstOrDefaultAsync();
        }
        public async Task DeletePost(long postId)
        {
            var isDeleted = await WorkScope.GetAll<Post>()
                .Where(q => q.Id == postId)
                .Where(q => q.IsDeleted)
                .AnyAsync();
            if (isDeleted)
                throw new UserFriendlyException($"Not Deleted, Post has been deleted");
            await WorkScope.DeleteAsync<Post>(postId);
        }


        public async Task<List<UserReferenceDto>> GetAllRecruitmentUser()
        {
            var roles = await _roleRepository.GetAllListAsync();
            var role = roles.FirstOrDefault(x => x.Name == StaticRoleNames.Tenants.Recruitment);
            if (role != null)
            {
                return await WorkScope.GetAll<User>()
                .Where(x => x.Roles.Any(s => s.RoleId == role.Id))
                .Select(s => new UserReferenceDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.EmailAddress,
                    FullName = s.FullName,
                    SurName = s.Surname
                }).ToListAsync();
            }
            else
            {
                return null;
            }
        }
    }
}

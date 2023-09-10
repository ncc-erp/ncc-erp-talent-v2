using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.Constants.Const;
using TalentV2.DomainServices.ApplyCVs.Dtos;
using TalentV2.Entities;
using TalentV2.FileServices.Services.ApplyCV;
using TalentV2.Utils;

namespace TalentV2.DomainServices.ApplyCVs
{
    public class ApplyCvManager : BaseManager, IApplyCvManager
    {
        private readonly IFileApplyCVService _fileApplyCV;
        public ApplyCvManager(IFileApplyCVService fileApplyCV)
        {
            _fileApplyCV = fileApplyCV;
        }
        public async Task<long> Create(CreateApplyCVDto createApplyCVDto)
        {
            var applyCV = ObjectMapper.Map<ApplyCV>(createApplyCVDto);
            if (!string.IsNullOrEmpty(applyCV.Phone))
                applyCV.Phone = StringExtensions.ReplaceWhitespace(applyCV.Phone);
            await WorkScope.InsertAsync<ApplyCV>(applyCV);
            CommonUtils.CheckFormatFile(createApplyCVDto.Avatar, FileTypes.IMAGE);
            CommonUtils.CheckFormatFile(createApplyCVDto.AttachCV, FileTypes.DOCUMENT);
            if (createApplyCVDto.Avatar != null)
            {
                string avatarLink = await _fileApplyCV.UploadAvatar(createApplyCVDto.Avatar);
                applyCV.Avatar = avatarLink;
            }
            if (createApplyCVDto.AttachCV != null)
            {
                string cvLink = await _fileApplyCV.UploadAttachCV(createApplyCVDto.AttachCV);
                applyCV.AttachCV = cvLink;
            }
            var post = WorkScope.GetAll<Post>().Where(q => q.Id == createApplyCVDto.PostId).FirstOrDefault();
            if (post != null)
            {
                post.ApplyNumber++;
                await WorkScope.UpdateAsync(post);
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return applyCV.Id;
        }

        public async Task<ApplyCVDto> GetApplyCVById(long applyCVId)
        {
            var applyCV = await WorkScope.GetAll<ApplyCV>()
                            .Where(q => q.Id == applyCVId)
                            .Select(s => new ApplyCVDto
                            {
                                Id = s.Id,
                                FullName = s.Name,
                                IsFemale = s.IsFemale,
                                Email = s.Email,
                                Phone = s.Phone,
                                PositionType = s.PositionType,
                                JobTitle = s.JobTitle,
                                Branch = s.Branch,
                                Avatar = s.Avatar,
                                AttachCV = s.AttachCV,
                                PostId = s.PostId,
                                ApplyCVDate = s.CreationTime
                            })
                            .FirstOrDefaultAsync();
            return applyCV;
        }

        public IQueryable<ApplyCVDto> IQGetAll()
        {
            var applyCVs = from a in WorkScope.GetAll<ApplyCV>()
                           join b in WorkScope.GetAll<Branch>()
                           on a.Branch.ToLower() equals b.DisplayName.ToLower()
                           select new ApplyCVDto
                           {
                               Id = a.Id,
                               FullName = a.Name,
                               IsFemale = a.IsFemale,
                               Email = a.Email,
                               Phone = a.Phone,
                               PositionType = a.PositionType,
                               JobTitle = a.JobTitle,
                               Branch = a.Branch,
                               BranchId = b.Id,
                               Avatar = a.Avatar,
                               AttachCV = a.AttachCV,
                               PostId = a.PostId,
                               ApplyCVDate = a.CreationTime,
                               Applied = a.Applied,
                           };
            return applyCVs;
        }

        public async Task<ApplyCVDto> GetCVById(long cvId)
        {
            var query = from a in WorkScope.GetAll<ApplyCV>()
                        join b in WorkScope.GetAll<Branch>()
                        on a.Branch.ToLower() equals b.DisplayName.ToLower()
                        where a.Id == cvId
                        select new ApplyCVDto
                        {
                            Id = a.Id,
                            FullName = a.Name,
                            IsFemale = a.IsFemale,
                            Email = a.Email,
                            Phone = a.Phone,
                            PositionType = a.PositionType,
                            Branch = a.Branch,
                            Avatar = a.Avatar,
                            AttachCV = a.AttachCV,
                            BranchId = b.Id,

                        };
            var applyCV = query.FirstOrDefault();
            return applyCV;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.Constants.Const;
using TalentV2.DomainServices.ExternalCVs.Dtos;
using TalentV2.Entities;
using TalentV2.FileServices.Services.ExternalCVs;
using TalentV2.Utils;

namespace TalentV2.DomainServices.ExternalCVs
{
    public class ExternalCVManager : BaseManager, IExternalCVManager
    {
        private readonly IFileExternalCVService _fileExternalCV;

        public ExternalCVManager(IFileExternalCVService fileExternalCV)
        {
            _fileExternalCV = fileExternalCV;
        }

        public async Task<long> CreateExternalCV(CreateExternalCVDto input)
        {
            var cv = ObjectMapper.Map<ExternalCV>(input);

            if (!string.IsNullOrEmpty(cv.Phone))
                cv.Phone = StringExtensions.ReplaceWhitespace(cv.Phone);

            await WorkScope.InsertAsync<ExternalCV>(cv);

            CommonUtils.CheckFormatFile(input.Avatar, FileTypes.IMAGE);
            CommonUtils.CheckFormatFile(input.CV, FileTypes.DOCUMENT);

            if (input.Avatar != null)
            {
                string avatarLink = await _fileExternalCV.UploadAvatar(input.Avatar);
                cv.Avatar = avatarLink;
            }
            if (input.CV != null)
            {
                string cvLink = await _fileExternalCV.UploadCV(input.CV);
                cv.LinkCV = cvLink;
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return cv.Id;
        }

        public async Task<ExternalCVDto> GetExternalCVById(long cvId)
        {
            var externalCV = await WorkScope.GetAll<ExternalCV>()
                            .Where(q => q.Id == cvId)
                            .Select(s => new ExternalCVDto
                            {
                                ExternalId = s.ExternalId,
                                Address = s.Address,
                                Avatar = s.Avatar,
                                LinkCV = s.LinkCV,
                                Birthday = s.Birthday,
                                Email = s.Email,
                                Name = s.Name,
                                IsFemale = s.IsFemale,
                                Note = s.Note,
                                Phone = s.Phone,
                                PositionName = s.PositionName,
                                BranchName = s.BranchName,
                                ReferenceName = s.ReferenceName,
                                UserTypeName = s.UserTypeName,
                                NCCEmail = s.NCCEmail,
                                CVSourceName = s.CVSourceName,
                                Metadata = s.Metadata,
                            })
                            .FirstOrDefaultAsync();

            return externalCV;
        }
        public IQueryable<ExternalCVDto> IQGetAllExternalCVs()
        {
            var externalCVs = from e in WorkScope.GetAll<ExternalCV>()
                              select new ExternalCVDto
                              {
                                  Id = e.Id,
                                  ExternalId = e.ExternalId,
                                  Address = e.Address,
                                  Avatar = e.Avatar,
                                  LinkCV = e.LinkCV,
                                  Birthday = e.Birthday,
                                  Email = e.Email,
                                  Name = e.Name,
                                  IsFemale = e.IsFemale,
                                  Note = e.Note,
                                  Phone = e.Phone,
                                  PositionName = e.PositionName,
                                  BranchName = e.BranchName,
                                  ReferenceName = e.ReferenceName,
                                  UserTypeName = e.UserTypeName,
                                  NCCEmail = e.NCCEmail,
                                  CVSourceName = e.CVSourceName,
                                  CreationTime = e.CreationTime
                              };
            return externalCVs;
        }

        public async Task<ExternalCVDto> UpdateExternalCV(UpdateExternalCVDto input)
        {
            var externalCV = await WorkScope.GetAll<ExternalCV>()
                .FirstOrDefaultAsync(e => e.ExternalId == input.ExternalId && e.CVSourceName == input.CVSourceName);

            ObjectMapper.Map<UpdateExternalCVDto, ExternalCV>(input, externalCV);

            await CurrentUnitOfWork.SaveChangesAsync();

            return await GetExternalCVById(externalCV.Id);
        }
    }
}

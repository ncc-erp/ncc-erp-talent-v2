using Abp.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Categories;
using TalentV2.DomainServices.CVAutomation.Dto;
using TalentV2.DomainServices.ExternalCVs.Dtos;
using TalentV2.DomainServices.MezonCVs.Dtos;
using TalentV2.Entities;
using TalentV2.FileServices.Services.Candidates;
using TalentV2.Utils;

namespace TalentV2.DomainServices.MezonCVs
{
    public class MezonCVManager : BaseManager, IMezonCVManager
    {
        private readonly IFileCandidateService _fileService;
        private readonly ICategoryManager _categoryManager;
        public MezonCVManager(IFileCandidateService fileService, ICategoryManager categoryManager)
        {
            _fileService = fileService;
            _categoryManager = categoryManager;
        }

        public async Task<MezonInternCVDto> GetCVById(long id)
        {
            var cv = await WorkScope.GetAll<CV>()
                            .Include(cv => cv.Branch)
                            .Include(cv => cv.SubPosition)
                            .Where(cv => cv.Id == id)
                            .Select(cv => new MezonInternCVDto
                            {
                                Id = cv.Id,
                                Address = cv.Address,
                                LinkCV = cv.LinkCV,
                                Birthday = cv.Birthday,
                                Email = cv.Email,
                                Name = cv.Name,
                                IsFemale = cv.IsFemale,
                                Note = cv.Note,
                                Phone = cv.Phone,
                                SubPositionName = cv.SubPosition.Name,
                                BranchName = cv.Branch.Name,
                                BranchId = cv.BranchId,
                                SubPositionId = cv.SubPositionId,
                                CVSourceName = cv.CVSource.Name,
                                CVSourceId = cv.CVSourceId
                            })
                            .FirstOrDefaultAsync();

            return cv;
        }

        public async Task<long> CreateMezonInternCV(CreateMezonInternCVDto input)
        {
            if (input == null) throw new UserFriendlyException("Input data cannot be null");

            if (string.IsNullOrWhiteSpace(input.Name) 
                || string.IsNullOrWhiteSpace(input.Email) 
                || string.IsNullOrWhiteSpace(input.Phone))
                throw new UserFriendlyException("Name, email and phone number are required");

            var branch = await WorkScope.GetAll<Branch>()
                .FirstOrDefaultAsync(b => b.Id == input.BranchId);
            if (branch == null) throw new UserFriendlyException($"Branch with ID {input.BranchId} not found");

            var cvSource = await WorkScope.GetAll<CVSource>()
                .FirstOrDefaultAsync(s => s.Id == input.CVSourceId);
            if (cvSource == null) throw new UserFriendlyException($"CV Source with ID {input.CVSourceId} not found");

            var subPosition = await WorkScope.GetAll<SubPosition>()
                .FirstOrDefaultAsync(p => p.Id == input.SubPositionId);
            if (subPosition == null) throw new UserFriendlyException($"Sub Position with ID {input.SubPositionId} not found");

            var cv = new CV
            {
                Name = input.Name,
                Email = input.Email,
                UserType = UserType.Intern,
                BranchId = branch.Id,
                CVSourceId = cvSource.Id,
                SubPositionId = subPosition.Id,
                Birthday = input.Birthday,
                IsFemale = input.IsFemale,
                Address = input.Address,
                Note = input.Note,
                CVStatus = CVStatus.Draft
            };

            string phoneNumber = StringExtensions.FormatPhoneNumber(input.Phone);
            cv.Phone = phoneNumber.Length > 12 ? string.Empty : phoneNumber;

            IFormFile cvFile = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(input.LinkCV))
                {
                    cvFile = await _fileService.DownloadFileAsync(input.LinkCV);
                    if (cvFile != null)
                    {
                        string cvLink = await _fileService.UploadCV(cvFile);
                        cv.LinkCV = cvLink;
                    }
                }
            }
            catch (UserFriendlyException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException($"Process link cv failed: " + ex.ToString());
            }

            await WorkScope.InsertAsync(cv);
            await CurrentUnitOfWork.SaveChangesAsync();

            return cv.Id;
        }

        public async Task<MezonCVFormDto> GetMezonCVFormData()
        {
            var branches = await _categoryManager.IQGetAllBranches()
                .OrderBy(x => x.Name)
                .ToListAsync();
            var cvSources = await _categoryManager.IQGetAllCVSources()
                .OrderBy(x => x.Name)
                .ToListAsync();
            var subPositions = await _categoryManager.IQGetAllSubPosition()
                .OrderBy(x => x.Name)
                .ToListAsync();

            return new MezonCVFormDto
            {
                Branches = branches,
                CVSources = cvSources,
                SubPositions = subPositions
            };
        }
    }
}

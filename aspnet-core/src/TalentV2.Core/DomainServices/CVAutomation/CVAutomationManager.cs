using Abp.Dependency;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NccCore.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.Authorization.Users;
using TalentV2.Configuration;
using TalentV2.Constants.Const;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.CVAutomation.Dto;
using TalentV2.Entities;
using TalentV2.FileServices.Paths;
using TalentV2.FileServices.Providers;
using TalentV2.Utils;
using TalentV2.WebServices.ExternalServices.Autobot;

namespace TalentV2.DomainServices.CVAutomation
{
    public class CVAutomationManager : BaseManager, ICVAutomationManager
    {
        private readonly string FILE_CANDIDATE_FOLDER_SERVICE = "candidates";
        private readonly string INTERN_FOLDER_SERVICE = "cv_upload/intern";
        private readonly string STAFF_FOLDER_SERVICE = "cv_upload/staff";
        private readonly string FAILED_FOLDER = "failed";
        private readonly IAbpSession _session;
        private readonly IFileProvider _fileService;
        private readonly IFilePath _filePath;
        private readonly IConfiguration _configuration;
        private readonly AutobotService _autobotService;
        private readonly ILogger _logger;
        
        public CVAutomationManager(
            IAbpSession session,
            IFileProvider fileService,
            IFilePath filePath,
            IConfiguration configuration,
            AutobotService autobotService)
        {
            _session = session;
            _fileService = fileService;
            _filePath = filePath;
            _configuration = configuration;
            _autobotService = autobotService;
            _logger = IocManager.Instance.Resolve<ILogger>();
        }

        public async Task<AutomationResult> AutoCreateInternCV()
        {
            _logger.Info($"AutoCreateInternCV() start.");
            var result = new AutomationResult();
            var awsFolderMappingSection = _configuration.GetSection("AWSFolderMapping");
            if (!awsFolderMappingSection.Exists())
            {
                return result;
            }

            var dicFolderAndPosition = awsFolderMappingSection.Get<Dictionary<string, string>>();

            foreach (var positionMapping in dicFolderAndPosition)
            {
                var paths = await _filePath.GetPath(INTERN_FOLDER_SERVICE, positionMapping.Key, _session.TenantId);

                List<string> failedPaths = new List<string>();
                failedPaths.AddRange(paths);
                failedPaths.Add(FAILED_FOLDER);
                
                var fileNamesInPath = await _fileService.GetFileNamesAsync(paths);
                result.Total += fileNamesInPath.Count;

                foreach (var fileName in fileNamesInPath)
                {
                    try
                    {
                        CommonUtils.CheckFormatFile(fileName, FileTypes.DOCUMENT);
                        using var responseStream = await _fileService.ReadFileAsync(paths, fileName);
                        var cvExtractionData = await _autobotService.ExtractCVInformationAsync<CVExtractionData>(responseStream, fileName);

                        if (cvExtractionData == null
                            || string.IsNullOrEmpty(cvExtractionData.Email) && string.IsNullOrEmpty(cvExtractionData.PhoneNumber) && string.IsNullOrEmpty(cvExtractionData.Fullname))
                        {
                            await _fileService.MoveFileAsync(paths, failedPaths, fileName, true);
                            continue;
                        }

                        var cvCandidatePaths = await _filePath.GetPath(FILE_CANDIDATE_FOLDER_SERVICE, PathFolder.FOLDER_CV, _session.TenantId);
                        var cvLink = await _fileService.CopyFileAsync(paths, cvCandidatePaths, fileName, hasTimestamp: true);
                        await TransformDataAndCreateCV(cvExtractionData, UserType.Intern, cvLink, positionMapping.Value);
                        await _fileService.ArchiveFileAsync(paths, fileName, hasTimestamp: true);
                        result.Success++;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"AutoCreateInternCV() - {fileName} - exception", ex);
                        continue;
                    }
                }
            }

            return result;
        }

        public async Task<AutomationResult> AutoCreateStaffCV()
        {
            _logger.Info($"AutoCreateStaffCV() start.");
            var result = new AutomationResult();
            var awsFolderMappingSection = _configuration.GetSection("AWSFolderMapping");
            if (!awsFolderMappingSection.Exists())
            {
                return result;
            }

            var dicFolderAndPosition = awsFolderMappingSection.Get<Dictionary<string, string>>();

            foreach (var positionMapping in dicFolderAndPosition)
            {
                var paths = await _filePath.GetPath(STAFF_FOLDER_SERVICE, positionMapping.Key, _session.TenantId);

                List<string> failedPaths = new List<string>();
                failedPaths.AddRange(paths);
                failedPaths.Add(FAILED_FOLDER);

                var fileNamesInPath = await _fileService.GetFileNamesAsync(paths);
                result.Total += fileNamesInPath.Count;

                foreach (var fileName in fileNamesInPath)
                {
                    try
                    {
                        CommonUtils.CheckFormatFile(fileName, FileTypes.DOCUMENT);
                        using var responseStream = await _fileService.ReadFileAsync(paths, fileName);
                        var cvExtractionData = await _autobotService.ExtractCVInformationAsync<CVExtractionData>(responseStream, fileName);

                        if (cvExtractionData == null
                            || string.IsNullOrEmpty(cvExtractionData.Email) && string.IsNullOrEmpty(cvExtractionData.PhoneNumber) && string.IsNullOrEmpty(cvExtractionData.Fullname))
                        {
                            await _fileService.MoveFileAsync(paths, failedPaths, fileName, true);
                            continue;
                        }

                        var cvCandidatePaths = await _filePath.GetPath(FILE_CANDIDATE_FOLDER_SERVICE, PathFolder.FOLDER_CV, _session.TenantId);
                        var cvLink = await _fileService.CopyFileAsync(paths, cvCandidatePaths, fileName, hasTimestamp: true);
                        await TransformDataAndCreateCV(cvExtractionData, UserType.Staff, cvLink, positionMapping.Value);
                        await _fileService.ArchiveFileAsync(paths, fileName, hasTimestamp: true);
                        result.Success++;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"AutoCreateStaffCV() - {fileName} - exception", ex);
                        continue;
                    }
                }
            }

            return result;
        }

        private async Task TransformDataAndCreateCV(CVExtractionData cvExtractionData, UserType userType, string cvLink, string positionName)
        {
            var cv = new CV
            {
                TenantId = _session.TenantId,
                Name = StringExtensions.FormatName(cvExtractionData.Fullname),
                Email = cvExtractionData.Email,
                Phone = StringExtensions.FormatPhoneNumber(cvExtractionData.PhoneNumber),
                Address = cvExtractionData.Address,
                UserType = userType,
                LinkCV = cvLink,
                CVStatus = CVStatus.Draft,
            };

            if (DateTime.TryParse(cvExtractionData.Birthday, out var birthday))
            {
                cv.Birthday = birthday;
            }

            if (string.IsNullOrEmpty(cvExtractionData.Gender))
            {
                cv.IsFemale = false;
            }
            else if (cvExtractionData.Gender.ToLower().Equals("male")
                || cvExtractionData.Gender.ToLower().Equals("nam"))
            {
                cv.IsFemale = false;
            }
            else
            {
                cv.IsFemale = true;
            }

            var user = SettingManager.GetSettingValueForApplication(AppSettingNames.NoticeCVCreatedToHR);
            if (!string.IsNullOrEmpty(user))
            {
                var email = user.Split(',').Select(x => x.Trim()).FirstOrDefault();
                var defaultUserCreation = await WorkScope.GetAll<User>().FirstOrDefaultAsync(x => x.EmailAddress.Equals(email));
                cv.CreatorUserId = defaultUserCreation?.Id;
                cv.LastModifierUserId = defaultUserCreation?.Id;
            }

            var defaultBranch = await WorkScope.GetAll<Branch>().Where(x => x.Name.ToLower().Equals("toàn bộ")).FirstOrDefaultAsync()
                ?? await WorkScope.GetAll<Branch>().Where(x => x.Name.ToLower().Equals("hn1")).FirstOrDefaultAsync()
                ?? await WorkScope.GetAll<Branch>().FirstOrDefaultAsync();
            cv.BranchId = defaultBranch.Id;

            var defaultPosition = await WorkScope.GetAll<SubPosition>()
                .Where(x => x.Name.ToLower().Equals(positionName.ToLower()))
                .FirstOrDefaultAsync()
                ?? await WorkScope.GetAll<SubPosition>().FirstOrDefaultAsync();
            cv.SubPositionId = defaultPosition.Id;

            await WorkScope.InsertAsync(cv);
        }
    }
}
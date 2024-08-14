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
using TalentV2.NccCore;
using Abp.Domain.Uow;
using TalentV2.WebServices.ExternalServices.Firebase;

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
        private readonly FirebaseServices _firebaseService;
        private readonly IWorkScope _workScope;

        public CVAutomationManager(
            FirebaseServices firebaseService,
            IWorkScope workScope,
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
            _firebaseService = firebaseService;
            _workScope = workScope;
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
                            || (string.IsNullOrEmpty(cvExtractionData.Email) && string.IsNullOrEmpty(cvExtractionData.PhoneNumber) && string.IsNullOrEmpty(cvExtractionData.Fullname)))
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
                            || (string.IsNullOrEmpty(cvExtractionData.Email) && string.IsNullOrEmpty(cvExtractionData.PhoneNumber) && string.IsNullOrEmpty(cvExtractionData.Fullname)))
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
                TenantId = _session?.TenantId,
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

            var user = SettingManager.GetSettingValueForApplication(AppSettingNames.CVAutomationNotifyToUser);
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
        [UnitOfWork]
        public async Task<AutomationResult> AutoCreateCVFromFirebase()
        {
            _logger.Info($"AutoCreateCVFromFirebase() start.");

            var result = new AutomationResult();
            var data = await _firebaseService.CrawlData();
            var logList = await WorkScope.GetAll<FirebaseCareerLog>().ToListAsync();
            var logIdSet = new HashSet<string>(logList.Select(l => l.IdFirebase));
            foreach (var item in data)
            {

                if (logIdSet.Contains(item.Key))
                    data.Remove(item.Key);
            }

            if (data != null)
                foreach (var item in data)
                {
                    var CVInfor = await ExtractingCV(item.Value.FileURL, item.Key);
                    if (CVInfor != null)
                    {
                        if (await SaveCV(CVInfor, item))
                            result.Success++;
                    }
                }
            return result;
        }
        [UnitOfWork]
        private async Task<bool> SaveCV(CVScanResultFromFireBase CVInfor, KeyValuePair<string, Applicant> item)
        {
            try
            {
                var newCVToSave = new CV
                {
                    TenantId = _session?.TenantId,
                    Name = item.Value.FullName,
                    Email = item.Value.Email,
                    Phone = item.Value.PhoneNumber,
                    Address = CVInfor.Address,
                    UserType = UserType.Intern,
                    LinkCV = await _autobotService.SaveCVToAWS(item.Value.FileURL),//save CV to AWS and set AWS link to CV.LinkCV
                    CVStatus = CVStatus.Draft,
                };

                if (DateTime.TryParse(CVInfor.Dob, out var birthday))
                    newCVToSave.Birthday = birthday;

                if (string.IsNullOrEmpty(CVInfor.Gender))
                    newCVToSave.IsFemale = false;

                else if (CVInfor.Gender.ToLower().Equals("male")
                    || CVInfor.Gender.ToLower().Equals("nam"))
                    newCVToSave.IsFemale = false;

                else
                    newCVToSave.IsFemale = true;



                var user = await WorkScope.GetAll<User>().FirstOrDefaultAsync();
                newCVToSave.CreatorUserId = user.Id > 0 ? user.Id : 1L;
                newCVToSave.LastModifierUserId = user.Id > 0 ? user.Id : 1L;

                string branchName = ConvertToAliasName(item.Value.Office);
                var defaultBranch = await _workScope.GetAll<Branch>().FirstOrDefaultAsync(b => b.Name.Equals(branchName));
                newCVToSave.BranchId = defaultBranch.Id;

                var defaultSubPosition = await WorkScope.GetAll<SubPosition>().FirstOrDefaultAsync(p => item.Value.JobTitle.Contains(p.Name)) ?? await WorkScope.GetAll<SubPosition>().FirstOrDefaultAsync();
                newCVToSave.SubPositionId = defaultSubPosition.Id;


                var firebaseCareerLog = new FirebaseCareerLog
                {
                    IdFirebase = item.Key,
                    Status = FirebaseLogStatusConstant.ACCEPTED,
                };
                //save cv
                await WorkScope.InsertAsync(newCVToSave);
                //save log
                await WorkScope.InsertAsync(firebaseCareerLog);
            }
            catch (Exception ex)
            {
                _logger.Error($"CheckCVBeforeExtract() - {item.Key} - exception", ex);
                return false;
            };
            return true;
        }
        private async Task<CVScanResultFromFireBase> ExtractingCV(string fileUrl, string firebaseId)
        {

            try
            {
                if (!await _autobotService.IsAcceptedSize(fileUrl))
                {
                    var firebaseCareerLog = new FirebaseCareerLog
                    {
                        IdFirebase = firebaseId,
                        Status = FirebaseLogStatusConstant.CV_SIZE_TOO_BIG
                    };
                    await WorkScope.InsertAsync(firebaseCareerLog);
                }
                else if (!await _autobotService.IsAcceptedType(fileUrl) || !_autobotService.IsAcceptedExtension(fileUrl))
                {
                    var firebaseCareerLog = new FirebaseCareerLog
                    {
                        IdFirebase = firebaseId,
                        Status = FirebaseLogStatusConstant.CV_ERROR_TYPE
                    };
                    await WorkScope.InsertAsync(firebaseCareerLog);
                }

                else
                {
                    return await _autobotService.ExtractCVFromFirebaseAsync(fileUrl);
                }
            }

            
            catch (Exception e)
            {
                _logger.Error($"IsAcceptedFileAsync() - {firebaseId} - exception", e);

            }

            return null;
        }

        private static string ConvertToAliasName(string input)
        {
            var NCCBranchNames = new Dictionary<string, string>
        {
            { "Ho Chi Minh", "HCM" },
            { "Ha Noi 1", "HN1" },
            { "Ha Noi 2", "HN2" },
            { "Ha Noi 3", "HN3" },
            { "Da Nang", "DN" },
            { "Vinh", "Vinh" },
            { "Quy Nhon", "QN" },

        };
            foreach (var item in NCCBranchNames)
            {
                if (item.Key.Equals(input))
                    return item.Value;
            }
            return "HN1";
        }
    }
}
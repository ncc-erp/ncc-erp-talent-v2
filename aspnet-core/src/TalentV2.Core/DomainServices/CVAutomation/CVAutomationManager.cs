﻿using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NccCore.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using TalentV2.Authorization.Users;
using TalentV2.Configuration;
using TalentV2.Constants.Const;
using TalentV2.Constants.Dictionary;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.CVAutomation.Dto;
using TalentV2.Entities;
using TalentV2.FileServices.Paths;
using TalentV2.FileServices.Providers;
using TalentV2.NccCore;
using TalentV2.Utils;
using TalentV2.WebServices.ExternalServices.Autobot;
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
        public CVAutomationManager(
            FirebaseServices firebaseService,
            IWorkScope workScope,
            IAbpSession session,
            IFileProvider fileService,
            IFilePath filePath,
            IConfiguration configuration,
            AutobotService autobotService
            )
        {
            _session = session;
            _fileService = fileService;
            _filePath = filePath;
            _configuration = configuration;
            _autobotService = autobotService;
            _logger = IocManager.Instance.Resolve<ILogger>();
            _firebaseService = firebaseService;


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
                Email = cvExtractionData.Email,
                Address = cvExtractionData.Address,
                UserType = userType,
                LinkCV = cvLink,
                CVStatus = CVStatus.Draft,
            };

            string phoneNumber = StringExtensions.FormatPhoneNumber(cvExtractionData.PhoneNumber);
            cv.Phone = phoneNumber.Length > 12 ? string.Empty : phoneNumber;

            string name = StringExtensions.FormatName(cvExtractionData.Fullname);
            cv.Name = name.Length > 100 ? name.Substring(0, 100) : name;

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
        public async Task<Dictionary<UserType, int>> AutoCreateCVFromFirebase()
        {
            _logger.Info($"AutoCreateCVFromFirebase() start.");
            var resultStaff = new AutomationResult();
            var resultIntern = new AutomationResult();
            var data = await _firebaseService.CrawlData();
            var logList = await WorkScope.GetAll<FirebaseCareerLog>().ToListAsync();
            var logIdSet = new HashSet<string>(logList.Select(l => l.IdFirebase));
            if (data != null)
            {
                foreach (var item in data.Where(d => !logIdSet.Contains(d.Key)))
                {
                    var cvInfor = await ValidatingAndExtractingCV(item.Value.FileURL, item.Key);
                    if (cvInfor != null && await SavingCV(cvInfor, item))
                    {
                        (item.Value.Position.Equals("Staff", StringComparison.OrdinalIgnoreCase) ? resultStaff : resultIntern).Success++;
                    }
                }
            }
            var result = new Dictionary<UserType, int>
                {
                    { UserType.Staff, resultStaff.Success },
                    { UserType.Intern, resultIntern.Success }
                };

            return result;
        }

        [UnitOfWork]
        private async Task<bool> SavingCV(CVScanResultFromFireBase CVInfor, KeyValuePair<string, Applicant> item)
        {
            try
            {
                var newCVToSave = new CV
                {
                    TenantId = _session?.TenantId,
                    Name = CheckingOverFlow("Name",item.Value.FullName),
                    Email = item.Value.Email,
                    Phone = CheckingOverFlow("Phone",item.Value.PhoneNumber),
                    Address = CheckingOverFlow("Address",CVInfor.Address),
                    UserType = item.Value.Position.Equals("Staff", StringComparison.OrdinalIgnoreCase) ? UserType.Staff : UserType.Intern,
                    LinkCV = await SaveCVToAWS(item.Value.FileURL, CVInfor.CVData),//save CV to AWS and set AWS link to CV.LinkCV
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
                var user = SettingManager.GetSettingValueForApplication(AppSettingNames.CVAutomationNotifyToUser);
                if (!string.IsNullOrEmpty(user))
                {
                    var email = user.Split(',').Select(x => x.Trim()).FirstOrDefault();
                    var defaultUserCreation = await WorkScope.GetAll<User>().FirstOrDefaultAsync(x => x.EmailAddress.Equals(email));
                    newCVToSave.CreatorUserId = defaultUserCreation?.Id;
                    newCVToSave.LastModifierUserId = defaultUserCreation?.Id;
                }
                string branchName = ConvertToAliasName(item.Value.Office);
                var defaultBranch = await WorkScope.GetAll<Branch>().FirstOrDefaultAsync(b => b.Name.Equals(branchName)) ?? await WorkScope.GetAll<Branch>().FirstOrDefaultAsync();
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

        private async Task<CVScanResultFromFireBase> ValidatingAndExtractingCV(string fileUrl, string firebaseId)
        {
            try
            {
                var result = await _autobotService.ValidateAndExtractCVFromFirebaseAsync(fileUrl);

                if (result != null)
                {
                    if (result.Values.FirstOrDefault() == null)
                    {
                        var firebaseCareerLog = new FirebaseCareerLog
                        {
                            IdFirebase = firebaseId,
                            Status = result.Keys.FirstOrDefault()
                        };
                        await WorkScope.InsertAsync(firebaseCareerLog);
                    }
                    else
                        return result.Values.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                _logger.Error($"IsAcceptedFileAsync() - {firebaseId} - exception", e);
            }

            return null;
        }

        private async Task<string> SaveCVToAWS(string fileURL, byte[] fileBytes)
        {
            var fileName = Path.GetFileName(new Uri(fileURL).AbsolutePath);
            var fileNameDecoded = WebUtility.UrlDecode(fileName);
            var streamConverted = new MemoryStream(fileBytes);
            var formFile = CreateFormFile(fileBytes, fileNameDecoded, streamConverted);
            var paths = await _filePath.GetPath(FILE_CANDIDATE_FOLDER_SERVICE, PathFolder.FOLDER_CV, _session.TenantId);
            var cvAwslink = await _fileService.UploadFileAsync(paths, formFile);
            return cvAwslink;
        }

        private static IFormFile CreateFormFile(byte[] fileBytes, string fileName, MemoryStream stream)
        {
            return new FormFile(stream, 0, fileBytes.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream" // 
            };
        }

        private static string CheckingOverFlow(string fieldName,string data)
        {       
            var valueAnnotatoin = typeof(CV).GetProperty(fieldName).GetCustomAttribute<MaxLengthAttribute>();
            var result = data.Length > valueAnnotatoin.Length ? data[..valueAnnotatoin.Length] : data;
            return result;
        }

        private static string ConvertToAliasName(string input)
        {
            foreach (var item in DictionaryHelper.NCCBranchNames)
            {
                if (item.Key.Equals(input))
                    return item.Value;
            }
            return "HN1";
        }
    }
}
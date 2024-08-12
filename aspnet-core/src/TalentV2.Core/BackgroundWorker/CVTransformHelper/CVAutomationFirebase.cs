using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TalentV2.BackgroundWorker.ConstantHelper;
using TalentV2.Constants.Const;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.CVAutomation.Dto;
using TalentV2.Entities;
using TalentV2.FileServices.Paths;
using TalentV2.Firebase;
using TalentV2.WebServices.ExternalServices.Autobot;
using TalentV2.Authorization.Users;
using TalentV2.FileServices.Providers;
using Microsoft.EntityFrameworkCore;

namespace TalentV2.BackgroundWorker.CVTransformHelper
{
    public class CVAutomationFirebase : ITransientDependency
    {

        private readonly IAbpSession _session;
        private readonly ILogger<CVAutomationFirebase> _logger;
        private readonly FirebaseService _firebaseService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly UserManager _userManager;
        private readonly AutobotService _autobotService;
        private readonly IRepository<FirebaseCareerLog> _repositoryCareerLog;
        private readonly IRepository<Branch, long> _repositoryBranch;
        private readonly IRepository<SubPosition, long> _repositorySubPosition;
        private readonly IRepository<CV, long> _repositoryCV;
        private readonly IRepository<Position, long> _repositoryPosition;
        private readonly IFileProvider _fileServiceAWS;
        private readonly IFilePath _filePath;
        private readonly IAbpSession _abpSession;
        private readonly string FOLDER_SERVICE = "candidates";
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MBs


        public CVAutomationFirebase(
            IAbpSession session,
            ILogger<CVAutomationFirebase> logger,
            FirebaseService firebaseService,
            IHttpClientFactory httpClientFactory,
            IUnitOfWorkManager unitOfWorkManager,
            AutobotService autobotService,
            UserManager userManager,
            IRepository<FirebaseCareerLog> repositoryCareerLog,
            IRepository<Branch, long> repositoryBranch,
            IRepository<SubPosition, long> repositorySubPosition,
            IRepository<CV, long> repositoryCV,
            IRepository<Position, long> repositoryPosition,
            IFileProvider fileServiceAWS,
            IFilePath filePath,
            IAbpSession abpSession
            )

        {
            _session = session;
            _firebaseService = firebaseService;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _repositoryCareerLog = repositoryCareerLog;
            _unitOfWorkManager = unitOfWorkManager;
            _repositoryBranch = repositoryBranch;
            _repositorySubPosition = repositorySubPosition;
            _repositoryCV = repositoryCV;
            _userManager = userManager;
            _autobotService = autobotService;
            _repositoryPosition = repositoryPosition;
            _fileServiceAWS = fileServiceAWS;
            _filePath = filePath;
            _abpSession = abpSession;
        }



        public async Task<AutomationResult> AutoCreateCVFromFirebase()
        {
            _logger.LogInformation($"AutoCreateCVFromFirebase() start.");

            var data = await _firebaseService.GetCrawl();
            var client = _httpClientFactory.CreateClient();
            var result = new AutomationResult();

            foreach (var item in data)
            {
                var CVInfor = await IsAcceptedFileAsync(client, item.Value.FileURL, item.Key);
                if (await IsSavedCV(CVInfor, item))
                {
                    result.Success++;
                };
            }
            return result;
        }

        private async Task<bool> IsSavedCV(CVScanResulFromFireBase CVInfor, KeyValuePair<string, Applicant> item)
        {
            var unitOfWork = _unitOfWorkManager.Begin();
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
                    LinkCV = item.Value.FileURL,
                    CVStatus = CVStatus.Draft,
                };

                if (DateTime.TryParse(CVInfor.Dob, out var birthday))
                {
                    newCVToSave.Birthday = birthday;
                }

                if (string.IsNullOrEmpty(CVInfor.Gender))
                {
                    newCVToSave.IsFemale = false;
                }
                else if (CVInfor.Gender.ToLower().Equals("male")
                    || CVInfor.Gender.ToLower().Equals("nam"))
                {
                    newCVToSave.IsFemale = false;
                }
                else
                {
                    newCVToSave.IsFemale = true;
                }
                var user = await _userManager.Users.FirstOrDefaultAsync();
                newCVToSave.CreatorUserId = user.Id > 0 ? user.Id : 1L;
                newCVToSave.LastModifierUserId = user.Id > 0 ? user.Id : 1L;

                string branchName = ConvertToAliasName(item.Value.Office);
                var defaultBranch = await _repositoryBranch.FirstOrDefaultAsync(b => b.Name.Equals(branchName));
                //if (defaultBranch == null)
                //{
                //    var newBranch = new Branch { Name = ConvertBranch(item.Value.Office) };
                //    await _repositoryBranch.InsertAsync(newBranch);
                //    defaultBranch = newBranch;
                //}
                newCVToSave.BranchId = defaultBranch.Id;

                var defaultSubPosition = await _repositorySubPosition.FirstOrDefaultAsync(p => item.Value.JobTitle.Contains(p.Name)) ?? await GetOrCreateSubPositionAsync("NeedToUpdate");
                newCVToSave.SubPositionId = defaultSubPosition.Id;


                var firebaseCareerLog = new FirebaseCareerLog
                {
                    IdFirebase = item.Key,
                    Status = FirebaseSyncConstant.ACCEPTED,
                };
                //save cv
                await _repositoryCV.InsertAsync(newCVToSave);
                //save log
                await _repositoryCareerLog.InsertAsync(firebaseCareerLog);
                //end local transaction
                await unitOfWork.CompleteAsync();
                //save CV to AWS
                await SaveCVToAWS(item.Value.FileURL);
            }
            catch (Exception ex)
            {
                _logger.LogError($"CheckCVBeforeExtract() - {item.Key} - exception", ex);
                return false;
            };
            return true;
        }



        private async Task SaveCVToAWS(string fileURL)
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                var fileName = Path.GetFileName(new Uri(fileURL).AbsolutePath);
                var fileNameDecoded = WebUtility.UrlDecode(fileName);
                var fileBytes = await client.GetByteArrayAsync(fileURL);
                var streamConverted = new MemoryStream(fileBytes);
                var formFile = CreateFormFile(fileBytes, fileNameDecoded, streamConverted);
                var paths = await _filePath.GetPath(FOLDER_SERVICE, PathFolder.FOLDER_CV, _abpSession.TenantId);
                await _fileServiceAWS.UploadFileAsync(paths, formFile);
            };

        }

        private static IFormFile CreateFormFile(byte[] fileBytes, string fileName, MemoryStream stream)
        {
            return new FormFile(stream, 0, fileBytes.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream" // 
            };




        }
        private async Task<Position> GetOrCreatePositionAsync(string positionName)
        {
            var position = await _repositoryPosition.FirstOrDefaultAsync(p =>
                p.Name.Equals(positionName));
            if (position == null)
            {

                using (var unitOfWorkPosition = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
                {
                    position = new Position { Name = positionName };
                    await _repositoryPosition.InsertAsync(position);
                    await unitOfWorkPosition.CompleteAsync();
                };
            }
            return position;
        }
        private async Task<SubPosition> GetOrCreateSubPositionAsync(string subPositionName)
        {
            var defaultSubPosition = await _repositorySubPosition.FirstOrDefaultAsync(p => p.Name.Equals(subPositionName));
            if (defaultSubPosition == null)
            {
                var position = await GetOrCreatePositionAsync("NotCreatedPosition");

                using (var unitOfWorkSubPosition = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
                {
                    defaultSubPosition = new SubPosition
                    {
                        Name = "NeedToUpdate",
                        PositionId = position.Id,

                    };
                    await _repositorySubPosition.InsertAsync(defaultSubPosition);
                    await unitOfWorkSubPosition.CompleteAsync();
                };
            }

            return defaultSubPosition;
        }
        private async Task<CVScanResulFromFireBase> IsAcceptedFileAsync(HttpClient client, string fileUrl, string firebaseId)
        {

            try
            {
                var response = await client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);
                CheckFileSize(response.Content.Headers.ContentLength, firebaseId);


                var contentType = response.Content.Headers.ContentType.MediaType;
                CheckFileType(contentType, firebaseId);

                var fileName = Path.GetFileName(new Uri(fileUrl).AbsolutePath);
                var fileNameDecoded = WebUtility.UrlDecode(fileName);
                CheckFileExtension(fileNameDecoded, firebaseId);


                var fileBytes = await client.GetByteArrayAsync(fileUrl);
                return await _autobotService.ExtractCVFromFirebaseAsync(fileBytes, fileNameDecoded);
            }
            catch (HttpRequestException e)
            {
                throw new ApplicationException($"Error at CV 's Url: {e.Message}");
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception: {e.Message}");
            }
        }
        private void CheckFileSize(long? fileSize, string firebaseId)
        {
            if (fileSize > MaxFileSize)
            {
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    var firebaseCareerLog = new FirebaseCareerLog
                    {
                        IdFirebase = firebaseId,
                        Status = FirebaseSyncConstant.CV_SIZE_TOO_BIG
                    };
                    _repositoryCareerLog.InsertAsync(firebaseCareerLog).Wait();
                }
                throw new ApplicationException($"CV 's size is over the limit {MaxFileSize / (1024 * 1024)}MB");
            }
        }
        private void CheckFileType(string contentType, string firebaseId)
        {
            var validContentTypes = new HashSet<string>
                {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                 };

            if (!validContentTypes.Contains(contentType))
            {
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    var firebaseCareerLog = new FirebaseCareerLog
                    {
                        IdFirebase = firebaseId,
                        Status = FirebaseSyncConstant.CV_ERROR_TYPE
                    };
                    _repositoryCareerLog.InsertAsync(firebaseCareerLog).Wait();
                }
                throw new ApplicationException("Only file type PDF, DOC, DOCX is accepted.");
            }
        }
        private void CheckFileExtension(string fileName, string firebaseId)
        {
            var validExtensions = new HashSet<string> { ".pdf", ".doc", ".docx" };
            var fileExtension = Path.GetExtension(fileName).ToLower();

            if (!validExtensions.Contains(fileExtension))
            {
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    var firebaseCareerLog = new FirebaseCareerLog
                    {
                        IdFirebase = firebaseId,
                        Status = FirebaseSyncConstant.CV_ERROR_TYPE
                    };
                    _repositoryCareerLog.InsertAsync(firebaseCareerLog).Wait();
                }
                throw new ApplicationException("Only file type PDF, DOC, DOCX is accepted.");
            }
        }
        private static string ConvertToAliasName(string input)
        {
            var NCCBranchNames = new Dictionary<string, string>
        {
           { "Ha Noi 1", "HN1" },
            { "Vinh", "Vinh" },
            { "Ho Chi Minh", "HCM" },
            { "Ha Noi 2", "HN2" },
            { "Ha Noi 3", "HN3" },
            { "Da Nang", "DN" },
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

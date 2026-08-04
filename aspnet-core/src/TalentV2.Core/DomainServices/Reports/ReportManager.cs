using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TalentV2.Constants.Dictionary;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.DomainServices.Reports.Dtos;
using TalentV2.Entities;
using TalentV2.Utils;

namespace TalentV2.DomainServices.Reports
{
    public class ReportManager : BaseManager, IReportManager
    {
        public async Task<OverviewHiringDto> GetOverviewHiring(DateTime fd, DateTime td, UserType? userType, long? branchId, long? userId)
        {
            var fromDate = fd.Date;
            var toDate = td.Date;
            var qsubPositionStatistic = from sp in WorkScope.GetAll<SubPosition>()
                                        join r in WorkScope.GetAll<Request>() on sp.Id equals r.SubPositionId into rr
                                        from gr in rr.DefaultIfEmpty()
                                        group gr by new { sp.Id, sp.Name } into r
                                        select new
                                        {
                                            SubPositionId = r.Key.Id,
                                            SubPositionName = r.Key.Name,
                                            QuantityHiring = r.Where(q => q.CreationTime.Date >= fd.Date && q.CreationTime.Date <= td.Date)
                                                              .Where(q => branchId.HasValue ? q.BranchId == branchId.Value : true)
                                                              .Where(q => userType.HasValue ? q.UserType == userType : true)
                                                              .Sum(s => s.Quantity)
                                        };
            var subPositionStatistics = await qsubPositionStatistic
                .ToDictionaryAsync
                (
                    x => x.SubPositionId,
                    x => new SubPositionStatistic
                    {
                        SubPositionId = x.SubPositionId,
                        SubPositionName = x.SubPositionName,
                        QuantityHiring = x.QuantityHiring
                    }
                );

            var cvStatistics = await GetDicSubPositionIdToStatisticQuantityCV(fd, td, userType, branchId, userId);

            var cvsources = await WorkScope.GetAll<CVSource>().OrderBy(s => s.Name).ToListAsync();
            var qstatisticIQCVSources = IQStatisticCVSources(fd, td, userType, branchId, cvsources, userId);
            var qstatisticBySubPostion = IQStatisticRequestCVHistory(fd, td, userType, branchId, userId);

            var statisticBySubPosition = qstatisticBySubPostion.ToDictionary
                (
                    x => x.SubPositionId,
                    x => new { x.StatusStatistics }
                );

            var statisticCVSources = qstatisticIQCVSources.ToDictionary
                (
                    x => x.SubPositionId,
                    x => new { x.CVSourceStatistics }
                );

            var listKeyHaveZeroValue = new List<long>();

            foreach (var subPositionId in subPositionStatistics.Keys)
            {
                var positionStatistic = subPositionStatistics[subPositionId];
                positionStatistic.QuantityApply = cvStatistics[subPositionId].QuantityApply;

                positionStatistic.StatusStatistics = statisticBySubPosition.ContainsKey(subPositionId) ?
                                                        statisticBySubPosition[subPositionId].StatusStatistics :
                                                            GetStatusZero();

                var countCVOnboarded = positionStatistic.StatusStatistics.Where(s => s.Id == RequestCVStatus.Onboarded.GetHashCode()).Select(s => s.TotalCV).FirstOrDefault();
                if (countCVOnboarded == 0 && positionStatistic.QuantityApply == 0 && positionStatistic.QuantityHiring == 0)
                {
                    listKeyHaveZeroValue.Add(subPositionId);
                    continue;
                }

                positionStatistic.CVSourceStatistics = statisticCVSources.ContainsKey(subPositionId) ?
                                                        statisticCVSources[subPositionId].CVSourceStatistics :
                                                          GetCVSourcesZero(cvsources);

                positionStatistic.QuantityContacting = cvStatistics[subPositionId].QuantityContacting;
                positionStatistic.QuantityFailed = cvStatistics[subPositionId].QuantityFailed;
                positionStatistic.QuantityPassed = cvStatistics[subPositionId].QuantityPassed;
                positionStatistic.QuantityNewCV = cvStatistics[subPositionId].QuantityNewCV;
            }

            foreach (var key in listKeyHaveZeroValue)
            {
                subPositionStatistics.Remove(key);
            }

            var finalPositionStatistics = subPositionStatistics.Values.ToList();
            var overviewHiring = await GetResultOverviewHiring(branchId, finalPositionStatistics);
            overviewHiring.TotalOverviewHiring = GetTotalOverviewHiring(finalPositionStatistics, cvsources.Count);

            return overviewHiring;
        }

        public async Task<List<RecruitmentOverviewResponseDto>> GetRecruitmentOverview(RecruitmentOverviewRequestDto payload)
        {
            var fromDate = payload.FromDate.ToLocalTime().Date;
            var toDate = payload.ToDate.ToLocalTime().Date;
            var hasBranches = payload.BranchIds != null && payload.BranchIds.Any();
            DateTime currentDateTime = DateTime.Now;

            var CVs = await WorkScope.GetAll<CV>()
                .Where(CV => CV.LastModificationTime >= fromDate && CV.LastModificationTime < toDate.AddDays(1) && !CV.IsDeleted)
                        .WhereIf(payload.UserType.HasValue, CV => CV.UserType.Equals(payload.UserType.Value))
                        .WhereIf(payload.UserId.HasValue, CV => CV.CreatorUserId.Equals(payload.UserId.Value))
                        .Select(CV => new CVStatistic
                        {
                            BranchId = CV.BranchId,
                            SubPositionId = CV.SubPositionId,
                            SubPositionName = CV.SubPosition.Name,
                            CVStatusId = CV.CVStatus,
                            CVSourceId = CV.CVSourceId,
                            CVSourceName = CV.CVSource.Name,
                            CandidateStatusId = CV.RequestCVs
                            .Where(requestCV => !requestCV.IsDeleted)
                            .OrderByDescending(requestCV => requestCV.LastModificationTime)
                            .FirstOrDefault().Status,
                            LastModificationTime = CV.LastModificationTime
                        })
                        .ToListAsync();

            var requests = await WorkScope.GetAll<Request>()
                .Where(request => request.CreationTime >= fromDate && request.CreationTime < toDate.AddDays(1) && !request.IsDeleted)
                .WhereIf(payload.UserType.HasValue, request => request.UserType.Equals(payload.UserType.Value))
                .WhereIf(payload.UserId.HasValue, request => request.CreatorUserId.Equals(payload.UserId.Value))
                .Select(request => new RequestStatistic
                {
                    BranchId = request.BranchId,
                    SubPositionId = request.SubPositionId,
                    SubPositionName = request.SubPosition.Name,
                    Quantity = request.Quantity
                })
                .ToListAsync();

            var recruitmentOverviewResult = new List<RecruitmentOverviewResponseDto>();

            if (payload.IsGetAllBranch)
            {
                recruitmentOverviewResult.Add(StatisticRecruitmentOverviewByBranch(fromDate, toDate, CVs, requests, currentDateTime, null));
            }
            if (hasBranches)
            {
                foreach (var branchId in payload.BranchIds)
                {
                    recruitmentOverviewResult.Add(StatisticRecruitmentOverviewByBranch(fromDate, toDate, CVs, requests, currentDateTime, branchId));
                }
            }

            return recruitmentOverviewResult;
        }

        private RecruitmentOverviewResponseDto StatisticRecruitmentOverviewByBranch(DateTime fromDate, DateTime toDate, List<CVStatistic> CVs, List<RequestStatistic> requests, DateTime currentDateTime, long? branchId)
        {
            string branchName = "All company";
            if (branchId.HasValue)
            {
                branchName = WorkScope.GetAll<Branch>()
                    .Where(branch => branch.Id == branchId && !branch.IsDeleted)
                    .Select(branch => branch.Name).FirstOrDefault();
            }

            List<CVStatistic> filterCVs = branchId.HasValue ? CVs.FindAll(CV => CV.BranchId == branchId) : CVs;
            List<RequestStatistic> filterRequests = branchId.HasValue ? requests.FindAll(request => request.BranchId == branchId) : requests;

            List<SubPositionDto> subPositions = new List<SubPositionDto>();
            foreach (var CV in filterCVs)
            {
                if (!subPositions.Any(subPosition => subPosition.Id == CV.SubPositionId))
                {
                    subPositions.Add(new SubPositionDto
                    {
                        Id = CV.SubPositionId,
                        Name = CV.SubPositionName,
                    });
                }
            }
            foreach (var request in filterRequests)
            {
                if (!subPositions.Any(subPosition => subPosition.Id == request.SubPositionId))
                {
                    subPositions.Add(new SubPositionDto
                    {
                        Id = request.SubPositionId,
                        Name = request.SubPositionName,
                    });
                }
            }
            subPositions = subPositions.OrderBy(subPosition => subPosition.Name).ToList();

            return new RecruitmentOverviewResponseDto
            {
                BranchId = branchId,
                BranchName = branchName,
                SubPositionStatistics = subPositions.Select(subPosition => new SubPositionStatisticV2
                {
                    SubPositionId = subPosition.Id,
                    SubPositionName = subPosition.Name,
                    RequestQuantity = filterRequests
                    .Where(request => request.SubPositionId == subPosition.Id)
                    .Select(request => request.Quantity)
                    .Sum(),
                    ApplyQuantity = filterCVs
                    .Where(CV => CV.SubPositionId == subPosition.Id)
                    .Count(),
                    CVStatusStatistics = filterCVs
                    .Where(CV => CV.SubPositionId == subPosition.Id)
                    .GroupBy(CV => CV.CVStatusId)
                    .Select(CVStatusGroup => new CVStatusStatistic
                    {
                        Id = CVStatusGroup.Key,
                        Name = CVStatusGroup.Key.ToString(),
                        Quantity = CVStatusGroup.Count(),
                        UnprocessedQuantity = CVStatusGroup
                        .Where(item => item.LastModificationTime?.AddDays(3) < currentDateTime)
                        .Count(),
                        NormalQuantity = CVStatusGroup
                        .Where(item => item.LastModificationTime?.AddDays(3) >= currentDateTime)
                        .Count(),
                    }).ToList(),
                    CVSourceStatistics = filterCVs
                    .Where(CV => CV.SubPositionId == subPosition.Id)
                    .GroupBy(CV => new { CV.CVSourceId, CV.CVSourceName })
                    .Select(CVSourceGroup => new CVSourceStatisticV2
                    {
                        Id = CVSourceGroup.Key.CVSourceId,
                        Name = CVSourceGroup.Key.CVSourceName,
                        Quantity = CVSourceGroup.Count()
                    }).ToList(),
                    CandidateStatusStatistics = filterCVs
                    .Where(CV => CV.SubPositionId == subPosition.Id)
                    .GroupBy(CV => CV.CandidateStatusId)
                    .Select(CVSourceGroup => new CandidateStatusStatistic
                    {
                        Id = CVSourceGroup.Key,
                        Name = CVSourceGroup.Key.HasValue ? DictionaryHelper.RequestCVStatusDict[CVSourceGroup.Key.Value] : null,
                        Quantity = CVSourceGroup.Count()
                    }).ToList(),
                }).ToList(),
                Total = new TotalStatistics
                {
                    RequestQuantity = filterRequests.Select(request => request.Quantity).Sum(),
                    ApplyQuantity = filterCVs.Count(),
                    CVStatusStatistics = filterCVs
                        .GroupBy(CV => CV.CVStatusId)
                        .Select(CVStatusGroup => new CVStatusStatistic
                        {
                            Id = CVStatusGroup.Key,
                            Name = CVStatusGroup.Key.ToString(),
                            Quantity = CVStatusGroup.Count(),
                            UnprocessedQuantity = CVStatusGroup
                            .Where(item => item.LastModificationTime?.AddDays(3) < currentDateTime)
                            .Count(),
                            NormalQuantity = CVStatusGroup
                            .Where(item => item.LastModificationTime?.AddDays(3) >= currentDateTime)
                            .Count(),
                        }).ToList(),
                    CVSourceStatistics = filterCVs
                        .GroupBy(CV => new { CV.CVSourceId, CV.CVSourceName })
                        .Select(CVSourceGroup => new CVSourceStatisticV2
                        {
                            Id = CVSourceGroup.Key.CVSourceId,
                            Name = CVSourceGroup.Key.CVSourceName,
                            Quantity = CVSourceGroup.Count()
                        }).ToList(),
                    CandidateStatusStatistics = filterCVs
                        .GroupBy(CV => CV.CandidateStatusId)
                        .Select(CVSourceGroup => new CandidateStatusStatistic
                        {
                            Id = CVSourceGroup.Key,
                            Name = CVSourceGroup.Key.HasValue ? DictionaryHelper.RequestCVStatusDict[CVSourceGroup.Key.Value] : null,
                            Quantity = CVSourceGroup.Count()
                        }).ToList(),
                }
            };
        }

        private IQueryable<RecruitmentOverviewResponseDto> IQGetRecruitmentOverviewByRequests(DateTime fromDate, DateTime toDate, UserType? userType, List<long> branchIds, long? userId)
        {
            bool isGetDetailBranch = branchIds != null && branchIds.Any();
            return WorkScope.GetAll<Request>()
                .Where(request => request.CreationTime >= fromDate && request.CreationTime < toDate.AddDays(1) && !request.IsDeleted)
                .WhereIf(userType.HasValue, request => request.UserType.Equals(userType))
                .WhereIf(isGetDetailBranch, request => branchIds.Contains(request.BranchId))
                .WhereIf(userId.HasValue, request => request.CreatorUserId.Equals(userId))
                .GroupBy(request => isGetDetailBranch ? request.BranchId : 0)
                .Select(branchGroup => new RecruitmentOverviewResponseDto
                {
                    BranchId = isGetDetailBranch ? branchGroup.Key : null,
                    BranchName = isGetDetailBranch ? branchGroup.Select(request => request.Branch.Name).FirstOrDefault() : "All",
                    SubPositionStatistics = branchGroup
                    .GroupBy(request => request.SubPositionId)
                    .Select(subPositionGroup => new SubPositionStatisticV2
                    {
                        SubPositionId = subPositionGroup.Key,
                        SubPositionName = subPositionGroup.Select(request => request.SubPosition.Name).FirstOrDefault(),
                        RequestQuantity = subPositionGroup.Select(request => request.Quantity).Sum(),
                    }).ToList()
                });
        }

        private async Task<OverviewHiringDto> GetResultOverviewHiring(long? branchId, List<SubPositionStatistic> subPositionStatistics)
        {
            var overviewHiring = new OverviewHiringDto();
            overviewHiring.SubPositionStatistics = subPositionStatistics;

            await GetBranchInfo(overviewHiring, branchId);

            return overviewHiring;
        }
        private TotalOverviewHiring GetTotalOverviewHiring(List<SubPositionStatistic> subPositionStatistics, long sizeCVSource)
        {
            var sizeStatus = CommonUtils.ListRequestCVStatus.Count;
            var total = new TotalOverviewHiring();
            total.TotalCVSources = new long[sizeCVSource];
            total.TotalStatuses = ContructorListSize(sizeStatus);

            foreach (var item in subPositionStatistics)
            {
                total.TotalHiring += item.QuantityHiring;
                total.TotalApply += item.QuantityApply;
                total.TotalNewCV += item.QuantityNewCV;
                total.TotalContacting += item.QuantityContacting;
                total.TotalFailed += item.QuantityFailed;
                total.TotalPassed += item.QuantityPassed;
                for (int i = 0; i < sizeCVSource; i++)
                {
                    total.TotalCVSources[i] += item.CVSourceStatistics[i].TotalCV;
                }
                for (int i = 0; i < sizeStatus; i++)
                {
                    total.TotalStatuses[i].TotalCV += item.StatusStatistics[i].TotalCV;
                    total.TotalStatuses[i].Id = item.StatusStatistics[i].Id;
                }
            }
            return total;
        }
        private List<TotalStatus> ContructorListSize(int capacity)
        {
            var list = new List<TotalStatus>();
            for (int i = 0; i < capacity; i++)
            {
                list.Add(new TotalStatus());
            }
            return list;
        }

        private IEnumerable<StaticCVSourcesDto> IQStatisticCVSources(
          DateTime fd,
          DateTime td,
          UserType? userType,
          long? branchId,
          List<CVSource> cvsources,
          long? userId
          )
        {
            Expression<Func<CV, bool>> predic = q => (q.CreationTime.Date >= fd.Date && q.CreationTime.Date <= td.Date) &&
                                                                         (userType.HasValue ? q.UserType == userType : true) &&
                                                                         (!q.IsDeleted && !q.IsDeleted && !q.IsDeleted) &&
                                                                         (branchId.HasValue ? q.BranchId == branchId.Value : true) &&
                                                                         (userId.HasValue ? q.CreatorUserId == userId.Value : true);

            var requestCVSources = WorkScope.GetAll<CV>()
            .Where(predic)
            .Select(s => new
            {
                s.Id,
                s.BranchId,
                s.SubPositionId,
                s.CVSourceId
            })
            .ToList();
            var qstatisticByPostion = from r in requestCVSources
                                      group r by r.SubPositionId into gr
                                      select new StaticCVSourcesDto
                                      {
                                          SubPositionId = gr.Key,
                                          CVSourceStatistics = (from cs in cvsources
                                                                join rqc in gr on cs.Id equals rqc.CVSourceId into rc
                                                                from rsk in rc.DefaultIfEmpty()
                                                                group rsk by new { cs.Id, cs.Name } into lk
                                                                select new CVSourceStatistic
                                                                {
                                                                    Id = lk.Key.Id,
                                                                    Name = lk.Key.Name,
                                                                    TotalCV = lk.Where(s => s != null).Select(s => s.Id).Distinct().Count()
                                                                }).ToList(),
                                      };
            return qstatisticByPostion;
        }

        private IEnumerable<StatisticRequestCVHistory> IQStatisticRequestCVHistory(
            DateTime fd,
            DateTime td,
            UserType? userType,
            long? branchId,
            long? userId
            )
        {
            Expression<Func<RequestCVStatusHistory, bool>> predic = q => (q.TimeAt.Date >= fd.Date && q.TimeAt.Date <= td.Date) &&
                                                                         (userType.HasValue ? q.RequestCV.CV.UserType == userType : true) &&
                                                                         (!q.RequestCV.IsDeleted && !q.RequestCV.Request.IsDeleted && !q.RequestCV.CV.IsDeleted) &&
                                                                         (branchId.HasValue ? q.RequestCV.CV.BranchId == branchId.Value : true) &&
                                                                         (userId.HasValue ? q.RequestCV.CV.CreatorUserId == userId.Value : true);

            var requestCVStatusHistory = WorkScope.GetAll<RequestCVStatusHistory>()
            .Where(predic)
            .Select(s => new
            {
                s.Status,
                s.RequestCV.RequestId,
                s.RequestCV.CV.BranchId,
                s.RequestCV.CV.SubPositionId,
                s.RequestCV.CV.SubPosition.Name,
                s.RequestCV.CVId,
            })
            .ToList();
            var qstatisticByPostion = from r in requestCVStatusHistory
                                      group r by r.SubPositionId into gr
                                      select new StatisticRequestCVHistory
                                      {
                                          SubPositionId = gr.Key,
                                          StatusStatistics = (from s in CommonUtils.ListRequestCVStatus
                                                              join rqc in gr on s.Id equals rqc.Status.GetHashCode() into rs
                                                              from rsk in rs.DefaultIfEmpty()
                                                              group rsk by new { s.Id, s.Name } into lk
                                                              select new StatusStatistic
                                                              {
                                                                  Id = lk.Key.Id,
                                                                  Name = lk.Key.Name,
                                                                  TotalCV = lk.Where(s => s != null).Select(s => s.CVId).Distinct().Count()
                                                              }).ToList()
                                      };
            return qstatisticByPostion;
        }
        private async Task<Dictionary<long, StatisticQuantityCV>> GetDicSubPositionIdToStatisticQuantityCV(
            DateTime fd,
            DateTime td,
            UserType? userType,
            long? branchId,
            long? userId)
        {
            var qCVStatistics = from sp in WorkScope.GetAll<SubPosition>()
                                join cv in WorkScope.GetAll<CV>() on sp.Id equals cv.SubPositionId into cvp
                                from gr in cvp.DefaultIfEmpty()
                                group gr by sp.Id into r
                                select new
                                {
                                    SubPositionId = r.Key,
                                    qAllCVApply = r.Where(q => q.CreationTime.Date >= fd.Date && q.CreationTime.Date <= td.Date)
                                                   .Where(q => branchId.HasValue ? q.BranchId == branchId.Value : true)
                                                   .Where(q => userType.HasValue ? q.UserType == userType : true)
                                                   .Where(q => userId.HasValue ? q.CreatorUserId == userId.Value : true)
                                };
            var cvStatistics = await qCVStatistics
                .ToDictionaryAsync
                (
                    x => x.SubPositionId,
                    x => new StatisticQuantityCV
                    {
                        SubPositionId = x.SubPositionId,
                        QuantityApply = x.qAllCVApply.Count(),
                        QuantityNewCV = x.qAllCVApply
                                              .Where(q => q.CVStatus == CVStatus.New)
                                              .Count(),
                        QuantityContacting = x.qAllCVApply
                                              .Where(q => q.CVStatus == CVStatus.Contacting)
                                              .Count(),
                        QuantityPassed = x.qAllCVApply
                                          .Where(q => q.CVStatus == CVStatus.Passed)
                                          .Count(),
                        QuantityFailed = x.qAllCVApply
                                          .Where(q => q.CVStatus == CVStatus.Failed)
                                          .Count(),
                    }
                );
            return cvStatistics;
        }
        private List<CVSourceStatistic> GetCVSourcesZero(List<CVSource> cvsources)
        {
            return cvsources.Select(x => new CVSourceStatistic
            {
                Id = x.Id,
                Name = x.Name,
                TotalCV = 0
            }).ToList();
        }
        private List<StatusStatistic> GetStatusZero()
        {
            return Enum.GetValues(typeof(RequestCVStatus))
                .Cast<RequestCVStatus>()
                .Select(x => new StatusStatistic
                {
                    Id = x.GetHashCode(),
                    Name = x.ToString(),
                    TotalCV = 0
                }).ToList();
        }
        public async Task<CVSourceStatisticDto> GetPerformanceCVSource(DateTime fd, DateTime td, UserType userType, long? branchId)
        {
            var test = fd.Date;
            var qstatisticSVSource = from cvs in WorkScope.GetAll<CVSource>()
                                     join cv in WorkScope.GetAll<CV>() on cvs.Id equals cv.CVSourceId into cs
                                     from csgr in cs.DefaultIfEmpty()
                                     group csgr by cvs.Name into gr
                                     select new SourcePerformance
                                     {
                                         SourceName = gr.Key,
                                         TotalCV = gr.Where(q => q.CreationTime.Date >= fd.Date && q.CreationTime.Date <= td.Date)
                                                     .Where(q => branchId.HasValue ? q.BranchId == branchId.Value : true)
                                                     .Where(q => q.UserType == userType)
                                                     .Count()
                                     };
            var result = new CVSourceStatisticDto();
            result.SourcePerformances = await qstatisticSVSource.ToListAsync();
            await GetBranchInfo(result, branchId);
            return result;
        }
        public async Task<ReportEducationByBranchDto<ReportEducationHaveCVPassTestDto>> GetEducationPassTest(DateTime fd, DateTime td, long? branchId)
        {
            Expression<Func<RequestCVStatusHistory, bool>> expression = q => (q.TimeAt >= fd.Date && q.TimeAt.Date <= td.Date) &&
                                                                             (!q.RequestCV.IsDeleted && !q.RequestCV.Request.IsDeleted && !q.RequestCV.CV.IsDeleted) &&
                                                                             (branchId.HasValue ? q.RequestCV.Request.BranchId == branchId.Value : true) &&
                                                                             (q.Status == RequestCVStatus.PassedTest);

            var educationHaveCVPassTest = WorkScope.GetAll<RequestCVStatusHistory>()
                .Where(expression)
                .Select(s => new { s.RequestCV.CVId })
                .AsNoTracking()
                .AsEnumerable()
                .DistinctBy(s => s.CVId)
                .GroupBy(s => s.CVId)
                .Select(s => new
                {
                    s.Key,
                    TotalCV = s.Count()
                });

            var educationSaveCVPassTest = educationHaveCVPassTest.ToDictionary(s => s.Key);

            var educationIdsHaveCVPassTest = educationSaveCVPassTest.Keys.ToList();
            var educations = WorkScope.GetAll<CVEducation>()
                .Where(s => educationIdsHaveCVPassTest.Contains(s.CVId))
                .Select(e => new EducationDto
                {
                    Id = e.Education.Id,
                    Name = e.Education.Name,
                    CVId = e.CVId,
                    ColorCode = e.Education.ColorCode
                })
                .AsNoTracking()
                .AsEnumerable()
                .ToList();

            var report = educations
                .GroupBy(e => e.Name)
                .Select(e => new ReportEducationHaveCVPassTestDto
                {
                    EducationId = e.First().Id,
                    EducationName = e.Key,
                    ColorCode = e.First().ColorCode,
                    TotalCV = e.Count()
                })
                .ToList();
            var result = new ReportEducationByBranchDto<ReportEducationHaveCVPassTestDto>();
            result.Educations = report;
            await GetBranchInfo(result, branchId);
            return result;
        }
        public async Task<ReportEducationByBranchDto<ReportEducationHaveCVOnboardDto>> GetEducationInternOnboarded(DateTime fd, DateTime td, long? branchId)
        {
            Expression<Func<RequestCV, bool>> expression = q => (q.OnboardDate.HasValue ? (q.OnboardDate.Value.Date >= fd.Date && q.OnboardDate.Value.Date <= td.Date) : false) &&
                                                                (q.Request.UserType == UserType.Intern) &&
                                                                (!q.Request.IsDeleted && !q.Request.IsDeleted) &&
                                                                (branchId.HasValue ? q.CV.BranchId == branchId.Value : true) &&
                                                                (q.Status == RequestCVStatus.Onboarded);

            var educationHaveCVOnboarded = WorkScope.GetAll<RequestCV>()
                .Where(expression)
                .Select(s => new { s.CVId })
                .AsNoTracking()
                .AsEnumerable()
                .DistinctBy(s => s.CVId)
                .GroupBy(s => s.CVId)
                .Select(s => new
                {
                    s.Key,
                    TotalCV = s.Count()
                })
                .ToDictionary(s => s.Key);

            var educationIdsaveCVOnboarded = educationHaveCVOnboarded.Keys.ToList();

            var educations = WorkScope.GetAll<CVEducation>()
                .Where(s => educationIdsaveCVOnboarded.Contains(s.CVId))
                .Select(e => new EducationDto
                {
                    Id = e.Education.Id,
                    Name = e.Education.Name,
                    CVId = e.CVId,
                    ColorCode = e.Education.ColorCode
                })
                .AsNoTracking()
                .AsEnumerable()
                .ToList();

            var report = educations
                .GroupBy(e => e.Name)
                .Select(e => new ReportEducationHaveCVOnboardDto
                {
                    EducationId = e.First().Id,
                    EducationName = e.Key,
                    ColorCode = e.First().ColorCode,
                    TotalCV = e.Count()
                })
                .ToList();

            var result = new ReportEducationByBranchDto<ReportEducationHaveCVOnboardDto>();
            result.Educations = report;
            await GetBranchInfo(result, branchId);

            return result;
        }

        public async Task<ReportEducationByBranchDto<ReportEducationHaveCVPassTestDto>> GetEducationPassInterView(DateTime fd, DateTime td, long? branchId)
        {
            Expression<Func<RequestCV, bool>> expression = q => (q.LastModificationTime >= fd.Date && q.LastModificationTime <= td.Date) &&
                                                                (q.Request.UserType == UserType.Intern) &&
                                                                (!q.Request.IsDeleted && !q.CV.IsDeleted) &&
                                                                (branchId.HasValue ? q.CV.BranchId == branchId.Value : true) &&
                                                                (q.Status == RequestCVStatus.PassedInterview);

            var educationHaveCVPassInterView = WorkScope.GetAll<RequestCV>()
                .Where(expression)
                .Select(s => new { s.CVId })
                .AsNoTracking()
                .AsEnumerable()
                .DistinctBy(s => s.CVId)
                .GroupBy(s => s.CVId)
                .Select(s => new
                {
                    s.Key,
                    TotalCV = s.Count()
                })
                .ToDictionary(s => s.Key);

            var educationIdsaveCVPassInterView = educationHaveCVPassInterView.Keys.ToList();

            var educations = WorkScope.GetAll<CVEducation>()
                .Where(s => educationIdsaveCVPassInterView.Contains(s.CVId))
                .Select(e => new EducationDto
                {
                    Id = e.Education.Id,
                    Name = e.Education.Name,
                    CVId = e.CVId,
                    ColorCode = e.Education.ColorCode
                })
                .AsNoTracking()
                .AsEnumerable()
                .ToList();

            var report = educations
                .GroupBy(e => e.Name)
                .Select(e => new ReportEducationHaveCVPassTestDto
                {
                    EducationId = e.First().Id,
                    EducationName = e.Key,
                    ColorCode = e.First().ColorCode,
                    TotalCV = e.Count()
                })
                .ToList();

            var result = new ReportEducationByBranchDto<ReportEducationHaveCVPassTestDto>();
            result.Educations = report;
            await GetBranchInfo(result, branchId);

            return result;
        }
        public async Task<List<IdAndNameDto>> GetUserCreated()
        {
            var query = from cv in WorkScope.GetAll<CV>()
                        where cv.CreatorUserId.HasValue && !cv.CreatorUser.IsDeleted && cv.CreatorUser.IsActive
                        select new IdAndNameDto
                        {
                            Id = cv.CreatorUserId.Value,
                            Name = cv.CreatorUserId == AbpSession.UserId ?
                                    "@Me" :
                                    cv.CreatorUser.FullName,
                        };
            var allCreator = query
                .Distinct()
                .AsNoTracking()
                .AsEnumerable()
                .OrderBy(x => x.Name);

            return allCreator.ToList();
        }
        public async Task<FileContentResult> ExportOverviewHiring(ExportChartInput input)
        {
            var noData = "NoData";
            var percentDefault = 0;
            var pieChartCvSource = new Chart();
            var pieChartStatusStatistics = new Chart();
            pieChartCvSource.ModelCharts = new List<ModelChart>();
            pieChartStatusStatistics.ModelCharts = new List<ModelChart>();
            foreach (var branch in input.Branchs)
            {
                var overviewHiring = await GetOverviewHiring(input.FromDate.Value, input.ToDate.Value, input.userType, branch.Id, input.userId);
                var subPositionStatistics = overviewHiring?.SubPositionStatistics.FirstOrDefault();
                var cvSourceStatistics = subPositionStatistics?.CVSourceStatistics.ToList();
                var totalOverviewHiring = overviewHiring.TotalOverviewHiring.TotalCVSources;
                var sumtotalOverviewHiring = totalOverviewHiring.Sum();
                var templatesCvSource = cvSourceStatistics?.Select(s => new Templates
                {
                    Key = s.Name,
                    Percent = sumtotalOverviewHiring > percentDefault ? (totalOverviewHiring[cvSourceStatistics.IndexOf(s)] / (float)sumtotalOverviewHiring) : percentDefault,
                    Quantity = (totalOverviewHiring[cvSourceStatistics.IndexOf(s)])
                }).ToList();
                pieChartCvSource.ModelCharts.Add(new ModelChart
                {
                    BranchName = branch.DisplayName,
                    Temaplates = templatesCvSource ?? new List<Templates>() { new Templates { Key = noData, Percent = percentDefault, Quantity = 0 } },
                });

                var statusStatistics = subPositionStatistics?.StatusStatistics.ToList();
                var totalStatus = overviewHiring.TotalOverviewHiring.TotalStatuses.Select(t => t.TotalCV).ToList();
                var sumtotalStatus = totalStatus.Sum();

                var templatesStatistics = statusStatistics?.Select(s => new Templates
                {
                    Key = s.Name,
                    Percent = sumtotalStatus > percentDefault ? (totalStatus[statusStatistics.IndexOf(s)] / (float)sumtotalStatus) : percentDefault,
                    Quantity = totalStatus[statusStatistics.IndexOf(s)]
                }).ToList();

                pieChartStatusStatistics.ModelCharts.Add(new ModelChart
                {
                    BranchName = branch.DisplayName,
                    Temaplates = templatesStatistics ?? new List<Templates>() { new Templates { Key = noData, Percent = percentDefault, Quantity = 0 } },
                });
            }

            pieChartCvSource.NameSheet = "CV Source";
            var excelBytesCvSources = await AddChart(pieChartCvSource, ChartType.Pie);
            pieChartStatusStatistics.NameSheet = "Candidate Status";
            var excelBytesCandidateStatus = await AddChart(pieChartStatusStatistics, ChartType.Pie);
            var combinedBytes = CombineExcelFiles(excelBytesCvSources, excelBytesCandidateStatus);

            return new FileContentResult(combinedBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "exported_data.xlsx"
            };
        }
        public async Task<FileContentResult> ExportInternEducation(ExportChartEducationInput input)
        {
            var statusData = await GetInternEducationStatusDataForExport(input);
            var selectedBranches = input.Branchs != null && input.Branchs.Any()
                ? input.Branchs
                : new List<BranchDtoExport>
                {
                    new BranchDtoExport
                    {
                        Id = null,
                        DisplayName = "All Branch"
                    }
                };
            var selectedCVStatuses = input.CVStatuses ?? new List<InternEducationCVStatus>();
            var selectedCandidateStatuses = input.CandidateStatuses ?? new List<RequestCVStatus>();

            // Keep the API backward compatible for callers that do not send status filters.
            if (!selectedCVStatuses.Any() && !selectedCandidateStatuses.Any())
            {
                selectedCVStatuses = Enum.GetValues(typeof(InternEducationCVStatus))
                    .Cast<InternEducationCVStatus>()
                    .ToList();
                selectedCandidateStatuses = Enum.GetValues(typeof(RequestCVStatus))
                    .Cast<RequestCVStatus>()
                    .ToList();
            }

            var excelFiles = new List<byte[]>();
            var now = DateTime.Now;

            foreach (var selectedCVStatus in selectedCVStatuses
                .Where(status => Enum.IsDefined(typeof(InternEducationCVStatus), status))
                .Distinct()
                .OrderBy(status => status))
            {
                string sheetName;
                Func<InternEducationStatusExportDto, bool> statusFilter;

                switch (selectedCVStatus)
                {
                    case InternEducationCVStatus.NewUnprocessed:
                        sheetName = "CV - New Unprocessed";
                        statusFilter = candidate =>
                            candidate.CVStatus == CVStatus.New
                            && candidate.CVStatusTime.AddDays(3) < now;
                        break;
                    case InternEducationCVStatus.NewNormal:
                        sheetName = "CV - New Normal";
                        statusFilter = candidate =>
                            candidate.CVStatus == CVStatus.New
                            && candidate.CVStatusTime.AddDays(3) >= now;
                        break;
                    case InternEducationCVStatus.Contacting:
                        sheetName = "CV - Contacting";
                        statusFilter = candidate => candidate.CVStatus == CVStatus.Contacting;
                        break;
                    case InternEducationCVStatus.Passed:
                        sheetName = "CV - Passed";
                        statusFilter = candidate => candidate.CVStatus == CVStatus.Passed;
                        break;
                    case InternEducationCVStatus.Failed:
                        sheetName = "CV - Failed";
                        statusFilter = candidate => candidate.CVStatus == CVStatus.Failed;
                        break;
                    case InternEducationCVStatus.Draft:
                        sheetName = "CV - Draft";
                        statusFilter = candidate => candidate.CVStatus == CVStatus.Draft;
                        break;
                    default:
                        continue;
                }

                excelFiles.Add(await AddChart(
                    BuildInternEducationStatusChart(
                        sheetName,
                        selectedBranches,
                        statusData,
                        statusFilter),
                    ChartType.Column,
                    includeBranch: true));
            }

            foreach (var candidateStatus in selectedCandidateStatuses
                .Where(status => Enum.IsDefined(typeof(RequestCVStatus), status))
                .Distinct()
                .OrderBy(status => status))
            {
                var candidateStatusName = DictionaryHelper.RequestCVStatusDict[candidateStatus];
                excelFiles.Add(await AddChart(
                    BuildInternEducationStatusChart(
                        $"Candidate - {candidateStatusName}",
                        selectedBranches,
                        statusData,
                        candidate => candidate.CandidateStatus == candidateStatus),
                    ChartType.Column,
                    includeBranch: true));
            }

            var combinedBytes = CombineExcelFiles(excelFiles.ToArray());
            return new FileContentResult(combinedBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "exported_data.xlsx"
            };
        }

        private async Task<List<InternEducationStatusExportDto>> GetInternEducationStatusDataForExport(ExportChartEducationInput input)
        {
            var fromDate = input.FromDate.Value.ToLocalTime().Date;
            var toDate = input.ToDate.Value.ToLocalTime().Date.AddDays(1);
            var selectedBranches = input.Branchs ?? new List<BranchDtoExport>();
            var selectedBranchIds = selectedBranches
                .Where(branch => branch.Id.HasValue)
                .Select(branch => branch.Id.Value)
                .Distinct()
                .ToList();
            var isAllBranch = selectedBranches.Count == 0 || selectedBranches.Any(branch => !branch.Id.HasValue);

            var cvs = await WorkScope.GetAll<CV>()
                .Where(cv => cv.LastModificationTime >= fromDate && cv.LastModificationTime < toDate)
                .Where(cv => cv.UserType == UserType.Intern && !cv.IsDeleted)
                .WhereIf(!isAllBranch, cv => selectedBranchIds.Contains(cv.BranchId))
                .Select(cv => new
                {
                    cv.BranchId,
                    cv.CVStatus,
                    CVCreationTime = cv.CreationTime,
                    CVLastModificationTime = cv.LastModificationTime,
                    CandidateStatus = cv.RequestCVs
                        .Where(requestCV => !requestCV.IsDeleted)
                        .OrderByDescending(requestCV => requestCV.LastModificationTime)
                        .Select(requestCV => (RequestCVStatus?)requestCV.Status)
                        .FirstOrDefault(),
                    EducationId = cv.CVEducations
                        .OrderBy(education => education.CreationTime)
                        .Select(education => (long?)education.EducationId)
                        .FirstOrDefault()
                })
                .AsNoTracking()
                .ToListAsync();

            var educationNames = await WorkScope.GetAll<Education>()
                .AsNoTracking()
                .ToDictionaryAsync(education => education.Id, education => education.Name);

            return cvs
                .Select(cv => new InternEducationStatusExportDto
                {
                    BranchId = cv.BranchId,
                    EducationId = cv.EducationId.HasValue && educationNames.ContainsKey(cv.EducationId.Value)
                        ? cv.EducationId.Value
                        : 0,
                    EducationName = cv.EducationId.HasValue && educationNames.ContainsKey(cv.EducationId.Value)
                        ? educationNames[cv.EducationId.Value]
                        : "No Education",
                    CVStatus = cv.CVStatus,
                    CandidateStatus = cv.CandidateStatus,
                    CVStatusTime = cv.CVLastModificationTime ?? cv.CVCreationTime
                })
                .ToList();
        }

        private Chart BuildInternEducationStatusChart(
            string sheetName,
            List<BranchDtoExport> branches,
            List<InternEducationStatusExportDto> statusData,
            Func<InternEducationStatusExportDto, bool> statusFilter)
        {
            var chart = new Chart
            {
                NameSheet = GetSafeWorksheetName(sheetName),
                ModelCharts = new List<ModelChart>()
            };

            foreach (var branch in branches)
            {
                var branchData = statusData
                    .Where(statusFilter)
                    .Where(candidate => !branch.Id.HasValue || candidate.BranchId == branch.Id.Value)
                    .GroupBy(candidate => new
                    {
                        candidate.EducationId,
                        candidate.EducationName
                    })
                    .Select(group => new
                    {
                        group.Key.EducationName,
                        Quantity = group.Count()
                    })
                    .OrderByDescending(education => education.Quantity)
                    .ThenBy(education => education.EducationName)
                    .ToList();

                var total = branchData.Sum(education => education.Quantity);
                var templates = branchData
                    .Select(education => new Templates
                    {
                        Key = education.EducationName,
                        Percent = total > 0 ? education.Quantity / (float)total : 0,
                        Quantity = education.Quantity
                    })
                    .ToList();

                if (templates.Count == 0)
                {
                    templates.Add(new Templates
                    {
                        Key = "NoData",
                        Percent = 0,
                        Quantity = 0
                    });
                }

                chart.ModelCharts.Add(new ModelChart
                {
                    BranchName = string.IsNullOrWhiteSpace(branch.DisplayName) ? "All Branch" : branch.DisplayName,
                    Temaplates = templates
                });
            }

            return chart;
        }

        private string GetSafeWorksheetName(string sheetName)
        {
            return sheetName.Length <= 31
                ? sheetName
                : sheetName.Substring(0, 31);
        }

        private async Task<byte[]> AddChart(Chart input, ChartType typeChart, bool includeBranch = false)
        {
            using (var package = new ExcelPackage())
            {
                var noData = "NoData";
                var startRow = 1;
                var endRow = 0;
                var startColumn = 1;
                var endColumn = 2;
                var columnKey = GetColumnNameFromNumber(startColumn);
                var columnValue = GetColumnNameFromNumber(endColumn);
                var worksheet = package.Workbook.Worksheets.Add(input.NameSheet);
                foreach (var item in input.ModelCharts)
                {
                    if (includeBranch)
                    {
                        var rows = item.Temaplates.Select(template => new
                        {
                            Key = template.Key,
                            Percent = template.Percent,
                            Quantity = template.Quantity,
                            Branch = item.BranchName ?? "All Branch"
                        }).ToList();
                        worksheet.Cells[$"A{startRow}"].LoadFromCollection(rows, true, TableStyles.Light9);
                    }
                    else
                    {
                        worksheet.Cells[$"{columnKey}{startRow}"].LoadFromCollection(item?.Temaplates, true, TableStyles.Light9);
                    }
                    worksheet.Cells.AutoFitColumns();
                    worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    endRow = (startRow + item.Temaplates.Count);
                    worksheet.Cells[$"{columnValue}{startRow + 1}:{columnValue}{endRow}"].Style.Numberformat.Format = "0.00%";

                    var totalRow = endRow + 1;
                    worksheet.Cells[$"{columnKey}{totalRow}"].Value = "Total";
                    worksheet.Cells[$"{columnKey}{totalRow}"].Style.Font.Bold = true;

                    var columnPercent = GetColumnNameFromNumber(startColumn + 1);
                    var sumPercent = item.Temaplates.Sum(s => s.Percent);
                    worksheet.Cells[$"{columnPercent}{totalRow}"].Value = sumPercent;
                    worksheet.Cells[$"{columnPercent}{totalRow}"].Style.Numberformat.Format = "0.00%";
                    worksheet.Cells[$"{columnPercent}{totalRow}"].Style.Font.Bold = true;

                    var columnQuantity = GetColumnNameFromNumber(startColumn + 2);
                    var sumQuantity = item.Temaplates.Sum(s => s.Quantity);
                    worksheet.Cells[$"{columnQuantity}{totalRow}"].Value = sumQuantity;
                    worksheet.Cells[$"{columnQuantity}{totalRow}"].Style.Font.Bold = true;


                    var labelsRange = worksheet.Cells[$"{columnKey}{startRow + 1}:{columnKey}{endRow}"];
                    var chartRange = worksheet.Cells[$"{columnValue}{startRow + 1}:{columnValue}{endRow}"];
                    input.Row = startRow - 1;
                    input.Column = endColumn + 3;
                    switch (typeChart)
                    {
                        case ChartType.Pie:
                            input.PixelWidth = 400;
                            input.PixelHeight = 300;
                            var pieChart = worksheet.Drawings.AddChart(item.BranchName ?? "All Branch", eChartType.Pie);
                            pieChart.Title.Text = item.BranchName ?? "All Branch";
                            pieChart.SetSize(input.PixelWidth, input.PixelHeight);
                            pieChart.SetPosition(input.Row, input.RowOffsetPixels, input.Column, input.ColumnOffsetPixels);
                            pieChart.Series.Add(chartRange, labelsRange);
                            break;
                        case ChartType.Column:
                            input.PixelWidth = 600;
                            input.PixelHeight = 300;
                            var columnChart = worksheet.Drawings.AddChart(item.BranchName ?? "All Branch", eChartType.ColumnClustered3D);
                            columnChart.Title.Text = item.BranchName ?? "All Branch";
                            columnChart.SetSize(input.PixelWidth, input.PixelHeight);
                            columnChart.SetPosition(input.Row, input.RowOffsetPixels, input.Column, input.ColumnOffsetPixels);
                            columnChart.Series.Add(chartRange, labelsRange);
                            break;
                    }
                    startRow = endRow + 15;
                }
                return package.GetAsByteArray();
            }
        }
        private string GetColumnNameFromNumber(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = string.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }
        public byte[] CombineExcelFiles(params byte[][] excelBytesArray)
        {
            using (var combinedPackage = new ExcelPackage())
            {

                foreach (var excelBytes in excelBytesArray)
                {
                    using (var package = new ExcelPackage(new MemoryStream(excelBytes)))
                    {
                        foreach (var sheet in package.Workbook.Worksheets)
                        {
                            combinedPackage.Workbook.Worksheets.Add(sheet.Name, sheet);
                        }
                    }
                }
                return combinedPackage.GetAsByteArray();
            }
        }
        private async Task GetBranchInfo<T>(T dto, long? branchId) where T : class
        {
            Type dtoType = typeof(T);
            var propBranchName = dtoType.GetProperty("BranchName");
            var propBranchId = dtoType.GetProperty("BranchId");
            if (!branchId.HasValue)
            {
                propBranchName.SetValue(dto, "All Company");
                return;
            }
            var branch = await WorkScope.GetAsync<Branch>(branchId.Value);
            propBranchName.SetValue(dto, branch.Name);
            propBranchId.SetValue(dto, branch.Id);
        }

        public async Task<ReportEducationByBranchDto<CandidateQuantityByEducationReportDto>> ReportCandidateQuantityByEducation(DateTime fd, DateTime td, long? branchId, UserType? userType = UserType.Intern)
        {
            var startDate = fd.Date;
            var endDate = td.Date.AddDays(1);

            Expression<Func<RequestCV, bool>> requestCVPredicate = q =>
                (q.Request.UserType == userType)
                && (q.LastModificationTime >= startDate && q.LastModificationTime < endDate)
                && (!branchId.HasValue || q.CV.BranchId == branchId.Value);

            var requestCVs = WorkScope.GetAll<RequestCV>()
                .Where(requestCVPredicate)
                .GroupBy(x => x.CVId)
                .Select(x => new
                {
                    CVId = x.Key,
                    RequestCVStatus = x.OrderByDescending(r => r.LastModificationTime).FirstOrDefault().Status,
                    EducationId = x.OrderByDescending(r => r.LastModificationTime).FirstOrDefault().CV.CVEducations.Any() ? x.OrderByDescending(r => r.LastModificationTime).FirstOrDefault().CV.CVEducations.OrderBy(e => e.CreationTime).FirstOrDefault().EducationId : (long?)null
                })
                .ToList();

            var educations = WorkScope.GetAll<Education>()
                .AsEnumerable()
                .Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.ColorCode,
                    CVs = requestCVs.Where(x => x.EducationId == e.Id).ToList()
                }).ToList();

            var report = educations.Select(x => new CandidateQuantityByEducationReportDto
            {
                EducationId = x.Id,
                EducationName = x.Name,
                ColorCode = x.ColorCode,
                TotalCV = x.CVs.Count,
                PassCV = x.CVs.Count(cv => cv.RequestCVStatus == RequestCVStatus.AddedCV
                                    || cv.RequestCVStatus == RequestCVStatus.ScheduledTest
                                    || cv.RequestCVStatus == RequestCVStatus.FailedTest
                                    || cv.RequestCVStatus == RequestCVStatus.RejectedTest
                                    || cv.RequestCVStatus == RequestCVStatus.RejectedApply),
                PassTest = x.CVs.Count(cv => cv.RequestCVStatus == RequestCVStatus.PassedTest
                                    || cv.RequestCVStatus == RequestCVStatus.ScheduledInterview
                                    || cv.RequestCVStatus == RequestCVStatus.FailedInterview
                                    || cv.RequestCVStatus == RequestCVStatus.RejectedInterview),
                PassInterview = x.CVs.Count(cv => cv.RequestCVStatus == RequestCVStatus.PassedInterview
                                    || cv.RequestCVStatus == RequestCVStatus.AcceptedOffer
                                    || cv.RequestCVStatus == RequestCVStatus.RejectedOffer),
                Onboard = x.CVs.Count(cv => cv.RequestCVStatus == RequestCVStatus.Onboarded)
            }).ToList();

            var result = new ReportEducationByBranchDto<CandidateQuantityByEducationReportDto>
            {
                Educations = report
            };
            await GetBranchInfo(result, branchId);

            return result;
        }

        public async Task<ReportEducationByBranchDto<CandidateDensityByEducationReportDto>> ReportCandidateDensityByEducation(DateTime fd, DateTime td, long? branchId, UserType? userType = UserType.Intern)
        {
            var startDate = fd.Date;
            var endDate = td.Date.AddDays(1);

            Expression<Func<RequestCV, bool>> requestCVPredicate = q =>
                (q.Request.UserType == userType)
                && (q.LastModificationTime >= startDate && q.LastModificationTime < endDate)
                && (!branchId.HasValue || q.CV.BranchId == branchId.Value);

            var requestCVs = WorkScope.GetAll<RequestCV>()
                .Where(requestCVPredicate)
                .GroupBy(x => x.CVId)
                .Select(x => new
                {
                    CVId = x.Key,
                    RequestCVStatus = x.OrderByDescending(r => r.LastModificationTime).FirstOrDefault().Status,
                    EducationId = x.OrderByDescending(r => r.LastModificationTime).FirstOrDefault().CV.CVEducations.Any() ? x.OrderByDescending(r => r.LastModificationTime).FirstOrDefault().CV.CVEducations.OrderBy(e => e.CreationTime).FirstOrDefault().EducationId : (long?)null
                })
                .ToList();

            var educations = WorkScope.GetAll<Education>()
                .AsEnumerable()
                .Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.ColorCode,
                    CVs = requestCVs.Where(x => x.EducationId == e.Id).ToList()
                }).ToList();

            var totalPassCV = requestCVs.Count(cv => cv.RequestCVStatus == RequestCVStatus.AddedCV
                                                || cv.RequestCVStatus == RequestCVStatus.ScheduledTest
                                                || cv.RequestCVStatus == RequestCVStatus.FailedTest
                                                || cv.RequestCVStatus == RequestCVStatus.RejectedTest
                                                || cv.RequestCVStatus == RequestCVStatus.RejectedApply);

            var totalPassTest = requestCVs.Count(cv => cv.RequestCVStatus == RequestCVStatus.PassedTest
                                                || cv.RequestCVStatus == RequestCVStatus.ScheduledInterview
                                                || cv.RequestCVStatus == RequestCVStatus.FailedInterview
                                                || cv.RequestCVStatus == RequestCVStatus.RejectedInterview);

            var totalPassInterview = requestCVs.Count(cv => cv.RequestCVStatus == RequestCVStatus.PassedInterview
                                                || cv.RequestCVStatus == RequestCVStatus.AcceptedOffer
                                                || cv.RequestCVStatus == RequestCVStatus.RejectedOffer);

            var report = educations.Select(x => new CandidateDensityByEducationReportDto
            {
                EducationId = x.Id,
                EducationName = x.Name,
                ColorCode = x.ColorCode,
                PassCV = CaculateCandidateDensity(x.CVs.Count(cv => cv.RequestCVStatus == RequestCVStatus.AddedCV
                                                                    || cv.RequestCVStatus == RequestCVStatus.ScheduledTest
                                                                    || cv.RequestCVStatus == RequestCVStatus.FailedTest
                                                                    || cv.RequestCVStatus == RequestCVStatus.RejectedTest
                                                                    || cv.RequestCVStatus == RequestCVStatus.RejectedApply), totalPassCV),
                PassTest = CaculateCandidateDensity(x.CVs.Count(cv => cv.RequestCVStatus == RequestCVStatus.PassedTest
                                                                    || cv.RequestCVStatus == RequestCVStatus.ScheduledInterview
                                                                    || cv.RequestCVStatus == RequestCVStatus.FailedInterview
                                                                    || cv.RequestCVStatus == RequestCVStatus.RejectedInterview), totalPassTest),
                PassInterview = CaculateCandidateDensity(x.CVs.Count(cv => cv.RequestCVStatus == RequestCVStatus.PassedInterview
                                                                    || cv.RequestCVStatus == RequestCVStatus.AcceptedOffer
                                                                    || cv.RequestCVStatus == RequestCVStatus.RejectedOffer), totalPassInterview),
                Onboard = CaculateCandidateDensity(x.CVs.Count(cv => cv.RequestCVStatus == RequestCVStatus.Onboarded), requestCVs.Count(cv => cv.RequestCVStatus == RequestCVStatus.Onboarded))
            }).ToList();

            var result = new ReportEducationByBranchDto<CandidateDensityByEducationReportDto>
            {
                Educations = report
            };
            await GetBranchInfo(result, branchId);

            return result;
        }

        private double CaculateCandidateDensity(int numerator, int denominator)
        {
            if (numerator == 0 || denominator == 0)
            {
                return Math.Round((double)0, 2);
            }

            return Math.Round((double)numerator / denominator * 100, 2);
        }

    }
}

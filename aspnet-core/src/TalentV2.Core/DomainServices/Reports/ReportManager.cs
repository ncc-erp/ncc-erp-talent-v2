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

            var cvStatistics = await GetDicSubPositionIdToStatisticQuantityCV(fd,td,userType,branchId, userId);

            var cvsources = await WorkScope.GetAll<CVSource>().OrderBy(s => s.Name).ToListAsync();
            var qstatisticIQCVSources = IQStatisticCVSources(fd, td, userType, branchId, cvsources, userId);
            var qstatisticBySubPostion = IQStatisticRequestCVHistory(fd, td, userType, branchId, userId);

            var statisticBySubPosition = qstatisticBySubPostion.ToDictionary
                (
                    x => x.SubPositionId,
                    x => new {x.StatusStatistics}
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
  
            foreach(var key in listKeyHaveZeroValue)
            {
                subPositionStatistics.Remove(key);
            }

            var finalPositionStatistics = subPositionStatistics.Values.ToList();
            var overviewHiring = await GetResultOverviewHiring(branchId, finalPositionStatistics);
            overviewHiring.TotalOverviewHiring = GetTotalOverviewHiring(finalPositionStatistics, cvsources.Count);

            return overviewHiring;
        }

        public async Task<List<RecruitmentOverviewResponseDto>> GetRecruitmentOverview(RecruitmentOverviewRequestDto request)
        {
            var fromDate = request.FromDate.Date;
            var toDate = request.ToDate.Date;

            var recruitmentOverviewResult = new List<RecruitmentOverviewResponseDto>();
            if (request.IsGetAllBranch)
            {
                var allBranchHasCVReport = await IQGetRecruitmentOverviewByCVs(fromDate, toDate, request.UserType, null, request.UserId).ToListAsync();
                var allBranchRequestReport = await IQGetRecruitmentOverviewByRequests(fromDate, toDate, request.UserType, null, request.UserId).ToListAsync();
                recruitmentOverviewResult.AddRange(AddRequestHasNotCVToRecruitmentOverviewResult(allBranchRequestReport, allBranchHasCVReport));
            }
            if (request.BranchIds != null && request.BranchIds.Any())
            {
                var detailBranchHasCVReport = await IQGetRecruitmentOverviewByCVs(fromDate, toDate, request.UserType, request.BranchIds, request.UserId).ToListAsync();
                var detailBranchRequestReport = await IQGetRecruitmentOverviewByRequests(fromDate, toDate, request.UserType, request.BranchIds, request.UserId).ToListAsync();
                recruitmentOverviewResult.AddRange(AddRequestHasNotCVToRecruitmentOverviewResult(detailBranchRequestReport, detailBranchHasCVReport));
            }

            return recruitmentOverviewResult;
        }

        private List<RecruitmentOverviewResponseDto> AddRequestHasNotCVToRecruitmentOverviewResult(List<RecruitmentOverviewResponseDto> requests, List<RecruitmentOverviewResponseDto> result)
        {
            foreach (var request in requests)
            {
                var existPosition = result
                    .Where(branch => branch.BranchId == request.BranchId)
                    .Select(branch => branch.SubPositionStatistics)
                    .FirstOrDefault();
                var existPositionIds = existPosition.Select(position => position.SubPositionId);
                foreach (var requestPosition in request.SubPositionStatistics)
                {
                    if (!existPositionIds.Contains(requestPosition.SubPositionId))
                    {
                        existPosition.Add(requestPosition);
                    }
                }
            }
            return result;
        }

        private IQueryable<RecruitmentOverviewResponseDto> IQGetRecruitmentOverviewByCVs(DateTime fromDate, DateTime toDate, UserType? userType, List<long> branchIds, long? userId)
        {
            bool isGetDetailBranch = branchIds != null && branchIds.Any();
            return WorkScope.GetAll<CV>()
                .Where(cv => cv.LastModificationTime >= fromDate && cv.LastModificationTime < toDate.AddDays(1) && !cv.IsDeleted)
                .WhereIf(userType.HasValue, cv => cv.UserType.Equals(userType))
                .WhereIf(isGetDetailBranch, cv => branchIds.Contains(cv.BranchId))
                .WhereIf(userId.HasValue, cv => cv.CreatorUserId.Equals(userId))
                .GroupBy(cv => isGetDetailBranch ? cv.BranchId : 0)
                .Select(branchGroup => new RecruitmentOverviewResponseDto
                {
                    BranchId = isGetDetailBranch ? branchGroup.Key : null,
                    BranchName = isGetDetailBranch ? branchGroup.Select(cv => cv.Branch.Name).FirstOrDefault() : "All Company",
                    SubPositionStatistics = branchGroup.GroupBy(cv => cv.SubPositionId).Select(positionGroup => new SubPositionStatisticV2
                    {
                        SubPositionId = positionGroup.Key,
                        SubPositionName = positionGroup.Select(cv => cv.SubPosition.Name).FirstOrDefault(),
                        RequestQuantity = IQGetRequests(fromDate, toDate, userType, branchIds, userId)
                        .Where(request => request.SubPositionId == positionGroup.Key && (isGetDetailBranch ? request.BranchId.Equals(branchGroup.Key) : true))
                        .Select(request => request.Quantity)
                        .Sum(),
                        ApplyQuantity = positionGroup.Count(),
                        CVStatusStatistics = positionGroup.GroupBy(cv => cv.CVStatus).Select(CVStatusGroup => new CVStatusStatistic
                        {
                            Id = CVStatusGroup.Key,
                            Name = CVStatusGroup.Key.ToString(),
                            Quantity = CVStatusGroup.Count()
                        }).ToList(),
                        CVSourceStatistics = positionGroup.GroupBy(cv => cv.CVSourceId).Select(CVSourceGroup => new CVSourceStatisticV2
                        {
                            Id = CVSourceGroup.Key,
                            Name = CVSourceGroup.Select(cv => cv.CVSource.Name).FirstOrDefault(),
                            Quantity = CVSourceGroup.Count()
                        }).ToList(),
                        CandidateStatusStatistics = positionGroup
                        .Select(cv => cv.RequestCVs.Where(requestCV => !requestCV.IsDeleted).OrderByDescending(requestCV => requestCV.LastModificationTime).FirstOrDefault())
                        .GroupBy(requestCV => requestCV.Status)
                        .Select(requestCVStatusGroup => new RequestCVStatusStatistic
                        {
                            Id = requestCVStatusGroup.Key,
                            Name = requestCVStatusGroup.Key.ToString(),
                            Quantity = requestCVStatusGroup.Count()
                        }).ToList(),
                    }).ToList(),
                    Total = new TotalStatistics
                    {
                        RequestQuantity = IQGetRequests(fromDate, toDate, userType, branchIds, userId)
                        .Where(request => isGetDetailBranch ? request.BranchId.Equals(branchGroup.Key) : true)
                        .Select(request => request.Quantity)
                        .Sum(),
                        ApplyQuantity = branchGroup.Count(),
                        CVStatusStatistics = branchGroup.GroupBy(cv => cv.CVStatus).Select(CVStatusGroup => new CVStatusStatistic
                        {
                            Id = CVStatusGroup.Key,
                            Name = CVStatusGroup.Key.ToString(),
                            Quantity = CVStatusGroup.Count()
                        }).ToList(),
                        CVSourceStatistics = branchGroup.GroupBy(cv => cv.CVSourceId).Select(CVSourceGroup => new CVSourceStatisticV2
                        {
                            Id = CVSourceGroup.Key,
                            Name = CVSourceGroup.Select(cv => cv.CVSource.Name).FirstOrDefault(),
                            Quantity = CVSourceGroup.Count()
                        }).ToList(),
                        CandidateStatusStatistics = branchGroup
                        .Select(cv => cv.RequestCVs.Where(requestCV => !requestCV.IsDeleted).OrderByDescending(requestCV => requestCV.LastModificationTime).FirstOrDefault())
                        .GroupBy(requestCV => requestCV.Status)
                        .Select(requestCVGroup => new RequestCVStatusStatistic
                        {
                            Id = requestCVGroup.Key,
                            Name = requestCVGroup.Key.ToString(),
                            Quantity = requestCVGroup.Count()
                        }).ToList(),
                    }
                });
        }

        private IQueryable<RequestStatistic> IQGetRequests(DateTime fromDate, DateTime toDate, UserType? userType, List<long> branchIds, long? userId)
        {
            bool isGetDetailBranch = branchIds != null && branchIds.Any();
            return WorkScope.GetAll<Request>()
                .Where(request => request.CreationTime >= fromDate && request.CreationTime < toDate.AddDays(1) && !request.IsDeleted)
                .WhereIf(userType.HasValue, request => request.UserType.Equals(userType))
                .WhereIf(isGetDetailBranch, request => branchIds.Contains(request.BranchId))
                .WhereIf(userId.HasValue, request => request.CreatorUserId.Equals(userId))
                .Select(request => new RequestStatistic
                {
                    BranchId = request.BranchId,
                    BranchName = request.Branch.Name,
                    SubPositionId = request.SubPositionId,
                    SubPositionName = request.SubPosition.Name,
                    Quantity = request.Quantity,
                });
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

            await GetBranchInfo(overviewHiring,branchId);

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
                for(int i = 0; i < sizeCVSource; i++)
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
            for(int i = 0; i < capacity; i++)
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
                                                                         (userId.HasValue ? q.CreatorUserId == userId.Value: true);

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
                                                                         (branchId.HasValue ? q.RequestCV.CV.BranchId == branchId.Value : true)&&
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
                                         TotalCV = gr.Where(q=> q.CreationTime.Date >= fd.Date && q.CreationTime.Date <= td.Date)
                                                     .Where(q=>branchId.HasValue ? q.BranchId == branchId.Value : true)
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
                    .Select(e => new ReportEducationHaveCVOnboardDto
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
                        where cv.CreatorUserId.HasValue  && !cv.CreatorUser.IsDeleted
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
                    Percent = sumtotalOverviewHiring > percentDefault ? totalOverviewHiring[cvSourceStatistics.IndexOf(s)] / (float)sumtotalOverviewHiring : percentDefault,
                }).ToList();
                pieChartCvSource.ModelCharts.Add(new ModelChart
                {
                    BranchName = branch.DisplayName,
                    Temaplates = templatesCvSource ?? new List<Templates>() { new Templates { Key = noData, Percent = percentDefault } },
                });

                var statusStatistics = subPositionStatistics?.StatusStatistics.ToList();
                var totalStatus = overviewHiring.TotalOverviewHiring.TotalStatuses.Select(t => t.TotalCV).ToList();
                var sumtotalStatus = totalStatus.Sum();

                var templatesStatistics = statusStatistics?.Select(s => new Templates
                {
                    Key = s.Name,
                    Percent = sumtotalStatus > percentDefault ? (totalStatus[statusStatistics.IndexOf(s)] / (float)sumtotalStatus) : percentDefault
                }).ToList();

                pieChartStatusStatistics.ModelCharts.Add(new ModelChart
                {
                    BranchName = branch.DisplayName,
                    Temaplates = templatesStatistics ?? new List<Templates>() { new Templates { Key = noData, Percent = percentDefault } },
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
            var noData = "NoData";
            var percentDefault = 0;
            var columChartOnbore = new Chart();
            var columChartPassTest = new Chart();
            var columChartPassInterView = new Chart();

            columChartOnbore.ModelCharts = new List<ModelChart>();
            columChartPassTest.ModelCharts = new List<ModelChart>();
            columChartPassInterView.ModelCharts = new List<ModelChart>();

            foreach (var branch in input.Branchs)
            {
                var listEducationInternOnboarded = await GetEducationInternOnboarded(input.FromDate.Value, input.ToDate.Value, branch.Id);
                var educationInternOnboardeds = listEducationInternOnboarded?.Educations.ToList();
                var top10EducationInternOnboardeds = educationInternOnboardeds
                  .OrderByDescending(s => s.TotalCV)
                  .Take(10)
                  .ToList();
                var sumtotalEducationInternOnboarded = educationInternOnboardeds.Select(s => s.TotalCV).ToList().Sum();
                var pieCharts = top10EducationInternOnboardeds?.Select(s => new Templates
                {
                    Key = s.EducationName,
                    Percent = sumtotalEducationInternOnboarded > percentDefault ? (s.TotalCV / (float)sumtotalEducationInternOnboarded) : percentDefault,
                }).ToList();
                columChartOnbore.ModelCharts.Add(new ModelChart
                {
                    BranchName = branch.DisplayName,
                    Temaplates = pieCharts.Count() <= 0  ? new List<Templates>() { new Templates { Key = noData, Percent = percentDefault } }: pieCharts,
                });
                var listGetEducationPassTest = await GetEducationPassTest(input.FromDate.Value, input.ToDate.Value, branch.Id);
                var educationPassTests = listGetEducationPassTest?.Educations.ToList();
                var top10educationPassTests = educationPassTests
               .OrderByDescending(s => s.TotalCV)
               .Take(10)
               .ToList();
                var sumtotaleducationPassTests = educationPassTests.Select(s => s.TotalCV).ToList().Sum();
                var columnCharts = top10educationPassTests?.Select(s => new Templates
                {
                    Key = s.EducationName,
                    Percent = sumtotaleducationPassTests > percentDefault ? (s.TotalCV / (float)sumtotaleducationPassTests) : percentDefault,
                }).ToList();
                columChartPassTest.ModelCharts.Add(new ModelChart
                {
                     BranchName = branch.DisplayName,
                     Temaplates = columnCharts.Count() <= 0 ? new List<Templates>() { new Templates { Key = noData, Percent = percentDefault } } : columnCharts,
                });

                var listGetEducationPassInterView = await GetEducationPassInterView(input.FromDate.Value, input.ToDate.Value, branch.Id);
                var educationPassInterView = listGetEducationPassInterView?.Educations.ToList();
                var sumtotaleducationPassInterView = educationPassInterView.Select(s => s.TotalCV).ToList().Sum();

                var pieChartPassInterViews = educationPassInterView?.Select(s => new Templates
                {
                    Key = s.EducationName,
                    Percent = sumtotaleducationPassInterView > percentDefault ? (s.TotalCV / (float)sumtotaleducationPassInterView) : percentDefault,
                }).ToList();
                columChartPassInterView.ModelCharts.Add(new ModelChart
                {
                    BranchName = branch.DisplayName,
                    Temaplates = pieChartPassInterViews.Count() <= 0 ? new List<Templates>() { new Templates { Key = noData, Percent = percentDefault } } : pieChartPassInterViews,
                });
            }
            columChartOnbore.NameSheet = "Education Intern Onboarded";
            var excelBytesEducationInternOnboarded = await AddChart(columChartOnbore, ChartType.Column);
            columChartPassTest.NameSheet = "Education Pass Test";
            var excelBytesEducationPassTests = await AddChart(columChartPassTest, ChartType.Column);
            columChartPassInterView.NameSheet = "Education Intern PassIterview";
            var excelBytesEducationPassInreView = await AddChart(columChartPassInterView, ChartType.Column);

            var combinedBytes = CombineExcelFiles(excelBytesEducationInternOnboarded, excelBytesEducationPassTests, excelBytesEducationPassInreView);
            return new FileContentResult(combinedBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "exported_data.xlsx"
            };
        }
        private async Task<byte[]> AddChart(Chart input, ChartType typeChart)
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
                    worksheet.Cells[$"{columnKey}{startRow}"].LoadFromCollection(item?.Temaplates, true, TableStyles.Light9);
                    worksheet.Cells.AutoFitColumns();
                    worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    endRow = (startRow + item.Temaplates.Count);
                    worksheet.Cells[$"{columnValue}{startRow + 1}:{columnValue}{endRow}"].Style.Numberformat.Format = "0.00%";
                    var labelsRange = worksheet.Cells[$"{columnKey}{startRow + 1}:{columnKey}{endRow}"];
                    var chartRange = worksheet.Cells[$"{columnValue}{startRow + 1}:{columnValue}{endRow}"];
                    input.Row = startRow - 1;
                    input.Column = endColumn + 2;
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
    }
}

using Abp.Collections.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
        public ReportManager() { }
        public async Task<OverviewHiringDto> GetOverviewHiring(DateTime fd, DateTime td, UserType? userType, long? branchId)
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
            
            var cvStatistics = await GetDicSubPositionIdToStatisticQuantityCV(fd,td,userType,branchId);

            var cvsources = await WorkScope.GetAll<CVSource>().OrderBy(s => s.Name).ToListAsync();

            var qstatisticBySubPostion = IQStatisticRequestCVHistory(fd, td, userType, branchId, cvsources);

            var statisticBySubPosition = qstatisticBySubPostion.ToDictionary
                (
                    x => x.SubPositionId,
                    x => new { x.CVSourceStatistics, x.StatusStatistics}
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

                positionStatistic.CVSourceStatistics = statisticBySubPosition.ContainsKey(subPositionId) ?
                                                        statisticBySubPosition[subPositionId].CVSourceStatistics :
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
        private IEnumerable<StatisticRequestCVHistory> IQStatisticRequestCVHistory(
            DateTime fd, 
            DateTime td, 
            UserType? userType, 
            long? branchId, 
            List<CVSource> cvsources)
        {
            Expression<Func<RequestCVStatusHistory, bool>> predic = q => (q.TimeAt.Date >= fd.Date && q.TimeAt.Date <= td.Date) &&
                                                                         (userType.HasValue ? q.RequestCV.CV.UserType == userType : true) &&
                                                                         (!q.RequestCV.IsDeleted && !q.RequestCV.Request.IsDeleted && !q.RequestCV.CV.IsDeleted) &&
                                                                         (branchId.HasValue ? q.RequestCV.CV.BranchId == branchId.Value : true);

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
                s.RequestCV.CV.CVSourceId
            })
            .ToList();
            var qstatisticByPostion = from r in requestCVStatusHistory
                                      group r by r.SubPositionId into gr
                                      select new StatisticRequestCVHistory
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
                                                                    TotalCV = lk.Where(s => s != null).Select(s => s.CVId).Distinct().Count()
                                                                }).ToList(),

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
            long? branchId)
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
                                                                             (q.Status == RequestCVStatus.PassedTest) && 
                                                                             (q.RequestCV.CV.CVSource.ReferenceType == CVSourceReferenceType.Education);

            var educationHaveCVPassTest = WorkScope.GetAll<RequestCVStatusHistory>()
                .Where(expression)
                .Select(s => new { s.RequestCV.CV.ReferenceId , s.RequestCV.CVId})
                .AsNoTracking()
                .AsEnumerable()
                .DistinctBy(s => s.CVId)
                .GroupBy(s => s.ReferenceId)
                .Select(s => new
                {
                    s.Key,
                    TotalCV = s.Count()
                })
                .ToDictionary(s => s.Key);

            var educationIdsHaveCVPassTest = educationHaveCVPassTest.Keys.ToList();

            var educations = WorkScope.GetAll<Education>()
                .Where(s => educationIdsHaveCVPassTest.Contains(s.Id))
                .Select(e => new { e.Name, e.Id, e.ColorCode })
                .AsNoTracking()
                .AsEnumerable();

            var report = educations
                .Select(e => new ReportEducationHaveCVPassTestDto
                {
                    EducationId = e.Id,
                    EducationName = e.Name,
                    ColorCode = e.ColorCode,
                    TotalCV = educationHaveCVPassTest[e.Id].TotalCV
                })
                .ToList();

            var result = new ReportEducationByBranchDto<ReportEducationHaveCVPassTestDto>();
            result.Educations = report;
            await GetBranchInfo(result,branchId);

            return result;
        }
        public async Task<ReportEducationByBranchDto<ReportEducationHaveCVOnboardDto>> GetEducationInternOnboarded(DateTime fd, DateTime td, long? branchId)
        {
            Expression<Func<RequestCV, bool>> expression = q => (q.OnboardDate.HasValue ? (q.OnboardDate.Value.Date >= fd.Date && q.OnboardDate.Value.Date <= td.Date) : false) &&
                                                                (q.Request.UserType == UserType.Intern) &&
                                                                (!q.Request.IsDeleted && !q.CV.IsDeleted) &&
                                                                (branchId.HasValue ? q.Request.BranchId == branchId.Value : true) &&
                                                                (q.Status == RequestCVStatus.Onboarded) &&
                                                                (q.CV.CVSource.ReferenceType == CVSourceReferenceType.Education);

            var educationHaveCVOnboarded = WorkScope.GetAll<RequestCV>()
                .Where(expression)
                .Select(s => new { s.CV.ReferenceId, s.CVId })
                .AsNoTracking()
                .AsEnumerable()
                .DistinctBy(s => s.CVId)
                .GroupBy(s => s.ReferenceId)
                .Select(s => new
                {
                    s.Key,
                    TotalCV = s.Count()
                })
                .ToDictionary(s => s.Key);

            var educationIdsaveCVOnboarded = educationHaveCVOnboarded.Keys.ToList();

            var educations = WorkScope.GetAll<Education>()
                .Where(s => educationIdsaveCVOnboarded.Contains(s.Id))
                .Select(e => new { e.Name, e.Id, e.ColorCode })
                .AsNoTracking()
                .AsEnumerable();

            var report = educations
                .Select(e => new ReportEducationHaveCVOnboardDto
                {
                    EducationId = e.Id,
                    EducationName = e.Name,
                    ColorCode = e.ColorCode,
                    TotalCV = educationHaveCVOnboarded[e.Id].TotalCV
                })
                .ToList();

            var result = new ReportEducationByBranchDto<ReportEducationHaveCVOnboardDto>();
            result.Educations = report;
            await GetBranchInfo(result, branchId);

            return result;
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
                var overviewHiring = await GetOverviewHiring(input.FromDate.Value, input.ToDate.Value, input.userType, branch.id);
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
                    BranchName = branch.displayName,
                    Temaplates = templatesCvSource ?? new List<Templates>() { new Templates { Key = noData, Percent = percentDefault } },
                });

                var statusStatistics = subPositionStatistics?.StatusStatistics.ToList();
                var totalStatus = overviewHiring.TotalOverviewHiring.TotalCVSources;
                var sumtotalStatus = totalStatus.Sum();
                var templatesStatistics = statusStatistics?.Select(s => new Templates
                {
                    Key = s.Name,
                    Percent = sumtotalStatus > percentDefault ? (totalStatus[statusStatistics.IndexOf(s)] / (float)sumtotalStatus) : percentDefault
                }).ToList();
                pieChartStatusStatistics.ModelCharts.Add(new ModelChart
                {
                    BranchName = branch.displayName,
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
            var pieChart = new Chart();
            var columnChart = new Chart();
            pieChart.ModelCharts = new List<ModelChart>();
            columnChart.ModelCharts = new List<ModelChart>();
            foreach (var branch in input.Branchs)
            {
                var listEducationInternOnboarded = await GetEducationInternOnboarded(input.FromDate.Value, input.ToDate.Value, branch.id);
                var educationInternOnboardeds = listEducationInternOnboarded?.Educations.ToList();
                var sumtotalEducationInternOnboarded = educationInternOnboardeds.Select(s => s.TotalCV).ToList().Sum();
                var pieCharts = educationInternOnboardeds?.Select(s => new Templates
                {
                    Key = s.EducationName,
                    Percent = sumtotalEducationInternOnboarded > percentDefault ? (s.TotalCV / (float)sumtotalEducationInternOnboarded) : percentDefault,
                }).ToList();
                pieChart.ModelCharts.Add(new ModelChart
                {
                    BranchName = branch.displayName,
                    Temaplates = pieCharts.Count() <= 0  ? new List<Templates>() { new Templates { Key = noData, Percent = percentDefault } }: pieCharts,
                });
                var listGetEducationPassTest = await GetEducationPassTest(input.FromDate.Value, input.ToDate.Value, branch.id);
                var educationPassTests = listGetEducationPassTest?.Educations.ToList();
                var sumtotaleducationPassTests = educationPassTests.Select(s => s.TotalCV).ToList().Sum();
                var columnCharts = educationPassTests?.Select(s => new Templates
                {
                    Key = s.EducationName,
                    Percent = sumtotaleducationPassTests > percentDefault ? (s.TotalCV / (float)sumtotaleducationPassTests) : percentDefault,
                }).ToList();
                columnChart.ModelCharts.Add(new ModelChart
                {
                     BranchName = branch.displayName,
                     Temaplates = columnCharts.Count() <= 0 ? new List<Templates>() { new Templates { Key = noData, Percent = percentDefault } } : columnCharts,
                });
            }
            pieChart.NameSheet = "Education Intern Onboarded";
            var excelBytesEducationInternOnboarded = await AddChart(pieChart, ChartType.Pie);
            columnChart.NameSheet = "Education Pass Test";
            var excelBytesEducationPassTests = await AddChart(columnChart, ChartType.Column);
            var combinedBytes = CombineExcelFiles(excelBytesEducationInternOnboarded, excelBytesEducationPassTests);
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
        public byte[] CombineExcelFiles(byte[] excelBytes1, byte[] excelBytes2)
        {
            using (var package1 = new ExcelPackage(new MemoryStream(excelBytes1)))
            using (var package2 = new ExcelPackage(new MemoryStream(excelBytes2)))
            {
                var sheetsFromPackage2 = package2.Workbook.Worksheets.ToList();
                var combinedPackage = new ExcelPackage();
                foreach (var sheet in package1.Workbook.Worksheets)
                {
                    var newSheet = combinedPackage.Workbook.Worksheets.Add(sheet.Name, sheet);
                }
                foreach (var sheet in sheetsFromPackage2)
                {
                    var newSheet = combinedPackage.Workbook.Worksheets.Add(sheet.Name, sheet);
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

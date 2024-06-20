using System;
using System.Collections.Generic;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Candidates.Dtos;

namespace TalentV2.DomainServices.Reports.Dtos
{
    public class OverviewHiringDto
    {
        public long? BranchId { get; set; }
        public string BranchName { get; set; }
        public List<SubPositionStatistic> SubPositionStatistics { get; set; }
        public TotalOverviewHiring TotalOverviewHiring { get; set; }
    }

    public class RecruitmentOverviewRequestDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public UserType? UserType { get; set; }
        public bool IsGetAllBranch { get; set; }
        public List<long> BranchIds { get; set; }
        public long? UserId { get; set; }
    }

    public class RecruitmentOverviewResponseDto
    {
        public long? BranchId { get; set; }
        public string BranchName { get; set; }
        public List<SubPositionStatisticV2> SubPositionStatistics { get; set; }
        public TotalStatistics Total { get; set; }
    }

    public class TotalStatistics
    {
        public long RequestQuantity { get; set; }
        public long ApplyQuantity { get; set; }
        public List<CVStatusStatistic> CVStatusStatistics { get; set; }
        public List<CVSourceStatisticV2> CVSourceStatistics { get; set; }
        public List<RequestCVStatusStatistic> CandidateStatusStatistics { get; set; }
    }

    public class SubPositionStatisticV2
    {
        public long SubPositionId { get; set; }
        public string SubPositionName { get; set; }
        public int RequestQuantity { get; set; }
        public int ApplyQuantity { get; set; }
        public List<CVStatusStatistic> CVStatusStatistics { get; set; }
        public List<CVSourceStatisticV2> CVSourceStatistics { get; set; }
        public List<RequestCVStatusStatistic> CandidateStatusStatistics { get; set; }
    }

    public class CVStatusStatistic
    {
        public CVStatus Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }

    public class CVSourceStatisticV2
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }

    public class RequestCVStatusStatistic
    {
        public RequestCVStatus Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }

    public class RequestStatistic
    {
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public long SubPositionId { get; set; }
        public string SubPositionName { get; set; }
        public int Quantity { get; set; }
    }

    public class SubPositionStatistic
    {
        public long SubPositionId { get; set; }
        public string SubPositionName { get; set; }
        public long QuantityHiring { get; set; }
        public long QuantityApply { get; set; }
        public long QuantityNewCV { get; set; }
        public long QuantityContacting { get; set; }
        public long QuantityPassed { get; set; }
        public long QuantityFailed { get; set; }
        public List<CVSourceStatistic> CVSourceStatistics { get; set; }
        public List<StatusStatistic> StatusStatistics { get; set; }
    }
    public class CVSourceStatistic
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long TotalCV { get; set; }
    }
    public class StatusStatistic
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long TotalCV { get; set; }
    }


    public class Chart
    {
        public int PixelWidth { get; set; }
        public int PixelHeight { get; set; }
        public string Color { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int RowOffsetPixels { get; set; }
        public int ColumnOffsetPixels { get; set; }
        public string NameSheet { get; set; }
        public List<ModelChart> ModelCharts { get; set; }

    }

    public class ModelChart
    {
        public string BranchName { get; set; }
        public List<Templates> Temaplates { get; set; }
    }

    public class Templates
    {
        public string Key { get; set; }
        public float Percent { get; set; }
    }
    public class BranchDtoExport
    {
        public long? Id { get; set; }
        public string DisplayName { get; set; }
    }

    public class ExportChartInput : ExportInput
    {
        public List<BranchDtoExport> Branchs { get; set; }
        public long? userId {  get; set; }
    }

    public class ExportChartEducationInput : DateInput
    {
        public List<BranchDtoExport> Branchs { get; set; }
    }

}

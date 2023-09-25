using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.DomainServices.Reports.Dtos
{
    public class OverviewHiringDto
    {
        public long? BranchId { get; set; }
        public string BranchName { get; set; }
        public List<SubPositionStatistic> SubPositionStatistics { get; set; }
        public TotalOverviewHiring TotalOverviewHiring { get; set; }
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
    public class DateInput
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
    public class Templates
    {
        public string Key { get; set; }
        public float Percent { get; set; }
    }
    public class ModelChart
    {
        public string BranchName { get; set; }
        public List<Templates> Temaplates { get; set; }
    }
    public class ExportInput : DateInput
    {
        public UserType userType { get; set; }
    }
    public class BranchDtoExport
    {
        public long? id { get; set; }
        public string displayName { get; set; }
    }
    public class ExportChartInput : ExportInput
    {
        public List<BranchDtoExport> Branchs { get; set; }
    }
    public class ExportChartEducationInput : DateInput
    {
        public List<BranchDtoExport> Branchs { get; set; }
    }
}

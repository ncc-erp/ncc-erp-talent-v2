export class SourceStatistic {
    branchId: number;
    branchName: string;
    sourcePerformances: ReportPerformance[];
}
export class ReportPerformance {
    sourceName: string;
    totalCV: number;
}

export class ReportOverview {
    branchId: number;
    branchName: string;
    positionStatistics: PositionStatistic[];
    totalOverviewHiring: TotalOverviewHiring;
}

export class PositionStatistic {
    subPositionId: number;
    subPositionName: string;
    quantityHiring: number;
    quantityApply: number;
    quantityNewCV: number;
    quantityContacting: number;
    quantityFailed: number;
    quantityPassed: number;
    cvSourceStatistics: CVSourceStatistic[];
    statusStatistics: StatusStatistic[];
}

export class CVSourceStatistic {
    id: number;
    sourceName: string;
    totalCV: number;
}

export class StatusStatistic {
    id: number;
    sourceName: string;
    totalCV: number;
}

export class TotalOverviewHiring {
    totalHiring: number;
    totalApply: number;
    totalNewCV: number;
    totalContacting: number;
    totalPassed: number;
    totalFailed: number;
    totalCVSources: Array<number>;
    totalStatuses: Array<TotalStatus>;
}

export class TotalStatus {
    totalCV: number;
    id: number;
}
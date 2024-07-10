
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

export interface RecruitmentOverviewRequest {
  fromDate: Date;
  toDate: Date;
  userType: number;
  isGetAllBranch: boolean;
  branchIds: number[];
  userId: number;
}

export interface RecruitmentOverview {
  branchId: number;
  branchName: string;
  subPositionStatistics: SubPositionStatistic[];
  total: TotalStatistics[];
}

export interface SubPositionStatistic {
  subPositionId: number;
  subPositionName: string;
  requestQuantity: number;
  applyQuantity: number;
  cvStatusStatistics: OverviewStatistic[];
  cvSourceStatistics: OverviewStatistic[];
  candidateStatusStatistics: OverviewStatistic[];
}

export interface TotalStatistics {
  requestQuantity: number;
  applyQuantity: number;
  cvStatusStatistics: OverviewStatistic[];
  cvSourceStatistics: OverviewStatistic[];
  candidateStatusStatistics: OverviewStatistic[];
}

export interface OverviewStatistic {
  id: number;
  name: string;
  quantity: number;
  unprocessedQuantity?: number;
  normalQuantity?: number;
}
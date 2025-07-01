export interface EducationData {
  totalCV?: number;
  passCV: number;
  passTest: number;
  passInterview: number;
  onboard: number;
  other: number;
  educationId: number;
  educationName: string;
  colorCode: string;
}

export interface BranchEducationData {
  branchId: number;
  branchName: string;
  educations: EducationData[];
}

export interface StageFilters {
  passCV: boolean;
  passTest: boolean;
  passInterview: boolean;
  other: boolean;
  onboard: boolean;
}
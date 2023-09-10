
export class EducationStatistic {
  branchId: number;
  branchName: string;
  educations: EducationReport[];
}

class EducationReport {
  educationId: number;
  educationName: string;
  colorCode: string;
  totalCV: number;
}
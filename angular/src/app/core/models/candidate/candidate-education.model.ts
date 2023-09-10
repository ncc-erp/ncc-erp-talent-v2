export class CandidateEducationPayload {
  id: number;
  cvId: number;
  educationId: number;
}

export class CandidateEducation {
  id: number;
  cvId: number;
  creatorUserId: number;
  lastModifiedTime: string;
  lastModifiedName: string;
  creatorName: string;
  creationTime: string;
  updatedName: string;
  updatedTime: string;
  educationId: number;
  educationName: string;
  educationTypeId: number;
  educationTypeName: string;
}
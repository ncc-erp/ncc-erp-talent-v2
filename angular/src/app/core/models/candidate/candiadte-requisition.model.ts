import { MailDetail } from "@app/core/models/candidate/candidate.model";
import { LevelInfo } from "./../common/common.dto";
import { Skill } from "./../categories/skill.model";

export class CandidateRequisiton {
  id: number; //requestCvID
  cvName: string;
  currentRequisition: CurrentRequisition;
  interviewCandidate: CandidatInterviewer[];
  capabilityCandidate: CandidateCapability[];
  applicationResult: CandidateApplyResult;
  interviewLevel: CandidateInterviewLevel;
  interviewed: CandidateInterviewed;
  interviewTime: string;
  creationTime: string;
}

export class CurrentRequisition {
  id: number;
  priority: number;
  priorityName: string;
  branchName: string;
  displayBranchName: string;
  userType: number;
  userTypeName: string;
  subPositionId: number;
  subPositionName: string;
  level: number;
  levelInfo: LevelInfo;
  skills: Skill[];
  requestStatus: number;
  requestStatusName: string;
  quantity: number;
  timeNeed: string;
  createdBy: string;
  createdByName: string;
  createdDate: string;
  note: string;
  creatorUserId: number;
  lastModifiedTime: string;
  lastModifiedName: string;
  creatorName: string;
  creationTime: string;
  updatedName: string;
  updatedTime: string;
}

export class CandidatInterviewer {
  id: number;
  requestCvId: number;
  interviewId: number;
  interviewName: string;
  emailAddress: string;
}

export class CandidateCapability {
  id: number;
  requestCvId: number;
  capabilityId: number;
  capabilityName?: string;
  score: number;
  note: string;
  isEditing?: boolean;
  factor: number;
  fromType?: boolean;
}

export class CandidateApplyResult {
  status: number;
  historyStatuses: HistoryStatus[];
  historyChangeStatuses: HistoryChangeStatus[];
  applyLevel: number;
  applyLevelName: string;
  interviewLevel: number;
  interviewLevelName: string;
  finalLevel: number;
  finalLevelName: string;
  salary: number;
  onboardDate: string;
  hrNote: string;
  mailDetail: MailDetail;
  lmsInfo: string;
}

export class CandidateApplyResultPayload {
  requestCvId: number;
  status: number;
  applyLevel: number;
  finalLevel: number;
  salary: number;
  onboardDate: string;
  hrNote: string;
  lmsInfo: string;
}

export class HistoryStatus {
  status: number;
  statusName: string;
  timeAt: string;
}

export class HistoryChangeStatus {
  fromStatus: number;
  fromStatusName: string;
  toStatus: number;
  toStatusName: string;
  timeAt: string;
}

export class CandidateInterviewLevel {
  interviewLevel: number;
  interviewLevelName: string;
}

export class CandidateInterviewLevelPayload {
  requestCvId: number;
  interviewLevel: number;
}

export class CandidateInterviewedPayload {
  requestCvId: number;
  interviewed?: boolean;
}

export class CandidateInterviewed {
  interviewed?: boolean;
}

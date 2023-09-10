import { Skill } from '../categories/skill.model';
import { LevelInfo } from '../common/common.dto';
import { PagedRequestDto } from './../../../../shared/paged-listing-component-base';
import { CandidateInfo } from './candidate.model';


export interface CandidateInterview extends CandidateInfo {
  id: number;
  requestId: number;
  cvId: number;
  creatorUserId: number;
  lastModifiedTime: string;
  lastModifiedName: string;
  creatorName: string;
  creationTime: string;
  updatedName: string;
  updatedTime: string;
  fullName: string;
  pathAvatar: string;
  pathLinkCV: string;
  subPositionId: number;
  branchId: number;
  displayBranchName: string;
  skills: Skill[],
  applyTime: string;
  interviews: Interview[],
  interviewTime: string;
  requestStatus: number;
  requestStatusName: string;
  requestCVStatus: number;
  requestCVStatusName: string;
  requestSubPositionId: number;
  requestSubPositionName: string;
  requestBracnhId: number;
  requestBranchName: string;
  applyLevel: number;
  applyLevelInfo: LevelInfo;
  interviewLevel: number;
  interviewLevelInfo: LevelInfo;
  finalLevel: number;
  finalLevelInfo: LevelInfo;
  onboardDate: string;
  hrNote: string
}

export interface CandidateInterviewPayloadList extends PagedRequestDto {
  interviewerIds: number[];
  isAndCondition: boolean;
}

export interface Interview {
  id: number;
  interviewerId: number;
  interviewerName: string
}

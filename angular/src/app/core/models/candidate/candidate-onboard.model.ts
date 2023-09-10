import { Skill } from '../categories/skill.model';
import { LevelInfo } from '../common/common.dto';
import { CandidateInfo } from './candidate.model';

export class CandidateOnboard extends CandidateInfo {
  cvId: number;
  creatorUserId: number;
  lastModifiedTime: string;
  lastModifiedName: string;
  creatorName: string;
  creationTime: string;
  updatedName: string;
  updatedTime: string;
  subPositionId: number;
  subPositionName: string;
  branchId: number;
  branchName: string;
  displayBranchName: string;
  subPositionIdRequest: number;
  subPositionNameRequest: string;
  note: string;
  hrNote: string;
  requestBracnhId: number;
  requestBranchName: string;
  requestStatus: number;
  requestStatusName: string;
  requestCVStatus: number;
  requestCVStatusName: string;
  finalLevel: number;
  finalLevelName: LevelInfo;
  salary: number;
  onboardDate: string | Date;
  requestId: number;
  requestLevel: number;
  requestLevelName: LevelInfo;
  requestSkills: Skill[];
  nccEmail: string;
  userTypeName: string;
}

export class CandidateOnboardPayload {
  id: number;
  status: number;
  onboardDate: string;
  nccEmail: string;
  hrNote: string;
}
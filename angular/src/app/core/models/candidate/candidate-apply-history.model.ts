import { Skill } from './../categories/skill.model';
import { LevelInfo } from './../common/common.dto';

export class CandidateApplyHistory {
  requestId: number;
  priority: number;
  priorityName: string;
  branchId: number;
  branchName: string;
  subPositionId: number;
  subPositionName: string;
  level: number;
  levelInfo: LevelInfo;
  skills: Skill[];
  timeNeed: string;
  requestStatus: number;F
  requestStatusName: string;
  quantity: number;
  timeClose: string;
  userType: number;
  userTypeName: string;
  note: string;
  statusHistories: RequestCvHistoryStatus[];
}


export interface RequestCvHistoryStatus {
  requestCVStatus: number;
  requestCVStatusName: string;
  timeAt: string;
  creatorUserId: number;
  lastModifiedTime: string;
  lastModifiedName: string;
  creatorName: string;
  creationTime: string;
  updatedName: string;
  updatedTime:string;
}

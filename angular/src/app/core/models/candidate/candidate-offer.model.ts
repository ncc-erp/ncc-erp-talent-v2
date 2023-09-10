import { Skill } from "../categories/skill.model";
import { LevelInfo } from "../common/common.dto";
import { CandidateInfo } from "./candidate.model";

export class CandidateOffer extends CandidateInfo {
  cvId: number;
  creatorUserId: number;
  lastModifiedTime: string;
  lastModifiedName: string;
  creatorName: string;
  creationTime: string;
  updatedName: string;
  updatedTime: string;
  subPositionId: number;
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

export class CandidateOfferPayload {
  id: number;
  finalLevel: number;
  status: number;
  onboardDate: string;
  salary: number;
  hrNote: string;
}

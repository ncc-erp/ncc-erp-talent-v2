import { Skill } from '@app/core/models/categories/skill.model';
import { PagedRequestDto } from '@shared/paged-listing-component-base';
import { LevelInfo } from '../common/common.dto';
import { CandidateRequisitionInfo } from '../requisition/requisition.model';

export class CandidateInfo {
  id: number;
  fullName: string;
  phone: string;
  processCVStatus?: number;
  email: string;
  avatar: string;
  linkCV: string;
  isFemale: boolean;
  userType: number;
  userTypeName: string;
  isClone?: boolean;
  cvStatus?: number;
  cvStatusName?: string;
  branchName?: string;
  subPositionName?: string;
  cvSkills?: CVSkill[] | Skill[];
  requestId?: number;
  isProjectTool?: boolean;
}

export class Candidate extends CandidateInfo {
  id: number;
  subPositionId: number;
  branchId: number;
  displayBranchName: string;
  cvSourceId?: number;
  referenceId?: string;
  dob?: string;
  address?: string;
  note?: string;
  requisitionInfos: CandidateRequisitionInfo[];
  lastModifiedTime: string;
  lastModifiedName: string;
  creatorName: string;
  creatorUserId: number;
  creationTime: string;
  updatedName: string;
  updatedTime: string;
  mailDetail?: MailDetail;
}

export interface CandidateStaff extends Candidate { }
export interface CandidateIntern extends Candidate { }

export interface CandidatePayloadListData extends PagedRequestDto {
  skillIds?: number[];
  isAndCondition: boolean;
  fromDate?: string;
  toDate?: string;
  requestCVStatus?: number;
  fromStatus?: number;
  toStatus?: number;
  processCVStatus?: number;
}

export class CandidatePayload {
  id?: number;
  name: string;
  email: string;
  phone: string;
  userType: number;
  subPositionId: number;
  cvSourceId: number;
  branchId: number;
  nccEmail?: string;
  birthday: string;
  referenceId: string;
  isFemale: boolean;
  address: string;
  note: string;
  cvStatus: number;
}

export interface CVSkill {
  id: number;
  name: string;
  level: number;
  levelInfo: LevelInfo;
}

export interface MailStatusHistory {
  id: number;
  cvId: number;
  description: string;
  creationTime: string;
  subject: string;
  mailFuncType: number;
}

export interface MailDetail {
  isAllowSendMail: boolean;
  isSentMailStatus?: boolean;
  mailStatusHistories: MailStatusHistory[];
}
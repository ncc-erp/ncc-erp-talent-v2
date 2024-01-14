import { ActionEnum } from '@shared/AppEnums';
import { PagedRequestDto, PagedResult } from '@shared/paged-listing-component-base';
import { CandidateInfo, CandidateIntern, CandidateStaff } from '../candidate/candidate.model';
import { Skill } from '../categories/skill.model';
import { LevelInfo } from '../common/common.dto';
import { CandidatInterviewer } from './../candidate/candiadte-requisition.model';

export interface RequisitionInfo {
  id: number;
  priority?: number;
  priorityName?: string;
  branchId?: number;
  branchName: string;
  displayBranchName?: string;
  subPositionId?: number;
  subPositionName: string;
  userType: number;
  userTypeName?: string;
  level?: number
  levelInfo?: LevelInfo;
  requestStatus?: number;
  requestStatusName?: string;
  skills?: Skill[],
  isProjectTool?: boolean;
}

export interface CandidateRequisitionInfo extends RequisitionInfo {
  note: string;
  requestCVStatus: number;
  requestCVStatusName: string;
  skillRequests: Skill[];
}

export interface Requisition extends RequisitionInfo {
  timeClose: string;
  timeNeed: string | Date;
  status: number;
  statusName: string;
  quantity: number;
  quantityOnboard: number;
  quantityFail: number;
  totalCandidateApply: number;
  projectToolRequestId?: number;
  note: string;
  lastModifiedTime: string;
  lastModifiedName: string;
  creatorName: string;
  creatorUserId: number;
  creationTime: string;
  updatedName: string;
  updatedTime: string;
  reqCvs?: RequisitonCandidate[];
}

export interface RequisitionStaffCreateResponse {
  requestCV: { cvIdsSuccess: number[]; cvIdsFail: [] };
  requisition: RequisitionStaff;
  cv: CandidateStaff;
}

export interface RequisitionInternCreateResponse {
  requestCV: { cvIdsSuccess: number[]; cvIdsFail: [] };
  requisition: RequisitionIntern;
  cv: CandidateIntern;
}

export interface RequisitionStaff extends Requisition { }

export interface RequisitionIntern extends Requisition {
  cvIds?: number[];
 }

export interface RequisitionPayloadList extends PagedRequestDto {
  skillIds?: number[];
  isAndCondition: boolean;
}

export interface RequisitionCloseAndClone {
  userType: number;
  subPositionId: number;
  priority: number;
  skillIds: number[];
  branchId: number;
  quantity: number;
  note: string;
  id: number;
  candidateRequisitions: CandidateInfo[];
}

export interface PayloadRequisition {
  id?: number;
  userType: number;
  subPositionId: number;
  priority: number;
  skillIds: number[];
  branchId: number;
  quantity: number;
  note: string;
  cvIds?: number[];
  timeNeed: string;
}

export interface PayloadRequisitionStaff extends PayloadRequisition {
  level: number;
}

export class RequisitionStaffConfigDiaLog {
  requisitionStaff: RequisitionStaff;
  action: ActionEnum;
}

export class RequisitionInternConfigDiaLog {
  requisitionIntern: RequisitionIntern;
  reqCloseAndClone?: RequisitionCloseAndClone;
  action: ActionEnum;
}

export class RequisitonCandidate {
  id: number;
  requestId: number;
  cvId: number;
  fullName: string;
  phone: string;
  email: string;
  avatar: string;
  linkCV: string;
  userType: number;
  userTypeName: string;
  skills: Skill[];
  applyTime: string;
  interviews: CandidatInterviewer[];
  interviewTime: string;
  requestCVStatus: number;
  requestCVStatusName: string;
  applyLevel: number;
  applyLevelInfo: LevelInfo;
  interviewLevel: number;
  interviewLevelInfo: LevelInfo;
  finalLevel: number;
  finalLevelInfo: LevelInfo;
  onboardDate: string;
  isFemale: boolean;
  creationTime: string;
  creatorName: string;
  creatorUserId: number;
  hrNote: string;
  lastModifiedName: string;
  lastModifiedTime: string;
  updatedName: string;
  updatedTime: string;
  processCVStatus: number;
}

export class CloseCloneAllRequestInternPayload {
  listRequisitionIntern: CloseCloneAllRequestIntern[];
}

export class CloseCloneAllRequestIntern {
  requestId: number;
  quantity: number;
  note: string;
  timeNeed: string;
}

export interface RequisitionPagedResult extends PagedResult {
  totalQuantity: number;
}

export class RequisitionPayload{
  cvId: number;
  requestId: number;
  currentRequestId: number;
  presentForHr: string;
}
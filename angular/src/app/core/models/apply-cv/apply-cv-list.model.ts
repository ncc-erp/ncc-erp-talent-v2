import { PagedRequestDto } from "../../../../shared/paged-listing-component-base";

export class ApplyCv {
  id: number;
  fullName: string;
  isFemale: true;
  email: string;
  phone: string;
  positionType: string;
  jobTitle: string;
  branch: string;
  avatar: string;
  avatarLink: string;
  attachCV: string;
  attachCVLink: string;
  postId: number;
  applyCVDate: string;
  branchId: number;
  applied: boolean;
}

export interface ApplyCvPayloadListData extends PagedRequestDto {
  searchText: string;
  fromDate?: string;
  toDate?: string;
}
export class ApplyCVPayload {
  id?: number;
  name: string;
  email: string;
  phone: string;
  positionType: string;
  cvSourceId: number;
  branchId: number;
  referenceId: string;
  isFemale: boolean;
}
import { PagedRequestDto } from '@shared/paged-listing-component-base';
export class ExternalCv {
      id: number;
      name: string;
      email: string;
      phone: string;
      address: string;
      referenceName: string;
      birthday: string;
      avatarUrl: string;
      userTypeName: string;
      isFemale: true;
      linkCVUrl: string;
      nccEmail: string;
      note: string;
      cvSourceName: string;
      branchName: string;
      positionName: string;
      creationTime: string;
      metadata:string;
}
export interface ExternalCvPayloadListData extends PagedRequestDto {
  searchText: string;
}
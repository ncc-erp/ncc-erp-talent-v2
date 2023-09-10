import { ActionEnum } from '../../../../shared/AppEnums';
import { PagedRequestDto } from '@shared/paged-listing-component-base';
export class Post {
  id: number;
  postName: string;
  url: string;
  type: string;
  postCreationTime: string;
  creatorsName?: string;
  createdByUser: number;
  content: string;
  metadata?: string;
  applyNumber?: number;
  applyCvLink?: string;
}

export class PostConfigDiaLog {
  post : Post;
  action: ActionEnum;
}
export interface PostPayloadListData extends PagedRequestDto {
  fromDate?: string;
  toDate?: string;
  searchText: string;
}

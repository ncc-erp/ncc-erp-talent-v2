import { ActionEnum } from './../../../../shared/AppEnums';
import { Category } from './categories.model';

export  class Branch extends Category {
  displayName: string;
  colorCode: string;
  address: string;
}

export class BranchConfigDiaLog {
  branch: Branch;
  action: ActionEnum;
}
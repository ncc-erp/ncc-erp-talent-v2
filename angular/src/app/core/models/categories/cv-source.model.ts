import { ActionEnum } from './../../../../shared/AppEnums';
import { Category } from './categories.model';

export  class CVSource extends Category {
  referenceType: number;
  referenceTypeName?: string;
  colorCode: string;
}

export class CVSourceConfigDiaLog {
  cvSource: CVSource;
  action: ActionEnum;
}
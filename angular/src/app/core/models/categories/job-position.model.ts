import { ActionEnum } from '../../../../shared/AppEnums';
import { Category } from './categories.model';

export  class JobPosition extends Category {
  colorCode?: string;
}

export class JobPositionConfigDiaLog {
  jobPosition: JobPosition;
  action: ActionEnum;
  showButtonSave: boolean;
}
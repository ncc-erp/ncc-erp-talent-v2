import { ActionEnum } from '../../../../shared/AppEnums';
import { Category } from './categories.model';

export class SubPosition extends Category {
  colorCode: string;
  positionId: number;
  positionName?: string
}

export class SubPositionConfigDiaLog {
  subPosition: SubPosition;
  action: ActionEnum;
}
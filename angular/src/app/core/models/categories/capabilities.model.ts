import { ActionEnum } from './../../../../shared/AppEnums';
import { Category } from './categories.model';

export  class Capability extends Category {
  note: string;
  fromType: boolean;
}

export class CapabilityConfigDiaLog {
  capability: Capability;
  action: ActionEnum;
}
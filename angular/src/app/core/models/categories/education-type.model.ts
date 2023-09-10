import { ActionEnum } from './../../../../shared/AppEnums';
import { Category } from './categories.model';

export  class EducationType extends Category {}

export class EducationTypeConfigDiaLog {
  educationType: EducationType;
  action: ActionEnum;
}
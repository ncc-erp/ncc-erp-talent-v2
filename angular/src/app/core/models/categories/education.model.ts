import { ActionEnum } from './../../../../shared/AppEnums';
import { Category } from './categories.model';

export  class Education extends Category {
  educationTypeId: number;
  educationTypeName?: string;
  colorCode: string;
}

export class EducationConfigDiaLog {
  education: Education;
  action: ActionEnum;
}
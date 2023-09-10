
import { ActionEnum } from './../../../../shared/AppEnums';

export class EmployeePosition {
  id: number;
  name: string;
  description: string;
}

export class EmployeePositionConfigDiaLog {
  employeePosition: EmployeePosition;
  action: ActionEnum;
}
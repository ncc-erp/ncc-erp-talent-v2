
import { ActionEnum } from '../../../../shared/AppEnums';

export class GroupSkill {
  id: number;
  name: string;
  default?: boolean;
}

export class GroupSkillConfigDiaLog {
  groupSkill: GroupSkill;
  action: ActionEnum;
  showButtonSave: boolean;
}
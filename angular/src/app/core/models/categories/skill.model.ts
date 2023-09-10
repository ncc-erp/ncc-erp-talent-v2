import { ActionEnum } from '../../../../shared/AppEnums';
import { Category } from './categories.model';

export class Skill extends Category {}

export class SkillConfigDiaLog {
  skill: Skill;
  action: ActionEnum;
}
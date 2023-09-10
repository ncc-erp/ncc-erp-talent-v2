
import { ActionEnum } from '../../../../shared/AppEnums';

export class FakeSkill {
  id: number;
  name: string;
  description: string;
  groupSkillId: number;
  groupSkilName?: string;
}

export class FakeSkillConfigDiaLog {
  fakeSkill: FakeSkill;
  action: ActionEnum;
}
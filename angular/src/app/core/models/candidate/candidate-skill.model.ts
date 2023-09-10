import { LevelInfo } from '../common/common.dto';

export class CandidateSkillPayload {
  id: number;
  cvId: number;
  skillId: number;
  level: number;
  note: string;
}

export class CandidateSkill {
  id: number;
  cvId: number;
  skillId: number;
  skillName: string;
  levelSkill: number;
  levelInfo: LevelInfo;
  note: string
  creatorUserId: number;
  lastModifiedTime: string;
  lastModifiedName: string;
  creatorName: string;
  creationTime: string;
  updatedName: string;
  updatedTime: string;
}
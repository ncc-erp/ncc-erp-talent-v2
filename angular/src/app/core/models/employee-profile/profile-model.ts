import { ActionEnum } from '@shared/AppEnums';

export class ContactInformation {
  name: string;
  surname: string;
  phoneNumber: string;
  emailAddressInCV: string;
  imgPath: string;
  currentPosition?: string;
  currentPositionId: number;
  userId: any;
  address: string;
  branch?: string;
  branchId: number;
}

export class IObjectFile {
  path: string;
  file: File;
  buffer: any;
}

export class ProfileEducation {
  cvcandidateId: number;
  cvemployeeId: number;
  schoolOrCenterName: string;
  degreeType: any;
  major: string;
  startYear: Date;
  endYear: Date;
  description: string;
  order: number;
  id: number;
  versionId?: number;
}

export class ProfileEducationConfigDiaLog {
  education: ProfileEducation;
  action: ActionEnum;
  isUser: boolean;
}


//Version
export class ProfileVersion {
  id: number;
  employeeId: number;
  versionName: string;
  positionId: number;
  languageId: number;
}


//Technical
export class TechnicalExpertise {
  userId: number;
  groupSkills: GroupTechnical[];
}
export class GroupTechnical {
  cvSkills: ProfileCvSkill[];
  groupSkillId: any;
  name: string;
}

export class ProfileCvSkill {
  id: number;
  level: number;
  order: number;
  skillId: number;
  skillName: string;
}

//PersonalAttribute
export class PersonalAttributeConfigDialog {
  listPersonalAttribute: any[];
  item?: any
  isUser: boolean;
  action: ActionEnum;
}

export class SkillCandidateDto {
  groupSkillId: Number;
  id: number;
  name: string;
}

//Working experience
export class WorkingExperience {
  id: number;
  order: number;
  projectId: number;
  projectName: string;
  position: string;
  projectDescription: string;
  responsibility: string;
  startTime: string;
  endTime: string;
  userId: number;
  technologies: string;
  isChecked?: boolean;
  versionId: number;
}

export class EmployeeDto {
  userId?: number;
  name?: string;
  positionId?: string;
  branchId?: string;
  isChecked?: boolean;
}

export class PositionDto {
  id: any;
  name: string;
  description?:string
}

export class WorkingExperienceConfigDiaLog {
  workingExperience: WorkingExperience;
  isUser?: boolean;
  action: ActionEnum;
}

export class VesionDto {
  versionId: number;
  versionName: string;
  employeeId: number;
  employeeName: string;
  versionLanguageId: number;
  versionLanguage: string;
  versionPositionId: number;
  versionPosition: string;
  isChecked: boolean;
}

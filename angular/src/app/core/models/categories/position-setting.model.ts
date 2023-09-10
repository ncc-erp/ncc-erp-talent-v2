export class PositionSetting {
  id: number;
  userType: number;
  userTypeName: string;
  positionId: number;
  positionName: string;
  subPositionId: number;
  subPositionName: string;
  lmsCourseName: string;
  lmsCourseId: string;
  projectInfo: string;
  discordInfo: string;
  imsInfo: string;
}

export class PositionSettingCreate {
  userType: number;
  subPositionId: number;
}

export class PositionSettingUpdate {
  id: number;
  userType: number;
  subPositionId: number;
  lmsCourseName: string;
  lmsCourseId: string;
  projectInfo: string;
  discordInfo: string;
  imsInfo: string;
}

export class LmsCourse {
  id: string;
  name: string;
  startDate: string;
  endDate: string;
  description: string;
  relationInfo: string;
  imageCover: string;
  fullPathImageCover: string;
}
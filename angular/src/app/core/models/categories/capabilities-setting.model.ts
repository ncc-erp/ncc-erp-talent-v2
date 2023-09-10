import { ActionEnum, UserType } from '../../../../shared/AppEnums';


export class CapabilitySettingPayload {
  id: number;
  userType: number;
  subPositionId: number;
  capabilityId: number;
  factor: number;
  guideLine: string;
  isDeleted: boolean
}

export class CapabilitySettingCreateResponse extends CapabilitySettingPayload {
  userTypeName: string;
  subPositionName: string;
  capabilityName: string;
  note: string;
  factor: number;
  isDeleted: boolean
}

export class CapabilitySetting {
  userType: number;
  userTypeName: string;
  subPositionId: number;
  subPositionName: string;
  capabilities: CapabilityWithSetting[];
}

export class CapabilitySettingConfigDiaLog {
  capabilitySetting: CapabilitySetting;
  capabilityWithSetting: CapabilityWithSetting;
  action: ActionEnum;
}

export interface CapabilityWithSetting {
  id: number; //CapabilitySetting's id
  note: string;
  guideLine: string;
  factor: number;
  capabilityId: number;
  capabilityName: string;
  fromType: boolean;
}

export interface CloneCapabilitySettingConfigDiaLog {
  userTypeFrom: UserType;
  subPositionIdFrom: number;
}

export interface CapabilitySettingClone {
  fromUserType: UserType;
  fromUserTypeName?: string;
  fromSubPositionId: number;
  fromSubPositionName?: string;
  toUserType: UserType;
  toSubPositionId: number;
  toSubPositionName?: string; //toSubPositionName response
  toUserTypeName?: string; //toUserTypeName response


}

export class ConfigurationSetting {
  url: string;
  securityCode: string;
}

export class TimesheetSetting extends ConfigurationSetting {
  isAutoUpdate: boolean;
}

export class KomuSetting {
  komuSetting: string;
  isSendNotify: boolean;
  secretCode: string;
  channelIdDevMode: string;
}

export class LMSSetting extends ConfigurationSetting {
  feAddress: string;
}

export class GoogleClientAppSetting {
  googleClientAppId: string;
  googleClientAppEnable: boolean;
  enableNormalLogin: boolean;
}

export class EmailSetting {
  displayName: string;
  defaultAddress: string;
  host: string;
  port: string;
  userName: string;
  password: string;
  enableSsl: string | boolean;
  useDefaultCredentials: string;
}

export class TalentSecretCode {
  secretCode: string;
}
export class TalentContestUrl {
  contestUrl : string;
}

export class DiscordChannelSettings {
  channelHRITId: string;
  komuResourceRequestInternChannelId: string;
  komuResourceRequestStaffChannelId: string;
}
export class NoticeInterviewSettingDto {
  noticeInterviewStartAtHour: string;
  noticeInterviewEndAtHour: string;
  noticeInterviewMinutes: string;
  noticeInterviewResultMinutes: string;
  isToChannel: string;
  scheduleChannel: string;
  resultChannel: string;
}

export class GetResultConnectDto{
  isConnected: boolean;
  message: string;
}


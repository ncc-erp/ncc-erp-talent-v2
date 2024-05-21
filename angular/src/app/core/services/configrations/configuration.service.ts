import { DiscordChannelSettings, TalentSecretCode, NoticeInterviewSettingDto, TalentContestUrl } from '@app/core/models/configuration/configuration.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { nullToEmpty } from '@app/core/helpers/utils.helper';
import { Observable } from 'rxjs';
import { GetResultConnectDto, ConfigurationSetting, EmailSetting, GoogleClientAppSetting, KomuSetting, LMSSetting, TimesheetSetting } from '../../models/configuration/configuration.model';
import { BaseApiService } from '../apis/base-api.service';
import { ApiResponse } from './../../../../shared/paged-listing-component-base';

@Injectable({
  providedIn: 'root'
})
export class ConfigurationService extends BaseApiService {

  changeUrl(): string {
    return 'Configuration';
  }

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  getKomuSetting(): Observable<ApiResponse<KomuSetting>> {
    return this.get("/GetKomuSettings");
  }

  getDiscordChanelSetting(): Observable<ApiResponse<DiscordChannelSettings>> {
    return this.get("/GetDiscordChannelHRIT");
  }

  getHRMSettings(): Observable<ApiResponse<ConfigurationSetting>> {
    return this.get("/GetHRMSettings");
  }

  getAutoBotSettings(): Observable<ApiResponse<ConfigurationSetting>> {
    return this.get('/GetAutoBotSettings');
  }

  getEmailSetting(): Observable<ApiResponse<EmailSetting>> {
    return this.get("/getEmailSetting");
  }

  getLMSSetting(): Observable<ApiResponse<LMSSetting>> {
    return this.get("/GetLMSSettings");
  }

  getGoogleClientAppSettings(): Observable<ApiResponse<GoogleClientAppSetting>> {
    return this.get("/GetGoogleClientAppSettings");
  }

  getTalentSecretCode(): Observable<ApiResponse<TalentSecretCode>> {
    return this.get("/GetTalentSecretCode");
  }
  getTimeNoticeInterviewer(): Observable<ApiResponse<NoticeInterviewSettingDto>> {
    return this.get("/GetNoticeInterviewSetting");
  }

  setKomuSetting(isSendNotify: boolean): Observable<ApiResponse<string>> {
    return this.create({ isSendNotify: isSendNotify }, '/SetKomuSettings');
  }

  setDiscordChanelSetting(discordChannel: DiscordChannelSettings): Observable<ApiResponse<{ channelHRITId: string }>> {
    return this.create(discordChannel, '/SetDiscordChannelHRIT');
  }

  setHRMSetting(payload: ConfigurationSetting): Observable<ApiResponse<ConfigurationSetting>> {
    return this.create(payload, `/SetHRMSettings`);
  }

  setTimesheetSetting(payload: TimesheetSetting): Observable<ApiResponse<TimesheetSetting>> {
    return this.create(payload, `/SetTimesheetSettings`);
  }

  setEmailsetting(payload: EmailSetting): Observable<ApiResponse<EmailSetting>> {
    return this.create(payload, `/SetEmailSetting`);
  }

  setGoogleClientAppSettings(googleClientApp: GoogleClientAppSetting): Observable<ApiResponse<GoogleClientAppSetting>> {
    return this.create(googleClientApp, `/SetGoogleClientAppSettings`);
  }

  setTalentSecretCode(payload: TalentSecretCode): Observable<ApiResponse<TalentSecretCode>> {
    return this.create(payload, `/SetTalentSecretCode`);
  }
  setTimeNoticeInterviewer(payload: NoticeInterviewSettingDto): Observable<ApiResponse<NoticeInterviewSettingDto>> {
    return this.create(payload, `/SetNoticeInterviewSetting`);
  }
  testLMSConnection(): Observable<ApiResponse<GetResultConnectDto>> {
    return this.get(`/CheckConnectToLMS`);
  }
  testHRMConnection(): Observable<ApiResponse<GetResultConnectDto>> {
    return this.get(`/CheckConnectToHRM`);
  }
  getContestUrl(): Observable<ApiResponse<TalentContestUrl>> {
    return this.get("/GetContestUrl");
  }
}

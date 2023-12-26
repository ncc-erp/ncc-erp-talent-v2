import { Component, Injector, OnInit } from '@angular/core';
import { BreadCrumbConfig } from '@app/core/models/common/common.dto';
import { GetResultConnectDto ,ConfigurationSetting, DiscordChannelSettings, NoticeInterviewSettingDto, EmailSetting, GoogleClientAppSetting, KomuSetting, LMSSetting, TalentSecretCode, TalentContestUrl } from '@app/core/models/configuration/configuration.model';
import { MESSAGE } from '@shared/AppConsts';
import { DefaultRoute, ToastMessageType } from '@shared/AppEnums';
import { NccAppComponentBase } from '@shared/ncc-component-base';
import * as _ from 'lodash';
import { ConfigurationService } from './../../../core/services/configrations/configuration.service';


enum SETTING_TYPE {
  KOMU = 'komu',
  DISCORD_CHANEL = 'discordChanel',
  HRM = 'hrm',
  EMAIL = 'email',
  GOOGLE_LOGIN = 'google',
  TALENT = 'talent',
  NOTIFYINTERVIEWER = 'notifyinterviewer',
  CONTEST = 'contest',
}
@Component({
  selector: 'talent-configurations',
  templateUrl: './configurations.component.html',
  styleUrls: ['./configurations.component.scss']
})
export class ConfigurationsComponent extends NccAppComponentBase implements OnInit {
  readonly SETTING_TYPE = SETTING_TYPE;

  komuSetting: KomuSetting;
  discordChanelSetting = {} as DiscordChannelSettings;
  hrmSetting: ConfigurationSetting;
  googleClientAppSetting: GoogleClientAppSetting;
  emailSetting: EmailSetting;
  lmsSetting: LMSSetting;
  talentSecretCode: TalentSecretCode;
  talentContestUrl: TalentContestUrl;
  noticeTimerInterviewer: NoticeInterviewSettingDto;
  public hrmResult: GetResultConnectDto = {} as GetResultConnectDto;
  public lmsResult: GetResultConnectDto = {} as GetResultConnectDto;
  isEditing = {
    komu: false,
    discordChanel: false,
    hrm: false,
    lms: false,
    email: false,
    google: false,
    talent: false,
    notifyinterviewer: false,
    contest: false
  }

  originalData = {
    komu: null,
    discordChanel: null,
    hrm: null,
    lms: null,
    email: null,
    google: null,
    talent: null,
    notifyinterviewer: null,
    contest: null
  }

  public isPanelCollapse = {
    komuSetting: false,
    discordChanel: false,
    hrmSetting: false,
    lmsSetting: false,
    emailSetting: false,
    googleClientAppSetting: false,
    talentSecretCode: false,
    talentContestUrl: false,
    noticeTimerInterviewer: false
  }
  constructor(
    injector: Injector,
    private _configuration: ConfigurationService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
    this.testHRMConnection();
    this.testLMSConnection();
  }

  toggleEditing(settingType: SETTING_TYPE) {
    this.isEditing[settingType] = !this.isEditing[settingType];
  }

  cancelEditing(settingType: SETTING_TYPE) {
    this.toggleEditing(settingType);
    switch (settingType) {
      case SETTING_TYPE.KOMU:
        return this.komuSetting = { ...this.originalData[settingType] };
      case SETTING_TYPE.DISCORD_CHANEL:
        return this.discordChanelSetting = this.originalData[settingType];
      case SETTING_TYPE.HRM:
        return this.hrmSetting = { ...this.originalData[settingType] };
      case SETTING_TYPE.EMAIL:
        return this.emailSetting = { ...this.originalData[settingType] };
      case SETTING_TYPE.GOOGLE_LOGIN:
        return this.googleClientAppSetting = { ...this.originalData[settingType] };
      case SETTING_TYPE.TALENT:
        return this.talentSecretCode = { ...this.originalData[settingType] };
      case SETTING_TYPE.NOTIFYINTERVIEWER:
        return this.noticeTimerInterviewer = { ...this.originalData[settingType] };
      case SETTING_TYPE.CONTEST:
        return this.talentContestUrl = { ...this.originalData[settingType] };
      default: return;
    }
  }

  saveKomuSetting() {
    this.subs.add(
      this._configuration.setKomuSetting(this.komuSetting.isSendNotify).subscribe(res => {
        this.isLoading = res.loading;
        if (res.success) {
          this.originalData.komu = _.cloneDeep(res.result);
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, 'Komu Setting');
          this.toggleEditing(SETTING_TYPE.KOMU);
        }
      })
    );
  }

  saveDisChanelSetting() {
    this.subs.add(
      this._configuration.setDiscordChanelSetting(this.discordChanelSetting).subscribe(res => {
        this.isLoading = res.loading;
        if (res.success) {
          this.originalData.discordChanel = _.cloneDeep(res.result.channelHRITId);
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, 'Discord chanel setting');
          this.toggleEditing(SETTING_TYPE.DISCORD_CHANEL);
        }
      })
    );
  }

  saveHrmSetting() {
    this.subs.add(
      this._configuration.setHRMSetting(this.hrmSetting).subscribe(res => {
        this.isLoading = res.loading;
        if (res.success) {
          this.originalData.hrm = _.cloneDeep(res.result);
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, 'HRM Setting');
          this.toggleEditing(SETTING_TYPE.HRM);
        }
      })
    );
  }

  saveGoogleLoginSetting() {
    this.subs.add(
      this._configuration.setGoogleClientAppSettings(this.googleClientAppSetting).subscribe(res => {
        this.isLoading = res.loading;
        if (res.success) {
          this.originalData.google = _.cloneDeep(res.result);
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, 'Google login Setting');
          this.toggleEditing(SETTING_TYPE.GOOGLE_LOGIN);
        }
      })
    );
  }

  saveEmailSetting() {
    this.subs.add(
      this._configuration.setEmailsetting(this.emailSetting).subscribe(res => {
        this.isLoading = res.loading;
        if (res.success) {
          this.originalData.email = _.cloneDeep(res.result);
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, 'Email Setting');
          this.toggleEditing(SETTING_TYPE.EMAIL);
        }
      })
    );
  }

  saveTalentSecretCode() {
    this.subs.add(
      this._configuration.setTalentSecretCode(this.talentSecretCode).subscribe(res => {
        this.isLoading = res.loading;
        if (res.success) {
          this.originalData.talent = _.cloneDeep(res.result);
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, 'Talent SecretCode');
          this.toggleEditing(SETTING_TYPE.TALENT);
        }
      })
    );
  }
  saveNotifyInterviewerChannelId() {
    this.subs.add(
      this._configuration.setTimeNoticeInterviewer(this.noticeTimerInterviewer).subscribe(res => {
        this.isLoading = res.loading;
        if (res.success) {
          this.originalData.notifyinterviewer = _.cloneDeep(res.result);
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, 'Timer Notify Interviewer Saved!');
          this.toggleEditing(SETTING_TYPE.NOTIFYINTERVIEWER);
        }
      })
    );
  }

  testLMSConnection() {
    this.subs.add(
      this._configuration.testLMSConnection().subscribe(value => {
        this.lmsResult = value.result;
      })
    )
  }
  testHRMConnection() {
    this.subs.add(
      this._configuration.testHRMConnection().subscribe(value => {
        this.hrmResult = value.result;
      })
    )
  }

  protected getBreadCrumbConfig(): BreadCrumbConfig {
    return {
      menuItem: [{ label: "Admin", routerLink: DefaultRoute.Admin, styleClass: 'menu-item-click' }, { label: "Configuration" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }

  private getData() {
    this.getKomuSetting();
    this.getDiscordChanelSetting();
    this.getHRMSettings();
    this.getGoogleLoginSetting();
    this.getEmailsetting();
    this.geLMSSetting();
    this.getTalentSecretCode();
    this.getNotifyInterViewerTimerSetting();
    this.getContestUrl();
  }

  private getContestUrl() {
    if (!this.isGranted(this.PS.PermissionNames_Pages_Configurations_ConfigureContestUrl)) return;
   this.subs.add(
     this._configuration.getContestUrl().subscribe(res => {
       this.isLoading = res.loading;
       if (res.success) {
         this.talentContestUrl = res.result;
         this.originalData.contest = _.cloneDeep(res.result);
       }
     })
   );
 }

  private getEmailsetting() {
    if (!this.isGranted(this.PS.Pages_Configurations_ViewEmailSettings)) return;

    this.subs.add(
      this._configuration.getEmailSetting().subscribe(res => {
        this.isLoading = res.loading;
        if (res.success) {
          this.emailSetting = res.result;
          this.originalData.email = _.cloneDeep(res.result);
        }
      })
    );
  }

  private getGoogleLoginSetting() {
    if (!this.isGranted(this.PS.Pages_Configurations_ViewGoogleClientAppSettings)) return;

    this.subs.add(
      this._configuration.getGoogleClientAppSettings().subscribe(res => {
        this.isLoading = res.loading;
        if (res.success) {
          this.googleClientAppSetting = res.result;
          this.originalData.google = _.cloneDeep(res.result);
        }
      })
    );
  }

  private geLMSSetting() {
    if (!this.isGranted(this.PS.Pages_Configurations_ViewLMSSettings)) return;

    this.subs.add(
      this._configuration.getLMSSetting().subscribe(res => {
        this.isLoading = res.loading;
        if (res.success) {
          this.lmsSetting = res.result;
        }
      })
    );
  }

  private getHRMSettings() {
    if (!this.isGranted(this.PS.Pages_Configurations_ViewHRMSettings)) return;

    this.subs.add(
      this._configuration.getHRMSettings().subscribe(res => {
        this.isLoading = res.loading;
        if (res.success) {
          this.hrmSetting = res.result;
          this.originalData.hrm = _.cloneDeep(res.result);
        }
      })
    );
  }

  private getDiscordChanelSetting() {
    if (!this.isGranted(this.PS.Pages_Configurations_ViewChannelHRITSettings)) return;

    this.subs.add(
      this._configuration.getDiscordChanelSetting().subscribe(res => {
        this.isLoading = res.loading;
        if (res.success) {
          this.discordChanelSetting = res.result;
          this.originalData.discordChanel = _.cloneDeep(res.result);
        }
      })
    );
  }

  private getKomuSetting() {
    if (!this.isGranted(this.PS.Pages_Configurations_ViewKomuSettings)) return;

    this.subs.add(
      this._configuration.getKomuSetting().subscribe(res => {
        this.isLoading = res.loading;
        if (res.success) {
          this.komuSetting = res.result;
          this.originalData.komu = _.cloneDeep(res.result);
        }
      })
    );
  }

  private getTalentSecretCode() {
    if (!this.isGranted(this.PS.Pages_Configurations_ViewTalentSecretCode)) return;

    this.subs.add(
      this._configuration.getTalentSecretCode().subscribe(res => {
        this.isLoading = res.loading;
        if (res.success) {
          this.talentSecretCode = res.result;
          this.originalData.talent = _.cloneDeep(res.result);
        }
      })
    );
  }
  private getNotifyInterViewerTimerSetting() {
    if (!this.isGranted(this.PS.Pages_Configurations_ViewTalentNotifyInterviewSettings)) return;

    this.subs.add(
      this._configuration.getTimeNoticeInterviewer().subscribe(res => {
        this.isLoading = res.loading;
        if (res.success) {
          this.noticeTimerInterviewer = res.result;
          this.originalData.notifyinterviewer = _.cloneDeep(res.result);
        }
      })
    );
  }
}

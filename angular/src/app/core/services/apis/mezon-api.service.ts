import { Injectable, InjectionToken } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { BaseApiService } from './base-api.service';
import { IHashMezonAuthModel } from 'account/login/login.service';
import { AppConsts } from '@shared/AppConsts';
import { MezonWebViewEvent, MezonAppEvent } from 'types/mezon/webview';

@Injectable({
  providedIn: 'root'
})
export class MezonLoginService extends BaseApiService {
  private userHashInfo = new Subject<any>();
  private currentUserInfo = new Subject<any>();
  private isInMezon = new Subject<boolean>();

  private eventListenersRegistered = false;

  userHashInfo$ = this.userHashInfo.asObservable();
  currentUserInfo$ = this.currentUserInfo.asObservable();
  isInMezon$ = this.isInMezon.asObservable();


  changeUrl(): string {
     return 'Mezon';
  }

  constructor(
    http: HttpClient
  ) {
    super(http);

    this.initMezonEventListeners();
  }


  public initMezonEventListeners(): void {
      if (this.eventListenersRegistered)
          return;

      this.eventListenersRegistered = true;

      if (window.Mezon && window.Mezon.WebView) {
          this.ping();
          this.sendBotId();

          this.listenToPong();
          this.listenToUserHashInfo();
          this.listenToCurrentUserInfo();
      }
  }
  
    ping() {
        window.Mezon.WebView.postEvent("PING" as MezonWebViewEvent, { message: "PING" }, () => { })
    }

    listenToPong() {
        window.Mezon.WebView.onEvent("PONG" as MezonAppEvent, () => {
            this.isInMezon.next(true);
        });
    }

    sendBotId() {
        window.Mezon.WebView.postEvent("SEND_BOT_ID" as MezonWebViewEvent, { appId: AppConsts.mezonAppId }, () => { })
    }

    listenToUserHashInfo() {
        window.Mezon.WebView.onEvent("USER_HASH_INFO" as MezonAppEvent, async (_, userHashData: any) => {
            this.userHashInfo.next(userHashData.message);
        });
    }

    listenToCurrentUserInfo() {
        window.Mezon.WebView.onEvent("CURRENT_USER_INFO" as MezonAppEvent, async (_, userData: any) => {
            if (!userData || !userData.user) {
                return;
            }
            const mezonUser = {
                email: userData.email,
                mezon_id: userData.mezon_id,
                user: {
                    avatar_url: userData.user.avatar_url,
                    display_name: userData.user.display_name,
                    id: userData.user.id,
                    username: userData.user.username,
                },
                wallet: userData.wallet,
            };
            this.currentUserInfo.next(mezonUser);
        });
    }

    removeEventListeners() {
        window.Mezon.WebView.offEvent("CURRENT_USER_INFO" as MezonAppEvent, () => { })
        window.Mezon.WebView.offEvent("USER_HASH_INFO" as MezonAppEvent, () => { })
    }
      
  redirectToOAuth() {
    window.location.href = `${this.baseUrl}/api/TokenAuth/MezonRedirect`;
  }

  mezonAuthenticate(token: string): Observable<any> {
    return this.http.post(this.baseUrl + '/api/TokenAuth/MezonAuthenticate', {token: token});
  }

  mezonHashAuthenticate(authDto: IHashMezonAuthModel): Observable<any> {
    return this.http.post(this.baseUrl + '/api/TokenAuth/MezonHashAuthenticate', authDto);
  }
}

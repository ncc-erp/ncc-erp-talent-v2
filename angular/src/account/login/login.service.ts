import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MezonLoginService } from '@app/core/services/apis/mezon-api.service';
import { HttpErrorResponse } from '@node_modules/@angular/common/http';
import { Observable, of, throwError } from '@node_modules/rxjs';
import { AppConsts } from '@shared/AppConsts';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { AuthenticateModel, AuthenticateResultModel, ExternalAuthenticateModel, ExternalAuthenticateResultModel, ExternalLoginProviderInfoModel, TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { LogService, MessageService, PermissionCheckerService, TokenService, UtilsService } from 'abp-ng2-module';
import { catchError, finalize, map, startWith } from 'rxjs/operators';
import { AppSessionService } from '@shared/session/app-session.service';

export interface IHashMezonAuthModel {
    hashData: string;
    tenancyName: string;
}

@Injectable({
    providedIn: 'root'
})
export class LoginService {

    static readonly twoFactorRememberClientTokenName = 'TwoFactorRememberClientToken';

    authenticateModel: AuthenticateModel;
    authenticateResult: AuthenticateResultModel;

    rememberMe: boolean;
    private silentAuthChecked = false;

    constructor(
        private _tokenAuthService: TokenAuthServiceProxy,
        private _router: Router,
        private _utilsService: UtilsService,
        private _tokenService: TokenService,
        private _logService: LogService,
        private _mezonService: MezonLoginService,
        private _sessionService: AppSessionService
    ) {
        this.clear();
    }

    logout(reload?: boolean): void {
        abp.auth.clearToken();
        abp.utils.deleteCookie(AppConsts.authorization.encryptedAuthTokenName);

        if (reload !== false) {
            location.href = AppConsts.appBaseUrl;
        }
    }

    authenticate(finallyCallback?: () => void): void {
        finallyCallback = finallyCallback || (() => { });

        this._tokenAuthService
            .authenticate(this.authenticateModel)
            .pipe(finalize(() => { finallyCallback(); }))
            .subscribe((result: AuthenticateResultModel) => {
                this.processAuthenticateResult(result);
            });
    }

    authenticateMezon(token: string, scope: string): Observable<any> {
        return this._mezonService.mezonAuthenticate(token).pipe(
            map(data => {
                var result = this.processAuthenticateResult(data.result);
                return { ...data, loading: false }
            }),
            startWith({ loading: true, success: false }),
            catchError((err: HttpErrorResponse) => {
                return of({ loading: false, success: false, error: err.error.error });
            }),
        );
    }

    authenticateMezonHash(authDto: IHashMezonAuthModel): Observable<any> {
        return this._mezonService.mezonHashAuthenticate(authDto).pipe(
            map(data => {
                this.processAuthenticateResult(data.result);
                return { ...data, loading: false }
            }),
            startWith({ loading: true, success: false }),
            catchError((err: HttpErrorResponse) => {
                return of({ loading: false, success: false, error: err.error.error });
            }),
        );
    }

    // authenticateGoogle(googleToken: string, finallyCallback?: () => void): void {
    //     finallyCallback = finallyCallback || (() => { });

    //     this._googleLoginService.googleAuthenticate(googleToken)
    //         .subscribe((result: any) => {
    //             this.processAuthenticateResult(result.result);
    //         }, (error) => {
    //             const errObj = error?.error?.error;
    //             this._message.error(errObj?.details, errObj?.message);
    //         });
    // }

    private async checkPopupPermission(): Promise<boolean> {
        try {
            const testPopup = window.open('', 'popup-test', 'width=1,height=1,left=-1000,top=-1000');
            if (testPopup) {
                testPopup.close();
                return true;
            }
            return false;
        } catch {
            return false;
        }
    }

    private async requestPopupPermission(): Promise<boolean> {
        return new Promise((resolve) => {
            abp.message.confirm(
                'To enable automatic login, please allow popups for this site.',
                'Enable Automatic Login',
                (result) => resolve(!!result)
            );
        });
    }

    async checkSilentAuth(): Promise<{ success: boolean; authenticated?: boolean; error?: string }> {
        try {
            // Check popup permission first
            const hasPermission = await this.checkPopupPermission();
            if (!hasPermission) {
                const userAllowed = await this.requestPopupPermission();
                if (!userAllowed) {
                    return { success: false, authenticated: false, error: 'popup_permission_denied' };
                }
            }

            return new Promise((resolve) => {
                this._mezonService.getSilenetOathUrl().subscribe({
                    next: (response) => {
                        const silentUrl = response.result;
                        const popup = window.open(
                            silentUrl,
                            'mezon-silent-auth',
                            'width=500,height=600,scrollbars=yes,resizable=yes,toolbar=no,menubar=no,location=no,directories=no,status=no'
                        );

                        if (!popup) {
                            resolve({ success: false, authenticated: false, error: 'popup_blocked' });
                            return;
                        }

                        const timeoutId = setTimeout(() => {
                            cleanup();
                            resolve({ success: false, authenticated: false, error: 'timeout' });
                        }, 30000);

                        const messageHandler = (event: MessageEvent) => {
                            if (!event.origin.includes(window.location.hostname) || 
                                !event.data?.type || event.data.type !== 'mezon_silent_auth') {
                                return;
                            }

                            cleanup();
                            if (event.data.code) {
                                this.authenticateMezon(event.data.code, 'openid offline').subscribe({
                                    next: (authResult) => resolve({
                                        success: authResult.success,
                                        authenticated: authResult.success,
                                        error: authResult.error
                                    }),
                                    error: (error) => resolve({
                                        success: false,
                                        authenticated: false,
                                        error: error.message
                                    })
                                });
                            } else {
                                resolve({ success: false, authenticated: false, error: event.data.error || 'no_code' });
                            }
                        };

                        const checkClosed = setInterval(() => {
                            if (popup.closed) {
                                cleanup();
                                resolve({ success: false, authenticated: false, error: 'popup_closed' });
                            }
                        }, 1000);

                        const cleanup = () => {
                            clearTimeout(timeoutId);
                            clearInterval(checkClosed);
                            window.removeEventListener('message', messageHandler);
                            if (popup && !popup.closed) {
                                popup.close();
                            }
                        };

                        window.addEventListener('message', messageHandler);
                    },
                    error: () => resolve({ success: false, authenticated: false, error: 'api_error' })
                });
            });
        } catch (error) {
            return { success: false, authenticated: false, error: error.message };
        }
    }


    private processAuthenticateResult(authenticateResult: AuthenticateResultModel) {
        this.authenticateResult = authenticateResult;

        if (authenticateResult.accessToken) {
            this.login(
                authenticateResult.accessToken,
                authenticateResult.encryptedAccessToken,
                authenticateResult.expireInSeconds,
                this.rememberMe);

        } else {
            this._logService.warn('Unexpected authenticateResult!');
            this._router.navigate(['account/login']);
        }
    }

    private login(accessToken: string, encryptedAccessToken: string, expireInSeconds: number, rememberMe?: boolean): void {

        const tokenExpireDate = rememberMe ? (new Date(new Date().getTime() + 1000 * expireInSeconds)) : undefined;
        this._tokenService.setToken(
            accessToken,
            tokenExpireDate
        );

        this._utilsService.setCookieValue(
            AppConsts.authorization.encryptedAuthTokenName,
            encryptedAccessToken,
            tokenExpireDate,
            abp.appPath
        );

        let initialUrl = UrlHelper.initialUrl;
        if (initialUrl.indexOf('/login') > 0) {
            initialUrl = AppConsts.appBaseUrl;
        }

        location.href = initialUrl;
    }


    async checkSilentAuthOnce(): Promise<void> {
        if (this.silentAuthChecked || this._sessionService.isActiveSession) return;
        this.silentAuthChecked = true;
        try {
            const result = await this.checkSilentAuth();
            if (result.success && result.authenticated) {
                this._router.navigate([UrlHelper.getInitialUrl()]);
            }
        } catch (error) {
            console.error('Silent auth error:', error);
        }
    }

    selectBestRoute(): string {
        return '/app';
    }

    private clear(): void {
        this.authenticateModel = new AuthenticateModel();
        this.authenticateModel.rememberClient = false;
        this.authenticateResult = null;
        this.rememberMe = false;
    }
}

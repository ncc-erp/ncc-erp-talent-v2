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

export interface IHashMezonAuthModel {
    hashKey: string;
    userId: string;
    userName: string;
    userEmail: string;
    avatar: string;
    name: string;
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

    constructor(
        private _tokenAuthService: TokenAuthServiceProxy,
        private _router: Router,
        private _utilsService: UtilsService,
        private _tokenService: TokenService,
        private _logService: LogService,
        private _message: MessageService,
        private _mezonService: MezonLoginService
        //private _permissionChecker: PermissionCheckerService
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

    private processAuthenticateResult(authenticateResult: AuthenticateResultModel) {
        this.authenticateResult = authenticateResult;

        if (authenticateResult.accessToken) {
            // Successfully logged in
            this.login(
                authenticateResult.accessToken,
                authenticateResult.encryptedAccessToken,
                authenticateResult.expireInSeconds,
                this.rememberMe);

        } else {
            // Unexpected result!
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

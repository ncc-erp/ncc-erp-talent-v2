import { AppConsts } from '@shared/AppConsts';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { Type, CompilerOptions, NgModuleRef } from '@angular/core';
import { environment } from './environments/environment';

declare var $: any;
export class AppPreBootstrap {
    static run(appRootUrl: string, callback: () => void): void {
        AppPreBootstrap.getApplicationConfig(appRootUrl, () => {
            AppPreBootstrap.getGoogleClientAppId(callback);
            AppPreBootstrap.getEnableNormalLogin(callback);
        });
    }

    static bootstrap<TM>(moduleType: Type<TM>, compilerOptions?: CompilerOptions | CompilerOptions[]): Promise<NgModuleRef<TM>> {
        return platformBrowserDynamic().bootstrapModule(moduleType, compilerOptions);
    }

    private static getApplicationConfig(appRootUrl: string, callback: () => void) {
        return $.ajax({
            url: appRootUrl + 'assets/' + environment.appConfig,
            method: 'GET',
            headers: {
                'Abp.TenantId': abp.multiTenancy.getTenantIdCookie()
            }
        }).done(result => {
            AppConsts.appBaseUrl = result.appBaseUrl;
            AppConsts.remoteServiceBaseUrl = result.remoteServiceBaseUrl;
            AppConsts.autoBotServiceBaseUrl = result.autoBotServiceBaseUrl;
            AppConsts.localeMappings = result.localeMappings;
            AppConsts.enableNormalLogin = result.enableNormalLogin;
            AppConsts.backendIsNotABP = result.backendIsNotABP;
            callback();
        });
    }

    private static getGoogleClientAppId(callback: () => void) {
        if (AppConsts.backendIsNotABP) {
            callback();
        } else {
            return $.ajax({
                url: AppConsts.remoteServiceBaseUrl + '/api/services/app/Configuration/GetGoogleClientAppId',
                method: 'GET',
                headers: {
                    'Abp.TenantId': abp.multiTenancy.getTenantIdCookie()
                }
            }).done(res => {
                AppConsts.googleClientAppId = res.result;
                callback();
            });
        }

    }

    private static getEnableNormalLogin(callback: () => void) {
        if (AppConsts.backendIsNotABP) {
            callback();
        } else {
            return $.ajax({
                url: AppConsts.remoteServiceBaseUrl + '/api/services/app/Configuration/GetEnableNormalLogin',
                method: 'GET',
                headers: {
                    'Abp.TenantId': abp.multiTenancy.getTenantIdCookie()
                }
            }).done(res => {
                AppConsts.enableNormalLogin = res.result;
                callback();
            });
        }
    }
}

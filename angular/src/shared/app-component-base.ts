import { MESSAGE } from '@shared/AppConsts';
import { Observable } from 'rxjs';
import { Injector, ElementRef, Component, OnDestroy } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import {
    LocalizationService,
    PermissionCheckerService,
    FeatureCheckerService,
    NotifyService,
    SettingService,
    AbpMultiTenancyService
} from 'abp-ng2-module';

import { AppSessionService } from '@shared/session/app-session.service';
import { SubSink } from 'subsink';
import { ConfirmationService, MenuItem, MessageService } from 'primeng/api';
import { ApiResponse } from './paged-listing-component-base';
import { ActionEnum, API_RESPONSE_STATUS, ToastMessageType, UserType } from './AppEnums';
import { BreadCrumbConfig } from '@app/core/models/common/common.dto';
import { ActivatedRoute, Router } from '@angular/router';
import { PERMISSIONS_CONSTANT } from '@shared/permission/permissions';

@Component({
    template: ''
})
export abstract class AppComponentBase implements OnDestroy {
    readonly DIALOG_ACTION = ActionEnum;
    readonly USER_TYPE = UserType;
    readonly PS = PERMISSIONS_CONSTANT;

    localizationSourceName = AppConsts.localization.defaultLocalizationSourceName;

    isLoading: boolean = false;
    localization: LocalizationService;
    permission: PermissionCheckerService;
    feature: FeatureCheckerService;
    notify: NotifyService;
    setting: SettingService;
    message: MessageService;
    multiTenancy: AbpMultiTenancyService;
    appSession: AppSessionService;
    elementRef: ElementRef;
    confirmationService: ConfirmationService;
    breadcrumbConfig: BreadCrumbConfig;
    route: ActivatedRoute;
    router: Router;
    homeMenuItem: MenuItem = { icon: "pi pi-home", routerLink: "/" };
    public subs = new SubSink();

    constructor(injector: Injector) {
        this.localization = injector.get(LocalizationService);
        this.permission = injector.get(PermissionCheckerService);
        this.feature = injector.get(FeatureCheckerService);
        this.notify = injector.get(NotifyService);
        this.setting = injector.get(SettingService);
        this.message = injector.get(MessageService);
        this.multiTenancy = injector.get(AbpMultiTenancyService);
        this.appSession = injector.get(AppSessionService);
        this.elementRef = injector.get(ElementRef);
        this.confirmationService = injector.get(ConfirmationService);
        this.router = injector.get(Router);
        this.route = injector.get(ActivatedRoute);
    }
    ngOnDestroy(): void {
        this.subs.unsubscribe();
    }

    l(key: string, ...args: any[]): string {
        let localizedText = this.localization.localize(key, this.localizationSourceName);

        if (!localizedText) {
            localizedText = key;
        }

        if (!args || !args.length) {
            return localizedText;
        }

        args.unshift(localizedText);
        return abp.utils.formatString.apply(this, args);
    }

    isGranted(permissionName: string): boolean {
        return this.permission.isGranted(permissionName);
    }

    showToastMessage(type?: string, summary?: string, detail?: string) {
        this.message.add({ severity: type, summary: summary, detail: detail });
    }

    deleteConfirmAndShowToastMessage(ob: Observable<ApiResponse<any>>, titleName: string) {
        return new Observable(observer => {
            this.confirmationService.confirm({
                message: `Are you sure that you want to delete <strong>${titleName}</strong> ?`,
                header: 'Delete Confirmation',
                icon: 'pi pi-exclamation-triangle',
                accept: () => {
                    ob.subscribe(rs => {
                        this.isLoading = rs.loading;
                        if (!rs.loading) {
                            this.showToastMessage(rs.success ? ToastMessageType.SUCCESS : ToastMessageType.ERROR,
                                rs.success ? rs.result : MESSAGE.DELETE_FAILED,
                                titleName);
                            observer.next(rs.success ? API_RESPONSE_STATUS.SUCCESS : API_RESPONSE_STATUS.FAILD);
                            observer.complete();
                        }
                    })
                },
            })
        })
    }

    validPermissionUserType(userType: UserType, permissionNameIntern: string, permissionNameStaff: string): boolean {
        if (userType == UserType.INTERN)
            return this.permission.isGranted(permissionNameIntern);
        return this.permission.isGranted(permissionNameStaff);
    }

}

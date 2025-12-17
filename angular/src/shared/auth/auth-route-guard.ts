import { MessageService } from 'primeng/api';
import { Injectable } from '@angular/core';
import { PermissionCheckerService } from 'abp-ng2-module';
import { AppSessionService } from '../session/app-session.service';
import {
    CanActivate, Router,
    ActivatedRouteSnapshot,
    RouterStateSnapshot,
    CanActivateChild
} from '@angular/router';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { AppRoutes } from '@shared/AppRoutes';
import { MezonLoginService } from '@app/core/services/apis/mezon-api.service';

@Injectable()
export class AppRouteGuard implements CanActivate, CanActivateChild {

    constructor(
        private _permissionChecker: PermissionCheckerService,
        private _router: Router,
        private _sessionService: AppSessionService,
        private _mezonLoginService: MezonLoginService,
    ) {}

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        if (!this._sessionService.isActiveSession) {
            UrlHelper.setRedirectUrl(state.url);
            this._mezonLoginService.redirectToOAuth()
            return false;
        }

        if (!route.data || !route.data['permission']) {
            return true;
        }

        if (this._permissionChecker.isGranted(route.data['permission'])) {
            return true;
        }

        this._router.navigate([AppRoutes.APP.HOME]);
        return false;
    }

    canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        return this.canActivate(route, state);
    }
}

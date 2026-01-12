import { Injectable } from "@angular/core";
import {
  ActivatedRouteSnapshot,
  CanActivate,
  CanActivateChild,
  Router,
  RouterStateSnapshot,
} from "@angular/router";
import { AppRoutes } from "@shared/AppRoutes";
import { PermissionCheckerService } from "abp-ng2-module";
import { MessageService } from "primeng/api";
import { AppSessionService } from "../session/app-session.service";

@Injectable()
export class AppRouteGuard implements CanActivate, CanActivateChild {
  constructor(
    private _permissionChecker: PermissionCheckerService,
    private _router: Router,
    private _sessionService: AppSessionService,
    private _message: MessageService
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    if (!this._sessionService.user) {
      const loginRoute = "/account/login";

      this._router.navigate([loginRoute]);

      return false;
    }

    if (!route.data || !route.data["permission"]) {
      return true;
    }

    if (this._permissionChecker.isGranted(route.data["permission"])) {
      return true;
    }

    this._router.navigate([this.selectBestRoute()]);
    return false;
  }

  canActivateChild(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    return this.canActivate(route, state);
  }

  selectBestRoute(): string {
    if (!this._sessionService.user) {
      return "/account/login";
    }
    return AppRoutes.APP.HOME;
  }
}

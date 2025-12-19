import { Component, Injector, OnDestroy, OnInit } from "@angular/core";
import { isFromMezonApp } from "@app/core/helpers/utils.helper";
import { MezonLoginService } from "@app/core/services/apis/mezon-api.service";
import { accountModuleAnimation } from "@shared/animations/routerTransition";
import { AppComponentBase } from "@shared/app-component-base";
import { AppConsts } from "@shared/AppConsts";
import { UrlHelper } from "@shared/helpers/UrlHelper";
import { AppSessionService } from "@shared/session/app-session.service";
import { IHashMezonAuthModel, LoginService } from "./login.service";

@Component({
  templateUrl: "./login.component.html",
  animations: [accountModuleAnimation()],
})
export class LoginComponent
  extends AppComponentBase
  implements OnInit, OnDestroy
{
  submitting = false;
  nccCode: string;
  isShowPassword = true;
  enableNormalLogin: boolean = AppConsts.enableNormalLogin;

  hashData: string;

  constructor(
    injector: Injector,
    private _sessionService: AppSessionService,
    public loginService: LoginService,
    public mezonLoginService: MezonLoginService
  ) {
    super(injector);

    this.isLoading = true;
  }

  ngOnInit(): void {
    if (this._sessionService.isActiveSession) {
      this.router.navigate([UrlHelper.getInitialUrl()]);
      return;
    }
    if (isFromMezonApp()) {
      this.mezonLoginService.hashDataParams$.subscribe((userHashData) => {
        this.isLoading = true;
        this.hashData = userHashData;
        this.loginWithHash(this.hashData);
      });
    } else {
      this.mezonLoginService.redirectToOAuth();
      return;
    }
  }

  ngOnDestroy(): void {
    this.mezonLoginService.removeEventListeners();
  }

  // get multiTenancySideIsTeanant(): boolean {
  //   return this._sessionService.tenantId > 0;
  // }

  // get isSelfRegistrationAllowed(): boolean {
  //   if (!this._sessionService.tenantId) {
  //     return false;
  //   }

  //   return true;
  // }

  // login(): void {
  //   this.submitting = true;
  //   this.loginService.authenticate(() => (this.submitting = false));
  // }
  // signInWithGoogle(): void {
  //   this._authSocialService.signIn(GoogleLoginProvider.PROVIDER_ID).then((rs: any) =>{
  //     this.loginService.authenticateGoogle(rs.idToken)
  //   })
  // }

  loginWithHash(hash: string) {
    if (hash) {
      const hashData: IHashMezonAuthModel = {
        hashData: btoa(hash),
        tenancyName: null,
      };

      this.loginService
        .authenticateMezonHash(hashData)
        .subscribe((data) => (this.isLoading = data.isLoading));
    }
  }

  signInWithMezon(): void {
    this.mezonLoginService.redirectToOAuth();
  }
}

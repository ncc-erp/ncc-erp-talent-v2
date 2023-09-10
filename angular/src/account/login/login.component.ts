import { LoginService } from './login.service';
import { Component, Injector } from '@angular/core';
import { AbpSessionService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { GoogleLoginProvider, SocialAuthService } from 'angularx-social-login';
import { AppConsts } from '@shared/AppConsts';
@Component({
  templateUrl: './login.component.html',
  animations: [accountModuleAnimation()]
})
export class LoginComponent extends AppComponentBase {
  submitting = false;
  nccCode: string;
  isShowPassword = true;
  enableNormalLogin: boolean = AppConsts.enableNormalLogin;

  constructor(
    injector: Injector,
    public _authSocialService: SocialAuthService,
    private _sessionService: AbpSessionService,
    public loginService: LoginService,
  ) {
    super(injector);
  }

  get multiTenancySideIsTeanant(): boolean {
    return this._sessionService.tenantId > 0;
  }

  get isSelfRegistrationAllowed(): boolean {
    if (!this._sessionService.tenantId) {
      return false;
    }

    return true;
  }

  login(): void {
    this.submitting = true;
    this.loginService.authenticate(() => (this.submitting = false));
  }
  signInWithGoogle(): void {
    this._authSocialService.signIn(GoogleLoginProvider.PROVIDER_ID).then((rs: any) =>{
      this.loginService.authenticateGoogle(rs.idToken)
    })
  }
}

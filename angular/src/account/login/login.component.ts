import { IHashMezonAuthModel, LoginService } from './login.service';
import { Component, Injector, OnDestroy, OnInit, Renderer2 } from '@angular/core';
import { AbpSessionService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { MezonLoginService } from '@app/core/services/apis/mezon-api.service';
import { AppConsts } from '@shared/AppConsts';
import { isFromMezon } from '@app/core/helpers/utils.helper';
@Component({
  templateUrl: './login.component.html',
  animations: [accountModuleAnimation()]
})
export class LoginComponent extends AppComponentBase implements OnInit, OnDestroy {
  submitting = false;
  nccCode: string;
  isShowPassword = true;
  enableNormalLogin: boolean = AppConsts.enableNormalLogin;

  hashData: string;
  
  constructor(
    injector: Injector,
    private _sessionService: AbpSessionService,
    public loginService: LoginService,
    public mezonLoginService: MezonLoginService
  ) {
    super(injector);

    this.isLoading = true;
  }

  ngOnInit(): void {
    this.mezonLoginService.hashDataParams$
      .subscribe((userHashData) => {
        this.isLoading = true;
        this.hashData = userHashData;
        this.loginWithHash(this.hashData);
      });
  }

  ngOnDestroy(): void {
    this.mezonLoginService.removeEventListeners();
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
  // signInWithGoogle(): void {
  //   this._authSocialService.signIn(GoogleLoginProvider.PROVIDER_ID).then((rs: any) =>{
  //     this.loginService.authenticateGoogle(rs.idToken)
  //   })
  // }

  loginWithHash(hash: string) {
    if (hash) {
      const hashData: IHashMezonAuthModel = {
        hashData: btoa(hash),
        tenancyName: null
      };

      this.loginService.authenticateMezonHash(hashData).subscribe(data => this.isLoading = data.isLoading);
    }
  }

  signInWithMezon(): void {
    this.mezonLoginService.redirectToOAuth();
  }
}

import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { ToastMessageType } from '@shared/AppEnums';
import { AppRoutes } from '@shared/AppRoutes';
import { LoginService } from 'account/login/login.service';

@Component({
  selector: 'talent-auth-callback',
  templateUrl: './auth-callback.component.html',
  styleUrls: ['./auth-callback.component.scss']
})
export class AuthCallbackComponent extends AppComponentBase implements OnInit {
  constructor(
    injector: Injector,
    public loginService: LoginService,
  ) {
    super(injector)
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.isLoading = true;

      const code = params['code'];
      const scope = params['scope'];

      if (!code) {
        this.isLoading = false;
        this.showToastMessage(ToastMessageType.ERROR, 'Invalid authentication parameters!');
        this.router.navigate([AppRoutes.ACCOUNT.LOGIN]);
      }

      this.loginService.authenticateMezon(code, scope).subscribe({
        next: (res) => {
          this.isLoading = res.loading;
          if (res.error) {
            this.router.navigate([AppRoutes.ACCOUNT.LOGIN]);
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.router.navigate([AppRoutes.ACCOUNT.LOGIN]);
        }
      });
    });
  }
}

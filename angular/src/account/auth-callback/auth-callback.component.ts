import { AppRoutes } from '@shared/AppRoutes';
import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { ToastMessageType } from '@shared/AppEnums';
import { LoginService } from 'account/login/login.service';

@Component({
  selector: 'talent-auth-callback',
  templateUrl: './auth-callback.component.html',
  styleUrls: ['./auth-callback.component.scss']
})
export class AuthCallbackComponent extends AppComponentBase implements OnInit {
  window = window; // Make window accessible in template
  
  constructor(
    injector: Injector,
    public loginService: LoginService,
  ) {
    super(injector)
  }

  ngOnInit(): void {
    
    this.route.queryParams.subscribe(params => {
      const code = params['code'];
      const scope = params['scope'];
      const error = params['error'];

      const isPopupContext = window.opener && !window.opener.closed;
      
      if (isPopupContext) {
        this.handleSilentAuth(code, error, params['state']);
        return;
      }
      this.isLoading = true;

      if (!code) {
        this.isLoading = false;
        this.showToastMessage(ToastMessageType.ERROR, 'Invalid authentication parameters!');
        this.router.navigate([AppRoutes.ACCOUNT.LOGIN]);
        return;
      }

      this.loginService.authenticateMezon(code, scope).subscribe({
        next: (res) => {
          this.isLoading = res.loading;
          if (res.error) {
            console.error('Regular auth error:', res.error);
            this.router.navigate([AppRoutes.ACCOUNT.LOGIN]);
          }
        },
        error: (error) => {
          console.error('Regular auth error:', error);
          this.isLoading = false;
          this.router.navigate([AppRoutes.ACCOUNT.LOGIN]);
        }
      });
    });
  }

  private handleSilentAuth(code: string | null, error: string | null, state: string | null): void {
    const message = {
      type: 'mezon_silent_auth',
      code: code,
      error: error,
      state: state,
      timestamp: Date.now()
    };

    try {
      if (window.opener && !window.opener.closed) {
        window.opener.postMessage(message, window.location.origin);
        console.log('Message sent to opener successfully');
        setTimeout(() => {
          console.log('Closing popup window');
          window.close();
        }, 1000);
      } else {
        console.error('No opener window found or opener is closed');
      }
    } catch (error) {
      console.error('Error sending message to opener:', error);
    }
  }
}

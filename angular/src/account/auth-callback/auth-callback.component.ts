import { Component, Injector, OnInit } from '@angular/core';
import { HttpClient } from '@node_modules/@angular/common/http';
import { ActivatedRoute, Router } from '@node_modules/@angular/router/router';
import { AppComponentBase } from '@shared/app-component-base';
import { ToastMessageType } from '@shared/AppEnums';
import { API_BASE_URL } from '@shared/service-proxies/service-proxies';
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
      const state = params['state']
      if (code && scope && state) {
        this.showToastMessage(ToastMessageType.ERROR, 'something went wrong!');
      }

      this.loginService.authenticateMezon(code, scope).subscribe(res => {
        this.isLoading = res.loading;
      });
    });
  }
}

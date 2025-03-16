import { MezonLoginService } from "@app/core/services/apis/mezon-api.service";
import { Component, Injector, OnInit, Renderer2 } from "@angular/core"
import { AppComponentBase } from "@shared/app-component-base"
import { IHashMezonAuthModel, LoginService } from "account/login/login.service";
import { IUserHashInfo, IMezonUser } from "types/mezon/user-types";

@Component({
  selector: 'talent-mezon-auth',
  templateUrl: './mezon-auth.component.html',
  styleUrls: ['./mezon-auth.component.scss']
})
export class MezonAuthComponent extends AppComponentBase implements OnInit {

  hashUser: IUserHashInfo;
  mezonUser: IMezonUser;

  constructor(injector: Injector, private _mezonService: MezonLoginService, private _loginService: LoginService, private renderer: Renderer2) {
    super(injector);
  }

  ngOnInit(): void {
    if (this.appSession.user)
      return;

    this._mezonService.currentUserInfo$.subscribe((userData) => {
      this.mezonUser = userData;
    });

    this._mezonService.userHashInfo$.subscribe((userHashData) => {
      this.hashUser = userHashData;
      this.loginWithHash(this.hashUser, this.mezonUser);
    });
  }

  loginWithHash(hashUser: IUserHashInfo, mezonUser: IMezonUser) {
    if (hashUser && mezonUser) {
      const hashData: IHashMezonAuthModel = {
        hashKey: hashUser.hash,
        userId: hashUser.user_id,
        userName: mezonUser.user.username,
        userEmail: mezonUser.email,
        name: mezonUser.user.display_name,
        avatar: mezonUser.user.avatar_url,
        tenancyName: null
      };

      this._loginService.authenticateMezonHash(hashData).subscribe(data => this.isLoading = data.isLoading);
    }
  }
}


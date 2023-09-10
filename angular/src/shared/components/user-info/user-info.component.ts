import { AppComponentBase } from '@shared/app-component-base';
import { UserDto } from './../../service-proxies/service-proxies';
import { Component, OnInit, Input, Injector } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.scss']
})
export class UserInfoComponent extends AppComponentBase implements OnInit {
  @Input() userData: UserDto
  public user: UserDto
  constructor(injector: Injector) {
    super(injector)
  }
  ngOnInit(): void {
  }

  ngOnChanges(): void {
    this.user = this.userData
  }
  public getProjectTypefromEnum(projectType: number, enumObject: any) {
    for (const key in enumObject) {
      if (enumObject[key] == projectType) {
        return key;
      }
    }
  }
  getAvatar(member) {
    if (member.avatarPath) {
      return AppConsts.remoteServiceBaseUrl + member.avatarPath;
    }
    return '/assets/img/user.png';
  }
}


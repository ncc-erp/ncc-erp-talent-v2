import { Component, Injector, Input, OnInit } from '@angular/core';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { AppComponentBase } from '@shared/app-component-base';
import { UserType } from '@shared/AppEnums';
import { RequisitionInfo } from './../../../app/core/models/requisition/requisition.model';

@Component({
  selector: 'talent-requisition-info',
  templateUrl: './requisition-info.component.html',
  styleUrls: ['./requisition-info.component.scss']
})
export class RequisitionInfoComponent extends AppComponentBase implements OnInit {

  @Input() data: RequisitionInfo;
  @Input() showSkills = true;
  @Input() showPriority = false;
  @Input() showLevel = true;
  @Input() showUserType = false;

  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
  ) {
    super(injector);
  }

  ngOnInit(): void { }

  navigateToRequestDetail(id: number) {
    const reqPath = this.data.userType === UserType.INTERN ? 'req-intern' : 'req-staff';
    this.router.navigate([`/app/requisition/${reqPath}`, id], {
      queryParams: { type: this.data.userType }
    })
  }

}

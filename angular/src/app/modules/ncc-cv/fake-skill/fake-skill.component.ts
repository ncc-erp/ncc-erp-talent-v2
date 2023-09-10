import { Component, Injector, OnDestroy, OnInit } from "@angular/core";
import { FakeSkillService } from './../../../core/services/employee-profile/fake-skill.service';

import {
  ActionEnum,
  API_RESPONSE_STATUS, ToastMessageType
} from "@shared/AppEnums";
import {
  ApiResponse,
  PagedListingComponentBase,
  PagedRequestDto
} from "@shared/paged-listing-component-base";
import { DialogService, DynamicDialogRef } from "primeng/dynamicdialog";
import { DefaultRoute } from './../../../../shared/AppEnums';
import { FakeSkill,FakeSkillConfigDiaLog } from './../../../core/models/employee-profile/fake-skill-model';
import { FakeSkillDialogComponent } from "./fake-skill-dialog/fake-skill-dialog.component"; 
import { MESSAGE } from "@shared/AppConsts";


@Component({
  selector: 'talent-fake-skill',
  templateUrl: './fake-skill.component.html',
  styleUrls: ['./fake-skill.component.scss']
})
export class FakeSkillComponent extends PagedListingComponentBase<FakeSkill> implements OnInit, OnDestroy {
  public fakeSkills: FakeSkill[] = [];
  private dialogRef: DynamicDialogRef;

  constructor(
    injector: Injector,
    public dialogService: DialogService,
    private _fakeSkillService: FakeSkillService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
  }

  ngOnDestroy() {
    super.ngOnDestroy();
    if (this.dialogRef) this.dialogRef.close()
  }

  openDialog(obj: FakeSkill, dialogAction: ActionEnum) {
    const dialogConfig: FakeSkillConfigDiaLog = { fakeSkill: obj, action: dialogAction };
    const dialogRef = this.dialogService.open(FakeSkillDialogComponent, {
      header: `${dialogConfig.action} Skill`,
      width: "40%",
      contentStyle: { "max-height": "100%", overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<FakeSkill>) => {
      if (dialogConfig.action === ActionEnum.UPDATE) {
        const index = this.fakeSkills.findIndex((x) => x.id == res.result.id);
        this.fakeSkills[index] = res.result;
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.name);
        return;
      }
      this.refresh();
    });
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subs.add(
      this._fakeSkillService.getAllPagging(request).subscribe((rs) => {
        this.fakeSkills = [];
        if (rs.success) {
          this.fakeSkills = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(position: FakeSkill): void {
    const deleteRequest = this._fakeSkillService.delete(position.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, position.name).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
      })
    );
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Employee Profile", routerLink: DefaultRoute.Employee_Profile, styleClass: 'menu-item-click' }, { label: "Skill" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }

}

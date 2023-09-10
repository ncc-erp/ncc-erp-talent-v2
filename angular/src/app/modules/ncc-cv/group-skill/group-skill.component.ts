import { Component, Injector, OnDestroy, OnInit } from "@angular/core";
import { GroupSkillService } from './../../../core/services/employee-profile/group-skill.service';
import {
  ActionEnum,
  API_RESPONSE_STATUS,
  DefaultRoute,
  ToastMessageType
} from "@shared/AppEnums";
import {
  ApiResponse,
  PagedListingComponentBase,
  PagedRequestDto
} from "@shared/paged-listing-component-base";
import { DialogService, DynamicDialogRef } from "primeng/dynamicdialog";
import { GroupSkill,GroupSkillConfigDiaLog } from './../../../core/models/employee-profile/group-skill-model';
import { GroupSkillDialogComponent } from "./group-skill-dialog/group-skill-dialog.component"; 
import { MESSAGE } from "@shared/AppConsts";

@Component({
  selector: 'talent-group-skill',
  templateUrl: './group-skill.component.html',
  styleUrls: ['./group-skill.component.scss']
})
export class GroupSkillComponent extends PagedListingComponentBase<GroupSkill> implements OnInit, OnDestroy {
  public groupSkills: GroupSkill[] = [];
  dialogRef: DynamicDialogRef;

  constructor(
    injector: Injector,
    public dialogService: DialogService,
    private _groupSkillService: GroupSkillService
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

  openDialog(obj: GroupSkill, dialogAction: ActionEnum) {
    const dialogConfig: GroupSkillConfigDiaLog = { groupSkill: obj, action: dialogAction,showButtonSave:true }
    this.dialogRef = this.dialogService.open(GroupSkillDialogComponent, {
      header: `${dialogConfig.action} Group Skill`,
      width: "40%",
      contentStyle: { "max-height": "500px", overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    this.dialogRef.onClose.subscribe((res: ApiResponse<GroupSkill>) => {
      if (dialogConfig.action === ActionEnum.UPDATE) {
        const index = this.groupSkills.findIndex((x) => x.id == res.result.id);
        this.groupSkills[index] = res.result;
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.name);
        return;
      }
      this.refresh();
    });
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subs.add(
      this._groupSkillService.getAllPagging(request).subscribe((rs) => {
        this.groupSkills = [];
        if (rs.success) {
          this.groupSkills = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(entity: GroupSkill): void {
    const deleteRequest = this._groupSkillService.delete(entity.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, entity.name).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
      })
    );
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Employee Profile", routerLink: DefaultRoute.Employee_Profile, styleClass: 'menu-item-click' }, { label: "Group Skill" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}

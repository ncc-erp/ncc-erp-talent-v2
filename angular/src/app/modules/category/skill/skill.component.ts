import { MESSAGE } from '@shared/AppConsts';
import { Component, Injector, OnDestroy, OnInit } from "@angular/core";
import { SkillService } from '@app/core/services/categories/skill.service';
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
import { Skill, SkillConfigDiaLog } from '../../../core/models/categories/skill.model';
import { SkillDialogComponent } from './skill-dialog/skill-dialog.component';

@Component({
  selector: "app-skill",
  templateUrl: "./skill.component.html",
  styleUrls: ["./skill.component.scss"],
})
export class SkillComponent extends PagedListingComponentBase<Skill> implements OnInit, OnDestroy {
  public skills: Skill[] = [];
  dialogRef: DynamicDialogRef;

  constructor(
    injector: Injector,
    public dialogService: DialogService,
    private _skill: SkillService
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

  openDialog(obj: Skill, dialogAction: ActionEnum) {
    const dialogConfig: SkillConfigDiaLog = { skill: obj, action: dialogAction }
    this.dialogRef = this.dialogService.open(SkillDialogComponent, {
      header: `${dialogConfig.action} Skill`,
      width: "40%",
      contentStyle: { "max-height": "500px", overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    this.dialogRef.onClose.subscribe((res: ApiResponse<Skill>) => {
      if (dialogConfig.action === ActionEnum.UPDATE) {
        const index = this.skills.findIndex((x) => x.id == res.result.id);
        this.skills[index] = res.result;
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.name);
        return;
      }
      this.refresh();
    });
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subs.add(
      this._skill.getAllPagging(request).subscribe((rs) => {
        this.skills = [];
        if (rs.success) {
          this.skills = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(entity: Skill): void {
    const deleteRequest = this._skill.delete(entity.id);
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
      menuItem: [{ label: "Categories", routerLink: DefaultRoute.Category, styleClass: 'menu-item-click' }, { label: "Skills",}],
      homeItem: { icon: "pi pi-home", routerLink: "/app/home" },
    };
  }
}

import { Component, Injector, OnInit } from '@angular/core';
import { BreadCrumbConfig } from '@app/core/models/common/common.dto';
import { WorkingExperience, WorkingExperienceConfigDiaLog } from '@app/core/models/employee-profile/profile-model';
import {EmployeeProfileService} from '@app/core/services/employee-profile/employee-profile.service';
import { MESSAGE } from '@shared/AppConsts';
import { ActionEnum, DefaultRoute, ToastMessageType } from '@shared/AppEnums';
import { ApiResponse, PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { DialogService } from 'primeng/dynamicdialog';
import { DialogProjectComponent, ProjectDialogConfig } from '../project/dialog-project/dialog-project.component';
import { DialogWorkingExperienceComponent } from './dialog-working-experience/dialog-working-experience.component';

@Component({
  selector: 'talent-working-experience',
  templateUrl: './working-experience.component.html',
  styleUrls: ['./working-experience.component.scss']
})
export class WorkingExperienceComponent extends PagedListingComponentBase<WorkingExperience> implements OnInit {
  workingExperiences: WorkingExperience[] = [];
  searchTextProjectName : string = '';
  searchTextPositions : string = '';
  searchTextTechnology : string = '';
  searchTextEmployeeName : string = '';
  isIncludeVers : boolean = false;
  constructor(
    injector: Injector,
    private _employeeProfile: EmployeeProfileService,
    private _dialog: DialogService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadCrumbConfig();
  }
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    const payload = {
      projectName: this.searchTextProjectName?.trim().replace(/ +/g, ' '),
      technologies: this.searchTextTechnology?.trim().replace(/ +/g, ' '),
      positions: this.searchTextPositions?.trim().replace(/ +/g, ' '),
      employeeName: this.searchTextEmployeeName?.trim().replace(/ +/g, ' '),
      isIncludeVers: this.isIncludeVers,
      maxResultCount: request.maxResultCount,
      skipCount: request.skipCount,
    };
    this.subs.add(
      this._employeeProfile.getWorkingExperiencePaging(payload).subscribe((rs) => {
        this.workingExperiences = [];
        if (rs.success) {
          this.workingExperiences = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  toggleIsIncludeVers() {
    this.isIncludeVers = !this.isIncludeVers;
    this.onGetDataChange();
  }
  openDialog(obj: WorkingExperience, dialogAction: ActionEnum) {
    const dialogConfig: WorkingExperienceConfigDiaLog = { workingExperience: obj, action: dialogAction};
    const dialogRef = this._dialog.open(DialogWorkingExperienceComponent, {
      header: "Project Detail",
      width: "55%",
      contentStyle: { "max-height": "800px", overflow: "auto" },
      baseZIndex: 10000,
      data: dialogConfig,
    });
    dialogRef.onClose.subscribe((res: ApiResponse<WorkingExperience>) => {
      if (res) {
        this.onGetDataChange();
        if (dialogConfig.action === ActionEnum.UPDATE) {
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.ADD_SUCCESS);
          return;
        }
      }
    });
  }
  
  onGetDataChange() {
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  protected delete(): void {
    throw new Error('Method not implemented.');
  }

  private getBreadCrumbConfig(): BreadCrumbConfig {
    return {
      homeItem: this.homeMenuItem,
      menuItem: [
        { label: "Employee Profile", routerLink: DefaultRoute.ProjectManagement, styleClass: 'menu-item-click' },
        { label: "Work Experience" }
      ]
    }
  }
}
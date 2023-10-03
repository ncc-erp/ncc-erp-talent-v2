import { CandidateInternService } from '@app/core/services/candidate/candidate-intern.service';
import { CandidateStaffService } from './../../../core/services/candidate/candidate-staff.service';
import { Component, Injector, OnDestroy, OnInit, Optional } from '@angular/core';
import { copyObject } from '@app/core/helpers/utils.helper';
import { CandidateEducationPayload } from '@app/core/models/candidate/candidate-education.model';
import { Education, EducationConfigDiaLog } from '@app/core/models/categories/education.model';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { ActionEnum, API_RESPONSE_STATUS, COMPARISION_OPERATOR, DefaultRoute, ToastMessageType } from '@shared/AppEnums';
import { ApiResponse, Filter, PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { EducationService } from './../../../core/services/categories/education.service';
import { EducationDialogComponent } from './education-dialog/education-dialog.component';
import { MESSAGE } from '@shared/AppConsts';

@Component({
  selector: 'talent-education',
  templateUrl: './education.component.html',
  styleUrls: ['./education.component.scss']
})
export class EducationComponent extends PagedListingComponentBase<Education> implements OnInit, OnDestroy {

  public educations: Education[] = [];
  public searchWithEducationType: string;

  private dialogRef: DynamicDialogRef;
  public isSelectMode: boolean;
  public eduSelectedIds: number[];
  public canEducationPayload: CandidateEducationPayload;
  public _candidate: CandidateStaffService | CandidateInternService;

  constructor(
    @Optional() public config: DynamicDialogConfig,
    @Optional() public ref: DynamicDialogRef,
    injector: Injector,
    public dialogService: DialogService,
    public _utilities: UtilitiesService,
    private _education: EducationService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
    this.handleDataForSelectMode();
  }

  ngOnDestroy() {
    super.ngOnDestroy();
    if (this.dialogRef) this.dialogRef.close()
  }

  openDialog(obj: Education, dialogAction: ActionEnum) {
    const dialogConfig: EducationConfigDiaLog = { education: obj, action: dialogAction };
    const dialogRef = this.dialogService.open(EducationDialogComponent, {
      header: `${dialogConfig.action} Education`,
      width: "40%",
      contentStyle: { "max-height": "500px", overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<Education>) => {
      if (dialogConfig.action === ActionEnum.UPDATE && res) {
        const index = this.educations.findIndex((x) => x.id == res.result.id);
        this.educations[index] = res.result;
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.name);
        return;
      }
      this.refresh();
    });
  }


  isSelectedAlready(education: Education) {
    return this.eduSelectedIds.includes(education.id);
  }

  onEducationSelected(entity: Education) {
    if (!this.eduSelectedIds.includes(entity.id)) {
      this.eduSelectedIds.push(entity.id);

      const payload = { ...this.canEducationPayload, educationId: entity.id }
      this.subs.add(
        this._candidate.createEducation(payload).subscribe(res => {
          if (!res.loading && res.success) {
            this.showToastMessage(ToastMessageType.SUCCESS, `Added ${entity.name}`);
          }
        })
      );
    }
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    const filterItem: Filter[] = [{
      propertyName: 'educationTypeId',
      value: this.searchWithEducationType,
      comparision: COMPARISION_OPERATOR.Equal,
    }];
    request.filterItems = this.searchWithEducationType ? filterItem : [];
    this.subs.add(
      this._education.getAllPagging(request).subscribe((rs) => {
        this.educations = [];
        if (rs.success) {
          this.educations = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(education: Education): void {
    const deleteRequest = this._education.delete(education.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, education.name).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
      })
    );
  }

  private handleDataForSelectMode() {
    const dataDialog = this.config?.data;
    if (dataDialog) {
      this.isSelectMode = dataDialog.dialogMode === ActionEnum.SELECT;
      this.eduSelectedIds = copyObject(dataDialog.eduSeletedIds);
      this._candidate = dataDialog._candidate;
      this.canEducationPayload = dataDialog.canEducationPayload;
    }
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Categories", routerLink: DefaultRoute.Category , styleClass: 'menu-item-click'}, { label: "Education" }],
      homeItem: { icon: "pi pi-home", routerLink: "/", },
    };
  }
}

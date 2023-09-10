import { DateOption } from './../../../../../shared/AppConsts';
import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { getFormControlValue } from '@app/core/helpers/utils.helper';
import { PayloadRequisition, RequisitionCloseAndClone, RequisitionIntern, RequisitionInternConfigDiaLog } from '@app/core/models/requisition/requisition.model';
import { RequisitionInternService } from '@app/core/services/requisition/requisition-intern.service';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { AppComponentBase } from '@shared/app-component-base';
import { DateFormat } from '@shared/AppConsts';
import { ActionEnum, SearchType, UserType } from '@shared/AppEnums';
import { ApiResponse } from '@shared/paged-listing-component-base';
import * as moment from 'moment';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'talent-requisition-intern-dialog',
  templateUrl: './requisition-intern-dialog.component.html',
  styleUrls: ['./requisition-intern-dialog.component.scss']
})
export class RequisitionInternDialogComponent extends AppComponentBase implements OnInit {
  submitted: boolean = false;
  form: FormGroup;
  dataDialogConfig: RequisitionInternConfigDiaLog;
  requistionCloseAndClone: RequisitionCloseAndClone;
  isCreating: boolean = false;
  isSearchAnd: boolean = false;

  catalogConfig = {
    labelName: 'Skills',
    catalogList: this._utilities.catSkill,
    optionLabel: 'name',
    optionValue: 'id'
  }

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    public _utilities: UtilitiesService,
    public dialogService: DialogService,
    private injector: Injector,
    private _fb: FormBuilder,
    private _reqIntern: RequisitionInternService,
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.dataDialogConfig = this.config.data;
    this.requistionCloseAndClone = this.dataDialogConfig?.reqCloseAndClone || null;
    this.initForm(this.dataDialogConfig.action);
  }

  onSkillChange(value: any) {
    this.formControls['skillIds'].patchValue(value, { emitEvent: false });
  }

  onPositionSelect(subPositionId: number) {
    this.formControls?.subPositionId?.setValue(subPositionId);
  }

  onSearchTypeChange(value: string) {
    this.isSearchAnd = value === SearchType.AND;
  }

  onSave() {
    this.submitted = true;
    if (this.form.invalid) return;

    const action = this.dataDialogConfig.action;
    const payload: PayloadRequisition = {
      userType: getFormControlValue(this.form, 'userType'),
      subPositionId: getFormControlValue(this.form, 'subPositionId'),
      priority: getFormControlValue(this.form, 'priorityId'),
      skillIds: getFormControlValue(this.form, 'skillIds'),
      branchId: getFormControlValue(this.form, 'branchId'),
      quantity: getFormControlValue(this.form, 'quantity'),
      note: getFormControlValue(this.form, 'note'),
      timeNeed: moment(this.formControls['timeNeed'].value).format(DateFormat.YYYY_MM_DD),
    };

    if (action === ActionEnum.UPDATE || action === ActionEnum.CLOSE_AND_CLONE) {
      payload['id'] = this.dataDialogConfig.requisitionIntern.id;
    }

    if (action === ActionEnum.CLOSE_AND_CLONE) {
      const listCV = this.requistionCloseAndClone.candidateRequisitions.filter(item => item.isClone);
      payload['cvIds'] = listCV.map(cv => cv.id);
    }

    this.handleSave(payload, action);
  }

  checkAndClosePopup(res: ApiResponse<RequisitionIntern>) {
    this.isCreating = res.loading;
    if (!res.loading) this.ref.close(res);
  }

  private initForm(action: ActionEnum) {
    const isFillData = action !== ActionEnum.CREATE;
    const reqIntern = this.dataDialogConfig.requisitionIntern;

    const branchId: number = this.getPropertyVal(isFillData, reqIntern, 'branchId');
    const subPositionId: number = this.getPropertyVal(isFillData, reqIntern, 'subPositionId');
    const skillIds: number[] = isFillData ? reqIntern.skills.map(obj => obj.id) : [];
    const quantity: number = this.getPropertyVal(isFillData, reqIntern, 'quantity') || 1;
    const note: string = this.getPropertyVal(isFillData, reqIntern, 'note');
    const priorityId: number = this.getPropertyVal(isFillData, reqIntern, 'priority');
    const timeNeed: Date = this.getTimeNeed(isFillData, reqIntern?.timeNeed as string);

    this.form = this._fb.group({
      userType: [UserType.INTERN, [Validators.required]],
      branchId: [branchId, [Validators.required]],
      subPositionId: [subPositionId, [Validators.required]],
      skillIds: [skillIds, [Validators.required]],
      note: [note, []],
      quantity: [quantity, [Validators.required]],
      priorityId: [priorityId, [Validators.required]],
      timeNeed: [timeNeed, [Validators.required]],
    });
  }

  private getTimeNeed(isFillData: boolean, timeNeed: string) {
    if (isFillData) {
      return timeNeed && new Date(timeNeed);
    }
    const defaultDate = new Date();
    defaultDate.setDate(defaultDate.getDate() + DateOption.DAYS_PER_WEEK);
    return defaultDate;
  }

  private handleSave(payload: PayloadRequisition, action: ActionEnum) {
    if (action === ActionEnum.CREATE || action === ActionEnum.CLONE) {
      this.subs.add(
        this._reqIntern.create(payload).subscribe(res => this.checkAndClosePopup(res))
      );
      return;
    }

    if (action === ActionEnum.CLOSE_AND_CLONE) {
      this.subs.add(
        this._reqIntern.closeAndCloneRequest(payload).subscribe(res => this.checkAndClosePopup(res))
      );
      return;
    }

    this.subs.add(
      this._reqIntern.update(payload).subscribe(res => this.checkAndClosePopup(res))
    );
  }

  private getPropertyVal(isFillData: boolean, obj: any, propName: string) {
    return isFillData ? obj[propName] : null;
  }
}

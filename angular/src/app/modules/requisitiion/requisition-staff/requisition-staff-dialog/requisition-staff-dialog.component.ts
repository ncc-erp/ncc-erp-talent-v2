import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { getFormControlValue } from '@app/core/helpers/utils.helper';
import { CustomValidators } from '@app/core/helpers/validator.helper';
import { PayloadRequisitionStaff, RequisitionStaff, RequisitionStaffConfigDiaLog } from '@app/core/models/requisition/requisition.model';
import { RequisitionStaffService } from '@app/core/services/requisition/requisition-staff.service';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { AppComponentBase } from '@shared/app-component-base';
import { DateFormat, DateOption } from '@shared/AppConsts';
import { ActionEnum, SearchType, UserType } from '@shared/AppEnums';
import { ApiResponse } from '@shared/paged-listing-component-base';
import * as moment from 'moment';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'talent-requisition-staff-dialog',
  templateUrl: './requisition-staff-dialog.component.html',
  styleUrls: ['./requisition-staff-dialog.component.scss']
})
export class RequisitionStaffDialogComponent extends AppComponentBase implements OnInit {

  submitted: boolean = false;
  form: FormGroup;
  dataDialogConfig: RequisitionStaffConfigDiaLog;
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
    private _requisitionStaff: RequisitionStaffService,
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.dataDialogConfig = this.config.data;
    this.initForm(this.dataDialogConfig.action);
  }

  onSkillChange(value: any) {
    this.formControls['skillIds'].patchValue(value, {emitEvent: false});
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
    const payload: PayloadRequisitionStaff = {
      userType: getFormControlValue(this.form, 'userType'),
      subPositionId: getFormControlValue(this.form, 'subPositionId'),
      priority: getFormControlValue(this.form, 'priorityId'),
      skillIds: getFormControlValue(this.form, 'skillIds'),
      branchId: getFormControlValue(this.form, 'branchId'),
      quantity: getFormControlValue(this.form, 'quantity'),
      note: getFormControlValue(this.form, 'note'),
      level: getFormControlValue(this.form, 'levelId'),
      timeNeed: moment(this.formControls['timeNeed'].value).format(DateFormat.YYYY_MM_DD),
    };

    action === ActionEnum.UPDATE && (payload['id'] = this.dataDialogConfig.requisitionStaff.id);
    this.handleSave(payload, action);
  }

  checkAndClosePopup(res: ApiResponse<RequisitionStaff>) {
    this.isCreating = res.loading;
    if (!res.loading) this.ref.close(res);
  }

  private initForm(action: ActionEnum) {
    const isFillData = action === ActionEnum.UPDATE || action === ActionEnum.CLONE;
    const reqStaff = this.dataDialogConfig.requisitionStaff;

    const branchId: number = this.getPropertyVal(isFillData, reqStaff, 'branchId');
    const subPositionId: number = this.getPropertyVal(isFillData, reqStaff, 'subPositionId');
    const levelId: number = this.getPropertyVal(isFillData, reqStaff, 'level');
    const skillIds: number[] = isFillData ? reqStaff.skills.map(obj => obj.id) : [];
    const quantity: number = this.getPropertyVal(isFillData, reqStaff, 'quantity') || 1;
    const note: string = this.getPropertyVal(isFillData, reqStaff, 'note');
    const priorityId: number = this.getPropertyVal(isFillData, reqStaff, 'priority');
    const defaultDate = new Date();
    defaultDate.setDate(defaultDate.getDate() + DateOption.DAYS_PER_WEEK);
    const timeNeed: Date = isFillData ? new Date(reqStaff.timeNeed) : defaultDate;
    
    this.form = this._fb.group({
      userType: [UserType.STAFF, [Validators.required]],
      branchId: [branchId, [Validators.required]],
      subPositionId: [subPositionId, [Validators.required]],
      levelId: [levelId, [Validators.required]],
      skillIds: [skillIds, [Validators.required]],
      note: [note, []],
      quantity: [quantity, [Validators.required]],
      priorityId: [priorityId, [Validators.required]],
      timeNeed: [timeNeed, [Validators.required]],
    });
  }

  private handleSave(payload: PayloadRequisitionStaff, action: ActionEnum) {
    if (action === ActionEnum.CREATE || action === ActionEnum.CLONE) {
      this.subs.add(
        this._requisitionStaff.create(payload).subscribe(res => this.checkAndClosePopup(res))
      );
      return;
    }

    this.subs.add(
      this._requisitionStaff.update(payload).subscribe(res => this.checkAndClosePopup(res))
    );
  }

  private getPropertyVal(isFillData: boolean, obj: any, propName: string) {
    return isFillData ? obj[propName] : null;
  }
}

import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {
  FormDialogCreateScoreSetting, FormDialogUpdateScoreRange, FormFieldDisable,
  ParamsGetScoreRange, ScoreRangeWithSetting, ScoreSetting, ScoreSettingDialogData
} from '@app/core/models/categories/score-range-setting.model';
import { ScoreSettingService } from '@app/core/services/categories/score-setting.service';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { ActionEnum } from '@shared/AppEnums';
import { AppComponentBase } from '@shared/app-component-base';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { LevelInfo } from '@app/core/models/common/common.dto';
import { InternSalaryCatalog } from '@app/core/models/common/common.dto';
import { checkArrayUndefined, checkNumber } from '@app/core/helpers/utils.helper';

@Component({
  selector: 'talent-dialog-score-setting',
  templateUrl: './dialog-score-setting.component.html',
  styleUrls: ['./dialog-score-setting.component.scss']
})
export class DialogScoreSettingComponent extends AppComponentBase implements OnInit {

  dialogData: ScoreSettingDialogData;
  form: FormGroup;
  isValidateScoreExists: boolean;
  isValidateLevelExists: boolean;
  submitted = false;
  scoreRangeResult: ScoreRangeWithSetting[];
  catInternLevels: InternSalaryCatalog[] = this._utilities.catLevelFinalIntern.filter((item: any) => item?.defaultName !== "FresherPlus");;
  catStaffLevels: LevelInfo[] = this._utilities.catLevelFinalStaff.filter((item: any) => item?.id !== 100);
  min: number = 0;
  max: number = 5;
  step: number = 0.1;

  constructor(public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private injector: Injector,
    public _utilities: UtilitiesService,
    private _fb: FormBuilder,
    public dialogService: DialogService,
    private _scoreSettingService: ScoreSettingService,
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  onPositionSelect(subPositionId?: number) {
    this.formControls?.subPositionId?.setValue(subPositionId);
    this.getScoreRanges();
  }

  getScoreRanges() {
    if (checkNumber(this.form.get('userType').value) && checkNumber(this.form.get('subPositionId').value)) {
      const params: ParamsGetScoreRange = {
        userType: this.form.get('userType').value,
        subPositionId: this.form.get('subPositionId').value
      }
      this._scoreSettingService.getAllRange(params).subscribe((rs: ApiResponse<ScoreRangeWithSetting[]>) => {
        this.scoreRangeResult = rs.result;
      })
    }
  }

  getLevel(userTypeEvent: any) {
    if (userTypeEvent.selectedOption?.id === this.USER_TYPE.STAFF) return this.catStaffLevels;
    else if (userTypeEvent.selectedOption?.id === this.USER_TYPE.INTERN) return this.catInternLevels;
    else return []
  }

  ngOnInit(): void {
    this.dialogData = this.config.data;
    this.initForm(this.dialogData.dialogAction);
  }

  private initForm(action: ActionEnum) {
    const isFillData: boolean = action === ActionEnum.UPDATE;
    const scoreSettingData: ScoreSetting = this.dialogData.scoreSetting;
    const scoreRangeData: ScoreRangeWithSetting = this.dialogData.scoreRange;

    const userType: FormFieldDisable = this.getPropertyValDisabled(isFillData, scoreSettingData, 'userType');
    const subPositionId: FormFieldDisable = this.getPropertyValDisabled(isFillData, scoreSettingData, 'subPositionId');
    const scoreFrom: FormFieldDisable = this.getValueScoreFrom(isFillData, scoreRangeData, 'scoreFrom');
    const scoreTo: FormFieldDisable = this.getValueScoreTo(isFillData, scoreRangeData, 'scoreTo');
    const level: FormFieldDisable = this.getPropertyVal(isFillData, scoreRangeData, 'level');

    this.form = this._fb.group({
      userType: [userType, [Validators.required]],
      subPositionId: [subPositionId, [Validators.required]],
      scoreFrom: [scoreFrom, [Validators.min(0), Validators.max(5), Validators.required]],
      scoreTo: [scoreTo, [Validators.min(0), Validators.max(5), Validators.required]],
      level: [level, [Validators.required]],
    });
  }

  disabledSubPosition() {
    if (this.dialogData.dialogAction === ActionEnum.UPDATE) return true
  }

  Save() {
    this.submitted = true;
    this.isValidateScoreExists = false;
    this.isValidateLevelExists = false;
    const payloadCreate: FormDialogCreateScoreSetting = this.form.getRawValue();
    this.validateExists(payloadCreate, this.dialogData.dialogAction);
    if (this.isValidateScoreExists || this.isValidateLevelExists || this.form.invalid) return;
    if (this.dialogData.dialogAction === ActionEnum.CREATE) {
      this.create(payloadCreate);
    } else if (this.dialogData.dialogAction === ActionEnum.UPDATE) {
      const payloadUpdate: FormDialogUpdateScoreRange = {
        scoreFrom: payloadCreate.scoreFrom,
        scoreTo: payloadCreate.scoreTo,
        scoreRangeId: this.dialogData.scoreRange.id,
        level: payloadCreate.level
      }
      this.update(payloadUpdate);
    }
  }

  maxScoreForm() {
    if (this.formControls.scoreTo.value >= (this.min + this.step)) {
      return this.formControls.scoreTo.value - this.step;
    }
    else if (!checkNumber(this.formControls.scoreTo.value)) {
      return this.min;
    }
  }

  incrementButtonClassScoreForm() {
    return this.formControls.scoreFrom.value == this.max || !checkNumber(this.formControls.scoreTo.value) ||
      this.formControls.scoreFrom.value == this.formControls.scoreTo.value - this.step ? 'p-button-secondary' : '';
  }

  decrementButtonClassScoreForm() {
    return this.formControls.scoreFrom.value == this.min ? 'p-button-secondary' : '';
  }

  minScoreTo() {
    if (checkNumber(this.formControls.scoreFrom.value)) {
      return this.formControls.scoreFrom.value + this.step;
    }
    return this.min + this.step;
  }

  incrementButtonClassScoreTo() {
    return this.formControls.scoreTo.value == this.max ? 'p-button-secondary' : ''
  }

  decrementButtonClassScoreTo() {
    return this.formControls.scoreTo.value == this.min || this.minScoreTo() == this.formControls.scoreTo.value ? 'p-button-secondary' : ''
  }


  validateExists(payload: FormDialogCreateScoreSetting, action: ActionEnum) {
    let scoreRangeValidate: ScoreRangeWithSetting[] = [];
    scoreRangeValidate = action === ActionEnum.UPDATE ?
      this.dialogData.scoreSetting.scoreRanges.filter((arr) => {
        return arr !== this.dialogData.scoreRange;
      }) : checkArrayUndefined(this.scoreRangeResult);
    scoreRangeValidate.find((rs) => {
      const scoreToValidate: boolean = payload.scoreTo > rs.scoreTo && payload.scoreFrom >= rs.scoreTo || payload.scoreTo <= rs.scoreFrom;
      if (!scoreToValidate) {
        this.isValidateScoreExists = true;
      }
    });

    scoreRangeValidate.find((rs) => {
      if (payload.level == rs.level) {
        this.isValidateLevelExists = true;
      };
    })
  }

  checkAndClosePopup(res: ApiResponse<ScoreSetting[]>) {
    if (!res.loading) this.ref.close(res);
  }

  create(payload: FormDialogCreateScoreSetting) {
    this._scoreSettingService.createScoreSetting(payload).subscribe((res) => { this.checkAndClosePopup(res) });
  }

  update(payload: FormDialogUpdateScoreRange) {
    this._scoreSettingService.updateScoreRange(payload).subscribe((res) => { this.checkAndClosePopup(res) });
  }

  close() {
    this.ref.close();
  }

  private getPropertyValDisabled(isFillData: boolean, obj: any, propName: string) {
    return isFillData ? { value: obj[propName], disabled: true } : null;
  }

  private getValueScoreFrom(isFillData: boolean, obj: any, propName: string) {
    return isFillData ? obj[propName] : 0;
  }

  private getValueScoreTo(isFillData: boolean, obj: any, propName: string) {
    return isFillData ? obj[propName] : 1;
  }

  private getPropertyVal(isFillData: boolean, obj: any, propName: string) {
    return isFillData ? obj[propName] : null;
  }
}

import { Component, Injector, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {getFormControlValue, randomHexColor} from '@app/core/helpers/utils.helper';
import {CandidateLanguage, CandidateLanguageConfigDiaLog} from '@app/core/models/candidate/candidate-language.model';
import {CandidateLanguageService} from '@app/core/services/candidate/candidate-language.service';
import {MESSAGE} from '@shared/AppConsts';
import {ActionEnum, ToastMessageType} from '@shared/AppEnums';
import {AppComponentBase} from '@shared/app-component-base';
import {ApiResponse} from '@shared/paged-listing-component-base';
import {DynamicDialogConfig, DynamicDialogRef} from 'primeng/dynamicdialog';

@Component({
  selector: 'talent-language-dialog',
  templateUrl: './language-dialog.component.html',
  styleUrls: ['./language-dialog.component.scss']
})
export class LanguageDialogComponent extends AppComponentBase implements OnInit {
  submitted: boolean = false;
  form: FormGroup;
  candidateLanguagesConfigDiaLog: CandidateLanguageConfigDiaLog;
  action: ActionEnum;
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private injector: Injector,
    private _fb: FormBuilder,
    private _candidatelanguage: CandidateLanguageService
  ) {
    super(injector);
  }
  get formControls() {
    return this.form.controls;
  }
  ngOnInit() {

    this.candidateLanguagesConfigDiaLog = this.config.data;
    this.action = this.candidateLanguagesConfigDiaLog.action;
    this.initForm();
  }

  private initForm() {
    const isUpdate = this.candidateLanguagesConfigDiaLog.action == ActionEnum.UPDATE;
    const hexColor = isUpdate ? this.candidateLanguagesConfigDiaLog.candidateLanguage.colorCode : randomHexColor();
    this.form = this._fb.group({
      name: [
        isUpdate ? this.candidateLanguagesConfigDiaLog.candidateLanguage.name : '',
        [Validators.required]
      ],
      alias: [
        isUpdate ? this.candidateLanguagesConfigDiaLog.candidateLanguage.alias : '',
        [Validators.required]
      ],
      note: [
        isUpdate ? this.candidateLanguagesConfigDiaLog.candidateLanguage.note : '',
      ],
      colorCode: [hexColor, [Validators.required]],
      colorCodeIp: [hexColor, [Validators.required]]
    });
  }

  eventHandler(event) {
    if((event.keyCode === 9 || event.keyCode === 13) && !getFormControlValue(this.form,'name')){
      this.formControls.name.patchValue(event.target.value);
    }
  }
  private resetForm() {
    this.initForm();
    this.submitted = false;
  }
  private handleSave(payload: CandidateLanguage, action: ActionEnum, isClose: boolean) {
    if (action === ActionEnum.CREATE) {
      this.subs.add(
        this._candidatelanguage.create(payload).subscribe(res => {
          this.isLoading = res.loading;
          if (res.loading) return;
          this.doSave(res,isClose);
        })
      );
      return;
    }

    this.subs.add(
      this._candidatelanguage.update(payload).subscribe(res => {
        this.isLoading = res.loading;
        this.checkAndClosePopup(res)
      })
    );
  }
  private doSave(res,isClose:boolean) {
    this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CREATE_SUCCESS, res.result.name);
    if(res.success && isClose){
      this.checkAndClosePopup(res);
      return;
    }
    this.resetForm();
  }
  checkAndClosePopup(res: ApiResponse<CandidateLanguage>) {
    if (!res.loading) this.ref.close(res);
  }

  onSave(isClose: boolean) {
    this.submitted = true;
    if (this.form.invalid) return;
    const action = this.candidateLanguagesConfigDiaLog.action;
    const payload: CandidateLanguage = {
      id: action === ActionEnum.UPDATE ? this.candidateLanguagesConfigDiaLog.candidateLanguage.id : 0,
      name: getFormControlValue(this.form, 'name'),
      colorCode: getFormControlValue(this.form, 'colorCode'),
      alias:getFormControlValue(this.form, 'alias'),
      note:getFormControlValue(this.form,'note')
    };
    this.handleSave(payload, action, isClose);
  }
}

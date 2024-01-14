import { distinctUntilChanged, debounceTime } from 'rxjs/operators';
import { Component, Injector, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { copyObject, getFormControlValue } from '@app/core/helpers/utils.helper';
import { CandidateApplyResult, CandidateApplyResultPayload, CandidateCapability, CandidateInterviewed, CandidateInterviewedPayload, CandidateInterviewLevel, CandidateInterviewLevelPayload, CandidateRequisiton, CandidatInterviewer, CurrentRequisition, HistoryChangeStatus, HistoryStatus } from '@app/core/models/candidate/candiadte-requisition.model';
import { CatalogModel } from '@app/core/models/common/common.dto';
import { RequisitionPayload, RequisitionStaff } from '@app/core/models/requisition/requisition.model';
import { CandidateInternService } from '@app/core/services/candidate/candidate-intern.service';
import { CandidateStaffService } from '@app/core/services/candidate/candidate-staff.service';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { RequisitionInternComponent } from '@app/modules/requisitiion/requisition-intern/requisition-intern.component';
import { RequisitionStaffComponent } from '@app/modules/requisitiion/requisition-staff/requisition-staff.component';
import { AppComponentBase } from '@shared/app-component-base';
import { DateFormat, MESSAGE } from '@shared/AppConsts';
import { ActionEnum, API_RESPONSE_STATUS, CANDIDATE_DETAILT_TAB_DEFAULT, REQUEST_CV_STATUS, ToastMessageType, UserType } from '@shared/AppEnums';
import * as moment from 'moment';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import * as _ from 'lodash';
import { MailDialogConfig, MailPreviewInfo } from '@app/core/models/mail/mail.model';
import { MailDialogComponent } from '@app/modules/admin/mail/mail-dialog/mail-dialog.component';
import { CapabilitySettingService } from '@app/core/services/categories/capability-setting.service';
import { CapabilityWithSetting } from '@app/core/models/categories/capabilities-setting.model';
import { ParamsGetScoreRange, ScoreRangeWithSetting } from '@app/core/models/categories/score-range-setting.model';
import { ScoreSettingService } from '@app/core/services/categories/score-setting.service';
import { ApiResponse } from '@shared/paged-listing-component-base';

@Component({
  selector: 'talent-current-requisition',
  templateUrl: './current-requisition.component.html',
  styleUrls: ['./current-requisition.component.scss']
})
export class CurrentRequisitionComponent extends AppComponentBase implements OnInit {
  @Input() userType: number;
  @Input() candidateId: number;
  @Input() _candidate: CandidateInternService | CandidateStaffService;

  public readonly DATE_FORMAT = DateFormat
  public readonly DEBOUNE_1S = 1000;
  public readonly STATUS_DISABLED = 'DISABLED';
  public readonly REQUEST_CV_STATUS = REQUEST_CV_STATUS;

  public listRequestStatus = [...this._utilities.catReqCvStatus];
  public listCapabilityFactor = [];
  candidateRequisiton: CandidateRequisiton;
  originalApprResultFormData: CandidateApplyResult;
  originalInterviewLevelFormData: CandidateInterviewLevel;
  headerCurrentReq: string = 'Current Requisition';
  editingRowKey: { [s: string]: boolean } = {};
  isEditingAll = false;
  isEditingFactors = false;
  isAddingInterviewer = false;
  requisitionDetail: CurrentRequisition;
  ref: DynamicDialogRef;

  capabilitySettings: CapabilityWithSetting[] = [];
  scoreRangeResults: ScoreRangeWithSetting[];
  clonedCanCapability: { [s: string]: CandidateCapability; } = {};
  isLoadingCapabilityTable = false;
  isLoadingSendMail = false;

  form: FormGroup;
  submitted = false;
  isApplyResultEditing = false;
  isApplyInterviewLevel = false;
  showModalMailDetail = false;
  catInterviewer: CatalogModel[];
  isInterviewed;
  visible: boolean;
  position: string;

  optionFailInternLevel = [
    {
      defaultName: "Fail",
      id: 101,
      salary: 0,
      shortName: "F",
      standardName: "Fail"
    }
  ]
  catInternLevels = this.optionFailInternLevel.concat(this._utilities.catLevelFinalIntern).filter(item => item.defaultName !== "FresherPlus");
  catStaffLevels = this._utilities.catLevelFinalStaff.filter(item => item.id !== 100);

  //permission
  PS_EditSalaryIntern: string = this.PS.Pages_CandidateIntern_ViewDetail_RequestCV_EditSalary;
  PS_EditSalaryStaff: string = this.PS.Pages_CandidateStaff_ViewDetail_RequestCV_EditSalary;
  PS_ViewEditInterviewTimeIntern: string = this.PS.Pages_CandidateIntern_ViewDetail_RequestCV_ViewEditInterview;
  PS_ViewEditInterviewTimeStaff: string = this.PS.Pages_CandidateStaff_ViewDetail_RequestCV_ViewEditInterview;
  PS_EditInterviewLevel: string = this.PS.Pages_CandidateStaff_ViewDetail_RequestCV_EditInterviewLevel;
  PS_EditApplicationStatusIntern: string = this.PS.Pages_CandidateStaff_ViewDetail_RequestCV_EditApplicationStatus;
  PS_EditApplicationStatusStaff: string = this.PS.Pages_CandidateIntern_ViewDetail_RequestCV_EditApplicationStatus;
  PS_EditFactorCapabilityResultIntern: string = this.PS.Pages_CandidateIntern_ViewDetail_RequestCV_EditFactorCapabilityResult;
  PS_EditFactorCapabilityResultStaff: string = this.PS.Pages_CandidateStaff_ViewDetail_RequestCV_EditFactorCapabilityResult;
  private dialogRef: DynamicDialogRef;
  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
    public _dialog: DialogService,
    public _capSetting: CapabilitySettingService,
    private fb: FormBuilder,
    private dialogService: DialogService,
    private _scoreSettingService: ScoreSettingService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.interviewerMaper();
    this.initForm();
    this.getCanRequisitionData();
    this._utilities.loadCatalogForCategories();
    }
  ngOnDestroy(): void {
    super.ngOnDestroy();
    if (this.dialogRef) this.dialogRef.close()
  }
  get requisitonForm() { return this.form.get('requisitonForm'); }
  get interviewForm() { return this.form.get('interviewForm') as FormGroup; }
  get applyResultForm() { return this.form.get('applyResultForm') as FormGroup; }
  get applyResultFormControls() { return (this.form.get('applyResultForm') as FormGroup).controls; }
  get applyHistoryStatuses() { return this.applyResultForm.get('historyStatuses') as FormArray; }
  get applyHistoryChangeStatuses() { return this.applyResultForm.get('historyChangeStatuses') as FormArray; }
  get mailStatusHistories() { return this.applyResultForm.get('mailDetail')?.value?.mailStatusHistories; }
  get isAllowSendMail() { return this.applyResultForm.get('mailDetail')?.value?.isAllowSendMail; }
  get isSentMailStatus() { return this.applyResultForm.get('mailDetail')?.value?.isSentMailStatus; }
  get interviewLevelForm() { return this.form.get('interviewLevelForm') as FormGroup; }

  onToggleAddInterviewer() {
    this.isAddingInterviewer = !this.isAddingInterviewer;
  }

  setTimeInterviewChange(date: Date) {
    this.candidateRequisiton.interviewTime = date.toString();
  }

  showDialogScoreRanges(position: string) {
    this.visible = true;
    this.position = position
  }

  onTimeInterviewChange(date: Date) {
    const payload = {
      requestCVId: this.candidateRequisiton.id,
      interviewTime: date && moment(date).format(DateFormat.YYYY_MM_DD_HH_MM_SS)
    }

    this.subs.add(
      this._candidate.updateInterviewTime(payload).subscribe(res => {
        this.isLoading = res.loading;
        if (!res.loading && res.success) {
          this.candidateRequisiton.interviewTime = date.toString();
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS);
        }
      })
    );
  }

  onInterviewerChange(id: number) {
    this.interviewForm.get('interviewId').setValue(id);
  }

  onApplyResultFinalLevelChange(id: number) {
    if (this.userType === UserType.INTERN) {
      const item = this._utilities.catInternSalary.find(item => item.id === id);
      const salary = item ? item.salary : 0;
      this.applyResultForm.get('salary').setValue(salary);
    }
  }

  onSendMail() {
    if (!this.candidateRequisiton.id) return;

    if (this.isSentMailStatus) {
      this.confirmationService.confirm({
        message: '<strong class=text-danger>This mail sent already, do you want to send again?</strong> ',
        header: 'Warning',
        icon: 'pi pi-exclamation-triangle',
        accept: () => this.handleSendMail()
      })
      return;
    }

    this.handleSendMail();
  }
  
  getButtonLabel(userType: UserType): string {
    return userType === UserType.INTERN ? 'Create LMS Account' : 'Create Contest';
  }

  toggleEditingApplyResult() {
    this.isApplyResultEditing = !this.isApplyResultEditing;
    if (!this.isApplyResultEditing) {
      this.applyResultForm.patchValue(this.originalApprResultFormData, { emitEvent: false });
    }

    !this.isApplyResultEditing ? this.applyResultForm.disable() : this.applyResultForm.enable();

    const hasPermission = this.validPermissionUserType(this.userType, this.PS_EditSalaryIntern, this.PS_EditSalaryStaff)
    if (this.userType === UserType.INTERN || !hasPermission) {
      this.applyResultForm.get('salary').disable();
    }
    if ((this.userType !== UserType.INTERN && !this.permission.isGranted(this.PS_EditApplicationStatusIntern))) {
      this.applyResultForm.get('status').disable();
    }
    if ((this.userType !== UserType.STAFF && !this.permission.isGranted(this.PS_EditApplicationStatusStaff))) {
      this.applyResultForm.get('status').disable();
    }
  }

  toggleEditingInterviewLevel() {
    this.isApplyInterviewLevel = !this.isApplyInterviewLevel;
    if (!this.isApplyInterviewLevel) {
      this.interviewLevelForm.patchValue(this.originalInterviewLevelFormData, { emitEvent: false });
    }

    !this.isApplyInterviewLevel ? this.interviewLevelForm.disable() : this.interviewLevelForm.enable();

    if (!this.permission.isGranted(this.PS_EditInterviewLevel)) {
      this.interviewLevelForm.get('interviewLevel').disable();
    }
  }

  createLMSAccount() {
    this.confirmationService.confirm({
      message: ` <div>Create account for <strong>${this.candidateRequisiton.cvName} </strong>
        in <span class=text-success>LMS tool ?</span></div>`,
      header: 'Create LMS account',
      icon: 'pi pi-exclamation-circle',
      accept: () => {
        this.subs.add(
          this._candidate.createLMSAccount(this.candidateId, this.candidateRequisiton.id).subscribe(res => {
            this.isLoading = res.loading;

            if (!res.loading && res.success) {
              this.applyResultForm.get('lmsInfo').setValue(res.result);
              this.originalApprResultFormData = this.applyResultForm.getRawValue();
              this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CREATE_SUCCESS, 'LMS Account');
            }
          })
        );
      },
    })
  }

  addCurentReq() {
    if (!this.candidateRequisiton?.currentRequisition) {
      this.handleAddCurrentReq();
      return;
    }

    this.confirmationService.confirm({
      message: `A requisition exists, do you want to add new?`,
      header: 'Confirmation',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.handleAddCurrentReq();
        return;
      },
    });
  }

  addInterviewer() {
    this.submitted = true;
    if (this.interviewForm.invalid) return;

    const payload = {
      interviewerId: getFormControlValue(this.interviewForm as FormGroup, 'interviewId'),
      requestCvId: this.candidateRequisiton.id
    }

    this.subs.add(
      this._candidate.addInterviewer(payload).subscribe(res => {
        this.isLoading = res.loading;
        if (!res.loading && res.success) {
          this.candidateRequisiton.interviewCandidate.unshift(res.result);
          this.onResetInterviewForm();

          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.ADD_SUCCESS);
        }
      })
    );
  }

  editCapability(item: CandidateCapability) {
    this.clonedCanCapability[item.id] = copyObject(item);
  }

  saveManyCapability(payload: CandidateCapability[]) {
    this.subs.add(
      this._candidate.updateManyCapabilityCV(payload).subscribe(res => {
        this.isLoadingCapabilityTable = res.loading;
        if (!res.loading && res.success) {
          this.candidateRequisiton.capabilityCandidate = _.unionBy(this.candidateRequisiton.capabilityCandidate, res.result, 'id');
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, `${res.result.length} Capabilities `);
          this.getCanRequisitionData();
        }
      })
    );
  }
  saveManyFactors(payload: CandidateCapability[]) {
    this.subs.add(
      this._candidate.updateManyFactorsCapabilityCV(payload).subscribe(res => {
        this.isLoadingCapabilityTable = res.loading;
        if (!res.loading && res.success) {
          this.candidateRequisiton.capabilityCandidate = _.unionBy(this.candidateRequisiton.capabilityCandidate, res.result, 'id');
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, `${res.result.length} Capabilities `);
        }
      })
    );
  }

  saveCapability(entity: CandidateCapability) {
    const { id, requestCvId, capabilityId, capabilityName, score, note, factor } = entity;
    const payload = { id, requestCvId, capabilityId, capabilityName, score, note, factor };

    this.subs.add(
      this._candidate.updateCapabilityCV(payload).subscribe(res => {
        this.isLoadingCapabilityTable = res.loading;

        if (!res.loading && res.success) {
          const candidateCaps = this.candidateRequisiton?.capabilityCandidate;
          const idx = candidateCaps.findIndex(item => item.id === payload.id);
          candidateCaps[idx] = res.result;
          this.clonedCanCapability[payload.id] = copyObject(entity);
          this.removeEditingCapaRow(payload.id);
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.capabilityName);
          this.adjustInterviewlevel();
        }
      })
    );
    this.totalScore();
  }

  onRequestStatusChange(id: number) {
    const hasRequired = this.applyResultForm.get('onboardDate').hasValidator(Validators.required);
    if (id !== REQUEST_CV_STATUS.Onboarded && hasRequired) {
      this.applyResultForm.get('onboardDate').removeValidators(Validators.required);
      this.applyResultForm.get('onboardDate').updateValueAndValidity();
      return;
    }
    if (id === REQUEST_CV_STATUS.Onboarded) {
      this.applyResultForm.get('onboardDate').setValidators([Validators.required]);
      this.applyResultForm.get('onboardDate').updateValueAndValidity();
    }
  }

  private removeEditingCapaRow(id: number) {
    this.editingRowKey[id] = false;
    const values = Object.keys(this.editingRowKey).filter(key => this.editingRowKey[key] === true);
    if (values.length === 0) this.isEditingAll = false;
  }

  saveApplyResult() {
    this.submitted = true;
    if (this.applyResultForm.invalid) return;

    const payload = this.getPayloadApplyResult();
    this._candidate.updateApplicationResult(payload).subscribe(res => {
      if (!res) return;

      if (!res.loading && res.success) {
        this.updateValueApplyResultForm(res.result);
        this.onResetApplyResultForm();
        this._candidate.setCurrentReqUpdated(true);
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS);
      }
    })
  }

  saveInterviewLevel() {
    this.submitted = true;
    if (this.interviewLevelForm.invalid) return;
    const payload = this.getPayloadApplyInverviewLevel();
    if(payload.interviewLevel !=null){
      this._candidate.updateInterviewLevel(payload).subscribe(res => {
        if (!res) return;
        if (!res.loading && res.success) {
          this.onResetInterviewLevelForm();
          this._candidate.setCurrentReqUpdated(true);
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS);
          this.adjustInterviewlevel();
        }
      })
    }else {
      this.showToastMessage(ToastMessageType.WARN, 'Interview level cannot be left blank');
    }
  }

  adjustInterviewlevel(){
    this.subs.add(
      this._candidate.getCurentReqByCanId(this.candidateId).subscribe(rs => {
        this.isLoading = rs.loading;
        if (rs.success) {
            const isinterviewLevel = rs.result?.interviewLevel;
            this.updateValueInterviewLevelForm(isinterviewLevel);
            this.isInterviewed =  rs.result.interviewed.interviewed;
            this.saveInterviewed();
        }
      })
    );
  }

  deleteCurentReq() {
    this.isEditingAll = false;
    this.isEditingFactors = false;
    const deleteRequest = this._candidate.deleteReqCV(this.candidateRequisiton.id);
    const currentReq = this.candidateRequisiton.currentRequisition;
    const header = `${currentReq.userTypeName} - ${currentReq.subPositionName}`
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, header).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.candidateRequisiton && (this.candidateRequisiton = null);
          this._candidate.setCurrentReqUpdated(true);
          this.headerCurrentReq = `Current Requisition`;
          this.getCanRequisitionData();
          this.isInterviewed = false;
        } else {
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.DELETE_FAILED);
        }
      })
    );
  }

  deleteInterview(entity: CandidatInterviewer) {
    const deleteRequest = this._candidate.deleteRequestCVInterview(entity.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, entity.interviewName).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          if (this.candidateRequisiton?.interviewCandidate) {
            let interviewCandidate = this.candidateRequisiton.interviewCandidate;
            this.candidateRequisiton.interviewCandidate = interviewCandidate.filter(interview => interview.id !== entity.id)
          };
        } else {
          this.showToastMessage(ToastMessageType.ERROR, MESSAGE.DELETE_FAILED);
        }
      })
    );
  }

  onResetCapability(entity: CandidateCapability) {
    const idx: number = this.candidateRequisiton.capabilityCandidate.findIndex(item => item.id === entity.id);
    this.candidateRequisiton.capabilityCandidate[idx] = this.clonedCanCapability[entity.id];
    this.removeEditingCapaRow(entity.id);
  }

  navigateToRequestDetail() {
    const IsCurrentRequisition = this.candidateRequisiton?.currentRequisition?.id
    if(!IsCurrentRequisition)
    {
      return;
    }
    const reqPath = this.userType === UserType.INTERN ? 'req-intern' : 'req-staff';
    const url = `/app/requisition/${reqPath}/${this.candidateRequisiton.currentRequisition.id}?type=${this.userType}`;
    window.open(url, '_blank');
    }
   
  private handleSendMail() {
    this._candidate.getPreviewRequestCvMail(this.candidateRequisiton.id).subscribe(res => {
      if (!res.success || res.loading) return;

      const data: MailDialogConfig = { mailInfo: res.result, showEditBtn: true }
      const dialogRef = this._dialog.open(MailDialogComponent, {
        showHeader: false,
        width: '70%',
        contentStyle: { 'background-color': 'rgba(242,245,245)', overflow: 'visible' },
        baseZIndex: 5000,
        data: data
      });

      dialogRef.onClose.subscribe((mailInfo: MailPreviewInfo) => {
        if (!mailInfo) return;

        this._candidate.sendMailRequestCV(mailInfo, this.candidateRequisiton.id).subscribe(res => {
          this.isLoadingSendMail = res.loading;
          if (res?.success) {
            this.showToastMessage(ToastMessageType.SUCCESS, 'Send successfully!');
            this.candidateRequisiton.applicationResult.mailDetail = res.result;
            this.applyResultForm.get('mailDetail').setValue(res.result, { emitEvent: false });
            this.originalApprResultFormData = this.applyResultForm.getRawValue();
          }
        });
      });
    })
  }

  private getCanRequisitionData() {
    this.subs.add(
      this._candidate.getCurentReqByCanId(this.candidateId).subscribe(rs => {
        this.isLoading = rs.loading;
        if (rs.success) {
          this.candidateRequisiton = rs.result;
          const currentRequisition = this.candidateRequisiton?.currentRequisition;
          const appilicationResult = this.candidateRequisiton?.applicationResult;
          const interviewLevel = this.candidateRequisiton?.interviewLevel;
          this.updateValueRequisitionForm(currentRequisition);
          this.updateValueInterviewTime(this.candidateRequisiton);
          this.updateValueApplyResultForm(appilicationResult);
          this.updateValueInterviewLevelForm(interviewLevel);
          setTimeout(() => this.listenFragment())
          this.OnReqCvStatus();
          this.totalScore();
          if (this.candidateRequisiton)
          {
           this.isInterviewed = this.candidateRequisiton.interviewed.interviewed;
           this.saveInterviewed();
          }
        }
      })
    );
  }

  onEditAllCapability() {
    this.isEditingAll = true;
    this.isEditingFactors = false;
    this.candidateRequisiton?.capabilityCandidate.forEach(item => {
      this.clonedCanCapability[item.id] = copyObject(item);
      this.editingRowKey[item.id] = true;
    })
  }

  onSaveAllCapability() { 
    this.isEditingAll = false;
    const payload = [];
    const canCapabilities = this.candidateRequisiton?.capabilityCandidate;
    canCapabilities.filter(item => this.editingRowKey[item.id] === true).forEach(item => {
      const { id, score, note } = item;
      payload.push({ id, score, note });
    })
        
    canCapabilities.forEach(item => this.editingRowKey[item.id] = false)
    this.saveManyCapability(payload);
    this.totalScore();
  }
  onCancelAllCapability() {
    this.isEditingAll = false;
    this.candidateRequisiton?.capabilityCandidate.forEach(item => this.onResetCapability(item))
  }

  onEditFactors() {
    this.isEditingFactors = true;
    this.isEditingAll = false;
    this.candidateRequisiton?.capabilityCandidate.forEach(item => {
      this.clonedCanCapability[item.id] = copyObject(item);
    })
  }

  onSaveFactors() {
    this.isEditingAll = false;
    this.isEditingFactors = false;
    const payload = [];
    const canCapabilities = this.listCapabilityFactor;
    canCapabilities.forEach(item => {
      const { id, factor } = item;
      payload.push({ id, factor });
    })
    this.saveManyFactors(payload);
    this.listCapabilityFactor = []
    this.totalScore();
  }
  onCancelFactors() {
    this.isEditingFactors = false;
    this.listCapabilityFactor = [];
    this.candidateRequisiton?.capabilityCandidate.forEach(item => this.onResetCapability(item));
  }
  private initForm() {
    const requisitonForm = this.fb.group({
      priority: null,
      requestStatus: '',
      branchId: null,
      quantity: null,
      userType: null,
      timeNeed: '',
      subPositionId: null,
      updatedName: '',
      level: null,
      updatedTime: '',
      skills: '',
      note: ''
    });

    const interviewForm = this.fb.group({
      interviewId: '',
      interviewName: ['', [Validators.required]],
      interviewTime: {
        value: null,
        disabled: !this.validPermissionUserType(this.userType, this.PS_ViewEditInterviewTimeIntern, this.PS_ViewEditInterviewTimeStaff)
      },
    })

    const applyResultForm = this.fb.group({
      status: null,
      historyStatuses: this.fb.array([]),
      historyChangeStatuses: this.fb.array([]),
      applyLevel: null,
      applyLevelName: '',
      finalLevel: null,
      finalLevelName: '',
      salary: null,
      onboardDate: null,
      hrNote: '',
      mailDetail: null,
      lmsInfo: null,
      percentage: '',
    })

    const interviewLevelForm = this.fb.group({
      interviewLevel: null,
      interviewLevelName: ''
    })

    this.form = this.fb.group({
      id: 0,
      cvName: '',
      creationTime: '',
      requisitonForm: requisitonForm,
      interviewForm: interviewForm,
      applyResultForm: applyResultForm,
      interviewLevelForm: interviewLevelForm,
    });

    this.interviewForm.get('interviewTime').valueChanges.pipe(
      distinctUntilChanged(), debounceTime(this.DEBOUNE_1S)).subscribe(date => {
        this.onTimeInterviewChange(date);
      })

    this.applyResultForm.get('status').valueChanges.subscribe(status => {
      this.onRequestStatusChange(status);
    })

    this.interviewLevelForm.get('interviewLevel').valueChanges.subscribe(interviewLevel => {
      this.onRequestStatusChange(interviewLevel);
    })

    this.requisitonForm.disable();
    this.applyResultForm.disable();
    this.interviewLevelForm.disable();
  }

  private onResetInterviewForm() {
    this.submitted = false;

    const value = this.interviewForm.getRawValue();
    this.interviewForm.reset({
      ...value,
      interviewName: '',
    }, { emitEvent: false })
  }

  private getHistoryStatusFormGroup(entity: HistoryStatus) {
    return this.fb.group({
      status: entity.status ?? null,
      statusName: entity.statusName ?? '',
      timeAt: entity.timeAt ?? null
    })
  }

  private getHistoryChangeStatusFormGroup(entity: HistoryChangeStatus) {
    return this.fb.group({
      fromStatus: entity.fromStatus ?? null,
      fromStatusName: entity.fromStatusName ?? '',
      toStatus: entity.toStatus ?? null,
      toStatusName: entity.toStatusName ?? '',
      timeAt: entity.timeAt ?? null
    })
  }


  private onResetApplyResultForm() {
    this.submitted = false;
    this.isApplyResultEditing = false;
    this.applyResultForm.disable();
  }

  private onResetInterviewLevelForm() {
    this.submitted = false;
    this.isApplyInterviewLevel = false;
    this.interviewLevelForm.disable();
  }

  private updateValueApplyResultForm(applyResult: CandidateApplyResult) {
    applyResult && (this.candidateRequisiton.applicationResult = applyResult);
    if (applyResult) {
      this.applyResultForm.patchValue({
        ...applyResult,
        onboardDate: applyResult.onboardDate ? new Date(applyResult.onboardDate) : null,
      });

      this.applyHistoryStatuses.clear();
      const historyStatuses = applyResult.historyStatuses;
      historyStatuses.forEach(item => {
        this.applyHistoryStatuses.push(this.getHistoryStatusFormGroup(item))
      })

      this.applyHistoryChangeStatuses.clear();
      const historyChangeStatuses = applyResult.historyChangeStatuses;
      historyChangeStatuses.forEach(item => {
        this.applyHistoryChangeStatuses.push(this.getHistoryChangeStatusFormGroup(item))
      })
    }
    this.originalApprResultFormData = this.applyResultForm.getRawValue();
  }

  private updateValueInterviewLevelForm(interviewLevel: CandidateInterviewLevel) {
    interviewLevel && (this.candidateRequisiton.interviewLevel = interviewLevel);
    if (interviewLevel) {
      this.interviewLevelForm.patchValue(interviewLevel);
    }
    this.originalInterviewLevelFormData = this.interviewLevelForm.getRawValue();
  }

  private updateValueInterviewTime(candidateRequisiton: CandidateRequisiton) {
    if (candidateRequisiton) {
      const interviewTime = candidateRequisiton?.interviewTime ? new Date(candidateRequisiton.interviewTime) : null
      this.interviewForm.get('interviewTime').patchValue(interviewTime, { emitEvent: false });
    }
  }

  private updateValueRequisitionForm(currentRequisition: CurrentRequisition) {
    this.requisitionDetail = currentRequisition;
    if(this.requisitionDetail){
      this.loadScoreRanges(this.requisitionDetail.userType, this.requisitionDetail.subPositionId);
    } 
    this.headerCurrentReq = `Current Requisition ${currentRequisition?.id ? '#' + currentRequisition?.id : ''} `;
    this.requisitonForm.patchValue(currentRequisition);
  }

  loadScoreRanges(userType?: number, subPositionId?: number) {
    const params: ParamsGetScoreRange = {
      userType: userType,
      subPositionId: subPositionId
    }
    this._scoreSettingService.getAllRange(params).subscribe((rs: ApiResponse<ScoreRangeWithSetting[]>) => {
      if (rs.success) {
        this.scoreRangeResults = rs.result;
      }
    })
  }

  private interviewerMaper() {
    if (this._utilities?.catAllUser) {
      this.catInterviewer = this._utilities.catAllUser.map(item => {
        return {
          id: item.id,
          name: `${item.fullName} (${item.email})`
        }
      });
    }
  }

  private getPayloadApplyResult(): CandidateApplyResultPayload {
    const onboardValue = this.applyResultForm.controls['onboardDate'].value;
    const payload = {
      requestCvId: this.candidateRequisiton.id,
      status: getFormControlValue(this.applyResultForm, 'status'),
      applyLevel: getFormControlValue(this.applyResultForm, 'applyLevel'),
      finalLevel: getFormControlValue(this.applyResultForm, 'finalLevel'),
      salary: getFormControlValue(this.applyResultForm, 'salary'),
      onboardDate: onboardValue && moment(onboardValue).format(DateFormat.YYYY_MM_DD),
      hrNote: getFormControlValue(this.applyResultForm, 'hrNote'),
      lmsInfo: getFormControlValue(this.applyResultForm, 'lmsInfo'),
      percentage: getFormControlValue(this.applyResultForm, "percentage")
    }

    if (this.userType === UserType.INTERN) {
      payload.applyLevel = payload.finalLevel;
    }
    return payload;
  }

  getPayloadApplyInverviewLevel(): CandidateInterviewLevelPayload {
    const payload = {
      requestCvId: this.candidateRequisiton.id,
      interviewLevel: getFormControlValue(this.interviewLevelForm, 'interviewLevel'),
    }
    return payload;
  }

  private handleAddCurrentReq() {
    const presentrequestId= this.candidateRequisiton?.currentRequisition.id
    const requisitionComponent = this.userType === UserType.INTERN ? RequisitionInternComponent : RequisitionStaffComponent
    const subHeader = this.userType === UserType.INTERN ? 'Intern' : 'Staff'
    const dialogRef = this._dialog.open(requisitionComponent, {
      header: `Select Requisition ${subHeader}`,
      width: "95%",
      contentStyle: { "max-height": "100%", overflow: "auto" },
      baseZIndex: 10000,
      data: { dialogMode: ActionEnum.SELECT },
    });

    dialogRef.onClose.subscribe((entity: RequisitionStaff) => {
      const isHiddenPresentForHr = localStorage.getItem('presentForHr');
      if (entity?.id) {
        const payload : RequisitionPayload = { 
          cvId: this.candidateId,
          requestId: entity.id, 
          currentRequestId: presentrequestId ,
          presentForHr: isHiddenPresentForHr 
        }
          this.subs.add(
            this._candidate.createReqCV(payload).subscribe(res => {
              if (!res.loading && res.success) {
                this.candidateRequisiton = res.result;

                const currentRequisition = this.candidateRequisiton?.currentRequisition;
                const appilicationResult = this.candidateRequisiton?.applicationResult;
                const interviewLevel = this.candidateRequisiton?.interviewLevel;

                this.updateValueRequisitionForm(currentRequisition);
                this.updateValueInterviewTime(this.candidateRequisiton);
                this.updateValueApplyResultForm(appilicationResult);
                this.updateValueInterviewLevelForm(interviewLevel);
                this._candidate.setCurrentReqUpdated(true);
                this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.ADD_SUCCESS);
              }
            })
          );
      }
    });
  }

  private listenFragment() {
    this._candidate.getFragment$().subscribe(res => res && this.scrollToView(res));
  }

  private scrollToView(data: { tab: number, fragment: string }) {
    if (data.tab !== CANDIDATE_DETAILT_TAB_DEFAULT.CURRENT_REQ) return;
    const el = document.getElementById(data.fragment);
    el.scrollIntoView({ behavior: 'smooth' });
  }
  OnReqCvStatus() {
    if (getFormControlValue(this.applyResultForm, "status") == this.REQUEST_CV_STATUS.ScheduledInterview ||
      getFormControlValue(this.applyResultForm, "status") == this.REQUEST_CV_STATUS.RejectInterview ||
      getFormControlValue(this.applyResultForm, "status") == this.REQUEST_CV_STATUS.FailedInterview) {
      this.listRequestStatus = [...this._utilities.catReqCvStatus.filter(value => value.id > 3 && value.id < 8)]
    }
    else {
      this.listRequestStatus = [...this._utilities.catReqCvStatus]
    }
  }
  totalScore(): string {
    let totalPoint: number = 0;
    let totalFactor: number = 0;
    if (this.candidateRequisiton && this.candidateRequisiton.capabilityCandidate) {
      const capability = [...this.candidateRequisiton.capabilityCandidate];
      const reviewerCapability = capability.filter(x => !x.fromType);
      for (let i = 0; i < reviewerCapability.length; i++) {
        totalPoint += reviewerCapability[i].score * reviewerCapability[i].factor;
        totalFactor += reviewerCapability[i].factor;
      }
      return (totalPoint / totalFactor).toFixed(2);
    }
    return '';
  }

  public display: boolean = false;
  public guideLineTitle: string = '';
  public guideLineContent: string = '';
  showDialog(capabilityId: number) {
    const payload = { userType: this.userType, subPositionId: this.candidateRequisiton.currentRequisition.subPositionId }
    this._capSetting.getCapabilitiesByUserTypeAndPositionId(payload).subscribe((res) => {
      this.capabilitySettings = res.result;
      if (this.capabilitySettings !== undefined) {
        const capabilitySetting = this.capabilitySettings.find(cap => cap.capabilityId === capabilityId);
        this.guideLineTitle = capabilitySetting.capabilityName;
        this.guideLineContent = capabilitySetting.guideLine || "We will update soon!";
        this.display = true;
      }
    });
  }

  isGrantedEditFactor(): boolean {
    if ((this.isGranted(this.PS_EditFactorCapabilityResultIntern) && this.userType === UserType.INTERN)
      ||
      (this.isGranted(this.PS_EditFactorCapabilityResultStaff) && this.userType === UserType.STAFF)) {
      return true;
    }
    return false;
  }

  onChangeFactor(item: CandidateCapability) {
    const oldCandidateCapability = this.listCapabilityFactor.find(value => value.id == item.id);
    if (!oldCandidateCapability) {
      this.listCapabilityFactor.push({
        ...item
      });
    }
    else {
      oldCandidateCapability.factor = item.factor;
    }
  }

  saveInterviewed() {
    this.submitted = true;
    const payload = {
      requestCvId: this.candidateRequisiton.id,
      interviewed: this.isInterviewed,
    }
    this._candidate.updateInterviewed(payload).subscribe(res => {
      if (!res) return;
      if (!res.loading && res.success) {
        this._candidate.setCurrentReqUpdated(true);
      }
    })
  }
}

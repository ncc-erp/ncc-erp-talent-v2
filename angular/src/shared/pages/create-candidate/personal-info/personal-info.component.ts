import { Component, EventEmitter, HostListener, Injector, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { checkNumber, getFormControlValue, isCVExtensionAllow, isImageExtensionAllow } from '@app/core/helpers/utils.helper';
import { CustomValidators } from '@app/core/helpers/validator.helper';
import {CandidateLanguageConfigDiaLog} from '@app/core/models/candidate/candidate-language.model';
import { Candidate, CandidatePayload, MailStatusHistory } from '@app/core/models/candidate/candidate.model';
import { MailDialogConfig, MailPreviewInfo } from '@app/core/models/mail/mail.model';
import { CandidateInternService } from '@app/core/services/candidate/candidate-intern.service';
import { CandidateStaffService } from '@app/core/services/candidate/candidate-staff.service';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { MailDialogComponent } from '@app/modules/admin/mail/mail-dialog/mail-dialog.component';
import { AppComponentBase } from '@shared/app-component-base';
import { DateFormat, MESSAGE } from '@shared/AppConsts';
import { CANDIDATE_DETAILT_TAB_DEFAULT, CV_REFERENCE_TYPE, SearchType, ToastMessageType, UserType } from '@shared/AppEnums';
import { UploadAvatarComponent } from '@shared/components/upload-avatar/upload-avatar.component';
import { AppSessionService } from '@shared/session/app-session.service';
import * as moment from 'moment';
import { DialogService } from 'primeng/dynamicdialog';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'talent-personal-info',
  templateUrl: './personal-info.component.html',
  styleUrls: ['./personal-info.component.scss']
})
export class PersonalInfoComponent extends AppComponentBase implements OnInit {
  readonly ACCEPT_IMAGE = '.png, .jpg, .jpeg, .gif';
  readonly ACCEPT_CV = '.doc, .docx, .xlsx, .csv, .pdf';
  readonly DEBOUNCE_TIME = 200; //0.2s
  readonly REFERT_TYPE = CV_REFERENCE_TYPE;
  readonly DATE_FORMAT = DateFormat;
  readonly FORM_DISABLED = 'DISABLED';

  @Input() userType: number;
  @Input() candidateId: number = null;
  @Input() _candidate: CandidateInternService | CandidateStaffService;
  @Input() isViewMode: boolean;
  @Input() tabActived: number;

  @Input() candidate: Candidate;

  @Output() activeChange = new EventEmitter<number>();
  @Output() onCreateCandidate = new EventEmitter<number>();
  @Output() candidateChange = new EventEmitter<Candidate>();

  public form: FormGroup;
  originalFormData: Candidate;
  isSearchAnd: boolean = false;
  cvFile: File;
  cvUrl: string;
  cvFileName: string;
  avatarUrl: string;
  avatarFile: File;
  imageChangedEvent;
  referenceType: CV_REFERENCE_TYPE;

  catalogConfig = {
    labelName: '',
    catalogList: this._utilities.catLanguages,
    optionLabel: 'name',
    optionValue: 'id'
  }

  public dataApplyCv = this.getDataApplyCv("idApplyCV");

  @HostListener("window:unload", ["$event"])  
   unloadHandler(event)
   { this.setDataApplyCv(this.dataApplyCv);   }
    setDataApplyCv(data: any) {
    localStorage.setItem("idApplyCV", JSON.stringify(data));
  }

  isAvata: boolean;
  isApplyCV: boolean;
  submitted = false;
  isEnaleRefBy = false;
  isEditing = false;

  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
    private _fb: FormBuilder,
    private _dialog: DialogService,
    private _apSession: AppSessionService
    ) {
    super(injector);

  }

  ngOnInit(): void {
    this.initForm();
    if (this.candidate) {
      this.updateCandidateInfoData(this.candidate);
    }

    if (this.dataApplyCv){
      this.setValueApplyCV(this.dataApplyCv);
      const urlCv = this.dataApplyCv.attachCVLink?? null
      const applyCvlink= urlCv.slice(urlCv.indexOf('_',urlCv.indexOf('_') + 1) + 1)
      this.avatarUrl = this.dataApplyCv.avatarLink ?? null;
      this.cvFileName = applyCvlink;
    }
  }

  get formControls() {
    return this.form.controls;
  }

  setValueApplyCV(data: any) {
    this.form.patchValue({
      fullName: data?.fullName ,
      email: data?.email,
      isFemale: data?.isFemale,
      phone: data?.phone,
      userType: data?.positionType === "Intern" ? 0 : 1,
      branchId: data?.branchId,
      linkCV: data?.attachCVLink,
      avatar: data.avatarLink 
    });
  }
  
  getDataApplyCv(key: string): any {
    const data = localStorage.getItem(key);
    localStorage.clear()
    return data ? JSON.parse(data) : null;
  }

  toggleEditing() {
    this.isEditing = !this.isEditing;
    if (!this.isEditing) {
      this.form.patchValue(this.originalFormData, { emitEvent: false });
      this.cvFileName = this.getCvFileName(this.originalFormData);
    }
    (this.isViewMode && !this.isEditing) ? this.form.disable() : this.form.enable();
    this.checkEnableReferBy(getFormControlValue(this.form, 'cvSourceId')); 
  }

  onCVFileChange(fileList: FileList) {
    let file = fileList[0];

    this.cvFile = file;
    this.cvFileName = file?.name;
    if (!isCVExtensionAllow(file)) {
      this.formControls['linkCV'].setErrors({ invalidCVExtension: true });
      return;
    }
    const reader = new FileReader();
    reader.onload = (event) => {
      this.uploadFile(this.cvFile);
    }
    reader.readAsText(file);
  }

  onAvatarFileChange(event) {
    this.imageChangedEvent = event;
    const file = event.target.files[0];
    if (!isImageExtensionAllow(file)) {
      this.formControls['avatar'].setErrors({ invalidImageExtension: true });
      return;
    }
    const dialogRef = this._dialog.open(UploadAvatarComponent, {
      header: `Croping image`,
      width: "50%",
      contentStyle: { "max-height": "100%", overflow: "auto" },
      baseZIndex: 10000,
      data: { image: this.imageChangedEvent },
    });

    dialogRef.onClose.subscribe((result: { fileImageString: string, fileImage: File }) => {
      if (!result) return;
      const { fileImageString, fileImage } = result
      this.avatarUrl = fileImageString;
      this.avatarFile = fileImage;
      this.formControls['avatar'].patchValue(this.avatarUrl);
      this.uploadFile(this.avatarFile, true);
    });
  }

  onSaveClose() {
    this.submitted = true;
    if (this.form.invalid) return;

    const payload = this.getPayload();
    this._candidate.create(payload).subscribe(res => {
      this.isLoading = res.loading;
      if (!res.loading && res.success) {
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CREATE_SUCCESS);
        this.checkExistCreatedUserAndNavigate(true, null);
      }
    });
  }

  onSaveContinue() {
    this.submitted = true;
    if (this.form.invalid) return;

    const payload = this.getPayload();
    this._candidate.create(payload).subscribe(res => {
      this.isLoading = res.loading;

      if (!res.loading && res?.success) {
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CREATE_SUCCESS);
        this.tabActived++;
        this.onCreateCandidate.emit(res.result.id);
        this.activeChange.emit(this.tabActived);

        const jsonArray = JSON.parse(res.result.candidateLanguage);
        const json = jsonArray?.map(item => (item.id )
        );
        this.form.patchValue({
          ...res.result,
          candidateLanguage:json
        });
        this.isViewMode = true;
        (this.isViewMode && !this.isEditing) ? this.form.disable() : this.form.enable();
        this.originalFormData = this.form.getRawValue();

        this.checkExistCreatedUserAndNavigate(false, res.result.id);
      }
    });
  }

  moveToEducationStep(candidateId: number) {
    const candidatePath = this.userType === UserType.INTERN ? 'intern-list' : 'staff-list';
    this.router.navigate([`/app/candidate/${candidatePath}`, candidateId], {
      queryParams: { userType: this.userType, tab: CANDIDATE_DETAILT_TAB_DEFAULT.EDUCATION },
    });
  }

  onUpdate() {
    this.submitted = true;
    if (this.form.invalid) return;

    const payload = this.getPayload(true) as CandidatePayload;
    this._candidate.updateCV(payload).subscribe(res => {
      this.isLoading = res.loading;
      if (!res.loading && res.success && res.result) {
        this.updateCandidateInfoData(res.result);
        this.resetStatus();
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS);
      }
    })
  }

  onCVSourceChange(value: number) {
    this.isEnaleRefBy = this.checkEnableReferBy(value);
  }

  onPositionSelect(subPositionId: number) {
    this.formControls?.subPositionId?.setValue(subPositionId);
  }

  onClose() {
    this.backToCandidate();
  }

  sendMailFailed() {
    const isSentMailStatus = this.formControls.mailDetail?.value?.isSentMailStatus;
    if (isSentMailStatus) {
      this.confirmationService.confirm({
        message: '<strong class="text-danger">Failed email sent already, do you want to send again?</strong> ',
        header: 'Warning',
        icon: 'pi pi-exclamation-triangle',
        accept: () => this.handleSendMail()
      })
      return;
    }

    this.handleSendMail();
  }

  navigateToExisted(candExist: { cvId: number, userType: number }) {
    const routhPath = candExist.userType === UserType.INTERN ? 'intern-list' : 'staff-list'
    const url = this.router.createUrlTree([`/app/candidate/${routhPath}`, candExist.cvId],
      { queryParams: { userType: candExist.userType, tab: CANDIDATE_DETAILT_TAB_DEFAULT.PERSONAL_INFO } });
    window.open(url.toString(), '_blank')
  }

  navigateToApplicationResult() {
    this.activeChange.emit(CANDIDATE_DETAILT_TAB_DEFAULT.CURRENT_REQ);
    setTimeout(() => this._candidate.setFragment({ tab: this.tabActived, fragment: 'application-result-panel' }))
  }

  getLastEmailInfo(): MailStatusHistory {
    const mailStatuses: MailStatusHistory[] = this.formControls?.mailDetail?.value?.mailStatusHistories;
    return mailStatuses?.length ? mailStatuses[0] : null;
  }

  viewFile(link: string): string {
    return this.isViewMode ? this._utilities.getLinkFile(link) : null;
  }

  private handleSendMail() {
    this._candidate.getPreviewMailFailed(this.candidateId).subscribe(res => {
      if (!res.success || res.loading) return;
      const data: MailDialogConfig = { mailInfo: res.result, showEditBtn: true }
      const dialogRef = this._dialog.open(MailDialogComponent, {
        showHeader: false,
        width: '70%',
        contentStyle: { 'background-color': 'rgba(242,245,245)', overflow: 'visible' },
        baseZIndex: 10000,
        data: data
      });

      dialogRef.onClose.subscribe((mailInfo: MailPreviewInfo) => {
        if (!mailInfo) return;

        this._candidate.sendMailCV(mailInfo, this.candidateId).subscribe(res => {
          if (res?.success) {
            this.showToastMessage(ToastMessageType.SUCCESS, 'Send successfully!');
            this.formControls.mailDetail.setValue(res.result, { emitEvent: false });
            this.originalFormData = this.form.getRawValue();
          }
        });
      });
    })
  }

  private uploadFile(file: File, isAvatarFile: boolean = false) {
    if (!this.candidateId) return;

    const formData = new FormData();
    formData.append('CVId', getFormControlValue(this.form, 'id'));
    formData.append('FileUpdate', file);

    if (isAvatarFile) {
      this.subs.add(
        this._candidate.updateFileAvatar(formData).subscribe(res => {
          if (!res.loading && res.success) {
            this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, 'Candidate Photo');
          }
        })
      );
      return;
    }

    this.subs.add(
      this._candidate.updateFileCV(formData).subscribe(res => {
        if (!res.loading && res.success) {
          this.originalFormData.linkCV = res.result;
          this.formControls['linkCV'].patchValue(res.result);
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, 'Candidate CV');
        }
      })
    );
  }

  private initForm() {
    const userType = this.userType === UserType.INTERN ? UserType.INTERN : UserType.STAFF;
    this.form = this._fb.group({
      id: 0,
      fullName: ['', [Validators.required]],
      dob: [null, [CustomValidators.isDateMustLessThanCurrent()]],
      email: ['', [Validators.required, Validators.email]],
      isFemale: true,
      phone: ['', [Validators.required]],
      address: '',
      userType: [userType, [Validators.required]], //number
      note: '',
      subPositionId: [null, [Validators.required]],  //number
      branchId: [null, [Validators.required]],
      branchName: '',
      cvStatus: [this._utilities.catCvStatus[0].id, [Validators.required]],
      cvStatusName: '',
      linkCV: [null, []],
      cvSourceId: [null, [Validators.required]], //number
      candidateLanguage: [null, [Validators.required]],
      referenceId: ['', [Validators.required]],
      avatar: null,
      mailDetail: null
    });

    (this.isViewMode && !this.isEditing) ? this.form.disable() : this.form.enable();

    this.form.valueChanges.pipe(debounceTime(this.DEBOUNCE_TIME)).subscribe(res => {
      this.candidateChange.emit(this.form.getRawValue());
    })

    this.formControls.email.valueChanges.pipe(debounceTime(this.DEBOUNCE_TIME)).subscribe(email => {
      this.checkExistEmail(email);
    })

    this.formControls.phone.valueChanges.pipe(debounceTime(this.DEBOUNCE_TIME)).subscribe(phone => {
      this.checkExistPhone(phone);
    })
  }

  private checkExistEmail(email: string) {
    if (!email) return;
    this.subs.add(
      this._candidate.checkValidMail(this.candidateId, email).subscribe(res => {
        if (!res.loading && res.result) {
          this.formControls.email.setErrors({ 'existEmail': res.result })
        }
      })
    );
  }

  private checkExistPhone(phone: string) {
    if (!phone) return;
    this.subs.add(
      this._candidate.checkValidPhone(this.candidateId, phone).subscribe(res => {
        if (!res.loading && res.result) {
          this.formControls.phone.setErrors({ 'existPhone': res.result })
        }
      })
    );
  }

  private backToCandidate() {
    if (this.userType === UserType.INTERN) {
      return this.router.navigate(['/app/candidate/intern-list'])
    }
    return this.router.navigate(['/app/candidate/staff-list'])
  }

  private checkEnableReferBy(cvSourceId: number) {
    this.referenceType = this._utilities.catCvSource.find(item => item.id === cvSourceId)?.referenceType;
    if (cvSourceId && checkNumber(this.referenceType)) {
      this.formControls['referenceId'].enable();
      return true;
    }
    this.formControls['referenceId'].setValue(null);
    this.formControls['referenceId'].disable();
    return false;
  }

  private resetStatus() {
    this.submitted = false;
    this.isEditing = false;

    (this.isViewMode && !this.isEditing) ? this.form.disable() : this.form.enable();
  }

  private updateCandidateInfoData(candidate: Candidate) {
    const jsonArray = JSON.parse(candidate.candidateLanguage);
    const json = jsonArray?.map(item => (item.id )
    )
    this.form.patchValue({
      ...candidate,
      dob: candidate.dob ? new Date(candidate.dob) : null,
      candidateLanguage:json
    })

    this.originalFormData = this.form.getRawValue();
    this.avatarUrl = this._utilities.getLinkFile(candidate.avatar);
    this.cvFileName = this.getCvFileName(candidate);
    this.isEnaleRefBy = this.checkEnableReferBy(candidate.cvSourceId);
  }

  private getPayload(isUpdate: boolean = false) {
    const languages = this._utilities.catLanguages;
    const laguagesjs = this.formControls['candidateLanguage'].value
    const formattedLanguages = languages.filter(language => 
      laguagesjs.includes(language.id)
      );

    const payloadString = JSON.stringify(formattedLanguages);

    if (isUpdate) {
      const payload: CandidatePayload = {
        id: getFormControlValue(this.form, 'id'),
        name: getFormControlValue(this.form, 'fullName'),
        email: getFormControlValue(this.form, 'email'),
        phone: getFormControlValue(this.form, 'phone'),
        userType: getFormControlValue(this.form, 'userType'),
        subPositionId: getFormControlValue(this.form, 'subPositionId'),
        cvSourceId: getFormControlValue(this.form, 'cvSourceId'),
        branchId: getFormControlValue(this.form, 'branchId'),
        birthday: getFormControlValue(this.form, 'dob')
          ? moment(getFormControlValue(this.form, 'dob')).format(DateFormat.YYYY_MM_DD) : null,
        referenceId: getFormControlValue(this.form, 'referenceId'),
        isFemale: getFormControlValue(this.form, 'isFemale'),
        address: getFormControlValue(this.form, 'address'),
        note: getFormControlValue(this.form, 'note'),
        cvStatus: getFormControlValue(this.form, 'cvStatus'),
        candidateLanguage: payloadString,
      }
          return payload;
    }

    const formData = new FormData();
    formData.append('name', getFormControlValue(this.form, 'fullName'));
    formData.append('email', getFormControlValue(this.form, 'email'));
    formData.append('phone', getFormControlValue(this.form, 'phone'));
    formData.append('userType', getFormControlValue(this.form, 'userType'));
    formData.append('subPositionId', getFormControlValue(this.form, 'subPositionId'));
    formData.append('branchId', getFormControlValue(this.form, 'branchId'));
    formData.append('cvSourceId', getFormControlValue(this.form, 'cvSourceId'));
    formData.append('isFemale', getFormControlValue(this.form, 'isFemale'));
    formData.append('address', getFormControlValue(this.form, 'address'));
    formData.append('note', getFormControlValue(this.form, 'note'));
    formData.append('cvStatus', getFormControlValue(this.form, 'cvStatus'));
    formData.append('candidateLanguage', payloadString);   
    getFormControlValue(this.form, 'dob') ?
    formData.append('birthDay', moment(this.formControls['dob'].value).format(DateFormat.YYYY_MM_DD)) : formData.append('birthDay', '');

    const linkCv = this.dataApplyCv;
    this.isApplyCV = this.dataApplyCv ? true : false;
    if (this.isApplyCV == true) {
      formData.append("linkCv", linkCv.attachCV);
      formData.append('applyId',linkCv.id.toString())
    }
    this.cvFile && formData.append("cv", this.cvFile);
    this.isAvata =this.dataApplyCv ? true : false; 
    if (this.isAvata == true) {
    formData.append("avatarCv", linkCv.avatar);
    }
    this.avatarFile && formData.append('avatar', this.avatarFile);
    this.isEnaleRefBy && formData.append('referenceId', getFormControlValue(this.form, 'referenceId'));

    return formData;
  }

  private checkonLanguageChange(candidateLanguage: number) {
    const reference = this._utilities.catLanguages.find(item => item.id === candidateLanguage);
    if (candidateLanguage && !reference) {
      this.formControls['candidateLanguage'].enable();
      this.formControls['candidateLanguage'].patchValue(candidateLanguage)
      return true;
    }
    this.formControls['candidateLanguage'].setValue(null);
    return false;
  }

 onLanguageChange(candidateLanguage: number) {
    this.checkonLanguageChange(candidateLanguage);
  }

  onSearchTypeChange(value: string) {
    this.isSearchAnd = value === SearchType.AND;
  }

  private getCvFileName(entity: Candidate) {
    if (entity?.linkCV) {
      const extensionFile = entity.linkCV.split('.');
      return `CV_${entity.fullName}.${extensionFile[extensionFile.length - 1]}`
    }
    return '';
  }

  private checkExistCreatedUserAndNavigate(saveClose: boolean, candidateId: number) {
    const userId = this._apSession.userId;
    let userCreatedIndex: number;
    const isIntern = this.userType === UserType.INTERN;
    userCreatedIndex = isIntern ? this._utilities.catCanInternCreatedBy.findIndex(item => item.id === userId) :
      this._utilities.catCanStaffCreatedBy.findIndex(item => item.id === userId);

    if (userCreatedIndex >= 0) {
      saveClose ? this.backToCandidate() : this.moveToEducationStep(candidateId);
      return;
    }

    this._candidate.getAllUserCreated().subscribe(res => {
      if (!res?.result) return;
      isIntern ? this._utilities.catCanInternCreatedBy = res.result : this._utilities.catCanStaffCreatedBy = res.result
      saveClose ? this.backToCandidate() : this.moveToEducationStep(candidateId);
    })

  }

}

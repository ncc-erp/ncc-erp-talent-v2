import { Component, Injector, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { copyObject, getFormControlValue } from '@app/core/helpers/utils.helper';
import { CandidateSkill, CandidateSkillPayload } from '@app/core/models/candidate/candidate-skill.model';
import { CandidateInternService } from '@app/core/services/candidate/candidate-intern.service';
import { CandidateStaffService } from '@app/core/services/candidate/candidate-staff.service';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { AppComponentBase } from '@shared/app-component-base';
import { DateFormat, MESSAGE } from '@shared/AppConsts';
import { ActionEnum, API_RESPONSE_STATUS, ToastMessageType } from '@shared/AppEnums';
import { DialogService } from 'primeng/dynamicdialog';

@Component({
  selector: 'talent-candidate-skill',
  templateUrl: './candidate-skill.component.html',
  styleUrls: ['./candidate-skill.component.scss']
})
export class CandidateSkillComponent extends AppComponentBase implements OnInit, OnDestroy {

  @Input() userType: number;
  @Input() candidateId: number;
  @Input() _candidate: CandidateInternService | CandidateStaffService;
  @Input() isViewMode: boolean;

  readonly ACTION = ActionEnum;
  readonly DATE_FORMAT = DateFormat;

  isCreating: boolean = false;
  candidateSkills: CandidateSkill[] = [];
  clonedCanSkill: { [s: string]: CandidateSkill; } = {};
  form: FormGroup;
  submitted = false;

  constructor(
    injector: Injector,
    public dialogService: DialogService,
    public _utilities: UtilitiesService,
    private _fb: FormBuilder,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.initForm();
    this.getCanSkillData();
  }

  get formControls() {
    return this.form.controls;
  }

  onReset() {
    this.submitted = false;

    this.form.reset({
      id: 0,
      cvId: null,
      skillId: null,
      levelSkill: null,
      note: ''
    })
  }

  onDelete(entity: CandidateSkill) {
    const deleteRequest = this._candidate.deleteSkillCV(entity.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, entity.skillName).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.candidateSkills = this.candidateSkills.filter(item => item.id !== entity.id);
        }
      })
    );
  }

  onEdit(item: CandidateSkill) {
    this.clonedCanSkill[item.id] = copyObject(item);
  }

  saveSkill(item: CandidateSkill, action: ActionEnum) {
    if (action === ActionEnum.UPDATE) {
      this.handleUpdate(item);
      return
    }

    this.submitted = true;
    if (this.form.invalid) return;
    this.handleAdd();
  }

  onEditCancel(entity: CandidateSkill) {
    const idx: number = this.candidateSkills.findIndex(item => item.id === entity.id);
    this.candidateSkills[idx] = this.clonedCanSkill[entity.id];
  }

  onCreateCancel() {
    this.isCreating = false;
  }

  onAddSkill() {
    this.isCreating = true;
  }

  private handleAdd() {
    const payload: CandidateSkillPayload = {
      id: 0,
      cvId: this.candidateId,
      skillId: getFormControlValue(this.form, 'skillId'),
      level: getFormControlValue(this.form, 'levelSkill'),
      note: getFormControlValue(this.form, 'note')
    }

    this.subs.add(
      this._candidate.createSkill(payload).subscribe(res => {
        this.isLoading = res.loading;
        if (!res.loading && res.success) {
          this.candidateSkills.unshift(res.result);
          this.onReset();
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.ADD_SUCCESS, res.result.skillName);
        }
      })
    );
  }

  private handleUpdate(item: CandidateSkill) {
    const payload: CandidateSkillPayload = {
      id: item.id,
      cvId: this.candidateId,
      skillId: item.skillId,
      level: item.levelSkill,
      note: item.note
    }

    this.subs.add(
      this._candidate.updateCvSkill(payload).subscribe(res => {
        this.isLoading = res.loading;
        if (res.error) {
          this.onEditCancel(item);
          return;
        }

        if (!res.loading && res.success) {
          const idx = this.candidateSkills.findIndex(item => item.id === payload.id);
          this.candidateSkills[idx] = res.result;
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.skillName);
        }
      })
    );
  }

  private getCanSkillData() {
    this.subs.add(
      this._candidate.getCanSkillById(this.candidateId).subscribe(rs => {
        this.candidateSkills = [];
        this.isLoading = rs.loading;
        if (rs.success) {
          this.candidateSkills = rs.result;
        }
      })
    );
  }

  private initForm() {
    this.form = this._fb.group({
      id: 0,
      cvId: null,
      skillId: [null, [Validators.required]],
      levelSkill: [null, [Validators.required]],
      note: ''
    });
  }
}
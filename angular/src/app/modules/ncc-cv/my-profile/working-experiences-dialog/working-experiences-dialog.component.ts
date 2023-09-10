import { getFormControlValue } from '@app/core/helpers/utils.helper';
import { DateFormat } from './../../../../../shared/AppConsts';
import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { WorkingExperience } from '@app/core/models/employee-profile/profile-model';
import { MyProfileService } from '@app/core/services/employee-profile/my-profile.service';
import { ProjectService } from '@app/core/services/project/project.service';
import { Project } from '@app/modules/ncc-cv/project/list-project/list-project.component';
import { AppComponentBase } from '@shared/app-component-base';
import { AppSessionService } from '@shared/session/app-session.service';
import { PermissionCheckerService } from 'abp-ng2-module';
import * as moment from 'moment';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ActionEnum, ToastMessageType } from './../../../../../shared/AppEnums';
import { UtilitiesService } from './../../../../core/services/utilities.service';

@Component({
  selector: 'talent-working-experiences-dialog',
  templateUrl: './working-experiences-dialog.component.html',
  styleUrls: ['./working-experiences-dialog.component.scss']
})
export class WorkingExperiencesDialogComponent extends AppComponentBase implements OnInit {
  workingExp: WorkingExperience;
  currentlyWorking = false;
  listTechnologies: WorkingExperience[] = [];
  isSale: boolean;
  isUser: boolean;
  isEmployee = false;
  id: number;
  stateForm: FormGroup;
  isResult = false;
  isEdit: boolean;
  isChoose = false;
  listSelectTechnologies = [];
  listProjects: Project[] = [];
  filteredListProject: Project[] = [];
  name: string;
  submitted = false;

  workingExperience: WorkingExperience;
  action: ActionEnum;
  constructor(
    injector: Injector,
    private _myProfile: MyProfileService,
    public _utilities: UtilitiesService,
    public config: DynamicDialogConfig,
    public dialogRef: DynamicDialogRef,
    private session: AppSessionService,
    private _permissionChecker: PermissionCheckerService,
    private _formBuilder: FormBuilder,
    private _project: ProjectService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    Object.assign(this, this.config.data);
    if (this.isEmployee === false) this.id = this.session.userId;

    this.buildForm();
    this.isSale = this._permissionChecker.isGranted("Employee.EditAsPM");
    this.getListProject();

    if (this.workingExperience.id !== 0) {
      this.stateForm.patchValue({
        startTime: new Date(this.workingExperience.startTime),
        endTime: this.workingExperience.endTime ? new Date(this.workingExperience.endTime) : null,
        projectId: this.workingExperience.projectId,
        projectName: this.workingExperience.projectName,
        technologies: this.workingExperience.technologies,
        position: this.workingExperience.position,
        projectDescription: this.workingExperience.projectDescription,
        responsibility: this.workingExperience.responsibility,
        currentlyWorking: this.workingExperience.endTime ? false : true,
        id: this.workingExperience.id,
        userId: this.workingExperience.userId,
        order: this.workingExperience.order
      });
    }
    if (this.workingExperience.id !== 0) {
      this.isEdit = true;
    } else {
      this.searchByTechnologies();
      this.isEdit = false;
    }

    // fake working
    this.stateForm.get('currentlyWorking').valueChanges.subscribe(res => {
      this.currentlyWorking = res;
      if (res === false) {
        this.stateForm.get('endTime').setValue(new Date())
      }
    });
  }

  get formControls() { return this.stateForm.controls; }

  getListProject() {
    let type = -1;
    this._project.getAllProject('', type).subscribe(res => {
      if (!res || res.loading) return;
      this.listProjects = res.result;
      if (res.result.length === 0) {
        this.isChoose = false;
      }
    });
  }

  filterProject(event) {
    let filtered: any[] = [];
    let query = event.query;
    if (!query) {
      this.filteredListProject = [...this.listProjects];
      return;
    };

    this.listProjects.forEach(item => {
      if (item.name?.toLowerCase().indexOf(query?.toLowerCase()) == 0) {
        filtered.push(item);
      }
    })
    this.filteredListProject = filtered;
  }

  changeProject(item: Project, isInput: boolean = false) {
    if(isInput) {
      this.stateForm.patchValue({
        projectName: getFormControlValue(this.stateForm, 'projectName')
      });
      return;
    };


    if(!item) return;
    this.stateForm.patchValue({
      projectId: item.id,
      projectName: item.name,
      technologies: item.technology,
      projectDescription: item.description,
    });
    this.isChoose = true;
  }

  // NOT USE
  searchByTechnologies() {
    // const formdata = this.stateForm.value;
    // this.isResult = true;
    // this.employeeService.GetWorkingExperiencePaging(formdata.search, formdata.positionSearch,
    //   formdata.projectSearch, formdata.personName, false).subscribe(res => {
    //     if (res) {
    //       this.listTechnologies = res.result;
    //     }
    //   });
  }

  //NOT USE
  // selectedProjectName(work: WorkingExperience, event, index) {
  //   this.listTechnologies[index].isChecked = event.target.checked;
  //   this.pacthValueWhenFocus(work);
  //   this.listSelectTechnologies.push(this.listTechnologies[index]);
  // }


  //NOT USE
  // end fake 
  // pacthValueWhenFocus(work: WorkingExperience) {
  //   this.stateForm.patchValue({
  //     startTime: moment(work.startTime),
  //     endTime: work.endTime == null ? moment() : moment(work.endTime),
  //     projectId: work.projectId,
  //     projectName: work.projectName,
  //     technologies: work.technologies,
  //     position: work.position,
  //     projectDescription: work.projectDescription,
  //     responsibility: work.responsibility,
  //     currentlyWorking: work.endTime ? false : true,
  //     userId: work.userId,
  //     order: work.order
  //   });
  // }

  submitWorkingExp() {
    this.submitted = true;
    if (this.stateForm.invalid) return;

    const formData = this.stateForm.value;

    if (this.isUser) {
      const listNameProject = this.listProjects.map(el => el.name)
      const exitProject = listNameProject.includes(formData.projectName);
      if (exitProject === false && this.isEdit === false) {
        formData.projectId = 0;
      }
    }
    let curentDate = new Date();
    const data = {
      id: this.workingExperience.id,
      versionId: this.workingExperience.versionId,
      startTime: formData.startTime,
      endTime: formData.currentlyWorking == true ? curentDate : formData.endTime,
      projectId: formData.projectId === '' ? 0 : formData.projectId,
      projectName: formData.projectName,
      technologies: formData.technologies,
      position: formData.position,
      projectDescription: formData.projectDescription,
      responsibility: formData.responsibility,
      userId: this.workingExperience.userId,
      order: this.workingExperience.order
    };

    data.startTime = moment(data.startTime).format(DateFormat.YYYY_MM_DD)
    data.endTime = moment(data.endTime).format(DateFormat.YYYY_MM_DD)

    if (!data.endTime) {
      const endTime = new Date();
      if (endTime.getTime() - new Date(data.startTime).getTime() < 0) {
        this.showToastMessage(ToastMessageType.ERROR, 'Invalid date', 'Year end must be greater than start')
        return;
      }
    } else {
      if (new Date(data.endTime).getTime() - new Date(data.startTime).getTime() < 0) {
        this.showToastMessage(ToastMessageType.ERROR, 'Invalid date', 'Year end must be greater than start')
        return;
      }
    }


    this._myProfile.updateWorkingExperience(data).subscribe(res => {
      this.isLoading = res.loading;
      res.success && this.dialogRef.close(res);
    })
  }

  private buildForm() {
    this.stateForm = this._formBuilder.group({
      startTime: [new Date(), Validators.required],
      endTime: [new Date()],
      project: null,
      projectName: ['', Validators.required],
      projectId: [''],
      technologies: ['', Validators.required],
      position: ['', Validators.required],
      projectDescription: ['', Validators.required],
      responsibility: ['', Validators.required],
      currentlyWorking: [false],
      search: [''],
      positionSearch: [''],
      projectSearch: [''],
      personName: ['']
    });
  }
}

//NOT USE
// export interface StateGroup {
//   letter: string;
//   names: string[];
// }

//NOT USE
// export const _filter = (opt: string[], value: string): string[] => {
//   const filterValue = value.toLowerCase();
//   return opt.filter(item => item.toLowerCase().indexOf(filterValue) === 0);
// };

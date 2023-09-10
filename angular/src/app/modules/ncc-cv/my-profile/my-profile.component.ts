import { Component, Injector, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { JobPosition } from '@app/core/models/categories/job-position.model';
import { BreadCrumbConfig } from '@app/core/models/common/common.dto';
import { CommonService } from '@app/core/services/common.service';
import { AppConsts, MESSAGE } from '@shared/AppConsts';
import { PERMISSIONS_CONSTANT } from '@shared/permission/permissions';
import { ActionEnum, DefaultRoute, ToastMessageType } from '@shared/AppEnums';
import { LayoutStoreService } from '@shared/layout/layout-store.service';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { AppSessionService } from '@shared/session/app-session.service';
import { PermissionCheckerService } from 'abp-ng2-module';
import * as moment from 'moment';
import { DialogService } from 'primeng/dynamicdialog';
import { NccAppComponentBase } from '../../../../shared/ncc-component-base';
import { findDegreeName } from '../employee-profile.helper';
import { ContactInformation, PersonalAttributeConfigDialog, ProfileEducation, ProfileEducationConfigDiaLog, TechnicalExpertise, WorkingExperience, WorkingExperienceConfigDiaLog } from './../../../core/models/employee-profile/profile-model';
import { MyProfileService } from './../../../core/services/employee-profile/my-profile.service';
import { VersionService } from './../../../core/services/employee-profile/version.service';
import { ContactInformationComponent } from './contact-information/contact-information.component';
import { CreateVersionComponent } from './create-version/create-version.component';
import { ExportDialogComponent } from './export-dialog/export-dialog.component';
import { PersonalAttributeComponent } from './personal-attribute/personal-attribute.component';
import { ProfileEducationComponent } from './profile-education/profile-education.component';
import { TechnicalExpertisesComponent } from './technical-expertises/technical-expertises.component';
import { WorkingExperiencesDialogComponent } from './working-experiences-dialog/working-experiences-dialog.component';


@Component({
  selector: 'talent-my-profile',
  templateUrl: './my-profile.component.html',
  styleUrls: ['./my-profile.component.scss']
})
export class MyProfileComponent extends NccAppComponentBase implements OnInit {

  MYPROFILE_USER_VIEW = PERMISSIONS_CONSTANT.MyProfile_User_View;
  MYPROFILE_USER_EDIT = PERMISSIONS_CONSTANT.MyProfile_User_Edit;
  MYPROFILE_VERSION_CREATE = PERMISSIONS_CONSTANT.MyProfile_Version_Create;
  MYPROFILE_VERSION_DELETE = PERMISSIONS_CONSTANT.MyProfile_Version_Delete;
  MYPROFILE_EDUCATION_CREATE = PERMISSIONS_CONSTANT.MyProfile_Education_Create;
  MYPROFILE_EDUCATION_EDIT = PERMISSIONS_CONSTANT.MyProfile_Education_Edit;
  MYPROFILE_EDUCATION_VIEW = PERMISSIONS_CONSTANT.MyProfile_Education_View;
  MYPROFILE_EDUCATION_DELETE = PERMISSIONS_CONSTANT.MyProfile_Education_Delete;
  MYPROFILE_TECHNICALEXPERTISE_CREATE = PERMISSIONS_CONSTANT.MyProfile_TechnicalExpertise_Create;
  MYPROFILE_TECHNICALEXPERTISE_EDIT = PERMISSIONS_CONSTANT.MyProfile_TechnicalExpertise_Edit;
  MYPROFILE_TECHNICALEXPERTISE_VIEW = PERMISSIONS_CONSTANT.MyProfile_TechnicalExpertise_View;
  MYPROFILE_PERSONALATTRIBUTE_DELETE = PERMISSIONS_CONSTANT.MyProfile_PersonalAttribute_Delete;
  MYPROFILE_PERSONALATTRIBUTE_VIEW = PERMISSIONS_CONSTANT.MyProfile_PersonalAttribute_View;
  MYPROFILE_PERSONALATTRIBUTE_CREATE = PERMISSIONS_CONSTANT.MyProfile_PersonalAttribute_Create;
  MYPROFILE_PERSONALATTRIBUTE_EDIT = PERMISSIONS_CONSTANT.MyProfile_PersonalAttribute_Edit;
  MYPROFILE_WORKINGEXPERIENCE_CREATE = PERMISSIONS_CONSTANT.MyProfile_WorkingExperience_Create;
  MYPROFILE_WORKINGEXPERIENCE_EDIT = PERMISSIONS_CONSTANT.MyProfile_WorkingExperience_Edit;
  MYPROFILE_WORKINGEXPERIENCE_DELETE = PERMISSIONS_CONSTANT.MyProfile_WorkingExperience_Delete;
  MYPROFILE_WORKINGEXPERIENCE_VIEW = PERMISSIONS_CONSTANT.MyProfile_WorkingExperience_View;

  //PersonalAttribute
  employeeName: string;
  inform: ContactInformation;
  listskill: TechnicalExpertise;
  groupSkillNumber = 0;
  image: string;
  urlImg = AppConsts.remoteServiceBaseUrl;
  fullName: string;
  surname: string;
  name: string;
  listEducation: ProfileEducation[] = [];
  dataForEdit: any;
  workingExperience: WorkingExperience[] = [];
  tempWorkingExperience: any;
  value: number
  highValue: number
  listDegree = {
    options: [
      { id: 0, value: 'HighSchool' },
      { id: 1, value: 'Bachelor' },
      { id: 2, value: 'Master' },
      { id: 3, value: 'PostDoctor' },
      { id: 4, value: 'Certificate' }
    ]
  };
  isUser: boolean;
  listPersonalAtribute: any;
  isSale = false;
  editAsSale = false;
  isVersionDefault = true;
  startProject: number = 2000;
  endProject: number = 2010;
  dateRange: Date[] = this.customDateRange();
  @Input() id: number;
  @Input() isEmployee = false;
  filePathofExport: string;
  fileNameExport: string;
  position: JobPosition[] = [];
  version: { id: number, name: string }[] = [
    { id: 0, name: 'Default' }
  ];
  isOpenMenu = false;
  versionForm: FormGroup;
  constructor(
    private fb: FormBuilder,
    injector: Injector,
    private _dialog: DialogService,
    private _myProfile: MyProfileService,
    private session: AppSessionService,
    private _permissionChecker: PermissionCheckerService,
    private commonService: CommonService,
    private _layoutStore: LayoutStoreService,
    private versionService: VersionService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (this.isEmployee === false) {
      this.id = this.session.userId;
    }

    this.buildForm();
    this.versionForm.get('version').setValue(0);
    if (this.versionForm.get('version').value === 0) {
      this.isVersionDefault = false;
    }
    this.isUser = this.session.userId === Number(this.id);
    this.isSale = this._permissionChecker.isGranted('Employee.EditAsPM');
    this.editAsSale = this._permissionChecker.isGranted('Employee.EditAsPM');
    this.getAll('');
    this.GetAllPositionType();
    this.getAllVersion();
    this._layoutStore.sidebarExpanded.subscribe((value) => {
      if (this.isOpenMenu === false) {
        this.isOpenMenu = true;
      } else { this.isOpenMenu = false; }
    });
    this.versionForm.get('version').valueChanges.subscribe(vl => {
      if (vl === 0) {
        this.isVersionDefault = false;
        this.getAll('');
      }
      if (vl && vl !== 0) {
        this.isVersionDefault = true;
        this.getAll(vl);
      }
    });
  }

  getAll(data) {
    this.getInformUser(data);
    this.getTechnicalExpertise(data);
    this.getEducationInfo(data);
    this.getUserWorkingExperience(data);
    this.getPersonalAttribute();
  }

  buildForm() {
    this.versionForm = this.fb.group({
      version: ['']
    });
  }

  getAllVersion() {
    this.versionService.getAllVersion(Number(this.id)).subscribe(res => {
      if (!res || res.loading) return;
      const result = res.result.map(el => {
        return {
          id: el.versionId,
          name: el.versionName
        };
      });
      this.version = result;
      this.version.unshift({ id: 0, name: 'Default' })
    });
  }

  // dialog thêm mới version
  showDialogCreateVersion() {
    const dialogRef = this._dialog.open(CreateVersionComponent, {
      width: "40%",
      contentStyle: { "max-height": "100%", overflow: "visible" },
      showHeader: false,
      baseZIndex: 5000,
      data: this.id,
    });

    dialogRef.onClose.subscribe(res => {
      if (!res) return;
      this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CREATE_SUCCESS, res?.result?.versionName)
      this.getAllVersion();
    });
  }


  deleteVersion() {
    this.versionService.deleteVersion(this.versionForm.get('version').value).subscribe(res => {
      if (res.loading) return;

      this.notify.success('Delete version success');
      this.ngOnInit();
    })
  }

  // dialog chọn định dạng kết xuất
  showDialogExport() {
    const versionId = this.versionForm.get('version').value === 0 ? '' : this.versionForm.get('version').value;
    if (this.isSale && !this.isUser) {
      const item = [{
        typeOffile: '',
        isHiddenYear: false,
        employeeInfo: this.inform,
        educationBackGround: this.listEducation,
        technicalExpertises: this.listskill,
        personalAttributes: { personalAttributes: this.listPersonalAtribute },
        workingExperiences: this.workingExperience,
      },
      this.isUser,
      ];
      const dialogRef = this._dialog.open(ExportDialogComponent, {
        showHeader: false,
        data: item
      });
      dialogRef.onClose.subscribe(res => {
      });
    } else {
      const userId = this.id;
      const item = [{ userId, versionId }, this.isUser];
      const dialogRef = this._dialog.open(ExportDialogComponent, {
        showHeader: false,
        data: item
      });
    }
  }

  // Lấy thông tin nhân viên
  getInformUser(vId) {//UU
    if (this.permission.isGranted(PERMISSIONS_CONSTANT.MyProfile_User_View)) {
      let versionId = vId;
      this._myProfile.getUserGeneralInfo(this.id, versionId).subscribe(res => {
        if (!res || res.loading) return;

        this.inform = res.result;
        this.employeeName = res.result.surname + ' ' + res.result.name;
        this.surname = res.result.surname;
        this.name = res.result.name;
      });
    }
  }

  showDialogCreatEdit(entity: ContactInformation) { // UU
    const dialogRef = this._dialog.open(ContactInformationComponent, {
      width: "40%",
      contentStyle: { "max-height": "100%", overflow: "visible" },
      showHeader: false,
      baseZIndex: 5000,
      data: entity || null,
    });
    dialogRef.onClose.subscribe(res => {
      if (this.isSale && !this.isUser) {
        this.inform = res;
        this.inform.currentPosition = this.filterPosition(this.position, res.currentPosition);
        this.employeeName = res.surname + ' ' + res.name;
        this.surname = res.surname;
        this.name = res.name;
      } else {
        if (this.versionForm.get('version').value === 0) {
          this.getInformUser('');
        } else {
          this.getInformUser(this.versionForm.get('version').value);
        }
      }
    });
  }
  // end infor

  // Lấy thông tin, thêm sửa xóa  kinh nghiệm công việc
  getUserWorkingExperience(versionId) {
    if (this.permission.isGranted(PERMISSIONS_CONSTANT.MyProfile_WorkingExperience_View)) {
      this._myProfile.getUserWorkingExperience(this.id, versionId).subscribe(res => {
        if (!res || res.loading) return;

        this.workingExperience = res.result.map(el => {
          el.endTime = moment(new Date(el.endTime)).format('YYYY-MM-DD');
          el.startTime = moment(new Date(el.startTime)).format('YYYY-MM-DD');
          return { ...el }
        });
        this.tempWorkingExperience = res.result;
        this.convertDate()
        this.startProject = Number(moment(new Date(this.workingExperience[this.workingExperience.length - 1]?.startTime)).format('YYYY'));
        this.endProject = Number(moment(new Date(this.workingExperience[0]?.endTime)).format('YYYY'))
        this.convertDate();
        this.dateRange = this.customDateRange();
        this.options = {
          stepsArray: this.dateRange.map((date) => {
            return {
              value: date.getTime(),
              highValue: date.getTime()
            };
          }),
          translate: (value: number, highValue: number): string => {
            return moment(new Date(value).toDateString()).format("MM/YYYY");
          }
        };
      });
    }
  }

  editWorkingExp(workingEpx: WorkingExperience) {
    this.showDialogCreateEditWorkingExp(workingEpx, ActionEnum.UPDATE);
  }

  deleteWorkingExp(workingEpx: WorkingExperience) {
    abp.message.confirm(undefined, 'Are you sure that you want to delete?',
      (result: boolean) => {
        if (result) {

          this._myProfile.deleteWorkingExperience(workingEpx.id).subscribe(res => {
            if (res) {
              this.notify.success(MESSAGE.DELETE_SUCCES);
              if (this.versionForm.get('version').value === 0) { this.getUserWorkingExperience('') }
              else { this.getUserWorkingExperience(this.versionForm.get('version').value); }
            }
          });
        }
      }
    );
  }

  createWorkingExp() {
    const workingExp = {
      id: 0,
      userId: this.id,
      versionId: this.versionForm.get('version').value === 0 ? '' : this.versionForm.get('version').value
    } as WorkingExperience;
    this.showDialogCreateEditWorkingExp(workingExp, ActionEnum.CREATE);
  }

  showDialogCreateEditWorkingExp(workingEpx: WorkingExperience, action: ActionEnum) {
    const dialogConfig: WorkingExperienceConfigDiaLog = { workingExperience: workingEpx, isUser: this.isUser, action };
    const dialogRef = this._dialog.open(WorkingExperiencesDialogComponent, {
      width: "60%",
      contentStyle: { "max-height": "100%", overflow: "auto" },
      showHeader: false,
      baseZIndex: 5000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe(res => {
      if (!res) return;

      this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS)
      if (this.versionForm.get('version').value === 0) {
        this.getUserWorkingExperience('');
      } else {
        this.getUserWorkingExperience(this.versionForm.get('version').value);
      }
    });
  }
  // end working experience

  // Lấy thông tin. thêm sửa xóa thông tin giáo dục
  getEducationInfo(vrsId) { //OK
    if (this.permission.isGranted(PERMISSIONS_CONSTANT.MyProfile_Education_View)) {
      this._myProfile.getEducationInfo(this.id, vrsId).subscribe(res => {
        if (!res || res.loading) return;
        this.listEducation = res.result.map(el => {
          return {
            ...el,
            degreeType: findDegreeName(el.degreeType)
          };
        });
      });
    }
  }

  createEducation() {
    const education = {
      id: 0,
      cvemployeeId: this.id,
      versionId: this.versionForm.get('version').value === 0 ? '' : this.versionForm.get('version').value
    } as ProfileEducation;
    this.dialogCreateEditEducation(education, ActionEnum.CREATE);
  }

  editEducation(item: ProfileEducation) {
    this.dialogCreateEditEducation(item, ActionEnum.UPDATE);
  }

  deleteEducation(item: ProfileEducation) {
    this.subs.add(
      this._myProfile.deleteEducation(item.id).subscribe(res => {
        if (res?.success) {
          this.notify.success(MESSAGE.DELETE_SUCCES);
          if (this.versionForm.get('version').value === 0) { this.getEducationInfo(''); }
          else { this.getEducationInfo(this.versionForm.get('version').value); }
        }
      })
    );
  }

  dialogCreateEditEducation(education: ProfileEducation, dialogAction: ActionEnum) {
    const dialogConfig: ProfileEducationConfigDiaLog = { education: education, action: dialogAction, isUser: this.isUser };
    const dialogRef = this._dialog.open(ProfileEducationComponent, {
      width: "50%",
      contentStyle: { "max-height": "100%", overflow: "visible" },
      showHeader: false,
      baseZIndex: 5000,
      data: dialogConfig,
    });

    const index = this.listEducation.indexOf(education);
    dialogRef.onClose.subscribe((res: ApiResponse<ProfileEducation>) => {
      if (!res) return;

      if (this.isSale && !this.isUser) {
        if (res) {
          if (!education.schoolOrCenterName) {
            res.result.degreeType = findDegreeName(res.result.degreeType);
            this.listEducation.push(res.result);
          } else {
            this.listEducation[index] = res.result;
            this.listEducation[index].degreeType = findDegreeName(res.result.degreeType);
          }
        } else {
        }
      } else {
        if (this.versionForm.get('version').value === 0) {
          this.getEducationInfo('');
        } else {
          this.getEducationInfo(this.versionForm.get('version').value);
        }
      }
    });
  }

  // Lấy thông tin thêm sửa xóa thông tin atribute
  getPersonalAttribute() {
    if (this.permission.isGranted(PERMISSIONS_CONSTANT.MyProfile_PersonalAttribute_View)) {
      this._myProfile.getPersonalAttribute(this.id).subscribe(res => {
        if (!res || res.loading) return;

        if (res.result === null) {
          this.listPersonalAtribute = [];
        }
        this.listPersonalAtribute = res.result.personalAttributes;
      });
    }
  }

  createAtribute(listPersonalAtribute): void {
    this.showDialogCreateEditAtribute(listPersonalAtribute, ActionEnum.CREATE);
  }

  editAtribute(listPersonalAtribute, item): void {
    this.showDialogCreateEditAtribute(listPersonalAtribute, ActionEnum.UPDATE, item);
  }

  deleteAtribute(index) {
    abp.message.confirm(undefined, 'Are you sure that you want to delete?', (result: boolean) => {
      if (result) {
        if (this.isSale && !this.isUser) {
          this.listPersonalAtribute.splice(index, 1);
        } else {
          this.listPersonalAtribute.splice(index, 1);
          const data = { personalAttributes: this.listPersonalAtribute };
          this._myProfile.updatePersonalAttribute(data).subscribe(res => {
            if (res) {
              this.notify.success(MESSAGE.DELETE_SUCCES);
              this.getPersonalAttribute();
            }
          });
        }
      }
    });
  }

  showDialogCreateEditAtribute(listPersonalAtribute, action: ActionEnum, item = null) {
    const body: PersonalAttributeConfigDialog = {
      listPersonalAtribute,
      item,
      isUser: this.isUser,
      action
    }

    const dialogRef = this._dialog.open(PersonalAttributeComponent, {
      width: "50%",
      contentStyle: { "max-height": "100%", overflow: "visible" },
      showHeader: false,
      baseZIndex: 5000,
      data: body,
    });

    dialogRef.onClose.subscribe(res => {
      if (this.isSale && !this.isUser) {
        if (res) { this.listPersonalAtribute = res.personalAttributes; }
      } else {
        if (res) this.getPersonalAttribute();
      }
    });
  }
  // end atribute


  // Lấy thông tin , thêm sửa xóa edit technical
  getTechnicalExpertise(versionId) {
    if (this.permission.isGranted(PERMISSIONS_CONSTANT.MyProfile_TechnicalExpertise_View)) {

      this._myProfile.getTechnicalExpertise(this.id, versionId).subscribe(res => {
        if (!res || res.loading) return;
        this.listskill = res.result;
        this.groupSkillNumber = this.listskill.groupSkills.length;
      });
    }
  }

  createTechnical(): void {
    const listskill = {
      userId: this.id,
      versionId: this.versionForm.get('version').value === 0 ? '' : this.versionForm.get('version').value,
      groupSkills: []
    } as TechnicalExpertise;
    this.showDialogCreateEditTechnical(listskill, ActionEnum.CREATE);
  }

  editTechnical(listskill: TechnicalExpertise): void {
    this.showDialogCreateEditTechnical(listskill, ActionEnum.UPDATE);
  }

  showDialogCreateEditTechnical(listskill: TechnicalExpertise, dialogAction: ActionEnum) {
    const item = {
      listskill,
      isUser: this.isUser,
      action: dialogAction
    };

    const dialogRef = this._dialog.open(TechnicalExpertisesComponent, {
      width: "40%",
      contentStyle: { "max-height": "100%", overflow: "visible" },
      showHeader: false,
      baseZIndex: 5000,
      data: item,
    });


    dialogRef.onClose.subscribe(res => {
      if (this.isSale && !this.isUser) {
        if (res) {
          this.listskill = res;
          this.groupSkillNumber = this.listskill.groupSkills.length;
        } else {

        }
      } else {
        if (this.versionForm.get('version').value === 0) {
          this.getTechnicalExpertise('');
        } else {
          this.getTechnicalExpertise(this.versionForm.get('version').value);
        }
      }
    });
  }

  // end dialog techincal
  xuongDong(value: string) {
    if (value) {
      return value.split('\n').join('<br>');
    }
  }

  GetAllPositionType() {
    // this.commonService.GetCBBPositionType().subscribe(res => {
    //   this.position = res.result;
    // });
  }

  filterPosition(position: JobPosition[], type: number) {
    const founded = position.find(el => el.id === type);
    return founded ? founded.name : '';
  }

  exitEmployee() {
    this.router.navigate(['app/ncc-cv/employee-list']);
  }

  UpdateProject(item) {
    item.startTime = moment(item.startTime).format('L');
    item.endTime = moment(item.endTime).format('L');
    this._myProfile.updateWorkingExperience(item).subscribe(ul => {
      this.notify.success(MESSAGE.UPDATE_SUCCESS)
      if (this.versionForm.get('version').value === 0) {
        this.getUserWorkingExperience('');
      } else {
        this.getUserWorkingExperience(this.versionForm.get('version').value);
      }
    });
  }

  // options: Options = {
  options = {
    stepsArray: this.dateRange.map((date) => {
      return {
        value: date.getTime(),
        highValue: date.getTime()
      };
    }),
    translate: (value: number, highValue: number): string => {
      return moment(new Date(value).toDateString()).format("MM/YYYY");
    }
  };

  customDateRange(): Date[] {
    const dates: Date[] = [];
    for (let y: number = this.startProject; y <= this.endProject; y++) {
      for (let i: number = 1; i <= 12; i++) {
        dates.push(new Date(y, i));
      }
    }
    return dates;
  }

  convertDate() {
    this.tempWorkingExperience.forEach(item => {
      item.startTime = new Date(item.startTime).getTime();
      item.endTime = new Date(item.endTime).getTime();
    })
  }

  toggleSidebar() {
    let bar = document.getElementById("sideBarWorkingExperience");
    let mainBar = document.getElementById("mainMenuWorkingExperience");
    if (bar.style.display === "none") {
      bar.style.display = "block";
      mainBar.className = "col-md-8";
    } else {
      bar.style.display = "none";
      mainBar.className = "col-md-12"
    }
  }

  protected getBreadCrumbConfig(): BreadCrumbConfig {
    return {
      menuItem: [{ label: "Employee Profile", routerLink: DefaultRoute.Employee_Profile, styleClass: 'menu-item-click' }, { label: "Profile" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}

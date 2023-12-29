import { CommonEmployeeService } from './employee-profile/common-employee.service';
import { Injectable } from "@angular/core";
import { Education } from '@app/core/models/categories/education.model';
import { LmsCourse } from '@app/core/models/categories/position-setting.model';
import { CvSourceService } from "@app/core/services/categories/cv-source.service";
import { PositionSettingService } from '@app/core/services/categories/position-setting.service';
import { DateFormat, PROCESS_STATUS, PROCESS_STATUS_OPTION, RQ_PROCESS_STATUS } from '@shared/AppConsts';
import {
  CandidateStatus,
  DIFF_DATE_COLOR,
  PriorityColor,
  RequestCvStatusColor,
  RequisitionStatusColor,
  UserTypeColor
} from "@shared/AppEnums";
import { ApiResponse } from '@shared/paged-listing-component-base';
import * as moment from 'moment';
import { forkJoin } from "rxjs";
import { Capability } from "../../core/models/categories/capabilities.model";
import { JobPosition } from "../../core/models/categories/job-position.model";
import { Branch } from "../models/categories/branch.model";
import { CVSource } from "../models/categories/cv-source.model";
import { EducationType } from "../models/categories/education-type.model";
import { CatalogModel, LevelInfo, PositionCatalog } from "../models/common/common.dto";
import { Skill } from "./../models/categories/skill.model";
import { CandidateLanguage } from "../models/candidate/candidate-language.model";
import {
  InternSalaryCatalog,
  UserCatalog
} from "./../models/common/common.dto";
import { CandidateInternService } from "./candidate/candidate-intern.service";
import { CandidateInterviewService } from './candidate/candidate-interview.service';
import { CandidateStaffService } from "./candidate/candidate-staff.service";
import { BranchService } from "./categories/branch.service";
import { CapabilitySettingService } from "./categories/capability-setting.service";
import { CapabilityService } from "./categories/capability.service";
import { EducationTypeService } from "./categories/education-type.service";
import { EducationService } from './categories/education.service';
import { JobPositionService } from "./categories/job-position.service";
import { SkillService } from "./categories/skill.service";
import { CommonService } from "./common.service";
import { CandidateLanguageService } from "./candidate/candidate-language.service";

@Injectable({
  providedIn: "root",
})
export class UtilitiesService {
  constructor(
    private _educationType: EducationTypeService,
    private _eduction: EducationService,
    private _jobPosition: JobPositionService,
    private _capability: CapabilityService,
    private _capabilitySetting: CapabilitySettingService,
    private _branch: BranchService,
    private _skill: SkillService,
    private _cvSource: CvSourceService,
    private _common: CommonService,
    private _candidateStaff: CandidateStaffService,
    private _candidateIntern: CandidateInternService,
    private _candidateInterview: CandidateInterviewService,
    private _positionSetting: PositionSettingService,
    private _commonEmployee: CommonEmployeeService,
    private _candidatelanguage: CandidateLanguageService,
  ) { }

  catEducation: Education[];
  catEducationType: EducationType[];
  catCapability: Capability[];
  catJobPosition: JobPosition[];
  catPosition: PositionCatalog[];
  catUserType: CatalogModel[];
  catBranch: Branch[];
  catLanguages: CandidateLanguage[];
  catSkill: Skill[];
  catReqCvStatus: CatalogModel[];
  catProcessCvStatus: CatalogModel[] = PROCESS_STATUS;
  catProcessCvStatusOption = PROCESS_STATUS_OPTION;
  catRqProcessCvStatus: CatalogModel[] = RQ_PROCESS_STATUS;
  catCvStatus: CatalogModel[];
  catCvSource: CVSource[];
  catInterviewStatus: CatalogModel[];
  catReqStatus: CatalogModel[];
  catCanOnboardStatus: CatalogModel[];
  catCanOfferStatus: CatalogModel[];
  catPriority: CatalogModel[];
  catLevel: LevelInfo[];
  catStaffLeveL: LevelInfo[];
  catStandardLevel: CatalogModel[];
  catCanStaffCreatedBy: CatalogModel[];
  catCanInternCreatedBy: CatalogModel[];
  catAllUser: UserCatalog[];
  catAllInterviewer: UserCatalog[];
  catInternSalary: InternSalaryCatalog[];
  catCvReferType: CatalogModel[];
  catLmsCourse: LmsCourse[];
  catEmployeePosition: CatalogModel[];
  catLanguage: CatalogModel[];
  catCCBGroupSkill: CatalogModel[];
  catLevelFinalIntern: InternSalaryCatalog[];
  catLevelFinalStaff: LevelInfo[];

  loadCatalogForCategories() {
    forkJoin([
      this._educationType.getAll(),
      this._capability.getAll(),
      this._jobPosition.getAll(),
      this._common.getDropdownPositions(),
      this._capabilitySetting.getUserTypes(),
      this._common.getListCVSourceReferenceType(),
    ]).subscribe({
      next: ([eduTypes, capabilities, position, commonPosition, userType, cvReferType]) => {
        this.catEducationType = eduTypes.result;
        this.catCapability = capabilities.result;
        this.catJobPosition = position.result;
        this.catPosition = commonPosition.result;
        this.catUserType = userType.result;
        this.catCvReferType = cvReferType.result;
      },
    });
  }

  async LevelFinalCandidate(): Promise<void> {
    const promises = [
      this._common.getLevelFinalIntern().toPromise(),
      this._common.getLevelFinalStaff().toPromise(),
    ];
    return await Promise.all(promises).then((res) => {
      this.catLevelFinalIntern = res[0].result as InternSalaryCatalog[],
      this.catLevelFinalStaff = res[1].result
    })
  }

  async loadCatalogForCandidate(): Promise<void> {
    const promises = [
      this._branch.getAll().toPromise(),
      this._jobPosition.getAll().toPromise(),
      this._skill.getAll().toPromise(),
      this._common.getRequestCVStatus().toPromise(),
      this._capabilitySetting.getUserTypes().toPromise(),
      this._cvSource.getAll().toPromise(),
      this._candidateStaff.getAllUserCreated().toPromise(),
      this._candidateIntern.getAllUserCreated().toPromise(),
      this._common.getLevel().toPromise(),
      this._common.getAllUser().toPromise(),
      this._common.getPriority().toPromise(),
      this._common.getRequestStatus().toPromise(),
      this._common.getInternSalary().toPromise(),
      this._educationType.getAll().toPromise(),
      this._common.getLevelStaff().toPromise(),
      this._common.getStatusCandidateOnboard().toPromise(),
      this._common.getStatusCandidateOffer().toPromise(),
      this._common.getListCVStatus().toPromise(),
      this._eduction.getAll().toPromise(),
      this._common.getDropdownPositions().toPromise(),
      this._common.getLevelFinalIntern().toPromise(),
      this._common.getLevelFinalStaff().toPromise(),
      this._candidatelanguage.getAll().toPromise(),
    ];
    return await Promise.all(promises).then((res) => {
      this.catBranch = res[0].result;
      this.catJobPosition = res[1].result;
      this.catSkill = res[2].result;
      this.catReqCvStatus = res[3].result;
      this.catUserType = res[4].result;
      this.catCvSource = res[5].result;
      this.catCanStaffCreatedBy = res[6].result;
      this.catCanInternCreatedBy = res[7].result;
      this.catLevel = res[8].result;
      this.catAllUser = res[9].result;
      this.catPriority = res[10].result;
      this.catReqStatus = res[11].result;
      this.catInternSalary = res[12].result;
      this.catEducationType = res[13].result;
      this.catStaffLeveL = res[14].result;
      this.catCanOnboardStatus = res[15].result;
      this.catCanOfferStatus = res[16].result;
      this.catCvStatus = res[17].result;
      this.catEducation = res[18].result;
      this.catPosition = res[19].result;
      this.catLevelFinalIntern = res[20].result;
      this.catLevelFinalStaff = res[21].result;
      this.catLanguages = res[22].result;
    });
  }

  async loadCatalogForInterview(): Promise<void> {
    const promises = [
      this._candidateInterview.getAllInterview().toPromise(),
      this._common.getListInterviewStatus().toPromise(),
    ];
    return await Promise.all(promises).then((res: ApiResponse<any>[]) => {
      this.catAllInterviewer = res[0].result;
      this.catInterviewStatus = res[1].result;
    });
  }

  async loadCatalogForEmployeeProfile(): Promise<void> {
    const promises = [
      this._commonEmployee.getPosition().toPromise(),
      this._commonEmployee.getLanguage().toPromise(),
      this._commonEmployee.getCBBGroupSkill().toPromise(),
      this._branch.getAll().toPromise(),
    ];
    return await Promise.all(promises).then((res: ApiResponse<any>[]) => {
      this.catEmployeePosition = res[0].result;
      this.catLanguage = res[1].result;
      this.catCCBGroupSkill = res[2].result;
      this.catBranch = res[3].result;
    });
  }

  async loadCatalogForPositionSetting(): Promise<void> {
    const promises = [
      this._positionSetting.getListCourse().toPromise(),
    ];
    return await Promise.all(promises).then((res: ApiResponse<any>[]) => {
      this.catLmsCourse = res[0].result;
    });
  }

  async loadCatalogForRequisition(): Promise<void> {
    const promises = [
      this._branch.getAll().toPromise(),
      this._jobPosition.getAll().toPromise(),
      this._skill.getAll().toPromise(),
      this._capabilitySetting.getUserTypes().toPromise(),
      this._common.getRequestStatus().toPromise(),
      this._common.getPriority().toPromise(),
      this._common.getLevel().toPromise(),
      this._candidateStaff.getAllUserCreated().toPromise(),
      this._candidateIntern.getAllUserCreated().toPromise(),
      this._common.getRequestCVStatus().toPromise(),
      this._common.getRequestLevel().toPromise(),
      this._common.getLevelStaff().toPromise(),
      this._common.getListCVStatus().toPromise(),
      this._common.getDropdownPositions().toPromise()
    ];
    return await Promise.all(promises).then((res) => {
      this.catBranch = res[0].result;
      this.catJobPosition = res[1].result;
      this.catSkill = res[2].result;
      this.catUserType = res[3].result;
      this.catReqStatus = res[4].result;
      this.catPriority = res[5].result;
      this.catLevel = res[6].result;
      this.catCanStaffCreatedBy = res[7].result;
      this.catCanInternCreatedBy = res[8].result;
      this.catReqCvStatus = res[9].result;
      this.catStandardLevel = res[10].result;
      this.catStaffLeveL = res[11].result;
      this.catCvStatus = res[12].result;
      this.catPosition = res[13].result;
    });
  }

  getLinkFile(filePath: string) {
    return filePath;
  }


  async loadLabelForReportRecruitmentOverview(): Promise<any> {
    const promises = [
      this._common.getRequestCVStatus().toPromise(),
      this._cvSource.getAll().toPromise(),
      this._branch.getAll().toPromise(),
      this._capabilitySetting.getUserTypes().toPromise(),
    ];
    return await Promise.all(promises).then((res) => {
      this.catReqCvStatus = res[0].result;
      this.catCvSource = res[1].result;
      this.catBranch = res[2].result;
      this.catUserType = res[3].result;
    });
  }

  public getBgTagPriorityColor(name: string) {
    return PriorityColor[name?.split(" ").join("")] || PriorityColor.Default;
  }

  public getBgTagReqStatusColor(name: string) {
    return RequisitionStatusColor[name?.split(" ").join("")];
  }

  public getBgTagBranchColor(name: string) {
    return this.catBranch?.find((item) => item.name === name).colorCode;
  }

  public getBgTagRequestCvStatusColor(name: string) {
    return (
      RequestCvStatusColor[name?.split(" ").join("")] ||
      RequestCvStatusColor.Default
    );
  }

  public getUserTypeColor(name: string) {
    return UserTypeColor[name?.split(' ').join('')] || UserTypeColor.Default;
  }

  public getCVStatusColor(name: string) {
    return CandidateStatus[name?.split(" ").join("")];
  }

  public isNumberOnly(event) {
    return event.charCode >= 48 && event.charCode <= 57;
  }

  getDiffDateTime(inputDate: string) {
    if (!inputDate) return { label: null, color: null };

    const current = moment(new Date, DateFormat.YYYY_MM_DD_HH_MM_SS);
    const input = moment(inputDate, DateFormat.YYYY_MM_DD_HH_MM_SS);

    const year = input.diff(current, 'year');
    const day = input.diff(current, 'day');
    const hour = input.diff(current, 'hour');
    const minute = input.diff(current, 'minute');

    if (year !== 0) {
      return year < 0 ? { label: `${Math.abs(year)}y ago`, color: DIFF_DATE_COLOR.PAST } : { label: `in ${year}y`, color: DIFF_DATE_COLOR.FUTURE };
    }

    if (day !== 0) {
      return day < 0 ? { label: `${Math.abs(day)}d ago`, color: DIFF_DATE_COLOR.PAST } : { label: `in ${day}d`, color: DIFF_DATE_COLOR.FUTURE }
    }

    if (hour !== 0) {
      return hour < 0 ? { label: `${Math.abs(hour)}h ago`, color: DIFF_DATE_COLOR.PAST } : { label: `in ${Math.abs(hour)}h`, color: DIFF_DATE_COLOR.FUTURE }
    }

    if (minute !== 0) {
      return minute < 0 ? { label: `${Math.abs(minute)}m ago`, color: DIFF_DATE_COLOR.PAST } : { label: `in ${Math.abs(minute)}m`, color: DIFF_DATE_COLOR.FUTURE }
    }
  }

}

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { NccCvRoutingModule } from './ncc-cv-routing.module';
import { NccCvComponent } from './ncc-cv.component';
import { MyProfileComponent } from './my-profile/my-profile.component';
import { EmployeeComponent } from './employee/employee.component';
import { ProjectComponent } from './project/project.component';
import { WorkingExperienceComponent } from './working-experience/working-experience.component';
import { VersionComponent } from './version/version.component';
import { SharedModule } from '@shared/shared.module';
import { ContactInformationComponent } from './my-profile/contact-information/contact-information.component';
import { ProfileEducationComponent } from './my-profile/profile-education/profile-education.component';
import { CreateVersionComponent } from './my-profile/create-version/create-version.component';
import { TechnicalExpertisesComponent } from './my-profile/technical-expertises/technical-expertises.component';
import { PersonalAttributeComponent } from './my-profile/personal-attribute/personal-attribute.component';
import { WorkingExperiencesDialogComponent } from './my-profile/working-experiences-dialog/working-experiences-dialog.component';
import { NgxSliderModule } from '@angular-slider/ngx-slider';
import { ExportDialogComponent } from './my-profile/export-dialog/export-dialog.component';
import { DetailEmployeeComponent } from './employee/detail-employee/detail-employee.component';
import { DialogWorkingExperienceComponent } from './working-experience/dialog-working-experience/dialog-working-experience.component';
import { EmployeePositionComponent } from './employee-position/employee-position.component';
import { EmployeePositionDialogComponent } from './employee-position/employee-position-dialog/employee-position-dialog.component';
import { GroupSkillComponent } from './group-skill/group-skill.component';
import { GroupSkillDialogComponent } from './group-skill/group-skill-dialog/group-skill-dialog.component';
import { FakeSkillComponent } from './fake-skill/fake-skill.component';
import { FakeSkillDialogComponent } from './fake-skill/fake-skill-dialog/fake-skill-dialog.component';


@NgModule({
  declarations: [
    NccCvComponent,
    MyProfileComponent,
    EmployeeComponent,
    ProjectComponent,
    WorkingExperienceComponent,
    VersionComponent,
    ContactInformationComponent,
    ProfileEducationComponent,
    CreateVersionComponent,
    TechnicalExpertisesComponent,
    PersonalAttributeComponent,
    WorkingExperiencesDialogComponent,
    ExportDialogComponent,
    DetailEmployeeComponent,
    DialogWorkingExperienceComponent,
    EmployeePositionComponent,
    EmployeePositionDialogComponent,
    GroupSkillComponent,
    GroupSkillDialogComponent,
    FakeSkillComponent,
    FakeSkillDialogComponent
  ],
  imports: [
    CommonModule,
    NccCvRoutingModule,
    NgxSliderModule,
    SharedModule
  ]
})
export class NccCvModule { }

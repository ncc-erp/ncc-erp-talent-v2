import { DetailProjectComponent } from './detail-project/detail-project.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProjectRoutingModule } from './project-routing.module';
import { ListProjectComponent } from './list-project/list-project.component';
import { SharedModule } from '@shared/shared.module';
import { ProjectStaffComponent } from './project-staff/project-staff.component';
import { ProjectStaffFakeComponent } from './project-staff-fake/project-staff-fake.component';
import { DialogProjectComponent } from './dialog-project/dialog-project.component';


@NgModule({
  declarations: [
    ListProjectComponent,
    ProjectStaffComponent,
    ProjectStaffFakeComponent,
    DetailProjectComponent,
    DialogProjectComponent
  ],
  imports: [
    CommonModule,
    ProjectRoutingModule,
    SharedModule
  ]
})
export class ProjectModule { }

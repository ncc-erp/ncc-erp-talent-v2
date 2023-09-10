import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { EmployeeProfileResolver } from './../../core/resolver/employee-profile.resolver';
import { EmployeePositionComponent } from './employee-position/employee-position.component';
import { DetailEmployeeComponent } from './employee/detail-employee/detail-employee.component';
import { EmployeeComponent } from './employee/employee.component';
import { FakeSkillComponent } from './fake-skill/fake-skill.component';
import { GroupSkillComponent } from './group-skill/group-skill.component';
import { MyProfileComponent } from './my-profile/my-profile.component';
import { NccCvComponent } from './ncc-cv.component';
import { VersionComponent } from './version/version.component';
import { WorkingExperienceComponent } from './working-experience/working-experience.component';

const routes: Routes = [
  {
    path: '',
    component: NccCvComponent,
    canActivateChild: [AppRouteGuard],
    children: [
      {
        path: 'my-profile',
        component: MyProfileComponent,
        resolve: { profileResover: EmployeeProfileResolver }
      },
      {
        path: 'working-experience',
        component: WorkingExperienceComponent,
        resolve: { profileResover: EmployeeProfileResolver }
      },
      {
        path: 'project',
        loadChildren: () => import('./project/project.module').then(rs => rs.ProjectModule)
      },
      {
        path: 'version',
        component: VersionComponent
      },
      {
        path: 'employee-list',
        component: EmployeeComponent,
        resolve: { profileResover: EmployeeProfileResolver }
      },
      {
        path: 'employee/detail-employee/:id',
        component: DetailEmployeeComponent,
      },
      {
        path: 'employee-position',
        component: EmployeePositionComponent,
      },
      {
        path: 'group-skill',
        component: GroupSkillComponent,
      },
      {
        path: 'skill',
        component: FakeSkillComponent,
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class NccCvRoutingModule { }

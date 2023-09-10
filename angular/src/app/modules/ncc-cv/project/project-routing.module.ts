import { ProjectComponent } from './project.component';
import { ListProjectComponent } from './list-project/list-project.component';
import { DetailProjectComponent } from './detail-project/detail-project.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import * as path from 'path';

const routes: Routes = [
  {
    path: '',
    component: ProjectComponent,
    children: [
      { path: '', pathMatch: 'full', component: ListProjectComponent },
      { path: 'detail-project', component: DetailProjectComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProjectRoutingModule { }

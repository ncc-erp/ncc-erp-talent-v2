import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CategoryRoutingModule } from './category-routing.module';
import { CategoryComponent } from './category.component';
import { SkillComponent } from './skill/skill.component';
import { SharedModule } from '@shared/shared.module';
import { EducationTypesComponent } from './education-types/education-types.component';
import { EducationComponent } from './education/education.component';
import { CvSourcesComponent } from './cv-sources/cv-sources.component';
import { JobPositionsComponent } from './job-positions/job-positions.component';
import { CapabilitiesComponent } from './capabilities/capabilities.component';
import { CapabilitySettingComponent } from './capability-setting/capability-setting.component';
import { SkillDialogComponent } from './skill/skill-dialog/skill-dialog.component';
import { EducationTypesDialogComponent } from './education-types/education-types-dialog/education-types-dialog.component';
import { EducationDialogComponent } from './education/education-dialog/education-dialog.component';
import { CvSourceDialogComponent } from './cv-sources/cv-source-dialog/cv-source-dialog.component';
import { JobPositionDialogComponent } from './job-positions/job-position-dialog/job-position-dialog.component';
import { BranchComponent } from './branch/branch.component';
import { CapabilityDialogComponent } from './capabilities/capability-dialog/capability-dialog.component';
import { BranchDialogComponent } from './branch/branch-dialog/branch-dialog.component';
import { PickListCapabilityComponent } from './capability-setting/pick-list-capability/pick-list-capability.component';
import { SubPositionComponent } from './sub-position/sub-position.component';
import { SubPositionDialogComponent } from './sub-position/sub-position-dialog/sub-position-dialog.component';
import { PositionSettingComponent } from './position-setting/position-setting.component';
import { PositionSettingDialogComponent } from './position-setting/position-setting-dialog/position-setting-dialog.component';
import { LmsSettingDialogComponent } from './position-setting/lms-setting-dialog/lms-setting-dialog.component';
import { CloneCapabilitySettingComponent } from './capability-setting/clone-capability-setting/clone-capability-setting.component'
import { EditGuideLineDialogComponent } from './capability-setting/edit-guideline-dialog/edit-guideline-dialog.component';
import { PostsComponent } from './posts/posts.component';
import { PostDialogComponent } from './posts/post-dialog/post-dialog.component';
import { PostDetailComponent } from './posts/post-detail/post-detail.component'
import { ScoreSettingComponent } from './score-setting/score-setting.component';
import { DialogScoreSettingComponent } from './score-setting/dialog-score-setting/dialog-score-setting.component';
import {FormsModule} from '@angular/forms';

const dialogComponents = [
  EducationTypesDialogComponent,
  SkillDialogComponent,
  EducationDialogComponent,
  CvSourceDialogComponent,
  JobPositionDialogComponent,
  BranchDialogComponent,
  CapabilityDialogComponent,
  EditGuideLineDialogComponent,
  SubPositionDialogComponent,
  PositionSettingDialogComponent,
  LmsSettingDialogComponent,
  PostDialogComponent,
  PostDetailComponent,
]

@NgModule({
  declarations: [
    CategoryComponent,
    SkillComponent,
    EducationTypesComponent,
    EducationComponent,
    CvSourcesComponent,
    JobPositionsComponent,
    BranchComponent,
    CapabilitiesComponent,
    CapabilitySettingComponent,
    PickListCapabilityComponent,
    SubPositionComponent,
    PositionSettingComponent,
    ...dialogComponents,
    CloneCapabilitySettingComponent,
    PostsComponent,
    ScoreSettingComponent,
    DialogScoreSettingComponent,
    
  ],
  imports: [
    CommonModule,
    CategoryRoutingModule,
    SharedModule,
    FormsModule 
  ]
})
export class CategoryModule { }

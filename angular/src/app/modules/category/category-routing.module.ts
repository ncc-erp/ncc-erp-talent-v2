import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { PERMISSIONS_CONSTANT } from '@shared/permission/permissions';
import { CategoriesResolver } from './../../core/resolver/categories.resolver';
import { PositionSettingResolver } from './../../core/resolver/position-setting.resolver';
import { BranchComponent } from './branch/branch.component';
import { CapabilitiesComponent } from "./capabilities/capabilities.component";
import { CapabilitySettingComponent } from "./capability-setting/capability-setting.component";
import { PickListCapabilityComponent } from './capability-setting/pick-list-capability/pick-list-capability.component';
import { CategoryComponent } from "./category.component";
import { CvSourcesComponent } from "./cv-sources/cv-sources.component";
import { EducationTypesComponent } from "./education-types/education-types.component";
import { EducationComponent } from "./education/education.component";
import { JobPositionsComponent } from "./job-positions/job-positions.component";
import { PositionSettingComponent } from './position-setting/position-setting.component';
import { SkillComponent } from "./skill/skill.component";
import { SubPositionComponent } from './sub-position/sub-position.component';
import { PostsComponent } from "./posts/posts.component";
import {ScoreSettingComponent} from "./score-setting/score-setting.component";
import { CandidateLevelResolver } from "@app/core/resolver/candidate-level.resolver";
import {CanditateLanguageComponent} from "../candidate/canditate-language/canditate-language.component";

const routes: Routes = [
  {
    path: "",
    component: CategoryComponent,
    children: [
      {
        path: "",
        component: EducationTypesComponent,
      },
      {
        path: "education-types",
        component: EducationTypesComponent,
        data: { permission: PERMISSIONS_CONSTANT.Pages_EducationTypes },
      },
      {
        path: "educations",
        component: EducationComponent,
        resolve: { categoriesResolver: CategoriesResolver },
        data: { permission: PERMISSIONS_CONSTANT.Pages_Educations },
      },
      {
        path: "skill",
        component: SkillComponent,
        data: { permission: PERMISSIONS_CONSTANT.Pages_Skills },
      },
      {
        path: "cv-sources",
        component: CvSourcesComponent,
        resolve: { categoriesResolver: CategoriesResolver },
        data: { permission: PERMISSIONS_CONSTANT.Pages_CVSources },
      },
      {
        path: "branches",
        component: BranchComponent,
        data: { permission: PERMISSIONS_CONSTANT.Pages_Branches },
      },
      {
        path: "language",
        component: CanditateLanguageComponent,
      },
      {
        path: "job-positions",
        component: JobPositionsComponent,
        data: { permission: PERMISSIONS_CONSTANT.Pages_JobPositions },
      },
      {
        path: "sub-positions",
        component: SubPositionComponent,
        resolve: { categoriesResolver: CategoriesResolver },
      },
      {
        path: "capabilities",
        component: CapabilitiesComponent,
        data: { permission: PERMISSIONS_CONSTANT.Pages_Capabilities },
      },
      {
        path: "capability-setting",
        component: CapabilitySettingComponent,
        resolve: { categoriesResolver: CategoriesResolver },
        data: { permission: PERMISSIONS_CONSTANT.Pages_CapabilitySettings },
      },
      {
        path: "position-setting",
        component: PositionSettingComponent,
        resolve: { categoriesResolver: CategoriesResolver, positionSettingResolver: PositionSettingResolver },
      },
      {
        path: "capability-setting/capabilities",
        component: PickListCapabilityComponent,
        resolve: { categoriesResolver: CategoriesResolver },
      },
      {
        path: "posts",
        component: PostsComponent,
        data: { permission: PERMISSIONS_CONSTANT.Pages_Posts },
      },
      {
        path: "score-setting",
        component: ScoreSettingComponent,
        resolve: { categoriesResolver: CategoriesResolver, CandidateLevelResolver: CandidateLevelResolver },
        data: { permission: PERMISSIONS_CONSTANT.Pages_ScoreSettings },
      }
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CategoryRoutingModule { }

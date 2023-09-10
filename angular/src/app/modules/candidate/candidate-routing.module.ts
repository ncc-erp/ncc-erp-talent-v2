
import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { CandidateResolver } from './../../core/resolver/candidate.resolver';
import { CandidateInternListComponent } from './candidate-intern-list/candidate-intern-list.component';
import { CandidateOfferListComponent } from './candidate-offer-list/candidate-offer-list.component';
import { CandidateOnboardListComponent } from './candidate-onboard-list/candidate-onboard-list.component';
import { CandidateStaffListComponent } from './candidate-staff-list/candidate-staff-list.component';
import { CandidateComponent } from './candidate.component';
import { CandidateInterviewResolver } from './../../core/resolver/candidate-interview.resolver';
import { CandidateInterviewComponent } from './candidate-interview/candidate-interview.component';
import { CreateCandidateComponent } from '@shared/pages/create-candidate/create-candidate.component';
import {ApplyCvComponent} from "../candidate/apply-cv/apply-cv.component";
import {ExternalCvComponent} from "./external-cv/external-cv.component";
import {DetailExternalCvComponent} from "./external-cv/detail-external-cv/detail-external-cv.component";

const routes: Routes = [
  {
    path: '',
    component: CandidateComponent,
    resolve: { candidateResolver: CandidateResolver },
    children: [
      { path: '', pathMatch: 'full', component: CandidateStaffListComponent },
      { path: 'staff-list', component: CandidateStaffListComponent },
      { path: 'staff-list/:id', component: CreateCandidateComponent },
      { path: 'staff-list/create', component: CreateCandidateComponent },
      { path: 'intern-list', component: CandidateInternListComponent },
      { path: 'intern-list/:id', component: CreateCandidateComponent },
      { path: 'intern-list/create', component: CreateCandidateComponent },
      { path: 'offer-list', component: CandidateOfferListComponent },
      { path: 'onboard-list', component: CandidateOnboardListComponent },
      { path: 'interview-list', component: CandidateInterviewComponent, resolve: { interviewer: CandidateInterviewResolver } },
      {
        path: "external-cv",
        component: ExternalCvComponent,
      },
      {
        path: "external-cv/detail/:id",
        component: DetailExternalCvComponent,
      },
      {
        path: "apply-cv",component: ApplyCvComponent,
      },
      {
        path: "apply-cv/create",component: CreateCandidateComponent,
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CandidateRoutingModule { }

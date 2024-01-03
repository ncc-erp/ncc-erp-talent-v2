import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CandidateRoutingModule } from './candidate-routing.module';
import { CandidateComponent } from './candidate.component';
import { CandidateStaffListComponent } from './candidate-staff-list/candidate-staff-list.component';
import { CandidateInternListComponent } from './candidate-intern-list/candidate-intern-list.component';
import { CandidateOfferListComponent } from './candidate-offer-list/candidate-offer-list.component';
import { CandidateOnboardListComponent } from './candidate-onboard-list/candidate-onboard-list.component';
import { CandidateInterviewComponent } from './candidate-interview/candidate-interview.component';
import { SharedModule } from '@shared/shared.module';
import { ApplyCvComponent } from '../candidate/apply-cv/apply-cv.component';
import { ExternalCvComponent } from '../candidate/external-cv/external-cv.component';
import { DetailExternalCvComponent } from '../candidate/external-cv/detail-external-cv/detail-external-cv.component';
import { ShowMetadataComponent } from './external-cv/showmetadata/showmetadata.component';
import { CanditateLanguageComponent } from './canditate-language/canditate-language.component';
import { LanguageDialogComponent } from './canditate-language/language-dialog/language-dialog.component';
@NgModule({
  declarations: [
    CandidateComponent,
    CandidateStaffListComponent,
    CandidateInternListComponent,
    CandidateOfferListComponent,
    CandidateOnboardListComponent,
    CandidateInterviewComponent,
    ExternalCvComponent,
    DetailExternalCvComponent,
    ShowMetadataComponent,
    ApplyCvComponent,
    CanditateLanguageComponent,
    LanguageDialogComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    CandidateRoutingModule,
  ]
})
export class CandidateModule { }

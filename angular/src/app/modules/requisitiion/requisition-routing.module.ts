import { RequisitionResolver } from './../../core/resolver/requisition.resolver';
import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { RequisitionComponent } from "./requisition.component";
import { RequisitionInternComponent } from "./requisition-intern/requisition-intern.component";
import { RequisitionStaffComponent } from "./requisition-staff/requisition-staff.component";
import { RequisitionDetailComponent } from '@shared/pages/requisition-detail/requisition-detail.component';

const routes: Routes = [
  {
    path: '',
    component: RequisitionComponent,
    resolve: { requisitionResolver: RequisitionResolver },
    children: [
      { path: "", pathMatch: "full", component: RequisitionStaffComponent },
      { path: "req-staff", component: RequisitionStaffComponent },
      { path: 'req-staff/:id', component: RequisitionDetailComponent},
      { path: "req-intern", component: RequisitionInternComponent },
      { path: 'req-intern/:id', component: RequisitionDetailComponent},
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class RequisitionRoutingModule { }

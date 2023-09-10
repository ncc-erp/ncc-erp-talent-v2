import { RequisitionComponent } from './requisition.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RequisitionRoutingModule } from './requisition-routing.module';
import { RequisitionStaffComponent } from './requisition-staff/requisition-staff.component';
import { RequisitionInternComponent } from './requisition-intern/requisition-intern.component';
import { SharedModule } from '@shared/shared.module';
import { RequisitionStaffDialogComponent } from './requisition-staff/requisition-staff-dialog/requisition-staff-dialog.component';
import { RequisitionInternDialogComponent } from './requisition-intern/requisition-intern-dialog/requisition-intern-dialog.component';
import { CloneAllDialogComponent } from './requisition-intern/clone-all-dialog/clone-all-dialog.component';

@NgModule({
  declarations: [
    RequisitionComponent,
    RequisitionStaffComponent,
    RequisitionInternComponent,
    RequisitionStaffDialogComponent,
    RequisitionInternDialogComponent,
    CloneAllDialogComponent,
  ],
  imports: [
    CommonModule,
    RequisitionRoutingModule,
    SharedModule
  ]
})
export class RequisitionModule { }

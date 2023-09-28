import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ExportDialogComponent } from './export-dialog.component';
import {SharedModule} from '@shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
  ],
  declarations: [
    ExportDialogComponent
  ]
})
export class ExportDialogModule { }

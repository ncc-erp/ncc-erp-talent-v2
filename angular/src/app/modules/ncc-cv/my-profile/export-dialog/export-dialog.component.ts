import { ToastMessageType } from '@shared/AppEnums';
import { ExportService } from './../../../../core/services/employee-profile/export.service';
import { ExportFakeService } from './../../../../core/services/employee-profile/export-fake.service';
import { Component, Injector, OnInit } from '@angular/core';
import { AppSessionService } from '@shared/session/app-session.service';
import { PermissionCheckerService } from 'abp-ng2-module';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AppComponentBase } from '@shared/app-component-base';
import { MESSAGE } from '@shared/AppConsts';

@Component({
  selector: 'talent-export-dialog',
  templateUrl: './export-dialog.component.html',
  styleUrls: ['./export-dialog.component.scss']
})
export class ExportDialogComponent extends AppComponentBase implements OnInit {
  labelPosition: string;
  isHiddenYear = false;
  filePathofExport: string;
  fileNameExport: string;
  isSale: boolean;
  isUser: boolean;
  isEmployee = false;
  id: number;
  data;

  constructor(
    injector: Injector,
    public config: DynamicDialogConfig,
    public dialogRef: DynamicDialogRef,
    private _permissionChecker: PermissionCheckerService,
    private _export: ExportService,
    private _exportFake: ExportFakeService,
    private session: AppSessionService,
  ) {
    super(injector);
   }

  ngOnInit(): void {
    this.data = this.config.data;
    this.labelPosition = '1';
    this.isSale = this._permissionChecker.isGranted('Employee.EditAsPM');
    this.isUser = this.data[1];
  }

  exportCV() {
    if (this.isSale && !this.isUser) {
      this.data[0].typeOffile = Number(this.labelPosition);
      this.data[0].isHiddenYear = this.isHiddenYear;
      this._exportFake.ExportCVFake(this.data[0]).subscribe(res => {
        if (res.success && res?.result) {
          this.filePathofExport = res.result;
          this.fileNameExport = 'my_profile';
          this.dialogRef.close(res);
          this.downloadURI(this.filePathofExport, this.fileNameExport);
        }
      });
      return;
    }
    this._export.exportCV(this.data[0].userId,
      Number(this.labelPosition),
      this.isHiddenYear,
      this.data[0].versionId).subscribe(res => {
        if (res.success && res?.result) {
          this.filePathofExport = res.result;
          this.fileNameExport = 'my_profile';
          this.dialogRef.close(res);
          this.downloadURI(this.filePathofExport, this.fileNameExport);
        }

      });
  }

  downloadURI(uri, fileName) {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = uri;
    link.target = 'blank';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
}

import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { DefaultRoute } from '@shared/AppEnums';
import { ExternalCvService } from '../../../../core/services/categories/external-cv.service';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { ExternalCv } from '@app/core/models/categories/external-cv.model';
import { DateFormat} from '@shared/AppConsts';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { ShowMetadataComponent } from '../showmetadata/showmetadata.component';
import { DialogService } from 'primeng/dynamicdialog';
import { CustomDialogService } from '@app/core/services/custom-dialog/custom-dialog.service';

@Component({
  selector: 'talent-detail-external-cv',
  templateUrl: './detail-external-cv.component.html',
  styleUrls: ['./detail-external-cv.component.scss']
})
export class DetailExternalCvComponent extends AppComponentBase implements OnInit {

  id: number;
  externalCv: ExternalCv = new ExternalCv();
  public readonly DATE_FORMAT = DateFormat;
  constructor(
    injector: Injector,
    public _utilities:UtilitiesService,
    private _externalCv: ExternalCvService,
    public dialogService: DialogService,
    private customDialogService: CustomDialogService
  ) {
    super(injector);
   }

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get("id"));
    this._externalCv
      .getExternalCvById(this.id)
      .subscribe((rs:ApiResponse<ExternalCv>) => {
        if (rs.success) {
          this.externalCv = rs.result;
          this.breadcrumbConfig = this.getBreadcrumbConfig();
        }
      });
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [
        { label: "Categories", styleClass: 'menu-item-click', routerLink: DefaultRoute.Category },
        { label: "External CV", styleClass: 'menu-item-click', routerLink: DefaultRoute.ExternalCV },
        { label: "Detail External CV " + this.externalCv.name }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }

  openDialog(externalCv: ExternalCv) {
    this.dialogService.open(ShowMetadataComponent, {
      header: `${externalCv.name} - ${externalCv?.branchName} - ${externalCv?.positionName} - Metadata`,
      width: "40%",
      contentStyle: { "max-height": "500px", overflow: "visible" },
      baseZIndex: 10000,
      data: { id: this.id }
    });
  }

  openPDFDocViewer(url: string) {
    this.customDialogService.openPDFDocViewerDialog(url);
  }
}

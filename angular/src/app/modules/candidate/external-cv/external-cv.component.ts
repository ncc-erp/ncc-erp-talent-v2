import { Component, OnInit,Injector } from '@angular/core';
import {
  PagedListingComponentBase,
  PagedRequestDto
} from '@shared/paged-listing-component-base';
import { DateFormat } from "@shared/AppConsts";
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ExternalCv, ExternalCvPayloadListData } from '@app/core/models/categories/external-cv.model';
import { ExternalCvService } from '@app/core/services/categories/external-cv.service';
import { ActionEnum, DefaultRoute, SortType } from '@shared/AppEnums';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { JsonHubProtocol } from '@aspnet/signalr';
import { ShowMetadataComponent } from './showmetadata/showmetadata.component';
import { DialogService } from 'primeng/dynamicdialog';

class PagedExternalCvRequestDto extends PagedRequestDto {
  keyword: string;
}

@Component({
  selector: 'talent-external-cv',
  templateUrl: './external-cv.component.html',
  styleUrls: ['./external-cv.component.scss'],
  animations: [appModuleAnimation()]
})
export class ExternalCvComponent extends PagedListingComponentBase<ExternalCv> implements OnInit {
  externalsCv: ExternalCv[] = [];
  keyword = '';
  public readonly DATE_FORMAT = DateFormat;
  public readonly SORT_TYPE = SortType;
  id: number;

  constructor(
    injector: Injector,
    private _externalCv: ExternalCvService,
    public _utilities:UtilitiesService,
    public dialogService: DialogService,
  ) {
    super(injector);
    this.breadcrumbConfig = this.getBreadcrumbConfig();
   }

  ngOnInit(): void {
  }

  list(
    request: PagedExternalCvRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    const payload = this.getPayLoad(request);
   
    this.subs.add(
      this._externalCv.getAllPagging(payload).subscribe((rs) => {
        this.externalsCv = [];
        if (rs.success) {
          this.externalsCv = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  private getPayLoad(request: PagedRequestDto): ExternalCvPayloadListData {
    const payload: any = { ...request }
    payload.searchText = this.keyword;
    return payload;
  }

  delete(externalCv:ExternalCv ): void {}

  navigateToInfoExternalCv(externalCv: ExternalCv): void{
    this.router.navigate(["/app/categories/external-cv/detail",externalCv.id])
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Candidate", routerLink: DefaultRoute.Candidate, styleClass: 'menu-item-click' }, { label: "External CV" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
  
  openDialog(externalCv: ExternalCv) {
    this.dialogService.open(ShowMetadataComponent, {
      header: `${externalCv.name} - ${externalCv?.branchName} - ${externalCv?.positionName} - Metadata`,
      width: "40%",
      contentStyle: {"font-size":"2.50rem", "max-height": "500px", overflow: "visible"  },
      baseZIndex: 10000,
      data: {
        id: externalCv.id
      }
    });
  }
}
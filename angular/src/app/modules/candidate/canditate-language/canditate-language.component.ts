import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import {CandidateLanguage, CandidateLanguageConfigDiaLog} from '@app/core/models/candidate/candidate-language.model';
import {CandidateLanguageService} from '@app/core/services/candidate/candidate-language.service';
import {API_RESPONSE_STATUS, ActionEnum, DefaultRoute, ToastMessageType} from '@shared/AppEnums';
import {ApiResponse, PagedListingComponentBase, PagedRequestDto} from '@shared/paged-listing-component-base';
import {DialogService, DynamicDialogRef} from 'primeng/dynamicdialog';
import {LanguageDialogComponent} from './language-dialog/language-dialog.component';
import {MESSAGE} from '@shared/AppConsts';

@Component({
  selector: 'talent-canditate-language',
  templateUrl: './canditate-language.component.html',
  styleUrls: ['./canditate-language.component.scss']
})
export class CanditateLanguageComponent extends PagedListingComponentBase<CandidateLanguage> implements OnInit, OnDestroy {

  public language: CandidateLanguage[] = [];
  private dialogRef: DynamicDialogRef;

  constructor(
    injector: Injector,
    public dialogService: DialogService,
    private _language: CandidateLanguageService
  ) {
    super(injector);
  }

  ngOnInit() {

    this.breadcrumbConfig = this.getBreadcrumbConfig();
  }

  ngOnDestroy() {
    super.ngOnDestroy();
    if (this.dialogRef) this.dialogRef.close()
  }
  protected list(request: PagedRequestDto,pageNumber: number,finishedCallback: Function): void {
    this.subs.add(
      this._language.getAllPagging(request).subscribe((rs) => {
        this.language = [];
        if (rs.success) {
          this.language = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }
  protected delete(entity: CandidateLanguage): void {
    const deleteRequest = this._language.delete(entity.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, entity.name).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
      })
    );
  }
  openDialog(obj: CandidateLanguage, dialogAction: ActionEnum) {
    const dialogConfig: CandidateLanguageConfigDiaLog = { candidateLanguage: obj, action: dialogAction };
    const dialogRef = this.dialogService.open(LanguageDialogComponent, {
      header: `${dialogConfig.action} Language`,
      width: "40%",
      contentStyle: { "max-height": "100%", overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<CandidateLanguage>) => {
      if (dialogConfig.action === ActionEnum.UPDATE && res) {
        const index = this.language.findIndex((x) => x.id == res.result.id);
        this.language[index] = res.result;
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.name);
        return;
      }
      this.refresh();
    });
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Categories", routerLink: DefaultRoute.Category, styleClass: 'menu-item-click' }, { label: "Language" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}

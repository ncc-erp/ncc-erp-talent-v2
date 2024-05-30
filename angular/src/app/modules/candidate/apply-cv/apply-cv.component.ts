import { Component, OnInit, Injector, Input } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';
import { UtilitiesService } from '../../../core/services/utilities.service';
import { CANDIDATE_DETAILT_TAB_DEFAULT, CreationTimeEnum, DefaultRoute, SortType, UserType } from '../../../../shared/AppEnums';
import { Filter, PagedListingComponentBase, PagedRequestDto } from '../../../../shared/paged-listing-component-base';
import { BreadCrumbConfig } from '../../../core/models/common/common.dto';
import { DateFormat, FILTER_TIME } from '../../../../shared/AppConsts';
import { ApplyCv, ApplyCvPayloadListData } from '../../../core/models/apply-cv/apply-cv-list.model';
import { ApplyCVService } from '../../../core/services/candidate/apply-cv-list.service';
import {TalentDateTime} from '@shared/components/date-selector/date-selector.component';
import { CustomDialogService } from '@app/core/services/custom-dialog/custom-dialog.service';

class PagedApplyCvRequestDto extends PagedRequestDto {
  keyword: string;
}

@Component({
  selector: 'talent-apply-cv',
  templateUrl: './apply-cv.component.html',
  styleUrls: ['./apply-cv.component.scss']
})
export class ApplyCvComponent extends PagedListingComponentBase<ApplyCv> implements OnInit {

  applyCvList: ApplyCv[] = [];
  keyword = '';
  public readonly FILTER_TIME = FILTER_TIME;
  public readonly DATE_FORMAT = DateFormat;
  public readonly SORT_TYPE = SortType;
  id: number;

  breadcrumbConfig: BreadCrumbConfig;
  first = 0;
  searchWithCreationTime: TalentDateTime;

  @Input() isOpenDetail = true;
  usertype: null;
  branchId: null;

  searchDetail:{
    branchId: null;
  }

  constructor(
    injector: Injector,
    private _applyCv: ApplyCVService,
    public _utilities:UtilitiesService,
    public dialogService: DialogService,
    private customDialogService: CustomDialogService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
  }

  list( request: PagedApplyCvRequestDto, pageNumber: number, finishedCallback: Function): void {
    const payload = this.getPayLoad(request);

    this.subs.add(
      this._applyCv.getAllPagging(payload).subscribe((rs) => {
        this.applyCvList = [];
        if (rs.success) {
          this.applyCvList = rs.result.items;
          this.applyCvList.map(item =>{
            if(item.postId <= 0){
              item.postId = null;
            };
          })
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }
  onTalentDateChange(talentDateTime: TalentDateTime) {
    this.searchWithCreationTime = talentDateTime;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  private getPayLoad(request: PagedRequestDto): ApplyCvPayloadListData {
    const payload: any = { ...request };
    const filterItems: Filter[] = [];
    payload.usertype = this.usertype;
    payload.searchText = this.keyword;
    payload.branchId =this.branchId;

    if (this.searchWithCreationTime && this.searchWithCreationTime.dateType !== CreationTimeEnum.ALL) {
      payload.fromDate = this.searchWithCreationTime?.fromDate?.format(DateFormat.YYYY_MM_DD);
      payload.toDate = this.searchWithCreationTime?.toDate?.format(DateFormat.YYYY_MM_DD);
    }
    payload.filterItems = filterItems;
    return payload;
  }

  onSearchEnter() {
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  delete(applyCv:ApplyCv): void {}

  navigateToCanDetail(candExist: { userType: number ,positionType: string }) {
    const userType = candExist.positionType === 'Intern' ? 0 : 1
    const routhPath = userType === UserType.INTERN ? 'intern-list' : 'staff-list'
    localStorage.setItem('idApplyCV', JSON.stringify(candExist));
    const url = this.router.createUrlTree([`/app/candidate/${routhPath}/create` ],
      { queryParams: { userType: userType } });
    window.open(url.toString(), '_blank')
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Candidate", routerLink: DefaultRoute.Candidate, styleClass: 'menu-item-click' }, { label: "Apply CV" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }

  openPDFDocViewer(url: string) {
    this.customDialogService.openPDFDocViewerDialog(url);
  }
}

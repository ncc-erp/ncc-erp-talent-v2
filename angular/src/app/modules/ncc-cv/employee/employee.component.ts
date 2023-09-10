import { Component, Injector } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Branch } from '@app/core/models/categories/branch.model';
import { EmployeeDto, PositionDto } from '@app/core/models/employee-profile/profile-model';
import { PERMISSIONS_CONSTANT } from '@shared/permission/permissions';
import { DefaultRoute } from '@shared/AppEnums';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { EmployeeProfileService } from './../../../core/services/employee-profile/employee-profile.service';
import { UtilitiesService } from './../../../core/services/utilities.service';

class PagedEmployeeRequestDto extends PagedRequestDto {
  searchText: string = '';
  positionId?: string;
  branchId?: string;
}

@Component({
  selector: 'talent-employee',
  templateUrl: './employee.component.html',
  styleUrls: ['./employee.component.scss']
})
export class EmployeeComponent extends PagedListingComponentBase<EmployeeDto> {
  EMPLOYEE_VIEWLIST = PERMISSIONS_CONSTANT.Employee_ViewList;
  EMPLOYEE_VIEWDETAIL = PERMISSIONS_CONSTANT.Employee_ViewDetail;
  searchForm: FormGroup;
  listEmployee: EmployeeDto[] = [];
  searchText = '';
  isActive: boolean | null;
  advancedFiltersVisible = false;
  branch: Branch[] = [];
  position: PositionDto[] = [];
  positionVersion: PositionDto[] = [];
  language: PositionDto[] = [];
  positionId: number;
  branchId: number;
  positionVersId: number;
  languageId: number;
  employeeNumber = 0;
  pageSizeType = 20;
  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
    private _employeeProfile: EmployeeProfileService,
  ) {
    super(injector);
  }

  ngOnInit() {
    this.breadcrumbConfig = this.getBreadCrumbConfig();
  }

  list(request: PagedEmployeeRequestDto, pageNumber: number, finishedCallback: Function): void {
    const data = {
      name: this.searchText.trim().replace(/ +/g, ' '),
      positionId: this.positionId,
      branchId: this.branchId,
      languageId: this.languageId,
      positionVersId: this.positionVersId,
      maxResultCount: request.maxResultCount,
      skipCount: request.skipCount,
    };
    this._employeeProfile.getAllEmployeePaging(data).pipe(
      finalize(() => {
        finishedCallback();
      })
    ).subscribe(res => {
      this.isLoading = res.loading;
      this.listEmployee = [];
      if (res?.success) {
        this.listEmployee = res.result.items;
        this.employeeNumber = res.result.totalCount;
        this.showPaging(res.result, pageNumber);
      }
    });
  }

  changePageSize() {
    if (this.pageSizeType > this.employeeNumber) {
      this.pageNumber = 1;
    }
    this.pageSize = this.pageSizeType;
    this.getDataPage(1);
  }

  findId(list: any[], id: number) {
    const foundId = list.find(el => el.id === id);
    return foundId ? foundId.id : '';
  }

  getNameById(list: any[], id: number): string {
    const founded = list.find(el => el.id === id);
    return founded ? founded.name : '';
  }

  clearFilters(): void {
    this.searchText = '';
    this.positionId = null;
    this.branchId = null;
    this.positionVersId = null;
    this.languageId = null;
    this.getDataPage(1);
  }

  delete(): void { }

  detailEmployee(id) {
    if (this.permission.isGranted(PERMISSIONS_CONSTANT.Employee_ViewDetail)) {
      this.router.navigate(['app/ncc-cv/employee/detail-employee', id]);
    }
  }

  private getBreadCrumbConfig() {
    return {
      menuItem: [{ label: "Employee Profile", routerLink: DefaultRoute.Employee_Profile, styleClass: 'menu-item-click' }, { label: "Employee" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}

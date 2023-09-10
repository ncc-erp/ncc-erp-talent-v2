import { Component, Injector } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  PagedListingComponentBase,
  PagedRequestDto
} from '@shared/paged-listing-component-base';
import {
  RoleServiceProxy,
  RoleDto,
  RoleDtoPagedResultDto
} from '@shared/service-proxies/service-proxies';
import { LazyLoadEvent } from 'primeng/api';
import { DefaultRoute } from '@shared/AppEnums';

class PagedRolesRequestDto extends PagedRequestDto {
  keyword: string;
}

@Component({
  templateUrl: './roles.component.html',
  animations: [appModuleAnimation()]
})
export class RolesComponent extends PagedListingComponentBase<RoleDto> {
  roles: RoleDto[] = [];
  keyword = '';

  constructor(
    injector: Injector,
    private _rolesService: RoleServiceProxy,
  ) {
    super(injector);
    this.breadcrumbConfig = this.getBreadcrumbConfig();
  }
  

  createRole() {
    this.router.navigate(['create'], { relativeTo: this.route });
  }

  list(
    request: PagedRolesRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    this.isLoading = true;

    this._rolesService
      .getAll(request.keyword, request.skipCount, request.maxResultCount)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: RoleDtoPagedResultDto) => {
        this.isLoading = false;
        this.roles = result.items;
        this.showPaging(result, pageNumber);
      });
  }

  delete(role: RoleDto): void {
    abp.message.confirm(
      this.l('RoleDeleteWarningMessage', role.displayName),
      undefined,
      (result: boolean) => {
        if (result) {
          this._rolesService
            .delete(role.id)
            .pipe(
              finalize(() => {
                abp.notify.success(this.l('SuccessfullyDeleted'));
                this.refresh();
              })
            )
            .subscribe(() => { });
        }
      }
    );
  }

  editRole(role: RoleDto): void {
    this.router.navigate(["/app/admin/roles/edit", role.id])
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Admin", routerLink: DefaultRoute.Admin, styleClass: 'menu-item-click' }, { label: "Roles" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }

}

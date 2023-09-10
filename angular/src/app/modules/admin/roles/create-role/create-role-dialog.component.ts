import { DefaultRoute } from '@shared/AppEnums';
import { ToastMessageType } from './../../../../../shared/AppEnums';
import { ApiResponse } from '@shared/paged-listing-component-base';
import {
  Component,
  Injector,
  OnInit,
  EventEmitter,
  Output,
} from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import {
  RoleServiceProxy,
  RoleDto,
  PermissionDto,
  CreateRoleDto,
  PermissionDtoListResultDto
} from '@shared/service-proxies/service-proxies';
import { forEach as _forEach, map as _map } from 'lodash-es';
import { TreeNode } from 'primeng/api';
import { RoleService } from '@app/core/services/roles/role.service';
import { SystemPermission } from '@app/core/models/role/role.model';
import { MESSAGE } from '@shared/AppConsts';

@Component({
  templateUrl: './create-role-dialog.component.html'
})
export class CreateRoleDialogComponent extends AppComponentBase
  implements OnInit {
  disabledSaveBtn = false;
  role = new RoleDto();
  permissions: SystemPermission[] = [];
  checkedPermissionsMap: { [key: string]: boolean } = {};
  defaultPermissionCheckedStatus = true;
  grantedPermissionSelection: TreeNode[] = [];

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _roleService: RoleServiceProxy,
    private _role: RoleService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
    this._role
      .getAllPermissions()
      .subscribe((rs: ApiResponse<SystemPermission[]>) => {
        if (rs.success) {
          this.permissions = rs.result;
          this.mapToTree(this.permissions);
        }
      });
  }

  mapToTree(node: any) {
    for (let i = 0; i < node.length; i++) {
      if (node[i].children)
        this.mapToTree(node[i].children);

      node[i] = {
        ...node[i],
        "label": node[i].displayName,
        "data": node[i].name,
      } as TreeNode;

      this.grantedPermissionSelection.push(node[i]);
    }
  }

  getCheckedPermissions(): string[] {
    const permissions: string[] = [];
    _forEach(this.grantedPermissionSelection, function (item) {
      permissions.push(item.data);
    });
    return permissions;
  }

  save(): void {
    this.disabledSaveBtn = true;

    const role = new CreateRoleDto();
    role.init(this.role);
    role.grantedPermissions = this.getCheckedPermissions();

    this._roleService
      .create(role)
      .subscribe(
        () => {
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CREATE_SUCCESS);
          this.router.navigate(["/app/admin/roles"]);
        },
        () => {
          this.disabledSaveBtn = false;
        },
        () => {
          this.disabledSaveBtn = false;
        }
      );
  }
  cancel() {
    this.router.navigate(["/app/admin/roles"]);
  }
  itemClick($event) {
    //this.router.navigate(["/app"]);
  }
  private getBreadcrumbConfig() {
    return {
      menuItem: [
        { label: "Admin", styleClass: 'menu-item-click', routerLink: DefaultRoute.Admin },
        { label: "Role", styleClass: 'menu-item-click', routerLink: DefaultRoute.Admin },
        { label: "Create", disabled: true }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}

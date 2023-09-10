import { DefaultRoute } from '@shared/AppEnums';
import { ToastMessageType } from './../../../../../shared/AppEnums';
import { RoleForEdit, RoleEdit } from './../../../../core/models/role/role.model';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { RoleService } from './../../../../core/services/roles/role.service';
import {
  Component,
  Injector,
  OnInit,
} from '@angular/core';
import { forEach as _forEach, includes as _includes, map as _map } from 'lodash-es';
import { AppComponentBase } from '@shared/app-component-base';
import {
  RoleServiceProxy,
  RoleDto,
} from '@shared/service-proxies/service-proxies';
import { TreeNode } from 'primeng/api';
import { MESSAGE } from '@shared/AppConsts';

export enum ROLE_TAB {
  DETAIL = 0,
  PERMISSION = 1,
  USER = 2
}

@Component({
  templateUrl: './edit-role-dialog.component.html',
  styleUrls: ['./edit-role-dialog.component.scss']
})
export class EditRoleDialogComponent extends AppComponentBase implements OnInit {
  disabledSaveBtn = false;
  id: number;
  role = new RoleEdit();
  permissions: any[];
  grantedPermissionNames: string[];
  checkedPermissionsMap: { [key: string]: boolean } = {};
  grantedPermissionSelection: TreeNode[] = [];

  ROLE_TAB = ROLE_TAB;
  tabActived: ROLE_TAB;

  constructor(
    injector: Injector,
    private _roleService: RoleServiceProxy,
    private _role: RoleService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get("id"));
    this._role
      .getRoleForEdit(this.id)
      .subscribe((rs: ApiResponse<RoleForEdit>) => {
        if (rs.success) {
          this.role = rs.result.role;
          this.permissions = this.disabledUpdateRoleAdmin(rs.result.permissions, this.role.isStatic);
          this.grantedPermissionNames = rs.result.grantedPermissionNames;
          this.mapToTree(this.permissions, this.role.isStatic);
          this.breadcrumbConfig = this.getBreadcrumbConfig();
        }
      });
  }

  onNodeSelect(node: TreeNode) {
    node.partialSelected = false;
    this.addNodeParentToSelected(node);
  }
  addNodeParentToSelected(node: TreeNode) {
    let nodeParent: TreeNode = node.parent;
    if (!nodeParent) return;
    const exist = this.grantedPermissionSelection.includes(nodeParent);
    if (!exist) this.grantedPermissionSelection.push(nodeParent);
    this.checkParentNodeChecked(nodeParent);
    this.addNodeParentToSelected(nodeParent);
  }

  private checkParentNodeChecked(node: TreeNode) {
    let count = 0;
    if (!node.children?.length) return;
    for (let i = 0; i < node.children.length; i++) {
      if (this.grantedPermissionSelection.includes(node.children[i]) && node.children[i].partialSelected == false)
        count++;
    }
    if (count < node.children.length) node.partialSelected = true;
  }

  nodeUnselect(node: TreeNode) {
    this.addNodeParentToUnselected(node)
  }
  addNodeParentToUnselected(node: TreeNode) {
    let nodeParent: TreeNode = node.parent;
    if (!nodeParent) return;

    const isCanAddParentNode = this.checkParentNodeUnhecked(nodeParent);
    const exist = this.grantedPermissionSelection.includes(nodeParent);
    if (!exist && isCanAddParentNode) this.grantedPermissionSelection.push(nodeParent);
    this.addNodeParentToUnselected(nodeParent);
  }
  private checkParentNodeUnhecked(node: TreeNode) {
    let count = 0;
    let countChildSelected = 0;
    if (!node.children?.length) return;
    const length = node.children?.length;
    for (let i = 0; i < length; i++) {
      if (!this.grantedPermissionSelection.includes(node.children[i])) {
        countChildSelected++;
        continue;
      }

      if (this.grantedPermissionSelection.includes(node.children[i]) && node.children[i].partialSelected)
        count++;
    }
    if (count > 0) node.partialSelected = true;

    if (countChildSelected == length) return false;
    return true;
  }

  disabledUpdateRoleAdmin(permission: any[], isStatic: boolean) {
    if (!isStatic) return permission;
    permission.forEach(item => {
      item['selectable'] = false;
    })
    return permission;
  }

  mapToTree(node: any, isStatic: boolean) {
    let count = 0;
    let countNodeSelected;
    for (let i = 0; i < node.length; i++) {
      if (node[i].children) {
        countNodeSelected = this.mapToTree(node[i].children, isStatic);
      }

      let isHasFullPS = node[i].children?.includes(s => s.partialSelected == true);
      let isPartialSelected = this.isPartialSelected(node[i].children?.length, countNodeSelected, isHasFullPS, node[i].name);

      node[i] = {
        ...node[i],
        "label": node[i].displayName,
        "data": node[i].name,
        "selectable": !isStatic,
        "partialSelected": isPartialSelected
      } as TreeNode;

      if (this.isPermissionChecked(node[i].data)) {
        this.grantedPermissionSelection.push(node[i]);
        if (node[i].partialSelected == false)
          count++;
      }
    }
    return count;
  }

  private isPartialSelected(lengthParentNode: number, lengthChildSelected: number, isHasFullPS: boolean, name: string) {
    if (lengthChildSelected > 0 && lengthChildSelected < lengthParentNode)
      return true;

    if (lengthChildSelected == 0 && !isHasFullPS && this.grantedPermissionNames.includes(name))
      return true;

    return false;
  }

  isPermissionChecked(permissionName: string): boolean {
    return _includes(this.grantedPermissionNames, permissionName);
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

    const role = new RoleDto();
    role.init(this.role);
    role.grantedPermissions = this.getCheckedPermissions();
    this._roleService.update(role).subscribe(
      (rs) => {
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS);
        this.disabledSaveBtn = false;
      },
      (err) => {
        this.showToastMessage(ToastMessageType.ERROR, err);
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
        { label: "Edit Role " + this.role.name }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}

import { MESSAGE } from './../../../../../shared/AppConsts';
import { RoleService } from '@app/core/services/roles/role.service';
import { Component, OnInit, Injector, Input, ViewChild } from '@angular/core';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { AppComponentBase } from '@shared/app-component-base';
import { forkJoin } from 'rxjs';
import { RoleUserInfo } from '@app/core/models/role/role.model';
import { ToastMessageType } from '@shared/AppEnums';
import { PickList } from 'primeng/picklist';

@Component({
  selector: 'talent-tab-user-role',
  templateUrl: './tab-user-role.component.html',
  styleUrls: ['./tab-user-role.component.scss']
})
export class TabUserRoleComponent extends AppComponentBase implements OnInit {
  @ViewChild('pickList') pickList: PickList;
  readonly GET_TYPE = {
    IN_ROLE: 0,
    NOT_IN_ROLE: 1,
    BOTH: 2
  }

  roleId: number;
  roleMembers: RoleUserInfo[] = [];
  availableMembers: RoleUserInfo[] = [];
  showMemberNotInRole: boolean = false;

  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
    private _role: RoleService
  ) {
    super(injector)
    this.roleId = Number(this.route.snapshot.paramMap.get("id"));
    this.getListUserRole(this.GET_TYPE.IN_ROLE);
  }

  ngOnInit(): void { }

  deleteMemberRole(data: RoleUserInfo[], showConfirm = false) {
    if (!showConfirm) {
      this.handleDeleteRole(data);
      return;
    }

    this.isLoading = true;
    this.confirmationService.confirm({
      message: `Do you want to remove all member from this role?`,
      header: 'Confirmation',
      icon: 'pi pi-exclamation-circle',
      accept: () => {
        this.handleDeleteRole(data);
      },
      reject: () => {
        this.pickList.resetFilter();
        this.getListUserRole();
      }
    })
  }

  addMemberRole(data: RoleUserInfo[], showConfirm = false) {
    if (!showConfirm) {
      this.handleAddRole(data);
      return;
    }

    this.isLoading = true;
    this.confirmationService.confirm({
      message: `Do you want to add all member into this role?`,
      header: 'Confirmation',
      icon: 'pi pi-exclamation-circle',
      accept: () => {
        this.handleAddRole(data);
      },
      reject: () => {
        this.pickList.resetFilter();
        this.getListUserRole();
      }
    })
  }

  openAddMember() {
    this.showMemberNotInRole = true;
    this.getListUserRole(this.GET_TYPE.NOT_IN_ROLE);
  }

  closeAddMember() {
    this.showMemberNotInRole = false;
    this.availableMembers = [];
  }

  private handleAddRole(data: RoleUserInfo[]) {
    const payload = {
      roleId: this.roleId,
      userIds: data.map(item => item.userId)
    };

    this.subs.add(
      this._role.createUserRole(payload).subscribe(rs => {
        this.isLoading = rs.loading;
        if (!rs.loading && rs.success) {
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.ADD_SUCCESS, rs.result);
        }
      })
    )
  }

  private handleDeleteRole(data: RoleUserInfo[]) {
    const payload = {
      userRoleIds: data.map(item => item.userRoleId)
    };

    this.subs.add(
      this._role.deleteUserRole(payload).subscribe(rs => {
        this.isLoading = rs.loading;
        if (!rs.loading && rs.success) {
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.ADD_SUCCESS, rs.result);
        }
      })
    )
  }

  private getListUserRole(getType: number = this.GET_TYPE.BOTH) {
    const roleId = this.roleId;

    switch (getType) {
      case this.GET_TYPE.IN_ROLE:
        this._role.getUserInRole(roleId).subscribe((res) => {
          this.isLoading = res.loading;
          res.result && (this.roleMembers = res.result);
        })
        return;
      case this.GET_TYPE.NOT_IN_ROLE:
        this._role.getUserNotInRole(roleId).subscribe((res) => {
          this.isLoading = res.loading;
          res.result && (this.availableMembers = res.result);
        })
        return;
      default:
        forkJoin([
          this._role.getUserInRole(roleId),
          this._role.getUserNotInRole(roleId)
        ]).subscribe({
          next: ([has, not]) => {
            this.isLoading = (has.loading && not.loading);
            this.roleMembers = has.result;
            this.availableMembers = not.result;
          }
        })
        return;
    }
  }

}

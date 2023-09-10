import { RoleForEdit, RoleUserInfo, SystemPermission } from './../../models/role/role.model';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from '../apis/base-api.service';
import { CatalogModel } from '@app/core/models/common/common.dto';

@Injectable({
  providedIn: 'root'
})
export class RoleService extends BaseApiService {

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  changeUrl(): string {
    return 'Role';
  }

  getRoleForEdit(id: number): Observable<ApiResponse<RoleForEdit>> {
    return this.get("/GetRoleForEdit?Id=" + id);
  }
  
  getAllPermissions(): Observable<ApiResponse<SystemPermission[]>> {
    return this.get("/GetAllPermissions");
  }

  getUserInRole(roleId: number): Observable<ApiResponse<RoleUserInfo[]>> {
    return this.get("/GetUserInRole?id=" + roleId);
  }

  getUserNotInRole(roleId: number): Observable<ApiResponse<RoleUserInfo[]>> {
    return this.get("/GetUserNotInRole?id=" + roleId);
  }

  createUserRole(payload: { roleId: number, userIds: number[] }): Observable<ApiResponse<string>> {
    return this.create(payload, "/CreateUserRole");
  }

  deleteUserRole(payload: { userRoleIds: number[] }): Observable<ApiResponse<string>> {
    return this.create(payload, "/DeleteUserRole");
  }

}

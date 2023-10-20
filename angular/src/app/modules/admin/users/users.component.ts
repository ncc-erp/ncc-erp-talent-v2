import { allUsers } from './../../../store/selectors/user.selector';
import { Component, Injector } from '@angular/core';
import { Observable, of } from "rxjs";
import { catchError, map, startWith } from "rxjs/operators";
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  PagedListingComponentBase,
  PagedRequestDto
} from 'shared/paged-listing-component-base';
import {
  UserServiceProxy,
  UserDto,
  UserDtoPagedResultDto
} from '@shared/service-proxies/service-proxies';
import { CreateUserDialogComponent } from './create-user/create-user-dialog.component';
import { EditUserDialogComponent } from './edit-user/edit-user-dialog.component';
import { ResetPasswordDialogComponent } from './reset-password/reset-password.component';
import { select, Store } from '@ngrx/store';
import { UserModel } from '@app/store/models/user.model';
import { retrievedUserList } from '@app/store/actions/user.action';
import { LazyLoadEvent, MenuItem } from 'primeng/api';
import { HttpErrorResponse } from '@angular/common/http';
import { DefaultRoute } from '@shared/AppEnums';

class PagedUsersRequestDto extends PagedRequestDto {
  keyword: string;
  isActive: boolean | null;
}

@Component({
  templateUrl: './users.component.html',
  animations: [appModuleAnimation()]
})
export class UsersComponent extends PagedListingComponentBase<UserDto> {
  users: UserDto[] = [];
  keyword = '';
  isActive: boolean | null;
  advancedFiltersVisible = false;
  listUsers$ = this._store.pipe(select(allUsers));
  public loading: boolean;

  constructor(
    injector: Injector,
    private _userService: UserServiceProxy,
    private _modalService: BsModalService,
    private _store: Store<{ users: UserModel[] }>
  ) {
    super(injector);
    this.breadcrumbConfig = this.getBreadcrumbConfig();
  }

  ngOnInit(): void {
    this.subs.sink = this._userService.getAllUser().subscribe(rs => {
      this._store.dispatch(
        retrievedUserList({ allUser: rs.result as UserModel[] })
      );
    });
    this.loading = true;
  }

  public loadUsers(event: LazyLoadEvent) {
    this.setSortThead(event.sortField, event.sortOrder);
    this.loading = true;
    this.pageNumber = event.first / this.pageSize + 1;
    this.pageSize = event.rows;
    this.refresh();
  };

  createUser(): void {
    this.showCreateOrEditUserDialog();
  }

  editUser(user: UserDto): void {
    this.showCreateOrEditUserDialog(user.id);
  }

  public resetPassword(user: UserDto): void {
    this.showResetPasswordUserDialog(user.id);
  }

  clearFilters(): void {
    this.searchText = '';
    this.isActive = undefined;
    this.getDataPage(1);
  }

  protected list(
    request: PagedUsersRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.searchText = this.searchText;
    request.isActive = this.isActive;
    this.subs.add(
      this._userService
        .getAll(
          request
        )
        .pipe(
          map(data => ({ ...data, loading: false })),
          startWith({ loading: true, success: false }),
          catchError((err: HttpErrorResponse) => {
            return of({ loading: false, success: false, error: err.error.error });
          })
        )
        .subscribe((rs: any) => {
          this.loading = rs.loading;
          if (!rs.success) {
            this.users = rs.items;
            this.showPaging(rs, pageNumber);
          }
        })
    );
  }

  protected delete(user: UserDto): void {
    abp.message.confirm(
      this.l('UserDeleteWarningMessage', user.fullName),
      undefined,
      (result: boolean) => {
        if (result) {
          this._userService.delete(user.id).subscribe(() => {
            abp.notify.success(this.l('SuccessfullyDeleted'));
            this.refresh();
          });
        }
      }
    );
  }

  private showResetPasswordUserDialog(id?: number): void {
    this._modalService.show(ResetPasswordDialogComponent, {
      class: 'modal-lg',
      initialState: {
        id: id,
      },
    });
  }

  private showCreateOrEditUserDialog(id?: number): void {
    let createOrEditUserDialog: BsModalRef;
    if (!id) {
      createOrEditUserDialog = this._modalService.show(
        CreateUserDialogComponent,
        {
          class: 'modal-lg',
        }
      );
    } else {
      createOrEditUserDialog = this._modalService.show(
        EditUserDialogComponent,
        {
          class: 'modal-lg',
          initialState: {
            id: id,
          },
        }
      );
    }

    createOrEditUserDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }

  public getListItem(user: UserDto): MenuItem[] {
    return [{
      label: 'Action',
      items: [{
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => {
          this.editUser(user);
        },
        visible: this.permission.isGranted(this.PS.Pages_Users_Edit)
      },{
        label: 'Delete',
        icon: 'pi pi-trash',
        command: () => {
          this.delete(user);
        },
        visible: this.permission.isGranted(this.PS.Pages_Users_Delete)
      }, {
        label: 'ResetPassword',
        icon: 'fas fa-lock',
        command: () => {
          this.resetPassword(user);
        },
        visible:this.permission.isGranted(this.PS.Pages_Users_ResetPassword)
      },]
    }]
  }


  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Admin", routerLink: DefaultRoute.Admin, styleClass: 'menu-item-click' }, { label: "Users" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}

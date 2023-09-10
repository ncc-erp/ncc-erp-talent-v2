import { TabUserRoleComponent } from './roles/tab-user-role/tab-user-role.component';
import { UsersComponent } from './users/users.component';
import { NgxPaginationModule } from 'ngx-pagination';
import { CreateTenantDialogComponent } from './tenants/create-tenant/create-tenant-dialog.component';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { SharedModule } from './../../../shared/shared.module';
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

// Roles
import { AdminRoutingModule } from "./admin-routing.module";
import { RolesComponent } from "./roles/roles.component";
import { CreateRoleDialogComponent } from "./roles/create-role/create-role-dialog.component";

// Tenants
import { TenantsComponent } from './tenants/tenants.component';
import { EditRoleDialogComponent } from "./roles/edit-role/edit-role-dialog.component";
import { EditTenantDialogComponent } from './tenants/edit-tenant/edit-tenant-dialog.component';
import { CreateUserDialogComponent } from './users/create-user/create-user-dialog.component';
import { EditUserDialogComponent } from './users/edit-user/edit-user-dialog.component';
import { ResetPasswordDialogComponent } from './users/reset-password/reset-password.component';
import { ConfigurationsComponent } from './configurations/configurations.component';
import { MailComponent } from './mail/mail.component';
import { MailDialogComponent } from './mail/mail-dialog/mail-dialog.component';
import { DialogModule } from 'primeng/dialog';
import { EditMailDialogComponent } from './mail/edit-mail-dialog/edit-mail-dialog.component';

@NgModule({
  declarations: [
    RolesComponent,
    CreateRoleDialogComponent,
    EditRoleDialogComponent,
    CreateTenantDialogComponent,
    TenantsComponent,
    CreateTenantDialogComponent,
    EditTenantDialogComponent,
    UsersComponent,
    CreateUserDialogComponent,
    EditUserDialogComponent,
    ResetPasswordDialogComponent,
    ConfigurationsComponent,
    MailComponent,
    MailDialogComponent,
    EditMailDialogComponent,
    TabUserRoleComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    AdminRoutingModule,
    TabsModule,
    NgxPaginationModule,
    DialogModule
  ],
  entryComponents: [
    CreateTenantDialogComponent,
    EditTenantDialogComponent,
    CreateUserDialogComponent,
    EditUserDialogComponent,
    ResetPasswordDialogComponent,
  ]
})
export class AdminModule { }

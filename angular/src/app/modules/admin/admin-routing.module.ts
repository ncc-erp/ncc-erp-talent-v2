import { MailComponent } from './mail/mail.component';
import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { RolesComponent } from "./roles/roles.component";
import { TenantsComponent } from "./tenants/tenants.component";
import { UsersComponent } from "./users/users.component";
import { ConfigurationsComponent } from './configurations/configurations.component';
import { EditRoleDialogComponent } from './roles/edit-role/edit-role-dialog.component';
import { CreateRoleDialogComponent } from './roles/create-role/create-role-dialog.component';
import { PERMISSIONS_CONSTANT } from '@shared/permission/permissions';
import { MezonWebhooksComponent } from './mezon-webhooks/mezon-webhooks.component';

const routes: Routes = [
  {
    path: "users",
    component: UsersComponent,
    data: { permission: PERMISSIONS_CONSTANT.Pages_Users },
  },
  {
    path: "roles",
    component: RolesComponent,
    data: { permission: PERMISSIONS_CONSTANT.Pages_Roles },
  },
  {
    path: "roles/create",
    component: CreateRoleDialogComponent,
    data: { permission: PERMISSIONS_CONSTANT.Pages_Roles_Create },
  },
  {
    path: "roles/edit/:id",
    component: EditRoleDialogComponent,
    data: { permission: PERMISSIONS_CONSTANT.Pages_Roles_Edit },
  },
  {
    path: "tenants",
    component: TenantsComponent,
    data: { permission: PERMISSIONS_CONSTANT.Pages_Tenants },
  },
  {
    path: "configurations",
    component: ConfigurationsComponent,
    data: { permission: PERMISSIONS_CONSTANT.Pages_Configurations },
  },
  {
    path: "mail",
    component: MailComponent,
    data: { permission: PERMISSIONS_CONSTANT.Pages_Mails },
  }, 
  {
    path: "mezon-webhooks",
    component: MezonWebhooksComponent,
    data: { permission: PERMISSIONS_CONSTANT.Pages_MezonWebhooks },
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule { }

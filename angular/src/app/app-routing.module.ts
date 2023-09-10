import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { PERMISSIONS_CONSTANT } from '@shared/permission/permissions';
import { ChangePasswordComponent } from './../account/change-password/change-password.component';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';

const routes: Routes = [
    {
        path: '',
        component: AppComponent,
        canActivateChild: [AppRouteGuard],
        children: [
            { path: 'home', component: HomeComponent },
            { path: 'update-password', component: ChangePasswordComponent },
            {
                path: 'admin',
                loadChildren: () => import('app/modules/admin/admin.module').then(m => m.AdminModule),
                data: { permission: PERMISSIONS_CONSTANT.TabAdmin },
            },
            {
                path: 'categories',
                loadChildren: () => import('app/modules/category/category.module').then(m => m.CategoryModule),
                data: { permission: PERMISSIONS_CONSTANT.TabCategory },
            },
            {
                path: 'candidate',
                loadChildren: () => import('app/modules/candidate/candidate.module').then(m => m.CandidateModule),
            },
            {
                path: 'requisition',
                loadChildren: () => import('app/modules/requisitiion/requisition.module').then(m => m.RequisitionModule),
            },
            {
                path: 'report',
                loadChildren: () => import('app/modules/report/report.module').then(m => m.ReportModule),
            },
            {
                path: 'ncc-cv',
                loadChildren: () => import('app/modules/ncc-cv/ncc-cv.module').then(m => m.NccCvModule),
            },
        ]
    }
]

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }

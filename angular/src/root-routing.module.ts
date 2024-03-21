import { ApplyCvComponent } from "./apply-cv/apply-cv.component";
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PageErrorComponent } from '@shared/pages/page-error/page-error.component';

const routes: Routes = [
    { path: '', redirectTo: '/applycv', pathMatch: 'full' },
    {
        path: 'applycv',
        component: ApplyCvComponent
    },
    { path: '**', pathMatch: 'full', component: PageErrorComponent },
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
    exports: [RouterModule],
    providers: []
})
export class RootRoutingModule { }

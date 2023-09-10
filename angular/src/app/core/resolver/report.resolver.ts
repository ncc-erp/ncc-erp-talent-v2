import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { UtilitiesService } from './../services/utilities.service';

@Injectable({
  providedIn: 'root'
})
export class ReportResolver implements Resolve<void> {
  constructor(
    public _utilities: UtilitiesService,

  ) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<void> {
    return this._utilities.loadLabelForReportRecruitmentOverview();
  }
}

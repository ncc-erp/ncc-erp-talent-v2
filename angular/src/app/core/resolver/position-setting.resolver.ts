import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot, Resolve,
  RouterStateSnapshot
} from '@angular/router';
import { UtilitiesService } from '../services/utilities.service';

@Injectable({
  providedIn: 'root'
})
export class PositionSettingResolver implements Resolve<void> {

  constructor(
    public _utilities: UtilitiesService, 
   
  ) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<void> {
    return this._utilities.loadCatalogForPositionSetting();
  }

}

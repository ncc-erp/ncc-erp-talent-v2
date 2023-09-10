import { UtilitiesService } from './../services/utilities.service';
import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CategoriesResolver implements Resolve<void> {

  constructor(
    public _utilities: UtilitiesService, 
   
  ) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): void {
    return this._utilities.loadCatalogForCategories();
  }

}

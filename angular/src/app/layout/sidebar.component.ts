import {
  Component,
  Renderer2,
  OnInit
} from '@angular/core';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { LayoutStoreService } from '@shared/layout/layout-store.service';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['sidebar.component.scss']
})
export class SidebarComponent implements OnInit {
  sidebarExpanded: boolean;

  constructor(
    private renderer: Renderer2,
    private _layoutStore: LayoutStoreService,
    private _authService: AppAuthService
  ) {}

  ngOnInit(): void {
    this._layoutStore.sidebarExpanded.subscribe((value) => {
      this.sidebarExpanded = value;
      this.toggleSidebar();
    });
  }

  toggleSidebar(): void {
    if (this.sidebarExpanded) {
      this.hideSidebar();
    } else {
      this.showSidebar();
    }
  }

  showSidebar(): void {
    this.renderer.removeClass(document.body, 'sidebar-collapse');
    this.renderer.addClass(document.body, 'sidebar-open');
  }

  hideSidebar(): void {
    this.renderer.removeClass(document.body, 'sidebar-open');
    this.renderer.addClass(document.body, 'sidebar-collapse');
  }

  logout(): void {
    this._authService.logout();
  }
}

import {
  Component,
  Renderer2,
  OnInit,
  OnDestroy
} from '@angular/core';
import { LayoutStoreService } from '@shared/layout/layout-store.service';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['sidebar.component.scss']
})
export class SidebarComponent implements OnInit, OnDestroy {
  sidebarExpanded: boolean;
  hideSideBarTimeout: NodeJS.Timeout;

  constructor(
    private renderer: Renderer2,
    private _layoutStore: LayoutStoreService
  ) {}

  ngOnInit(): void {
    this._layoutStore.sidebarExpanded.subscribe((value) => {
      this.sidebarExpanded = value;
      this.toggleSidebar();
    });
    this._layoutStore.checkToHideSidebar();
  }

  ngOnDestroy(): void {
    clearTimeout(this.hideSideBarTimeout);
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
    const sidebar = document.querySelector('.main-sidebar');
    if (sidebar) {
      this.renderer.setStyle(sidebar, 'pointer-events', 'none');
      this.hideSideBarTimeout = setTimeout(() => {
        this.renderer.setStyle(sidebar, 'pointer-events', 'unset');
      }, 500);
    }
  }
}

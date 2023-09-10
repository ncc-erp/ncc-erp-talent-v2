import { Component, OnInit } from '@angular/core';
import { LayoutStoreService } from '@shared/layout/layout-store.service';

@Component({
  selector: 'header-left-navbar',
  templateUrl: './header-left-navbar.component.html',
  styleUrls: ['./header-left-navbar.component.scss'],
})
export class HeaderLeftNavbarComponent implements OnInit {
  sidebarExpanded: boolean;
  isSidebarCollapse: boolean = false;

  constructor(private _layoutStore: LayoutStoreService) { }

  ngOnInit(): void {
    this._layoutStore.sidebarExpanded.subscribe((value) => {
      this.sidebarExpanded = value;
      this.isSidebarCollapse = this.checkSidebarOpen();
    });
  }

  toggleSidebar(): void {
    this._layoutStore.setSidebarExpanded(!this.sidebarExpanded);
  }

  private checkSidebarOpen() {
    return document.body.classList.contains('sidebar-collapse');
  }
}

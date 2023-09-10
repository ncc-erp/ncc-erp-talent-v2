import { ChangeDetectorRef, Component, Injector, OnInit, Renderer2 } from '@angular/core';
import { NavigationCancel, NavigationEnd, NavigationError, NavigationStart } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { SignalRAspNetCoreHelper } from '@shared/helpers/SignalRAspNetCoreHelper';
import { LayoutStoreService } from '@shared/layout/layout-store.service';
import { BehaviorSubject } from 'rxjs';

@Component({
  templateUrl: './app.component.html'
})
export class AppComponent extends AppComponentBase implements OnInit {
  sidebarExpanded: boolean;
  isLoading$  = new BehaviorSubject<boolean>(false);

  constructor(
    injector: Injector,
    private renderer: Renderer2,
    private _layoutStore: LayoutStoreService,
    private cdr: ChangeDetectorRef,
  ) {
    super(injector);
  
    this.router.events.subscribe(event => {
      this.handleLoadingBar(event)
    });
  }

  ngOnInit(): void {
    this.renderer.addClass(document.body, 'sidebar-mini');

    SignalRAspNetCoreHelper.initSignalR();

    abp.event.on('abp.notifications.received', (userNotification) => {
      abp.notifications.showUiNotifyForUserNotification(userNotification);

      // Desktop notification
      Push.create('AbpZeroTemplate', {
        body: userNotification.notification.data.message,
        icon: abp.appPath + 'assets/app-logo-small.png',
        timeout: 6000,
        onClick: function () {
          window.focus();
          this.close();
        }
      });
    });

    this._layoutStore.sidebarExpanded.subscribe((value) => {
      this.sidebarExpanded = value;
    });
  }

  ngAfterViewChecked(){
    this.cdr.detectChanges();
  }

  toggleSidebar(): void {
    this._layoutStore.setSidebarExpanded(!this.sidebarExpanded);
  }

  private handleLoadingBar(event) {
    if (event instanceof NavigationStart) {  
      this.isLoading$.next(true);
    }
    else if (event instanceof NavigationEnd) {
      this.isLoading$.next(false);
    }
    else if (event instanceof NavigationCancel) {
      this.isLoading$.next(false);
    }
    else if (event instanceof NavigationError) {
      this.isLoading$.next(false);
    }
  }
}

import { Component } from '@angular/core';
import { AppAuthService } from '@shared/auth/app-auth.service';

@Component({
  selector: 'talent-sidebar-footer',
  templateUrl: './sidebar-footer.component.html',
  styleUrls: ['./sidebar-footer.component.scss']
})
export class SidebarFooterComponent {
  constructor(private _authService: AppAuthService) { }

  logout(): void {
    this._authService.logout();
  }
}

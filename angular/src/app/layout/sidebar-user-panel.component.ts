import {
  Component,
  Injector,
  OnInit
} from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { ChangeAvatarComponent } from 'account/change-avatar/change-avatar.component';
import { DialogService } from 'primeng/dynamicdialog';

@Component({
  selector: 'sidebar-user-panel',
  templateUrl: './sidebar-user-panel.component.html',
})
export class SidebarUserPanelComponent extends AppComponentBase
  implements OnInit {
  shownLoginName: string;
  isDialogShown: boolean = false;

  constructor(injector: Injector,
    private diaLog: DialogService) {
    super(injector);
  }
  ngOnInit() {
    this.shownLoginName = this.appSession.getShownLoginName();
  }

  updateAvatar(): void {
    if (!this.isDialogShown) {
      let diaLogref = this.diaLog.open(ChangeAvatarComponent, {
        width: "600px",
        contentStyle: { "max-height": "100%", overflow: "visible" },
        showHeader: false,
        baseZIndex: 1000
      });
      this.isDialogShown = true;
      diaLogref.onClose.subscribe(() =>{
        this.isDialogShown = false;
      });
    }
  }
}

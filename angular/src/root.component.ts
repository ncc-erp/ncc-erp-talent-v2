import { Component, OnInit } from '@angular/core';
import { MezonLoginService } from '@app/core/services/apis/mezon-api.service';
import { isFromMezonApp } from '@app/core/helpers/utils.helper';
import { LoginService } from 'account/login/login.service';

@Component({
    selector: 'app-root',
    template: `<router-outlet></router-outlet>`
})
export class RootComponent implements OnInit {

    constructor(
        private mezonService: MezonLoginService,
    ) {
        this.initializeMezonIntegration();
    }

    ngOnInit(): void {
    }

    private async initializeMezonIntegration() {
      if (!isFromMezonApp()) return;
      const urlParams = new URLSearchParams(window.location.search);
      const hashData = urlParams.get("data");
      this.mezonService.setMezonHashData(hashData);
    }
}


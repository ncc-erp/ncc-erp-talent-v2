import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MezonLoginService } from '@app/core/services/apis/mezon-api.service';
import { isFromMezon } from '@app/core/helpers/utils.helper';

@Component({
    selector: 'app-root',
    template: `<router-outlet></router-outlet>`
})
export class RootComponent implements OnInit {

    constructor(
        private router: Router,
        private mezonService: MezonLoginService
    ) {
        this.initializeMezonIntegration();
    }

    ngOnInit(): void {
    }

    private async initializeMezonIntegration() {
        const urlParams = new URLSearchParams(window.location.search);
        const hashData = urlParams.get('data');
        
        const isMezon = isFromMezon();
        
        if (isMezon && hashData) this.mezonService.setMezonHashData(hashData);
    }
}

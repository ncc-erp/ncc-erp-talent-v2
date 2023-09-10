import { Injector, Component, OnInit } from '@angular/core';
import { AppComponentBase } from './app-component-base';
import { BreadCrumbConfig } from '../app/core/models/common/common.dto';

@Component({
    template: ''
})
export abstract class NccAppComponentBase extends AppComponentBase implements OnInit {
    constructor(injector: Injector) {
        super(injector);
        this.breadcrumbConfig = this.getBreadCrumbConfig();
    }

    ngOnInit(): void {
    }
    protected abstract getBreadCrumbConfig(): BreadCrumbConfig;
}

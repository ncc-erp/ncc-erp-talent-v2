import { AppComponentBase } from '@shared/app-component-base';
import { Component, OnInit, Injector } from '@angular/core';
import { BreadCrumbConfig } from '@app/core/models/common/common.dto';
import { DefaultRoute } from '@shared/AppEnums';

@Component({
  selector: 'talent-detail-project',
  templateUrl: './detail-project.component.html',
  styleUrls: ['./detail-project.component.scss']
})
export class DetailProjectComponent extends AppComponentBase implements OnInit {
  projectId: number;
  projectName: string;
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
    this.getParamsURL();
    this.breadcrumbConfig = this.getBreadCrumbConfig();
  }

  getParamsURL() {
    this.projectId = +this.route.snapshot.queryParamMap.get('id');
    this.projectName = this.route.snapshot.queryParamMap.get('projectName');
  }

  private getBreadCrumbConfig(): BreadCrumbConfig {
    return {
      homeItem: this.homeMenuItem,
      menuItem: [
        { label: "Employee Profile", routerLink: DefaultRoute.ProjectManagement, styleClass: 'menu-item-click' },
        { label: "Project Management", routerLink: '/app/ncc-cv/project' },
        { label: this.projectName }
      ]
    }
  }
}

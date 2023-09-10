import { Component, Injector, OnInit } from '@angular/core';
import { BreadCrumbConfig } from '@app/core/models/common/common.dto';
import { ProjectService } from '@app/core/services/project/project.service';
import { MESSAGE } from '@shared/AppConsts';
import { ActionEnum, DefaultRoute, ToastMessageType } from '@shared/AppEnums';
import { ApiResponse, PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { DialogService } from 'primeng/dynamicdialog';
import { DialogProjectComponent, ProjectDialogConfig } from '../dialog-project/dialog-project.component';

@Component({
  selector: 'talent-list-project',
  templateUrl: './list-project.component.html',
  styleUrls: ['./list-project.component.scss']
})
export class ListProjectComponent extends PagedListingComponentBase<Project> implements OnInit {

  projectTypes: ProjectType[] = listTypeProjects;
  optionProjectTypes: ProjectType[] = [
    { code: undefined, name: 'All' },
    ...this.projectTypes
  ];

  slProjectType: ProjectType;
  projects: Project[] = [];

  constructor(
    injector: Injector,
    private _project: ProjectService,
    public dialogService: DialogService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadCrumbConfig();
  }
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    const payload = {
      name: this.searchText?.trim().replace(/ +/g, ' '),
      type: this.slProjectType,
      maxResultCount: request.maxResultCount,
      skipCount: request.skipCount,
    };
    this.subs.add(
      this._project.getAllPagingProject(payload).subscribe((rs) => {
        this.projects = [];
        if (rs.success) {
          this.projects = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  openDialog(obj: Project, dialogAction: ActionEnum) {
    const dialogConfig = {
      project: obj,
      action: dialogAction
    } as ProjectDialogConfig;
    const dialogRef = this.dialogService.open(DialogProjectComponent, {
      header: `${dialogConfig.action} Project`,
      width: '50%',
      contentStyle: { "max-height": "600px", overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig
    });

    dialogRef.onClose.subscribe((rs: ApiResponse<Project>) => {
      if (rs) {
        if (dialogAction == ActionEnum.UPDATE) {
          const index = this.projects.findIndex((x) => x.id == rs.result.id);
          this.projects[index] = rs.result;
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, rs.result.name);
          return;
        }

        this.projects.unshift(rs.result);
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CREATE_SUCCESS, rs.result.name);
      }
    })
  }

  usedList(id: number, name: string) {
    this.router.navigate([`app/ncc-cv/project/detail-project`], { queryParams: { id: id, projectName: name } });
  }

  protected delete(entity: Project): void {
    throw new Error('Method not implemented.');
  }

  private getBreadCrumbConfig(): BreadCrumbConfig {
    return {
      homeItem: this.homeMenuItem,
      menuItem: [
        { label: "Employee Profile", routerLink: DefaultRoute.ProjectManagement, styleClass: 'menu-item-click' },
        { label: "Project Management" }
      ]
    }
  }

}

export class Project {
  name: string;
  type: number;
  technology: string;
  description: string;
  id: number;
}
export interface ProjectType {
  code: number;
  name: string;
}
export interface PayloadSearchProject {
  name: string;
  type?: number;
  maxResultCount: number;
  skipCount: number;
}
const listTypeProjects: ProjectType[] = [
  { code: 0, name: 'Created by PM' },
  { code: 1, name: 'Created by User' },
];

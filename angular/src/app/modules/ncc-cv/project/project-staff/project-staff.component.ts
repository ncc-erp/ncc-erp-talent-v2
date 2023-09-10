import { ProjectService } from '@app/core/services/project/project.service';
import { Component, Injector, Input, OnInit } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';

@Component({
  selector: 'talent-project-staff',
  templateUrl: './project-staff.component.html',
  styleUrls: ['./project-staff.component.scss']
})
export class ProjectStaffComponent extends PagedListingComponentBase<UserWorkingProject> implements OnInit {
  @Input() projectId: number;

  users: UserWorkingProject[] = [];

  constructor(injector: Injector, private _project: ProjectService) {
    super(injector);
  }

  ngOnInit(): void {
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subs.add(
      this._project.getUserWorkingInProject(
        this.projectId,
        request.skipCount,
        request.maxResultCount
      ).subscribe(res => {
        this.users = [];
        if (res.success) {
          this.users = res.result.items;
          this.showPaging(res.result, pageNumber);
        }
        this.isLoading = res.loading;
      })
    );
  }
  protected delete(entity: UserWorkingProject): void {
    throw new Error('Method not implemented.');
  }

}

export class UserWorkingProject {
  userId: number;
  name: string;
  position: string;
  startDate: Date;
  endDate: Date;
  workingExpId;
}

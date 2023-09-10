import { Component, Injector, Input, OnInit } from '@angular/core';
import { ProjectService } from '@app/core/services/project/project.service';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';

@Component({
  selector: 'talent-project-staff-fake',
  templateUrl: './project-staff-fake.component.html',
  styleUrls: ['./project-staff-fake.component.scss']
})
export class ProjectStaffFakeComponent extends PagedListingComponentBase<UserUsedProject> implements OnInit {
  @Input() projectId: number;

  users: UserUsedProject[] = [];

  constructor(injector: Injector, private _project: ProjectService) {
    super(injector);
  }

  ngOnInit(): void {
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subs.add(
      this._project.getUserUsedInProject(
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
  protected delete(entity: UserUsedProject): void {
    throw new Error('Method not implemented.');
  }
}

export class UserUsedProject {
  userId: number;
  name: string;
  position: string;
  startDate: Date;
  endDate: Date;
  workingExpId;
}
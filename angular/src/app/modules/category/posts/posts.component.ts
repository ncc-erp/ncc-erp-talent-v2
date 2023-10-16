import { Component, Injector, OnInit, OnDestroy} from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FILTER_TIME,MESSAGE } from '@shared/AppConsts';
import {
  PagedListingComponentBase,
  PagedRequestDto,
  ApiResponse,
} from '@shared/paged-listing-component-base';
import {
  ActionEnum,
  API_RESPONSE_STATUS,
  ToastMessageType,
} from "@shared/AppEnums";
import { CreationTimeEnum, DefaultRoute } from '@shared/AppEnums';
import { PostDialogComponent } from './post-dialog/post-dialog.component';
import { DialogService, DynamicDialogRef } from "primeng/dynamicdialog";
import { PostService } from '@app/core/services/categories/post.service';
import { Post, PostPayloadListData } from '@app/core/models/categories/post.model';
import { TalentDateTime } from '@shared/components/date-selector/date-selector.component';
import { DateFormat} from '@shared/AppConsts';
import { PostDetailComponent } from './post-detail/post-detail.component';
import {MenuItem} from 'primeng/api';

class PagedRolesRequestDto extends PagedRequestDto {
  keyword: string;
}

@Component({
  templateUrl: './posts.component.html',
  animations: [appModuleAnimation()],
  styleUrls: ['./posts.component.scss']
})
export class PostsComponent extends PagedListingComponentBase<Post> implements OnInit, OnDestroy{
  public readonly DATE_FORMAT = DateFormat;
  public readonly FILTER_TIME = FILTER_TIME;
  defaultOptionTime: string = CreationTimeEnum.ALL;
  searchWithCreationTime: TalentDateTime;
  public posts: Post[] = [];
    private dialogRef: DynamicDialogRef;
  keyword = '';

  constructor(
    injector: Injector,
    public dialogService: DialogService,
    private _post: PostService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
  }

  ngOnDestroy() {
    super.ngOnDestroy();
    if (this.dialogRef) this.dialogRef.close()
  }

  onTalentDateChange(talentDateTime: TalentDateTime) {
    this.searchWithCreationTime = talentDateTime;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  openMetadata(obj: Post) {
    this.dialogService.open(PostDetailComponent, {
      header: `${obj.postName} - Metadata`,
      contentStyle: { "max-height": "500px", overflow: "visible" },
      baseZIndex: 100,
      data: obj
    });
  }

  openDialog(obj: Post, dialogAction: ActionEnum) {
      const dialogConfig  = { post: obj, action: dialogAction };    
    const dialogRef = this.dialogService.open(PostDialogComponent, {
      header: `${dialogConfig.action} Post`,
      width: "40%",
      contentStyle: { "max-height": "500px", overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<Post>) => {
      if (dialogConfig.action === ActionEnum.UPDATE) {
        if (res && res.success) {
          const index = this.posts.findIndex((x) => x.id == res?.result?.id);
          this.posts[index] = res.result;
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res?.result?.postName);
          return;
        }
      }
      this.refresh();
    });
  }

  
  protected list(
    request: PagedRolesRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    const payload = this.getPayLoad(request);
    this.subs.add(
      this._post.getAllPagging(payload).subscribe((rs) => {
        this.posts = [];
        if (rs.success) {
          this.posts = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }
  private getPayLoad(request: PagedRequestDto): PostPayloadListData {
    const payload: any = { ...request }
    payload.searchText = this.keyword;
    if (this.searchWithCreationTime && this.searchWithCreationTime.dateType !== CreationTimeEnum.ALL) {
      payload.fromDate = this.searchWithCreationTime?.fromDate?.format(DateFormat.YYYY_MM_DD);
      payload.toDate = this.searchWithCreationTime?.toDate?.format(DateFormat.YYYY_MM_DD);
    }
    return payload;
  }

  protected delete(post: Post): void {
    const deleteRequest = this._post.delete(post.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, post.postName).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
        })
    );
  }

  public getListItem( obj: Post): MenuItem[] {
    return [{
      label: 'Action',
      items: [{
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => {
          this.openDialog(obj, this.DIALOG_ACTION.UPDATE);
        },
        visible: this.permission.isGranted(this.PS.Pages_Posts_Edit)
      },{
        label: 'Delete',
        icon: 'pi pi-trash',
        command: () => {
          this.delete(obj);
        },
        visible: this.permission.isGranted(this.PS.Pages_Posts_Delete)
      }, {
        label: 'Metadata',
        icon: 'pi pi-eye',
        command: () => {
          this.openMetadata(obj);
        },
        visible:this.permission.isGranted(this.PS.Pages_Users_ResetPassword)
      },]
    }]
  }
  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: this.l("Categories"), routerLink: DefaultRoute.Category, styleClass: 'menu-item-click' }, { label: this.l("Posts") }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
 
}

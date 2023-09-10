import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MESSAGE } from '@shared/AppConsts';
import { ToastMessageType } from '@shared/AppEnums';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AppComponentBase } from '../../../../../shared/app-component-base';
import { ActionEnum } from '../../../../../shared/AppEnums';
import { Post,PostConfigDiaLog} from '@app/core/models/categories/post.model'
import { PostService } from '@app/core/services/categories/post.service';
import * as moment from 'moment';
import { DateFormat } from '@shared/AppConsts';
import { getFormControlValue } from '@app/core/helpers/utils.helper';
import { UserCatalog } from '@app/core/models/common/common.dto';
import { RoleService } from '@app/core/services/roles/role.service';
@Component({
  selector: 'talent-post-dialog',
  templateUrl: './post-dialog.component.html',
  styleUrls: ['./post-dialog.component.scss']
})
export class PostDialogComponent extends AppComponentBase implements OnInit {

  submitted: boolean = false;
  form: FormGroup;
  postConfigDialog: PostConfigDiaLog;
  action: ActionEnum;
  catAllUser: UserCatalog[];
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private injector: Injector,
    private _fb: FormBuilder,
    private _post: PostService
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.postConfigDialog = this.config.data;  
    this.action = this.postConfigDialog.action;
    this.initForm();
    this._post.getAllRecruitmentUser().subscribe((allUser) => {
      this.catAllUser = allUser.result;
    })
  }

  onSave(isClose: boolean) {
    this.submitted = true;
    if (this.form.invalid) return;
    const action = this.postConfigDialog.action;
    const payload: Post = {
      id: action === ActionEnum.UPDATE ? this.postConfigDialog.post.id : 0,
      postName: getFormControlValue(this.form, 'postName'),
      url: getFormControlValue(this.form, 'url'),
      type: getFormControlValue(this.form, 'type'),
      postCreationTime: moment(this.formControls['postCreationTime'].value).format(DateFormat.YYYY_MM_DD),
      createdByUser: getFormControlValue(this.form, 'createdByUser'),
      content : '',
    };
    this.handleSave(payload, action, isClose);
  }

  checkAndClosePopup(res: ApiResponse<Post>) {
    if (!res.loading) this.ref.close(res);
  }

  private initForm() {
    const isUpdate = this.postConfigDialog.action == ActionEnum.UPDATE;
    const reqPost = this.postConfigDialog.post;

    const defaultDate = new Date();
    defaultDate.setDate(defaultDate.getDate());
    const postCreationTime: Date = isUpdate ? new Date(reqPost.postCreationTime) : defaultDate;

    this.form = this._fb.group({
      postName: [
        isUpdate ? this.postConfigDialog.post.postName : '',
        [Validators.required]
      ],
      url: [isUpdate ? this.postConfigDialog.post.url : '', 
        [Validators.required]],
      type: isUpdate ? this.postConfigDialog.post.type : '', 
      postCreationTime: [postCreationTime, 
        [Validators.required]],
      createdByUser: [isUpdate ? this.postConfigDialog.post.createdByUser : '', 
        [Validators.required]],
    });
  }

  private resetForm() {
    this.initForm();
    this.submitted = false;
  }

  private handleSave(payload: Post, action: ActionEnum, isClose: boolean) {
    if (action === ActionEnum.CREATE) {
      this.subs.add(
        this._post.create(payload).subscribe(res => {
          
          this.isLoading = res.loading;
          if (res.loading) return;
          this.doSave(res, isClose);
        })
      );
      return;
    }

    this.subs.add(
      this._post.update(payload).subscribe(res => {
        this.isLoading = res.loading;
        this.checkAndClosePopup(res)
      })
    );
  }

  private doSave(res,isClose:boolean) {
    if (res && res.success) {
      this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CREATE_SUCCESS, res?.result?.postName);
      if( isClose){
        this.ref.close();
        return;
      }
      this.resetForm();
    }
  }
}

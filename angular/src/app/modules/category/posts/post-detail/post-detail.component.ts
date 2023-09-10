import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '../../../../../shared/app-component-base';
import { Post } from '@app/core/models/categories/post.model';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'talent-post-detail',
  templateUrl: './post-detail.component.html',
  styleUrls: ['./post-detail.component.scss']
})
export class PostDetailComponent extends AppComponentBase implements OnInit {

  id: number;
  post: Post = new Post();
  constructor(
    injector: Injector,
    public ref: DynamicDialogRef,
    public dialogService: DialogService,
    public config: DynamicDialogConfig
  ) {
    super( injector)
  }

  ngOnInit() : void {
    this.post = this.config.data;
  }

  formatMetadata() {
    const meta = this.post.metadata;
    const jsonData = JSON.stringify(JSON.parse(meta), null, 2);
    $("#postMetadata").text(jsonData);
  }
}

import { Component, Injector, OnDestroy, OnInit, ViewChild } from '@angular/core';
import {ToastMessageType} from '@shared/AppEnums';
import {AppComponentBase} from '@shared/app-component-base';
import { NgxDocViewerComponent } from 'ngx-doc-viewer';

@Component({
  selector: 'talent-view-flles',
  templateUrl: './view-flles.component.html',
  styleUrls: ['./view-flles.component.scss']
})
export class ViewFllesComponent extends AppComponentBase  implements OnInit, OnDestroy {
  documentUrl: string = '';
  isLoading: boolean ;
  @ViewChild('docViewer') docViewer: NgxDocViewerComponent;
  constructor(injector: Injector,) {
    super(injector);
  }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.documentUrl = params['documentUrl'];
      if(this.documentUrl){
       this.isLoading = true;
      }
     if(!this.documentUrl){
       this.showToastMessage(ToastMessageType.ERROR, 'File does not exist!');
      }
   });
  }
  ngOnDestroy(): void {
    localStorage.removeItem('documentUrl');
  }
  onDocumentLoad(): void {
    this.isLoading = false;
  }

}

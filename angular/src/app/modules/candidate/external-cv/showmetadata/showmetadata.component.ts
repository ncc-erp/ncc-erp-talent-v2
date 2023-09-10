import { Component, Injector, OnInit } from '@angular/core';
import { ExternalCv } from '@app/core/models/categories/external-cv.model';
import { ExternalCvService } from '@app/core/services/categories/external-cv.service';
import { AppComponentBase } from '@shared/app-component-base';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';

@Component({
  selector: 'talent-showmetadata',
  templateUrl: './showmetadata.component.html',
  styleUrls: ['./showmetadata.component.scss']
})
export class ShowMetadataComponent  extends AppComponentBase implements OnInit {

  id: number;
  externalCv: ExternalCv = new ExternalCv ;
  metadata: string = '';
  constructor( 
    injector: Injector,
    private _externalCv: ExternalCvService,
    public cofig: DynamicDialogConfig
  ){
    super(injector);
  }

  ngOnInit():void {
    this.id = this.cofig.data?.id;
    this.getExternalVyById();
  }

  getExternalVyById() {
    this._externalCv
    .getExternalCvById(this.id)
    .subscribe((rs:ApiResponse<ExternalCv>) => {
      if (rs.success) {
        this.externalCv = rs.result;
      }
    });
  }

  formatMetadata() {
    if (this.externalCv && this.externalCv.metadata) {
      this.metadata = JSON.stringify(JSON.parse(this.externalCv.metadata), null, 2);
      $("#externalCv").text(this.metadata);
    }
  }
}
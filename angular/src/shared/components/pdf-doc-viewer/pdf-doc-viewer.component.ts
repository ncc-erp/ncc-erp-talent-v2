import { Component, OnInit, Optional  } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'talent-pdf-doc-viewer',
  templateUrl: './pdf-doc-viewer.component.html',
  styleUrls: ['./pdf-doc-viewer.component.scss']
})
export class PdfDocViewerComponent implements OnInit {
  public safeUrl: SafeResourceUrl;
  public fileUrl: string;
  public isPDFType: boolean;
  public isFileValid: boolean;

  constructor(
    @Optional() public ref: DynamicDialogRef,
    @Optional() public config: DynamicDialogConfig,
    private sanitizer: DomSanitizer
  ) { }

  ngOnInit(): void {
    Object.assign(this, this.config.data);
    const url = this.isPDFType ? `${this.fileUrl}#zoom=100` : `https://docs.google.com/viewerng/viewer?url=${this.fileUrl}`;
    this.safeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
  }
}

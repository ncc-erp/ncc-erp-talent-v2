import { Injectable } from '@angular/core';
import { PdfDocViewerComponent } from '@shared/components/pdf-doc-viewer/pdf-doc-viewer.component';
import { DialogService } from 'primeng/dynamicdialog';

@Injectable({
  providedIn: 'root'
})

export class CustomDialogService {
  const

  constructor(
    public _dialog: DialogService
  ) {}

  openPDFDocViewerDialog(fileUrl: string) {
    const isFileValid = this.doesFileExist(fileUrl);

    if (!this.isDocUrl(fileUrl) && !this.isPdfUrl(fileUrl) && isFileValid) {
      window.open(fileUrl, '_blank');
      return;
    }

    this._dialog.open(PdfDocViewerComponent, {
      width: "60%",
      contentStyle: { height: '100vh', overflow: 'auto', 'min-width': '600px', 'border-radius': '8px' },
      styleClass: 'pdf-doc-viewer-dialog',
      baseZIndex: 10000,
      data: { fileUrl, isPDFType: this.isPdfUrl(fileUrl), isFileValid },
      showHeader: false,
      dismissableMask: true
    });
  }

  isDocUrl(url: string): boolean {
    return url.toLowerCase().endsWith('.doc') || url.toLowerCase().endsWith('.docx');
  }

  isPdfUrl(url: string): boolean {
    return url.toLowerCase().endsWith('.pdf');
  }

  doesFileExist(urlToFile: string) {
    var xhr = new XMLHttpRequest();
    xhr.open('HEAD', urlToFile, false);
    xhr.send();
    if (xhr.status === 404) {
        return false;
    } else {
        return true;
    }
  }
}

import { Component, OnInit } from "@angular/core";
import { MatDialogRef } from "@angular/material/dialog";
import  { ExportDialogService } from '../../../app/core/services/export/export-dialog.service'
@Component({
  selector: "talent-export-dialog",
  templateUrl: "./export-dialog.component.html",
  styleUrls: ["./export-dialog.component.scss"],
})
export class ExportDialogComponent implements OnInit {
  Loading: boolean = true;
  constructor(
    public dialogService: MatDialogRef<ExportDialogComponent>,
    public exportService: ExportDialogService
  ) {}
  ngOnInit() {
    this.exportService.getDownloadStatus().subscribe((status) => {
      this.Loading = status;    
    });
  }
  clonedialog() {
    this.dialogService.close()
  }
}

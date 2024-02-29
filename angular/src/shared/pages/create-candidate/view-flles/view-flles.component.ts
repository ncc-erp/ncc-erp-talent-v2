import { Component, OnDestroy, OnInit } from '@angular/core';

@Component({
  selector: 'talent-view-flles',
  templateUrl: './view-flles.component.html',
  styleUrls: ['./view-flles.component.scss']
})
export class ViewFllesComponent implements OnInit, OnDestroy {
  documentUrl: string = '';
  constructor() { }

  ngOnInit() {
    this.documentUrl = localStorage.getItem('documentUrl') || '';
  }
  ngOnDestroy(): void {
    localStorage.removeItem('documentUrl');
  }

}

import { BreadCrumbConfig } from '@app/core/models/common/common.dto';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'talent-section-breadcrumb',
  templateUrl: './section-breadcrumb.component.html',
  styleUrls: ['./section-breadcrumb.component.scss']
})
export class SectionBreadcrumbComponent implements OnInit {

  @Input() breadcrumbConfig: BreadCrumbConfig;
  @Input() showBreadCrumb = true;
  @Input() isShowBtn = false;
  @Input() btnConfig = {
    label: 'Create',
    icon: 'pi pi-plus'
  };

  @Output() onBtnClick = new EventEmitter<any>();

  constructor() { }

  ngOnInit(): void { }

  onBtnClicked() {
    this.onBtnClick.emit();
  }

}

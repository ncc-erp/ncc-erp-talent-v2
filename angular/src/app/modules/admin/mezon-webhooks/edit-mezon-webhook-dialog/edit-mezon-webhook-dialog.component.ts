import { Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { MezonWebhookDto } from '@app/core/models/mezon-webhook/mezon-webhook.model';
import { CommonService } from '@app/core/services/common.service';
import { MezonWebhookService } from '@app/core/services/mezon-webhook/mezon-webhook.service';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'talent-edit-mezon-webhook-dialog',
  templateUrl: './edit-mezon-webhook-dialog.component.html',
  styleUrls: ['./edit-mezon-webhook-dialog.component.scss']
})
export class EditMezonWebhookDialogComponent extends AppComponentBase implements OnInit {
  saving = false;
  webhook: MezonWebhookDto = new MezonWebhookDto();
  id: number;

  filterMessageFunctionConfig = {
    catalogList: [],
    optionLabel: 'name',
    optionValue: 'key'
  }

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _common: CommonService,
    public _mezonWebhookService: MezonWebhookService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._common.getSupportedMezonMessageFunction().subscribe((res) => {
      if (res.success) {
        this.filterMessageFunctionConfig.catalogList = res.result;
      }
    });

    this._mezonWebhookService.getById(this.id).subscribe((res) => {
      if (res.success) {
        this.webhook = res.result;
      }
    });
  }

  onFunctionChange(value: any) {
    this.webhook.functions = value;
  }

  save(): void {
    this.saving = true;

    this._mezonWebhookService.update(this.webhook).subscribe(
      () => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      () => {
        this.saving = false;
      }
    );
  }
}

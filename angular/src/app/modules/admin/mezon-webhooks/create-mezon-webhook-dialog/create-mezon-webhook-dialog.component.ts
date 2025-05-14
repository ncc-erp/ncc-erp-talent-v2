import { Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { MezonWebhookDto } from '@app/core/models/mezon-webhook/mezon-webhook.model';
import { CommonService } from '@app/core/services/common.service';
import { MezonWebhookService } from '@app/core/services/mezon-webhook/mezon-webhook.service';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'talent-create-mezon-webhook-dialog',
  templateUrl: './create-mezon-webhook-dialog.component.html',
  styleUrls: ['./create-mezon-webhook-dialog.component.scss']
})
export class CreateMezonWebhookDialogComponent extends AppComponentBase implements OnInit {

  saving = false;
  webhook: MezonWebhookDto = new MezonWebhookDto();

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
    this.webhook.isActive = true;
    this._common.getSupportedMezonMessageFunction().subscribe((res) => {
      if (res.success) {
        this.filterMessageFunctionConfig.catalogList = res.result;
      }
    });
  }

  onFunctionChange(value: any) {
    this.webhook.functions = value;
  }

  save(): void {
    this.saving = true;

    this._mezonWebhookService.create(this.webhook).subscribe(
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

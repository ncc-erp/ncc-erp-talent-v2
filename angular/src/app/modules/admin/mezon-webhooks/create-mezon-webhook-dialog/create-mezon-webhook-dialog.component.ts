import { Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { MezonWebhookDto } from '@app/core/models/mezon-webhook/mezon-webhook.model';
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

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    public _mezonWebhookService: MezonWebhookService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.webhook.isActive = true;
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

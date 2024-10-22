import { Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { MezonWebhookDto } from '@app/core/models/mezon-webhook/mezon-webhook.model';
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

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    public _mezonWebhookService: MezonWebhookService
  ) {
    super(injector);
    
  }

  ngOnInit(): void{
    this._mezonWebhookService.getById(this.id).subscribe((result) => {
      this.webhook = result.result;
    });
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

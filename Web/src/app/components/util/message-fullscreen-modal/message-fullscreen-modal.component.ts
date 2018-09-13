import { Component, OnInit, EventEmitter, Input, Output, NgZone } from '@angular/core';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { Router } from '@angular/router';

@Component({
  selector: 'message-fullscreen-modal',
  templateUrl: './message-fullscreen-modal.component.html',
  styleUrls: ['./message-fullscreen-modal.component.css']
})
export class MessageFullscreenModalComponent implements ModalComponent, OnInit {
  modalTitle: string = "";
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();

  constructor(private router: Router, private zone: NgZone) { 
  }

  ngOnInit() {
  }

  onOk() {
    if (!!this.data) {
      if (this.data.redirectUrl !== undefined && this.data.redirectUrl !== null) {
        this.zone.run(() => this.router.navigateByUrl(this.data.redirectUrl));
        this.setClose.emit();
      } else if (this.data.reload) {
        window.location.reload();
      }
    }
    this.setClose.emit();
  }
}

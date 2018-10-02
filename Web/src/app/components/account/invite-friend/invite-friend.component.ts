import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';

@Component({
  selector: 'invite-friend',
  templateUrl: './invite-friend.component.html',
  styleUrls: ['./invite-friend.component.css']
})
export class InviteFriendComponent implements OnInit, ModalComponent {
  modalTitle: string = "Invite a friend";
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();

  constructor() { }

  ngOnInit() {
  }

}

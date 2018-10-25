import { Component, OnInit, Input, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { CONFIG } from '../../../services/config.service';
import { AccountService } from '../../../services/account.service';

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
  link: string = '';
  description: string = 'Sign up for Auctus Experts today and follow trading recommendations from transparently tracked and ranked crypto experts. Use this referral link for a 20% discount!';
  copied: boolean = false;
  bonusAmount: number;
  @ViewChild("Link") Link: ElementRef;

  constructor(private accountService: AccountService) { }

  ngOnInit() {
    this.accountService.getReferralProgramInfo().subscribe(result => 
      {
        if (!!result) {
          this.bonusAmount = result.bonusToReferred;
          this.link = CONFIG.webUrl + '?register=true&ref=' + result.referralCode;
        }
      });
  }

  copyClick() {
    this.copied = true;
    this.Link.nativeElement.select();
    document.execCommand('copy');
    this.Link.nativeElement.setSelectionRange(0, 0);
  }

}

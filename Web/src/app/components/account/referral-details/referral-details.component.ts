import { Component, OnInit, Input, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { ReferralProgramInfoResponse } from '../../../model/account/ReferralProgramInfoResponse';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { NavigationService } from '../../../services/navigation.service';
import { CONFIG } from '../../../services/config.service';

@Component({
  selector: 'referral-details',
  templateUrl: './referral-details.component.html',
  styleUrls: ['./referral-details.component.css']
})
export class ReferralDetailsComponent implements ModalComponent, OnInit {
  modalTitle: string = "Rewards Summary";
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();
  
  link: string = '';
  copied: boolean = false;
  code: string = '';
  pending: string = '';
  available: string = '';
  cashedOut: string = '';
  canceled: string = '';
  bonusAmount: string = '';
  @ViewChild("Link") Link: ElementRef;

  constructor(private accountService: AccountService,
    private navigationService : NavigationService) { }

  ngOnInit() {
    let loginData = this.accountService.getLoginData();
    if (!loginData) {
      this.setClose.emit();
      this.navigationService.goToLogin();
    } else if (!loginData.isAdvisor) {
      this.setClose.emit();
      this.navigationService.goToCompleteRegistration();
    } else {
      this.accountService.getReferralProgramInfo().subscribe(result => 
        {
          if (!!result) {
            this.link = CONFIG.webUrl + '?register=true&ref=' + result.referralCode;
            this.code = result.referralCode;
            this.available = this.getStringValue(result.available);
            this.pending = this.getStringValue(result.pending);
            this.cashedOut = this.getStringValue(result.cashedOut) ;
            this.canceled = this.getStringValue(result.canceled);
            this.bonusAmount = this.getStringValue(result.bonusToReferred);
          }
        });
    }
  }

  getStringValue(value: number) {
    return value.toString(10) + " AUC";
  }

  copyClick() {
    this.copied = true;
    this.Link.nativeElement.select();
    document.execCommand('copy');
    this.Link.nativeElement.setSelectionRange(0, 0);
  }

  getReferralCodeOptions() {
    return { textOptions: { placeHolder: "Referral code", showValidatorError: false, showHintSize: false, required: false, disabled: true } };
  }
}

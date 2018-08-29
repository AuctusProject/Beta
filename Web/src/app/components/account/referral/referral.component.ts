import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { ReferralProgramInfoResponse } from '../../../model/account/ReferralProgramInfoResponse';

@Component({
  selector: 'referral',
  templateUrl: './referral.component.html',
  styleUrls: ['./referral.component.css']
})
export class ReferralComponent implements OnInit {
  referralInfo : ReferralProgramInfoResponse;
  constructor(private accountService: AccountService) { }

  ngOnInit() {
    this.accountService.getReferralProgramInfo().subscribe(result => this.referralInfo = result);
  }

}

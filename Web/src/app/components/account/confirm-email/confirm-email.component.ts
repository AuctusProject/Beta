import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../../services/account.service';

@Component({
  selector: 'confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css']
})
export class ConfirmEmailComponent implements OnInit {
  loggedUserEmail : string;

  constructor(private accountService : AccountService) { }

  ngOnInit() {
    this.loggedUserEmail = this.accountService.getUserEmail();
  }

  resendConfirmationEmail(){
    this.accountService.resendConfirmationEmail();
  }

}

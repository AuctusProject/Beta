import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';

@Component({
  selector: 'confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css']
})
export class ConfirmEmailComponent implements OnInit {
  loggedUserEmail : string;

  constructor(private accountService : AccountService, private notificationService : NotificationsService) { }

  ngOnInit() {
    this.loggedUserEmail = this.accountService.getUserEmail();
  }

  resendEmailConfirmation(){
    this.accountService.resendEmailConfirmation().subscribe(result => this.notificationService.success(null, "A new confirmation email was sent."));
  }

}

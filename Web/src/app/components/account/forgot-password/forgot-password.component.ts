import { Component, OnInit } from '@angular/core';
import { ForgotPasswordRequest } from '../../../model/account/forgotPasswordRequest';
import { AccountService } from '../../../services/account.service';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';

@Component({
  selector: 'forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent implements OnInit {
  forgotPasswordRequest: ForgotPasswordRequest = new ForgotPasswordRequest();
  constructor(private accountService: AccountService, private notificationsService: NotificationsService) { }

  ngOnInit() {
  }

  send(){
    this.accountService.forgotPassword(this.forgotPasswordRequest).subscribe(result => this.notificationsService.success(null, "Please follow the instructions on your email to recover your password."));
  }
}

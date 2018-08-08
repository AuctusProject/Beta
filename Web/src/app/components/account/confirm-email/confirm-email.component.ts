import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';
import { ActivatedRoute, Router } from '../../../../../node_modules/@angular/router';
import { ConfirmEmailRequest } from '../../../model/account/confirmEmailRequest';
import { AuthRedirect } from '../../../providers/authRedirect';

@Component({
  selector: 'confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css']
})
export class ConfirmEmailComponent implements OnInit {
  loggedUserEmail: string;
  confirmationCode: string;
  constructor(private route: ActivatedRoute, 
    private accountService : AccountService,
    private notificationService : NotificationsService,
    private authRedirect: AuthRedirect) { }

  ngOnInit() {
    this.loggedUserEmail = this.accountService.getUserEmail();
    this.getConfirmationCodeAndConfirmEmail();
  }

  getConfirmationCodeAndConfirmEmail(){
    this.confirmationCode = this.route.snapshot.queryParams['c'];
    if(this.confirmationCode){
      this.confirmEmail();
    }
  }

  confirmEmail(){
    var confirmEmailRequest = new ConfirmEmailRequest();
    confirmEmailRequest.code = this.confirmationCode;
    this.accountService.confirmEmail(confirmEmailRequest).subscribe(result => 
      {
        this.accountService.setLoginData(result);
        this.notificationService.success(null, "Email confirmed successfully");
        this.authRedirect.redirectAfterLoginAction();
      }
    );
  }

  resendEmailConfirmation(){
    this.accountService.resendEmailConfirmation().subscribe(result => this.notificationService.success(null, "A new confirmation email was sent."));
  }

}

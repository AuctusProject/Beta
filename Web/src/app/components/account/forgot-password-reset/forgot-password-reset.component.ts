import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { AccountService } from '../../../services/account.service';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';
import { RecoverPasswordRequest } from '../../../model/account/recoverPasswordRequest';
import { NavigationService } from '../../../services/navigation.service';

@Component({
  selector: 'forgot-password-reset',
  templateUrl: './forgot-password-reset.component.html',
  styleUrls: ['./forgot-password-reset.component.css']
})
export class ForgotPasswordResetComponent implements OnInit {
  recoverPasswordRequest: RecoverPasswordRequest = new RecoverPasswordRequest();
  constructor(
    private route: ActivatedRoute,
    private accountService : AccountService,
    private notificationService : NotificationsService,
    private navigationService: NavigationService) { }

  ngOnInit() {
    this.recoverPasswordRequest.code = this.route.snapshot.queryParams['c'];
  }

  send(){
    this.accountService.recoverPassword(this.recoverPasswordRequest).subscribe(result => 
      {
        this.notificationService.success(null, "Password was changed.");
        this.navigationService.goToLogin();
      }
    );
  }

}

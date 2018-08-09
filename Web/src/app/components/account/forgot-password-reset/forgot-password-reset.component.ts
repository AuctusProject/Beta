import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '../../../../../node_modules/@angular/router';
import { AccountService } from '../../../services/account.service';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';
import { RecoverPasswordRequest } from '../../../model/account/recoverPasswordRequest';

@Component({
  selector: 'forgot-password-reset',
  templateUrl: './forgot-password-reset.component.html',
  styleUrls: ['./forgot-password-reset.component.css']
})
export class ForgotPasswordResetComponent implements OnInit {
  recoverPasswordRequest: RecoverPasswordRequest = new RecoverPasswordRequest();
  constructor(
    private route: ActivatedRoute,
    private router: Router, 
    private accountService : AccountService,
    private notificationService : NotificationsService) { }

  ngOnInit() {
    this.recoverPasswordRequest.code = this.route.snapshot.queryParams['c'];
  }

  send(){
    this.accountService.recoverPassword(this.recoverPasswordRequest).subscribe(result => 
      {
        this.notificationService.success(null, "Password was changed.");
        this.router.navigateByUrl('login');
      }
    );
  }

}
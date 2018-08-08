import { Component, OnInit } from '@angular/core';
import { ChangePasswordRequest } from '../../../model/account/changePasswordRequest';
import { AccountService } from '../../../services/account.service';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';

@Component({
  selector: 'change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements OnInit {
  changePasswordRequest : ChangePasswordRequest = new ChangePasswordRequest();
  constructor(private accountService : AccountService, private notificationsService: NotificationsService) { }

  ngOnInit() {
  }

  send(){
    this.accountService.changePassword(this.changePasswordRequest).subscribe(result =>
      this.notificationsService.success(null, "Password was changed.")
    );
  }
}

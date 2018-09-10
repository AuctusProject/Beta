import { Component, OnInit, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { AccountService } from '../../../services/account.service';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';
import { RecoverPasswordRequest } from '../../../model/account/recoverPasswordRequest';
import { NavigationService } from '../../../services/navigation.service';
import { Subscription } from 'rxjs';
import { InheritanceInputComponent } from '../../util/inheritance-input/inheritance-input.component';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { InputType } from '../../../model/inheritanceInputOptions';
import { AuthRedirect } from '../../../providers/authRedirect';

@Component({
  selector: 'forgot-password-reset',
  templateUrl: './forgot-password-reset.component.html',
  styleUrls: ['./forgot-password-reset.component.css']
})
export class ForgotPasswordResetComponent implements ModalComponent, OnInit {
  modalTitle: string = "Set a new password";
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();
  
  recoverPasswordRequest: RecoverPasswordRequest = new RecoverPasswordRequest();
  promise: Subscription;
  @ViewChild("Password") Password: InheritanceInputComponent;
  
  constructor(
    private route: ActivatedRoute,
    private accountService : AccountService,
    private notificationService : NotificationsService,
    private authRedirect : AuthRedirect,
    private navigationService: NavigationService) { }

  ngOnInit() {
    this.recoverPasswordRequest.code = this.route.snapshot.queryParams['c'];
    if (!this.recoverPasswordRequest.code) {
      this.setClose.emit();
      this.navigationService.goToLogin();
    }
  }

  send() {
    if (this.Password.isValid()) {
      this.accountService.recoverPassword(this.recoverPasswordRequest).subscribe(result => 
        {
          this.accountService.setLoginData(result.data);
          this.notificationService.success(null, "Password was changed successfully");
          this.setClose.emit();
          this.authRedirect.redirectAfterLoginAction();
        }
      );
    }
  }

  getPasswordOptions() {
    return { inputType: InputType.Password, textOptions: { placeHolder: "New password", browserAutocomplete: "", minLength: 8, maxLength: 100 } };
  }
}

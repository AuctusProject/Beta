import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { ChangePasswordRequest } from '../../../model/account/changePasswordRequest';
import { AccountService } from '../../../services/account.service';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { NavigationService } from '../../../services/navigation.service';
import { Subscription } from 'rxjs';
import { InheritanceInputComponent } from '../../util/inheritance-input/inheritance-input.component';
import { InputType } from '../../../model/inheritanceInputOptions';

@Component({
  selector: 'change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements ModalComponent, OnInit {
  modalTitle: string = "Change password";
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();
  
  changePasswordRequest : ChangePasswordRequest = new ChangePasswordRequest();
  promise: Subscription;

  @ViewChild("CurrentPassword") CurrentPassword: InheritanceInputComponent;
  @ViewChild("NewPassword") NewPassword: InheritanceInputComponent;

  constructor(private accountService : AccountService, 
    private notificationService: NotificationsService,
    private navigationService : NavigationService) { }

  ngOnInit() {
    if (!this.accountService.isLoggedIn()) {
      this.setClose.emit();
      this.navigationService.goToLogin();
    }
  }

  send() {
    let isValid = this.NewPassword.isValid();
    if (this.CurrentPassword.isValid() && isValid) {
      this.promise = this.accountService.changePassword(this.changePasswordRequest).subscribe(result =>
        {
          this.notificationService.success(null, "Password was changed successfully");
          this.setClose.emit();
        }
      );
    }
  }

  getCurrentPasswordOptions() {
    return { inputType: InputType.Password, textOptions: { placeHolder: "Current password", minLength: 8, maxLength: 100 } };
  }

  getNewPasswordOptions() {
    return { inputType: InputType.Password, textOptions: { placeHolder: "New password", browserAutocomplete: "off", minLength: 8, maxLength: 100 } };
  }
}

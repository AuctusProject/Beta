import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { NotificationsService } from 'angular2-notifications';
import { ActivatedRoute } from '@angular/router';
import { ConfirmEmailRequest } from '../../../model/account/confirmEmailRequest';
import { AuthRedirect } from '../../../providers/authRedirect';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { MessageFullscreenModalComponent } from '../../util/message-fullscreen-modal/message-fullscreen-modal.component';
import { Subscription } from 'rxjs';
import { NavigationService } from '../../../services/navigation.service';
import { LoginResponse } from '../../../model/account/loginResponse';

@Component({
  selector: 'confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css']
})
export class ConfirmEmailComponent implements ModalComponent, OnInit {
  modalTitle: string = "Pending email confirmation";
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();
  
  loggedUserEmail: string;
  confirmationCode: string;
  promise: Subscription;
  
  constructor(private route: ActivatedRoute, 
    private accountService : AccountService,
    private notificationService : NotificationsService,
    private authRedirect: AuthRedirect,
    private navigationService : NavigationService) { }

  ngOnInit() { 
    this.confirmationCode = this.route.snapshot.queryParams['c'];
    if (this.accountService.isLoggedIn()) {
      let userData = this.accountService.getLoginData();
      if (!userData.pendingConfirmation) {
        this.setConfirmedEmailAction();
      } else {
        this.loggedUserEmail = userData.email;
        if (!!this.confirmationCode) {
          this.confirmEmail();
        }
      }
    } else if (!this.confirmationCode) {
      this.setClose.emit();
      this.navigationService.goToLogin();
    } else {
      this.confirmEmail();
    }
  }

  confirmEmail() {
    var confirmEmailRequest = new ConfirmEmailRequest();
    confirmEmailRequest.code = this.confirmationCode;
    this.accountService.confirmEmail(confirmEmailRequest).subscribe(result => 
      {
        this.accountService.setLoginData(result.data);
        this.setConfirmedEmailAction(result.data);
      }
    );
  }

  setConfirmedEmailAction(loginResponse?: LoginResponse) {
    this.notificationService.success(null, "Email confirmed successfully");
    this.setClose.emit();
    this.authRedirect.redirectAfterLoginAction(loginResponse);
  }

  resendEmailConfirmation() {
    this.promise = this.accountService.resendEmailConfirmation().subscribe(result => {
      let modalData = new FullscreenModalComponentInput();
      modalData.hiddenClose = true;
      modalData.component = MessageFullscreenModalComponent;
      modalData.componentInput = { message: "The confirmation email was resent. Please check your mail box and follow the instructions.", redirectUrl: "" };
      this.setNewModal.emit(modalData);
    });
  }
}

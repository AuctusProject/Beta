import { Component, OnInit, Output, Input, EventEmitter, ViewChild } from '@angular/core';
import { ForgotPasswordRequest } from '../../../model/account/forgotPasswordRequest';
import { AccountService } from '../../../services/account.service';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Subscription } from '../../../../../node_modules/rxjs';
import { RecaptchaComponent } from '../../util/recaptcha/recaptcha.component';
import { MessageFullscreenModalComponent } from '../../util/message-fullscreen-modal/message-fullscreen-modal.component';
import { InheritanceInputComponent } from '../../util/inheritance-input/inheritance-input.component';
import { InputType } from '../../../model/inheritanceInputOptions';

@Component({
  selector: 'forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent implements ModalComponent, OnInit {
  modalTitle: string = "Forgot password";
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();

  forgotPasswordRequest: ForgotPasswordRequest = new ForgotPasswordRequest();
  promise: Subscription;
  @ViewChild("RecaptchaComponent") RecaptchaComponent: RecaptchaComponent;
  @ViewChild("Email") Email: InheritanceInputComponent;

  constructor(private notificationsService: NotificationsService, 
    private accountService: AccountService) { 
  }

  ngOnInit() {
  }

  send(){
    if (!this.forgotPasswordRequest.captcha) {
      this.notificationsService.error(null, "You must fill the captcha.");
    } else if (this.Email.isValid()) {
      this.accountService.forgotPassword(this.forgotPasswordRequest).subscribe(result => 
        {
          let modalData = new FullscreenModalComponentInput();
          modalData.component = MessageFullscreenModalComponent;
          modalData.componentInput = { message: "Please follow the instructions on your email to recover your password.", redirectUrl: "" };
          this.setNewModal.emit(modalData);
        }, this.RecaptchaComponent.reset);
    }
  }

  public onCaptchaResponse(captchaResponse: string) {
    this.forgotPasswordRequest.captcha = captchaResponse;
  }

  getEmailOptions() {
    return { inputType: InputType.Email, textOptions: { placeHolder: "Email", showHintSize: false, maxLength: 50 } };
  }
}

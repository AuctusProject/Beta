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
  forgotPasswordForm: FormGroup;
  promise: Subscription;
  @ViewChild("RecaptchaComponent") RecaptchaComponent: RecaptchaComponent;

  constructor(private formBuilder: FormBuilder, 
    private accountService: AccountService) { 
    this.buildForm(); 
  }

  ngOnInit() {
  }

  private buildForm() {
    this.forgotPasswordForm = this.formBuilder.group({
      email: ['', Validators.compose([Validators.required, Validators.maxLength(50)])]
    });
  }

  send(){
    this.accountService.forgotPassword(this.forgotPasswordRequest).subscribe(result => 
      {
        let modalData = new FullscreenModalComponentInput();
        modalData.component = MessageFullscreenModalComponent;
        modalData.componentInput = { message: "Please follow the instructions on your email to recover your password.", redirectUrl: "" };
        this.setNewModal.emit(modalData);
      }, this.RecaptchaComponent.reset);
  }

  public onCaptchaResponse(captchaResponse: string) {
    this.forgotPasswordRequest.captcha = captchaResponse;
  }
}

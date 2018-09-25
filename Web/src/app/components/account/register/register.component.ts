import { Component, OnInit, ViewChild, NgZone, Input, Output, EventEmitter } from '@angular/core';
import { RegisterRequest } from '../../../model/account/registerRequest';
import { Subscription } from 'rxjs';
import { AccountService } from '../../../services/account.service';
import { ActivatedRoute } from '@angular/router';
import { NotificationsService } from 'angular2-notifications';
import { AuthRedirect } from '../../../providers/authRedirect';
import { RecaptchaComponent } from '../../util/recaptcha/recaptcha.component';
import { InheritanceInputComponent } from '../../util/inheritance-input/inheritance-input.component';
import { InputType } from '../../../model/inheritanceInputOptions';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { LoginResult } from '../../../model/account/loginResult';
import { ForgotPasswordComponent } from '../forgot-password/forgot-password.component';
import { LocalStorageService } from '../../../services/local-storage.service';
import { EntryOptionComponent } from '../entry-option/entry-option.component';

@Component({
  selector: 'register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements ModalComponent, OnInit {
  modalTitle: string = "Investor register";
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();
 
  registerRequest: RegisterRequest = new RegisterRequest();
  registerPromise: Subscription;
  @ViewChild("RecaptchaComponent") RecaptchaComponent: RecaptchaComponent;
  @ViewChild("Password") Password: InheritanceInputComponent;
  @ViewChild("Email") Email: InheritanceInputComponent;
  @ViewChild("Referral") Referral: InheritanceInputComponent;
  
  discountMessage: string = "";

  constructor(private notificationsService: NotificationsService,
    private accountService: AccountService,
    private authRedirect : AuthRedirect,
    private activatedRoute: ActivatedRoute,
    private localStorageService: LocalStorageService) { }

  ngOnInit() {
    if (this.accountService.isLoggedIn()) {
      this.setClose.emit();
      this.authRedirect.redirectAfterLoginAction();
    } else {
      this.registerRequest.referralCode = this.activatedRoute.snapshot.queryParams['ref'];
      if (!!this.registerRequest.referralCode) {
        this.validateReferralCode(this.registerRequest.referralCode);
      }
    }
  }

  onCreateAccountClick(){ 
    if (!this.registerRequest.captcha) {
      this.notificationsService.error(null, "You must fill the captcha.");
    } else if (this.isValidRequest()) {
      this.registerPromise = this.accountService.register(this.registerRequest)
        .subscribe(response => { this.registerResponse(response); }, this.RecaptchaComponent.reset);
    }
  }
  
  onForgotPasswordClick() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = ForgotPasswordComponent;
    this.setNewModal.emit(modalData);
  }

  registerResponse(response: LoginResult){
    if (!!response && !response.error && response.data) {
      this.accountService.setLoginData(response.data);
      this.setClose.emit();
      this.authRedirect.redirectAfterLoginAction(response.data);
    } else {
      if (!!response && response.error) {
        this.notificationsService.info("Info", response.error);
      }
      this.RecaptchaComponent.reset();
    }
  }

  onChangeReferralCode(value: string) {
    this.registerRequest.referralCode = value;
    this.validateReferralCode(value);
  }

  validateReferralCode(value: string) {
    if (!value || value.length == 0) {
      this.setInvalidReferral("");
    } else if (!!value && value.length == 7) {
      this.accountService.isValidReferralCode(value).subscribe(response => {
        if (!!response && response.valid) {
          this.Referral.setForcedError("");
          this.localStorageService.setLocalStorage("referralCode", value);
          this.discountMessage = "Congratulations, using the referral code you need hold " + response.discount + "% less AUC in your own wallet!" 
        } else {
          this.setInvalidReferral("Invalid referral code");
        }
      });
    } else {
      this.setInvalidReferral("Invalid referral code");
    }
  }

  setInvalidReferral(message: string) {
    this.Referral.setForcedError(message);
    this.discountMessage = "";
    this.localStorageService.setLocalStorage("referralCode", "");
  }

  public onCaptchaResponse(captchaResponse: string) {
    this.registerRequest.captcha = captchaResponse;
  }

  onLoginClick() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = EntryOptionComponent;
    modalData.componentInput = { login: true };
    this.setNewModal.emit(modalData);
  }

  isValidRequest() : boolean {
    let isValid = this.Password.isValid();
    isValid = this.Email.isValid() && isValid;
    return this.Referral.isValid() && isValid;
  }

  getEmailOptions() {
    return { inputType: InputType.Email, textOptions: { placeHolder: "Email", showHintSize: false, maxLength: 50 } };
  }

  getPasswordOptions() {
    return { inputType: InputType.Password, textOptions: { placeHolder: "Password", browserAutocomplete: "off", minLength: 8, maxLength: 100 } };
  }

  getReferralOptions() {
    return { textOptions: { outlineField: false, placeHolder: "Referral code (optional)", required: false, showHintSize: false, minLength: 7, maxLength: 7 } };
  }
}

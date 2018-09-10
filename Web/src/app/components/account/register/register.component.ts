import { Component, OnInit, ViewChild, NgZone, Input, Output, EventEmitter } from '@angular/core';
import { RegisterRequest } from '../../../model/account/registerRequest';
import { Subscription } from '../../../../../node_modules/rxjs';
import { AccountService } from '../../../services/account.service';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';
import { AuthRedirect } from '../../../providers/authRedirect';
import { RecaptchaComponent } from '../../util/recaptcha/recaptcha.component';
import { InheritanceInputComponent } from '../../util/inheritance-input/inheritance-input.component';
import { InputType } from '../../../model/inheritanceInputOptions';
import { SocialLoginRequest } from '../../../model/account/socialLoginRequest';
import { FacebookLoginProvider, GoogleLoginProvider, AuthService } from 'angular5-social-login';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { LoginComponent } from '../login/login.component';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { LoginResult } from '../../../model/account/loginResult';

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
  
  acceptTermsAndConditions: boolean;
  discountMessage: string = "";

  constructor(private notificationsService: NotificationsService,
    private accountService: AccountService,
    private authRedirect : AuthRedirect,
    private activatedRoute: ActivatedRoute,
    private socialAuthService: AuthService,
    private zone: NgZone) { }

  ngOnInit() {
    if (this.accountService.isLoggedIn()) {
      this.setClose.emit();
      this.authRedirect.redirectAfterLoginAction();
    } else {
      this.registerRequest.referralCode = this.activatedRoute.snapshot.queryParams['ref'];
      if (!!this.registerRequest.referralCode) this.validateReferralCode(this.registerRequest.referralCode);
    }
  }

  onCreateAccountClick(){ 
    if (!this.acceptTermsAndConditions) {
      this.notificationsService.error(null, "You must accept the terms and conditions to continue.");
    } else if (!this.registerRequest.captcha) {
      this.notificationsService.error(null, "You must fill the captcha.");
    } else if (this.isValidRequest()) {
      this.registerPromise = this.accountService.register(this.registerRequest)
        .subscribe(response => { this.registerResponse(response); }, this.RecaptchaComponent.reset);
    }
  }
  
  public socialSignIn(socialPlatform : string) {
    let socialPlatformProvider;
    var socialNetworkType;
    if(socialPlatform == "facebook"){
      socialPlatformProvider = FacebookLoginProvider.PROVIDER_ID;
      socialNetworkType = 0;
    }else if(socialPlatform == "google"){
      socialPlatformProvider = GoogleLoginProvider.PROVIDER_ID;
      socialNetworkType = 1;
    }
    
    this.socialAuthService.signIn(socialPlatformProvider).then(
      (userData) => {
        var request = new SocialLoginRequest();
        request.email = userData.email;
        request.token = userData.token;
        request.socialNetworkType = socialNetworkType;
        this.accountService.socialLogin(request).subscribe(result => this.zone.run(() => { this.registerResponse(result); }, this.RecaptchaComponent.reset));
      }
    );
  }

  registerResponse(response: LoginResult){
    if (!!response && !response.error && response.data) {
      this.accountService.setLoginData(response.data);
      this.setClose.emit();
      this.authRedirect.redirectAfterLoginAction();
    }
    else {
      if (!!response) this.notificationsService.info("Info", response.error);
      this.RecaptchaComponent.reset();
    }
  }

  onChangeReferralCode(value: string) {
    this.registerRequest.referralCode = value;
    this.validateReferralCode(value);
  }

  validateReferralCode(value: string) {
    if (!value || value.length == 0) {
      this.Referral.setForcedError("");
      this.discountMessage = "";
    } else if (!!value && value.length == 7) {
      this.accountService.isValidReferralCode(value).subscribe(response => {
        if (!!response && response.valid) {
          this.Referral.setForcedError("");
          this.discountMessage = "Congratulations, using the referral code you need hold " + response.discount + "% less AUC in your wallet!" 
        } else {
          this.Referral.setForcedError("Invalid referral code");
          this.discountMessage = "";
        }
      });
    } else {
      this.Referral.setForcedError("Invalid referral code");
      this.discountMessage = "";
    }
  }

  public onCaptchaResponse(captchaResponse: string) {
    this.registerRequest.captcha = captchaResponse;
  }

  onLoginClick() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = LoginComponent;
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
    return { inputType: InputType.Password, textOptions: { placeHolder: "Password", browserAutocomplete: "", minLength: 8, maxLength: 100 } };
  }

  getReferralOptions() {
    return { textOptions: { outlineField: false, placeHolder: "Referral code (optional)", required: false, showHintSize: false, minLength: 7, maxLength: 7 } };
  }
}

import { Component, OnInit, Input, NgZone, ViewChild, Output, EventEmitter } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { LoginRequest } from '../../../model/account/loginRequest';
import { Subscription } from '../../../../../node_modules/rxjs';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';
import { AuthRedirect } from '../../../providers/authRedirect';
import { AuthService, FacebookLoginProvider, GoogleLoginProvider } from 'angular5-social-login';
import { SocialLoginRequest } from '../../../model/account/socialLoginRequest';
import { LoginResult } from '../../../model/account/loginResult';
import { RecaptchaComponent } from '../../util/recaptcha/recaptcha.component';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { ForgotPasswordComponent } from '../forgot-password/forgot-password.component';
import { InheritanceInputComponent } from '../../util/inheritance-input/inheritance-input.component';
import { InputType } from '../../../model/inheritanceInputOptions';
import { RegisterComponent } from '../register/register.component';

@Component({
  selector: 'login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements ModalComponent, OnInit {
  modalTitle: string = "Login";
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();

  loginRequest: LoginRequest = new LoginRequest();
  promise: Subscription;
  @ViewChild("RecaptchaComponent") RecaptchaComponent: RecaptchaComponent;
  @ViewChild("Password") Password: InheritanceInputComponent;
  @ViewChild("Email") Email: InheritanceInputComponent;

  constructor(private formBuilder: FormBuilder, 
    private accountService: AccountService, 
    private notificationsService: NotificationsService,
    private authRedirect : AuthRedirect,
    private socialAuthService: AuthService,
    private zone : NgZone) { 
  }

  ngOnInit() {
    if (this.accountService.isLoggedIn()) {
      this.setClose.emit();
      this.authRedirect.redirectAfterLoginAction();
    }
  }

  onLoginClick(){
    if (!this.loginRequest.captcha) {
      this.notificationsService.error(null, "You must fill the captcha.");
    } else if (this.isValidRequest()) {
      this.promise = this.accountService.login(this.loginRequest)
        .subscribe(result => this.zone.run(() => { this.loginResponse(result); }), this.RecaptchaComponent.reset);
    }
  }

  loginResponse(response: LoginResult){
    if (!!response && !response.error && response.data) {
      this.accountService.setLoginData(response.data);
      this.setClose.emit();
      this.authRedirect.redirectAfterLoginAction();
    } else {
      if (!!response) this.notificationsService.info("Info", response.error);
      this.RecaptchaComponent.reset();
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
        this.accountService.socialLogin(request).subscribe(result => this.zone.run(() => { this.loginResponse(result); }, this.RecaptchaComponent.reset));
      }
    );
  }

  public onCaptchaResponse(captchaResponse: string) {
    this.loginRequest.captcha = captchaResponse;
  }

  onForgotPasswordClick() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = ForgotPasswordComponent;
    this.setNewModal.emit(modalData);
  }

  onRegisterClick() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = RegisterComponent;
    this.setNewModal.emit(modalData);
  }

  isValidRequest() : boolean {
    let isValid = this.Password.isValid();
    return this.Email.isValid() && isValid;
  }

  getEmailOptions() {
    return { inputType: InputType.Email, textOptions: { placeHolder: "Email", showHintSize: false, maxLength: 50 } };
  }

  getPasswordOptions() {
    return { inputType: InputType.Password, textOptions: { placeHolder: "Password", showHintSize: false, showPasswordVisibility: false, minLenth: 8, maxLength: 100 } };
  }
}

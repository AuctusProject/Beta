import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { NotificationsService } from 'angular2-notifications';
import { AuthRedirect } from '../../../providers/authRedirect';
import { SocialLoginRequest } from '../../../model/account/socialLoginRequest';
import { FacebookLoginProvider, GoogleLoginProvider, AuthService } from 'angular5-social-login';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { LoginComponent } from '../login/login.component';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { LoginResult } from '../../../model/account/loginResult';
import { ForgotPasswordComponent } from '../forgot-password/forgot-password.component';
import { RegisterComponent } from '../register/register.component';

@Component({
  selector: 'entry-option',
  templateUrl: './entry-option.component.html',
  styleUrls: ['./entry-option.component.css']
})
export class EntryOptionComponent implements ModalComponent, OnInit {
  modalTitle: string = "";
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();

  emailButtonMessage: string = "";
  headerFirstMessage: string = "";
  headerSecondMessage: string = "";

  constructor(private notificationsService: NotificationsService,
    private accountService: AccountService,
    private authRedirect : AuthRedirect,
    private socialAuthService: AuthService) { }

  ngOnInit() {
    if (this.accountService.isLoggedIn()) {
      this.setClose.emit();
      this.authRedirect.redirectAfterLoginAction();
    } else {
      if (this.isLogin()) {
        this.modalTitle = "Sign In";
        this.emailButtonMessage = "Sign in with Email";
        this.headerFirstMessage = "Choose how you want sign in.";
      } else {
        this.modalTitle = "Investor register";
        this.emailButtonMessage = "Sign up with Email";
        this.headerFirstMessage = "Choose how you want to register. It's totally free!";
        this.headerSecondMessage = "You only need to hold some AUC in your own wallet.";
      }
    }
  }

  isLogin() {
    return !!this.data && this.data.login;
  }
  
  socialEntry(socialPlatform: string) {
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
        this.accountService.socialLogin(request).subscribe(result => this.socialEntryResponse(result));
      }
    );
  }

  socialEntryResponse(response: LoginResult){
    if (!!response && !response.error && response.data) {
      this.accountService.setLoginData(response.data);
      this.setClose.emit();
      this.authRedirect.redirectAfterLoginAction();
    } else if (!!response && response.error) {
        this.notificationsService.info("Info", response.error);
    }
  }

  emailClick() {
    if (this.isLogin()) {
      let modalData = new FullscreenModalComponentInput();
      modalData.component = LoginComponent;
      this.setNewModal.emit(modalData);
    } else {
      let modalData = new FullscreenModalComponentInput();
      modalData.component = RegisterComponent;
      this.setNewModal.emit(modalData);
    }
  }

  onSignUpClick() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = EntryOptionComponent;
    this.setNewModal.emit(modalData);
  }

  onLoginClick() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = EntryOptionComponent;
    modalData.componentInput = { login: true };
    this.setNewModal.emit(modalData);
  }

  onForgotPasswordClick() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = ForgotPasswordComponent;
    this.setNewModal.emit(modalData);
  }
}

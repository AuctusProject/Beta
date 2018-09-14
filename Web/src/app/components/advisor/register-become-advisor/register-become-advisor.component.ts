import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { NotificationsService } from 'angular2-notifications';
import { AuthRedirect } from '../../../providers/authRedirect';
import { SocialLoginRequest } from '../../../model/account/socialLoginRequest';
import { FacebookLoginProvider, GoogleLoginProvider, AuthService } from 'angular5-social-login';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { LoginResult } from '../../../model/account/loginResult';
import { ModalService } from '../../../services/modal.service';
import { BecomeAdvisorComponent } from '../become-advisor/become-advisor.component';
import { EntryOptionComponent } from '../../account/entry-option/entry-option.component';
import { ForgotPasswordComponent } from '../../account/forgot-password/forgot-password.component';

@Component({
  selector: 'register-become-advisor',
  templateUrl: './register-become-advisor.component.html',
  styleUrls: ['./register-become-advisor.component.css']
})
export class RegisterBecomeAdvisorComponent implements ModalComponent, OnInit {
  modalTitle: string = "Become an Expert";
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();

  constructor(private notificationsService: NotificationsService,
    private accountService: AccountService,
    private socialAuthService: AuthService) { }

  ngOnInit() {
    if (this.accountService.isLoggedIn()) {
      let modalData = new FullscreenModalComponentInput();
      modalData.component = BecomeAdvisorComponent;
      this.setNewModal.emit(modalData);
    }
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
      let modalData = new FullscreenModalComponentInput();
      modalData.component = BecomeAdvisorComponent;
      this.setNewModal.emit(modalData);
    } else if (!!response && response.error) {
        this.notificationsService.info("Info", response.error);
    }
  }

  emailClick() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = BecomeAdvisorComponent;
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

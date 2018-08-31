import { Component, OnInit, Input, NgZone } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { LoginRequest } from '../../../model/account/loginRequest';
import { Subscription } from '../../../../../node_modules/rxjs';
import { FormGroup, FormBuilder, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';
import { AuthRedirect } from '../../../providers/authRedirect';
import { AuthService, FacebookLoginProvider, GoogleLoginProvider } from 'angular5-social-login';
import { SocialLoginRequest } from '../../../model/account/socialLoginRequest';
import { LoginResult } from '../../../model/account/loginResult';

@Component({
  selector: 'login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginRequest: LoginRequest = new LoginRequest();
  loginPromise: Subscription;
  loginForm: FormGroup;

  constructor(private formBuilder: FormBuilder, 
    private accountService: AccountService, 
    private notificationsService: NotificationsService,
    private authRedirect : AuthRedirect,
    private socialAuthService: AuthService,
    private zone : NgZone) { 
    this.buildForm();
  }

  private buildForm() {
    this.loginForm = this.formBuilder.group({
      email: ['', Validators.compose([Validators.required])],
      password: ['', Validators.compose([Validators.required])]
    });
  }

  ngOnInit() {
  }

  onLoginClick(){
    this.doLogin();
  }

  doLogin() {
    this.loginPromise = this.accountService.login(this.loginRequest)
      .subscribe(this.loginResponse);
  }

  loginResponse(response: LoginResult){
    if (response){
      if (!response.error && response.data) {
        this.accountService.setLoginData(response.data);
        this.authRedirect.redirectAfterLoginAction();
      }
      else {
        this.notificationsService.info("Info", response.error);
      }
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
        this.accountService.socialLogin(request).subscribe(result => this.zone.run(() => {this.loginResponse(result);}));
      }
    );
  }
}

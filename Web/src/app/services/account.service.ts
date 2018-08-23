import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { ValidateSignatureRequest } from '../model/account/validateSignatureRequest';
import { LoginResponse } from '../model/account/loginResponse';
import { LoginRequest } from '../model/account/loginRequest';
import { LoginResult } from '../model/account/loginResult';
import { ConfirmEmailRequest } from '../model/account/confirmEmailRequest';
import { ForgotPasswordRequest } from '../model/account/forgotPasswordRequest';
import { RecoverPasswordRequest } from '../model/account/recoverPasswordRequest';
import { ChangePasswordRequest } from '../model/account/changePasswordRequest';
import { RegisterRequest } from '../model/account/registerRequest';
import { RegisterResponse } from '../model/account/registerResponse';
import { FeedResponse } from '../model/advisor/feedResponse';
import { ReferralProgramInfoResponse } from '../model/account/ReferralProgramInfoResponse';
import { SetReferralRequest } from '../model/account/setReferralRequest';
import { ConfigurationResponse } from '../model/account/configurationResponse';
import { ConfigurationRequest } from '../model/account/configurationRequest';
import { NavigationService } from './navigation.service';


@Injectable()
export class AccountService {
  private validateSignatureUrl = this.httpService.apiUrl("/v1/accounts/me/signatures");
  private loginUrl = this.httpService.apiUrl("v1/accounts/login");
  private confirmationEmailUrl = this.httpService.apiUrl("v1/accounts/me/confirmations");
  private recoverPasswordUrl = this.httpService.apiUrl("v1/accounts/passwords/recover");
  private changePasswordUrl = this.httpService.apiUrl("v1/accounts/me/passwords");
  private registerUrl = this.httpService.apiUrl("v1/accounts");
  private listFeedUrl = this.httpService.apiUrl("v1/accounts/me/advices");
  private referralsUrl = this.httpService.apiUrl("v1/accounts/me/referrals");
  private configurationUrl = this.httpService.apiUrl("v1/accounts/me/configuration");

  constructor(private httpService : HttpService, private navigationService: NavigationService) { }

  validateSignature(validateSignatureRequest: ValidateSignatureRequest): Observable<LoginResponse> {
    return this.httpService.post(this.validateSignatureUrl, validateSignatureRequest);
  }

  login(loginRequest : LoginRequest):Observable<LoginResult>{
    return this.httpService.post(this.loginUrl, loginRequest);
  }

  setLoginData(loginData: LoginResponse): void {
    this.httpService.setLoginData(loginData);
  }

  getLoginData(): LoginResponse {
    return this.httpService.getLoginData();
  }

  getUserEmail(): string {
    return this.httpService.getUserEmail();
  }

  logout(): void {
    this.httpService.logout();
    this.navigationService.goToLogin();
  }

  logoutWithoutRedirect(): void {
    this.httpService.logout();
  }

  isLoggedIn() : boolean{
    return this.httpService.isLoggedIn();
  }

  resendEmailConfirmation() : Observable<void>{
    return this.httpService.get(this.confirmationEmailUrl);
  }

  confirmEmail(confirmEmailRequest: ConfirmEmailRequest) : Observable<LoginResponse>{
    return this.httpService.post(this.confirmationEmailUrl, confirmEmailRequest);
  }

  forgotPassword(forgotPasswordRequest: ForgotPasswordRequest) : Observable<void>{
    return this.httpService.post(this.recoverPasswordUrl, forgotPasswordRequest);
  }
  
  recoverPassword(recoverPasswordRequest: RecoverPasswordRequest) : Observable<void>{
    return this.httpService.put(this.recoverPasswordUrl, recoverPasswordRequest);
  }

  changePassword(changePasswordRequest: ChangePasswordRequest) : Observable<void>{
    return this.httpService.put(this.changePasswordUrl, changePasswordRequest);
  }

  register(registerRequest: RegisterRequest) : Observable<RegisterResponse>{
    return this.httpService.post(this.registerUrl, registerRequest)
  }

  listFeed(top? : number, lastAdviceId? : number) : Observable<FeedResponse>{
    var url = this.listFeedUrl + "?";
    if(top != null){
      url += "top="+top;
    }
    if(lastAdviceId != null){
      url += "&lastAdviceId="+lastAdviceId;
    }
    return this.httpService.get(url);
  }

  getReferralProgramInfo() : Observable<ReferralProgramInfoResponse>{
    return this.httpService.get(this.referralsUrl);
  }

  setReferralCode(setReferralRequest: SetReferralRequest) : Observable<void>{
    return this.httpService.post(this.referralsUrl, setReferralRequest);
  }

  getConfiguration() : Observable<ConfigurationResponse>{
    return this.httpService.get(this.configurationUrl);
  }

  setConfiguration(configurationRequest: ConfigurationRequest) : Observable<void>{
    return this.httpService.post(this.configurationUrl, configurationRequest);
  }
}
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
import { DashboardResponse } from '../model/admin/dashboardresponse';
import { SearchResponse } from '../model/search/searchResponse';
import { SocialLoginRequest } from '../model/account/socialLoginRequest';
import { ValidReferralCodeResponse } from '../model/account/validReferralCodeResponse';


@Injectable()
export class AccountService {
  private validateSignatureUrl = this.httpService.apiUrl("/v1/accounts/me/signatures");
  private loginUrl = this.httpService.apiUrl("v1/accounts/login");
  private socialLoginUrl = this.httpService.apiUrl("v1/accounts/social_login");
  private confirmationEmailUrl = this.httpService.apiUrl("v1/accounts/me/confirmations");
  private recoverPasswordUrl = this.httpService.apiUrl("v1/accounts/passwords/recover");
  private changePasswordUrl = this.httpService.apiUrl("v1/accounts/me/passwords");
  private registerUrl = this.httpService.apiUrl("v1/accounts");
  private listFeedUrl = this.httpService.apiUrl("v1/accounts/me/advices");
  private meReferralsUrl = this.httpService.apiUrl("v1/accounts/me/referrals");
  private referralsUrl = this.httpService.apiUrl("v1/accounts/referrals");
  private configurationUrl = this.httpService.apiUrl("v1/accounts/me/configuration");
  private dashboardUrl = this.httpService.apiUrl("v1/accounts/dashboard");
  private searchUrl = this.httpService.apiUrl("v1/accounts/search");

  constructor(private httpService : HttpService, private navigationService: NavigationService) { }

  validateSignature(validateSignatureRequest: ValidateSignatureRequest): Observable<LoginResponse> {
    return this.httpService.post(this.validateSignatureUrl, validateSignatureRequest);
  }

  login(loginRequest : LoginRequest):Observable<LoginResult>{
    return this.httpService.post(this.loginUrl, loginRequest);
  }

  socialLogin(socialLoginRequest : SocialLoginRequest):Observable<LoginResult>{
    return this.httpService.post(this.socialLoginUrl, socialLoginRequest);
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
    this.navigationService.goToHome();
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
  
  recoverPassword(recoverPasswordRequest: RecoverPasswordRequest) : Observable<LoginResult>{
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
    if(!!top) url += "top="+top;
    if(!!lastAdviceId) url += "&lastAdviceId="+lastAdviceId;
    return this.httpService.get(url);
  }

  getReferralProgramInfo() : Observable<ReferralProgramInfoResponse>{
    return this.httpService.get(this.meReferralsUrl);
  }

  setReferralCode(setReferralRequest: SetReferralRequest) : Observable<void>{
    return this.httpService.post(this.meReferralsUrl, setReferralRequest);
  }

  isValidReferralCode(referralCode: string) : Observable<ValidReferralCodeResponse>{
    return this.httpService.get(this.referralsUrl + "?referralCode=" + referralCode);
  }

  getConfiguration() : Observable<ConfigurationResponse>{
    return this.httpService.get(this.configurationUrl);
  }

  setConfiguration(configurationRequest: ConfigurationRequest) : Observable<void>{
    return this.httpService.post(this.configurationUrl, configurationRequest);
  }

  getDashboard() : Observable<DashboardResponse>{
    return this.httpService.get(this.dashboardUrl);
  }

  search(searchTerm: string): Observable<SearchResponse> {
    return this.httpService.get(this.searchUrl + "?term=" + searchTerm);
  }
}
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { ValidateSignatureRequest } from '../model/account/validateSignatureRequest';
import { LoginResponse } from '../model/account/loginResponse';
import { LoginRequest } from '../model/account/loginRequest';
import { Router } from '../../../node_modules/@angular/router';
import { LoginData } from '../model/account/loginData';
import { ConfirmEmailRequest } from '../model/account/confirmEmailRequest';
import { ForgotPasswordRequest } from '../model/account/forgotPasswordRequest';
import { RecoverPasswordRequest } from '../model/account/recoverPasswordRequest';
import { ChangePasswordRequest } from '../model/account/changePasswordRequest';


@Injectable()
export class AccountService {
  private baseGetAccountsUrl = this.httpService.apiUrl("v1/accounts");
  private validateSignatureUrl = this.httpService.apiUrl("/v1/accounts/me/signatures");
  private loginUrl = this.httpService.apiUrl("v1/accounts/login");
  private confirmationEmailUrl = this.httpService.apiUrl("v1/accounts/me/confirmation");
  private recoverPasswordUrl = this.httpService.apiUrl("v1/accounts/passwords/recover");
  private changePasswordUrl = this.httpService.apiUrl("v1/accounts/me/passwords");

  constructor(private httpService : HttpService, private router : Router) { }

  validateSignature(validateSignatureRequest: ValidateSignatureRequest): Observable<LoginData> {
    return this.httpService.post(this.validateSignatureUrl, validateSignatureRequest);
  }

  login(loginRequest : LoginRequest):Observable<LoginResponse>{
    return this.httpService.post(this.loginUrl, loginRequest);
  }

  setLoginData(loginData: LoginData): void {
    this.httpService.setLoginData(loginData);
  }

  getLoginData(): LoginData {
    return this.httpService.getLoginData();
  }

  getUserEmail(): string {
    return this.httpService.getUserEmail();
  }

  logout(): void {
    this.httpService.logout();
    this.router.navigateByUrl('home');
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

  confirmEmail(confirmEmailRequest: ConfirmEmailRequest) : Observable<LoginData>{
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
}
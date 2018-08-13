import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { ValidateSignatureRequest } from '../model/account/validateSignatureRequest';
import { LoginResponse } from '../model/account/loginResponse';
import { LoginRequest } from '../model/account/loginRequest';
import { Router } from '../../../node_modules/@angular/router';
import { LoginResult } from '../model/account/loginResult';
import { ConfirmEmailRequest } from '../model/account/confirmEmailRequest';
import { ForgotPasswordRequest } from '../model/account/forgotPasswordRequest';
import { RecoverPasswordRequest } from '../model/account/recoverPasswordRequest';
import { ChangePasswordRequest } from '../model/account/changePasswordRequest';
import { RegisterRequest } from '../model/account/registerRequest';
import { RegisterResponse } from '../model/account/registerResponse';


@Injectable()
export class AccountService {
  private baseGetAccountsUrl = this.httpService.apiUrl("v1/accounts");
  private validateSignatureUrl = this.httpService.apiUrl("/v1/accounts/me/signatures");
  private loginUrl = this.httpService.apiUrl("v1/accounts/login");
  private confirmationEmailUrl = this.httpService.apiUrl("v1/accounts/me/confirmations");
  private recoverPasswordUrl = this.httpService.apiUrl("v1/accounts/passwords/recover");
  private changePasswordUrl = this.httpService.apiUrl("v1/accounts/me/passwords");
  private registerUrl = this.httpService.apiUrl("v1/accounts")

  constructor(private httpService : HttpService, private router : Router) { }

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
}
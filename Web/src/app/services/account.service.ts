import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { ValidateSignatureRequest } from '../model/account/validateSignatureRequest';
import { LoginResponse } from '../model/account/loginResponse';
import { LoginRequest } from '../model/account/loginRequest';
import { Router } from '../../../node_modules/@angular/router';
import { LoginResult } from '../model/account/loginResult';
import { ConfirmEmailRequest } from '../model/account/confirmEmailRequest';
import { RegisterRequest } from '../model/account/registerRequest';
import { RegisterResponse } from '../model/account/registerResponse';


@Injectable()
export class AccountService {
  private baseGetAccountsUrl = this.httpService.apiUrl("accounts/v1");
  private validateSignatureUrl = this.httpService.apiUrl("accounts/v1/validate");
  private loginUrl = this.httpService.apiUrl("accounts/v1/login");
  private confirmationEmailUrl = this.httpService.apiUrl("accounts/v1/email/confirmation");
  private registerUrl = this.httpService.apiUrl("accounts/v1/register")

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

  register(registerRequest: RegisterRequest) : Observable<RegisterResponse>{
    return this.httpService.post(this.registerUrl, registerRequest)
  }
}
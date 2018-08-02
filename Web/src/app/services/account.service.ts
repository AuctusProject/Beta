import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { ValidateSignatureRequest } from '../model/account/validateSignatureRequest';
import { LoginResponse } from '../model/account/loginResponse';


@Injectable()
export class AccountService {
  private baseGetAccountsUrl = this.httpService.apiUrl("accounts/v1");
  private validateSignatureUrl = this.httpService.apiUrl("accounts/v1/validate");

  constructor(private httpService : HttpService) { }

  validateSignature(validateSignatureRequest: ValidateSignatureRequest): Observable<LoginResponse> {
    return this.httpService.post(this.validateSignatureUrl, validateSignatureRequest);
  }
}
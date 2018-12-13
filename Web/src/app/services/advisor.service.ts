import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { AdvisorResponse } from "../model/advisor/advisorResponse";
import { RequestToBeAdvisor } from '../model/advisor/requestToBeAdvisor';
import { RegisterAdvisorRequest } from '../model/advisor/registerAdvisorRequest';
import { AdviseRequest } from '../model/advisor/adviseRequest';
import { AdvisorRequest } from '../model/advisor/advisorRequest';
import { Advisor } from '../model/advisor/advisor';
import { HallOfFameResponse } from '../model/advisor/hallOfFameResponse';
import { LoginResult } from '../model/account/loginResult';
import { OrderResponse } from '../model/trade/orderResponse';
import { AdvisorPerformanceResponse } from '../model/advisor/advisorPerformanceResponse';

@Injectable()
export class AdvisorService {
  private advisorsUrl = this.httpService.apiUrl("v1/advisors");
  private loggedAdvisorUrl = this.httpService.apiUrl("v1/advisors/me");
  private advisorsDetailsUrl = this.httpService.apiUrl("v1/advisors/{id}/details");
  private advisorsPerformanceUrl = this.httpService.apiUrl("v1/advisors/{id}/performance");
  private advisorsOrdersUrl = this.httpService.apiUrl("v1/advisors/{id}/orders");
  private followAdvisorsUrl = this.httpService.apiUrl("v1/advisors/{id}/followers");
  private advisorMonthlyRanking = this.httpService.apiUrl("v1/advisors/ranking");
  private advisorHallOfFame = this.httpService.apiUrl("v1/advisors/hall_of_fame");

  constructor(private httpService : HttpService) { }

  getAdvisor(id: string): Observable<Advisor> {
    return this.httpService.get(this.advisorsUrl + "/" + id);
  }

  getAdvisorPerfomance(id: number): Observable<AdvisorPerformanceResponse> {
    return this.httpService.get(this.advisorsPerformanceUrl.replace("{id}", id.toString()));
  }

  editAdvisor(id: number, advisorRequest: AdvisorRequest): Observable<string> {
    let formData: FormData = new FormData();
    formData.append('formFile', advisorRequest.file);
    formData.append('name', advisorRequest.name);
    formData.append('description', advisorRequest.description);
    formData.append('changedPicture', advisorRequest.changedPicture + '');
    return this.httpService.postWithoutContentType(this.advisorsUrl + "/" + id, formData);
  }

  getExpertDetails(id: number): Observable<AdvisorResponse> {
    return this.httpService.get(this.advisorsDetailsUrl.replace("{id}", id.toString()));
  }

  getAdvisorOrders(userId:number, orderStatusTypes:number[] = [], assetId:number = null,  orderType:number = null): Observable<OrderResponse[]>{
    return this.httpService.get(this.buildAdvisorsOrdersUrl(userId, assetId, orderStatusTypes,orderType));
  }

  private buildAdvisorsOrdersUrl(userId:number, assetId:number, orderStatusTypes:number[], orderType:number){
    let result = this.advisorsOrdersUrl + "?";
    result = result.replace("{id}", userId.toString());

    if (assetId)
      result = result + "assetId=" + assetId.toString() + "&";
    if (orderStatusTypes)
    orderStatusTypes.forEach(e => result = result + "status=" + e.toString() + "&" )
    if (orderType)
      result = result + "type=" + orderType.toString() + "&";
    return result.substr(0, result.length-1);
  }


  getHallOfFame(): Observable<HallOfFameResponse[]> {
    return this.httpService.get(this.advisorHallOfFame);
  }

  getExpertsMonthlyRanking(year?: number, month?: number): Observable<AdvisorResponse[]> {
    let url = this.advisorMonthlyRanking;
    if (year) {
      url += "/" + year.toString();
    }
    if (month) {
      url += "/" + month.toString();
    }
    return this.httpService.get(url);
  }

  getLoggedExpertDetails(): Observable<AdvisorResponse> {
    return this.httpService.get(this.loggedAdvisorUrl);
  }

  getAdvisors(): Observable<AdvisorResponse[]> {
    return this.httpService.get(this.advisorsUrl);
  }

  postRegisterAdvisor(requestAdvisorRequest: RegisterAdvisorRequest): Observable<LoginResult> {
    let formData: FormData = new FormData();
    formData.append('formFile', requestAdvisorRequest.file);
    formData.append('name', requestAdvisorRequest.name);
    formData.append('description', requestAdvisorRequest.description);
    formData.append('changedPicture', requestAdvisorRequest.changedPicture + '');
    formData.append('captcha', requestAdvisorRequest.captcha);
    formData.append('email', requestAdvisorRequest.email);
    formData.append('password', requestAdvisorRequest.password);
    formData.append('referralCode', requestAdvisorRequest.referralCode);
    return this.httpService.postWithoutContentType(this.advisorsUrl, formData);
  }

  followAdvisor(advisorId:number):Observable<void>{
    return this.httpService.post(this.followAdvisorsUrl.replace("{id}", advisorId.toString()));
  }

  unfollowAdvisor(advisorId:number):Observable<void>{
    return this.httpService.delete(this.followAdvisorsUrl.replace("{id}", advisorId.toString()));
  }
}

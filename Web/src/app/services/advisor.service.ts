import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { AdvisorResponse } from "../model/advisor/advisorResponse";
import { RequestToBeAdvisor } from '../model/advisor/requestToBeAdvisor';
import { RegisterAdvisorRequest } from '../model/advisor/registerAdvisorRequest';
import { AdviseRequest } from '../model/advisor/adviseRequest';
import { AdvisorRequest } from '../model/advisor/advisorRequest';
import { Advisor } from '../model/advisor/advisor';
import { FeedResponse } from '../model/advisor/feedResponse';
import { LoginResult } from '../model/account/loginResult';

@Injectable()
export class AdvisorService {
  private advisorsUrl = this.httpService.apiUrl("v1/advisors");
  private advisorsDetailsUrl = this.httpService.apiUrl("v1/advisors/{id}/details");
  private requestToBeAdvisorsUrl = this.httpService.apiUrl("v1/advisors/me/requests");
  private listPendingRequestToBeAdvisorUrl = this.httpService.apiUrl("v1/advisors/requests");
  private approveRequestToBeAdvisorUrl = this.httpService.apiUrl("v1/advisors/requests/{id}/approve");
  private rejectRequestToBeAdvisorUrl = this.httpService.apiUrl("v1/advisors/requests/{id}/reject");
  private followAdvisorsUrl = this.httpService.apiUrl("v1/advisors/{id}/followers");
  private adviseUrl = this.httpService.apiUrl("v1/advisors/advices");
  private listLatestAdvicesForEachTypeUrl = this.httpService.apiUrl("v1/advisors/advices/latest_by_type");

  constructor(private httpService : HttpService) { }

  getAdvisor(id: string): Observable<Advisor> {
    return this.httpService.get(this.advisorsUrl + "/" + id);
  }

  editAdvisor(id: number, advisorRequest: AdvisorRequest): Observable<string> {
    let formData: FormData = new FormData();
    formData.append('formFile', advisorRequest.file);
    formData.append('name', advisorRequest.name);
    formData.append('description', advisorRequest.description);
    formData.append('changedPicture', advisorRequest.changedPicture + '');
    return this.httpService.postWithoutContentType(this.advisorsUrl + "/" + id, formData);
  }

  getExpertDetails(id: string): Observable<AdvisorResponse> {
    return this.httpService.get(this.advisorsDetailsUrl.replace("{id}", id));
  }

  getAdvisors(): Observable<AdvisorResponse[]> {
    return this.httpService.get(this.advisorsUrl);
  }

  getRequestToBeAdvisor(): Observable<RequestToBeAdvisor> {
    return this.httpService.get(this.requestToBeAdvisorsUrl);
  }

  listPendingRequestToBeAdvisor(): Observable<RequestToBeAdvisor[]> {
    return this.httpService.get(this.listPendingRequestToBeAdvisorUrl);
  }

  approveAdvisor(requestId:number):Observable<void>{
    return this.httpService.post(this.approveRequestToBeAdvisorUrl.replace("{id}", requestId.toString()));
  }

  rejectAdvisor(requestId:number):Observable<void>{
    return this.httpService.post(this.rejectRequestToBeAdvisorUrl.replace("{id}", requestId.toString()));
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

  advise(adviseRequest:AdviseRequest):Observable<void>{
    return this.httpService.post(this.adviseUrl, adviseRequest);
  }

  listLatestAdvicesForEachType(numberOfAdvicesOfEachType:number):Observable<FeedResponse>{
    return this.httpService.get(this.listLatestAdvicesForEachTypeUrl + "?numberOfAdvicesOfEachType=" + numberOfAdvicesOfEachType);
  }

}

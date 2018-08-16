import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { AdvisorResponse } from "../model/advisor/advisorResponse";
import { RequestToBeAdvisor } from '../model/advisor/requestToBeAdvisor';
import { RequestToBeAdvisorRequest } from '../model/advisor/requestToBeAdvisorRequest';
import { AdviseRequest } from '../model/advisor/adviseRequest';

@Injectable()
export class AdvisorService {
  private baseGetAdvisorsUrl = this.httpService.apiUrl("v1/advisors");
  private requestToBeAdvisorsUrl = this.httpService.apiUrl("v1/advisors/me/requests");
  private followAdvisorsUrl = this.httpService.apiUrl("v1/advisors/{id}/followers");
  private adviseUrl = this.httpService.apiUrl("v1/advisors/advices");

  constructor(private httpService : HttpService) { }

  getAdvisor(id: string): Observable<AdvisorResponse> {
    return this.httpService.get(this.baseGetAdvisorsUrl + "/" + id);
  }

  getAdvisors(): Observable<AdvisorResponse[]> {
    return this.httpService.get(this.baseGetAdvisorsUrl);
  }

  getRequestToBeAdvisor(): Observable<RequestToBeAdvisor> {
    return this.httpService.get(this.requestToBeAdvisorsUrl);
  }

  postRequestToBeAdvisor(requestToBeAdvisorRequest: RequestToBeAdvisorRequest): Observable<RequestToBeAdvisor> {
    return this.httpService.post(this.requestToBeAdvisorsUrl, requestToBeAdvisorRequest);
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
}
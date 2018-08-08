import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { Advisor } from "../model/advisor/advisor";
import { RequestToBeAdvisor } from '../model/advisor/requestToBeAdvisor';
import { RequestToBeAdvisorRequest } from '../model/advisor/requestToBeAdvisorRequest';

@Injectable()
export class AdvisorService {
  private baseGetAdvisorsUrl = this.httpService.apiUrl("v1/advisors");
  private requestToBeAdvisorsUrl = this.httpService.apiUrl("v1/advisors/me/requests");
  
  constructor(private httpService : HttpService) { }

  getAdvisor(id: string): Observable<Advisor> {
    return this.httpService.get(this.baseGetAdvisorsUrl + "/" + id);
  }

  getAdvisors(): Observable<Advisor[]> {
    return this.httpService.get(this.baseGetAdvisorsUrl);
  }

  getRequestToBeAdvisor(): Observable<RequestToBeAdvisor> {
    return this.httpService.get(this.requestToBeAdvisorsUrl);
  }

  postRequestToBeAdvisor(requestToBeAdvisorRequest: RequestToBeAdvisorRequest): Observable<RequestToBeAdvisor> {
    return this.httpService.post(this.requestToBeAdvisorsUrl, requestToBeAdvisorRequest);
  }
}
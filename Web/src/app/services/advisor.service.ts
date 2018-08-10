import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { AdvisorResponse } from "../model/advisor/advisorResponse";
import { RequestToBeAdvisor } from '../model/advisor/requestToBeAdvisor';
import { RequestToBeAdvisorRequest } from '../model/advisor/requestToBeAdvisorRequest';

@Injectable()
export class AdvisorService {
  private baseGetAdvisorsUrl = this.httpService.apiUrl("v1/advisors");
  private requestToBeAdvisorsUrl = this.httpService.apiUrl("v1/advisors/me/requests");
  
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
}
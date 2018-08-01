import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { Advisor } from "../model/advisor/advisor";

@Injectable()
export class AdvisorService {
  private baseGetAdvisorsUrl = this.httpService.apiUrl("advisors/v1");
  
  constructor(private httpService : HttpService) { }

  getAdvisor(id: string): Observable<Advisor> {
    return this.httpService.get(this.baseGetAdvisorsUrl + "/" + id);
  }

  getAdvisors(): Observable<Advisor[]> {
    return this.httpService.get(this.baseGetAdvisorsUrl);
  }
}
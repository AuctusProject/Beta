import { Component, OnInit } from '@angular/core';
import { RequestToBeAdvisor } from '../../../model/advisor/requestToBeAdvisor';
import { AdvisorService } from '../../../services/advisor.service';
import { RequestToBeAdvisorRequest } from '../../../model/advisor/requestToBeAdvisorRequest';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';
import { Router } from '../../../../../node_modules/@angular/router';

@Component({
  selector: 'become-advisor',
  templateUrl: './become-advisor.component.html',
  styleUrls: ['./become-advisor.component.css']
})
export class BecomeAdvisorComponent implements OnInit {
  currentRequestToBeAdvisor: RequestToBeAdvisor;
  requestToBeAdvisorRequest: RequestToBeAdvisorRequest = new RequestToBeAdvisorRequest();
  constructor(private advisorService: AdvisorService, 
    private notificationsService: NotificationsService,
    private router: Router
  ) { }

  ngOnInit() {
    this.advisorService.getRequestToBeAdvisor().subscribe(result => 
      {
        this.currentRequestToBeAdvisor = result;
        this.fillRequestToBeAdvisor();
      });
  }

  private fillRequestToBeAdvisor(){
    if(this.currentRequestToBeAdvisor != null){
      this.requestToBeAdvisorRequest.name = this.currentRequestToBeAdvisor.name;
      this.requestToBeAdvisorRequest.description = this.currentRequestToBeAdvisor.description;
      this.requestToBeAdvisorRequest.previousExperience = this.currentRequestToBeAdvisor.previousExperience;
    }
  }

  sendRequest(){
    this.advisorService.postRequestToBeAdvisor(this.requestToBeAdvisorRequest).subscribe(
      result => this.notificationsService.success(null, "Request was successfully sent.")
    );
  }

  walletLogin(){
    this.router.navigateByUrl('wallet-login');
  }
}

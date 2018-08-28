import { Component, OnInit } from '@angular/core';
import { AdvisorService } from '../../../services/advisor.service';
import { RequestToBeAdvisor } from '../../../model/advisor/requestToBeAdvisor';
import { MatProgressBar, MatSpinner } from '../../../../../node_modules/@angular/material';
import { Subscription } from '../../../../../node_modules/rxjs';

@Component({
  selector: 'advisors-requests',
  templateUrl: './advisors-requests.component.html',
  styleUrls: ['./advisors-requests.component.css']
})
export class AdvisorsRequestsComponent implements OnInit {
  requests : RequestToBeAdvisor[];
  matProgressBar: MatProgressBar;
  matSpinner : MatSpinner;
  promise:Subscription; 
  
  constructor(private advisorService: AdvisorService) 
  { 
  }

  ngOnInit() {
    this.refreshList();
  }

  refreshList(){
    this.promise = this.advisorService.listPendingRequestToBeAdvisor().subscribe(result => 
      this.requests = result
    );
  }

  approve(id:number){
    this.promise = this.advisorService.approveAdvisor(id).subscribe(result => 
      {
        this.refreshList();
      }
    );
  }

  reject(id:number){
    this.promise = this.advisorService.rejectAdvisor(id).subscribe(result => 
      {
        this.refreshList();
      }
    );
  }
}

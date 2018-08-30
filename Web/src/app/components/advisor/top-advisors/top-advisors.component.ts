import { Component, OnInit } from '@angular/core';
import { AdvisorService } from '../../../services/advisor.service';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { NavigationService } from '../../../services/navigation.service';

@Component({
  selector: 'top-advisors',
  templateUrl: './top-advisors.component.html',
  styleUrls: ['./top-advisors.component.css']
})
export class TopAdvisorsComponent implements OnInit {
  experts : AdvisorResponse[] = [];

  constructor(private advisorService: AdvisorService, private navigationService: NavigationService) { }

  ngOnInit() {
    this.advisorService.getAdvisors().subscribe(result => {
      if(result!= null && result.length > 4){
        this.experts = result.slice(0,4);
      }
      else{
        this.experts = result;
      }
    });
  }

  goToListAdvisors(){
    this.navigationService.goToListAdvisors();
  }
}

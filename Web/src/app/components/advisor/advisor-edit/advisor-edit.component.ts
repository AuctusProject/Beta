import { Component, OnInit } from '@angular/core';
import { AdvisorService } from '../../../services/advisor.service';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { AdvisorRequest } from '../../../model/advisor/advisorRequest';
import { Subscription } from '../../../../../node_modules/rxjs';
import { AccountService } from '../../../services/account.service';
import { NavigationService } from '../../../services/navigation.service';
import { Advisor } from '../../../model/advisor/advisor';

@Component({
  selector: 'advisor-edit',
  templateUrl: './advisor-edit.component.html',
  styleUrls: ['./advisor-edit.component.css']
})
export class AdvisorEditComponent implements OnInit {
  advisor: Advisor;
  promise: Subscription;
  constructor(private route: ActivatedRoute, private advisorService: AdvisorService, private accountService: AccountService, private navigationService: NavigationService) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
        if(this.accountService.getLoginData().isAdvisor){
          if(this.accountService.getLoginData().id == params['id']){
            this.advisorService.getAdvisor(params['id']).subscribe(advisor => this.advisor = advisor)
          }
          else{
            this.navigationService.goToAdvisorDetail(params['id']);
          }
        }
        else{
          this.navigationService.goToFeed();
        }
      }
    );
  }

  save(){
    var request = new AdvisorRequest();
    request.name = this.advisor.name;
    request.description = this.advisor.description;
    this.promise = this.advisorService.editAdvisor(this.advisor.id, request).subscribe(result => {});
  }

}

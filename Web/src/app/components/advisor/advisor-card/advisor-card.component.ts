import { Component, OnInit, Input } from '@angular/core';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { AdvisorService } from '../../../services/advisor.service';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'advisor-card',
  templateUrl: './advisor-card.component.html',
  styleUrls: ['./advisor-card.component.css']
})
export class AdvisorCardComponent implements OnInit {
  @Input() advisor: AdvisorResponse;
  
  constructor(private advisorServices:AdvisorService) { }

  ngOnInit() {
  }

  onFollowClick(){
    this.advisorServices.followAdvisor(this.advisor.userId).subscribe(result =>this.advisor.following = true);
  }
  onUnfollowClick(){
    this.advisorServices.unfollowAdvisor(this.advisor.userId).subscribe(result =>this.advisor.following = false);
  }

  getAdvisorImgUrl(){
    return environment.profileImgUrl.replace("{id}", this.advisor.userId.toString());
  }
}

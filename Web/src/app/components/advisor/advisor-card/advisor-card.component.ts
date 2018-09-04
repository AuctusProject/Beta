import { Component, OnInit, Input } from '@angular/core';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { AdvisorService } from '../../../services/advisor.service';
import { CONFIG} from "../../../services/config.service";
import { Util } from '../../../util/Util';

@Component({
  selector: 'advisor-card',
  templateUrl: './advisor-card.component.html',
  styleUrls: ['./advisor-card.component.css']
})
export class AdvisorCardComponent implements OnInit {
  @Input() advisor: AdvisorResponse;
  @Input() advisorsLength: number;
  
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
    return CONFIG.profileImgUrl.replace("{id}", this.advisor.urlGuid);
  }

  getTotalRecommendations(){
    var total = 0;
    if (!!this.advisor.recommendationDistribution) {
      for(var i =0; i < this.advisor.recommendationDistribution.length; i++){
        total += this.advisor.recommendationDistribution[i].total;
      }
    }
    return total;
  }

  getRecommendationPercentage(type: number){
    if (!!this.advisor.recommendationDistribution) {
      for(var i =0; i < this.advisor.recommendationDistribution.length; i++){
        if(this.advisor.recommendationDistribution[i].type == type)
          return this.advisor.recommendationDistribution[i].total/this.getTotalRecommendations() * 100;
      }
    }
    return 0;
  }

  getBuyRecommendationPercentage(){
    return this.getRecommendationPercentage(Util.BUY);
  }

  getCloseRecommendationPercentage(){
    return this.getRecommendationPercentage(Util.CLOSE);
  }
    
  getSellRecommendationPercentage(){
    return this.getRecommendationPercentage(Util.SELL);    
  }
}

import { Component, OnInit } from '@angular/core';
import { FeedResponse } from '../../../model/advisor/feedResponse';
import { AdvisorService } from '../../../services/advisor.service';
import { NavigationService } from '../../../services/navigation.service';

@Component({
  selector: 'recommendation-box-list',
  templateUrl: './recommendation-box-list.component.html',
  styleUrls: ['./recommendation-box-list.component.css']
})
export class RecommendationBoxListComponent implements OnInit {

  advices : FeedResponse[] = [];
  buyAdvices : FeedResponse[] = [];
  sellAdvices : FeedResponse[] = [];
  closePositionAdvices : FeedResponse[] = [];

  constructor(private advisorServices:AdvisorService, private navigationService: NavigationService) { }

  ngOnInit() {
    this.appendRecommendations();
  }

  appendRecommendations(){
    this.advisorServices.listLatestAdvicesForEachType(3).subscribe(result => 
      {
        this.advices = this.advices.concat(result);
        this.sellAdvices = this.advices.filter(advice => advice.adviceType == 0);
        this.buyAdvices = this.advices.filter(advice => advice.adviceType == 1);
        this.closePositionAdvices = this.advices.filter(advice => advice.adviceType == 2);
      }
    );
  }

  goToTopAssets(){
    this.navigationService.goToTopAssets();
  }

}

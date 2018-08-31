import { Component, OnInit } from '@angular/core';
import { FeedResponse } from '../../../model/advisor/feedResponse';
import { AdvisorService } from '../../../services/advisor.service';

@Component({
  selector: 'recommendation-box',
  templateUrl: './recommendation-box.component.html',
  styleUrls: ['./recommendation-box.component.css']
})
export class RecommendationBoxComponent implements OnInit {
  advices : FeedResponse[] = [];

  constructor(private advisorServices:AdvisorService) { }

  ngOnInit() {
    this.appendRecommendations();
  }

  appendRecommendations(){
    this.advisorServices.listLatestAdvicesForEachType(3).subscribe(result => 
      this.advices = this.advices.concat(result));
  }

}

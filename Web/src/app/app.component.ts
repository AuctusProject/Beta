import { Component, OnInit, OnChanges } from '@angular/core';
import { RecommendationDistributionResponse } from './model/recommendationDistributionResponse';
import { AssetValue } from './model/asset/AssetValue';
import { AdvisorResponse } from './model/advisor/advisorResponse';
import { Router, ActivatedRoute } from '../../node_modules/@angular/router';
import { AuthRedirect } from './providers/authRedirect';
import { Route } from '../../node_modules/@angular/compiler/src/core';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{

  constructor(){}

  ngOnInit(){
  }

  public notificationOptions = {
    position: ["bottom", "left"],
    maxStack: 1,
    preventDuplicates: true,
    preventLastDuplicates: "visible",
    clickToClose: true
  }

  title = 'app';

  data: RecommendationDistributionResponse[] = [
    new RecommendationDistributionResponse(1, 30),
    new RecommendationDistributionResponse(0, 15),
    new RecommendationDistributionResponse(2, 65)
  ];

  assetValues: AssetValue[]= [{
      date: new Date(),
      value:107
    }
  ];

  advisor : AdvisorResponse = {
    userId: 1,
    description: "Description",
    name: "Name",
    averageReturn: 23.5,
    following: false,
    owner: false,
    creationDate: new Date(2018, 7, 1),
    numberOfFollowers: 121,
    totalAssetsAdvised: 5,
    ranking: 2,
    rating: 3.9,
    successRate: 75,
    recommendationDistribution:[
      new RecommendationDistributionResponse(1, 30),
      new RecommendationDistributionResponse(0, 15),
      new RecommendationDistributionResponse(2, 65)
    ]
  }
}

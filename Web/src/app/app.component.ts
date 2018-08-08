import { Component, OnInit, OnChanges } from '@angular/core';
import { RecommendationDistribution } from './model/recommendationDistribution';
import { AssetValue } from './model/asset/AssetValue';
import { Advisor } from './model/advisor/advisor';
import { AdvisorDetails } from './model/advisor/advisorDetails';
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

  data: RecommendationDistribution[] = [
    new RecommendationDistribution(1, 30),
    new RecommendationDistribution(0, 15),
    new RecommendationDistribution(2, 65)
  ];

  assetValues: AssetValue[]= [{
      date: new Date(),
      value:107
    }
  ];

  advisor : Advisor = {
    id: 1,
    description: "Description",
    name: "Name",
    urlPhoto:"https://cdn.tipranks.com/expert-pictures/485_EJBGDHCAD_tsqr.jpg",
    advisorDetails: {
      averageReturn: 23.5,
      following: false,
      numberOfFollowers: 121,
      ranking: 2,
      rating: 3.9,
      successRate: 75,
      recommendationDistribution:[
        new RecommendationDistribution(1, 30),
        new RecommendationDistribution(0, 15),
        new RecommendationDistribution(2, 65)
      ]
    }
  }
}

import { Component, OnInit, OnChanges } from '@angular/core';
import { RecommendationDistributionResponse } from './model/recommendationDistributionResponse';
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
}

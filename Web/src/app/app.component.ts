import { Component } from '@angular/core';
import { RecommendationDistribution } from './model/recommendationDistribution';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'app';

  data: RecommendationDistribution[] = [{
    type: "BUY",
    total: 30
  },{
    type: "SELL",
    total: 15
  },{
    type: "HOLD",
    total: 75
  }
]

}

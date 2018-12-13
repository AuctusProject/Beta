import { Component, OnInit, Input } from '@angular/core';
import { PositionResponse, AdvisorResponse } from '../../../../../model/advisor/advisorResponse';
import { ValueDisplayPipe } from '../../../../../util/value-display.pipe';
import { AdvisorPerformanceResponse } from '../../../../../model/advisor/advisorPerformanceResponse';

@Component({
  selector: 'trade-summary',
  templateUrl: './trade-summary.component.html',
  styleUrls: ['./trade-summary.component.css']
})
export class TradeSummaryComponent implements OnInit {
  @Input() advisor: AdvisorResponse;
  @Input() closedPosition: PositionResponse;
  @Input() openPosition: PositionResponse;
  advisorPerformance: AdvisorPerformanceResponse;
  
  constructor() { }

  ngOnInit() {
  }

  setAdvisorPerformance(advisorPerformance: AdvisorPerformanceResponse) {
    this.advisorPerformance = advisorPerformance;
  }

  getAvgTradeMinutes(){
    var ret = new Date()
    ret.setMinutes(ret.getMinutes() - this.closedPosition.averageTradeMinutes);
    return ret;
  }
}

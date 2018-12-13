import { Component, OnInit, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { AdvisorResponse, PositionResponse, AssetPositionResponse } from '../../../../model/advisor/advisorResponse';
import { AdvisorPerformanceResponse, DailyPerformanceResponse } from '../../../../model/advisor/advisorPerformanceResponse';
import { AdvisorService } from '../../../../services/advisor.service';
import { PerformanceClosedPositionsComponent } from './perfomance-closed-positions/perfomance-closed-positions.component';
import { TradeSummaryComponent } from './trade-summary/trade-summary.component';
import { PerfomanceOpenPositionsComponent } from './perfomance-open-positions/perfomance-open-positions.component';

@Component({
  selector: 'performance',
  templateUrl: './performance.component.html',
  styleUrls: ['./performance.component.css']
})
export class PerformanceComponent implements OnChanges {
  @Input() advisor: AdvisorResponse;
  @Input() closedPosition: PositionResponse;
  @Input() openPosition: PositionResponse;
  
  advisorPerformance: AdvisorPerformanceResponse;

  @ViewChild("Closed") Closed: PerformanceClosedPositionsComponent;
  @ViewChild("Open") Open: PerfomanceOpenPositionsComponent;
  @ViewChild("Trade") Trade: TradeSummaryComponent;

  constructor(private advisorService: AdvisorService) { }

  ngOnChanges(changes: SimpleChanges) {
    if ((changes.advisor && changes.advisor.currentValue && (!changes.advisor.previousValue || 
        changes.advisor.previousValue.userId != changes.advisor.currentValue.userId)) ||
        (changes.closedPosition && (!changes.closedPosition.previousValue || !changes.closedPosition.currentValue || 
        changes.closedPosition.previousValue.orderCount != changes.closedPosition.currentValue.orderCount))) {
      
      if (changes.advisor && changes.advisor.currentValue) {
        this.refreshData(changes.advisor.currentValue.userId);
      } else if (this.advisor) {
        this.refreshData(this.advisor.userId);
      }
    }
  }

  setAllClosedPosition(allClosedPosition: AssetPositionResponse[]) {
    this.Closed.setClosedPositions(allClosedPosition);
  }

  setAllOpenPosition(allOpenPostion: AssetPositionResponse[]) {
    this.Open.setOpenPositions(allOpenPostion);
  }

  refreshData(userId: number) {
    if (userId) {  
      this.advisorService.getAdvisorPerfomance(userId).subscribe(ret =>
      {
        if (this.advisor) {
          let currentValue = new DailyPerformanceResponse();
          currentValue.date = new Date();
          currentValue.equity = this.advisor.equity;
          if (ret.dailyPerformance.length > 0) {
            currentValue.variation = currentValue.equity / ret.dailyPerformance[ret.dailyPerformance.length - 1].equity - 1;
          } else {
            currentValue.variation = 0;
          }
          ret.dailyPerformance.push(currentValue);
        }
        this.advisorPerformance = ret;
        this.Trade.setAdvisorPerformance(this.advisorPerformance);
      });
    }
  }
}

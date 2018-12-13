import { Component, OnInit, OnDestroy } from '@angular/core';
import { AdvisorService } from '../../../services/advisor.service';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { ExpertsTableType } from '../experts-table/experts-table.component';

@Component({
  selector: 'trading-contest',
  templateUrl: './trading-contest.component.html',
  styleUrls: ['./trading-contest.component.css']
})
export class TradingContestComponent implements OnInit {
  monthlyContestType = ExpertsTableType.monthlyContest;
  experts:AdvisorResponse[];
  constructor(private advisorService: AdvisorService) { }

  ngOnInit() {
    this.advisorService.getExpertsMonthlyRanking().subscribe(result => {
      this.experts = result;
    });
  }
}

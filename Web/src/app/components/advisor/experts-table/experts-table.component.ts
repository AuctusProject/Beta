import { Component, OnInit, Input, OnDestroy, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { FollowUnfollowType } from '../../util/follow-unfollow/follow-unfollow.component';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { AdvisorDataService } from '../../../services/advisor-data.service';
import { MatSort, MatTableDataSource, MatPaginator } from '@angular/material';
import { Util } from '../../../util/util';

export enum ExpertsTableType {
  default=0,
  monthlyContest=1
}

@Component({
  selector: 'experts-table',
  templateUrl: './experts-table.component.html',
  styleUrls: ['./experts-table.component.css'],
  providers: [AdvisorDataService]
})
export class ExpertsTableComponent implements OnInit, OnChanges, OnDestroy {
  expertFollowUnfollowType = FollowUnfollowType.expert;
  @Input() showViewMore : boolean;
  @Input() experts: AdvisorResponse[];
  @Input() type: ExpertsTableType;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  
  dataSource= new MatTableDataSource<AdvisorResponse>(this.experts);
  
  constructor(private advisorDataService:AdvisorDataService) { }

  ngOnInit() {
    this.setDataSource();
  }

  setDataSource(){
    this.dataSource.data = this.experts;
    if(this.dataSource.data != null && this.dataSource.data.length > 0){
      if(!this.dataSource.sort){
        this.dataSource.sortingDataAccessor = Util.CustomSortingData;
        this.dataSource.sort = this.sort;
      }
      if(!this.dataSource.paginator){
        this.dataSource.paginator = this.paginator;
      }
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if((changes.experts.previousValue == null || changes.experts.previousValue.length == 0) && (changes.experts.currentValue != null && changes.experts.currentValue.length != 0)){
      this.advisorDataService.initializeWithListResponse(this.experts);
      this.advisorDataService.advisorListResponse().subscribe(result => {
        this.experts = result;
        this.setDataSource();
      });
    }
  }

  ngOnDestroy(){
    this.advisorDataService.destroy();
  }

  getNumberOfTradesDescription(expert: AdvisorResponse){
    var text=expert.totalTrades + " trade";
    if(expert.totalTrades != 1)
      text+="s";
    return "("+text+")";
  }

  getTotalExperts(){
    if(this.experts)
      return this.experts.length;
    else
      return 0;
  }

  isMonthlyContest(){
    return this.type == ExpertsTableType.monthlyContest;
  }

  getDisplayedColumns(){
    if(this.isMonthlyContest()){
      return [
        "badge",
        "trader",
        "monthlyPL",
        "follow"
      ];
    }
    else{
      return [
        "trader",
        "ranking",
        "successRate",
        "averageReturn",
        "PL",
        "follow"
      ];
    }
  }

  getDefaultSorting(){
    if(this.isMonthlyContest()){
      return "monthlyRankingHistory.ranking";
    }
    else{
      return "ranking";
    }
  }

  getRowClass(expert: AdvisorResponse){
    if(this.type == ExpertsTableType.monthlyContest && expert.monthlyRankingHistory && 
      expert.monthlyRankingHistory.ranking >= 1 && expert.monthlyRankingHistory.ranking <= 3){
        return "highlight";
    }
    return "";
  }
}

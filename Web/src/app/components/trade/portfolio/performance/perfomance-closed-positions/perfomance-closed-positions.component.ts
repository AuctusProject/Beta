import { Component, OnInit, Input, SimpleChanges, ViewChild } from '@angular/core';
import { MatTableDataSource, MatSort } from '@angular/material';
import { AssetPositionResponse } from '../../../../../model/advisor/advisorResponse';
import { Util } from '../../../../../util/util';

@Component({
  selector: 'perfomance-closed-positions',
  templateUrl: './perfomance-closed-positions.component.html',
  styleUrls: ['./perfomance-closed-positions.component.css']
})
export class PerformanceClosedPositionsComponent implements OnInit {

  @ViewChild(MatSort) sort: MatSort;
  dataSource= new MatTableDataSource<AssetPositionResponse>();

  totalTrades: number = 0;

  displayedColumns: string[] = [
    "market",
    "trades",
    "return",
    "tradetime",
    "success",
    "chart"
  ];

  constructor() { }

  ngOnInit() {
  }

  setClosedPositions(closedPosition: AssetPositionResponse[]) {
    this.dataSource.data = closedPosition;
    if (!this.dataSource.sort) {
      this.dataSource.sortingDataAccessor = Util.CustomSortingData;
      this.dataSource.sort = this.sort;
    }
    this.totalTrades = 0;
    for (let i = 0; i < this.dataSource.data.length; ++i) {
      this.totalTrades += this.dataSource.data[i].positionResponse.orderCount;
    }
  }

  getOrderPercentage(count: number) : number {
    return count / this.totalTrades; 
  }

  getAvgTradeMinutes(position: AssetPositionResponse) {
    var ret = new Date()
    ret.setMinutes(ret.getMinutes() - position.positionResponse.averageTradeMinutes);
    return ret;
  }
}

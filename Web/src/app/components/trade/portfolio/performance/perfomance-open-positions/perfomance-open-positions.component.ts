import { Component, OnInit, Input, SimpleChanges, ViewChild } from '@angular/core';
import { MatTableDataSource, MatSort } from '@angular/material';
import { AssetPositionResponse } from '../../../../../model/advisor/advisorResponse';
import { Util } from '../../../../../util/util';
import { Constants } from "../../../../../util/constants";

@Component({
  selector: 'perfomance-open-positions',
  templateUrl: './perfomance-open-positions.component.html',
  styleUrls: ['./perfomance-open-positions.component.css']
})
export class PerfomanceOpenPositionsComponent implements OnInit {
  @ViewChild(MatSort) sort: MatSort;

  displayedColumns: string[] = [
    "asset",
    "units",    
    "value",
    "avgOpen",
    "PL"
  ];
  utilProxy = Util;
  constantsProxy = Constants;
  totalTrades: number = 0;
  constructor() { }

  ngOnInit() {
  }
  dataSource= new MatTableDataSource<AssetPositionResponse>();
  setOpenPositions(openPosition: AssetPositionResponse[]) {

    this.dataSource.data = openPosition;
    if (!this.dataSource.sort) {
      this.dataSource.sortingDataAccessor = Util.CustomSortingData;
      this.dataSource.sort = this.sort;
    }
    this.totalTrades = 0;
    for (let i = 0; i < this.dataSource.data.length; ++i) {
      this.totalTrades += this.dataSource.data[i].positionResponse.orderCount;
    }
  }

}

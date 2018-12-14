import { Component, OnInit, Input, SimpleChanges, ViewChild } from '@angular/core';
import { MatTableDataSource, MatSort } from '@angular/material';
import { AssetPositionResponse } from '../../../../../model/advisor/advisorResponse';
import { Util } from '../../../../../util/util';
import { Constants } from "../../../../../util/constants";
import { NavigationService } from '../../../../../services/navigation.service';

export enum OpenPositionsType {
  performance=0,
  miniView=1
}

@Component({
  selector: 'perfomance-open-positions',
  templateUrl: './perfomance-open-positions.component.html',
  styleUrls: ['./perfomance-open-positions.component.css']
})
export class PerfomanceOpenPositionsComponent implements OnInit {
  @Input() type: OpenPositionsType;

  @ViewChild(MatSort) sort: MatSort;

  utilProxy = Util;
  constantsProxy = Constants;
  totalTrades: number = 0;
  constructor(private navigationService: NavigationService) { }

  ngOnInit() {
  }

  dataSource= new MatTableDataSource<AssetPositionResponse>();
  setOpenPositions(openPosition: AssetPositionResponse[]) {
    let forceRefresh = !this.dataSource || !this.dataSource.data || this.dataSource.data.length != openPosition.length;
    if (forceRefresh || (this.dataSource.data.length == 1 && this.dataSource.data[0].assetId != openPosition[0].assetId)) {
      this.setDataSourceElement(openPosition);
    }
    else{
      for (let i = 0; i < openPosition.length; ++i) {
        for (let j = 0; j < this.dataSource.data.length; ++j) {
          if (openPosition[i].assetId == this.dataSource.data[j].assetId) {
            this.dataSource.data[j].positionResponse.averageReturn = openPosition[i].positionResponse.averageReturn;
            this.dataSource.data[j].positionResponse.totalProfit = openPosition[i].positionResponse.totalProfit;
            this.dataSource.data[j].positionResponse.totalVirtual = openPosition[i].positionResponse.totalVirtual;
            if (this.dataSource.data[j].positionResponse.averagePrice != openPosition[i].positionResponse.averagePrice ||
              this.dataSource.data[j].positionResponse.totalQuantity != openPosition[i].positionResponse.totalQuantity ||
              this.dataSource.data[j].positionResponse.type != openPosition[i].positionResponse.type) {
              this.dataSource.data[j].positionResponse.totalQuantity = openPosition[i].positionResponse.totalQuantity;
              this.dataSource.data[j].positionResponse.averagePrice = openPosition[i].positionResponse.averagePrice;
              this.dataSource.data[j].positionResponse.type = openPosition[i].positionResponse.type;
            }
            break;
          }
        }
      }
    }
    this.totalTrades = 0;
    for (let i = 0; i < this.dataSource.data.length; ++i) {
      this.totalTrades += this.dataSource.data[i].positionResponse.orderCount;
    }
  }

  setDataSourceElement(positionResponse: AssetPositionResponse[]) {
    this.dataSource.data = positionResponse;
    if(!this.dataSource.sort){
      this.dataSource.sortingDataAccessor = Util.CustomSortingData;
      this.dataSource.sort = this.sort;
    }
  }

  getDisplayedColumns(){
    if(this.isMiniView()){
      return [
        "assetCode",
        "units",
        "value",
        "PL"
      ];
    }
    else{
      return [
        "asset",
        "units",    
        "value",
        "avgOpen",
        "PL"
      ];
    }
  }

  onRowClick(position: AssetPositionResponse){
    if(this.isMiniView()){
      this.navigationService.goToAssetDetails(position.assetId);
    }
  }

  isMiniView(){
    return this.type == OpenPositionsType.miniView;
  }

}

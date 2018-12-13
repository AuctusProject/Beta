import { Component, OnInit, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { NotificationsService } from 'angular2-notifications';
import { OrderResponse } from "../../../../model/trade/orderResponse";
import { CONFIG } from "../../../../services/config.service";
import { Util } from "../../../../util/util";
import { Constants } from "../../../../util/constants";
import { AdvisorService } from "../../../../services/advisor.service";
import { AdvisorResponse, PositionResponse } from '../../../../model/advisor/advisorResponse';
import { Moment } from 'moment';
import { MomentModule } from 'ngx-moment';
import { MatSort, MatTableDataSource } from '@angular/material';
import { NavigationService } from '../../../../services/navigation.service';

@Component({
  selector: 'history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.css']
})
export class HistoryComponent implements OnInit {
  displayedColumns: string[] = [
    "asset",
    "units",
    "open",
    "openTime",
    "close",
    "closeTime",
    "PLValue",
    "PLPercentage"
  ];
  
  @ViewChild(MatSort) sort: MatSort;
  dataSource = new MatTableDataSource<OrderResponse>();
  
  utilProxy = Util;
  bestTrade: number = 0;
  worstTrade: number = 0;
  
  @Input() userId: number;
  positionResponse : PositionResponse;
  constructor(public advisorService: AdvisorService, 
    public notificationsService: NotificationsService,
    public navigationService: NavigationService) { }

  ngOnInit() {
    this.refresh();
  }
  
  setPosition(positionResponse: PositionResponse) {
    this.positionResponse = positionResponse;
  }

  refresh() {
    this.advisorService.getAdvisorOrders(this.userId, [Constants.OrderStatus.Close]).subscribe(result => {
      this.dataSource.data = result;
      if (!this.dataSource.sort) {
        this.dataSource.sort = this.sort;
      }
      this.setBestWorstTrade();
    });
  }

  onAssetClick(assetId: number) {
    this.navigationService.goToAssetDetails(assetId);
  }

  getAssetImgUrl(id: number) {
    return CONFIG.assetImgUrl.replace("{id}", id.toString());
  }

  getAvgTradeMinutes(){
    var ret = new Date()
    ret.setMinutes( ret.getMinutes() - this.positionResponse.averageTradeMinutes);
    return ret;
  }

  getOrderDescription(order: OrderResponse){
    return Util.GetOrderTypeText(!order.type ? Constants.OrderType.Buy : Constants.OrderType.Sell).toUpperCase()
  }

  private setBestWorstTrade(){
    let best = -10000000;
    let worst = 10000000;
    this.dataSource.data.forEach(o => 
      {
        if (o.profit > best) {
          best = o.profit;
        }
        if (o.profit < worst) {
          worst = o.profit;
        }
      });
    if (best != -10000000) {
      this.bestTrade = best;
    } else {
      this.bestTrade = 0;
    }
    if (worst != 10000000) {
      this.worstTrade = worst;
    } else {
      this.worstTrade = 0;
    }
  }
}

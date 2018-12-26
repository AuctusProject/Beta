import { Component, OnInit, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { NotificationsService } from 'angular2-notifications';
import { OrderResponse } from "../../../../model/trade/orderResponse";
import { CONFIG } from "../../../../services/config.service";
import { Util } from "../../../../util/util";
import { Constants } from "../../../../util/constants";
import { AdvisorService } from "../../../../services/advisor.service";
import { AdvisorResponse, PositionResponse, AssetPositionResponse } from '../../../../model/advisor/advisorResponse';
import { Moment } from 'moment';
import { MomentModule } from 'ngx-moment';
import { MatSort, MatTableDataSource } from '@angular/material';
import { NavigationService } from '../../../../services/navigation.service';
import { PercentageDisplayPipe } from '../../../../util/percentage-display.pipe';
import { ValueDisplayPipe } from '../../../../util/value-display.pipe';

@Component({
  selector: 'history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.css']
})
export class HistoryComponent implements OnInit, OnChanges {
  displayedColumns: string[] = [
    "asset",
    "units",
    "open",
    "openTime",
    "close",
    "closeTime",
    "PL",
    "fee"
  ];
  
  @ViewChild(MatSort) sort: MatSort;
  dataSource = new MatTableDataSource<OrderResponse>();
  
  utilProxy = Util;
  bestTrade: number = 0;
  worstTrade: number = 0;
  
  @Input() userId: number;
  @Input() assetId?: number = null;

  filteredAssetId: number;
  positionResponse: PositionResponse;
  
  allPositionsResponse: PositionResponse;
  assetPositionsResponse: AssetPositionResponse[];
  orders: OrderResponse[];

  constructor(public advisorService: AdvisorService, 
    public notificationsService: NotificationsService,
    public navigationService: NavigationService) { }

  ngOnChanges(changes: SimpleChanges) {
    if(changes.assetId && changes.assetId.previousValue != changes.assetId.currentValue) {  
      this.positionResponse = new PositionResponse();
      this.positionResponse.averagePrice = 0;
      this.positionResponse.totalInvested = 0;
      this.positionResponse.totalProfit = 0;
      this.positionResponse.totalVirtual = 0;
      this.positionResponse.totalQuantity = 0;
      this.positionResponse.averageReturn = 0;
      this.positionResponse.successRate = 0;
      this.positionResponse.orderCount = 0;
      this.positionResponse.successCount = 0;
      this.positionResponse.totalFee = 0;
      this.positionResponse.averageTradeMinutes = null;
      this.positionResponse.type = null;
      this.refresh();
    }
  }

  ngOnInit() {
    this.refresh();
  }
  
  setPosition(positionResponse: PositionResponse) {
    if (this.assetId && this.positionResponse && positionResponse && this.positionResponse.orderCount != positionResponse.orderCount) {
      this.refresh();
    }
    this.allPositionsResponse = positionResponse;
    this.positionResponse = positionResponse;
  }

  setAssetPositions(assetPositionResponse: AssetPositionResponse[]) {
    this.assetPositionsResponse = assetPositionResponse;
  }

  onFilterCoin(assetId: any) {
    if (assetId) {
      assetId = parseInt(assetId);
      for (let i = 0; i < this.assetPositionsResponse.length; ++i) {
        if (this.assetPositionsResponse[i].assetId == assetId) {
          this.setFilteredAsset(this.assetPositionsResponse[i]);
          break;
        }
      }
    } else {
      this.setFilteredAsset(null);
    }
  }

  setFilteredAsset(position: AssetPositionResponse) {
    if (position) {
      this.positionResponse = position.positionResponse;
      this.setDataSource(this.orders.filter(option => option.assetId == position.assetId));
    } else {
      this.positionResponse = this.allPositionsResponse;
      this.setDataSource(this.orders);
    }
  }

  refresh() {
    this.advisorService.getAdvisorOrders(this.userId, [Constants.OrderStatus.Close], this.assetId).subscribe(result => {
      this.orders = result;
      this.setDataSource(this.orders);
    });
  }

  setDataSource(orders: OrderResponse[]) {
    this.dataSource.data = orders;
      if (!this.dataSource.sort) {
        this.dataSource.sort = this.sort;
      }
      this.setBestWorstTrade(orders);
  }

  onAssetClick(assetId: number) {
    if (!this.assetId) {
      this.navigationService.goToAssetDetails(assetId);
    }
  }

  onClickSearchCoin(event: any) {
    event.stopPropagation();
  }

  getAssetImgUrl(id: number) {
    return CONFIG.assetImgUrl.replace("{id}", id.toString());
  }

  getAvgTradeMinutes(){
    var ret = new Date()
    ret.setMinutes( ret.getMinutes() - this.positionResponse.averageTradeMinutes);
    return ret;
  }

  getProfitTooltip(order: OrderResponse) {
    let profit = new PercentageDisplayPipe().transform(order.profitWithoutFee);
    let profitValue = new ValueDisplayPipe().transform(order.profitWithoutFeeValue);
    return "Profit without fee: " + profit + " / " + profitValue;
  }

  getOrderDescription(order: OrderResponse){
    return Util.GetOrderTypeText(!order.type ? Constants.OrderType.Buy : Constants.OrderType.Sell).toUpperCase();
  }

  getCloseReasonDescription(type?: number){
    if (type == Constants.OrderActionType.TakeProfit) {
      return "TAKE PROFIT";
    } else if (type == Constants.OrderActionType.StopLoss) {
      return "STOP LOSS";
    }
    return "";
  }

  private setBestWorstTrade(orders: OrderResponse[]){
    let best = -10000000;
    let worst = 10000000;
    orders.forEach(o => 
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

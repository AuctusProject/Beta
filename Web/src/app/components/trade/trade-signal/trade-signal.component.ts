import { Component, OnInit, Output, ViewChild, EventEmitter } from '@angular/core';
import { OrderResponse } from '../../../model/trade/orderResponse';
import { MatSort, MatTableDataSource } from '@angular/material';
import { TradeService } from '../../../services/trade.service';
import { EventsService } from 'angular-event-service';
import { AccountService } from '../../../services/account.service';
import { Util } from '../../../util/util';
import { Constants } from '../../../util/constants';

@Component({
  selector: 'trade-signal',
  templateUrl: './trade-signal.component.html',
  styleUrls: ['./trade-signal.component.css']
})
export class TradeSignalComponent implements OnInit {
  orders: OrderResponse[];
  @Output() newSignal = new EventEmitter<void>();
  @ViewChild(MatSort) sort: MatSort;
  dataSource = new MatTableDataSource<OrderResponse>();
  displayedColumns: string[] = [
    "trader",
    "date",
    "trade"
  ];

  constructor(private tradeService: TradeService,
    private accountService: AccountService,
    private eventsService: EventsService) { }

  ngOnInit() {
    if (this.accountService.isLoggedIn())
    {
      this.tradeService.listFollowedTrades().subscribe(ret =>
        {
            this.orders = ret;
            this.setDataSource();
        });
      this.eventsService.on("onTradeSignal", (data) => this.setNewOrders(data));
    }
  }

  setNewOrders(newOrders: OrderResponse[]) {
    for (let i = newOrders.length - 1; i >= 0; --i) {
      this.orders.unshift(newOrders[i]);
    }
    this.setDataSource();
    this.newSignal.emit();
  }

  setDataSource() {
    this.dataSource.data = this.orders;
    if (!this.dataSource.sort) {
      this.dataSource.sort = this.sort;
    }
  }

  getOrderDescription(order: OrderResponse) {
    return Util.GetOrderTypeText(this.getNormalizedType(order) ? Constants.OrderType.Buy : Constants.OrderType.Sell).toUpperCase();
  }

  getNormalizedType(order: OrderResponse) {
    return order.status == Constants.OrderStatus.Close ? !order.type : !!order.type;
  }

  getOrderTypeText(order: OrderResponse) {
    if (order.status == Constants.OrderStatus.Close) {
      if (order.actionType == Constants.OrderActionType.StopLoss) {
        return "SL";
      } else if (order.actionType == Constants.OrderActionType.TakeProfit) {
        return "TP";
      } else {
        return "Close";
      }
    } else {
      return "Open";
    }
  }

  getOrderTypeHint(order: OrderResponse) {
    if (order.status == Constants.OrderStatus.Close) {
      if (order.actionType == Constants.OrderActionType.StopLoss) {
        return "Close triggered by Stop Loss";
      } else if (order.actionType == Constants.OrderActionType.TakeProfit) {
        return "Close triggered by Take Profit";
      } else {
        return "Close";
      }
    } else {
      return "Open";
    }
  }
}

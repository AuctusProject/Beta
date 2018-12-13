import { Component, OnInit, OnChanges, SimpleChanges, Input, EventEmitter, Output, ViewChild } from '@angular/core';
import { OrderResponse } from "../../../../../model/trade/orderResponse";
import { Util } from "../../../../../util/util";
import { ModalService } from '../../../../../services/modal.service';
import { Subscription } from 'rxjs';
import { ValueDisplayPipe } from '../../../../../util/value-display.pipe';
import { MatTableDataSource, MatSort } from '@angular/material';

@Component({
  selector: 'orders-table',
  templateUrl: './orders-table.component.html',
  styleUrls: ['./orders-table.component.css']
})

export class OrdersTableComponent implements OnChanges {
  displayedColumns: string[] = [
    "positions",
    "units",
    "value",
    "open",
    "PL",
    "SL",
    "TP",
    "close"
  ];
  utilProxy = Util;

  @ViewChild(MatSort) sort: MatSort;
  dataSource = new MatTableDataSource<OrderResponse>();

  @Input() userId: number;
  @Input() assetId: number;
  @Input() orders: OrderResponse[] = [];
  @Output() updated = new EventEmitter<boolean>();
  promise: Subscription;

  constructor(private modalService: ModalService) { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.orders && changes.orders.currentValue && (changes.orders.previousValue != changes.orders.currentValue ||
      changes.orders.previousValue.length != changes.orders.currentValue.length)) {
      this.dataSource.data = changes.orders.currentValue;
      if (!this.dataSource.sort) {
        this.dataSource.sort = this.sort;
      }
    }
  }

  onNewPercentageProfit($event, order: OrderResponse){
    order.profit = $event;
  }

  onNewProfit($event, order: OrderResponse){
    order.profitValue = $event;
  }

  closeOrder(order: OrderResponse){
    this.modalService.setCloseOrderDialog(order).afterClosed().subscribe(ret => 
      {
        if (ret) {
          this.updated.emit(true);
        }
      });
  }

  onEditStopLoss(order: OrderResponse) {
    this.modalService.setEditStopLossDialog(order).afterClosed().subscribe(ret => 
      {
        if (ret) {
          this.updated.emit(false);
        }
      });
  }

  onEditTakeProfit(order: OrderResponse) {
    this.modalService.setEditTakeProfitDialog(order).afterClosed().subscribe(ret => 
      {
        if (ret) {
          this.updated.emit(false);
        }
      });
  }
}

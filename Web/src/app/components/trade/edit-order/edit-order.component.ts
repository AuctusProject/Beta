import { Component, OnInit, Input, Output, EventEmitter, Inject, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { MatDialog, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { OrderResponse } from '../../../model/trade/orderResponse';
import { TradeService } from '../../../services/trade.service';
import { SetTradeComponent } from '../new-trade/set-trade/set-trade.component';
import { Constants } from '../../../util/constants';

@Component({
  selector: 'edit-order',
  templateUrl: './edit-order.component.html',
  styleUrls: ['./edit-order.component.css']
})
export class EditOrderComponent implements OnInit {
  order: OrderResponse;
  promise: Subscription;
  @ViewChild("Limit") Limit: SetTradeComponent;

  constructor(public dialogRef: MatDialogRef<EditOrderComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit() {
    this.order = this.data.order;
  }

  getTitle() : string {
      return "Update " + (this.order.type == Constants.OrderType.Buy ? "BUY " : "SELL ") + this.order.assetCode + " order";
  }

  onConfirm() {
    if (this.order.type == Constants.OrderType.Buy) {
        this.promise = this.Limit.onBuy();
    } else {
        this.promise = this.Limit.onSell();
    }
  }

  onOrderCreated() {
    this.dialogRef.close(true);
  }
}

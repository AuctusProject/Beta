import { Component, OnInit, Input, Output, EventEmitter, Inject, ViewChild, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { MatDialog, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { OrderResponse } from '../../../model/trade/orderResponse';
import { TradeService } from '../../../services/trade.service';
import { Constants } from '../../../util/constants';
import { InheritanceInputComponent } from '../../util/inheritance-input/inheritance-input.component';
import { InputType } from '../../../model/inheritanceInputOptions';
import { TickerService } from '../../../services/ticker.service';
import { ValueDisplayPipe } from '../../../util/value-display.pipe';
import { NotificationsService } from 'angular2-notifications';
import { EventsService } from 'angular-event-service/dist';
import { ConfirmationDialogComponent } from '../../util/confirmation-dialog/confirmation-dialog.component';
import { BinanceTickerPayload } from '../../../model/binanceTickerPayload';
import { CONFIG } from '../../../services/config.service';

@Component({
  selector: 'edit-trade-value',
  templateUrl: './edit-trade-value.component.html',
  styleUrls: ['./edit-trade-value.component.css']
})
export class EditTradeValueComponent implements OnInit, OnDestroy {
  order: OrderResponse;
  stopLossField: boolean;
  takeProfitField: boolean;
  amountField: boolean;

  promise: Subscription;
  value?: number;
  valueOptions: any = this.getBaseOptions();
  mainTickerSubscription: Subscription;
  multiplierTickerSubscription: Subscription;
  currentValue?: number = null;
  multiplierValue?: number = null;
  baseValue?: number = null;
  initialized: boolean = false;
  orderClosedFully: boolean = false;
  executing: boolean = false;
  @ViewChild("Field") Field: InheritanceInputComponent;

  constructor(public dialogRef: MatDialogRef<EditTradeValueComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialog: MatDialog,
    private tickerService: TickerService,
    private tradeService: TradeService,
    private notificationsService: NotificationsService,
    private eventsService: EventsService) { }

  ngOnInit() {
    this.order = this.data.order;
    this.stopLossField = this.data.stopLossField;
    this.takeProfitField = this.data.takeProfitField;
    this.amountField = this.data.amountField;
    if (this.stopLossField) {
      this.value = this.order.stopLoss;
      this.valueOptions.textOptions["placeHolder"] = "Stop loss";
    } else if (this.takeProfitField) {
      this.value = this.order.takeProfit;
      this.valueOptions.textOptions["placeHolder"] = "Take profit";
    } else {
      this.value = this.order.remainingQuantity;
      this.valueOptions.textOptions["placeHolder"] = "Amount";
      this.valueOptions.numberOptions["max"] = this.order.remainingQuantity;
      this.valueOptions["suffixText"] = this.order.assetCode;
    }
    if (this.order.pair) {
      if (this.order.pair.symbol) {
        this.mainTickerSubscription = this.tickerService.binanceTicker(this.order.pair.symbol).subscribe(ret =>
          {
            if (!this.order.pair.multipliedSymbol) {
              this.setCurrentPrice(this.getConsideredPrice(ret));
            } else {
              this.baseValue = this.getConsideredPrice(ret);
              if (this.multiplierValue || this.multiplierValue == 0) {
                this.setCurrentPrice(this.baseValue * this.multiplierValue);
              }
            }
          });
      }
      if (this.order.pair.multipliedSymbol) {
        this.multiplierTickerSubscription = this.tickerService.binanceTicker(this.order.pair.multipliedSymbol).subscribe(ret =>
          {
            this.multiplierValue = this.getConsideredPrice(ret);
            if (this.baseValue || this.baseValue == 0) {
              this.setCurrentPrice(this.baseValue * this.multiplierValue);
            }
          });
      }
      this.initialized = true;
    }
  }

  private getConsideredPrice(ticker: BinanceTickerPayload) : number {
    return this.order.type === Constants.OrderType.Sell ? ticker.bestAskPrice : ticker.bestBidPrice;
  }

  ngOnDestroy() {
    this.initialized = false;
    this.baseValue = null;
    this.multiplierValue = null;
    this.currentValue = null;
    if (this.mainTickerSubscription) {
      this.mainTickerSubscription.unsubscribe();
    }
    if (this.multiplierTickerSubscription) {
      this.multiplierTickerSubscription.unsubscribe();
    }
  }

  onEditValue(newValue?: any) {
    this.value = (newValue || newValue === 0 || newValue === "0" ? parseFloat(newValue) : null);
    this.Field.setForcedError(null);
    if (this.amountField) {
      if (!this.value || !this.currentValue) {
        this.Field.forceHint(null);
      } else {
        this.setAmountField();
      }
    } 
  }

  getTitle() : string {
    let orderName = (this.order.type == Constants.OrderType.Buy ? "BUY " : "SELL ") + this.order.assetCode;
    if (this.stopLossField || this.takeProfitField) {
        return "Update " + (this.stopLossField ? "stop loss" : "take profit") + " for " + orderName;
    } else {
        return "Close " + orderName + " order";
    }
  }

  getCancelText() : string {
    return "CANCEL";
  } 

  getConfirmText() : string {
    return (this.stopLossField || this.takeProfitField) ? "UPDATE" : "CLOSE";
  }

  onConfirm() {
    if (this.isValidField()) {
      if (this.stopLossField) {
        this.executing = true;
        this.promise = this.tradeService.editOrderStopLoss(this.order.id, this.value).subscribe(ret =>
          {
            this.dialogRef.close(true);
            this.notificationsService.success(null, "Order updated successfully.");
          }, error => this.executing = false);
      } else if (this.takeProfitField) {
        this.executing = true;
        this.promise = this.tradeService.editOrderTakeProfit(this.order.id, this.value).subscribe(ret =>
          {
            this.dialogRef.close(true);
            this.notificationsService.success(null, "Order updated successfully.");
          }, error => this.executing = false);
      } else if (this.orderClosedFully) {
          var data = {
            title: "Order will be fully closed",
            message: "Are you sure that you want to close this position?",
            closeLabel: "CANCEL",
            confirmLabel: "CONFIRM"
          };
          this.dialog.open(ConfirmationDialogComponent, {width: '370px', height: '180px', hasBackdrop: true, disableClose: true, panelClass:"confirm-dialog", data: data}).afterClosed().subscribe(ret =>
            {
              if (ret) {
                this.executing = true;
                this.setCloseOrder();
              }
            });
      } else {
        this.executing = true;
        this.setCloseOrder();
      }
    }
  }

  private setCloseOrder() {
    this.promise = this.tradeService.closeOrder(this.order.id, this.value).subscribe(ret =>
      {
        let message = "Order " + (this.orderClosedFully ? "fully" : "partially") + " closed." 
        this.eventsService.broadcast("onUpdateAdvisor", [ret]);
        this.dialogRef.close(true);
        this.notificationsService.success(null, message);
      }, error => this.executing = false);
  }

  private isValidField() : boolean {
    this.orderClosedFully = false;
    let isValid = this.Field.isValid();
    if (isValid) {
      let messageError = this.stopLossField ? "Invalid stop loss for current value" : this.takeProfitField ? "Invalid take profit for current value" : "Invalid amount value";
      if (this.value === 0) {
        this.Field.setForcedError(messageError);
        return false;
      }
      if (this.currentValue) {
        if (this.stopLossField || this.takeProfitField) {
          if (this.value) {
            if (Constants.OrderType.Buy == this.order.type) {
              if ((this.takeProfitField && this.value <= this.currentValue) || (this.stopLossField && this.value >= this.currentValue)) {
                this.Field.setForcedError(messageError);
                return false;
              }
            } else if ((this.takeProfitField && this.value >= this.currentValue) || (this.stopLossField && (this.value <= this.currentValue || this.value > (this.order.price * 2)))) {
              this.Field.setForcedError(messageError);
              return false;
            }
          }
        } else if (!this.value || this.value == this.order.remainingQuantity) {
          this.orderClosedFully = true;
        } 
      }
    }
    return isValid;
  }

  private setCurrentPrice(price: number) {
    this.currentValue = price; 
    if (this.amountField && this.value) {
      this.setAmountField();
    }
  }

  private setAmountField() {
    let fee = this.value * this.currentValue * CONFIG.orderFee;
    this.Field.forceHint("≈ " + new ValueDisplayPipe().transform(this.value * this.currentValue - fee) + "   /   ≈ " + new ValueDisplayPipe().transform(fee) + " fee");
  }

  private getBaseOptions() : any {
    return { inputType: InputType.Number, formClass: "number-trade-form", labelClass: "number-trade-label number-trade-optional", inputClass: "number-trade-input", suffixClass: "number-trade-suffix number-trade-optional", darkLayout: true,  suffixText: "USD", textOptions: { outlineField: true, showHintSize: false, required: false }, numberOptions: { min: 0, max: 999999999 } };
  }
}

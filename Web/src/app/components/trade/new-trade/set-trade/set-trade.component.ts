import { Component, OnInit, Input, Output, OnDestroy, ViewChild, EventEmitter, SimpleChanges } from '@angular/core';
import { InputType } from '../../../../model/inheritanceInputOptions';
import { PairResponse } from '../../../../model/asset/assetResponse';
import { Subscription } from 'rxjs';
import { TickerService } from '../../../../services/ticker.service';
import { TradeService } from '../../../../services/trade.service';
import { InheritanceInputComponent } from '../../../util/inheritance-input/inheritance-input.component';
import { Constants } from "../../../../util/constants";
import { ValueDisplayPipe } from '../../../../util/value-display.pipe';
import { NotificationsService } from 'angular2-notifications';
import { EventsService } from 'angular-event-service/dist';
import { OrderResponse } from '../../../../model/trade/orderResponse';

@Component({
  selector: 'set-trade',
  templateUrl: './set-trade.component.html',
  styleUrls: ['./set-trade.component.css']
})
export class SetTradeComponent implements OnInit, OnDestroy {
  @Input() limit: boolean;
  @Input() edition: boolean;
  @Input() order: OrderResponse;
  @Input() assetId: number;
  @Input() assetCode: string;
  @Input() shortSellingEnabled: boolean;
  @Input() assetPair: PairResponse;
  @ViewChild("TakeProfit") TakeProfit: InheritanceInputComponent;
  @ViewChild("StopLoss") StopLoss: InheritanceInputComponent;
  @ViewChild("Price") Price: InheritanceInputComponent;
  @ViewChild("Amount") Amount: InheritanceInputComponent;
  @Output() orderCreated = new EventEmitter<void>();

  promise: Subscription;
  mainTickerSubscription: Subscription;
  multiplierTickerSubscription: Subscription;
  currentValue?: number = null;
  multiplierValue?: number = null;
  baseValue?: number = null;
  initialized: boolean = false;
  limitOrderExecutedMarket: boolean = false;
  btnDisabled: boolean = false;

  stopLossValue?: number;
  takeProfitValue?: number;
  amountValue?: number;
  priceValue?: number;

  stopLossOptions: any = this.getBaseOptions();
  takeProfitOptions: any =  this.getBaseOptions();
  amountOptions: any = this.getBaseOptions();
  priceOptions: any = this.getBaseOptions();

  constructor(private tickerService: TickerService,
    private tradeService: TradeService,
    private notificationsService: NotificationsService,
    private eventsService: EventsService) { }

  ngOnChanges(changes: SimpleChanges) {
    if(this.initialized && (!changes.assetId.previousValue || changes.assetId.previousValue != changes.assetId.currentValue)) {
      this.ngOnDestroy();
      this.clear();
      if (changes.shortSellingEnabled) {
        this.shortSellingEnabled = changes.shortSellingEnabled.currentValue;
      }
      this.initialize();
    }
  }
  
  ngOnInit() {
    if (this.order) {
      this.assetId = this.order.assetId;
      this.assetCode = this.order.assetCode;
      this.assetPair = this.order.pair;
      this.stopLossValue = this.order.stopLoss;
      this.amountValue = this.order.remainingQuantity;
      this.takeProfitValue = this.order.takeProfit;
      this.priceValue = this.order.price;
      this.shortSellingEnabled = this.order.shortSellingEnabled;
    }
    this.stopLossOptions.textOptions["placeHolder"] = "Stop loss";
    this.stopLossOptions.textOptions["required"] = false;
    this.stopLossOptions["labelClass"] += " number-trade-optional";
    this.stopLossOptions["suffixClass"] += " number-trade-optional";
    this.takeProfitOptions.textOptions["placeHolder"] = "Take profit";
    this.takeProfitOptions.textOptions["required"] = false;
    this.takeProfitOptions["labelClass"] += " number-trade-optional";
    this.takeProfitOptions["suffixClass"] += " number-trade-optional";

    this.priceOptions.textOptions["placeHolder"] = "Price";
    if (!this.limit) {
      this.priceOptions["inputType"] = InputType.Text;
      this.priceOptions.textOptions["showValidatorError"] = false;
      this.priceOptions.textOptions["required"] = false;
      this.priceOptions.textOptions["disabled"] = true;
    }
    this.amountOptions.textOptions["placeHolder"] = "Amount";
    if (!!this.assetCode) {
      this.amountOptions["suffixText"] = this.assetCode;
    }

    this.initialize();
  }

  private initialize() {
    if (this.Amount && !!this.assetCode) {
      this.Amount.forceSuffixText(this.assetCode);
    }
    if (this.assetPair) {
      if (this.assetPair.symbol) {
        this.mainTickerSubscription = this.tickerService.binanceTicker(this.assetPair.symbol).subscribe(ret =>
          {
            if (!this.assetPair.multipliedSymbol) {
              this.setCurrentPrice(ret.currentClosePrice);
            } else {
              this.baseValue = ret.currentClosePrice;
              if (this.multiplierValue || this.multiplierValue == 0) {
                this.setCurrentPrice(this.baseValue * this.multiplierValue);
              }
            }
          });
      }
      if (this.assetPair.multipliedSymbol) {
        this.multiplierTickerSubscription = this.tickerService.binanceTicker(this.assetPair.multipliedSymbol).subscribe(ret =>
          {
            this.multiplierValue = ret.currentClosePrice;
            if (this.baseValue || this.baseValue == 0) {
              this.setCurrentPrice(this.baseValue * this.multiplierValue);
            }
          });
      }
      this.initialized = true;
    }
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

  public clear() {
    this.stopLossValue = null;
    this.amountValue = null;
    this.takeProfitValue = null;
    this.priceValue = null;
    this.btnDisabled = false;
    if (this.TakeProfit) {
      this.TakeProfit.setForcedError(null);
      this.TakeProfit.clear();
    }
    if (this.StopLoss) {
      this.StopLoss.setForcedError(null);
      this.StopLoss.clear();
    }
    if (this.Amount) {
      this.Amount.forceHint(null);
      this.Amount.clear();
    }
    if (this.Price) {
      this.Price.clear();
    }
  }

  private setCurrentPrice(price: number) {
    this.currentValue = price; 
    if (!this.limit && this.currentValue) {
      this.Price.forceValue(new ValueDisplayPipe().transform(this.currentValue, ''));
      if (this.amountValue) {
        this.Amount.forceHint("≈ " + new ValueDisplayPipe().transform(this.amountValue * this.currentValue));
      }
    }
  }

  public onTakeProfit(newValue?: any) {
    this.takeProfitValue = this.getFormattedNumber(newValue);
    this.TakeProfit.setForcedError(null);
  }

  public onStopLoss(newValue?: any) {
    this.stopLossValue = this.getFormattedNumber(newValue);
    this.StopLoss.setForcedError(null);
  }

  public onPrice(newValue?: any) {
    this.priceValue = this.getFormattedNumber(newValue);
    this.Price.setForcedError(null);
    if (this.limit) {
      if (!this.amountValue || !this.priceValue) {
        this.Amount.forceHint(null);
      } else {
        this.Amount.forceHint(new ValueDisplayPipe().transform(this.amountValue * this.priceValue));
      }
    }
  }

  public onAmount(newValue?: any) {
    this.amountValue = this.getFormattedNumber(newValue);
    this.Amount.setForcedError(null);
    if (!this.amountValue || (!this.currentValue && !this.priceValue)) {
      this.Amount.forceHint(null);
    } else if (!this.limit && this.currentValue) {
      this.Amount.forceHint("≈ " + new ValueDisplayPipe().transform(this.amountValue * this.currentValue));
    } else if (this.limit && this.priceValue) {
      this.Amount.forceHint(new ValueDisplayPipe().transform(this.amountValue * this.priceValue));
    }
  }

  private getFormattedNumber(value?: any) : number {
    return (value || value === 0 || value === "0" ? parseFloat(value) : null);
  }

  private getBaseOptions() : any {
    return { inputType: InputType.Number, formClass: "number-trade-form", labelClass: "number-trade-label", inputClass: "number-trade-input", suffixClass: "number-trade-suffix", darkLayout: true,  suffixText: "USD", textOptions: { outlineField: true, showHintSize: false }, numberOptions: { min: 0, max: 999999999 } };
  }

  public onSell() : Subscription {
    if (!this.btnDisabled && this.validTrade(Constants.OrderType.Sell)) {
      this.promise = this.setOrder(Constants.OrderType.Sell);
      return this.promise;
    }
    return null;
  }

  public onBuy() : Subscription {
    if (!this.btnDisabled && this.validTrade(Constants.OrderType.Buy)) {
      this.promise = this.setOrder(Constants.OrderType.Buy);
      return this.promise;
    }
    return null;
  }

  private setOrder(orderType: number) : Subscription {
    this.btnDisabled = true;
    if (!this.edition) {
      return this.tradeService.createOrder(this.assetId, orderType, this.amountValue, this.priceValue, this.stopLossValue, this.takeProfitValue).subscribe(ret => 
        {
          this.orderCreated.emit();
          this.btnDisabled = false;
          this.eventsService.broadcast("onUpdateAdvisor", [ret]);
          this.notificationsService.success(null, "Order created successfully.");
        }, error => 
        {
          this.btnDisabled = false;
        });
    } else {
      return this.tradeService.editOrder(this.order.id, this.amountValue, this.priceValue, this.stopLossValue, this.takeProfitValue).subscribe(ret => 
        {
          this.orderCreated.emit();
          this.btnDisabled = false;
          this.eventsService.broadcast("onUpdateAdvisor", [ret]);
          this.notificationsService.success(null, "Order updated successfully.");
        }, error => 
        {
          this.btnDisabled = false;
        });
    }
  }

  private validTrade(orderType: number) : boolean {
    this.limitOrderExecutedMarket = false;
    let isValid = this.Price.isValid();
    isValid = this.Amount.isValid() && isValid;
    isValid = this.StopLoss.isValid() && isValid;
    isValid = this.TakeProfit.isValid() && isValid;
    if (isValid) {
      if (this.priceValue === 0) {
        this.Price.setForcedError("Invalid price value");
          return false;
      } else {
        this.Price.setForcedError(null);
      }
      if (this.amountValue === 0) {
        this.Amount.setForcedError("Invalid amount value");
          return false;
      } else {
        this.Amount.setForcedError(null);
      }
      if (this.stopLossValue === 0) {
        this.StopLoss.setForcedError("Invalid stop loss for current value");
          return false;
      }
      if (this.takeProfitValue === 0) {
        this.TakeProfit.setForcedError("Invalid take profit for current value");
        return false;
      } 
      if (!this.edition && !this.shortSellingEnabled && Constants.OrderType.Sell == orderType) {
        this.notificationsService.error(null, "Invalid short operation for this market.");
        return false;
      }
      if (this.currentValue) {
        if (Constants.OrderType.Buy == orderType) {
          this.limitOrderExecutedMarket = this.limit && this.priceValue >= this.currentValue;
          if (this.takeProfitValue) {
            if (this.takeProfitValue <= this.currentValue && !this.limit) {
              this.TakeProfit.setForcedError("Invalid take profit for current value");
              return false;
            } else if (this.takeProfitValue <= this.priceValue) {
              this.TakeProfit.setForcedError("Invalid take profit for the price");
              return false;
            } else {
              this.TakeProfit.setForcedError(null);
            }
          }
          if (this.stopLossValue) {
            if (this.stopLossValue >= this.currentValue && !this.limit) {
              this.StopLoss.setForcedError("Invalid stop loss for current value");
              return false;
            } else if (this.stopLossValue >= this.priceValue) {
              this.StopLoss.setForcedError("Invalid stop loss for the price");
              return false;
            } else {
              this.StopLoss.setForcedError(null);
            }
          }
        } else {
          this.limitOrderExecutedMarket = this.limit && this.priceValue <= this.currentValue;
          if (this.takeProfitValue) {
            if (this.takeProfitValue >= this.currentValue && !this.limit) {
              this.TakeProfit.setForcedError("Invalid take profit for current value");
              return false;
            } else if (this.takeProfitValue >= this.priceValue) {
              this.TakeProfit.setForcedError("Invalid take profit for the price");
              return false;
            } else {
              this.TakeProfit.setForcedError(null);
            }
          }
          if (this.stopLossValue) {
            if (!this.limit && (this.stopLossValue <= this.currentValue || this.stopLossValue > (this.currentValue * 2))) {
              this.StopLoss.setForcedError("Invalid stop loss for current value");
              return false;
            } else if (this.stopLossValue <= this.priceValue || this.stopLossValue > (this.priceValue * 2)) {
              this.StopLoss.setForcedError("Invalid stop loss for the price");
              return false;
            } else {
              this.StopLoss.setForcedError(null);
            }
          }
        }
      }
    }
    return isValid;
  }
}

import { Component, Input, OnInit, OnDestroy, OnChanges, Output, EventEmitter } from '@angular/core';
import { TickerService } from '../../../services/ticker.service';
import { Subscription } from 'rxjs';
import { BinanceTickerPayload } from '../../../model/binanceTickerPayload';
import { PairResponse } from '../../../model/asset/assetResponse';
import { ValueDisplayPipe } from '../../../util/value-display.pipe';
import { Constants } from '../../../util/constants';
import { Util } from '../../../util/util';
import { CONFIG } from '../../../services/config.service';

@Component({
  selector: 'ticker-profit-field',
  templateUrl: './ticker-profit-field.component.html',
  styleUrls: ['./ticker-profit-field.component.css']
})
export class TickerProfitFieldComponent implements OnInit, OnDestroy, OnChanges {
  Math = Math;
  @Input() pair: PairResponse;
  @Input() startValue?: any = null;
  @Input() priceValue?: any = null;
  @Input() quantityValue?: any = null;
  @Input() orderType?: number = null;
  @Input() blinkGray: boolean = false;
  @Output() onNewValue = new EventEmitter<number>();
  value: number;
  mainTickerSubscription: Subscription;
  multiplierTickerSubscription: Subscription;
  quoteVariation?: number = null;
  baseVariation?: number = null;
  quoteValue?: number = null;
  baseValue?: number = null;
  initialized: boolean = false;

  constructor(private tickerService: TickerService) { }

  ngOnChanges() {
    if (this.initialized) {
      this.ngOnDestroy();
      this.ngOnInit();
    }
  }
  
  ngOnInit() {
    if (this.startValue != undefined) {
      this.value = parseFloat(this.startValue);
    }
    if (this.priceValue && this.quantityValue) {
      this.priceValue = parseFloat(this.priceValue);
      this.quantityValue = parseFloat(this.quantityValue);
    }
    if (this.pair) {
      if (this.pair.symbol) {
      this.mainTickerSubscription = this.tickerService.binanceTicker(this.pair.symbol).subscribe(ret =>
        {
          this.setValue(ret, true);
        });
      }
      if (this.pair.multipliedSymbol) {
        this.multiplierTickerSubscription = this.tickerService.binanceTicker(this.pair.multipliedSymbol).subscribe(ret =>
          {
            this.setValue(ret, false);
          });
      }
    }
    this.initialized = true;
  }

  setValue(ticker: BinanceTickerPayload, isMainPair: boolean) {
    if (!this.pair.multipliedSymbol) {
      if (this.priceValue && this.quantityValue) {
        this.value = this.priceValue * this.quantityValue * Util.GetProfit(this.orderType, this.priceValue, this.priceValue * this.quantityValue * CONFIG.orderFee, this.quantityValue, this.quantityValue, this.getConsideredPrice(ticker));
      } else {
        this.value = this.getConsideredPrice(ticker) * ticker.priceChangePercentage / 100;
      }
    } else {
      if (isMainPair) {
        this.baseValue = this.getConsideredPrice(ticker);
        this.baseVariation = ticker.priceChangePercentage;
      } else {
        this.quoteValue = this.getConsideredPrice(ticker);
        this.quoteVariation = ticker.priceChangePercentage;
      }
      if (this.priceValue && this.quantityValue) {
        if (this.baseValue && this.quoteValue) {
          this.value = this.priceValue * this.quantityValue * Util.GetProfit(this.orderType, this.priceValue, this.priceValue * this.quantityValue * CONFIG.orderFee, this.quantityValue, this.quantityValue, (this.baseValue * this.quoteValue));
        }
      } else if (this.baseValue && this.quoteValue && (this.baseVariation || this.baseVariation == 0) 
        && (this.quoteVariation || this.quoteVariation == 0)) {
          let previousBase = this.baseValue / (1 + (this.baseVariation / 100));
          let previousQuote = this.quoteValue / (1 + (this.quoteVariation / 100));
          this.value = this.baseValue * this.quoteValue * (((this.baseValue * this.quoteValue) / (previousBase * previousQuote)) - 1);
      }
    }
    if (this.onNewValue) {
      this.onNewValue.emit(this.value);
    }
  }

  getReferenceValueMultiplier() : number {
    return this.orderType === Constants.OrderType.Sell ? -1.0 : 1.0;
  }

  getConsideredPrice(ticker: BinanceTickerPayload) : number {
    return this.priceValue && this.quantityValue ? this.orderType === Constants.OrderType.Sell ? ticker.bestAskPrice : ticker.bestBidPrice : ticker.currentClosePrice;
  }

  getValue(): string {
    if (this.value == undefined || this.value == null) {
      return "";
    } else {
      return new ValueDisplayPipe().transform(this.value);
    }
  }

  ngOnDestroy() {
    this.initialized = false;
    if (this.mainTickerSubscription) {
      this.mainTickerSubscription.unsubscribe();
    }
    if (this.multiplierTickerSubscription) {
      this.multiplierTickerSubscription.unsubscribe();
    }
  }
}

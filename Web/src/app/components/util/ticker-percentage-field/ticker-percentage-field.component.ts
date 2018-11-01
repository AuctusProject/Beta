import { Component, Input, OnInit, OnDestroy, OnChanges } from '@angular/core';
import { TickerService } from 'src/app/services/ticker.service';
import { Subscription } from 'rxjs';
import { BinanceTickerPayload } from 'src/app/model/binanceTickerPayload';
import { PairResponse } from 'src/app/model/asset/assetResponse';

@Component({
  selector: 'ticker-percentage-field',
  templateUrl: './ticker-percentage-field.component.html',
  styleUrls: ['./ticker-percentage-field.component.css']
})
export class TickerPercentageFieldComponent implements OnInit, OnDestroy, OnChanges {
  @Input() pair: PairResponse;
  @Input() referenceValue?: any = null;
  @Input() startValue?: number = null;
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
    if (this.startValue) {
      this.value = Math.round(this.startValue * 10000) / 100;
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
      if (this.referenceValue) {
        this.value = Math.round(((ticker.currentClosePrice / parseFloat(this.referenceValue)) - 1) * 10000) / 100;
      } else {
        this.value = ticker.priceChangePercentage;
      }
    } else {
      if (isMainPair) {
        this.baseValue = ticker.currentClosePrice;
        this.baseVariation = ticker.priceChangePercentage;
      } else {
        this.quoteValue = ticker.currentClosePrice;
        this.quoteVariation = ticker.priceChangePercentage;
      }
      if (this.referenceValue) {
        if (this.baseValue && this.quoteValue) {
          this.value = Math.round((((this.baseValue * this.quoteValue) / parseFloat(this.referenceValue)) - 1) * 10000) / 100;
        }
      } else if (this.baseValue && this.quoteValue && (this.baseVariation || this.baseVariation == 0) 
        && (this.quoteVariation || this.quoteVariation == 0)) {
          let previousBase = this.baseValue * (1 + (this.baseVariation / 100));
          let previousQuote = this.quoteValue * (1 + (this.quoteVariation / 100));
          this.value = Math.round((((this.baseValue * this.quoteValue) / (previousBase * previousQuote)) - 1) * 10000) / 100;
      }
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

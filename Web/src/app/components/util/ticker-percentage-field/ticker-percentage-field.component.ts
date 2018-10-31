import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { TickerService } from 'src/app/services/ticker.service';
import { Subscription } from 'rxjs';
import { BinanceTickerPayload } from 'src/app/model/binanceTickerPayload';

@Component({
  selector: 'ticker-percentage-field',
  templateUrl: './ticker-percentage-field.component.html',
  styleUrls: ['./ticker-percentage-field.component.css']
})
export class TickerPercentageFieldComponent implements OnInit, OnDestroy {
  @Input() pair: string;
  @Input() pairToMultiplier: string;
  @Input() referenceValue?: any = null;
  value: number;
  mainTickerSubscription: Subscription;
  multiplierTickerSubscription: Subscription;
  quoteVariation?: number = null;
  baseVariation?: number = null;
  quoteValue?: number = null;
  baseValue?: number = null;

  constructor(private tickerService: TickerService) { }
  
  ngOnInit() {
    if (this.pair) {
      this.mainTickerSubscription = this.tickerService.binanceTicker(this.pair).subscribe(ret =>
        {
          this.setValue(ret, true);
        });
    }
    if (this.pairToMultiplier) {
      this.multiplierTickerSubscription = this.tickerService.binanceTicker(this.pairToMultiplier).subscribe(ret =>
        {
          this.setValue(ret, false);
        });
    }
  }

  setValue(ticker: BinanceTickerPayload, isMainPair: boolean) {
    if (!this.pairToMultiplier) {
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
    if (this.mainTickerSubscription) {
      this.mainTickerSubscription.unsubscribe();
    }
    if (this.multiplierTickerSubscription) {
      this.multiplierTickerSubscription.unsubscribe();
    }
  }
}

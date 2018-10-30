import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { TickerService } from 'src/app/services/ticker.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'ticker-field',
  templateUrl: './ticker-field.component.html',
  styleUrls: ['./ticker-field.component.css']
})
export class TickerFieldComponent implements OnInit, OnDestroy {
  @Input() pair: string;
  @Input() tickerProperty: string = "currentClosePrice";
  @Input() pairToMultiplier: string;
  @Input() tickerToMultiplierProperty: string = "currentClosePrice";
  @Input() suffix: string = '';
  @Input() prefix: string = '';
  value: number;
  mainTickerSubscription: Subscription;
  multiplierTickerSubscription: Subscription;
  multiplierValue?: number = null;
  baseValue?: number = null;

  constructor(private tickerService: TickerService) { }
  
  ngOnInit() {
    if (this.pair) {
      this.mainTickerSubscription = this.tickerService.binanceTicker(this.pair).subscribe(ret =>
        {
          if (!this.pairToMultiplier) {
            this.value = ret[this.tickerProperty];
          } else {
            this.baseValue = ret[this.tickerProperty];
            if (this.multiplierValue || this.multiplierValue == 0) {
              this.value = this.baseValue * this.multiplierValue;
            }
          }
        });
    }
    if (this.pairToMultiplier) {
      this.multiplierTickerSubscription = this.tickerService.binanceTicker(this.pairToMultiplier).subscribe(ret =>
        {
          this.multiplierValue = ret[this.tickerToMultiplierProperty];
          if (this.baseValue || this.baseValue == 0) {
            this.value = this.baseValue * this.multiplierValue;
          }
        });
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

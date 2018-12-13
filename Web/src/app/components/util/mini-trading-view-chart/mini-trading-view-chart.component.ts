import { Component, Input, AfterViewInit, OnChanges, SimpleChanges } from '@angular/core';
import { PairResponse } from '../../../model/asset/assetResponse';
import { MediaMatcher } from '@angular/cdk/layout';
import { CONFIG } from '../../../services/config.service';

@Component({
  selector: 'mini-trading-view-chart',
  templateUrl: './mini-trading-view-chart.component.html',
  styleUrls: ['./mini-trading-view-chart.component.css']
})

export class MiniTradingViewChartComponent implements AfterViewInit, OnChanges {
  @Input() pair: PairResponse;
  @Input() assetId: number;
  private symbol: string;
  private initialized: boolean = false;
  public uniqueIdentifier: string;

  public mobileScreen: boolean;

  constructor(media: MediaMatcher) { 
    this.mobileScreen = media.matchMedia('(max-width: 959px)').matches;
    this.uniqueIdentifier = "minitradingview_" + Math.floor(Math.random() * 10000).toString();
  }

  ngAfterViewInit(){
    this.loadChart();
    this.initialized = true;
  }

  ngOnChanges(changes: SimpleChanges) {
    if(this.initialized && (!changes.pair.previousValue || changes.pair.previousValue.symbol != changes.pair.currentValue.symbol)) {
      this.loadChart();
    }
  }

  private loadChart() : void {
    if (this.pair) {
      if (!this.pair.multipliedSymbol) {
        this.symbol = this.pair.symbol;
      } else {
        let quotePair = this.pair.multipliedSymbol.substr(0, this.pair.multipliedSymbol.indexOf("USDT"));
        let basePair = this.pair.symbol.substr(0, this.pair.symbol.indexOf(quotePair));
        this.symbol = basePair + "USD";
      }
      this.loadScript();
    }
  }

  private loadScript() : void {
    var chartScript = document.createElement("script");
    chartScript.type = "text/javascript";
    chartScript.src = "https://s3.tradingview.com/external-embedding/embed-widget-mini-symbol-overview.js";
    chartScript.async = true;
    chartScript.text = "{"
    + "\"symbol\":\"BINANCE:" + this.symbol + "\","
    + "\"width\":\"100%\","
    + "\"height\":\"100%\","
    + "\"locale\":\"en\","
    + "\"dateRange\":\"1m\","
    + "\"colorTheme\":\"dark\","
    + "\"trendLineColor\":\"#4771fc\","
    + "\"underLineColor\":\"rgba(71, 113, 252, 0.15)\","
    + "\"isTransparent\":true,"
    + "\"autosize\":true,"
    + "\"largeChartUrl\":\"" + CONFIG.webUrl + "/trade-markets/" + this.assetId + "\""
    + "}";
    document.getElementById(this.uniqueIdentifier).appendChild(chartScript);
  }
}
import { Component, Input, AfterViewInit, OnChanges, SimpleChanges } from '@angular/core';
import { PairResponse } from '../../../model/asset/assetResponse';
import { MediaMatcher } from '@angular/cdk/layout';

@Component({
  selector: 'trading-view-chart',
  templateUrl: './trading-view-chart.component.html',
  styleUrls: ['./trading-view-chart.component.css']
})

export class TradingViewChartComponent implements AfterViewInit, OnChanges {
  @Input() pair: PairResponse;
  private symbol: string;
  private initialized: boolean = false;
  public uniqueIdentifier: string;

  public mobileScreen: boolean;

  constructor(media: MediaMatcher) { 
    this.mobileScreen = media.matchMedia('(max-width: 959px)').matches;
    this.uniqueIdentifier = "tradingview_" + Math.floor(Math.random() * 10000).toString();
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
    chartScript.text = "new TradingView.widget({"
    + "'autosize':true,"
    + "'symbol':'BINANCE:" + this.symbol + "',"
    + "'interval':'15',"
    + "'timezone':Intl.DateTimeFormat().resolvedOptions().timeZone,"
    + "'theme':'Dark',"
    + "'style':'1',"
    + "'locale':'en',"
    + "'toolbar_bg':'#f1f3f6',"
    + "'enable_publishing':false,"
    + "'withdateranges':" + (this.mobileScreen ? "false," : "true,")
    + "'hide_side_toolbar':" + (this.mobileScreen ? "true," : "false,")
    + "'container_id':'" + this.uniqueIdentifier + "'"
    + "});";
    document.getElementsByTagName("body")[0].appendChild(chartScript);
  }
}
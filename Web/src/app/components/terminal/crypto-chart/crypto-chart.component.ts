import { Component, OnInit, ViewChild } from '@angular/core';
import { TerminalAssetResponse } from 'src/app/model/asset/terminalAssetResponse';

@Component({
  selector: 'crypto-chart',
  templateUrl: './crypto-chart.component.html',
  styleUrls: ['./crypto-chart.component.css']
})
export class CryptoChartComponent implements OnInit {
  @ViewChild("ChartContainer") ChartContainer: any;  

  constructor() { }

  ngOnInit() {
  }

  refresh(asset: TerminalAssetResponse) {
    if(asset && window && window["cryptowatch"]) {
      let chart = new window["cryptowatch"].Embed(asset.chartExchange, asset.chartPair, {
        presetColorScheme: 'standard', timePeriod: '15m'
      });
      for (let i = 0; i < this.ChartContainer.nativeElement.childNodes.length; ++i) {
        this.ChartContainer.nativeElement.removeChild(this.ChartContainer.nativeElement.childNodes[i]);
      }
      chart.mount('#cryptowatch-container');
    }
  }
}

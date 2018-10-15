import { Component, OnInit, Input, ViewChild } from '@angular/core';

@Component({
  selector: 'crypto-chart',
  templateUrl: './crypto-chart.component.html',
  styleUrls: ['./crypto-chart.component.css']
})
export class CryptoChartComponent implements OnInit {
  @Input() assetId: number;
  @ViewChild("ChartContainer") ChartContainer: any;  

  constructor() { }

  ngOnInit() {
    this.refresh(this.assetId);
  }

  refresh(id: number) {
    this.assetId = id;
    if(window && window["cryptowatch"]) {
      let pair;
      if (this.assetId == 1) {
        pair = 'btcusd';
      } else if (this.assetId == 2) {
        pair = 'ethusd';
      } else if (this.assetId == 3) {
        pair = 'eosusd';
      } else if (this.assetId == 4) {
        pair = 'xrpusd';
      } else if (this.assetId == 6) {
        pair = 'bchusd';
      }
      let chart = new window["cryptowatch"].Embed('bitfinex', pair, {
        presetColorScheme: 'standard'
      });
      for (let i = 0; i < this.ChartContainer.nativeElement.childNodes.length; ++i) {
        this.ChartContainer.nativeElement.removeChild(this.ChartContainer.nativeElement.childNodes[i]);
      }
      chart.mount('#cryptowatch-container');
    }
  }
}

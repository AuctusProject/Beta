import { Component, OnInit, ViewChild } from '@angular/core';
import { CONFIG } from 'src/app/services/config.service';
import { CryptoChartComponent } from './crypto-chart/crypto-chart.component';

@Component({
  selector: 'terminal',
  templateUrl: './terminal.component.html',
  styleUrls: ['./terminal.component.css']
})
export class TerminalComponent implements OnInit {
  assetId: number;
  @ViewChild("CryptoChart") CryptoChart: CryptoChartComponent;  

  constructor() { }

  ngOnInit() {
    this.assetId = 1;
  }

  getLogoImgUrl() {
    return CONFIG.platformImgUrl.replace("{id}", "logo");
  }

  onAssetChange(event: any) {
    if (event.index == 0) {
      this.setAsset(1);
    } else if (event.index == 1) {
      this.setAsset(2);
    } else if (event.index == 2) {
      this.setAsset(4);
    } else if (event.index == 3) {
      this.setAsset(3);
    } else if (event.index == 4) {
      this.setAsset(6);
    }
  }

  setAsset(id :number) {
    this.assetId = id;
    this.CryptoChart.refresh(this.assetId);
  }
}

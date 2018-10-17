import { Component, OnInit, ViewChild } from '@angular/core';
import { CONFIG } from 'src/app/services/config.service';
import { CryptoChartComponent } from './crypto-chart/crypto-chart.component';
import { AssetService } from 'src/app/services/asset.service';
import { TerminalAssetResponse } from 'src/app/model/asset/terminalAssetResponse';

@Component({
  selector: 'terminal',
  templateUrl: './terminal.component.html',
  styleUrls: ['./terminal.component.css']
})
export class TerminalComponent implements OnInit {
  public assets: TerminalAssetResponse[];
  public selectedAsset: TerminalAssetResponse;
  public assetId: number;
  @ViewChild("CryptoChart") CryptoChart: CryptoChartComponent;  

  constructor(private assetService: AssetService) { }

  ngOnInit() {
    this.assetService.getTerminalAssets().subscribe(result =>
      {
        this.assets = result;
        this.setSelectedAsset(this.assets[0]);
      });
  }

  getLogoImgUrl() {
    return CONFIG.platformImgUrl.replace("{id}", "logo");
  }

  onAssetChange(event: any) {
    this.setSelectedAsset(this.assets[event.index]);
  }

  onSelectCoinChanged(event: any) {
    for (let i = 0; i < this.assets.length; ++i) {
      if (this.assets[i].assetId == event.value) {
        this.setSelectedAsset(this.assets[i]);
        break;
      }
    }
  }

  setSelectedAsset(asset: TerminalAssetResponse) {
    this.assetId = asset.assetId;
    this.selectedAsset = asset;
    this.CryptoChart.refresh(this.selectedAsset);
  }
}

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
  @ViewChild("CryptoChart") CryptoChart: CryptoChartComponent;  

  constructor(private assetService: AssetService) { }

  ngOnInit() {
    this.loadScript();
    this.assetService.getTerminalAssets().subscribe(result =>
      {
        this.assets = result;
        this.setSelectedAsset(this.assets[0]);
      });
  }

  loadScript(){
    var script = document.createElement("script");
    script.type = "text/javascript";
    script.text = "window._urq = window._urq || []; "+
    " _urq.push(['initSite', 'd017ccc3-e423-46a6-ac7e-111f43466da6']); "+
    " (function() { " +
    " var ur = document.createElement('script'); ur.type = 'text/javascript'; ur.async = true; " +
    " ur.src = ('https:' == document.location.protocol ? 'https://cdn.userreport.com/userreport.js' : 'http://cdn.userreport.com/userreport.js'); " +
    " var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ur, s); " +
    " })();";
    document.getElementsByTagName("body")[0].appendChild(script);
  }

  getLogoImgUrl() {
    return CONFIG.platformImgUrl.replace("{id}", "logo");
  }

  onAssetChange(event: any) {
    this.setSelectedAsset(this.assets[event.index]);
  }

  setSelectedAsset(asset: TerminalAssetResponse) {
    this.selectedAsset = asset;
    this.CryptoChart.refresh(this.selectedAsset);
  }
}

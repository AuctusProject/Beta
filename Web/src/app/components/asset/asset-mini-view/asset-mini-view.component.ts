import { Component, OnInit, Input } from '@angular/core';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { AssetService } from '../../../services/asset.service';

@Component({
  selector: 'asset-mini-view',
  templateUrl: './asset-mini-view.component.html',
  styleUrls: ['./asset-mini-view.component.css']
})
export class AssetMiniViewComponent implements OnInit {
  assets : AssetResponse[];
  selectedAsset: AssetResponse;

  constructor(private assetService: AssetService) { }

  ngOnInit() {
    this.assetService.getAssetsDetails().subscribe(result => 
      {
        this.assets = result;
        if (this.assets && this.assets.length > 0) {
          this.selectedAsset = this.assets[0];
        }
      });
  }

  openMiniChart(asset: AssetResponse) {
    this.selectedAsset = asset;
  }
}

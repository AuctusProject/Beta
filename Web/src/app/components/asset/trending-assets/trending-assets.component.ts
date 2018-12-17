import { Component, OnInit } from '@angular/core';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { AssetService } from '../../../services/asset.service';
import { AssetsTableType } from '../assets-table/assets-table.component';

@Component({
  selector: 'trending-assets',
  templateUrl: './trending-assets.component.html',
  styleUrls: ['./trending-assets.component.css']
})
export class TrendingAssetsComponent implements OnInit {
  assets : AssetResponse[];
  assetsTableType = AssetsTableType.trending;

  constructor(private assetService: AssetService) { }

  ngOnInit() {
    this.assetService.getTrendingAssets(10).subscribe(result => 
      {
        this.assets = result;
      });
  }

}

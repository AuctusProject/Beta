import { Component, OnInit } from '@angular/core';
import { Asset } from '../../../model/asset/asset';
import { AssetService } from '../../../services/asset.service';

@Component({
  selector: 'top-assets',
  templateUrl: './top-assets.component.html',
  styleUrls: ['./top-assets.component.css']
})
export class TopAssetsComponent implements OnInit {
  assets : Asset[];
  constructor(private assetService: AssetService) { }

  ngOnInit() {
    this.getAssets();
  }

  getAssets(){
    this.assetService.getAssets().subscribe(
      assets => this.assets = assets
    );
  }
}

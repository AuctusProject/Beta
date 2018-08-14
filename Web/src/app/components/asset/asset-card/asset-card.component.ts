import { Component, OnInit, Input } from '@angular/core';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { AssetService } from '../../../services/asset.service';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'asset-card',
  templateUrl: './asset-card.component.html',
  styleUrls: ['./asset-card.component.css']
})
export class AssetCardComponent implements OnInit {
  @Input() asset: AssetResponse;
  constructor(private assetService:AssetService) { }

  ngOnInit() {
  }
  onFollowClick(){
    this.assetService.followAsset(this.asset.assetId).subscribe(result =>this.asset.following = true);
  }
  onUnfollowClick(){
    this.assetService.unfollowAsset(this.asset.assetId).subscribe(result =>this.asset.following = false);
  }

  getAssetImgUrl(){
    return environment.assetImgUrl.replace("{id}", this.asset.assetId.toString());
  }
}

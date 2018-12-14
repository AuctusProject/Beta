import { Component, OnInit, Input, OnChanges, SimpleChanges, OnDestroy } from '@angular/core';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { AssetService } from '../../../services/asset.service';
import { FollowUnfollow } from '../../../model/asset/followUnfollow';

@Component({
  selector: 'asset-mini-view',
  templateUrl: './asset-mini-view.component.html',
  styleUrls: ['./asset-mini-view.component.css']
})
export class AssetMiniViewComponent implements OnInit, OnChanges, OnDestroy {
  @Input() listOnlyWatchlist: boolean;
  allAssets : AssetResponse[];
  filteredAssets : AssetResponse[];
  selectedAsset: AssetResponse;
  initialized: boolean = false;

  constructor(private assetService: AssetService) { }

  ngOnDestroy() {
    this.initialized = false;
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.initialized && changes.listOnlyWatchlist.previousValue != changes.listOnlyWatchlist.currentValue) {
      this.setAssets();
    }
  }

  ngOnInit() {
    this.assetService.getAssetsDetails().subscribe(result => 
      {
        this.allAssets = result;
        this.initialized = true;
        this.setAssets();
      });
  }

  setAssets() {
    this.filteredAssets = !this.listOnlyWatchlist ? this.allAssets : this.allAssets.filter(option => option.following);
    if (this.filteredAssets && this.filteredAssets.length > 0) {
      this.selectedAsset = this.filteredAssets[0];
    }
  }

  openMiniChart(asset: AssetResponse) {
    this.selectedAsset = asset;
  }

  setFollowUnfollow(followUnfollow: FollowUnfollow) {
    if (followUnfollow && this.allAssets) {
      for(let i = 0; i < this.allAssets.length; ++i) {
        if (this.allAssets[i].assetId == followUnfollow.assetId) {
          this.allAssets[i].following = followUnfollow.isFollowing;
          break;
        }
      }
    }
  }
}

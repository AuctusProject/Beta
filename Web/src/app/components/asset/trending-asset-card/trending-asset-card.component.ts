import { Component, OnInit, Input } from '@angular/core';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { Util } from '../../../util/Util';
import { NavigationService } from '../../../services/navigation.service';
import { CONFIG} from "../../../services/config.service";
import { AssetService } from '../../../services/asset.service';

@Component({
  selector: 'trending-asset-card',
  templateUrl: './trending-asset-card.component.html',
  styleUrls: ['./trending-asset-card.component.css']
})
export class TrendingAssetCardComponent implements OnInit {
  @Input() asset : AssetResponse;
  constructor(private navigationService: NavigationService, private assetService:AssetService) { }

  ngOnInit() {
  }

  getGeneralRecommendation(){
    return Util.GetGeneralRecommendationDescription(this.asset.mode);
  }

  seeAllRatingsClick(){
    this.navigationService.goToAssetDetails(this.asset.assetId);
  }

  getAssetImgUrl(){
    return CONFIG.assetImgUrl.replace("{id}", this.asset.assetId.toString());
  }

  getCardColorClass(){
    if(this.asset.mode == Util.AssetModeType.StrongBuy ||
      this.asset.mode == Util.AssetModeType.ModerateBuy)
      return "green";
    else if(this.asset.mode == Util.AssetModeType.StrongSell||
      this.asset.mode == Util.AssetModeType.ModerateSell)
      return "red";
    else
      return "white";
  }

  onFollowClick(){
    this.assetService.followAsset(this.asset.assetId).subscribe(result => this.asset.following = true);
  }

  onUnfollowClick(){
    this.assetService.unfollowAsset(this.asset.assetId).subscribe(result => this.asset.following = false);
  }
}

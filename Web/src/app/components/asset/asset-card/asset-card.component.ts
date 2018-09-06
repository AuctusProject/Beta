import { Component, OnInit, Input } from '@angular/core';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { AssetService } from '../../../services/asset.service';
import { CONFIG} from "../../../services/config.service";
import { Util } from '../../../util/Util';

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
    return CONFIG.assetImgUrl.replace("{id}", this.asset.assetId.toString());
  }

  getGeneralRecommendation(){
    return Util.GetGeneralRecommendationDescription(this.asset.mode);
  }

  getLastValue(){
    return '$'+Math.round(this.asset.lastValue * 100) / 100;
  }

  getRatingLabel(){
    if(this.asset.totalRatings == 1.0) return 'Rating';
    return 'Ratings';
  }

  findRecommendationPercentageByType(type: number) {
    return recommendation => recommendation.type === type;
  }

  getRecommendationDistribution(type: number) {
    var distribution = this.asset.recommendationDistribution.find(this.findRecommendationPercentageByType(type));
    if (distribution != null) return (distribution.total / this.asset.totalRatings) * 100;
    return 0.0;
  }

  getBuyRecommendationPercentageLabel(){
    return this.getRecommendationDistribution(Util.BUY) + '% Buy';
  }

  getSellRecommendationPercentageLabel(){
    return this.getRecommendationDistribution(Util.SELL) + '% Sell';
  }

  getCloseRecommendationPercentageLabel(){
    return this.getRecommendationDistribution(Util.CLOSE) + '% Close';
  }

  get24hVariation(){
    if (this.asset.variation24h == null) return '0.00%';
    return this.asset.variation24h + '%';
  }
}

import { Component, OnInit, Input } from '@angular/core';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { Util } from '../../../util/Util';
import { NavigationService } from '../../../services/navigation.service';

@Component({
  selector: 'trending-asset-card',
  templateUrl: './trending-asset-card.component.html',
  styleUrls: ['./trending-asset-card.component.css']
})
export class TrendingAssetCardComponent implements OnInit {
  @Input() asset : AssetResponse;
  constructor(private navigationService: NavigationService) { }

  ngOnInit() {
  }

  getGeneralRecommendation(){
    return Util.GetGeneralRecommendationDescription(this.asset.mode);
  }

  seeAllRatingsClick(){
    this.navigationService.goToAssetDetails(this.asset.assetId);
  }
}

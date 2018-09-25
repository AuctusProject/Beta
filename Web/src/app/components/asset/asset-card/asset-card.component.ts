import { Component, OnInit, Input } from '@angular/core';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { AssetService } from '../../../services/asset.service';
import { CONFIG} from "../../../services/config.service";
import { Util } from '../../../util/Util';
import { AccountService } from '../../../services/account.service';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { NewAdviceComponent } from '../../advisor/new-advice/new-advice.component';
import { FullscreenModalComponent } from '../../util/fullscreen-modal/fullscreen-modal.component';
import { MatDialog } from '@angular/material';
import { NavigationService } from '../../../services/navigation.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'asset-card',
  templateUrl: './asset-card.component.html',
  styleUrls: ['./asset-card.component.css']
})
export class AssetCardComponent implements OnInit {
  @Input() asset: AssetResponse;
  showButtonForExpert: boolean = false;
  promise: Subscription;

  constructor(public dialog: MatDialog, 
    private assetService:AssetService,
    public accountService: AccountService,
    private navigationService: NavigationService) { }

  ngOnInit() {
    this.showButtonForExpert = this.accountService.isLoggedIn() && this.accountService.getLoginData().isAdvisor;
  }
  
  onFollowClick(event: Event){
    this.promise = this.assetService.followAsset(this.asset.assetId).subscribe(result =>this.asset.following = true);
    event.stopPropagation();
  }
  
  onUnfollowClick(event: Event){
    this.promise = this.assetService.unfollowAsset(this.asset.assetId).subscribe(result =>this.asset.following = false);
    event.stopPropagation();
  }

  getAssetImgUrl(){
    return CONFIG.assetImgUrl.replace("{id}", this.asset.assetId.toString());
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
    if (distribution != null) return Math.round((distribution.total / this.asset.totalRatings) * 10000) / 100;
    return 0.0;
  }

  getBuyRecommendationPercentageLabel(){
    return this.getRecommendationDistribution(Util.BUY) + '% Buy';
  }

  getSellRecommendationPercentageLabel(){
    return this.getRecommendationDistribution(Util.SELL) + '% Sell';
  }

  getCloseRecommendationPercentageLabel(){
    return this.getRecommendationDistribution(Util.CLOSE) + '% Hold';
  }

  get24hVariation(){
    if (this.asset.variation24h == null) return '0.00%';
    return Math.round(this.asset.variation24h * 10000) / 100 + '%';
  }

  onGiveRecommendationClick(event: Event) {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = NewAdviceComponent;
    modalData.componentInput = { assetId: this.asset.assetId };
    this.dialog.open(FullscreenModalComponent, { data: modalData });
    event.stopPropagation(); 
  }

  goToAssetDetails() {
    this.navigationService.goToAssetDetails(this.asset.assetId);
  }
}

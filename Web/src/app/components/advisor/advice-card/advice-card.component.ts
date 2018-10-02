import { Component, OnInit, Input } from '@angular/core';
import { FeedResponse } from '../../../model/advisor/feedResponse';
import { CONFIG } from '../../../services/config.service';
import { AssetService } from '../../../services/asset.service';
import { Util } from '../../../util/Util';
import { NavigationService } from '../../../services/navigation.service';
import { Subscription } from 'rxjs';
import { AccountService } from '../../../services/account.service';

@Component({
  selector: 'advice-card',
  templateUrl: './advice-card.component.html',
  styleUrls: ['./advice-card.component.css']
})
export class AdviceCardComponent implements OnInit {
  @Input() advice : FeedResponse;
  promise : Subscription;
  
  constructor(private assetService : AssetService,
    private navigationService: NavigationService,
    private accountService: AccountService) { }

  ngOnInit() {
  }
  
  getAdvisorImgUrl(){
    return CONFIG.profileImgUrl.replace("{id}", this.advice.advisorUrlGuid);
  }

  getAssetImgUrl(){
    return CONFIG.assetImgUrl.replace("{id}", this.advice.assetId.toString());
  }
  
  onFollowClick(event: Event){
    if(this.accountService.hasInvestmentToCallLoggedAction()){
      this.promise = this.assetService.followAsset(this.advice.assetId).subscribe(result =>
        this.advice.followingAsset = true
      );
    }
    event.stopPropagation();
  }
  
  onUnfollowClick(event: Event){
    this.promise = this.assetService.unfollowAsset(this.advice.assetId).subscribe(result =>this.advice.followingAsset = false);
    event.stopPropagation();
  }

  getAdviceTypeDescription(){
    return Util.GetRecommendationTypeDescription(this.advice.adviceType);
  }

  getAdviceTypeColor(){
    return Util.GetRecommendationTypeColor(this.advice.adviceType);
  }

  goToAssetDetails(){
    this.navigationService.goToAssetDetails(this.advice.assetId);
  }
  
  goToExpertDetails(){
    this.navigationService.goToExpertDetails(this.advice.advisorId);
  }
}

import { Component, OnInit, Input } from '@angular/core';
import { FeedResponse } from '../../../../model/advisor/feedResponse';
import { CONFIG } from '../../../../services/config.service';
import { NavigationService } from '../../../../services/navigation.service';
import { AccountService } from '../../../../services/account.service';
import { AssetService } from '../../../../services/asset.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'event-card',
  templateUrl: './event-card.component.html',
  styleUrls: ['./event-card.component.css']
})
export class EventCardComponent implements OnInit {
  @Input() eventFeed : FeedResponse;
  promise : Subscription;
  
  constructor(private assetService : AssetService,
    private navigationService: NavigationService,
    private accountService: AccountService) { }

  ngOnInit() {
  }

  
  getEventUrl(){
    return CONFIG.eventUrl.replace("{id}", this.eventFeed.event.eventId.toString());
  }

  getEventImgUrl(){
    return CONFIG.platformImgUrl.replace("{id}", "coinmarketcal");
  }

  getAssetImgUrl(){
    return CONFIG.assetImgUrl.replace("{id}", this.eventFeed.assetId.toString());
  }

  goToAssetDetails(){
    this.navigationService.goToAssetDetails(this.eventFeed.assetId);
  }

  onFollowClick(event: Event){
    if(this.accountService.hasInvestmentToCallLoggedAction()){
      this.promise = this.assetService.followAsset(this.eventFeed.assetId).subscribe(result =>
          this.eventFeed.followingAsset = true
      );
    }
    event.stopPropagation();
  }
  
  onUnfollowClick(event: Event){
    this.promise = this.assetService.unfollowAsset(this.eventFeed.assetId).subscribe(result =>this.eventFeed.followingAsset = false);
    event.stopPropagation();
  }
}

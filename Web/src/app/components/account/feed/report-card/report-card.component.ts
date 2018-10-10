import { Component, OnInit, Input } from '@angular/core';
import { FeedResponse } from '../../../../model/advisor/feedResponse';
import { CONFIG } from '../../../../services/config.service';
import { NavigationService } from '../../../../services/navigation.service';
import { AssetService } from '../../../../services/asset.service';
import { AccountService } from '../../../../services/account.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'report-card',
  templateUrl: './report-card.component.html',
  styleUrls: ['./report-card.component.css']
})
export class ReportCardComponent implements OnInit {
  @Input() reportFeed : FeedResponse;
  promise : Subscription;
  
  constructor(private assetService : AssetService,
    private navigationService: NavigationService,
    private accountService: AccountService) { }

  ngOnInit() {
  }

  
  getReportUrl(){
    return CONFIG.reportUrl.replace("{id}", this.reportFeed.report.reportId.toString());
  }

  getReportAgencyImgUrl(){
    return CONFIG.agencyImgUrl.replace("{id}", this.reportFeed.report.agencyId.toString());
  }

  getAssetImgUrl(){
    return CONFIG.assetImgUrl.replace("{id}", this.reportFeed.assetId.toString());
  }

  goToAssetDetails(){
    this.navigationService.goToAssetDetails(this.reportFeed.assetId);
  }

  getBackgroundColor(){
    if(this.reportFeed.report.rateDetails){
      return this.reportFeed.report.rateDetails.hexaColor;
    }
    return null;
  }

  onFollowClick(event: Event){
    if(this.accountService.hasInvestmentToCallLoggedAction()){
      this.promise = this.assetService.followAsset(this.reportFeed.assetId).subscribe(result =>
          this.reportFeed.followingAsset = true
      );
    }
    event.stopPropagation();
  }
  
  onUnfollowClick(event: Event){
    this.promise = this.assetService.unfollowAsset(this.reportFeed.assetId).subscribe(result =>this.reportFeed.followingAsset = false);
    event.stopPropagation();
  }
}

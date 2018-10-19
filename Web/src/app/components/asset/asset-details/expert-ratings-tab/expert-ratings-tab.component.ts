import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { AssetResponse, AssetAdvisorResponse } from '../../../../model/asset/assetResponse';
import { Subscription } from 'rxjs';
import { AccountService } from '../../../../services/account.service';
import { AdvisorService } from '../../../../services/advisor.service';
import { NavigationService } from '../../../../services/navigation.service';
import { Util } from '../../../../util/Util';
import { AdvisorResponse } from '../../../../model/advisor/advisorResponse';
import { ValueDisplayPipe } from '../../../../util/value-display.pipe';

@Component({
  selector: 'expert-ratings-tab',
  templateUrl: './expert-ratings-tab.component.html',
  styleUrls: ['./expert-ratings-tab.component.css']
})
export class ExpertRatingsTabComponent implements OnInit, OnChanges {
  displayedColumns: string[] = ['expertName', 'rankin', 'position', 'value', 'action', 'date', 'followButton'];
  @Input() asset: AssetResponse;
  visibleAdvices: AssetAdvisorResponse[];
  currentPage = 1;
  pageSize = 10;
  promiseExpert: Subscription;
  
  constructor(
    private accountService: AccountService,
    private advisorService: AdvisorService,
    private navigationService:NavigationService) { }

  ngOnInit() {
  }

  ngOnChanges(){
    this.setVisibleAdvices();
  }

  getRecommendationFromType(type: number){
    return Util.GetRecommendationTypeDescription(type);
  }

  getRecomendationFromType(mode: number){
    return Util.GetAdviceModeDescription(mode);
  }
  
  getAdviceTypeColor(type: number){
    return Util.GetRecommendationTypeColor(type);
  }

  getFirstAdviceParametersDescription(assetAdvisor: AssetAdvisorResponse) {
    if (!assetAdvisor.lastAdviceTargetPrice && assetAdvisor.lastAdviceOperationType == 0) {
      return '';
    } else {
      if (assetAdvisor.lastAdviceTargetPrice) {
        return 'Target value: ' +  new ValueDisplayPipe().transform(assetAdvisor.lastAdviceTargetPrice);
      } else if (assetAdvisor.lastAdviceOperationType != 0 && assetAdvisor.lastAdviceType == 2) {
        return 'Triggered by ' + Util.GetCloseReasonDescription(assetAdvisor.lastAdviceOperationType);
      } else {
        return '';
      }
    }
  }

  getSecondAdviceParametersDescription(assetAdvisor: AssetAdvisorResponse) {
    if (!assetAdvisor.lastAdviceStopLoss) {
      return '';
    } else {
      return 'Stop loss: ' +  new ValueDisplayPipe().transform(assetAdvisor.lastAdviceStopLoss);
    }
  }
  
  onFollowExpertClick(event: Event, expert: AdvisorResponse){
    if(this.accountService.hasInvestmentToCallLoggedAction()){
      this.promiseExpert = this.advisorService.followAdvisor(expert.userId).subscribe(result =>{
        expert.following = true;
        expert.numberOfFollowers = expert.numberOfFollowers + 1;
      });
    }
    event.stopPropagation();
  }

  onUnfollowExpertClick(event: Event, expert: AdvisorResponse){
    this.promiseExpert = this.advisorService.unfollowAdvisor(expert.userId).subscribe(result =>{
      expert.following = false;
      expert.numberOfFollowers = expert.numberOfFollowers - 1;
    });
    event.stopPropagation();
  }

  onRowClick(row){
    this.navigationService.goToExpertDetails(row.userId);
  }

  getAdvisor(userId : number) : AdvisorResponse{
    for(var i = 0; i < this.asset.assetAdvisor.length; i++){
      if(this.asset.advisors[i].userId == userId)
        return this.asset.advisors[i];
    }
    return null;
  }

  loadMoreAdvices(){
    this.currentPage++;
    this.setVisibleAdvices();
  }

  hasMoreAdvices(){
    return this.asset.assetAdvisor != null && this.visibleAdvices.length != this.asset.assetAdvisor.length;
  }

  setVisibleAdvices(){
    var numberToShow = this.pageSize * this.currentPage;
    this.visibleAdvices = this.asset.assetAdvisor.slice(0, numberToShow);
  }
}

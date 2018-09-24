import { Component, OnInit } from '@angular/core';
import { AssetResponse, AssetAdvisorResponse } from '../../../model/asset/assetResponse';
import { ActivatedRoute } from '@angular/router';
import { AssetService } from '../../../services/asset.service';
import { Util } from '../../../util/Util';
import { CONFIG} from "../../../services/config.service";
import { AccountService } from '../../../services/account.service';
import { ModalService } from '../../../services/modal.service';
import { AdvisorService } from '../../../services/advisor.service';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { Subscription } from 'rxjs';
import { NavigationService } from '../../../services/navigation.service';

@Component({
  selector: 'asset-details',
  templateUrl: './asset-details.component.html',
  styleUrls: ['./asset-details.component.css']
})
export class AssetDetailsComponent implements OnInit {
  displayedColumns: string[] = ['expertName', 'rankin', 'value', 'action', 'position', 'date', 'followButton'];
  asset: AssetResponse;
  showNewAdviceButton: boolean = false;
  visibleAdvices: AssetAdvisorResponse[];
  promise: Subscription;
  promiseExpert: Subscription;
  currentPage = 1;
  pageSize = 10;

  constructor(private route: ActivatedRoute, 
    private assetService: AssetService,
    private accountService: AccountService,
    private modalService: ModalService,
    private advisorService: AdvisorService,
    private navigationService:NavigationService) { }

  ngOnInit() {
    this.showNewAdviceButton = this.accountService.isLoggedIn() && this.accountService.getLoginData().isAdvisor;
    this.route.params.subscribe(params => 
      this.assetService.getAssetDetails(params['id']).subscribe(
        asset => {
          this.asset = asset;
          this.setVisibleAdvices();
        })
    )
  }

  onNewAdviceClick() {
    this.modalService.setNewAdvice(this.asset.assetId);
  }

  getAssetImgUrl(){
    return CONFIG.assetImgUrl.replace("{id}", this.asset.assetId.toString());
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
  
  onFollowExpertClick(event: Event, expert: AdvisorResponse){
    this.promiseExpert = this.advisorService.followAdvisor(expert.userId).subscribe(result =>expert.following = true);
    event.stopPropagation();
  }

  onUnfollowExpertClick(event: Event, expert: AdvisorResponse){
    this.promiseExpert = this.advisorService.unfollowAdvisor(expert.userId).subscribe(result =>expert.following = false);
    event.stopPropagation();
  }

  onFollowAssetClick(){
    this.promise = this.assetService.followAsset(this.asset.assetId).subscribe(result =>this.asset.following = true);
  }

  onUnfollowAssetClick(){
    this.promise = this.assetService.unfollowAsset(this.asset.assetId).subscribe(result =>this.asset.following = false);
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

  getTotalAdvisorsSentence(){
    var sentence = this.asset.totalAdvisors+" ";
    if(this.asset.totalAdvisors == 1){
      sentence += "expert recommend"
    }
    else{
      sentence += "experts recommend"
    }
    return sentence;
  }

  getFollowersSentence(){
    var sentence = this.asset.numberOfFollowers+" ";
    if(this.asset.numberOfFollowers == 1){
      sentence += "investor is following"
    }
    else{
      sentence += "investors are following"
    }
    return sentence;
  }

  getOperationDisclaimer() {
    let sentence = this.asset.totalAdvisors + " ";
    if(this.asset.totalAdvisors == 1) {
      sentence += "expert"
    } else {
      sentence += "experts"
    }
    return "Based on " + sentence + " offering recommendations for " + this.asset.code + " in the last month.";
  }
}

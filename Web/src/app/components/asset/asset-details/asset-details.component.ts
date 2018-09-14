import { Component, OnInit } from '@angular/core';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { AssetService } from '../../../services/asset.service';
import { Util } from '../../../util/Util';
import { CONFIG} from "../../../services/config.service";
import { AccountService } from '../../../services/account.service';
import { MatDialog } from '@angular/material';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { NewAdviceComponent } from '../../advisor/new-advice/new-advice.component';
import { FullscreenModalComponent } from '../../util/fullscreen-modal/fullscreen-modal.component';
import { AdvisorService } from '../../../services/advisor.service';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';

@Component({
  selector: 'asset-details',
  templateUrl: './asset-details.component.html',
  styleUrls: ['./asset-details.component.css']
})
export class AssetDetailsComponent implements OnInit {
  displayedColumns: string[] = ['expertName', 'rankin', 'value', 'action', 'position', 'date', 'followButton'];
  asset: AssetResponse;
  showNewAdviceButton: boolean = false;

  constructor(private route: ActivatedRoute, 
    private assetService: AssetService,
    public dialog: MatDialog, 
    public accountService: AccountService,
    private advisorService:AdvisorService) { }

  ngOnInit() {
    this.showNewAdviceButton = this.accountService.isLoggedIn() && this.accountService.getLoginData().isAdvisor;
    this.route.params.subscribe(params => 
      this.assetService.getAssetDetails(params['id']).subscribe(asset => this.asset = asset)
    )
  }

  onNewAdviceClick() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = NewAdviceComponent;
    modalData.componentInput = { assetId: this.asset.assetId };
    this.dialog.open(FullscreenModalComponent, { data: modalData }); 
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
    this.advisorService.followAdvisor(expert.userId).subscribe(result =>expert.following = true);
  }

  onUnfollowExpertClick(event: Event, expert: AdvisorResponse){
    this.advisorService.unfollowAdvisor(expert.userId).subscribe(result =>expert.following = false);
  }

  onFollowAssetClick(){
    this.assetService.followAsset(this.asset.assetId).subscribe(result =>this.asset.following = true);
  }

  onUnfollowAssetClick(){
    this.assetService.unfollowAsset(this.asset.assetId).subscribe(result =>this.asset.following = false);
  }

  getAdvisor(userId : number) : AdvisorResponse{
    for(var i = 0; i < this.asset.assetAdvisor.length; i++){
      if(this.asset.advisors[i].userId == userId)
        return this.asset.advisors[i];
    }
    return null;
  }
}

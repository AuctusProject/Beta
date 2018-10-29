import { Component, OnInit } from '@angular/core';
import { Title, Meta } from '@angular/platform-browser';
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
  asset: AssetResponse;
  promise: Subscription;

  constructor(private route: ActivatedRoute, 
    private assetService: AssetService,
    private accountService: AccountService,
    private modalService: ModalService,
    private advisorService: AdvisorService,
    private navigationService:NavigationService,
    private titleService: Title,
    private metaTagService: Meta) { }

  ngOnInit() {
    this.route.params.subscribe(params => 
      this.assetService.getAssetDetails(params['id']).subscribe(
        asset => {
          this.asset = asset;
          this.titleService.setTitle("Auctus Experts - " + asset.name);
          this.metaTagService.updateTag({name: 'description', content: "Expert ratings on " + asset.name + " (" + asset.code + ")" });
        })
    )
  }

  refreshDataSource(){
    this.assetService.getAssetDetails(this.asset.assetId.toString()).subscribe(
      asset => {
        this.asset = asset;
      })
  }
  
  getAssetImgUrl(){
    return CONFIG.assetImgUrl.replace("{id}", this.asset.assetId.toString());
  }

  onFollowAssetClick(){
    if(this.accountService.hasInvestmentToCallLoggedAction()){
      this.promise = this.assetService.followAsset(this.asset.assetId).subscribe(result =>
      {
        this.asset.following = true;
        this.asset.numberOfFollowers = this.asset.numberOfFollowers + 1;
      });
    }
  }

  onUnfollowAssetClick(){
    this.promise = this.assetService.unfollowAsset(this.asset.assetId).subscribe(result =>
      {
        this.asset.following = false;
        this.asset.numberOfFollowers = this.asset.numberOfFollowers - 1;
      });
  }

  getTotalAdvisorsSentence(){
    var sentence = this.asset.totalAdvisors+" ";
    if(this.asset.totalAdvisors == 1){
      sentence += "expert signaled"
    }
    else{
      sentence += "experts signaled"
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
}

import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { AdvisorService } from '../../../services/advisor.service';
import { AssetService } from '../../../services/asset.service';
import { Subscription } from 'rxjs';
import { FollowUnfollow } from '../../../model/asset/followUnfollow';

export enum FollowUnfollowType {
  asset=0,
  expert=1
}

@Component({
  selector: 'follow-unfollow',
  templateUrl: './follow-unfollow.component.html',
  styleUrls: ['./follow-unfollow.component.css']
})
export class FollowUnfollowComponent implements OnInit {
  promise:Subscription;
  @Input() following: boolean;
  @Input() numberOfFollowers?: number;
  @Input() type: FollowUnfollowType;
  @Input() id: number;
  @Input() showOnlyStar: boolean = false;
  @Output() onFollowUnfollow = new EventEmitter<FollowUnfollow>();

  constructor(private accountService:AccountService,
    private advisorService:AdvisorService,
    private assetService:AssetService) { }

  ngOnInit() {
  }

  onFollowClick(){
    if(this.accountService.hasInvestmentToCallLoggedAction()){
      var observable = null;
      if(this.type == FollowUnfollowType.asset){
        observable = this.assetService.followAsset(this.id);
      }
      else{
        observable = this.advisorService.followAdvisor(this.id);
      }
      this.promise = observable.subscribe(result =>
        this.onFollowUnfollowResponse(true)
      );
    }
  }

  onUnfollowClick(){
    var observable = null;
    if(this.type == FollowUnfollowType.asset){
      observable = this.assetService.unfollowAsset(this.id);
    }
    else{
      observable = this.advisorService.unfollowAdvisor(this.id);
    }
    this.promise = observable.subscribe(result =>
      this.onFollowUnfollowResponse(false)
    );
  }

  onFollowUnfollowResponse(isFollowing: boolean){
    this.following = isFollowing;
    if (isFollowing) {
      this.numberOfFollowers += 1;
    } else {
      this.numberOfFollowers -= 1;
    }
    let followUnfollow = new FollowUnfollow();
    followUnfollow.isFollowing = isFollowing;
    followUnfollow.assetId = this.id;
    this.onFollowUnfollow.emit(followUnfollow);
  }

  getFollowersDescription(){
    var text = this.numberOfFollowers + " follower";
    if(this.numberOfFollowers != 1)
      text += "s";
    return text;
  }

  onlyShowStar(){
    return this.showOnlyStar;
  }
}

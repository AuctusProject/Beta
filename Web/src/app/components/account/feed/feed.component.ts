import { Component, OnInit } from '@angular/core';
import { FeedResponse } from '../../../model/advisor/feedResponse';
import { AccountService } from '../../../services/account.service';
import { Subscription } from 'rxjs';
import { CONFIG } from '../../../services/config.service';

@Component({
  selector: 'feed',
  templateUrl: './feed.component.html',
  styleUrls: ['./feed.component.css']
})
export class FeedComponent implements OnInit {
  advices : FeedResponse[];
  hasMoreAdvices = false;
  pageSize = 10;
  promise : Subscription;

  dummyData = [
  { "assetId": 1, "assetName": "Bitcoin", "assetCode": "BTC", "assetMode": 1, "followingAsset": false,"date": new Date((new Date()).getTime() - 25*60000),  "advice": { "adviceId": 1, "advisorId": 1, "advisorName": "BTC Expert", "advisorUrlGuid": "f5f0bbb9-e57e-43ab-8a76-ce36e5916805", "advisorRanking": 1, "advisorRating": 4.5, "followingAdvisor": false, "adviceType": 1, "assetValueAtAdviceTime": 6501 },"report":null},
  { "assetId": 1, "assetName": "Bitcoin", "assetCode": "BTC", "assetMode": 1, "followingAsset": true, "date": new Date((new Date()).getTime() - 120*60000),"advice": {"adviceId": 2, "advisorId": 2, "advisorName": "BTC Holder", "advisorUrlGuid": "97108a6c-fa8f-4908-bc59-f581a0bd6233", "advisorRanking": 2, "advisorRating": 4.0, "followingAdvisor": false,  "adviceType": 2, "assetValueAtAdviceTime": 6203 },"report":null},
  { "assetId": 1, "assetName": "Bitcoin", "assetCode": "BTC", "assetMode": 1, "followingAsset": false,"date": new Date((new Date()).getTime() - 30000*60000), "advice": {"adviceId": 3, "advisorId": 3, "advisorName": "Furious Hater", "advisorUrlGuid": "402d8be1-7382-4a09-8024-742677bfb07e", "advisorRanking": 3, "advisorRating": 3.5, "followingAdvisor": true, "adviceType": 0, "assetValueAtAdviceTime": 6104.2 },"report":null},
  { "assetId": 2, "assetName": "Ethereum", "assetCode": "ETH", "assetMode": 3,"followingAsset": false,"date": new Date(), "advice": {"adviceId": 4,  "advisorId": 4, "advisorName": "ETH Expert", "advisorUrlGuid": "20753b51-e51c-4704-9704-ac775ff9eb46", "advisorRanking": 4, "advisorRating": 3.0, "followingAdvisor": false, "adviceType": 0, "assetValueAtAdviceTime": 310.5 },"report":null},
  { "assetId": 2, "assetName": "Ethereum", "assetCode": "ETH", "assetMode": 3,"followingAsset": true, "date": new Date((new Date()).getTime() - 50000*60000), "advice": {"adviceId": 5,  "advisorId": 5, "advisorName": "EOS Hater", "advisorUrlGuid": "4b8d8e42-df94-4469-bf59-076b12eb4979", "advisorRanking": 5, "advisorRating": 2.5, "followingAdvisor": false,  "adviceType": 1, "assetValueAtAdviceTime": 298.1 },"report":null}
];

  constructor(private accountService: AccountService) { }

  ngOnInit() {
    if (this.canView()) {
      this.loadMore();
    } else {
      this.advices = this.dummyData;
    }
  }

  canView() {
    let loginData = this.accountService.getLoginData();
    return !!loginData && loginData.hasInvestment;
  }

  loadMore() {
    this.promise = this.accountService.listFeed(this.pageSize, this.getLastAdviceId(), this.getLastReportId()).subscribe(result => 
      {
        if (this.advices == null)
          this.advices = [];
        this.advices = this.advices.concat(result);
        this.hasMoreAdvices = true;
        if(!result || result.length == 0 || result.length < this.pageSize){
          this.hasMoreAdvices = false;
        }
      }
    );
  }

  getLastAdviceId() {
    if(this.advices != null && this.advices.length > 0){
      for(var i = this.advices.length - 1; i > 0; i--){
        if(this.advices[i].advice) {
          return this.advices[i].advice.adviceId;
        }
      }
    }
    return null;
  }

  getLastReportId() {
    if(this.advices != null && this.advices.length > 0){
      for(var i = this.advices.length - 1; i > 0; i--){
        if(this.advices[i].report) {
          return this.advices[i].report.reportId;
        }
      }
    }
    return null;
  }

  getTopTitle() : string {
    return "HELLO";
  }

  getTopImage() : string {
    return CONFIG.platformImgUrl.replace("{id}", "feed1920px");
  }

  getTopText() : string {
    return "Recommendations from experts and cryptocurrencies that you are following";
  }
}
import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { AssetAdvisorResponse } from 'src/app/model/asset/assetResponse';
import { AssetService } from 'src/app/services/asset.service';
import { AssetRatingsResponse } from 'src/app/model/asset/assetRatingsResponse';
import { Util } from 'src/app/util/Util';
import { ValueDisplayPipe } from 'src/app/util/value-display.pipe';
import { NavigationService } from 'src/app/services/navigation.service';
import { AccountService } from 'src/app/services/account.service';
import { ModalService } from 'src/app/services/modal.service';

@Component({
  selector: 'ratings-list',
  templateUrl: './ratings-list.component.html',
  styleUrls: ['./ratings-list.component.css']
})
export class RatingsListComponent implements OnInit, OnChanges {
  @Input() assetId: number;
  displayedColumns: string[] = ['time','recommendation','target','stopLoss','expert','rating'];
  ratings: AssetRatingsResponse[];
  showRatingButton: boolean;
  constructor(private assetService: AssetService,
    private navigationService: NavigationService,
    private accountService: AccountService,
    private modalService: ModalService) { }

  ngOnInit() {
    let loginData = this.accountService.getLoginData();
    this.showRatingButton = !!loginData && loginData.isAdvisor;
  }  

  ngOnChanges() {
    this.ratings = null;
    this.loadRatings();
  }

  loadRatings(){
    this.assetService.getAssetRatings(this.assetId).subscribe(result => {
      this.ratings = result;
    });
  }

  getExpertDetailsUrl(element: AssetRatingsResponse){
    return 'expert-details/'+element.expertId;
  }

  getRecommendation(element: AssetRatingsResponse){
    return this.getRecommendationFromType(element.adviceType) + " @ " + new ValueDisplayPipe().transform(element.assetValue);
  }

  getRecommendationFromType(type: number){
    return Util.GetRecommendationTypeDescription(type);
  }
  
  getAdviceTypeColor(type: number){
    return Util.GetRecommendationTypeColor(type);
  }

  onNewAdviceClick() {
    this.modalService.setNewAdvice(this.assetId);
  }
}
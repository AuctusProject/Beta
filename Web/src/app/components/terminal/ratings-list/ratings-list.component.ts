import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { AssetAdvisorResponse } from 'src/app/model/asset/assetResponse';
import { AssetService } from 'src/app/services/asset.service';
import { AssetRatingsResponse } from 'src/app/model/asset/assetRatingsResponse';
import { Util } from 'src/app/util/Util';
import { ValueDisplayPipe } from 'src/app/util/value-display.pipe';
import { NavigationService } from 'src/app/services/navigation.service';

@Component({
  selector: 'ratings-list',
  templateUrl: './ratings-list.component.html',
  styleUrls: ['./ratings-list.component.css']
})
export class RatingsListComponent implements OnInit, OnChanges {
  @Input() assetId: number;
  displayedColumns: string[] = ['time','recommendation','target','stopLoss','expert','rating'];
  ratings: AssetRatingsResponse[];
  constructor(private assetService: AssetService,
    private navigationService: NavigationService) { }

  ngOnInit() {
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
}
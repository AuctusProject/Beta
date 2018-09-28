import { Component, OnInit, Input } from '@angular/core';
import { FeedResponse } from '../../../model/advisor/feedResponse';
import { NavigationService } from '../../../services/navigation.service';
import { CONFIG } from '../../../services/config.service';

@Component({
  selector: 'recommendation-box',
  templateUrl: './recommendation-box.component.html',
  styleUrls: ['./recommendation-box.component.css']
})
export class RecommendationBoxComponent implements OnInit {
  @Input() adviceList: FeedResponse[];
  @Input() adviceType: string;
  displayedColumns: string[] = ['assetCode', 'adviceType', 'advisorName'];
  
  constructor(private navigationService: NavigationService) { }

  ngOnInit() {
  }
  
  getAssetImgUrl(assetId: number) {
    return CONFIG.assetImgUrl.replace("{id}", assetId.toString());
  }
  goToExpertDetails(expertId: number){
    this.navigationService.goToExpertDetails(expertId);
  }

  goToAssetDetails(assetId: number){
    this.navigationService.goToAssetDetails(assetId);
  }

}

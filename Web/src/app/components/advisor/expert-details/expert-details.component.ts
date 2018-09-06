import { Component, OnInit } from '@angular/core';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { AdvisorService } from '../../../services/advisor.service';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { CONFIG } from "../../../services/config.service";
import { Util } from '../../../util/Util';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { DataSource } from '@angular/cdk/table';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'expert-details',
  templateUrl: './expert-details.component.html',
  styleUrls: ['./expert-details.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0', visibility: 'hidden' })),
      state('expanded', style({ height: '*', visibility: 'visible' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})
export class ExpertDetailsComponent implements OnInit {
  expert: AdvisorResponse;
  displayedColumns: string[] = ['assetName', 'position', 'action', 'date', 'ratings', 'followButton'];
  assets = [];
  isExpansionDetailRow = (i: number, row: Object) => row.hasOwnProperty('detailRow');
  expandedElement: any;

  constructor(private route: ActivatedRoute, private advisorService: AdvisorService) { }

  ngOnInit() {
    this.route.params.subscribe(params => 
      this.advisorService.getExpertDetails(params['id']).subscribe(expert => 
        {
          this.expert = expert;
          this.fillDataSource();
        })
    );
  }

  onRowClick(row){
    if(this.expandedElement == row){
      this.expandedElement = null;
    }
    else{
      this.expandedElement = row;
    }
  }

  fillDataSource(){
    this.expert.assets.forEach(element => this.assets.push(element, { detailRow: true, element }));
  }

  getAssetImgUrl(asset: AssetResponse){
    return CONFIG.assetImgUrl.replace("{id}", asset.assetId.toString());
  }

  getLastAdviceTypeDescription(asset: AssetResponse){
    return Util.GetRecommendationTypeDescription(asset.assetAdvisor[0].lastAdviceType);
  }

  getLastAdviceDate(asset: AssetResponse){
    return asset.assetAdvisor[0].lastAdviceDate;
  }

  getTotalRatings(asset:AssetResponse){
    return asset.assetAdvisor[0].totalRatings;
  }

  getAdviceMode(asset:AssetResponse){
    return Util.GetAdviceModeDescription(asset.assetAdvisor[0].lastAdviceMode);
  }
}
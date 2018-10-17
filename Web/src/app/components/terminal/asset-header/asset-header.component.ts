import { Component, OnInit, OnDestroy, ChangeDetectorRef, Input, OnChanges } from '@angular/core';
import { TerminalAssetResponse } from 'src/app/model/asset/terminalAssetResponse';
import { AssetResponse } from 'src/app/model/asset/assetResponse';
import { AssetService } from 'src/app/services/asset.service';
import { AssetStatusResponse } from 'src/app/model/asset/assetStatusResponse';
import { CONFIG } from 'src/app/services/config.service';
import { Util } from 'src/app/util/Util';
import { RecommendationDistributionResponse } from 'src/app/model/recommendationDistributionResponse';

@Component({
  selector: 'asset-header',
  templateUrl: './asset-header.component.html',
  styleUrls: ['./asset-header.component.css']
})
export class AssetHeaderComponent implements OnDestroy, OnChanges {
  @Input() assetTerminal: TerminalAssetResponse;
  assetData: AssetResponse;
  assetStatus: AssetStatusResponse;
  timer: any;

  constructor(private assetService: AssetService) { }

  ngOnChanges() {
    this.setNewAsset();
  }

  ngOnDestroy() {
    if (this.timer) {
      clearTimeout(this.timer);
    }
  }

  getAssetImgUrl() {
    return CONFIG.assetImgUrl.replace("{id}", this.assetTerminal.assetId.toString());
  }

  getAssetGeneralRecommendation() {
    return Util.GetGeneralRecommendationDescription(this.assetData.mode);
  }

  openReportPage() {
    if (window) {
      let url = 'rating-reports';
      if (this.assetTerminal) {
        url += "?coin=" + this.assetTerminal.assetId;
      }
      window.open(url);
    }
  }

  getTotalExpertRecommendation() {
    return this.assetData ? this.getTotalDistributionAmount(this.assetData.recommendationDistribution) : 0;
  }

  getTotalReportRatings() {
    return this.assetData ? this.getTotalDistributionAmount(this.assetData.reportRecommendationDistribution) : 0;
  }

  getTotalDistributionAmount(list: RecommendationDistributionResponse[]) : number {
    let total = 0;
    if (list) {
      for(let i = 0; i < list.length; ++i) {
        total += list[i].total;
      }
    }
    return total;
  }

  getExpertsBuyAmount() {
    return this.assetData ? this.getDistributionAmount(this.assetData.recommendationDistribution, 1) : 0;
  }

  getExpertsSellAmount() {
    return this.assetData ? this.getDistributionAmount(this.assetData.recommendationDistribution, 0) : 0;
  }

  getExpertsNeutralAmount() {
    return this.assetData ? this.getDistributionAmount(this.assetData.recommendationDistribution, 2) : 0;
  }

  getReportsBuyAmount() {
    return this.assetData ? this.getDistributionAmount(this.assetData.reportRecommendationDistribution, 1) : 0;
  }

  getReportsSellAmount() {
    return this.assetData ? this.getDistributionAmount(this.assetData.reportRecommendationDistribution, 0) : 0;
  }

  getReportsNeutralAmount() {
    return this.assetData ? this.getDistributionAmount(this.assetData.reportRecommendationDistribution, 2) : 0;
  }

  private getDistributionAmount(list: RecommendationDistributionResponse[], type: number) : number {
    let amount = 0;
    if (list) {
      for(let i = 0; i < list.length; ++i) {
        if (list[i].type == type) {
          amount = list[i].total;
          break;
        }
      }
    }
    return amount;
  }

  setNewAsset() {
    if (this.assetTerminal) {
      this.assetService.getAssetBaseData(this.assetTerminal.assetId).subscribe(result =>
        {
          this.assetData = result;
        });
      this.setStatusUpdate();
    }
  }

  setStatusUpdate() {
    if (this.timer) {
      clearTimeout(this.timer);
    }
    let self = this;
    this.assetService.getAssetStatus(this.assetTerminal.assetId).subscribe(result => 
      {
        self.assetStatus = result;
        self.timer = setTimeout(() => self.setStatusUpdate(), 40000);
      });
  }
}

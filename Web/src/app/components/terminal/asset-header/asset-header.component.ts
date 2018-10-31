import { Component, OnInit, OnDestroy, ChangeDetectorRef, Input, OnChanges } from '@angular/core';
import { TerminalAssetResponse } from '../../../model/asset/terminalAssetResponse';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { AssetService } from '../../../services/asset.service';
import { AssetStatusResponse } from '../../../model/asset/assetStatusResponse';
import { CONFIG } from '../../../services/config.service';
import { Util } from '../../../util/Util';
import { RecommendationDistributionResponse } from '../../../model/recommendationDistributionResponse';

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
  expanded: boolean = true;

  constructor(private assetService: AssetService) { }

  ngOnChanges() {
    this.assetData = null;
    this.assetStatus = null;
    this.ngOnDestroy();
    this.setNewAsset();
  }

  ngOnDestroy() {
    if (this.timer) {
      clearTimeout(this.timer);
    }
  }

  isMobile() {
    return window.screen.width < 600;
  }

  expand() {
    this.expanded = true;
  }

  collapse() {
    this.expanded = false;
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
        url += "?asset=" + this.assetTerminal.assetId;
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
    this.expanded = !this.isMobile();
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

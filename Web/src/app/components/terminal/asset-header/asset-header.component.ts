import { Component, OnInit, OnDestroy, ChangeDetectorRef, Input, OnChanges } from '@angular/core';
import { TerminalAssetResponse } from 'src/app/model/asset/terminalAssetResponse';
import { AssetResponse } from 'src/app/model/asset/assetResponse';
import { AssetService } from 'src/app/services/asset.service';
import { AssetStatusResponse } from 'src/app/model/asset/assetStatusResponse';
import { CONFIG } from 'src/app/services/config.service';
import { Util } from 'src/app/util/Util';

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

  getTotalExpertRecommendation() {
    let total = 0;
    if (this.assetData.recommendationDistribution) {
      for(let i = 0; i < this.assetData.recommendationDistribution.length; ++i) {
        total += this.assetData.recommendationDistribution[i].total;
      }
    }
    return total;
  }

  getTotalReportRatings() {
    let total = 0;
    if (this.assetData.reportRecommendationDistribution) {
      for(let i = 0; i < this.assetData.reportRecommendationDistribution.length; ++i) {
        total += this.assetData.reportRecommendationDistribution[i].total;
      }
    }
    return total;
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

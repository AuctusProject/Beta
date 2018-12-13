import { Component, OnInit, Input } from '@angular/core';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { CONFIG } from "../../../services/config.service";

@Component({
  selector: 'asset-summary',
  templateUrl: './asset-summary.component.html',
  styleUrls: ['./asset-summary.component.css']
})
export class AssetSummaryComponent implements OnInit {
  @Input() assetId: number;
  @Input() assetName: string;
  @Input() assetCode: string;
  @Input() transferToDetailsPage : boolean;

  constructor() { }

  ngOnInit() {
  }

  getAssetImgUrl(id: number) {
    return CONFIG.assetImgUrl.replace("{id}", id.toString());
  }
}

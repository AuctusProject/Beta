import { Component, OnInit } from '@angular/core';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { AssetService } from '../../../services/asset.service';
import { Util } from '../../../util/Util';
import { CONFIG} from "../../../services/config.service";

@Component({
  selector: 'asset-details',
  templateUrl: './asset-details.component.html',
  styleUrls: ['./asset-details.component.css']
})
export class AssetDetailsComponent implements OnInit {
  asset: AssetResponse;
  constructor(private route: ActivatedRoute, private assetService: AssetService) { }

  ngOnInit() {
    this.route.params.subscribe(params => 
      this.assetService.getAssetDetails(params['id']).subscribe(asset => this.asset = asset)
    )
  }

  getAssetImgUrl(asset: AssetResponse){
    return CONFIG.assetImgUrl.replace("{id}", asset.assetId.toString());
  }

  getRecommendationFromType(type: number){
    return Util.GetRecommendationTypeDescription(type);
  }
  
}

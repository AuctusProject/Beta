import { Component, OnInit } from '@angular/core';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { AdvisorService } from '../../../services/advisor.service';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { CONFIG } from "../../../services/config.service";

@Component({
  selector: 'expert-details',
  templateUrl: './expert-details.component.html',
  styleUrls: ['./expert-details.component.css']
})
export class ExpertDetailsComponent implements OnInit {
  advisor: AdvisorResponse;
  constructor(private route: ActivatedRoute, private advisorService: AdvisorService) { }

  ngOnInit() {
    this.route.params.subscribe(params => 
      this.advisorService.getExpertDetails(params['id']).subscribe(advisor => this.advisor = advisor)
    )
  }
  getAssetImgUrl(asset: AssetResponse){
    return CONFIG.assetImgUrl.replace("{id}", asset.assetId.toString());
  }
}

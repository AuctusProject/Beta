import { Component, OnInit } from '@angular/core';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { AdvisorService } from '../../../services/advisor.service';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { CONFIG } from "../../../services/config.service";

@Component({
  selector: 'advisor-details',
  templateUrl: './advisor-details.component.html',
  styleUrls: ['./advisor-details.component.css']
})
export class AdvisorDetailsComponent implements OnInit {
  advisor: AdvisorResponse;
  constructor(private route: ActivatedRoute, private advisorService: AdvisorService) { }

  ngOnInit() {
    this.route.params.subscribe(params => 
      this.advisorService.getAdvisorDetails(params['id']).subscribe(advisor => this.advisor = advisor)
    )
  }
  getAssetImgUrl(asset: AssetResponse){
    return CONFIG.assetImgUrl.replace("{id}", asset.assetId.toString());
  }
}

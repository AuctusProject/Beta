import { Component, OnInit, Input } from '@angular/core';
import { FeedResponse } from '../../../../model/advisor/feedResponse';
import { CONFIG } from '../../../../services/config.service';
import { NavigationService } from '../../../../services/navigation.service';

@Component({
  selector: 'report-card',
  templateUrl: './report-card.component.html',
  styleUrls: ['./report-card.component.css']
})
export class ReportCardComponent implements OnInit {
  @Input() reportFeed : FeedResponse;
  constructor(private navigationService : NavigationService) { }

  ngOnInit() {
  }

  
  getReportUrl(){
    return CONFIG.reportUrl.replace("{id}", this.reportFeed.report.reportId.toString());
  }

  getReportAgencyImgUrl(){
    return CONFIG.agencyImgUrl.replace("{id}", this.reportFeed.report.agencyId.toString());
  }

  getAssetImgUrl(){
    return CONFIG.assetImgUrl.replace("{id}", this.reportFeed.assetId.toString());
  }

  goToAssetDetails(){
    this.navigationService.goToAssetDetails(this.reportFeed.assetId);
  }
}

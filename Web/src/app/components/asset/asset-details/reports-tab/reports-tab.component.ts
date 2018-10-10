import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { ReportResponse } from '../../../../model/asset/reportResponse';
import { Subscription } from 'rxjs';
import { CONFIG } from '../../../../services/config.service';

@Component({
  selector: 'reports-tab',
  templateUrl: './reports-tab.component.html',
  styleUrls: ['./reports-tab.component.css']
})
export class ReportsTabComponent implements OnInit, OnChanges {
  displayedColumns: string[] = ['agencyName', 'score', 'rating',  'date', 'download' ];
  @Input() reports: ReportResponse[] = [];
  visibleReports: ReportResponse[] = [];
  promise: Subscription;
  currentPage = 1;
  pageSize = 10;

  constructor() { }

  ngOnInit(){

  }
  
  ngOnChanges() {
    this.setVisibleReports();
  }

  loadMoreReports(){
    this.currentPage++;
    this.setVisibleReports();
  }

  hasMoreReports(){
    return this.reports != null && this.visibleReports.length != this.reports.length;
  }

  setVisibleReports(){
    var numberToShow = this.pageSize * this.currentPage;
    this.visibleReports = this.reports.slice(0, numberToShow);
  }

  onRowClick(row){
    
  }

  getReportUrl(report: ReportResponse){
    return CONFIG.reportUrl.replace("{id}", report.reportId.toString());
  }

  getReportAgencyImgUrl(report: ReportResponse){
    return CONFIG.agencyImgUrl.replace("{id}", report.agencyId.toString());
  }

  getBackgroundColor(report: ReportResponse){
    if(report.rateDetails){
      return report.rateDetails.hexaColor;
    }
    return null;
  }
}

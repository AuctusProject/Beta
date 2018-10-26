import { Component, OnInit, ViewChild } from '@angular/core';
import { Title, Meta } from '@angular/platform-browser';
import { AssetService } from '../../../services/asset.service';
import { AccountService } from '../../../services/account.service';
import { ModalService } from '../../../services/modal.service';
import { CoinSearchComponent } from '../../util/coin-search/coin-search.component';
import { Subscription } from 'rxjs';
import { FeedResponse } from '../../../model/advisor/feedResponse';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'list-reports',
  templateUrl: './list-reports.component.html',
  styleUrls: ['./list-reports.component.css']
})
export class ListReportsComponent implements OnInit {
  @ViewChild("CoinSearch") CoinSearch: CoinSearchComponent;
  showNewAdviceButton: boolean = false;
  reports: FeedResponse[] = [];
  hasMoreReports: boolean = false;
  firstLoaded: boolean = false;
  pageSize: number = 10;
  promise: Subscription;
  selectedReportId?: number = null;

  constructor(private modalService: ModalService, 
    public accountService: AccountService, 
    private assetService: AssetService,
    private route: ActivatedRoute,
    private titleService: Title,
    private metaTagService: Meta) { }

  ngOnInit() {
    this.titleService.setTitle("Auctus Experts - Agency Rating Reports");
    this.metaTagService.updateTag({name: 'description', content: "All crypto reports in one place, easily accessible and ready for download"});
    if (this.route.snapshot.queryParams['coin']) { 
      this.selectedReportId = parseInt(this.route.snapshot.queryParams['coin']);
      this.CoinSearch.setForcedCoin(this.selectedReportId);
    }
    this.showNewAdviceButton = this.accountService.isLoggedIn() && this.accountService.getLoginData().isAdvisor;
    this.loadMoreReports();

    this.CoinSearch.onSelect.subscribe(newValue => 
      {
        if ((!newValue && this.selectedReportId) || (newValue && newValue.id != this.selectedReportId))
        {
          if (newValue) {
            this.selectedReportId = newValue.id;
          } else {
            this.selectedReportId = null;
          }
          this.loadMoreReports(true);
        }
      });
  }

  loadMoreReports(clear?: boolean) {
    let lastReportId = null; 
    if (!clear) {
      lastReportId = this.getLastReportId();
    }
    this.promise = this.assetService.getAssetsReports(this.pageSize, lastReportId, this.selectedReportId).subscribe(result => 
      {
        if (clear) {
          this.reports = result;
        } else {
          this.reports = this.reports.concat(result);
        }  
        this.hasMoreReports = true;
        if(!result || result.length == 0 || result.length < this.pageSize){
          this.hasMoreReports = false;
        }
        this.firstLoaded = true;
      });
  }

  getLastReportId() {
    if(!!this.reports && this.reports.length > 0) {
      return this.reports[this.reports.length - 1].report.reportId;
    } else {
      return null;
    }
  }

  onNewAdviceClick() {
    this.modalService.setNewAdvice();
  }

  getSearchOptions() {
    return { required: false, outlineField: true, darkStyle: true };
  }
}

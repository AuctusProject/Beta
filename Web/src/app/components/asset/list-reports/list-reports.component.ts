import { Component, OnInit, ViewChild } from '@angular/core';
import { AssetService } from '../../../services/asset.service';
import { AccountService } from '../../../services/account.service';
import { ModalService } from '../../../services/modal.service';
import { CoinSearchComponent } from '../../util/coin-search/coin-search.component';
import { NavigationService } from '../../../services/navigation.service';
import { ReportResponse } from '../../../model/asset/reportResponse';
import { Subscription } from 'rxjs';

@Component({
  selector: 'list-reports',
  templateUrl: './list-reports.component.html',
  styleUrls: ['./list-reports.component.css']
})
export class ListReportsComponent implements OnInit {
  @ViewChild("CoinSearch") CoinSearch: CoinSearchComponent;
  showNewAdviceButton: boolean = false;
  reports: ReportResponse[] = [];
  hasMoreReports = false;
  pageSize = 6;
  promise : Subscription;

  constructor(private modalService: ModalService, 
    public accountService: AccountService, 
    private assetService: AssetService,
    private navigationService: NavigationService) { }

  ngOnInit() {
    this.showNewAdviceButton = this.accountService.isLoggedIn() && this.accountService.getLoginData().isAdvisor;
    this.loadMoreReports();

    this.CoinSearch.onSelect.subscribe(newValue => 
      {
          if (newValue) {
              this.navigationService.goToAssetDetails(newValue.id);
          }
      });
  }

  loadMoreReports() {
    this.promise = this.assetService.getAssetsReports(this.pageSize, this.getLastReportId()).subscribe(result => 
      {
        this.reports = this.reports.concat(result);
        this.hasMoreReports = true;
        if(!result || result.length == 0 || result.length < this.pageSize){
          this.hasMoreReports = false;
        }
      });
  }

  getLastReportId() {
    if(!!this.reports && this.reports.length > 0) {
      return this.reports[this.reports.length - 1].reportId;
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

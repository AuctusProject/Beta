import { Component, OnInit, ViewChild } from '@angular/core';
import { AssetService } from '../../../services/asset.service';
import { AccountService } from '../../../services/account.service';
import { ModalService } from '../../../services/modal.service';
import { CoinSearchComponent } from '../../util/coin-search/coin-search.component';
import { Subscription } from 'rxjs';
import { FeedResponse } from '../../../model/advisor/feedResponse';

@Component({
  selector: 'list-events',
  templateUrl: './list-events.component.html',
  styleUrls: ['./list-events.component.css']
})
export class ListEventsComponent implements OnInit {
  @ViewChild("CoinSearch") CoinSearch: CoinSearchComponent;
  showNewAdviceButton: boolean = false;
  events: FeedResponse[] = [];
  hasMoreEvents = false;
  pageSize = 10;
  promise : Subscription;
  selectedAssetId?: number = null;

  constructor(private modalService: ModalService, 
    public accountService: AccountService, 
    private assetService: AssetService) { }

  ngOnInit() {
    this.showNewAdviceButton = this.accountService.isLoggedIn() && this.accountService.getLoginData().isAdvisor;
    this.loadMoreEvents();

    this.CoinSearch.onSelect.subscribe(newValue => 
      {
          if (newValue) {
            this.selectedAssetId = newValue.id;
          } else {
            this.selectedAssetId = null;
          }
          this.loadMoreEvents(true);
      });
  }

  loadMoreEvents(clear?: boolean) {
    this.promise = this.assetService.getAssetsEvents(this.pageSize, this.getLastEventId(), this.selectedAssetId).subscribe(result => 
      {
        if (clear) {
          this.events = result;
        } else {
          this.events = this.events.concat(result);
        }   
        this.hasMoreEvents = true;
        if(!result || result.length == 0 || result.length < this.pageSize) {
          this.hasMoreEvents = false;
        }
      });
  }

  getLastEventId() {
    if(!!this.events && this.events.length > 0) {
      return this.events[this.events.length - 1].event.eventId;
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
